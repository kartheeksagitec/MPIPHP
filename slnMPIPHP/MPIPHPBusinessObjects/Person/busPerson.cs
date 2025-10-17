#region Using directives

using System;
using System.Collections;
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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using System.Runtime.Remoting.Contexts;
using Sagitec.CustomDataObjects;
using System.Transactions;
using Microsoft.Reporting.WinForms;
using NeoSpin.BusinessObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busPerson:
    /// Inherited from busPersonGen, the class is used to customize the business object busPersonGen.
    /// </summary>
    [Serializable]
    public class busPerson : busPersonGen
    {
        #region Properties
        public string iblnParticipant { get; set; }

        public int iainthealthEligibleFlag { get; set; }
        public string iblnBeneficiary { get; set; }
        public string iblnAlternatePayee { get; set; }
        public string iblnDependent { get; set; }
        public string istrRetiree { get; set; }
        public int iintSelectedParticipantId { get; set; }
        public decimal idecAge { get; set; }

        public int iintAnalystId { get; set; }

        public string istrSixtyDaysPriorDate { get; set; }

        public string istrAddressSameAsParticipant { get; set; }
        public string istrSpousalFirstName { get; set; }
        public string istrSpousalLastName { get; set; }
        public string istrSpousalMiddleName { get; set; }
        public string istrSpousalPrefix { get; set; }
        public string istrSpousalSSN { get; set; }
        public string istrFullName { get; set; }
        //Rquired For Inbound File
        public string istrGroup { get; set; }
        public string istrHitType { get; set; }

        public int iintYear { get; set; }
        public string istrMinDistriDate { get; set; }
        public string istrFebDate { get; set; }
        public string istrBenefitOption { get; set; }
        public string istrMarchDt { get; set; } //For PER-0006(RASHMI)

       public string istrCurrentDate { get; set; }
        public string istrRetirementDate { get; set; }
        public string istrPersonAddress { get; set; }

        public string analystname { get;set; }

        public string istrNewMergedMPIID { get; set; }
        public int iintCurrentYear { get; set; }
        public string istrBenefitTypeDescription { get; set; }

        
        public Collection<busBenefitApplication> iclbBenefitApplication { get; set; }

        public Collection<busPersonDependent> iclbPersonDependent { get; set; }
        public Collection<busPersonAccount> iclbPersonAccount { get; set; }
        public Collection<busPersonBridgeHours> iclbPersonBridgeHours { get; set; }
        public Collection<busPersonAccountRetirementContribution> iclbPersonAccountRetirementContribution { get; set; }
        public Collection<busPersonAccountRetirementContribution> iclbEEContributionInterest { get; set; }
        public Collection<busPersonAccountRetirementContribution> iclbTotalEEContributionInterest { get; set; }
        public Collection<busPersonAccountRetirementContribution> iclbEEContributionInterestOPUS { get; set; }
        public Collection<busPersonAccountRetirementContribution> iclbTotalEEContributionInterestOPUS { get; set; }
        public Collection<busPersonSuspendibleMonth> iclbPersonSuspendibleMonth { get; set; }
        public Collection<busPersonAccountBeneficiary> iclbPersonAccountBeneficiaryOf { get; set; }
        //Ticket - 68547 
        public Collection<busPerson> iclbDependentOf { get; set; }

        public busBenefitApplication ibusBenefitApplication { get; set; }

        public busPersonOverview ibusPersonOverview { get; set; }

        public busPersonAddress ibusPersonAddressForCorr { get; set; }

        busPersonAddress ibusParticipantsActiveAddress { get; set; }
        int iintParticipantsAddrressId { get; set; }

        public busPerson ibusBeneficiaryPerson { get; set; }//Will load Address directly in this one.

        public busSystemManagement iobjSystemManagement { get; set; }

        public busPersonContact ibusPersonCourtContact { get; set; }
        public busPersonAddress ibusCourtAddressForCorr { get; set; }   // address required for participant for DRO-0008
        public busPersonAddress ibusAlternatePayeeAddressForCorr { get; set; }    // address required for alternatePayee for DRO-0008
        public busPersonContact ibusPersonContactForCor { get; set; }

        public busDocumentProcessCrossref ibusDocumentProcessCrossref { get; set; }
        public Collection<busNotes> iclbNotes { get; set; }
        public busRetirementApplication lbusRetirementApplication { get; set; }
        public ArrayList larrPersonBridgeHoursId = new ArrayList();
        private static string LegacyDBConnetion { get; set; }
        bool blnCheckActiveAddress = false;

        public string istrTransactionDate { get; set; }
        public decimal idecSumEEUVHPAmount { get; set; }
        public decimal idecEEContributionAmount { get; set; }
        public decimal idecEEContributionInterest { get; set; }
        public decimal idecUVHPContributionAmount { get; set; }
        public decimal idecUVHPContributionInterest { get; set; }

        public decimal idecIAPContributionAmount { get; set; }
        public decimal idecLocal52SpecialAcctBalanceAmt { get; set; }
        public decimal idecLocal161SpecialAcctBalanceAmt { get; set; }
        public busDataExtractionBatchInfo lbusDataExtractionBatchInfo { get; set; }

        public string istrOldSSN { get; set; }
        private object iobjLock = null;

        [NonSerialized()]
        private utlPassInfo _lobjLegacyPassInfoHEDB;
        [NonSerialized()]
        private utlPassInfo _lobjLegacy;

        public utlPassInfo lobjLegacyPassInfoHEDB { get { return _lobjLegacyPassInfoHEDB; } set { _lobjLegacyPassInfoHEDB = value; } }
        public utlPassInfo lobjLegacy { get { return _lobjLegacy; } set { _lobjLegacy = value; } }


        public string IsFromMPIIDChecked { get; set; }
        public string IsFromFirstNameChecked { get; set; }
        public string IsFromMiddleNameChecked { get; set; }
        public string IsFromLastNameChecked { get; set; }
        public string IsFromPrefixNameChecked { get; set; }
        public string IsFromSuffixNameChecked { get; set; }
        public string IsFromDOBChecked { get; set; }
        public string IsFromDODChecked { get; set; }
        public string IsFromAddressChecked { get; set; }
        public string IsFromSSNChecked { get; set; }


        public string IsToMPIIDChecked { get; set; }
        public string IsToFirstNameChecked { get; set; }
        public string IsToMiddleNameChecked { get; set; }
        public string IsToLastNameChecked { get; set; }
        public string IsToPrefixNameChecked { get; set; }
        public string IsToSuffixNameChecked { get; set; }
        public string IsToDOBChecked { get; set; }
        public string IsToDODChecked { get; set; }
        public string IsToAddressChecked { get; set; }
        public string IsToSSNChecked { get; set; }

        public Collection<cdoDummyWorkData> iclbPersonWorkHistory { get; set; }

        public int ldecHealthQualifiedYears { get; set; }

        public decimal ldecHealthQualifiedHours { get; set; }
        public string istrIsVested { get; set; }

        //PER-0011
        public string istrIsPOA { get; set; }
        public string istrIsConservator { get; set; }
        public string istrIsGuardian { get; set; }

        //PER-0015
        public busAnnualBenefitSummaryOverview ibusAnnualBenefitSummaryOverview { get; set; }
        public string istrIsVestedMPI { get; set; }
        public int iintVestedYearsMinus5 { get; set; }
        public DateTime idtPeriodEndingDate { get; set; }
        public int iintYearEnd { get; set; }
        public decimal idecIAPBalance { get; set; }

        //HS23
        public decimal idecTotalIAPBalance { get; set; }

        public bool iblnMssCheckForActiveAddress { get; set; }
        public string istrRetiredInCurrentYear { get; set; }

        //PIR-830
        public bool iblnIsManagerChangedBirthdateIfBenefitAppApproved { get; set; }
        public bool iblnManagerChangedBirthdateIfPayeeActApproved { get; set; }

        public Collection<busCorTracking> iclbCorTracking { get; set; }

        public Collection<busProcessActivityLog> iclbProcessActivityLog { get; set; }
        public string registered_email_address { get; set; }
        public string is_paper_statement { get; set; }
        public bool iblnEStatement { get; set; }
        public int iPerson_communication_preferences_id { get; set; }
        public Collection<busPersonCommunicationPreferences> iclbPersonCommunicationPreferences { get; set; }

        #endregion

        public void LoadPersonCommPref()
        {
            this.iclbPersonCommunicationPreferences = new Collection<busPersonCommunicationPreferences>();
            DataTable ldtblist = busPerson.Select("cdoPerson.GetIsPaperStatementFlagHistory", new object[1] { this.icdoPerson.person_id });
            this.iclbPersonCommunicationPreferences = GetCollection<busPersonCommunicationPreferences>(ldtblist, "icdoPersonCommunicationPreferences");
            if (this.iclbPersonCommunicationPreferences != null)
            this.iclbPersonCommunicationPreferences = this.iclbPersonCommunicationPreferences.OrderByDescending(obj => obj.icdoPersonCommunicationPreferences.modified_date).ToList().ToCollection<busPersonCommunicationPreferences>();
            ldtblist.Dispose();

            DataTable ldtbParticipantCommunicationPreference = busBase.Select("cdoPerson.GetCommunicationPreferences", new object[1] { this.icdoPerson.person_id });
            if (ldtbParticipantCommunicationPreference != null && ldtbParticipantCommunicationPreference.Rows.Count > 0)
            {
                this.iPerson_communication_preferences_id = Convert.ToInt32(ldtbParticipantCommunicationPreference.Rows[0]["PERSON_COMMUNICATION_PREFERENCES_ID"]);
                this.registered_email_address = ldtbParticipantCommunicationPreference.Rows[0]["REGISTERED_EMAIL_ADDRESS"].ToString();
                this.is_paper_statement = ldtbParticipantCommunicationPreference.Rows[0]["IS_PAPER_STATEMENT"].ToString();
                if (ldtbParticipantCommunicationPreference.Rows[0]["IS_PAPER_STATEMENT"].ToString() == "Y")
                {
                    iblnEStatement = true;
                }
            }
            ldtbParticipantCommunicationPreference.Dispose();
        }

        public ArrayList SavePersonCommPref()
        {
            ArrayList larrResult = new ArrayList();
            string lstrFormName = string.Empty;

            busPersonCommunicationPreferences lbusPersonCommunicationPreferences = new busPersonCommunicationPreferences { icdoPersonCommunicationPreferences = new doPersonCommunicationPreferences() };
            lbusPersonCommunicationPreferences.FindPersonCommunicationPreferences(this.iPerson_communication_preferences_id);
            if(lbusPersonCommunicationPreferences.icdoPersonCommunicationPreferences.person_communication_preferences_id > 0)
            {
                busPersonCommunicationPreferencesHist lbusPersonCommunicationPreferencesHist = new busPersonCommunicationPreferencesHist { icdoPersonCommunicationPreferencesHist = new doPersonCommunicationPreferencesHist() };
                lbusPersonCommunicationPreferencesHist.icdoPersonCommunicationPreferencesHist.person_communication_preferences_id = lbusPersonCommunicationPreferences.icdoPersonCommunicationPreferences.person_communication_preferences_id;
                lbusPersonCommunicationPreferencesHist.icdoPersonCommunicationPreferencesHist.person_id = lbusPersonCommunicationPreferences.icdoPersonCommunicationPreferences.person_id;
                lbusPersonCommunicationPreferencesHist.icdoPersonCommunicationPreferencesHist.registered_email_address = lbusPersonCommunicationPreferences.icdoPersonCommunicationPreferences.registered_email_address;
                lbusPersonCommunicationPreferencesHist.icdoPersonCommunicationPreferencesHist.is_paper_statement = lbusPersonCommunicationPreferences.icdoPersonCommunicationPreferences.is_paper_statement;
                lbusPersonCommunicationPreferencesHist.icdoPersonCommunicationPreferencesHist.created_by = lbusPersonCommunicationPreferences.icdoPersonCommunicationPreferences.created_by;
                lbusPersonCommunicationPreferencesHist.icdoPersonCommunicationPreferencesHist.created_date = lbusPersonCommunicationPreferences.icdoPersonCommunicationPreferences.created_date;
                lbusPersonCommunicationPreferencesHist.icdoPersonCommunicationPreferencesHist.modified_by = lbusPersonCommunicationPreferences.icdoPersonCommunicationPreferences.modified_by;
                lbusPersonCommunicationPreferencesHist.icdoPersonCommunicationPreferencesHist.modified_date = lbusPersonCommunicationPreferences.icdoPersonCommunicationPreferences.modified_date;
                lbusPersonCommunicationPreferencesHist.icdoPersonCommunicationPreferencesHist.log_by = iobjPassInfo.istrUserID;
                lbusPersonCommunicationPreferencesHist.icdoPersonCommunicationPreferencesHist.log_date = DateTime.Now;
                lbusPersonCommunicationPreferencesHist.icdoPersonCommunicationPreferencesHist.is_paper_spd = lbusPersonCommunicationPreferences.icdoPersonCommunicationPreferences.is_paper_spd;
                lbusPersonCommunicationPreferencesHist.icdoPersonCommunicationPreferencesHist.Insert();

                lbusPersonCommunicationPreferences.icdoPersonCommunicationPreferences.registered_email_address = this.registered_email_address;
                lbusPersonCommunicationPreferences.icdoPersonCommunicationPreferences.is_paper_statement = this.is_paper_statement;
                lbusPersonCommunicationPreferences.icdoPersonCommunicationPreferences.modified_by = iobjPassInfo.istrUserID;
                lbusPersonCommunicationPreferences.icdoPersonCommunicationPreferences.modified_date = DateTime.Now;
                lbusPersonCommunicationPreferences.icdoPersonCommunicationPreferences.Update();
            }
            else
            {
                lbusPersonCommunicationPreferences.icdoPersonCommunicationPreferences.registered_email_address = this.registered_email_address;
                lbusPersonCommunicationPreferences.icdoPersonCommunicationPreferences.is_paper_statement = this.is_paper_statement;
                lbusPersonCommunicationPreferences.icdoPersonCommunicationPreferences.created_by = iobjPassInfo.istrUserID;
                lbusPersonCommunicationPreferences.icdoPersonCommunicationPreferences.created_date = DateTime.Now;
                lbusPersonCommunicationPreferences.icdoPersonCommunicationPreferences.modified_by = iobjPassInfo.istrUserID;
                lbusPersonCommunicationPreferences.icdoPersonCommunicationPreferences.modified_date = DateTime.Now;
                lbusPersonCommunicationPreferences.icdoPersonCommunicationPreferences.is_paper_spd = "N";
                lbusPersonCommunicationPreferences.icdoPersonCommunicationPreferences.Insert();
            }
            LoadPersonCommPref();
            larrResult.Add(this);
            return larrResult;
        }

        #region Public Methods
        public void LoadProcessActivityLog()
        {
            DataTable ldtblist = busPerson.Select("cdoPerson.LoadProcessActivityLog", new object[1] { this.icdoPerson.person_id });
            iclbProcessActivityLog = GetCollection<busProcessActivityLog>(ldtblist, "icdoProcessActivityLog");
        }

        //Loading Person Account Beneficiaries Collection For all Beneficiaries
        public void LoadBeneficiaries()
        {
            // DataTable ldtblist = busPerson.Select("cdoPerson.LoadAllPlans&Beneficiaries", new object[1] { this.icdoPerson.person_id });
            DataTable ldtblist = busPerson.Select("cdoPerson.LoadAllBeneficiaries", new object[1] { this.icdoPerson.person_id });
            iclbPersonAccountBeneficiary = GetCollection<busPersonAccountBeneficiary>(ldtblist, "icdoPersonAccountBeneficiary");
            //int i = 1;
            //foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in iclbPersonAccountBeneficiary)
            //{
            //    lbusPersonAccountBeneficiary.ibusRelationship.icdoRelationship.iintAPrimaryKey = i;
            //    i++;
            //}
        }
        public void LoadBeneficiariesOf()
        {
            // DataTable ldtblist = busPerson.Select("cdoPerson.LoadAllPlans&Beneficiaries", new object[1] { this.icdoPerson.person_id });
            DataTable ldtblist = busPerson.Select("cdoPerson.GetBenificiaryOf", new object[1] { this.icdoPerson.person_id });
            iclbPersonAccountBeneficiaryOf = GetCollection<busPersonAccountBeneficiary>(ldtblist, "icdoPersonAccountBeneficiary");
        }
        //Ticket - 68547
        public void LoadDependentOf()
        {
            DataTable ldtblist = busPerson.Select("cdoPerson.GetDependentOf", new object[1] { this.icdoPerson.person_id });
            iclbDependentOf = GetCollection<busPerson>(ldtblist, "icdoPerson");
        }

        public void LoadRetirementContributionsbyPersonId(int aintPersonId)
        {
            DataTable ldtbList = busBase.Select("cdoPerson.GetUVHPAmounByPersonID", new object[1] { aintPersonId });
            iclbPersonAccountRetirementContribution = GetCollection<busPersonAccountRetirementContribution>(ldtbList, "icdoPersonAccountRetirementContribution");
        }

        public void LoadBenefitApplication()
        {
            DataTable ldtblist = busPerson.Select("cdoBenefitApplication.LaodBenefitApplicationforOverview", new object[1] { this.icdoPerson.person_id });
            iclbBenefitApplication = GetCollection<busBenefitApplication>(ldtblist, "icdoBenefitApplication");
        }

        public void LoadBeneficiariesForDeath()
        {
            // DataTable ldtblist = busPerson.Select("cdoPerson.LoadAllPlans&Beneficiaries", new object[1] { this.icdoPerson.person_id });
            DataTable ldtblist = busPerson.Select("cdoDeathNotification.LoadAllBeneficiaries", new object[1] { this.icdoPerson.person_id });
            iclbPersonAccountBeneficiary = GetCollection<busPersonAccountBeneficiary>(ldtblist, "icdoPersonAccountBeneficiary");
        }

        public void LoadPersonBridgedService()
        {
            DataTable ldtblist = busPerson.Select("cdoPerson.LoadPersonBridgedService", new object[1] { this.icdoPerson.person_id });
            iclbPersonBridgeHours = GetCollection<busPersonBridgeHours>(ldtblist, "icdoPersonBridgeHours");
        }

        public override busBase GetCorPerson()
        {
            this.LoadPersonAddresss();
            this.LoadPersonContacts();
            this.LoadCorrAddress();
            return this;
            //return this;
        }

        public void LoadPersonAccounts()
        {
            DataTable ldtblist = busPerson.Select("cdoPerson.LoadPersonAccounts", new object[1] { this.icdoPerson.person_id });
            iclbPersonAccount = GetCollection<busPersonAccount>(ldtblist, "icdoPersonAccount");
        }

        /// <summary>
        /// Loading Person Account : IF Person is a beneficiary
        /// </summary>
        public void LoadPersonAccountsForSurvivor()
        {
            DataTable ldtblist = busPerson.Select("cdoPerson.LoadPersonAccountsForSurvivor", new object[1] { this.icdoPerson.person_id });
            iclbPersonAccount = GetCollection<busPersonAccount>(ldtblist, "icdoPersonAccount");
        }

        public void LoadPersonAccountsByPlanId(int aintPlanId)
        {
            DataTable ldtblist = busPerson.Select("cdoPersonAccount.LoadPersonAccountbyPlanId", new object[2] { this.icdoPerson.person_id, aintPlanId });
            iclbPersonAccount = GetCollection<busPersonAccount>(ldtblist, "icdoPersonAccount");
        }

        public void LoadPersonDependents()
        {
            DataTable ldtblist = busPerson.Select("cdoPerson.GetDependentName", new object[1] { this.icdoPerson.person_id });
            iclbPersonDependent = GetCollection<busPersonDependent>(ldtblist, "icdoRelationship");
        }

        public int GetPersonAccountIdByPlanAndPersonId(int aintPersonID, int aintPlanId)
        {
            DataTable ldtbList = Select<cdoPersonAccount>(
                new string[2] { enmPersonAccount.person_id.ToString(), enmPersonAccount.plan_id.ToString() },
                new object[2] { aintPersonID, aintPlanId }, null, null);

            if (ldtbList.Rows.Count > 0)
            {
                return Convert.ToInt32(ldtbList.Rows[0][enmPersonAccount.person_account_id.ToString()]);
            }
            else
            {
                return 0;
            }

        }

        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            if (this is busPersonOverview)
                return;
            utlError lobjError = null;

            bool IsManager = false;
            busUserRoles lbusUserRoles = new busUserRoles { icdoRoles = new cdoRoles() };
            if (lbusUserRoles.FindUserRoles(iobjPassInfo.iintUserSerialID, busConstant.Role.MANAGER_ROLE))
            {
                IsManager = true;
            }

            if (iobjPassInfo.istrFormName == "wfmBeneficiaryMaintenance" && istrAddressSameAsParticipant.IsNotNull() && istrAddressSameAsParticipant == busConstant.FLAG_YES && iintSelectedParticipantId > 0)
            {
                DataTable ldtActiveAddressOfParticipant = busBase.Select("cdoPersonContact.GetActiveAddress", new object[1] { iintSelectedParticipantId });
                if (ldtActiveAddressOfParticipant.IsNull() || ldtActiveAddressOfParticipant.Rows.Count == 0)
                {
                    utlError lutlError = new utlError();
                    lutlError = AddError(1182, "Participant does not have an Valid Address");
                    this.iarrErrors.Add(lutlError);
                    return;
                }
            }
            //Ticket#76267
            if(this.icdoPerson.retirement_health_date != null && this.icdoPerson.retirement_health_date != DateTime.MinValue )
            {
                if (!this.CheckHealthEligibilityEffectiveDate())
                {
                    utlError lutlError = new utlError();
                    lutlError = AddError(6295, "Not Eligible.");
                    this.iarrErrors.Add(lutlError);
                    return;

                }

            }
            

            //base.ValidateHardErrors(aenmPageMode);
            // PROD PIR 135 below 3 conditions added
            if (this.icdoPerson.first_name == null)
            {
                lobjError = AddError(1100, " ");
                this.iarrErrors.Add(lobjError);
            }
            if (this.icdoPerson.last_name == null)
            {
                lobjError = AddError(1101, " ");
                this.iarrErrors.Add(lobjError);
            }
            if (this.icdoPerson.idtDateofBirth.IsNull() || this.icdoPerson.idtDateofBirth == DateTime.MinValue)
            {
                if (this.CheckMemberIsParticipant())
                {
                    lobjError = AddError(5032, " ");
                    this.iarrErrors.Add(lobjError);
                }
            }
            if (this.ibusPersonAddress != null && PersonAddressEntered())
            {
                if (this.ibusPersonAddress.iarrErrors != null)
                {
                    this.ibusPersonAddress.iarrErrors.Clear();
                }
                this.ibusPersonAddress.ValidateHardErrors(utlPageMode.All);

                //PIR 1050
                if (this.ibusPersonAddress.CheckAddressTypeMandatory())
                {
                    lobjError = AddError(1113, " ");
                    this.iarrErrors.Add(lobjError);
                }

                if (this.iarrErrors.Count == 0)
                {
                    this.iarrErrors = this.ibusPersonAddress.iarrErrors;
                }
            }

            if (string.IsNullOrEmpty(this.icdoPerson.communication_preference_value) && (!string.IsNullOrEmpty(this.icdoPerson.email_address_1) || blnCheckActiveAddress))
            {
                lobjError = AddError(1166, " ");
                this.iarrErrors.Add(lobjError);
            }

            if (this.icdoPerson.communication_preference_value == "MAIL")
            {
                if ((this.iclbPersonAddress != null && this.iclbPersonAddress.Count == 0) || this.iclbPersonAddress == null)
                {
                    lobjError = AddError(1163, " ");
                    this.iarrErrors.Add(lobjError);
                }
            }

            if (iobjPassInfo.ienmPageMode == utlPageMode.Update)
            {
                if (Convert.ToString(icdoPerson.ihstOldValues["date_of_birth"]) != string.Empty)
                {
                    string lstrDecryptedDOB = string.Empty;
                    lstrDecryptedDOB = Convert.ToString(icdoPerson.ihstOldValues["date_of_birth"]);
                    DateTime ldtOldDateOfBirth = new DateTime();
                    if (!string.IsNullOrEmpty(lstrDecryptedDOB))
                    {
                        ldtOldDateOfBirth = Convert.ToDateTime(lstrDecryptedDOB);
                    }
                    if (ldtOldDateOfBirth != icdoPerson.idtDateofBirth)
                    {
                        int lintCountPayees = (int)DBFunction.DBExecuteScalar("cdoPerson.GetPayeeAccountWithStatusNotCncld", new object[1] { this.icdoPerson.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                        if (lintCountPayees > 0 && !IsManager)
                        {
                            lobjError = AddError(6067, " ");
                            this.iarrErrors.Add(lobjError);
                        }
                        else if (lintCountPayees > 0 && IsManager) //PIR-830
                        {
                            iblnManagerChangedBirthdateIfPayeeActApproved = true;
                        }

                        busRetirementApplication lbusRetirementApplication = new busRetirementApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                        lbusRetirementApplication.icdoBenefitApplication.person_id = icdoPerson.person_id;
                        lbusRetirementApplication.GetRetirementDate();

                        if (lbusRetirementApplication.GetRetirementDate() != DateTime.MinValue && !IsManager)
                        {
                            lobjError = AddError(6090, " ");
                            this.iarrErrors.Add(lobjError);
                        }
                        else if (lbusRetirementApplication.GetRetirementDate() != DateTime.MinValue && IsManager)//PIR-830
                        {
                            iblnIsManagerChangedBirthdateIfBenefitAppApproved = true;
                        }
                    }
                }
                if (this.icdoPerson.ssn.IsNullOrEmpty() && this.iblnParticipant == busConstant.YES)
                {
                    lobjError = AddError(1109, " ");
                    this.iarrErrors.Add(lobjError);
                }

                //Change ID:53632
                if (Convert.ToString(icdoPerson.ihstOldValues["ssn"]) != string.Empty)
                {
                    string lstrOldSSN = string.Empty;
                    lstrOldSSN = Convert.ToString(icdoPerson.ihstOldValues["ssn"]);

                    if (lstrOldSSN != icdoPerson.ssn)
                    {
                        //Second parameter in the query is purposely send 0 as its not merge operation and just change of ssn.
                        int lintCountPayees = (int)DBFunction.DBExecuteScalar("cdoPerson.GetPayeeAccountWithStatusNotCncldCmpl", new object[2] { this.icdoPerson.person_id, 0 }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                        if (lintCountPayees > 0 && !IsManager)
                        {
                            lobjError = AddError(6288, " ");
                            this.iarrErrors.Add(lobjError);
                        }
                    }
                }
            }
            if (this.iclbPersonBridgeHours != null)
            {
                foreach (busPersonBridgeHours lbusPersonBridgeHours in this.iclbPersonBridgeHours)
                {
                    Hashtable lhstParams = new Hashtable();
                    lhstParams["icdoPersonBridgeHours.person_bridge_id"] = lbusPersonBridgeHours.icdoPersonBridgeHours.person_bridge_id;
                    lhstParams["icdoPersonBridgeHours.bridge_type_value"] = lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_type_value;
                    lhstParams["icdoPersonBridgeHours.hours_reported"] = lbusPersonBridgeHours.icdoPersonBridgeHours.hours_reported;
                    lhstParams["icdoPersonBridgeHours.bridge_start_date"] = lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_start_date;
                    lhstParams["icdoPersonBridgeHours.bridge_end_date"] = lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_end_date;
                    CheckErrorOnAddButton(this, lhstParams, ref iarrErrors, true);
                    CheckOverlappingPeriod(this, lhstParams, ref iarrErrors, true);
                }
            }
            //For validating/executing hard errors/methods
            base.ValidateHardErrors(aenmPageMode);
        }

        //public void GetRetireeHealthHours()
        //{
        //    utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
        //    string lstrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

        //    SqlParameter[] parameters = new SqlParameter[1];
        //    SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
        //    param1.Value = icdoPerson.istrSSNNonEncrypted;
        //    parameters[0] = param1;

        //    //SqlParameter param2 = new SqlParameter("@BATCH_RUN_DATE", DbType.String);
        //    //param2.Value = iobjSystemManagement.icdoSystemManagement.batch_date;
        //    //parameters[1] = param2;            

        //    DataTable ldtPersonHealthWorkHistory = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetHealthWorkData", lstrLegacyDBConnetion, null, parameters);
        //    if (ldtPersonHealthWorkHistory.Rows.Count > 0)
        //    {
        //        busPersonBatchFlags lbusPersonBatchFlags = new busPersonBatchFlags { icdoPersonBatchFlags = new cdoPersonBatchFlags() };
        //        iclbPersonWorkHistory = cdoDummyWorkData.GetCollection<cdoDummyWorkData>(ldtPersonHealthWorkHistory);

        //        if (ibusBenefitApplication == null || ibusBenefitApplication.ibusPerson == null)
        //        {
        //            ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
        //            ibusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
        //            ibusBenefitApplication.ibusPerson.FindPerson(icdoPerson.person_id);
        //        }

        //        if (ibusBenefitApplication.idecAge.IsNull() || ibusBenefitApplication.idecAge == 0)
        //        {
        //            ibusBenefitApplication.idecAge = busGlobalFunctions.CalculatePersonAgeInDec(icdoPerson.idtDateofBirth, DateTime.Now);
        //        }

        //        ibusBenefitApplication.ProcessWorkHistoryforBISandForfieture(iclbPersonWorkHistory, busConstant.MPIPP);
        //    }
        //}

        //public int WorkingDays(DateTime adtfirstDay, DateTime adtlastDay)
        //{
        //    int businessDays = 0;
        //    DateTime ldtTemp = new DateTime();
        //    adtfirstDay = adtfirstDay.Date;
        //    adtlastDay = adtlastDay.Date;
        //    ldtTemp = adtfirstDay;
        //    ldtTemp=ldtTemp.Date;

        //    while (ldtTemp <= adtlastDay)
        //    {

        //        if (ldtTemp.DayOfWeek != DayOfWeek.Saturday && ldtTemp.DayOfWeek != DayOfWeek.Sunday)
        //        {
        //            businessDays++;
        //        }
        //        ldtTemp= ldtTemp.AddDays(1);
        //    }

        //    return businessDays;
        //}

        public void AddPersonBridgingDetails(busPersonBridgeHours aobjPersonBridgeHours)
        {
            int lintWorkingDays = 0;
            int lintBridgeEndDateYear = 0;
            DateTime ldtBridgeStartDate = aobjPersonBridgeHours.icdoPersonBridgeHours.bridge_start_date;
            DateTime ldtBridgeEndDate = aobjPersonBridgeHours.icdoPersonBridgeHours.bridge_end_date;
            aobjPersonBridgeHours.icdoPersonBridgeHours.hours_reported = 0;
            DateTime ldtTemp = new DateTime();

            if (aobjPersonBridgeHours.iclbPersonBridgeHoursDetails.Count > 0)
            {
                foreach (busPersonBridgeHoursDetail lbusPersonBridgeHoursDetail in aobjPersonBridgeHours.iclbPersonBridgeHoursDetails)
                {
                    lbusPersonBridgeHoursDetail.Delete();
                }
            }

            ldtTemp = ldtBridgeStartDate;

            if (ldtBridgeEndDate > busGlobalFunctions.GetLastDateOfComputationYear(ldtBridgeEndDate.Year))
            {
                lintBridgeEndDateYear = ldtBridgeEndDate.Year + 1;
                
            }
            else
            {
                lintBridgeEndDateYear = ldtBridgeEndDate.Year;
            }

            while (ldtTemp.Year <= lintBridgeEndDateYear || (ldtBridgeStartDate.Year == ldtBridgeEndDate.Year && ldtBridgeEndDate > busGlobalFunctions.GetLastDateOfComputationYear(ldtTemp.Year - 1)))
            {
                if (ldtTemp.Year == ldtBridgeStartDate.Year && ldtTemp >= busGlobalFunctions.GetLastDateOfComputationYear(ldtTemp.Year))
                {
                    ldtTemp = ldtTemp.AddYears(1);
                }
                else
                {
                    lintWorkingDays = 0;
                    busPersonBridgeHoursDetail lbusPersonBridgeHoursDetail = new busPersonBridgeHoursDetail { icdoPersonBridgeHoursDetail = new cdoPersonBridgeHoursDetail() };
                    DateTime ldtLastDayOfYear = busGlobalFunctions.GetLastDateOfComputationYear(ldtTemp.Year);
                    DateTime ldtDayAfterLastDayOfPreYear = busGlobalFunctions.GetLastDateOfComputationYear(ldtTemp.Year - 1).AddDays(1);
                    if (ldtTemp.Year == ldtBridgeStartDate.Year)
                    {
                        if (ldtTemp.Year != ldtBridgeEndDate.Year)
                        {

                            lintWorkingDays = busGlobalFunctions.WorkingDays(ldtBridgeStartDate, ldtLastDayOfYear);
                            lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.from_date = ldtBridgeStartDate;
                            lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.to_date = ldtLastDayOfYear;
                            lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.computation_year = ldtTemp.Year;

                        }
                        else if (ldtBridgeEndDate < ldtLastDayOfYear && ldtTemp.Year == ldtBridgeEndDate.Year)
                        {
                            lintWorkingDays = busGlobalFunctions.WorkingDays(ldtBridgeStartDate, ldtBridgeEndDate);
                            lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.from_date = ldtBridgeStartDate;
                            lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.to_date = ldtBridgeEndDate;
                            lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.computation_year = ldtTemp.Year;
                        }
                        else
                        {
                            lintWorkingDays = busGlobalFunctions.WorkingDays(ldtBridgeStartDate, ldtLastDayOfYear);
                            lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.from_date = ldtBridgeStartDate;
                            lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.to_date = ldtLastDayOfYear;
                            lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.computation_year = ldtTemp.Year;
                        }
                    }
                    else if (ldtTemp.Year == ldtBridgeEndDate.Year)
                    {

                        if ((ldtTemp.Year == ldtBridgeStartDate.Year + 1) && (ldtBridgeStartDate >= busGlobalFunctions.GetLastDateOfComputationYear(ldtBridgeStartDate.Year)))
                        {
                            lintWorkingDays = busGlobalFunctions.WorkingDays(ldtBridgeStartDate, ldtLastDayOfYear);
                            lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.from_date = ldtBridgeStartDate;
                            lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.to_date = ldtLastDayOfYear;
                            lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.computation_year = ldtTemp.Year;
                        }
                        else if (ldtBridgeEndDate > busGlobalFunctions.GetLastDateOfComputationYear(ldtBridgeEndDate.Year))
                        {
                            lintWorkingDays = busGlobalFunctions.WorkingDays(ldtDayAfterLastDayOfPreYear, ldtLastDayOfYear);
                            lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.from_date = ldtDayAfterLastDayOfPreYear;
                            lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.to_date = ldtLastDayOfYear;
                            lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.computation_year = ldtTemp.Year;
                        }
                        else
                        {
                            lintWorkingDays = busGlobalFunctions.WorkingDays(ldtDayAfterLastDayOfPreYear, ldtBridgeEndDate);
                            lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.from_date = ldtDayAfterLastDayOfPreYear;
                            lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.to_date = ldtBridgeEndDate;
                            lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.computation_year = ldtTemp.Year;
                        }
                    }
                    else if (ldtTemp.Year > ldtBridgeEndDate.Year)
                    {
                        lintWorkingDays = busGlobalFunctions.WorkingDays(ldtDayAfterLastDayOfPreYear, ldtBridgeEndDate);
                        lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.from_date = ldtDayAfterLastDayOfPreYear;
                        lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.to_date = ldtBridgeEndDate;
                        lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.computation_year = ldtTemp.Year;
                    }
                    else
                    {
                        if ((ldtBridgeStartDate.Year == ldtBridgeEndDate.Year && ldtBridgeEndDate > busGlobalFunctions.GetLastDateOfComputationYear(ldtBridgeEndDate.Year)))
                        {
                            if ((ldtTemp.Year == ldtBridgeStartDate.Year + 1) && (ldtBridgeStartDate >= busGlobalFunctions.GetLastDateOfComputationYear(ldtBridgeStartDate.Year)))
                            {
                                lintWorkingDays = busGlobalFunctions.WorkingDays(ldtBridgeStartDate, ldtBridgeEndDate);
                                lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.from_date = ldtBridgeStartDate;
                                lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.to_date = ldtBridgeEndDate;
                                lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.computation_year = ldtTemp.Year;
                            }
                            else
                            {
                                lintWorkingDays = busGlobalFunctions.WorkingDays(ldtDayAfterLastDayOfPreYear, ldtBridgeEndDate);
                                lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.from_date = ldtDayAfterLastDayOfPreYear;
                                lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.to_date = ldtBridgeEndDate;
                                lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.computation_year = ldtTemp.Year;
                            }
                        }
                        else
                        {
                            if ((ldtTemp.Year == ldtBridgeStartDate.Year + 1) && (ldtBridgeStartDate >= busGlobalFunctions.GetLastDateOfComputationYear(ldtBridgeStartDate.Year)))
                            {
                                lintWorkingDays = busGlobalFunctions.WorkingDays(ldtBridgeStartDate, ldtLastDayOfYear);
                                lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.from_date = ldtBridgeStartDate;
                                lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.to_date = ldtLastDayOfYear;
                                lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.computation_year = ldtTemp.Year;
                            }
                            else
                            {
                                lintWorkingDays = busGlobalFunctions.WorkingDays(ldtDayAfterLastDayOfPreYear, ldtLastDayOfYear);
                                lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.from_date = ldtDayAfterLastDayOfPreYear;
                                lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.to_date = ldtLastDayOfYear;
                                lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.computation_year = ldtTemp.Year;
                            }
                        }
                    }

                    ////Ticket#85664
                    decimal ldtHoursReportedInparticularYear = Convert.ToDecimal(lintWorkingDays * 8);
                    if (aobjPersonBridgeHours.icdoPersonBridgeHours.bridge_type_value == "DSBL")
                    {   
                       // this.icdoPerson.ssn,ldtBridgeEndDate
                            
                        DataTable ldtbCreditedHours = busBase.Select("cdoPerson.GetCreditedHoursbySSN", new object[2] { Convert.ToInt32(ldtBridgeEndDate.Year), this.icdoPerson.ssn });
                        if(ldtbCreditedHours.Rows.Count > 0)
                        {
                          if(Convert.ToDecimal(ldtbCreditedHours.Rows[0][0]) < 200)
                            {
                                ldtHoursReportedInparticularYear = 200;
                            }
                        }
                        //if (ldtHoursReportedInparticularYear < 200)
                        //{
                        //    ldtHoursReportedInparticularYear = 200;
                        //}
                    }
                    //RID 119820 - Covid 19 hardship bridging ,  added new bridge code.
                    if (aobjPersonBridgeHours.icdoPersonBridgeHours.bridge_type_value == "COVD")
                    {
                        DataTable ldtbCreditedHours = busBase.Select("cdoPerson.GetCreditedHoursbySSN", new object[2] { Convert.ToInt32(ldtBridgeEndDate.Year), this.icdoPerson.ssn });
                        if (ldtbCreditedHours != null && ldtbCreditedHours.Rows.Count > 0)
                        {
                            if (Convert.ToDecimal(ldtbCreditedHours.Rows[0][0]) < 200)
                            {
                                ldtHoursReportedInparticularYear = 200;
                            }
                            else
                            {
                                ldtHoursReportedInparticularYear = Convert.ToDecimal(ldtbCreditedHours.Rows[0][0]);
                            }
                        }
                        else
                        {
                            ldtHoursReportedInparticularYear = 200;
                        }
                    }
                    //RID 151812 - added new bridge code for MOA 2024.
                    if (aobjPersonBridgeHours.icdoPersonBridgeHours.bridge_type_value == "MA24")
                    {
                        DataTable ldtbCreditedHours = busBase.Select("cdoPerson.GetCreditedHoursbySSN", new object[2] { Convert.ToInt32(ldtBridgeEndDate.Year), this.icdoPerson.ssn });
                        if (ldtbCreditedHours != null && ldtbCreditedHours.Rows.Count > 0)
                        {
                            if (Convert.ToDecimal(ldtbCreditedHours.Rows[0][0]) < 200)
                            {
                                ldtHoursReportedInparticularYear = 200;
                            }
                            else
                            {
                                ldtHoursReportedInparticularYear = Convert.ToDecimal(ldtbCreditedHours.Rows[0][0]);
                            }
                        }
                        else
                        {
                            ldtHoursReportedInparticularYear = 200;
                        }
                    }

                    lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.hours = ldtHoursReportedInparticularYear;
                    aobjPersonBridgeHours.icdoPersonBridgeHours.hours_reported = ldtHoursReportedInparticularYear;

                    lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.person_bridge_id = aobjPersonBridgeHours.icdoPersonBridgeHours.person_bridge_id;
                    lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.created_by = "OPUS";
                    lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.created_date = DateTime.Now;
                    lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.modified_by = "OPUS";
                    lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.modified_date = DateTime.Now;
                    lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.update_seq = 0;
                    lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.Insert();

                    aobjPersonBridgeHours.iclbPersonBridgeHoursDetails.Add(lbusPersonBridgeHoursDetail);
                    ldtTemp = ldtTemp.AddYears(1);
                }
            }
            aobjPersonBridgeHours.icdoPersonBridgeHours.Update();

        }


        public void LoadWorkHistory()
        {
            ibusBenefitApplication = new busBenefitApplication();
            ibusBenefitApplication.ibusPerson = new busPerson();
            ibusBenefitApplication.ibusPerson.FindPerson(this.icdoPerson.person_id);
            ibusBenefitApplication.ibusPerson.LoadPersonAccounts();

            ibusBenefitApplication.aclbPersonWorkHistory_MPI = new Collection<cdoDummyWorkData>();
            ibusBenefitApplication.aclbPersonWorkHistory_IAP = new Collection<cdoDummyWorkData>();
            ibusBenefitApplication.icdoBenefitApplication = new cdoBenefitApplication();
            ibusBenefitApplication.LoadWorkHistoryandSetupPrerequisites_PersonOverView();
        }

        public ArrayList CheckErrorOnAddButton(busPerson aobjPerson, Hashtable ahstParams, ref ArrayList aarrErrors, bool ablnHardError = false)
        {
            utlError lobjError = null;

            //if (Convert.ToInt32(ahstParams["icdoPersonBridgeHours.computation_year"]) == 0)
            //{
            //    lobjError = AddError(5134, "");
            //    aarrErrors.Add(lobjError);
            //    return aarrErrors;

            //}
            if (Convert.ToString(ahstParams["icdoPersonBridgeHours.bridge_type_value"]) == "")
            {
                lobjError = AddError(5135, "");
                aarrErrors.Add(lobjError);

            }

            //if (Convert.ToString(ahstParams["icdoPersonBridgeHours.hours_reported"]) == "")
            //{
            //    lobjError = AddError(5136, "");
            //    aarrErrors.Add(lobjError);

            //}
            if (Convert.ToString(ahstParams["icdoPersonBridgeHours.bridge_start_date"]) == "" || Convert.ToDateTime(ahstParams["icdoPersonBridgeHours.bridge_start_date"]) == DateTime.MinValue)
            {
                lobjError = AddError(5137, "");
                aarrErrors.Add(lobjError);
                return aarrErrors;
            }
            if (Convert.ToString(ahstParams["icdoPersonBridgeHours.bridge_end_date"]) == "" || Convert.ToDateTime(ahstParams["icdoPersonBridgeHours.bridge_end_date"]) == DateTime.MinValue)
            {
                lobjError = AddError(5138, "");
                aarrErrors.Add(lobjError);
                return aarrErrors;
            }

            if (Convert.ToDateTime(ahstParams["icdoPersonBridgeHours.bridge_start_date"]) > Convert.ToDateTime(ahstParams["icdoPersonBridgeHours.bridge_end_date"]))
            {
                lobjError = AddError(5139, "");
                aarrErrors.Add(lobjError);
                return aarrErrors;
            }
          //  int lbendDate = Convert.ToDateTime(ahstParams["icdoPersonBridgeHours.bridge_end_date"]).Year;


            if (Convert.ToDateTime(ahstParams["icdoPersonBridgeHours.bridge_end_date"]) > busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToDateTime(ahstParams["icdoPersonBridgeHours.bridge_end_date"]).Year))
            {
                lobjError = AddError(0, "Bridge End Date cannot be Greater than Last Date of Computation Year");
                aarrErrors.Add(lobjError);
                return aarrErrors;

            }
                //if (Convert.ToDateTime(ahstParams["icdoPersonBridgeHours.bridge_start_date"]).Year != Convert.ToInt32(ahstParams["icdoPersonBridgeHours.computation_year"])
                //    || Convert.ToDateTime(ahstParams["icdoPersonBridgeHours.bridge_end_date"]).Year != Convert.ToInt32(ahstParams["icdoPersonBridgeHours.computation_year"]))
                //{
                //    lobjError = AddError(5140, "");
                //    aarrErrors.Add(lobjError);
                //    return aarrErrors;
                //}

                //foreach (busPersonBridgeHours lbusPersonBridgeHours in aobjPerson.iclbPersonBridgeHours)
                //{
                //   if(((Convert.ToDateTime(ahstParams["icdoPersonBridgeHours.bridge_start_date"]) >=  lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_start_date && 
                //       Convert.ToDateTime(ahstParams["icdoPersonBridgeHours.bridge_start_date"]) <= lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_end_date)) || 
                //       ((Convert.ToDateTime(ahstParams["icdoPersonBridgeHours.bridge_end_date"]) >=  lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_start_date && 
                //       Convert.ToDateTime(ahstParams["icdoPersonBridgeHours.bridge_end_date"]) <= lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_end_date)))
                //   {
                //       lobjError = AddError(5141, "");
                //       aarrErrors.Add(lobjError);
                //       return aarrErrors;
                //   }
                //}

                return aarrErrors;
        }

        public void CheckOverlappingPeriod(busPerson aobjPerson, Hashtable ahstParams, ref ArrayList aarrErrors, bool ablnHardError = false)
        {
            utlError lobjError = null;
            foreach (busPersonBridgeHours lbusPersonBridgeHours in aobjPerson.iclbPersonBridgeHours)
            {
                if (lbusPersonBridgeHours.icdoPersonBridgeHours.person_bridge_id != Convert.ToInt32(ahstParams["icdoPersonBridgeHours.person_bridge_id"]))
                {
                    //UAT PIR 130
                    if (busGlobalFunctions.CheckDateOverlapping(Convert.ToDateTime(ahstParams["icdoPersonBridgeHours.bridge_start_date"]),
                                                            lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_start_date, lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_end_date) ||
                        busGlobalFunctions.CheckDateOverlapping(Convert.ToDateTime(ahstParams["icdoPersonBridgeHours.bridge_end_date"]),
                                                            lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_start_date, lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_end_date) ||
                        busGlobalFunctions.CheckDateOverlapping(lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_start_date,
                                                                Convert.ToDateTime(ahstParams["icdoPersonBridgeHours.bridge_start_date"]), Convert.ToDateTime(ahstParams["icdoPersonBridgeHours.bridge_end_date"])) ||
                        busGlobalFunctions.CheckDateOverlapping(lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_end_date,
                                                                Convert.ToDateTime(ahstParams["icdoPersonBridgeHours.bridge_start_date"]), Convert.ToDateTime(ahstParams["icdoPersonBridgeHours.bridge_end_date"])))
                    //if (((lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_start_date <= Convert.ToDateTime(ahstParams["icdoPersonBridgeHours.bridge_end_date"])) &&
                    //    (Convert.ToDateTime(ahstParams["icdoPersonBridgeHours.bridge_end_date"]) <= lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_end_date) ||
                    //    ((lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_start_date <= Convert.ToDateTime(ahstParams["icdoPersonBridgeHours.bridge_start_date"])) &&
                    //    (Convert.ToDateTime(ahstParams["icdoPersonBridgeHours.bridge_start_date"]) <= lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_end_date))))
                    {
                        bool flag = false;
                        foreach (utlError lerr in aarrErrors)
                        {
                            if (lerr.istrErrorID == "5141")
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            lobjError = AddError(5141, "");
                            aarrErrors.Add(lobjError);
                            break;
                        }
                    }
                }
            }

        }

        //public void LoadPersonDependentsForDeath()
        //{
        //    DataTable ldtblist = busPerson.Select("cdoDeathNotification.LoadAllDependents", new object[1] { this.icdoPerson.person_id });
        //    iclbPersonDependent = GetCollection<busPersonDependent>(ldtblist, "icdoRelationship");
        //}
        /// <summary>
        ///To check if SSN is already exists.
        /// </summary>        
        public bool IsSSNAlreadyExists()
        {
            if (icdoPerson.ssn != null)
            {
                DataTable ldtblist = Select<cdoPerson>(
                    new string[1] { enmPerson.ssn.ToString() },
                    new object[1] { icdoPerson.ssn }, null, null);

                if (ldtblist.Rows.Count > 0)
                {

                    if (this.icdoPerson.ienuObjectState.Equals(ObjectState.Insert))
                    {
                        return true;
                    }
                    else if (icdoPerson.ihstOldValues.Count > 0 && Convert.ToString(icdoPerson.ihstOldValues[enmPerson.ssn.ToString()]).IsNotNullOrEmpty()
                        && icdoPerson.ihstOldValues[enmPerson.ssn.ToString()].ToString() != icdoPerson.ssn)
                    {
                        return true;
                    }
                    else if (icdoPerson.ssn.IsNotNullOrEmpty() && icdoPerson.ihstOldValues.Count > 0 && Convert.ToString(icdoPerson.ihstOldValues[enmPerson.ssn.ToString()]).IsNullOrEmpty())
                    {
                        return true;
                    }
                }

            }
            return false;
        }

        public bool IsEmailValid()
        {
            return busGlobalFunctions.IsValidEmail(icdoPerson.email_address_1);
        }

        public void GetCurrentAge()
        {
            DateTime ldtDOB = icdoPerson.idtDateofBirth;
            idecAge = busGlobalFunctions.CalculatePersonAge(ldtDOB, DateTime.Today);
        }

        public bool CheckMemberIsBeneficiary()
        {
            DataTable ldtbBeneficiary = Select<cdoRelationship>(
               new string[1] { enmRelationship.beneficiary_person_id.ToString() },
               new object[1] { this.icdoPerson.person_id }, null, null);

            if (ldtbBeneficiary.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool CheckMemberIsAlternatePayee()
        {
            DataTable ldtbAlternatePayee = Select("cdoDroApplication.CheckCountApprovedApplication", new object[1] { this.icdoPerson.person_id });
            if (ldtbAlternatePayee != null && ldtbAlternatePayee.Rows.Count > 0)
            {
                int lintCount = Convert.ToInt32(ldtbAlternatePayee.Rows[0][0]);
                if (lintCount > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckMemberIsParticipant()
        {           
            if (!string.IsNullOrEmpty(this.icdoPerson.istrSSNNonEncrypted))
            {
                utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
                string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

                DataTable ldtbCheckIfParticipant = new DataTable();

                SqlParameter[] parameters = new SqlParameter[1];
                SqlParameter param1 = new SqlParameter("@SSN", DbType.String);


                param1.Value = this.icdoPerson.istrSSNNonEncrypted;
                parameters[0] = param1;

                ldtbCheckIfParticipant = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_PensionInterface4OPUS", astrLegacyDBConnetion, null, parameters);
                if (ldtbCheckIfParticipant.Rows.Count > 0)
                {
                    return true;
                }
                else  // PIR-788
                {
                    int iintIsParticipantCount = (int)DBFunction.DBExecuteScalar("cdoPerson.GetIsParticipantCount", new object[1] { this.icdoPerson.mpi_person_id },
                                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                    if (iintIsParticipantCount > 0)
                        return true;
                }
            }
            return false;
        }

        public bool CheckMemberIsDependent()
        {
            DataTable ldtbDependent = Select<cdoRelationship>(
                new string[1] { enmRelationship.dependent_person_id.ToString() },
                new object[1] { this.icdoPerson.person_id }, null, null);

            if (ldtbDependent.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckMemberIsRetiree()
        {
            DataTable ldtbBenefitApplcation = busBase.Select("cdoBenefitApplication.GetApprovedRetirement", new object[] { this.icdoPerson.person_id });
            if (ldtbBenefitApplcation != null && ldtbBenefitApplcation.Rows.Count > 0)
            {
                int lintCount = Convert.ToInt32(ldtbBenefitApplcation.Rows[0]["COUNT"]);
                if (lintCount > 0)
                {
                    return true;
                }
            }
            return false;

        }

        //PIR RID 56892
        public bool CheckAlreadyVested(string astrPlanCode)
        {
            busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };
            int lintAccountId = 0;
            if (iclbPersonAccount.IsNull())
            {
                LoadPersonAccounts();
            }

            if (this.iclbPersonAccount != null)
            {
                if (iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == astrPlanCode).Count() > 0)
                {
                    lintAccountId = iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == astrPlanCode).First().icdoPersonAccount.person_account_id;
                }
            }

            if (lintAccountId > 0)
            {
                DataTable ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { lintAccountId });
                if (ldtbPersonAccountEligibility.Rows.Count > 0)
                {
                    lbusPersonAccountEligibility.icdoPersonAccountEligibility.LoadData(ldtbPersonAccountEligibility.Rows[0]);
                    if (lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.IsNotNull() && lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date != DateTime.MinValue)
                    {
                        return true;
                    }
                }

            }
            return false;
        }

       
        public bool CheckAlternateCorresExists()
        {
            if (ibusPersonContactForCor == null)
            {
                ibusPersonContactForCor = new busPersonContact { icdoPersonContact = new cdoPersonContact() };
            }
            if (this.iclbPersonContact == null)
            {
                iclbPersonContact = new Collection<busPersonContact>();
                this.LoadPersonContacts();
            }
            if (!iclbPersonContact.IsNullOrEmpty())
            {
                foreach (busPersonContact lobjPersonContact in this.iclbPersonContact)
                {
                    if (lobjPersonContact.icdoPersonContact.effective_start_date <= DateTime.Now &&
                        ((lobjPersonContact.icdoPersonContact.effective_end_date == DateTime.MinValue) || DateTime.Now < lobjPersonContact.icdoPersonContact.effective_end_date) && lobjPersonContact.icdoPersonContact.correspondence_addr_flag == busConstant.FLAG_YES &&
                        (lobjPersonContact.icdoPersonContact.contact_type_value == busConstant.Gaurdian_CONTACT_VAL || lobjPersonContact.icdoPersonContact.contact_type_value == busConstant.PowOfAttr_CONTACT_VAL || lobjPersonContact.icdoPersonContact.contact_type_value == busConstant.Conservator_CONTACT_VAL))
                    {
                        ibusPersonContactForCor = lobjPersonContact;
                        return true;
                    }
                }
            }
            return false;
        }


        public void LoadParticipantPlan()
        {
            DataTable ldtblist = busPerson.Select("cdoPersonAccount.GetPlanForPersonOverview", new object[1] { this.icdoPerson.person_id });
            iclbPersonAccount = GetCollection<busPersonAccount>(ldtblist, "icdoPersonAccount");
        }

        #region Load Spouse Details
        public void LoadSpouseDetails()
        {
            DataTable ldtbSpousalDetails = busBase.Select("cdoRelationship.GetSpousalDetails", new object[] { this.icdoPerson.person_id });
            if (ldtbSpousalDetails != null && ldtbSpousalDetails.Rows.Count > 0)
            {
                DataRow dtrow = ldtbSpousalDetails.Rows[0];
                istrSpousalFirstName = Convert.ToString(dtrow[enmPerson.first_name.ToString()]);
                istrSpousalLastName = Convert.ToString(dtrow[enmPerson.last_name.ToString()]);
                istrSpousalMiddleName = Convert.ToString(dtrow[enmPerson.middle_name.ToString()]);
                istrSpousalPrefix = Convert.ToString(dtrow[enmPerson.name_prefix_value.ToString()]);
                istrSpousalSSN = HelperFunction.FormatData(this.icdoPerson.istrSSNNonEncrypted, "{0:000-##-####}");
            }
        }
        #endregion
        /// <summary>
        /// Pass Ben Id & Load the address if same as participant flag checked to no.
        /// 
        /// </summary>
        /// <param name="aintPersonID"></param>
        public void LoadBeneficiaryAddressForCorrespondence(int aintBenefeciaryPersonID)
        {
            if (ibusBeneficiaryPerson == null)
            {
                ibusBeneficiaryPerson = new busPerson { icdoPerson = new cdoPerson() };
            }
            DataTable ldtbRelation = Select<cdoRelationship>(
                   new string[1] { enmRelationship.beneficiary_person_id.ToString() },
                   new object[1] { this.icdoPerson.person_id }, null, null);
            string strSameAsParticipant = busConstant.FLAG_NO;
            if (ldtbRelation.Rows.Count > 0)
            {
                strSameAsParticipant = Convert.ToString(ldtbRelation.Rows[0][enmRelationship.addr_same_as_participant_flag.ToString()]);
                if (strSameAsParticipant == busConstant.FLAG_YES)
                {
                    ibusBeneficiaryPerson.ibusPersonAddressForCorr = ibusPersonAddressForCorr;
                }
                else
                {
                    ibusBeneficiaryPerson.FindPerson(aintBenefeciaryPersonID);
                    ibusBeneficiaryPerson.LoadCorrAddress();
                }
            }
        }

        public void LoadCorrAddress()
        {
            this.ibusPersonAddressForCorr = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() };

            bool blnCheckMail = false;
            if (this.CheckAlternateCorresExists())
            {
                this.ibusPersonAddressForCorr = LoadCorAddressFromContact(this.ibusPersonContactForCor);
            }
            else
            {
                if (this.iclbPersonAddress.IsNullOrEmpty())
                {
                    this.iclbPersonAddress = new Collection<busPersonAddress>();
                    this.LoadPersonAddresss();
                }
                if (!this.iclbPersonAddress.IsNullOrEmpty())
                {
                    foreach (busPersonAddress objbusPersonAddress in this.iclbPersonAddress)
                    {
                        if ((objbusPersonAddress.icdoPersonAddress.end_date >= DateTime.Now || objbusPersonAddress.icdoPersonAddress.end_date == DateTime.MinValue) &&
                            objbusPersonAddress.icdoPersonAddress.start_date <= DateTime.Now && objbusPersonAddress.icdoPersonAddress.start_date != objbusPersonAddress.icdoPersonAddress.end_date)
                        {
                            ibusPersonAddressForCorr = objbusPersonAddress;
                            if (!objbusPersonAddress.iclcPersonAddressChklist.IsNullOrEmpty())
                            {
                                foreach (cdoPersonAddressChklist lcdoPersonAddressChklist in objbusPersonAddress.iclcPersonAddressChklist)
                                {
                                    if (lcdoPersonAddressChklist.address_type_value == "MAIL")
                                    {
                                        blnCheckMail = true;
                                    }

                                }
                            }
                        }
                        if (blnCheckMail)
                        {
                            break;
                        }
                    }
                }
            }
        }

        public void LoadInitialData(bool ablnCheckIsMemberParticipant = true)
        {
            this.iblnBeneficiary = busConstant.NO;
            this.iblnAlternatePayee = busConstant.NO;
            this.iblnParticipant = busConstant.NO;
            this.iblnDependent = busConstant.NO;
            this.istrRetiree = busConstant.NO;
            this.lbusRetirementApplication = new busRetirementApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            this.lbusRetirementApplication.ibusPerson = this;
            if (this.CheckMemberIsBeneficiary())
            {
                this.iblnBeneficiary = busConstant.YES;
            }
            if (this.CheckMemberIsAlternatePayee())
            {
                this.iblnAlternatePayee = busConstant.YES;
            }
            if (!string.IsNullOrEmpty(this.icdoPerson.ssn) && ablnCheckIsMemberParticipant)
            {
                if (this.CheckMemberIsParticipant())
                {
                    this.iblnParticipant = busConstant.YES;
                }
            }
            if (this.CheckMemberIsDependent())
            {
                this.iblnDependent = busConstant.YES;
            }
            if (this.CheckMemberIsRetiree())
            {
                this.istrRetiree = busConstant.YES;
            }

        }

        public void LoadCourtAddress()
        {
            this.ibusPersonCourtContact = new busPersonContact { icdoPersonContact = new cdoPersonContact() };
            this.ibusCourtAddressForCorr = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() };
            foreach (busPersonContact lobjPersonContact in this.iclbPersonContact)
            {
                if (lobjPersonContact.icdoPersonContact.effective_start_date <= DateTime.Now &&
                   ((lobjPersonContact.icdoPersonContact.effective_end_date == DateTime.MinValue) || DateTime.Now < lobjPersonContact.icdoPersonContact.effective_end_date))
                {
                    if (lobjPersonContact.icdoPersonContact.contact_type_value == "CORT")
                    {
                        this.ibusPersonCourtContact = lobjPersonContact;
                        this.ibusCourtAddressForCorr = LoadCorAddressFromContact(lobjPersonContact);
                    }
                }
            }
        }

        public busPersonAddress LoadCorAddressFromContact(busPersonContact abusPersonContact)
        {
           
            busPersonAddress lbusPersonAddress = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() };
            lbusPersonAddress.icdoPersonAddress.addr_line_1 = abusPersonContact.icdoPersonContact.addr_line_1;
            lbusPersonAddress.icdoPersonAddress.addr_line_2 = abusPersonContact.icdoPersonContact.addr_line_2;
            lbusPersonAddress.icdoPersonAddress.addr_country_id = abusPersonContact.icdoPersonContact.addr_country_id;
            lbusPersonAddress.icdoPersonAddress.addr_country_value = abusPersonContact.icdoPersonContact.addr_country_value;
            lbusPersonAddress.icdoPersonAddress.addr_state_id = abusPersonContact.icdoPersonContact.addr_state_id;
            lbusPersonAddress.icdoPersonAddress.addr_state_value = abusPersonContact.icdoPersonContact.addr_state_value;
            lbusPersonAddress.icdoPersonAddress.addr_state_description = abusPersonContact.icdoPersonContact.addr_state_description;
            lbusPersonAddress.icdoPersonAddress.addr_city = abusPersonContact.icdoPersonContact.addr_city;
            lbusPersonAddress.icdoPersonAddress.county = abusPersonContact.icdoPersonContact.county;
            lbusPersonAddress.icdoPersonAddress.addr_zip_4_code = abusPersonContact.icdoPersonContact.addr_zip_4_code;
            lbusPersonAddress.icdoPersonAddress.addr_zip_code = abusPersonContact.icdoPersonContact.addr_zip_code;
            lbusPersonAddress.icdoPersonAddress.addr_country_description = abusPersonContact.icdoPersonContact.addr_country_description;
            lbusPersonAddress.icdoPersonAddress.foreign_postal_code = abusPersonContact.icdoPersonContact.foreign_postal_code;
            return lbusPersonAddress;
        }


        #endregion

        #region Overriden Methods

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (!string.IsNullOrEmpty(this.icdoPerson.istrSSNNonEncrypted) && this.icdoPerson.istrSSNNonEncrypted.Contains("-"))
            {
                this.icdoPerson.istrSSNNonEncrypted = this.icdoPerson.istrSSNNonEncrypted.Replace("-", string.Empty);
                this.icdoPerson.istrSSNNonEncrypted = this.icdoPerson.istrSSNNonEncrypted.Replace("_", string.Empty);   //PIR RID 63893  adding code to strip underscore
            }
            if (!string.IsNullOrEmpty(this.icdoPerson.istrSSNNonEncrypted))   //PIR RID 63893 updating icdoPerson.SSN field only if this is not null or empty string.
            {
                this.icdoPerson.ssn = this.icdoPerson.istrSSNNonEncrypted;
            }
            //this.icdoPerson.ssn = this.icdoPerson.istrSSNNonEncrypted;

            //if (!string.IsNullOrEmpty(Convert.ToString(this.icdoPerson.idtDateofBirth)))
            //{
            //    this.icdoPerson.date_of_birth = HelperFunction.SagitecEncryptAES(Convert.ToString(this.icdoPerson.idtDateofBirth));
            //}
            if (this.icdoPerson.communication_preference_value == busConstant.MAIL)
            {
                if (this.iclbPersonAddress != null)
                {
                    foreach (busPersonAddress objbusPersonAddress in this.iclbPersonAddress)
                    {
                        if (((objbusPersonAddress.icdoPersonAddress.end_date >= DateTime.Now || objbusPersonAddress.icdoPersonAddress.end_date == DateTime.MinValue) &&
                            objbusPersonAddress.icdoPersonAddress.start_date <= DateTime.Now) && objbusPersonAddress.icdoPersonAddress.start_date != objbusPersonAddress.icdoPersonAddress.end_date)
                        {
                            blnCheckActiveAddress = true;
                        }
                    }
                }
            }
            if (!blnCheckActiveAddress)
            {
                if (!string.IsNullOrEmpty(this.icdoPerson.email_address_1))
                {
                    this.icdoPerson.communication_preference_value = busConstant.EMAL;
                }
                else
                {
                    this.icdoPerson.communication_preference_value = string.Empty;
                }
            }
            base.BeforeValidate(aenmPageMode);
        }

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busPersonAccountBeneficiary)
            {
                busPersonAccountBeneficiary lbusPersonAccountBeneficiary = (busPersonAccountBeneficiary)aobjBus;

                lbusPersonAccountBeneficiary.ibusRelationship = new busRelationship { icdoRelationship = new cdoRelationship() };
                lbusPersonAccountBeneficiary.ibusRelationship.icdoRelationship.LoadData(adtrRow);

                lbusPersonAccountBeneficiary.ibusParticipant = new busPerson();
                lbusPersonAccountBeneficiary.ibusParticipant = this;

            }
            else if (aobjBus is busPersonDependent)
            {
                busPersonDependent lbusPersonDependent = (busPersonDependent)aobjBus;
                lbusPersonDependent.ibusPersonDependent = new busPerson { icdoPerson = new cdoPerson() };
                lbusPersonDependent.ibusPersonDependent.icdoPerson.LoadData(adtrRow);
            }
            else if (aobjBus is busPersonBridgeHours)
            {
                busPersonBridgeHours lbusPersonBridgeHours = aobjBus as busPersonBridgeHours;
                lbusPersonBridgeHours.iclbPersonBridgeHoursDetails = new Collection<busPersonBridgeHoursDetail>();

                DataTable ldtblist = busPerson.Select("cdoPerson.LoadBridgingDetails", new object[1] { this.icdoPerson.person_id });
                lbusPersonBridgeHours.iclbPersonBridgeHoursDetails = GetCollection<busPersonBridgeHoursDetail>(ldtblist, "icdoPersonBridgeHoursDetail");
            }
            else if (aobjBus is busPerson)
            {
                busPerson lbusPerson = (aobjBus as busPerson);
                lbusPerson.ibusPersonAddress = new busPersonAddress() { icdoPersonAddress = new cdoPersonAddress() };
                lbusPerson.ibusPersonAddress.icdoPersonAddress.LoadData(adtrRow);

            }

            base.LoadOtherObjects(adtrRow, aobjBus);
        }

        public override void BeforePersistChanges()
        {
            if (this is busPersonOverview)
                return;
            //EncryptDOB();

            base.BeforePersistChanges();

            //Shankar Bug_17 Empty Notes
            if (this.iclbNotes != null && this.iclbNotes.Count > 0)
            {
                if (iarrChangeLog != null && this.iarrChangeLog.Count > 0)
                {
                    foreach (busNotes lbusNotes in this.iclbNotes.Where(item => item.icdoNotes.notes.IsNullOrEmpty()))
                    {
                        if (iarrChangeLog.Contains(lbusNotes.icdoNotes))
                        {
                            iarrChangeLog.Remove(lbusNotes.icdoNotes);
                        }
                    }
                }
                this.iclbNotes = this.iclbNotes.Where(item => item.icdoNotes.notes.IsNotNullOrEmpty()).ToList().ToCollection();
            }
            //end

            //if (this.icdoPerson.person_id > 0)
            //{
            //    if (this.icdoPerson.analyst_id > 0)
            //    {
            //        this.icdoPerson.Update();
            //    }
            //}

            //WI 23234 Secure Document submission
            if (this.icdoPerson.document_upload_flag.IsNullOrEmpty())
                this.icdoPerson.document_upload_flag = busConstant.FLAG_NO;

            if (this.icdoPerson.vip_flag.IsNullOrEmpty())
                this.icdoPerson.vip_flag = busConstant.FLAG_NO;  //Default the VIP flag to No if the use doesnt check it as YES while creating the Person

            //U24074
            if (this.icdoPerson.adverse_interest_flag.IsNullOrEmpty())
                this.icdoPerson.adverse_interest_flag = busConstant.FLAG_NO;

            if (icdoPerson.ihstOldValues.Count > 0 && icdoPerson.ihstOldValues[enmPerson.ssn.ToString()] != null &&
            Convert.ToString(icdoPerson.ihstOldValues[enmPerson.ssn.ToString()]) != icdoPerson.istrSSNNonEncrypted)
            {
                istrOldSSN = Convert.ToString(icdoPerson.ihstOldValues[enmPerson.ssn.ToString()]);
            }

            //PIR RID 65797 - Triming any leading or training spaces from names
            if (icdoPerson.first_name.IsNotNullOrEmpty())
            {
                icdoPerson.first_name = icdoPerson.first_name.Trim();
            }

            if (icdoPerson.last_name.IsNotNullOrEmpty())
            {
                icdoPerson.last_name = icdoPerson.last_name.Trim();
            }

            if (icdoPerson.middle_name.IsNotNullOrEmpty())
            {
                icdoPerson.middle_name = icdoPerson.middle_name.Trim();
            }
            

            //PIR 1050
            #region In case of New Depenedent and Ben if Address same as participant checked thn insert same address as person
            if (istrAddressSameAsParticipant == busConstant.FLAG_YES)
            {
                if (iintSelectedParticipantId > 0)
                {
                    busPerson lbusParticipant = new busPerson { icdoPerson = new cdoPerson() };
                    if (lbusParticipant.FindPerson(iintSelectedParticipantId))
                    {
                        lbusParticipant.LoadPersonAddresss();
                        if (lbusParticipant.iclbPersonAddress != null && lbusParticipant.iclbPersonAddress.Count > 0)
                        {
                            if (lbusParticipant.iclbPersonAddress.Where(item => item.icdoPersonAddress.start_date <= DateTime.Now
                                && (item.icdoPersonAddress.end_date == DateTime.MinValue || item.icdoPersonAddress.end_date > DateTime.Now)).Count() > 0)
                            {
                                ibusParticipantsActiveAddress = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() };

                                if (lbusParticipant.iclbPersonAddress.Where(item => item.icdoPersonAddress.start_date <= DateTime.Now
                                    && (item.icdoPersonAddress.end_date == DateTime.MinValue || item.icdoPersonAddress.end_date > DateTime.Now)).Count() > 1)
                                {
                                    ibusParticipantsActiveAddress = lbusParticipant.iclbPersonAddress.Where(item => item.icdoPersonAddress.start_date <= DateTime.Now
                                         && item.icdoPersonAddress.istrPhysicalAddressType == busConstant.MAILING_ADDRESS
                                         && (item.icdoPersonAddress.end_date == DateTime.MinValue || item.icdoPersonAddress.end_date > DateTime.Now)).FirstOrDefault();
                                }
                                else
                                {
                                    ibusParticipantsActiveAddress = lbusParticipant.iclbPersonAddress.Where(item => item.icdoPersonAddress.start_date <= DateTime.Now
                                        && (item.icdoPersonAddress.end_date == DateTime.MinValue || item.icdoPersonAddress.end_date > DateTime.Now)).FirstOrDefault();
                                }

                                iintParticipantsAddrressId = ibusParticipantsActiveAddress.icdoPersonAddress.address_id;
                                this.ibusPersonAddress.icdoPersonAddress = ibusParticipantsActiveAddress.icdoPersonAddress;
                                this.ibusPersonAddress.icdoPersonAddress.person_id = this.icdoPerson.person_id;
                                this.ibusPersonAddress.icdoPersonAddress.start_date = DateTime.Now;
                                this.ibusPersonAddress.icdoPersonAddress.end_date = DateTime.MinValue;
                                this.ibusPersonAddress.icdoPersonAddress.istrAddSameAsParticipantFlag = busConstant.FLAG_YES;
                                this.ibusPersonAddress.icdoPersonAddress.ienuObjectState = ObjectState.Insert;
                                this.iarrChangeLog.Add(this.ibusPersonAddress.icdoPersonAddress);
                            }

                        }
                    }
                }
            }
            #endregion In case of New Depenedent and Ben if Address same as participant checked thn insert same address as person

            //When we create new address from Beneficiary maintennace screen
            //if same as participant checkbox is checked then need to load participant address and insert a new record. 
            //Need to change the object state to None since in this method it is making blank entry in database
            //PIR 1050
            if (ibusPersonAddress != null && ibusPersonAddress.icdoPersonAddress.istrAddSameAsParticipantFlag == busConstant.FLAG_YES && iintSelectedParticipantId != 0)
            {
                ibusPersonAddress.icdoPersonAddress.istrAddSameAsParticipantFlag = busConstant.FLAG_NO;//PIR 1050
                ibusPersonAddress.icdoPersonAddress.ienuObjectState = ObjectState.None;
            }
            if (icdoPerson.idtDateofBirth != null)
            {
                idecAge = busGlobalFunctions.CalculatePersonAge(icdoPerson.idtDateofBirth, DateTime.Today);
            }

            larrPersonBridgeHoursId.Clear();
            foreach (busPersonBridgeHours lbusPersonBridgeHours in this.iclbPersonBridgeHours)
            {

                if (lbusPersonBridgeHours.iclbPersonBridgeHoursDetails.IsNull())
                {
                    lbusPersonBridgeHours.iclbPersonBridgeHoursDetails = new Collection<busPersonBridgeHoursDetail>();
                }
                if (lbusPersonBridgeHours.iclbPersonBridgeHoursDetails.Count > 0)
                {
                    if (Convert.ToString(lbusPersonBridgeHours.icdoPersonBridgeHours.ihstOldValues[enmPersonBridgeHours.bridge_type_value.ToString()]) != lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_type_value
                        || Convert.ToDecimal(lbusPersonBridgeHours.icdoPersonBridgeHours.ihstOldValues[enmPersonBridgeHours.hours_reported.ToString()]) != lbusPersonBridgeHours.icdoPersonBridgeHours.hours_reported
                        || Convert.ToDateTime(lbusPersonBridgeHours.icdoPersonBridgeHours.ihstOldValues[enmPersonBridgeHours.bridge_start_date.ToString()]) != lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_start_date
                        || Convert.ToDateTime(lbusPersonBridgeHours.icdoPersonBridgeHours.ihstOldValues[enmPersonBridgeHours.bridge_end_date.ToString()]) != lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_end_date)
                    {
                        larrPersonBridgeHoursId.Add(lbusPersonBridgeHours.icdoPersonBridgeHours.person_bridge_id);
                    }
                }
            }


            this.iclbNotes.ForEach(item =>
            {
                if (item.icdoNotes.person_id == 0)
                    item.icdoNotes.person_id = this.icdoPerson.person_id;
                item.icdoNotes.form_id = busConstant.Form_ID;
                item.icdoNotes.form_value = busConstant.PERSON_MAINTAINENCE_FORM;
            });


        }


        public override int PersistChanges()   //ABHSIHEK-- ASK QUESTION TO DURGESH 
        {
            string old_vip_flag_value = String.Empty;
            string old_adverse_interest_flag = String.Empty;
            int iindexPositionOfcdoPerson = 0;

            if (iarrChangeLog.Contains(this.icdoPerson))
                iindexPositionOfcdoPerson = iarrChangeLog.IndexOf(this.icdoPerson);

            if (!(this is busPersonOverview))
            {
                if (iarrChangeLog.Count > 0 && iarrChangeLog[iindexPositionOfcdoPerson].ihstOldValues.Count > 0 && iarrChangeLog[iindexPositionOfcdoPerson].ihstOldValues.ContainsKey(enmPerson.vip_flag.ToString()))
                {
                    old_vip_flag_value = Convert.ToString(iarrChangeLog[iindexPositionOfcdoPerson].ihstOldValues[enmPerson.vip_flag.ToString()]);
                }
                if (iarrChangeLog.Count > 0 && iarrChangeLog[iindexPositionOfcdoPerson].ihstOldValues.Count > 0 && iarrChangeLog[iindexPositionOfcdoPerson].ihstOldValues.ContainsKey(enmPerson.adverse_interest_flag.ToString()))
                {
                    old_adverse_interest_flag = Convert.ToString(iarrChangeLog[iindexPositionOfcdoPerson].ihstOldValues[enmPerson.adverse_interest_flag.ToString()]);
                    if (old_adverse_interest_flag.IsNullOrEmpty())
                        old_adverse_interest_flag = busConstant.FLAG_NO;
                }




                #region DOB change
                DateTime ldtOldDOB = new DateTime();
                if (iarrChangeLog.Count > 0 && iarrChangeLog[iindexPositionOfcdoPerson].ihstOldValues.Count > 0 && iarrChangeLog[iindexPositionOfcdoPerson].ihstOldValues.ContainsKey(enmPerson.date_of_birth.ToString()))
                {
                    string lstrOldDOB = Convert.ToString(iarrChangeLog[iindexPositionOfcdoPerson].ihstOldValues[enmPerson.date_of_birth.ToString()]);
                    if (!string.IsNullOrEmpty(lstrOldDOB))
                    {
                        ldtOldDOB = Convert.ToDateTime(lstrOldDOB);
                        if (ldtOldDOB != this.icdoPerson.idtDateofBirth)
                        {
                            this.icdoPerson.recalculate_vesting_flag = busConstant.FLAG_YES;
                        }
                    }
                    else
                    {
                        this.icdoPerson.recalculate_vesting_flag = busConstant.FLAG_YES;
                    }
                }
                else
                {
                    this.icdoPerson.recalculate_vesting_flag = busConstant.FLAG_NO;
                }
                #endregion

                #region Bridging Hours Added or Changed
                if (!this.iclbPersonBridgeHours.IsNullOrEmpty())
                    this.icdoPerson.recalculate_vesting_flag = busConstant.FLAG_YES;

                #endregion

            }

            int return_value = base.PersistChanges();

            //Ticket : 55015
            DateTime? vip_flag_old_value_modified_date = null;
            if (this.icdoPerson.vip_flag != old_vip_flag_value && iarrChangeLog.Contains(this.icdoPerson))
            {
                cdoVipStatusHistory lVipStatusHistory = new cdoVipStatusHistory();
                DataTable ldtbVIpFalgOldValueModifiedBy = Select("cdoVipStatusHistory.SelectOldValueModifiedBy", new object[1] { this.icdoPerson.person_id });
                if (ldtbVIpFalgOldValueModifiedBy.Rows.Count > 0)
                {
                    lVipStatusHistory.vip_flag_old_value_modified_by = ldtbVIpFalgOldValueModifiedBy.Rows[0]["MODIFIED_BY"].ToString();
                    lVipStatusHistory.vip_flag_old_value_modified_date = Convert.ToDateTime(ldtbVIpFalgOldValueModifiedBy.Rows[0]["MODIFIED_DATE"]);
                }
                else
                {
                    lVipStatusHistory.vip_flag_old_value_modified_by = null;
                    lVipStatusHistory.vip_flag_old_value_modified_date = Convert.ToDateTime(vip_flag_old_value_modified_date);
                }
                lVipStatusHistory.person_id = this.icdoPerson.person_id;
                lVipStatusHistory.vip_flag_new_value = this.icdoPerson.vip_flag;
                lVipStatusHistory.vip_flag_old_value = old_vip_flag_value;
                lVipStatusHistory.message = "Participant updated successfully from OPUS screen";
                lVipStatusHistory.Insert();
            }

            if (this.icdoPerson.vip_flag != old_vip_flag_value && !(this is busPersonOverview))
            {

                ChangeVIPFlag();
            }

            //WI 23234 Secure Document submission
            if (iarrChangeLog.Count > 0 && iarrChangeLog[0].ihstOldValues.Count > 0 && iarrChangeLog[0].ihstOldValues.ContainsKey(enmPerson.document_upload_flag.ToString()))
            {
                string old_document_upload_flag;
                old_document_upload_flag = Convert.ToString(iarrChangeLog[0].ihstOldValues[enmPerson.document_upload_flag.ToString()]);
                if (this.icdoPerson.document_upload_flag != old_document_upload_flag && icdoPerson.document_upload_flag == busConstant.FLAG_YES)
                {
                    //Call webapi to trigger email.
                }

            }
            //U24074
            if (iarrChangeLog.Count > 0 && iarrChangeLog[0].ihstOldValues.Count > 0 && iarrChangeLog[0].ihstOldValues.ContainsKey(enmPerson.adverse_interest_flag.ToString()))
            {
                if (this.icdoPerson.adverse_interest_flag != old_adverse_interest_flag)
                {
                    string lstrNotes = string.Empty;
                    if (this.icdoPerson.adverse_interest_flag == "Y")
                        lstrNotes = "Potential Adverse Interest";
                    else
                        lstrNotes = "Adverse Interest Removed";
                    cdoNotes lcdoNotes = new cdoNotes();
                    lcdoNotes.person_id = this.icdoPerson.person_id;
                    lcdoNotes.form_value = busConstant.PERSON_MAINTAINENCE_FORM;
                    lcdoNotes.notes = lstrNotes;
                    lcdoNotes.created_by = iobjPassInfo.istrUserID;
                    lcdoNotes.created_date = DateTime.Now;
                    lcdoNotes.modified_by = iobjPassInfo.istrUserID;
                    lcdoNotes.modified_date = DateTime.Now;
                    lcdoNotes.Insert();
                    this.iclbNotes = busGlobalFunctions.LoadNotes(this.icdoPerson.person_id, 0, busConstant.PERSON_MAINTAINENCE_FORM);
                }
            }

            return return_value;
        }


        public override void AfterPersistChanges()
        {
            if (this is busPersonOverview)
                return;
            base.AfterPersistChanges();
            // can only be created once person_id is saved..

            //To Generate MPI Person ID Temp:
            if (string.IsNullOrEmpty(this.icdoPerson.mpi_person_id))
            {

                cdoCodeValue lobjcdoCodeValue = HelperUtil.GetCodeValueDetails(52, busConstant.MPID);
                int lintNewPersonID = Convert.ToInt32(lobjcdoCodeValue.data1);
                this.icdoPerson.mpi_person_id = "M" + lintNewPersonID.ToString("D8");
                this.icdoPerson.Update();

                lintNewPersonID += 1;
                lobjcdoCodeValue.data1 = lintNewPersonID.ToString();
                lobjcdoCodeValue.Update();
            }

           
            LoadInitialData();
            if (this.icdoPerson.analyst_id > 0)
            {
                if(this.icdoPerson.analyst_id != iintCurrentYear)
                {
                    DBFunction.DBNonQuery("cdoPerson.InsertPersonsCaseAnalyst",new object[3] { this.icdoPerson.person_id, this.icdoPerson.analyst_id, iobjPassInfo.istrUserID },
                                       iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                }
               
            }

            try
            {

                foreach (busPersonBridgeHours lbusPersonBridgeHours in this.iclbPersonBridgeHours)
                {

                    if (lbusPersonBridgeHours.iclbPersonBridgeHoursDetails.IsNull())
                    {
                        lbusPersonBridgeHours.iclbPersonBridgeHoursDetails = new Collection<busPersonBridgeHoursDetail>();
                    }
                    if (lbusPersonBridgeHours.iclbPersonBridgeHoursDetails.Count > 0)
                    {
                        if (larrPersonBridgeHoursId.Count > 0)
                        {
                            foreach (int lintPersonbridgeID in larrPersonBridgeHoursId)
                            {
                                if (lbusPersonBridgeHours.icdoPersonBridgeHours.person_bridge_id == lintPersonbridgeID)
                                {
                                    foreach (busPersonBridgeHoursDetail lbusPersonBridgeHoursDetail in lbusPersonBridgeHours.iclbPersonBridgeHoursDetails)
                                    {
                                        lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.Delete();
                                    }

                                    AddPersonBridgingDetails(lbusPersonBridgeHours);
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (busPersonBridgeHoursDetail lbusPersonBridgeHoursDetail in lbusPersonBridgeHours.iclbPersonBridgeHoursDetails)
                        {
                            lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.Delete();
                        }
                        AddPersonBridgingDetails(lbusPersonBridgeHours);
                    }
                }
                //TC 14 : IAP Allocation
                //Code to heal the IAP forfeiture in case of Bridging
                if (this.iclbPersonBridgeHours != null && this.iclbPersonBridgeHours.Count > 0)
                    NegateIAPForfeitureAmounts();
            }
            catch
            {

            }

            busSSNMerge lbusSSNMerge = new busSSNMerge() { icdoPerson = new cdoPerson() };

            int checkMergePerson = (int)DBFunction.DBExecuteScalar("cdoSsnMergeHistory.CheckIsPersonAlreadyMerged", new object[1] { this.icdoPerson.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            bool IsPersonMerged = false;
            bool IsPossibleDuplicates = false;
            if (checkMergePerson > 0)
                IsPersonMerged = true;
            DataTable ldtbPossibleSSNs = new DataTable();
            if (this.icdoPerson.first_name.IsNotNullOrEmpty() && this.icdoPerson.last_name.IsNotNullOrEmpty() && this.icdoPerson.istrSSNNonEncrypted.IsNotNullOrEmpty()
                && this.icdoPerson.date_of_birth != DateTime.MinValue)
            {
                ldtbPossibleSSNs = Select("cdoPerson.GetPossibleDuplicateSSNs", new object[5] { this.icdoPerson.first_name , this.icdoPerson.last_name,
                            this.icdoPerson.date_of_birth,this.icdoPerson.istrSSNNonEncrypted, this.icdoPerson.person_id});
            }
            if (ldtbPossibleSSNs != null && ldtbPossibleSSNs.Rows.Count > 0)
            {

                IsPossibleDuplicates = true;
            }

            if (IsPossibleDuplicates || lbusSSNMerge.CheckSSNExistsInHistory(this) || IsPersonMerged)
            {
                Hashtable lhstRequestParams = new Hashtable();

                lhstRequestParams.Add("PERSON_ID", this.icdoPerson.person_id);
                lhstRequestParams.Add("MPI_PERSON_ID", this.icdoPerson.mpi_person_id);

                busWorkflowHelper.InitializeWorkflowIfNotExists(busConstant.PROCESS_SSN_MERGE, this.icdoPerson.person_id, 0, 0, lhstRequestParams);
            }


            //PIR 1050
            if (iobjPassInfo.ienmPageMode == utlPageMode.New)
            {
                if (iintSelectedParticipantId > 0 && istrAddressSameAsParticipant != busConstant.FLAG_YES)
                {
                    //PIR RID 63893 added code in condition to check ibusPersonAddress.icdoPersonAddress.address_id > 0
                    if (ibusPersonAddress != null && ibusPersonAddress.icdoPersonAddress.address_id > 0 
                        && (ibusPersonAddress.iclcPersonAddressChklist == null || ibusPersonAddress.iclcPersonAddressChklist.Count() == 0))
                    {
                        cdoPersonAddressChklist lPersonAddressChklist = new cdoPersonAddressChklist();
                        lPersonAddressChklist.address_id = ibusPersonAddress.icdoPersonAddress.address_id;
                        lPersonAddressChklist.address_type_id = 6013;
                        lPersonAddressChklist.address_type_value = busConstant.MAIL;
                        lPersonAddressChklist.Insert();
                    }
                    else if (ibusPersonAddress != null && ibusPersonAddress.icdoPersonAddress.address_id > 0 
                        && ibusPersonAddress.iclcPersonAddressChklist.Where(t => t.address_type_value == busConstant.MAIL).Count() > 0)  //PIR RID 63893 safe check added for 'ibusPersonAddress != null' in else condition 
                    {
                        if (ibusPersonAddress.iclcPersonAddressChklist[0].address_id == 0)
                        {
                            ibusPersonAddress.iclcPersonAddressChklist[0].address_id = ibusPersonAddress.icdoPersonAddress.address_id;
                            ibusPersonAddress.iclcPersonAddressChklist[0].address_type_value = busConstant.MAIL;
                            ibusPersonAddress.iclcPersonAddressChklist[0].Update();
                        }
                    }
                }
            }


            #region In case of New Depenedent and Ben if Address same as participant checked thn insert same address as person
            if (istrAddressSameAsParticipant == busConstant.FLAG_YES)
            {
                if (iintSelectedParticipantId > 0)
                {
                    this.LoadPersonAddresss();
                    if (iclbPersonAddress != null && iclbPersonAddress.Count > 0)
                    {
                        foreach (busPersonAddress lbusPersonAddress in iclbPersonAddress)
                        {
                            lbusPersonAddress.LoadPersonAddressChklists();

                            foreach (cdoPersonAddressChklist lbusPersonAddressChklist in lbusPersonAddress.iclcPersonAddressChklist)
                            {
                                lbusPersonAddressChklist.Delete();
                            }

                            lbusPersonAddress.icdoPersonAddress.Delete();
                        }
                    }

                    busPersonAddress lbusDepenedentAddress = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() };
                    busPerson lbusParticipant = new busPerson { icdoPerson = new cdoPerson() };
                    if (lbusParticipant.FindPerson(iintSelectedParticipantId))
                    {
                        lbusParticipant.LoadPersonAddresss();
                        if (lbusParticipant.iclbPersonAddress != null && lbusParticipant.iclbPersonAddress.Count > 0)
                        {
                            if (lbusParticipant.iclbPersonAddress.Where(item => item.icdoPersonAddress.start_date <= DateTime.Now
                                && (item.icdoPersonAddress.end_date == DateTime.MinValue || item.icdoPersonAddress.end_date > DateTime.Now)).Count() > 0)
                            {
                                ibusParticipantsActiveAddress = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() };

                                if (lbusParticipant.iclbPersonAddress.Where(item => item.icdoPersonAddress.start_date <= DateTime.Now
                                    && (item.icdoPersonAddress.end_date == DateTime.MinValue || item.icdoPersonAddress.end_date > DateTime.Now)).Count() > 1)
                                {
                                    ibusParticipantsActiveAddress = lbusParticipant.iclbPersonAddress.Where(item => item.icdoPersonAddress.start_date <= DateTime.Now
                                         && item.icdoPersonAddress.istrPhysicalAddressType == busConstant.MAILING_ADDRESS
                                         && (item.icdoPersonAddress.end_date == DateTime.MinValue || item.icdoPersonAddress.end_date > DateTime.Now)).FirstOrDefault();
                                }
                                else
                                {
                                    ibusParticipantsActiveAddress = lbusParticipant.iclbPersonAddress.Where(item => item.icdoPersonAddress.start_date <= DateTime.Now
                                        && (item.icdoPersonAddress.end_date == DateTime.MinValue || item.icdoPersonAddress.end_date > DateTime.Now)).FirstOrDefault();
                                }


                                ibusParticipantsActiveAddress.iclcPersonAddressChklist = new utlCollection<cdoPersonAddressChklist>();
                                ibusParticipantsActiveAddress.LoadPersonAddressChklists();

                                iintParticipantsAddrressId = ibusParticipantsActiveAddress.icdoPersonAddress.address_id;
                                lbusDepenedentAddress.icdoPersonAddress = ibusParticipantsActiveAddress.icdoPersonAddress;
                                lbusDepenedentAddress.icdoPersonAddress.person_id = this.icdoPerson.person_id;
                                lbusDepenedentAddress.icdoPersonAddress.start_date = DateTime.Now;
                                lbusDepenedentAddress.icdoPersonAddress.end_date = DateTime.MinValue;
                                lbusDepenedentAddress.icdoPersonAddress.istrAddSameAsParticipantFlag = busConstant.FLAG_YES;
                                lbusDepenedentAddress.icdoPersonAddress.Insert();
                            }

                        }
                    }


                    if (lbusDepenedentAddress.icdoPersonAddress.address_id > 0)
                    {
                        int lintNewAddressId = lbusDepenedentAddress.icdoPersonAddress.address_id;

                        if (ibusParticipantsActiveAddress.iclcPersonAddressChklist != null && ibusParticipantsActiveAddress.iclcPersonAddressChklist.Count > 0)
                        {
                            foreach (cdoPersonAddressChklist lcdoPersonAddressChklist in ibusParticipantsActiveAddress.iclcPersonAddressChklist)
                            {
                                cdoPersonAddressChklist lPersonAddressChklist = new cdoPersonAddressChklist();
                                lPersonAddressChklist.address_id = lintNewAddressId;
                                lPersonAddressChklist.address_type_id = 6013;
                                lPersonAddressChklist.address_type_value = lcdoPersonAddressChklist.address_type_value;
                                lPersonAddressChklist.Insert();
                            }
                        }
                    }
                }
            }
            #endregion In case of New Depenedent and Ben if Address same as participant checked thn insert same address as person

            
            if ((iobjPassInfo.iconFramework).Database == "MPI" || (iobjPassInfo.iconFramework).Database == "MPIPHP")
            {
                FindPerson(this.icdoPerson.mpi_person_id);
                return;
            }
            
            if (iobjPassInfo.ienmPageMode == utlPageMode.Update)
            {

                try
                {

                    var transactionOptions = new TransactionOptions();
                    transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                    transactionOptions.Timeout = TransactionManager.MaximumTimeout;

                    // Distributed Transcation Co-ordinator should be SWITCHED ON on the server
                    using (TransactionScope ds = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
                    {

                        // decommissioning demographics informations, since HEDB is retiring.
                        //if (lobjLegacyPassInfoHEDB.IsNull())
                        //{
                        //    lobjLegacyPassInfoHEDB = new utlPassInfo();
                        //}
                        //if (lobjLegacy.IsNull())
                        //{
                        //    lobjLegacy = new utlPassInfo();
                        //}

                        //lobjLegacyPassInfoHEDB.iconFramework = DBFunction.GetDBConnection("HELegacy");
                        //lobjLegacy.iconFramework = DBFunction.GetDBConnection("LookupDB");



                        //if (lobjLegacyPassInfoHEDB.iconFramework != null)
                        //{
                        //    string lstrMPIPersonId = this.icdoPerson.mpi_person_id;
                        //    string lstrPersonSSN = this.icdoPerson.istrSSNNonEncrypted;
                        //    string lstrParticipantMPIID = string.Empty;
                        //    string lstrRelationshipType = string.Empty;
                        //    //Ticket : 55015
                        //    string lstrVipFlag = this.icdoPerson.vip_flag;
                        //    string lUpperlstrVipFlag = lstrVipFlag.ToUpper();
                        //    bool lboolVipFlag = false;
                        //    if (lUpperlstrVipFlag == "Y")
                        //    {
                        //        lboolVipFlag = true;
                        //    }

                        //    try
                        //    {
                        //        string strQuery = "select * from person where ssn = (select ssn from Eligibility_PID_Reference where PID = '" + this.icdoPerson.mpi_person_id + "')";
                        //        DataTable ldtbResult = DBFunction.DBSelect(strQuery, lobjLegacyPassInfoHEDB.iconFramework);
                        //        if (ldtbResult.Rows.Count == 0)
                        //        {
                        //            return;
                        //        }

                        //    }
                        //    catch
                        //    {

                        //    }

                        //    int CountDependent = (int)DBFunction.DBExecuteScalar("cdoPerson.ChechPersonIsDependent", new object[1] { this.icdoPerson.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                        //    int CountBeneficiary = (int)DBFunction.DBExecuteScalar("cdoPerson.CheckPersonIsBeneficiary", new object[1] { this.icdoPerson.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                        //    if (CountDependent > 0)
                        //    {
                        //        lstrRelationshipType = "D";
                        //    }
                        //    else if (CountBeneficiary > 0)
                        //    {
                        //        lstrRelationshipType = "B";
                        //    }
                        //    else
                        //    {
                        //        lstrRelationshipType = null;
                        //    }

                        //    string lstrFirstName = this.icdoPerson.first_name;
                        //    string lstrMiddleName = this.icdoPerson.middle_name;
                        //    string lstrlastName = this.icdoPerson.last_name;
                        //    string lstrGender = this.icdoPerson.gender_value;
                        //    DateTime lstrDOB = DateTime.MinValue;

                        //    if (lstrFirstName.IsNotNullOrEmpty())
                        //    {
                        //        lstrFirstName = lstrFirstName.ToUpper();
                        //    }

                        //    if (lstrMiddleName.IsNotNullOrEmpty())
                        //    {
                        //        lstrMiddleName = lstrMiddleName.ToUpper();
                        //    }

                        //    if (lstrlastName.IsNotNullOrEmpty())
                        //    {
                        //        lstrlastName = lstrlastName.ToUpper();
                        //    }


                        //    lstrDOB = this.icdoPerson.idtDateofBirth;

                        //    DateTime lstrDOD = DateTime.MinValue;

                        //    if (this.icdoPerson.date_of_death == DateTime.MinValue)
                        //    {
                        //        DataTable ldtblGetDateOfDeath = Select("cdoDeathNotification.GetDateOfDeathInProgress", new object[1] { this.icdoPerson.person_id });
                        //        if (ldtblGetDateOfDeath != null && ldtblGetDateOfDeath.Rows.Count > 0
                        //            && Convert.ToString(ldtblGetDateOfDeath.Rows[0][enmDeathNotification.date_of_death.ToString().ToUpper()]).IsNotNullOrEmpty())
                        //        {
                        //            lstrDOD = Convert.ToDateTime(ldtblGetDateOfDeath.Rows[0][enmDeathNotification.date_of_death.ToString().ToUpper()]);
                        //        }
                        //    }
                        //    else
                        //    {
                        //        lstrDOD = this.icdoPerson.date_of_death;
                        //    }


                        //    string lstrHomePhone = this.icdoPerson.home_phone_no;
                        //    string lstrCellPhone = this.icdoPerson.cell_phone_no;
                        //    string lstrFax = this.icdoPerson.fax_no;
                        //    string lstrEmail = this.icdoPerson.email_address_1;
                        //    string lstrCreatedBy = this.icdoPerson.modified_by;
                        //    if (iobjPassInfo.ienmPageMode == utlPageMode.Update)
                        //    {

                        //        //if (!String.IsNullOrEmpty(lstrPersonSSN))
                        //        //{
                        //        Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();


                        //        IDbDataParameter lobjParameter1 = DBFunction.GetDBParameter();
                        //        lobjParameter1.ParameterName = "@PID";
                        //        lobjParameter1.DbType = DbType.String;
                        //        lobjParameter1.Value = lstrMPIPersonId.ToLower();
                        //        lcolParameters.Add(lobjParameter1);

                        //        IDbDataParameter lobjParameter2 = DBFunction.GetDBParameter();
                        //        lobjParameter2.ParameterName = "@SSN";
                        //        lobjParameter2.DbType = DbType.String;

                        //        if (lstrPersonSSN.IsNullOrEmpty())
                        //            lobjParameter2.Value = DBNull.Value;
                        //        else
                        //            lobjParameter2.Value = lstrPersonSSN.ToLower();
                        //        lcolParameters.Add(lobjParameter2);

                        //        IDbDataParameter lobjParameter3 = DBFunction.GetDBParameter();
                        //        lobjParameter3.ParameterName = "@ParticipantPID";
                        //        lobjParameter3.DbType = DbType.String;
                        //        lobjParameter3.Value = DBNull.Value;    //Need to change            
                        //        lcolParameters.Add(lobjParameter3);

                        //        IDbDataParameter lobjParameter4 = DBFunction.GetDBParameter();
                        //        lobjParameter4.ParameterName = "@EntityTypeCode";
                        //        lobjParameter4.DbType = DbType.String;
                        //        lobjParameter4.Value = "P";                 //for now we will always use Person
                        //        lcolParameters.Add(lobjParameter4);

                        //        IDbDataParameter lobjParameter5 = DBFunction.GetDBParameter();
                        //        lobjParameter5.ParameterName = "@RelationType";
                        //        lobjParameter5.DbType = DbType.String;
                        //        lobjParameter5.Value = lstrRelationshipType;                //Need to change
                        //        lcolParameters.Add(lobjParameter5);

                        //        IDbDataParameter lobjParameter6 = DBFunction.GetDBParameter();
                        //        lobjParameter6.ParameterName = "@FirstName";
                        //        lobjParameter6.DbType = DbType.String;
                        //        lobjParameter6.Value = lstrFirstName;
                        //        lcolParameters.Add(lobjParameter6);

                        //        IDbDataParameter lobjParameter7 = DBFunction.GetDBParameter();
                        //        lobjParameter7.ParameterName = "@MiddleName";
                        //        lobjParameter7.DbType = DbType.String;
                        //        lobjParameter7.Value = lstrMiddleName;
                        //        lcolParameters.Add(lobjParameter7);

                        //        IDbDataParameter lobjParameter8 = DBFunction.GetDBParameter();
                        //        lobjParameter8.ParameterName = "@LastName";
                        //        lobjParameter8.DbType = DbType.String;
                        //        lobjParameter8.Value = lstrlastName;
                        //        lcolParameters.Add(lobjParameter8);

                        //        IDbDataParameter lobjParameter9 = DBFunction.GetDBParameter();
                        //        lobjParameter9.ParameterName = "@Gender";
                        //        lobjParameter9.DbType = DbType.String;
                        //        lobjParameter9.Value = lstrGender;
                        //        lcolParameters.Add(lobjParameter9);


                        //        IDbDataParameter lobjParameter10 = DBFunction.GetDBParameter();
                        //        lobjParameter10.ParameterName = "@DateOfBirth";
                        //        lobjParameter10.DbType = DbType.DateTime;
                        //        if (lstrDOB != DateTime.MinValue)
                        //        {
                        //            lobjParameter10.Value = lstrDOB;
                        //        }
                        //        else
                        //        {
                        //            lobjParameter10.Value = DBNull.Value;
                        //        }
                        //        lcolParameters.Add(lobjParameter10);


                        //        IDbDataParameter lobjParameter11 = DBFunction.GetDBParameter();
                        //        lobjParameter11.ParameterName = "@DateOfDeath";
                        //        lobjParameter11.DbType = DbType.DateTime;

                        //        if (lstrDOD != DateTime.MinValue)
                        //        {
                        //            lobjParameter11.Value = lstrDOD;
                        //        }
                        //        else
                        //        {
                        //            lobjParameter11.Value = DBNull.Value;
                        //        }
                        //        lcolParameters.Add(lobjParameter11);

                        //        IDbDataParameter lobjParameter12 = DBFunction.GetDBParameter();
                        //        lobjParameter12.ParameterName = "@HomePhone";
                        //        lobjParameter12.DbType = DbType.String;
                        //        lobjParameter12.Value = lstrHomePhone;
                        //        lcolParameters.Add(lobjParameter12);

                        //        IDbDataParameter lobjParameter13 = DBFunction.GetDBParameter();
                        //        lobjParameter13.ParameterName = "@CellPhone";
                        //        lobjParameter13.DbType = DbType.String;
                        //        lobjParameter13.Value = lstrCellPhone;
                        //        lcolParameters.Add(lobjParameter13);

                        //        IDbDataParameter lobjParameter14 = DBFunction.GetDBParameter();
                        //        lobjParameter14.ParameterName = "@Fax";
                        //        lobjParameter14.DbType = DbType.String;
                        //        lobjParameter14.Value = lstrFax;
                        //        lcolParameters.Add(lobjParameter14);

                        //        IDbDataParameter lobjParameter15 = DBFunction.GetDBParameter();
                        //        lobjParameter15.ParameterName = "@Email";
                        //        lobjParameter15.DbType = DbType.String;
                        //        lobjParameter15.Value = lstrEmail;
                        //        lcolParameters.Add(lobjParameter15);

                        //        IDbDataParameter lobjParameter16 = DBFunction.GetDBParameter();
                        //        lobjParameter16.ParameterName = "@AuditUser";
                        //        lobjParameter16.DbType = DbType.String;
                        //        lobjParameter16.Value = lstrCreatedBy;
                        //        lcolParameters.Add(lobjParameter16);
                        //        //Ticket : 55015
                        //        IDbDataParameter lobjParameter17 = DBFunction.GetDBParameter();
                        //        lobjParameter17.ParameterName = "@VipFlag";
                        //        lobjParameter17.DbType = DbType.Boolean;
                        //        lobjParameter17.Value = lboolVipFlag;
                        //        lcolParameters.Add(lobjParameter17);

                        //        //try
                        //        //{
                        //        //lobjPassInfo1.BeginTransaction();
                        //        DBFunction.DBExecuteProcedure("USP_PID_Person_UPD", lcolParameters, lobjLegacyPassInfoHEDB.iconFramework, lobjLegacyPassInfoHEDB.itrnFramework);
                        //        // lobjPassInfo1.Commit();

                        //        //}
                        //        //catch (Exception e)
                        //        //{
                        //        //    lobjPassInfo1.Rollback();
                        //        //    throw e;
                        //        //}
                        //        //finally
                        //        //{
                        //        //    lobjPassInfo1.iconFramework.Close();
                        //        //}

                        //        //}
                        //    }
                        //}

                        if (istrOldSSN.IsNotNullOrEmpty() && istrOldSSN != icdoPerson.ssn)
                        {
                          //  UpdateHoursOnImagingSystem(istrOldSSN, icdoPerson.ssn);
                        }

                        ds.Complete();
                    }

                }
                catch (Exception e)
                {
                    if (iobjPassInfo.itrnFramework.IsNotNull())
                    {
                        utlError lError = new utlError();
                        if (e.Message.IsNotNullOrEmpty())
                            lError.istrErrorMessage = e.Message.ToString();
                        else
                            lError.istrErrorMessage = "Error Occured/All transactions have been RollBacked";
                        throw (e);
                    }
                }
                finally
                {
                    // decommissioning demographics informations, since HEDB is retiring.
                    // if (lobjLegacyPassInfoHEDB.IsNotNull() && lobjLegacyPassInfoHEDB.itrnFramework.IsNotNull())
                    //if (lobjLegacyPassInfoHEDB.IsNotNull() && lobjLegacyPassInfoHEDB.iconFramework != null &&
                    //    lobjLegacyPassInfoHEDB.iconFramework.State == ConnectionState.Open)
                    //{
                    //    lobjLegacyPassInfoHEDB.iconFramework.Close();
                    //    lobjLegacyPassInfoHEDB.iconFramework.Dispose();
                    //}

                    //// if (lobjLegacy.IsNotNull() && lobjLegacy.itrnFramework.IsNotNull())
                    //if (lobjLegacy.IsNotNull() && lobjLegacy.iconFramework != null
                    //    && lobjLegacy.iconFramework.State == ConnectionState.Open)
                    //{
                    //    lobjLegacy.iconFramework.Close();
                    //    lobjLegacy.iconFramework.Dispose();
                    //}
                }

                FindPerson(this.icdoPerson.mpi_person_id);
            }
        }

        private void UpdateHoursOnImagingSystem(string astrOldSSN, string astrNewSSN)
        {



            if (lobjLegacy.iconFramework != null)
            {
                #region Merge SSN on HEADB

                Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();

                IDbDataParameter lobjParameter1 = DBFunction.GetDBParameter();
                lobjParameter1.ParameterName = "@OldSSN";
                lobjParameter1.DbType = DbType.String;
                if (astrOldSSN.IsNull())
                    astrOldSSN = "";
                lobjParameter1.Value = astrOldSSN;
                lcolParameters.Add(lobjParameter1);

                IDbDataParameter lobjParameter2 = DBFunction.GetDBParameter();
                lobjParameter2.ParameterName = "@CorrectSSN";
                lobjParameter2.DbType = DbType.String;
                lobjParameter2.Value = astrNewSSN;
                lcolParameters.Add(lobjParameter2);

                DBFunction.DBExecuteProcedure("sp_Merge_SSNs", lcolParameters, lobjLegacy.iconFramework, lobjLegacy.itrnFramework);

                #endregion
            }
        }



        public override ArrayList ValidateNew(Hashtable ahstParam)
        {
            ArrayList larrErrors = new ArrayList();

            //PIR 525
            if (ahstParam.ContainsKey("abln_address_flag") && Convert.ToString(ahstParam["abln_address_flag"]).IsNotNullOrEmpty()
                && Convert.ToString(ahstParam["abln_address_flag"]) == busConstant.FLAG_YES
                && Convert.ToString(ahstParam["aint_person_id"]).IsNotNullOrEmpty())
            {
                this.icdoPerson = new cdoPerson();
                FindPerson(Convert.ToInt32(ahstParam["aint_person_id"]));
                iclbPersonAddress = new Collection<busPersonAddress>();
                this.LoadPersonAddresss();

                if (iclbPersonAddress != null && iclbPersonAddress.Count > 0
                    && iclbPersonAddress.Where(item => item.icdoPersonAddress.start_date.Date == DateTime.Today.Date
                        && item.icdoPersonAddress.end_date.Date == DateTime.MinValue).Count() > 0)
                {
                    utlError lobjError = null;
                    lobjError = AddError(0, "Please update existing active address");
                    larrErrors.Add(lobjError);
                }

                return larrErrors;
            }

            busPersonBeneficiary lbusPersonBeneficiary = new busPersonBeneficiary();
            lbusPersonBeneficiary.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lbusPersonBeneficiary.ibusPerson.icdoPerson.person_id = Convert.ToInt32(ahstParam["aint_participant_id"]);
            Collection<cdoPlan> lcolPlan = lbusPersonBeneficiary.GetPlanValues("PART");

            if (lcolPlan == null && this.iblnParticipant == busConstant.YES)
            {
                utlError lobjError = null;
                lobjError = AddError(5092, "");
                larrErrors.Add(lobjError);
            }

            if (this.iblnParticipant != busConstant.YES && (this.iblnBeneficiary == busConstant.YES || this.iblnAlternatePayee == busConstant.YES))
            {
                DataTable ldtbPersonList = new DataTable();
                if (this.iblnAlternatePayee == busConstant.YES)
                {
                    ldtbPersonList = Select("cdoRelationship.GetAlternatePayeePartDetails", new object[1] { Convert.ToInt32(ahstParam["aint_participant_id"]) });
                }
                else if (this.iblnBeneficiary == busConstant.YES)
                {
                    ldtbPersonList = Select("cdoRelationship.GetSurvivorsPartDetails", new object[1] { Convert.ToInt32(ahstParam["aint_participant_id"]) });
                }

                if (ldtbPersonList == null || ldtbPersonList.Rows.Count == 0)
                {
                    utlError lobjError = null;
                    lobjError = AddError(1174, "");
                    larrErrors.Add(lobjError);
                }
            }
            return larrErrors;
        }


        public override void LoadPersonAddresss()
        {
            this.ibusPersonAddressForCorr = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() };
            base.LoadPersonAddresss();
            foreach (busPersonAddress lbusPersonAddress in iclbPersonAddress)
            {
                lbusPersonAddress.LoadPersonAddressChklists();
                lbusPersonAddress.SetPhysivcalMailingFlagValue(lbusPersonAddress.iclcPersonAddressChklist, lbusPersonAddress);
                lbusPersonAddress.LoadStateDescription();
            }

        }

        public override void SetParentKey(Sagitec.DataObjects.doBase aobjBase)
        {
            if (aobjBase is cdoPersonAddressChklist)
            {
                (aobjBase as cdoPersonAddressChklist).address_id = this.ibusPersonAddress.icdoPersonAddress.address_id;
            }
        }

        public override void LoadCorresProperties(string astrTemplateName)
        {
            base.LoadCorresProperties(astrTemplateName);
            LoadCorrespondenceProperties();

            DateTime ldtCurrentDate = System.DateTime.Now;
            istrCurrentDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtCurrentDate);
            iintCurrentYear = ldtCurrentDate.Year;

            if (astrTemplateName == busConstant.WORK_HISTORY_REQUEST || astrTemplateName == busConstant.SUBPOENA_RESPONSE_CERTIFICATION_OF_RECORDS || astrTemplateName == busConstant.MSS.WORK_HISTORY_REQUEST_MSS)
            {
                iclbPersonWorkHistory = new Collection<cdoDummyWorkData>();

                //if (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date != DateTime.MinValue)
                //{
                //    DateTime lRetirementDt = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
                //    istrRetirementDate = busGlobalFunctions.ConvertDateIntoDifFormat(lRetirementDt);

                DataTable ldtbPensionCredits = new DataTable();

                utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
                string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

                SqlParameter[] parameters = new SqlParameter[1];
                SqlParameter param1 = new SqlParameter("@SSN", DbType.String);

                param1.Value = icdoPerson.istrSSNNonEncrypted;
                parameters[0] = param1;

                DataTable ldtPersonWorkHistory = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetWeeklyWorkData", astrLegacyDBConnetion, null, parameters);
                if (ldtPersonWorkHistory != null && ldtPersonWorkHistory.Rows.Count > 0)
                {
                    //Ticket#118341
                    ldtPersonWorkHistory.DefaultView.Sort = "PlanYear";
                    ldtPersonWorkHistory = ldtPersonWorkHistory.DefaultView.ToTable();
                    
                    iclbPersonWorkHistory = cdoDummyWorkData.GetCollection<cdoDummyWorkData>(ldtPersonWorkHistory);
                                        
                    ////Ticket#118341
                    //iclbPersonWorkHistory.ToList().Sort((x, y) => DateTime.Compare(x.BeginingDate, y.BeginingDate));


                }
             
            }


            if (astrTemplateName == busConstant.RETIREE_HEALTH_PACKET || astrTemplateName == busConstant.RETIREE_HEALTH_PLAN_ANNUAL_WORK_HISTORY_SUMMARY)
            {
                ibusPersonOverview = new busPersonOverview { icdoPerson = new cdoPerson() };
                if (ibusPersonOverview.FindPerson(icdoPerson.person_id))
                {
                    ibusPersonOverview.LoadWorkHistory();

                    ibusPersonOverview.GetRetireeHealthHours();
                    if(ibusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI != null)
                    {
                        if (ibusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Count > 0)
                        {
                            ldecHealthQualifiedHours = (from item in ibusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI select item.idecTotalHealthHours).Sum();
                            ldecHealthQualifiedYears = (from item in ibusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI orderby item.year select item.iintHealthCount).Last();

                        }

                       if(astrTemplateName == busConstant.RETIREE_HEALTH_PACKET)
                        {
                            //this.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                            //this.LoadBenefitApplication();
                            //this.GetPriorDates();

                            DateTime ldtRetire = this.icdoPerson.retirement_health_date;//Convert.ToDateTime(this.iclbBenefitApplication.Select(i => i.icdoBenefitApplication.retirement_date).FirstOrDefault());
                            DateTime ldtRtrDt = ldtRetire.AddMonths(-2);
                            ldtRtrDt = ldtRtrDt.AddDays(-1);
                            istrSixtyDaysPriorDate = Convert.ToString(ldtRtrDt);
                            istrSixtyDaysPriorDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtRtrDt);

                        }

                    }
                   
                }
            }

           


            #region RETR-0020
            if (astrTemplateName == busConstant.MISSING_DOCUMENT_REQUEST)
            {
                int lintProcessID = busConstant.ZERO_INT;
                switch (iobjPassInfo.istrFormName)
                {
                    case busConstant.PAYEE_ACCOUNT_MAINTENANCE:
                        lintProcessID = busConstant.Payee_Account_WORKFLOW_PROCESS_ID;
                        istrBenefitTypeDescription = "Payee Account";
                        break;
                    case busConstant.PARTICIPANT_BENEFICIARY_MAINTENANCE:
                        lintProcessID = 2;
                        istrBenefitTypeDescription = busGlobalFunctions.GetScreenName(busConstant.PARTICIPANT_BENEFICIARY_MAINTENANCE);
                        break;
                    case "wfmBenefitCalculationPreRetirementDeathMaintenance":
                        lintProcessID = 11;
                        istrBenefitTypeDescription = busGlobalFunctions.GetScreenName("wfmBenefitCalculationPreRetirementDeathMaintenance");
                        break;
                    case "wfmDisabiltyBenefitCalculationMaintenance":
                        lintProcessID = 99;
                        istrBenefitTypeDescription = busGlobalFunctions.GetScreenName("wfmDisabiltyBenefitCalculationMaintenance");
                        break;
                    case "wfmQDROApplicationMaintenance":
                        lintProcessID = 1;
                        istrBenefitTypeDescription = "DRO Application";
                        break;
                    case busConstant.PERSON_MAINTENANCE:
                        lintProcessID = 4;
                        istrBenefitTypeDescription = busGlobalFunctions.GetScreenName(busConstant.PERSON_MAINTENANCE);
                        break;
                    case "wfmBenefitCalculationRetirementMaintenance":
                        lintProcessID = 5;
                        istrBenefitTypeDescription = busGlobalFunctions.GetScreenName("wfmBenefitCalculationRetirementMaintenance");
                        break;
                    case "wfmBenefitCalculationWithdrawalMaintenance":
                        lintProcessID = 10;
                        istrBenefitTypeDescription = busGlobalFunctions.GetScreenName("wfmBenefitCalculationWithdrawalMaintenance");
                        break;
                    case "wfmBenefitCalculationPostRetirementDeathMaintenance":
                        lintProcessID = 13;
                        istrBenefitTypeDescription = busGlobalFunctions.GetScreenName("wfmBenefitCalculationPostRetirementDeathMaintenance");
                        break;
                    case "wfmPersonContactMaintenance":
                        lintProcessID = 7;
                        istrBenefitTypeDescription = busGlobalFunctions.GetScreenName("wfmPersonContactMaintenance");
                        break;
                    case "wfmPersonOverviewMaintenance":
                        lintProcessID = 4;
                        istrBenefitTypeDescription = busGlobalFunctions.GetScreenName("wfmPersonOverviewMaintenance");
                        break;
                    case "wfmDeathNotificationMaintenance":
                        lintProcessID = 8;
                        istrBenefitTypeDescription = busGlobalFunctions.GetScreenName("wfmDeathNotificationMaintenance");
                        break;
                    case "wfmRetirementApplicationMaintenance":
                        lintProcessID = 5;
                        istrBenefitTypeDescription = busGlobalFunctions.GetScreenName("wfmRetirementApplicationMaintenance");
                        break;

                    case "wfmDisabilityApplicationMaintenance":
                        lintProcessID = 99;
                        istrBenefitTypeDescription = busGlobalFunctions.GetScreenName("wfmDisabilityApplicationMaintenance");
                        break;

                    case "wfmWithdrawalApplicationMaintenance":
                        lintProcessID = 10;
                        istrBenefitTypeDescription = busGlobalFunctions.GetScreenName("wfmWithdrawalApplicationMaintenance");
                        break;

                    case "wfmDeathPreRetirementMaintenance":
                        lintProcessID = 11;
                        istrBenefitTypeDescription = busGlobalFunctions.GetScreenName("wfmDeathPreRetirementMaintenance");
                        break;

                    case "wfmPersonDependentMaintenance":
                        lintProcessID = 3;
                        istrBenefitTypeDescription = busGlobalFunctions.GetScreenName("wfmPersonDependentMaintenance");
                        break;

                    case "wfmQDROCalculationMaintenance":
                        lintProcessID = 1;
                        istrBenefitTypeDescription = "QDRO Calculation";
                        break;
                }

                iclbDocumentsReceived = new Collection<busBenefitApplication>();
                DataTable ldtDocumentsReceived = busBase.Select("cdoBenefitApplication.GetDocumentsReceivedList", new object[2] { this.icdoPerson.person_id, lintProcessID });
                if (ldtDocumentsReceived.Rows.Count > 0 && ldtDocumentsReceived.Rows[0][0] != DBNull.Value)
                {
                    foreach (DataRow ldrDocumentReceived in ldtDocumentsReceived.AsEnumerable())
                    {
                        busBenefitApplication lobjbusBenefitApplicationDocRecieved = new busBenefitApplication() { icdoBenefitApplication = new cdoBenefitApplication() };

                        lobjbusBenefitApplicationDocRecieved.icdoBenefitApplication.istrDocument = ldrDocumentReceived["DOCUMENT_NAME"].ToString();

                        iclbDocumentsReceived.Add(lobjbusBenefitApplicationDocRecieved);
                    }
                }

                iclbDocumentsPending = new Collection<busBenefitApplication>();
                DataTable ldtDocumentsPending = busBase.Select("cdoBenefitApplication.GetDocumentsPendingList", new object[2] { this.icdoPerson.person_id, lintProcessID });
                if (ldtDocumentsPending.Rows.Count > 0 && ldtDocumentsPending.Rows[0][0] != DBNull.Value)
                {
                    foreach (DataRow ldrDocumentPending in ldtDocumentsPending.AsEnumerable())
                    {
                        busBenefitApplication lobjbusBenefitApplicationDocPending = new busBenefitApplication() { icdoBenefitApplication = new cdoBenefitApplication() };

                        lobjbusBenefitApplicationDocPending.icdoBenefitApplication.istrDocument = ldrDocumentPending["DOCUMENT_NAME"].ToString();

                        iclbDocumentsPending.Add(lobjbusBenefitApplicationDocPending);
                    }
                }
            }
            #endregion

            if (astrTemplateName == busConstant.PENSION_AND_IAP_VERIFICATION)
            {
                busCalculation lbusCalc = new busCalculation();
                ibusAnnualBenefitSummaryOverview = new busAnnualBenefitSummaryOverview();
                idtPeriodEndingDate = lbusCalc.GetLastWorkingDate(icdoPerson.ssn);
                if (ibusAnnualBenefitSummaryOverview.FindPerson(icdoPerson.person_id))
                {
                    ibusAnnualBenefitSummaryOverview.LoadWorkHistory();
                    //Ticket #104959
                    if (CheckIfPersonHasMPI())
                    {
                        busPersonAccount lbusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                        lbusPersonAccount = this.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault();
                        busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };
                        lbusPersonAccountEligibility = lbusPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(lbusPersonAccount.icdoPersonAccount.person_account_id);
                        if (lbusPersonAccountEligibility.icdoPersonAccountEligibility != null && lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date != null && lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date != DateTime.MinValue)
                        {
                            istrIsVestedMPI = busConstant.FLAG_YES;
                        }
                    }
                    if (ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI != null && ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Count > 0)
                    {
                        iintVestedYearsMinus5 = ibusAnnualBenefitSummaryOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().vested_years_count - 5;
                        if (iintVestedYearsMinus5 < 0) iintVestedYearsMinus5 = (iintVestedYearsMinus5 * -1);
                    }
                    else
                        iintVestedYearsMinus5 = 5;

                    DataTable ldtbIAPBalance = busBase.Select("cdoPerson.GetIAPBalanceSUM", new object[2] { this.icdoPerson.person_id, DateTime.Now.Year });
                    if (ldtbIAPBalance.Rows.Count > 0 && ldtbIAPBalance.Rows[0][0] != DBNull.Value)
                    {
                        idecIAPBalance = Convert.ToDecimal(ldtbIAPBalance.Rows[0]["IAP_BALANCE_AMOUNT"].ToString());
                        DateTime ldtEffectiveDate = Convert.ToDateTime(ldtbIAPBalance.Rows[0]["EFFECTIVE_DATE"].ToString());
                        iintYearEnd = ldtEffectiveDate.Year;
                    }

                    //Ticket#74011
                    if (icdoPerson.health_eligible_flag != null)
                    {
                        if (icdoPerson.health_eligible_flag == busConstant.Flag_Yes)
                        {
                            iainthealthEligibleFlag = 1;
                        }
                        else
                        {
                            iainthealthEligibleFlag = 2;
                        }

                    }
                       

                  
                       
                    
                }
            }
        }

       

        public void LoadCorrespondenceProperties()
        {
            DateTime ldtDob = icdoPerson.idtDateofBirth;
            ldtDob = ldtDob.AddYears(70);
            ldtDob = ldtDob.AddMonths(6);
            iintYear = Convert.ToInt32(ldtDob.Year);
            //PIR - 1031
            istrMinDistriDate = busGlobalFunctions.CalculateMinDistributionDate(icdoPerson.idtDateofBirth, icdoPerson.idtVestingDate);
            istrFebDate = busGlobalFunctions.CalculateFebDate(icdoPerson.idtDateofBirth);

            //For PER-0006(RASHMI)
            DateTime ldtMarchDt = new DateTime(DateTime.Now.Year, 02, 28); //PIR - 1031 Changed the Date from March 15 to Feb 28
            istrMarchDt = busGlobalFunctions.ConvertDateIntoDifFormat(ldtMarchDt);

            if (this.icdoPerson.marital_status_value != null)
            {
                if (this.icdoPerson.marital_status_value != "M")
                    istrBenefitOption = "Life Annuity";
                else
                    istrBenefitOption = "Qualified Joint & 50% Survivor Annuity";
            }
        }

        public void GetPriorDates()
        {
            if (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date != DateTime.MinValue)
            {
                DateTime ldtRetire = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
                DateTime ldtRtrDt = ldtRetire.AddMonths(-2);
                ldtRtrDt = ldtRtrDt.AddDays(-1);
                this.ibusBenefitApplication.istrSixtyDaysPriorDate = Convert.ToString(ldtRtrDt);
                this.ibusBenefitApplication.istrSixtyDaysPriorDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtRtrDt);

                DateTime ldtRetire1 = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
                DateTime ldtRtrDt1 = ldtRetire1.AddMonths(-1);
                DateTime ldtOneDayPriorRtmt = ldtRetire.AddDays(-1);
                ldtRtrDt1 = ldtRtrDt1.AddDays(-1);
                this.ibusBenefitApplication.istrThirtyDaysPriorDate = Convert.ToString(ldtRtrDt1);
                this.ibusBenefitApplication.istrThirtyDaysPriorDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtRtrDt1);

                this.ibusBenefitApplication.dtSixtyDaysPriorDate = ldtRetire.AddDays(-60);
                this.ibusBenefitApplication.dtThirtyDaysPriorDate = ldtRetire.AddDays(-30);

                this.ibusBenefitApplication.istrOneDayPriorRtDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtOneDayPriorRtmt);

            }

        }
        public bool FindPerson(string astrPersonMpiId)
        {
            bool lblnResult = false;
            if (icdoPerson == null)
            {
                icdoPerson = new cdoPerson();
            }

            DataTable ldtPerson = busBase.Select<cdoPerson>(new string[1] { enmPerson.mpi_person_id.ToString() }, new object[1] { astrPersonMpiId }, null, null);
            if (ldtPerson != null && ldtPerson.Rows.Count > 0)
            {
                icdoPerson.LoadData(ldtPerson.Rows[0]);
                lblnResult = true;
            }

            return lblnResult;
        }

        //Website and Mobile App
        public bool FindPersonSSN(string astrSSN)
        {
            bool lblnResult = false;
            if (icdoPerson == null)
            {
                icdoPerson = new cdoPerson();
            }

            DataTable ldtPerson = busBase.Select<cdoPerson>(new string[1] { enmPerson.ssn.ToString() }, new object[1] { astrSSN }, null, null);
            if (ldtPerson != null && ldtPerson.Rows.Count > 0)
            {
                icdoPerson.LoadData(ldtPerson.Rows[0]);
                lblnResult = true;
            }

            return lblnResult;
        }

        #endregion

        #region Private Methods
        //Code Modified Abhishek 
        //TO-DO  We need to ask MPI how VIP uncheck will work since a beneficiary could also be a Participant himself.
        private void ChangeVIPFlag()
        {
            DataTable ldtbBeneficariesDependants = Select("cdoPerson.GetVIPforBeneficaryandDependent", new object[1] { icdoPerson.person_id });
            //Put a or condition to check if role is VIPmanager  then save to SGT_Person
            if (ldtbBeneficariesDependants.Rows.Count > 0)
            {
                Collection<busPerson> lclbBenDepList = new Collection<busPerson>();
                lclbBenDepList = GetCollection<busPerson>(ldtbBeneficariesDependants, "icdoPerson");

                lclbBenDepList.ForEach(item =>
                                            {
                                                item.icdoPerson.vip_flag = this.icdoPerson.vip_flag;
                                                item.icdoPerson.Update();
                                            });
            }
        }

        private bool PersonAddressEntered()
        {
            bool lblnAddressEntered = false;
            //PIR RID 63893 added check to check address line 1 exists
            if (this.ibusPersonAddress.icdoPersonAddress.addr_line_1.IsNotNullOrEmpty())
            {
                if (!this.iarrChangeLog.Contains(this.ibusPersonAddress.icdoPersonAddress))
                {
                    this.iarrChangeLog.Add(this.ibusPersonAddress.icdoPersonAddress);
                }
                lblnAddressEntered = true;
                if (this.ibusPersonAddress.ibusRelationship == null)
                {
                    this.ibusPersonAddress.ibusRelationship = new busRelationship { icdoRelationship = new cdoRelationship() };
                    if (this.istrAddressSameAsParticipant == busConstant.FLAG_YES)
                    {
                        this.ibusPersonAddress.ibusRelationship.icdoRelationship.addr_same_as_participant_flag = busConstant.FLAG_YES;
                    }
                    else
                    {
                        this.ibusPersonAddress.ibusRelationship.icdoRelationship.addr_same_as_participant_flag = busConstant.FLAG_NO;
                    }
                }
            }
            else
            {
                if (this.ibusPersonAddress.iclcPersonAddressChklist != null && this.ibusPersonAddress.iclcPersonAddressChklist.Count > 0)
                {
                    foreach (cdoPersonAddressChklist lcdoPersonAddressChklist in this.ibusPersonAddress.iclcPersonAddressChklist.ToList())
                    {
                        if (iarrChangeLog != null && this.iarrChangeLog.Count > 0)
                        {
                            if (iarrChangeLog.Contains(lcdoPersonAddressChklist))
                            {
                                iarrChangeLog.Remove(lcdoPersonAddressChklist);
                            }
                        }
                        this.ibusPersonAddress.iclcPersonAddressChklist.Remove(lcdoPersonAddressChklist);
                    }
                }

                if (iarrChangeLog.Contains(this.ibusPersonAddress.icdoPersonAddress))
                {
                    this.iarrChangeLog.Remove(this.ibusPersonAddress.icdoPersonAddress);
                }      
            }
            return lblnAddressEntered;
        }

        public void LoadParticipantAndalternatePayeeAddress(busPerson lbusPerson, string astrTemplateName, busPerson ibusAlternatePayee)
        {
            foreach (busPersonContact lobjContactforpartipant in lbusPerson.iclbPersonContact)  // In participant we look for first contact addresses.
            {
                if (lobjContactforpartipant.icdoPersonContact.effective_start_date <= DateTime.Now &&
       ((lobjContactforpartipant.icdoPersonContact.effective_end_date == DateTime.MinValue) || DateTime.Now < lobjContactforpartipant.icdoPersonContact.effective_end_date) && lobjContactforpartipant.icdoPersonContact.correspondence_addr_flag == busConstant.FLAG_YES)
                {
                    lbusPerson.ibusPersonAddressForCorr = lbusPerson.LoadCorAddressFromContact(lobjContactforpartipant);
                }
            }
            if (lbusPerson.ibusPersonAddressForCorr == null)  // If no contact address found take its own active address
            {
                lbusPerson.LoadCorrAddress();
            }
            busPerson lbusAlternatePayee = ibusAlternatePayee;
            lbusAlternatePayee.LoadInitialData();
            lbusAlternatePayee.LoadPersonAddresss();
            lbusAlternatePayee.LoadPersonContacts();
            foreach (busPersonContact objPersonContact in lbusAlternatePayee.iclbPersonContact)  // In alternate we look for first contact addresses.
            {
                if (objPersonContact.icdoPersonContact.effective_start_date <= DateTime.Now &&
       ((objPersonContact.icdoPersonContact.effective_end_date == DateTime.MinValue) || DateTime.Now < objPersonContact.icdoPersonContact.effective_end_date) && objPersonContact.icdoPersonContact.correspondence_addr_flag == busConstant.FLAG_YES)
                {
                    lbusPerson.ibusAlternatePayeeAddressForCorr = lbusAlternatePayee.LoadCorAddressFromContact(objPersonContact);
                }
            }
            if (lbusPerson.ibusAlternatePayeeAddressForCorr == null)             // If no contact address found take its own active address
            {
                foreach (busPersonAddress objbusPersonAddress in lbusAlternatePayee.iclbPersonAddress)
                {
                    if (objbusPersonAddress.icdoPersonAddress.start_date <= DateTime.Now &&
           ((objbusPersonAddress.icdoPersonAddress.end_date == DateTime.MinValue) || DateTime.Now < objbusPersonAddress.icdoPersonAddress.end_date))
                    {
                        lbusPerson.ibusAlternatePayeeAddressForCorr = objbusPersonAddress;
                    }
                }
            }
        }

        public bool CheckPrefixAgainstGender()
        {
            if (this.icdoPerson.gender_value == busConstant.MALE)
            {
                if (this.icdoPerson.name_prefix_value == busConstant.MISS || this.icdoPerson.name_prefix_value == busConstant.MRS)
                {
                    return true;
                }
            }
            else if (this.icdoPerson.gender_value == busConstant.FEMALE)
            {
                if (this.icdoPerson.name_prefix_value == busConstant.MR)
                {
                    return true;
                }
            }
            return false;
        }

        public void EncryptDOB()
        {
            try
            {
                DataTable ldt = busBase.Select("cdoPerson.Lookup", new Object[] { });
                if (ldt.Rows.Count > 0)
                {
                    foreach (DataRow ldtr in ldt.Rows)
                    {
                        cdoPerson lcdo = new cdoPerson();
                        lcdo.LoadData(ldtr);
                        if (!string.IsNullOrEmpty(Convert.ToString(lcdo.date_of_birth)))
                        {
                            lcdo.date_of_birth = Convert.ToDateTime(lcdo.date_of_birth);
                            lcdo.Update();
                        }
                    }
                }

            }
            catch
            {

            }

        }


        #endregion

        #region For MPI Workhistory Plan Visibility in Grid
        public bool CheckIfPersonHasLocal161()
        {
            if (this.iclbPersonAccount.IsNotNull() && this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_161).Count() > 0)
                return true;
            else
                return false;
        }
        public bool CheckIfPersonHasLocal666()
        {
            if (this.iclbPersonAccount.IsNotNull() && this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_666).Count() > 0)
                return true;
            else
                return false;
        }
        public bool CheckIfPersonHasLocal600()
        {

            if (this.iclbPersonAccount.IsNotNull() && this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_600).Count() > 0)
                return true;
            else
                return false;
        }
        public bool CheckIfPersonHasLocal700()
        {

            if (this.iclbPersonAccount.IsNotNull() && this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).Count() > 0)
                return true;
            else
                return false;
        }
        public bool CheckIfPersonHasLocal52()
        {

            if (this.iclbPersonAccount.IsNotNull() && this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_52).Count() > 0)
                return true;
            else
                return false;
        }
        public bool CheckIfPersonHasMPI()
        {

            if (this.iclbPersonAccount.IsNotNull() && this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).Count() > 0)
                return true;
            else
                return false;
        }
        public bool CheckIfPersonHasIAP()
        {

            if (this.iclbPersonAccount.IsNotNull() && this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).Count() > 0)
                return true;
            else
                return false;
        }
        #endregion


        public bool VisibilityOnGenerateAnnualBtn()
        {
            int lintCheckForAnnualStatement = (int)DBFunction.DBExecuteScalar("cdoDataExtractionBatchInfo.CheckForAnnualFlag", new object[1] { this.icdoPerson.person_id },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            int lintCheckVisbility = (int)DBFunction.DBExecuteScalar("cdoPerson.Check4AnnualStmtButtonVisibility", new object[1] { this.icdoPerson.person_id },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            if (lintCheckForAnnualStatement > 0 && lintCheckVisbility > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Method to negate the forfeiture amounts for IAP when bridging is done
        /// </summary>
        public void NegateIAPForfeitureAmounts()
        {
            busBenefitApplication lobjApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            cdoDummyWorkData lcdoWorkData = new cdoDummyWorkData();

            lobjApplication.ibusPerson = new busPerson();
            lobjApplication.ibusPerson.FindPerson(this.icdoPerson.person_id);
            lobjApplication.ibusPerson.LoadPersonAccountsByPlanId(busConstant.IAP_PLAN_ID);
            if (lobjApplication.ibusPerson.iclbPersonAccount.Count > 0)
            {
                lobjApplication.LoadandProcessWorkHistory_ForAllPlans();
                DataTable ldtForfeitedContribution = Select("cdoPersonAccountRetirementContribution.GetIAPRetirementContributionForNegatingForfeiture", new object[1] { lobjApplication.ibusPerson.iclbPersonAccount[0].icdoPersonAccount.person_account_id });

                foreach (DataRow ldrContrb in ldtForfeitedContribution.Rows)
                {
                    lcdoWorkData = lobjApplication.aclbPersonWorkHistory_IAP.Where(o => o.year == Convert.ToInt32(ldrContrb["computational_year"])).FirstOrDefault();
                    if (lcdoWorkData == null || (lcdoWorkData != null && (lcdoWorkData.comments.IsNullOrEmpty() || !lcdoWorkData.comments.Contains(busConstant.FORFEITURE_COMMENT))))
                    {
                        PostIntoContribution(ldrContrb);
                    }
                }
            }
        }

        /// <summary>
        /// method to reverse the bridging when bridge hours are deleted
        /// </summary>
        public void ReverseBrigingOnDelete()
        {
            busBenefitApplication lobjApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            cdoDummyWorkData lcdoWorkData = new cdoDummyWorkData();

            lobjApplication.ibusPerson = new busPerson();
            lobjApplication.ibusPerson.FindPerson(this.icdoPerson.person_id);
            lobjApplication.ibusPerson.LoadPersonAccountsByPlanId(busConstant.IAP_PLAN_ID);
            if (lobjApplication.ibusPerson.iclbPersonAccount.Count > 0)
            {
                lobjApplication.LoadandProcessWorkHistory_ForAllPlans();
                var lvarForfeitureRecords = lobjApplication.aclbPersonWorkHistory_IAP.Where(o => o.comments.IsNotNullOrEmpty() && o.comments.Contains(busConstant.FORFEITURE_COMMENT));
                foreach (cdoDummyWorkData lcdoForfeiture in lvarForfeitureRecords)
                {
                    DataTable ldtBridgedContributions = Select("cdoPersonAccountRetirementContribution.GetIAPRetirementContributionToNegateBridging",
                        new object[2] { lobjApplication.ibusPerson.iclbPersonAccount[0].icdoPersonAccount.person_account_id, lcdoForfeiture.year });
                    foreach (DataRow ldrContrb in ldtBridgedContributions.Rows)
                    {
                        PostIntoContribution(ldrContrb);
                    }
                }
            }
        }

        /// <summary>
        /// Method to post the amounts into contribution table
        /// </summary>
        /// <param name="adrContrb">Contribution amounts</param>
        private void PostIntoContribution(DataRow adrContrb)
        {
            busPersonAccountRetirementContribution lbusContribution;
            int lintComputationYear = Convert.ToInt32(adrContrb["computational_year"]);
            if (adrContrb["alloc1"] != DBNull.Value && Convert.ToDecimal(adrContrb["alloc1"]) != busConstant.ZERO_DECIMAL)
            {
                lbusContribution = new busPersonAccountRetirementContribution();
                lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(adrContrb["person_account_id"]),
                                                                            busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
                                                                            DateTime.Now,
                                                                            lintComputationYear,
                                                                            adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(adrContrb["alloc1"]),
                                                                            astrTransactionType: busConstant.RCTransactionTypeBridging,
                                                                            astrContributionType: busConstant.RCContributionTypeAllocation1);
            }

            if (adrContrb["alloc2"] != DBNull.Value && Convert.ToDecimal(adrContrb["alloc2"]) != busConstant.ZERO_DECIMAL)
            {
                lbusContribution = new busPersonAccountRetirementContribution();
                lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(adrContrb["person_account_id"]),
                                                                            busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
                                                                            DateTime.Now,
                                                                            lintComputationYear,
                                                                            adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(adrContrb["alloc2"]),
                                                                            astrTransactionType: busConstant.RCTransactionTypeBridging,
                                                                            astrContributionType: busConstant.RCContributionTypeAllocation2);
            }

            if (adrContrb["alloc2_invt"] != DBNull.Value && Convert.ToDecimal(adrContrb["alloc2_invt"]) != busConstant.ZERO_DECIMAL)
            {
                lbusContribution = new busPersonAccountRetirementContribution();
                lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(adrContrb["person_account_id"]),
                                                                            busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
                                                                            DateTime.Now,
                                                                            lintComputationYear,
                                                                            adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(adrContrb["alloc2_invt"]),
                                                                            astrTransactionType: busConstant.RCTransactionTypeBridging,
                                                                            astrContributionType: busConstant.RCContributionTypeAllocation2,
                                                                            astrContributionSubtype: busConstant.RCContributionSubTypeInvestment);
            }

            if (adrContrb["alloc2_frft"] != DBNull.Value && Convert.ToDecimal(adrContrb["alloc2_frft"]) != busConstant.ZERO_DECIMAL)
            {
                lbusContribution = new busPersonAccountRetirementContribution();
                lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(adrContrb["person_account_id"]),
                                                                            busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
                                                                            DateTime.Now,
                                                                            lintComputationYear,
                                                                            adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(adrContrb["alloc2_frft"]),
                                                                            astrTransactionType: busConstant.RCTransactionTypeBridging,
                                                                            astrContributionType: busConstant.RCContributionTypeAllocation2,
                                                                            astrContributionSubtype: busConstant.RCContributionSubTypeForfeited);
            }

            if (adrContrb["alloc3"] != DBNull.Value && Convert.ToDecimal(adrContrb["alloc3"]) != busConstant.ZERO_DECIMAL)
            {
                lbusContribution = new busPersonAccountRetirementContribution();
                lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(adrContrb["person_account_id"]),
                                                                            busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
                                                                            DateTime.Now,
                                                                            lintComputationYear,
                                                                            adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(adrContrb["alloc3"]),
                                                                            astrTransactionType: busConstant.RCTransactionTypeBridging,
                                                                            astrContributionType: busConstant.RCContributionTypeAllocation3);
            }

            if (adrContrb["alloc4"] != DBNull.Value && Convert.ToDecimal(adrContrb["alloc4"]) != busConstant.ZERO_DECIMAL)
            {
                lbusContribution = new busPersonAccountRetirementContribution();
                lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(adrContrb["person_account_id"]),
                                                                            busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
                                                                            DateTime.Now,
                                                                            lintComputationYear,
                                                                            adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(adrContrb["alloc4"]),
                                                                            astrTransactionType: busConstant.RCTransactionTypeBridging,
                                                                            astrContributionType: busConstant.RCContributionTypeAllocation4);
            }

            if (adrContrb["alloc4_frft"] != DBNull.Value && Convert.ToDecimal(adrContrb["alloc4_frft"]) != busConstant.ZERO_DECIMAL)
            {
                lbusContribution = new busPersonAccountRetirementContribution();
                lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(adrContrb["person_account_id"]),
                                                                            busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
                                                                            DateTime.Now,
                                                                            lintComputationYear,
                                                                            adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(adrContrb["alloc4_frft"]),
                                                                            astrTransactionType: busConstant.RCTransactionTypeBridging,
                                                                            astrContributionType: busConstant.RCContributionTypeAllocation4,
                                                                            astrContributionSubtype: busConstant.RCContributionSubTypeForfeited);
            }

            if (adrContrb["alloc4_invt"] != DBNull.Value && Convert.ToDecimal(adrContrb["alloc4_invt"]) != busConstant.ZERO_DECIMAL)
            {
                lbusContribution = new busPersonAccountRetirementContribution();
                lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(adrContrb["person_account_id"]),
                                                                            busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
                                                                            DateTime.Now,
                                                                            lintComputationYear,
                                                                            adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(adrContrb["alloc4_invt"]),
                                                                            astrTransactionType: busConstant.RCTransactionTypeBridging,
                                                                            astrContributionType: busConstant.RCContributionTypeAllocation4,
                                                                            astrContributionSubtype: busConstant.RCContributionSubTypeInvestment);
            }

            //if (adrContrb["alloc5_affl"] != DBNull.Value && Convert.ToDecimal(adrContrb["alloc5_affl"]) != busConstant.ZERO_DECIMAL)
            //{
            //    lbusContribution = new busPersonAccountRetirementContribution();
            //    lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(adrContrb["person_account_id"]),
            //                                                                busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
            //                                                                DateTime.Now,
            //                                                                lintComputationYear,
            //                                                                adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(adrContrb["alloc5_affl"]),
            //                                                                astrTransactionType: busConstant.RCTransactionTypeBridging,
            //                                                                astrContributionType: busConstant.RCContributionTypeAllocation5,
            //                                                                astrContributionSubtype: busConstant.RCContributionSubTypeAffiliates);
            //}

            //if (adrContrb["alloc5_both"] != DBNull.Value && Convert.ToDecimal(adrContrb["alloc5_both"]) != busConstant.ZERO_DECIMAL)
            //{
            //    lbusContribution = new busPersonAccountRetirementContribution();
            //    lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(adrContrb["person_account_id"]),
            //                                                                busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
            //                                                                DateTime.Now,
            //                                                                lintComputationYear,
            //                                                                adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(adrContrb["alloc5_both"]),
            //                                                                astrTransactionType: busConstant.RCTransactionTypeBridging,
            //                                                                astrContributionType: busConstant.RCContributionTypeAllocation5,
            //                                                                astrContributionSubtype: busConstant.RCContributionSubTypeBoth);
            //}

            //if (adrContrb["alloc5_nonaffl"] != DBNull.Value && Convert.ToDecimal(adrContrb["alloc5_nonaffl"]) != busConstant.ZERO_DECIMAL)
            //{
            //    lbusContribution = new busPersonAccountRetirementContribution();
            //    lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(adrContrb["person_account_id"]),
            //                                                                busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
            //                                                                DateTime.Now,
            //                                                                lintComputationYear,
            //                                                                adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(adrContrb["alloc5_nonaffl"]),
            //                                                                astrTransactionType: busConstant.RCTransactionTypeBridging,
            //                                                                astrContributionType: busConstant.RCContributionTypeAllocation5,
            //                                                                astrContributionSubtype: busConstant.RCContributionSubTypeNonAffiliates);
            //}

            if (adrContrb["alloc5"] != DBNull.Value && Convert.ToDecimal(adrContrb["alloc5"]) != busConstant.ZERO_DECIMAL)
            {
                lbusContribution = new busPersonAccountRetirementContribution();
                lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(adrContrb["person_account_id"]),
                                                                            busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
                                                                            DateTime.Now,
                                                                            lintComputationYear,
                                                                            adecIAPBalanceAmount: busConstant.ZERO_DECIMAL - Convert.ToDecimal(adrContrb["alloc5"]),
                                                                            astrTransactionType: busConstant.RCTransactionTypeBridging,
                                                                            astrContributionType: busConstant.RCContributionTypeAllocation5);
            }

            if (adrContrb["L52ALLOC1"] != DBNull.Value && Convert.ToDecimal(adrContrb["L52ALLOC1"]) != busConstant.ZERO_DECIMAL)
            {
                lbusContribution = new busPersonAccountRetirementContribution();
                lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(adrContrb["person_account_id"]),
                                                                            busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
                                                                            DateTime.Now,
                                                                            lintComputationYear,
                                                                            adec52SplAccountBalance: busConstant.ZERO_DECIMAL - Convert.ToDecimal(adrContrb["L52ALLOC1"]),
                                                                            astrTransactionType: busConstant.RCTransactionTypeBridging,
                                                                            astrContributionType: busConstant.RCContributionTypeAllocation1);
            }

            if (adrContrb["L161ALLOC1"] != DBNull.Value && Convert.ToDecimal(adrContrb["L161ALLOC1"]) != busConstant.ZERO_DECIMAL)
            {
                lbusContribution = new busPersonAccountRetirementContribution();
                lbusContribution.InsertPersonAccountRetirementContirbution(Convert.ToInt32(adrContrb["person_account_id"]),
                                                                            busGlobalFunctions.GetLastDateOfComputationYear(lintComputationYear),
                                                                            DateTime.Now,
                                                                            lintComputationYear,
                                                                            adec161SplAccountBalance: busConstant.ZERO_DECIMAL - Convert.ToDecimal(adrContrb["L161ALLOC1"]),
                                                                           astrTransactionType: busConstant.RCTransactionTypeBridging,
                                                                            astrContributionType: busConstant.RCContributionTypeAllocation1);
            }
        }

        #region Get PersonID from SSN
        public int GetPersonIDFromSSN(string astrSSN)
        {
            int lintPersonid = 0;
            string lstrEncryptedSSN = astrSSN;
            DataTable ldtbPerosnID = busBase.Select("cdoPerson.GetPersonIDFromSSN", new object[1] { lstrEncryptedSSN });
            if (ldtbPerosnID.Rows.Count > 0)
            {
                lintPersonid = Convert.ToInt32(ldtbPerosnID.Rows[0][0]);
            }
            return lintPersonid;
        }
        #endregion

        public void LoadRetirementContributionsForEE(int aintPersonID)
        {
            this.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();
            DataTable ldtbEEContributions = busBase.Select("cdoPersonAccountRetirementContribution.LoadEEContributions", new object[2] { aintPersonID, busConstant.MPIPP_PLAN_ID });
            if (ldtbEEContributions.Rows.Count > 0)
            {
                iclbPersonAccountRetirementContribution = GetCollection<busPersonAccountRetirementContribution>(ldtbEEContributions, "icdoPersonAccountRetirementContribution");
            }
            this.idecEEContributionAmount = iclbPersonAccountRetirementContribution.Sum(item => item.icdoPersonAccountRetirementContribution.ee_contribution_amount);
            this.idecEEContributionInterest = iclbPersonAccountRetirementContribution.Sum(item => item.icdoPersonAccountRetirementContribution.ee_int_amount);


        }

        public void LoadRetirementContributions(int aintPersonId, DateTime adtTransactionDate)
        {
            this.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();
            DataTable ldtbEEContributions = busBase.Select("cdoPersonAccountRetirementContribution.LoadRetContributionByPersonIdAndTransactionDate",
                                            new object[2] { adtTransactionDate, aintPersonId });

            foreach (DataRow ldr in ldtbEEContributions.Rows)
            {
                if (ldr[enmPersonAccountRetirementContribution.ee_contribution_amount.ToString()].ToString().IsNotNullOrEmpty())
                    this.idecEEContributionAmount = Convert.ToDecimal(ldr[enmPersonAccountRetirementContribution.ee_contribution_amount.ToString()]);

                if (ldr[enmPersonAccountRetirementContribution.ee_int_amount.ToString()].ToString().IsNotNullOrEmpty())
                    this.idecEEContributionInterest = Convert.ToDecimal(ldr[enmPersonAccountRetirementContribution.ee_int_amount.ToString()]);

                if (ldr[enmPersonAccountRetirementContribution.uvhp_amount.ToString()].ToString().IsNotNullOrEmpty())
                    this.idecUVHPContributionAmount = Convert.ToDecimal(ldr[enmPersonAccountRetirementContribution.uvhp_amount.ToString()]);

                if (ldr[enmPersonAccountRetirementContribution.uvhp_int_amount.ToString()].ToString().IsNotNullOrEmpty())
                    this.idecUVHPContributionInterest = Convert.ToDecimal(ldr[enmPersonAccountRetirementContribution.uvhp_int_amount.ToString()]);
            }
        }

        public void LoadRetirementContributionsForIAP(int aintPersonAccountID, DateTime adtTransactionDate)
        {
            //this.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();
            //DataTable ldtbIAPContribution = busBase.Select("cdoPersonAccountRetirementContribution.GetRetirementContributionbyAccountId",new object[1] { aintPersonAccountID });
            //if (ldtbIAPContribution.Rows.Count > 0)
            //{
            //    iclbPersonAccountRetirementContribution = GetCollection<busPersonAccountRetirementContribution>(ldtbIAPContribution, "icdoPersonAccountRetirementContribution");
            //}
            //this.idecIAPContributionAmount = iclbPersonAccountRetirementContribution.Sum(item => item.icdoPersonAccountRetirementContribution.iap_balance_amount);
            //this.idecLocal52SpecialAcctBalanceAmt = iclbPersonAccountRetirementContribution.Sum(item => item.icdoPersonAccountRetirementContribution.local52_special_acct_bal_amount);
            //this.idecLocal161SpecialAcctBalanceAmt = iclbPersonAccountRetirementContribution.Sum(item=>item.icdoPersonAccountRetirementContribution.local161_special_acct_bal_amount);


            this.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();
            DataTable ldtbIAPContribution = busBase.Select("cdoPersonAccountRetirementContribution.LoadRetContributionByPersonAcntIdAndDateTilLastYear", new object[2] { aintPersonAccountID, adtTransactionDate });
            if (ldtbIAPContribution.Rows.Count > 0)
            {
                iclbPersonAccountRetirementContribution = GetCollection<busPersonAccountRetirementContribution>(ldtbIAPContribution, "icdoPersonAccountRetirementContribution");
            }
            this.idecIAPContributionAmount = iclbPersonAccountRetirementContribution.Sum(item => item.icdoPersonAccountRetirementContribution.iap_balance_amount);
            this.idecLocal52SpecialAcctBalanceAmt = iclbPersonAccountRetirementContribution.Sum(item => item.icdoPersonAccountRetirementContribution.local52_special_acct_bal_amount);
            this.idecLocal161SpecialAcctBalanceAmt = iclbPersonAccountRetirementContribution.Sum(item => item.icdoPersonAccountRetirementContribution.local161_special_acct_bal_amount);

        }


        public void LoadTotalEEUVHPAndIAPContributions(int aintPersonID)
        {
            DataTable ldtbTotalContribution = busBase.Select("cdoPersonAccountRetirementContribution.GetTotalUVHPEEandIAPContribns", new object[1] { aintPersonID });
            foreach (DataRow ldr in ldtbTotalContribution.Rows)
            {
                if (!ldr[enmPersonAccountRetirementContribution.ee_contribution_amount.ToString()].ToString().IsNullOrEmpty())
                    this.idecEEContributionAmount = Convert.ToDecimal(ldr[enmPersonAccountRetirementContribution.ee_contribution_amount.ToString()]);
                if (!ldr[enmPersonAccountRetirementContribution.ee_int_amount.ToString()].ToString().IsNullOrEmpty())
                    this.idecEEContributionInterest = Convert.ToDecimal(ldr[enmPersonAccountRetirementContribution.ee_int_amount.ToString()]);
                if (!ldr[enmPersonAccountRetirementContribution.uvhp_amount.ToString()].ToString().IsNullOrEmpty())
                    this.idecUVHPContributionAmount = Convert.ToDecimal(ldr[enmPersonAccountRetirementContribution.uvhp_amount.ToString()]);
                if (!ldr[enmPersonAccountRetirementContribution.uvhp_int_amount.ToString()].ToString().IsNullOrEmpty())
                    this.idecUVHPContributionInterest = Convert.ToDecimal(ldr[enmPersonAccountRetirementContribution.uvhp_int_amount.ToString()]);
                if (!ldr[enmPersonAccountRetirementContribution.iap_balance_amount.ToString()].ToString().IsNullOrEmpty())
                    this.idecIAPContributionAmount = Convert.ToDecimal(ldr[enmPersonAccountRetirementContribution.iap_balance_amount.ToString()]);
            }

        }

        //PIR 525
        public bool CheckOpenAddressWithTodaysStartDate()
        {
            if (iclbPersonAddress != null && iclbPersonAddress.Count > 0
                && iclbPersonAddress.Where(item => item.icdoPersonAddress.end_date.Date == DateTime.MinValue.Date && item.icdoPersonAddress.start_date.Date == DateTime.Today.Date).Count() > 0)
            {
                return true;
            }
            return false;
        }

        public void LoadPersonSuspendibleMonth()
        {
            DataTable ldtblist = busPerson.Select("cdoPerson.LoadPersonSuspendibleMonth", new object[1] { this.icdoPerson.person_id });
            iclbPersonSuspendibleMonth = GetCollection<busPersonSuspendibleMonth>(ldtblist, "icdoPersonSuspendibleMonth");
            //PIR 936
            if (iclbPersonSuspendibleMonth != null && iclbPersonSuspendibleMonth.Count > 0)
                iclbPersonSuspendibleMonth = iclbPersonSuspendibleMonth.OrderBy(t => t.icdoPersonSuspendibleMonth.plan_year).ThenBy(t => t.icdoPersonSuspendibleMonth.suspendible_month_value).ToList().ToCollection();
        }

        #region Create Annual Statement Batch
        //PIR 882
        //Annual Statement Report Changes PIR 960
        public void CreateAnnualStatementReport(ReportViewer rvViewer, cdoAnnualStatementBatchData lcdoAnnualStatementBatchData, int aintComputationalYear, bool ablnIsBatchFlag = false,
                                               string lstrPostPrefixName = "", bool ablnCorrectedFlag = false)
        {
            DataSet ldtReportResult = new DataSet();
            string lstrReportPath = string.Empty;
            string AddressFlag = "";
            bool lblnEStatement = false;

            #region Create Temp Table to store REGULAR HOURS INFO
            DataTable ldtTempAddrDetails = new DataTable();

            ldtTempAddrDetails.Columns.Add("PERSON_DOB", typeof(DateTime));
            ldtTempAddrDetails.Columns.Add("MPI_PERSON_ID", typeof(string));
            ldtTempAddrDetails.Columns.Add("person_name", typeof(string));
            ldtTempAddrDetails.Columns.Add("ADDR_LINE_1", typeof(string));
            ldtTempAddrDetails.Columns.Add("ADDR_LINE_2", typeof(string));
            ldtTempAddrDetails.Columns.Add("ADDR_CITY", typeof(string));
            ldtTempAddrDetails.Columns.Add("ADDR_STATE_VALUE", typeof(string));
            ldtTempAddrDetails.Columns.Add("ADDR_ZIP_CODE", typeof(string));
            ldtTempAddrDetails.Columns.Add("ADDR_COUNTRY_VALUE", typeof(string));
            ldtTempAddrDetails.Columns.Add("END_DATE", typeof(DateTime));
            ldtTempAddrDetails.Columns.Add("YEAR_BEFORE_LAST_YEAR", typeof(int));
            ldtTempAddrDetails.Columns.Add("LAST_YEAR", typeof(int));
            ldtTempAddrDetails.Columns.Add("STATUS_CODE_VALUE", typeof(string));

            ldtTempAddrDetails.Columns.Add("LOCAL_52_PREMERGER_TOTAL_QUALIFIED_YEARS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_161_PREMERGER_TOTAL_QUALIFIED_YEARS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_600_PREMERGER_TOTAL_QUALIFIED_YEARS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_666_PREMERGER_TOTAL_QUALIFIED_YEARS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_700_PREMERGER_TOTAL_QUALIFIED_YEARS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("IAP_HOURS_A2", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_52_CREDITED_HOURS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_600_CREDITED_HOURS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_666_CREDITED_HOURS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_700_CREDITED_HOURS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_161_CREDITED_HOURS", typeof(decimal));

            ldtTempAddrDetails.Columns.Add("LOCAL_52_PREMERGER_BENEFIT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_161_PREMERGER_BENEFIT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_600_PREMERGER_BENEFIT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_666_PREMERGER_BENEFIT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_700_PREMERGER_BENEFIT", typeof(decimal));

            ldtTempAddrDetails.Columns.Add("QUALIFIED_YRS_BEFORE_PRIOR_YR", typeof(int));
            ldtTempAddrDetails.Columns.Add("VESTED_YRS_BEFORE_PRIOR_YR", typeof(int));

            ldtTempAddrDetails.Columns.Add("MPI_QUALIFIED_YEARS_IN_CUREENT_YEAR", typeof(int));
            ldtTempAddrDetails.Columns.Add("MPI_VESTED_YEARS_IN_CUREENT_YEAR", typeof(int));

            ldtTempAddrDetails.Columns.Add("CREDITED_HRS_BEFORE_PRIOR_YR", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("EE_AMT_PRIOR_YEAR", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("ACCRUED_BEN_BEFORE_PRIOR_YR", typeof(decimal));

            ldtTempAddrDetails.Columns.Add("LATE_HOUR_REPORTED_SUM", typeof(decimal));

            ldtTempAddrDetails.Columns.Add("YTD_HOURS_FOR_LAST_COMP_YEAR", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("EE_CONTRIBUTION_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("ACCRUED_BENEFIT_FOR_PRIOR_YEAR", typeof(decimal));

            ldtTempAddrDetails.Columns.Add("PRIOR_YEAR_IAP_ACCOUNT_BALANCE_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("PRIOR_YEAR_LOCAL52_SPECIAL_ACC_ACCOUNT_BALANCE_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("PRIOR_YEAR_LOCAL161_SPECIAL_ACC_ACCOUNT_BALANCE_AMT", typeof(decimal));

            ldtTempAddrDetails.Columns.Add("LATE_IAP_ADJUSTMENT_AMOUNT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LATE_LOCAL52_SPECIAL_ACCOUNT_ADJUSTMENT_AMOUNT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LATE_LOCAL161_SPECIAL_ACCOUNT_ADJUSTMENT_AMOUNT", typeof(decimal));

            ldtTempAddrDetails.Columns.Add("IAP_ALLOCATION1_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("lOCAL52_SPECIAL_ACCOUNT_ALLOCATION1_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("lOCAL161_SPECIAL_ACCOUNT_ALLOCATION1_AMT", typeof(decimal));

            ldtTempAddrDetails.Columns.Add("IAP_ALLOCATION2_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("IAP_ALLOCATION2_INVESTMENT_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("IAP_ALLOCATION2_FORFIETURE_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("IAP_ALLOCATION4_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("IAP_ALLOCATION4_INVESTMENT_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("IAP_ALLOCATION4_FORFIETURE_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("CURRENT_PAYMENT_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("L52_PAYOUTS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("L161_PAYOUTS", typeof(decimal));

            ldtTempAddrDetails.Columns.Add("COMPUTATION_YEAR_BEGIN_DATE", typeof(DateTime));
            ldtTempAddrDetails.Columns.Add("COMPUTATION_YEAR_BEGIN_DATE_FORMAT", typeof(string));
            ldtTempAddrDetails.Columns.Add("DATE_BEFORE_COMPUTATION_YR_BEGIN", typeof(string));
            ldtTempAddrDetails.Columns.Add("COMPUTATION_YEAR_END_DATE", typeof(DateTime));
            ldtTempAddrDetails.Columns.Add("COMPUTATION_YEAR_END_DATE_FORMAT", typeof(string));
            ldtTempAddrDetails.Columns.Add("COUNTRY_DESCRIPTION", typeof(string));
            ldtTempAddrDetails.Columns.Add("MPI_VESTED_DATE", typeof(DateTime));
            ldtTempAddrDetails.Columns.Add("MPI_TOTAL_VESTED_YEARS", typeof(int));

            //Annual Statement Report Changes PIR 960
            ldtTempAddrDetails.Columns.Add("CORRECTED_FLAG", typeof(bool));
            //ChangeID: 57284
            ldtTempAddrDetails.Columns.Add("ELIGIBLE_ACTIVE_INCR_FLAG", typeof(string));
            ldtTempAddrDetails.Columns.Add("MD_FLAG", typeof(string));
            ldtTempAddrDetails.Columns.Add("RETR_SPECIAL_ACCOUNT_FLAG", typeof(string));
            //ChangeID: 59768
            ldtTempAddrDetails.Columns.Add("REEMPLOYED_UNDER_65_FLAG", typeof(string));
            //ChangeID: 61146
            ldtTempAddrDetails.Columns.Add("MPI_RETIREE_FLAG", typeof(string));
            ldtTempAddrDetails.Columns.Add("LOCAL600_RETIREE_FLAG", typeof(string));
            ldtTempAddrDetails.Columns.Add("LOCAL666_RETIREE_FLAG", typeof(string));
            ldtTempAddrDetails.Columns.Add("LOCAL700_RETIREE_FLAG", typeof(string));
            ldtTempAddrDetails.Columns.Add("LOCAL52_RETIREE_FLAG", typeof(string));
            ldtTempAddrDetails.Columns.Add("LOCAL161_RETIREE_FLAG", typeof(string));
            //RID#106354 FOR 2022 STATEMENT CHANGES
            ldtTempAddrDetails.Columns.Add("IAP_LUMSUM_BALANCE", typeof(string));
            ldtTempAddrDetails.Columns.Add("EST_IAP_LIFE_ANNUITY", typeof(string));
            ldtTempAddrDetails.Columns.Add("EST_IAP_JS100_ANNUITY", typeof(string));
            //Ticket##128387 
            ldtTempAddrDetails.Columns.Add("PENSION_ONLY_FLAG", typeof(string));
            ldtTempAddrDetails.Columns.Add("REGISTERED_EMAIL_ADDRESS", typeof(string));
            #endregion

            if (lcdoAnnualStatementBatchData != null)
            {
                DataRow ldrTempAddrdetails = ldtTempAddrDetails.NewRow();
                ldrTempAddrdetails["PERSON_DOB"] = lcdoAnnualStatementBatchData.PERSON_DOB;
                ldrTempAddrdetails["COUNTRY_DESCRIPTION"] = lcdoAnnualStatementBatchData.country_description;
                ldrTempAddrdetails["MPI_PERSON_ID"] = lcdoAnnualStatementBatchData.MPI_PERSON_ID;
                ldrTempAddrdetails["person_name"] = lcdoAnnualStatementBatchData.person_name;
                ldrTempAddrdetails["ADDR_LINE_1"] = lcdoAnnualStatementBatchData.addr_line_1;
                ldrTempAddrdetails["ADDR_LINE_2"] = lcdoAnnualStatementBatchData.addr_line_2;
                ldrTempAddrdetails["ADDR_CITY"] = lcdoAnnualStatementBatchData.addr_city;
                ldrTempAddrdetails["ADDR_STATE_VALUE"] = lcdoAnnualStatementBatchData.addr_state_value;
                ldrTempAddrdetails["ADDR_ZIP_CODE"] = lcdoAnnualStatementBatchData.addr_zip_code;
                ldrTempAddrdetails["ADDR_COUNTRY_VALUE"] = lcdoAnnualStatementBatchData.addr_country_value;
                ldrTempAddrdetails["END_DATE"] = lcdoAnnualStatementBatchData.end_date;
                ldrTempAddrdetails["MPI_VESTED_DATE"] = lcdoAnnualStatementBatchData.mpi_vested_date;

                ldrTempAddrdetails["YEAR_BEFORE_LAST_YEAR"] = aintComputationalYear - 1;//PIR 882
                ldrTempAddrdetails["LAST_YEAR"] = aintComputationalYear;//PIR 882
                ldrTempAddrdetails["MPI_TOTAL_VESTED_YEARS"] = lcdoAnnualStatementBatchData.mpi_total_vested_years;
                ldrTempAddrdetails["LOCAL_52_PREMERGER_TOTAL_QUALIFIED_YEARS"] = lcdoAnnualStatementBatchData.l52_vested_years;
                ldrTempAddrdetails["LOCAL_161_PREMERGER_TOTAL_QUALIFIED_YEARS"] = lcdoAnnualStatementBatchData.l161_vested_years;
                ldrTempAddrdetails["LOCAL_600_PREMERGER_TOTAL_QUALIFIED_YEARS"] = lcdoAnnualStatementBatchData.l600_vested_years;
                ldrTempAddrdetails["LOCAL_666_PREMERGER_TOTAL_QUALIFIED_YEARS"] = lcdoAnnualStatementBatchData.l666_vested_years;
                ldrTempAddrdetails["LOCAL_700_PREMERGER_TOTAL_QUALIFIED_YEARS"] = lcdoAnnualStatementBatchData.l700_vested_years;

                ldrTempAddrdetails["iap_hours_a2"] = lcdoAnnualStatementBatchData.iap_hours_a2;
                ldrTempAddrdetails["LOCAL_52_CREDITED_HOURS"] = lcdoAnnualStatementBatchData.l52_credited_hours;
                ldrTempAddrdetails["LOCAL_600_CREDITED_HOURS"] = lcdoAnnualStatementBatchData.l600_credited_hours;
                ldrTempAddrdetails["LOCAL_666_CREDITED_HOURS"] = lcdoAnnualStatementBatchData.l666_credited_hours;
                ldrTempAddrdetails["LOCAL_700_CREDITED_HOURS"] = lcdoAnnualStatementBatchData.l700_credited_hours;
                ldrTempAddrdetails["LOCAL_161_CREDITED_HOURS"] = lcdoAnnualStatementBatchData.l161_credited_hours;

                ldrTempAddrdetails["LOCAL_52_PREMERGER_BENEFIT"] = lcdoAnnualStatementBatchData.l52_frozen_benefits;
                ldrTempAddrdetails["LOCAL_161_PREMERGER_BENEFIT"] = lcdoAnnualStatementBatchData.l161_frozen_benefits;
                ldrTempAddrdetails["LOCAL_600_PREMERGER_BENEFIT"] = lcdoAnnualStatementBatchData.l600_frozen_benefits;
                ldrTempAddrdetails["LOCAL_666_PREMERGER_BENEFIT"] = lcdoAnnualStatementBatchData.l666_frozen_benefits;
                ldrTempAddrdetails["LOCAL_700_PREMERGER_BENEFIT"] = lcdoAnnualStatementBatchData.l700_frozen_benefits;

                ldrTempAddrdetails["MPI_QUALIFIED_YEARS_IN_CUREENT_YEAR"] = lcdoAnnualStatementBatchData.mpi_qualified_years_in_cureent_year;
                ldrTempAddrdetails["MPI_VESTED_YEARS_IN_CUREENT_YEAR"] = lcdoAnnualStatementBatchData.mpi_vested_years_in_cureent_year;

                ldrTempAddrdetails["QUALIFIED_YRS_BEFORE_PRIOR_YR"] = lcdoAnnualStatementBatchData.mpi_qualified_years_as_of_prior_year;
                ldrTempAddrdetails["VESTED_YRS_BEFORE_PRIOR_YR"] = lcdoAnnualStatementBatchData.mpi_vested_years_as_of_prior_year;
                ldrTempAddrdetails["CREDITED_HRS_BEFORE_PRIOR_YR"] = lcdoAnnualStatementBatchData.mpi_credited_hours_as_of_prior_year;
                ldrTempAddrdetails["EE_AMT_PRIOR_YEAR"] = lcdoAnnualStatementBatchData.mpi_ee_contributions_as_of_prior_year;
                ldrTempAddrdetails["ACCRUED_BEN_BEFORE_PRIOR_YR"] = lcdoAnnualStatementBatchData.mpi_accrued_benefit_as_of_prior_year;

                ldrTempAddrdetails["LATE_HOUR_REPORTED_SUM"] = lcdoAnnualStatementBatchData.mpi_late_credited_hours_in_cureent_year;

                ldrTempAddrdetails["YTD_HOURS_FOR_LAST_COMP_YEAR"] = lcdoAnnualStatementBatchData.mpi_credited_hours_in_cureent_year;
                // ldrTempAddrdetails["EE_CONTRIBUTION_AMT"] = lcdoAnnualStatementBatchData.mpi_ee_contributions_in_cureent_year;
                ldrTempAddrdetails["ACCRUED_BENEFIT_FOR_PRIOR_YEAR"] = lcdoAnnualStatementBatchData.mpi_accrued_benefits_in_cureent_year;

                ldrTempAddrdetails["PRIOR_YEAR_IAP_ACCOUNT_BALANCE_AMT"] = lcdoAnnualStatementBatchData.iap_ending_balance_for_prior_year;
                ldrTempAddrdetails["PRIOR_YEAR_LOCAL52_SPECIAL_ACC_ACCOUNT_BALANCE_AMT"] = lcdoAnnualStatementBatchData.l52_ending_balance_for_prior_year;
                ldrTempAddrdetails["PRIOR_YEAR_LOCAL161_SPECIAL_ACC_ACCOUNT_BALANCE_AMT"] = lcdoAnnualStatementBatchData.l161_ending_balance_for_prior_year;

                ldrTempAddrdetails["LATE_IAP_ADJUSTMENT_AMOUNT"] = lcdoAnnualStatementBatchData.iap_prior_adjustment;
                ldrTempAddrdetails["LATE_LOCAL52_SPECIAL_ACCOUNT_ADJUSTMENT_AMOUNT"] = lcdoAnnualStatementBatchData.l52_prior_adjustment;
                ldrTempAddrdetails["LATE_LOCAL161_SPECIAL_ACCOUNT_ADJUSTMENT_AMOUNT"] = lcdoAnnualStatementBatchData.l161_prior_adjustment;

                ldrTempAddrdetails["IAP_ALLOCATION1_AMT"] = lcdoAnnualStatementBatchData.iap_net_investment_income;
                ldrTempAddrdetails["lOCAL52_SPECIAL_ACCOUNT_ALLOCATION1_AMT"] = lcdoAnnualStatementBatchData.l52_net_investment_income;
                ldrTempAddrdetails["lOCAL161_SPECIAL_ACCOUNT_ALLOCATION1_AMT"] = lcdoAnnualStatementBatchData.l161_net_investment_income;

                ldrTempAddrdetails["IAP_ALLOCATION2_AMT"] = lcdoAnnualStatementBatchData.iap_hourly_contributions_iaphoura2;
                ldrTempAddrdetails["IAP_ALLOCATION2_INVESTMENT_AMT"] = lcdoAnnualStatementBatchData.iap_hourly_contributions_iaphoura2_ialc;
                ldrTempAddrdetails["IAP_ALLOCATION2_FORFIETURE_AMT"] = lcdoAnnualStatementBatchData.iap_hourly_contributions_iaphoura2_falc;

                ldrTempAddrdetails["IAP_ALLOCATION4_AMT"] = lcdoAnnualStatementBatchData.iap_percentage_of_compensation;
                ldrTempAddrdetails["IAP_ALLOCATION4_INVESTMENT_AMT"] = lcdoAnnualStatementBatchData.iap_percentage_of_compensation_ialc;
                ldrTempAddrdetails["IAP_ALLOCATION4_FORFIETURE_AMT"] = lcdoAnnualStatementBatchData.iap_percentage_of_compensation_falc;
                ldrTempAddrdetails["CURRENT_PAYMENT_AMT"] = lcdoAnnualStatementBatchData.iap_payouts;
                ldrTempAddrdetails["L52_PAYOUTS"] = lcdoAnnualStatementBatchData.l52_payouts;
                ldrTempAddrdetails["L161_PAYOUTS"] = lcdoAnnualStatementBatchData.l161_payouts;

                //Annual Statement Report Changes PIR 960
                ldrTempAddrdetails["CORRECTED_FLAG"] = ablnCorrectedFlag;
                ldrTempAddrdetails["ELIGIBLE_ACTIVE_INCR_FLAG"] = lcdoAnnualStatementBatchData.eligible_active_incr_flag; //ChangeID: 57284
                ldrTempAddrdetails["MD_FLAG"] = lcdoAnnualStatementBatchData.md_flag;
                ldrTempAddrdetails["RETR_SPECIAL_ACCOUNT_FLAG"] = lcdoAnnualStatementBatchData.retr_special_account_flag;
                //RequestID: 59768
                ldrTempAddrdetails["REEMPLOYED_UNDER_65_FLAG"] = lcdoAnnualStatementBatchData.reemployed_under_65_flag;
                //RequestID: 61146
                ldrTempAddrdetails["MPI_RETIREE_FLAG"] = lcdoAnnualStatementBatchData.mpi_retiree_flag;
                ldrTempAddrdetails["LOCAL600_RETIREE_FLAG"] = lcdoAnnualStatementBatchData.local600_retiree_flag;
                ldrTempAddrdetails["LOCAL666_RETIREE_FLAG"] = lcdoAnnualStatementBatchData.local666_retiree_flag;
                ldrTempAddrdetails["LOCAL700_RETIREE_FLAG"] = lcdoAnnualStatementBatchData.local700_retiree_flag;
                ldrTempAddrdetails["LOCAL52_RETIREE_FLAG"] = lcdoAnnualStatementBatchData.local52_retiree_flag;
                ldrTempAddrdetails["LOCAL161_RETIREE_FLAG"] = lcdoAnnualStatementBatchData.local161_retiree_flag;
                //RID#106354 FOR 2022 STATEMENT CHANGES
                ldrTempAddrdetails["IAP_LUMSUM_BALANCE"] = lcdoAnnualStatementBatchData.iap_lumsum_balance;
                ldrTempAddrdetails["EST_IAP_LIFE_ANNUITY"] = lcdoAnnualStatementBatchData.est_iap_life_annuity;
                ldrTempAddrdetails["EST_IAP_JS100_ANNUITY"] = lcdoAnnualStatementBatchData.est_iap_js100_annuity;
                //Ticket##128387 
                ldrTempAddrdetails["PENSION_ONLY_FLAG"] = lcdoAnnualStatementBatchData.pension_only_flag;

                DateTime ldtComputationYearBeginDate = new DateTime();
                DateTime ldtDateBeforeComputationYrBegin = new DateTime();
                ldtComputationYearBeginDate = busGlobalFunctions.GetFirstDateOfComputationYear(aintComputationalYear);//PIR 882
                ldrTempAddrdetails["COMPUTATION_YEAR_BEGIN_DATE"] = ldtComputationYearBeginDate;

                string lstrCompYearBeginDtFormat = string.Empty;
                lstrCompYearBeginDtFormat = busGlobalFunctions.ConvertDateIntoDifFormat(ldtComputationYearBeginDate);
                ldrTempAddrdetails["COMPUTATION_YEAR_BEGIN_DATE_FORMAT"] = lstrCompYearBeginDtFormat;

                string lstrBeforeComputationYrBegin = string.Empty;
                ldtDateBeforeComputationYrBegin = ldtComputationYearBeginDate.AddDays(-1);
                lstrBeforeComputationYrBegin = busGlobalFunctions.ConvertDateIntoDifFormat(ldtDateBeforeComputationYrBegin);
                ldrTempAddrdetails["DATE_BEFORE_COMPUTATION_YR_BEGIN"] = lstrBeforeComputationYrBegin;

                DateTime ldtComputationYearEndDate = new DateTime();
                ldtComputationYearEndDate = busGlobalFunctions.GetLastDateOfComputationYear(aintComputationalYear);//PIR 882
                ldrTempAddrdetails["COMPUTATION_YEAR_END_DATE"] = ldtComputationYearEndDate;

                string lstrCompYearEndDtFormat = string.Empty;
                lstrCompYearEndDtFormat = busGlobalFunctions.ConvertDateIntoDifFormat(ldtComputationYearEndDate);
                ldrTempAddrdetails["COMPUTATION_YEAR_END_DATE_FORMAT"] = lstrCompYearEndDtFormat;

                DataTable ldtbParticipantCommunicationPreference = busBase.Select("cdoPerson.GetCommunicationPreferences", new object[1] { lcdoAnnualStatementBatchData.person_id });

                if (ldtbParticipantCommunicationPreference != null && ldtbParticipantCommunicationPreference.Rows.Count > 0)
                {
                    lblnEStatement = true;
                    ldrTempAddrdetails["REGISTERED_EMAIL_ADDRESS"] = ldtbParticipantCommunicationPreference.Rows[0]["REGISTERED_EMAIL_ADDRESS"];
                    if (ldtbParticipantCommunicationPreference.Rows[0]["IS_PAPER_STATEMENT"].ToString() == "Y" 
                        || ldtbParticipantCommunicationPreference.Rows[0]["REGISTERED_EMAIL_ADDRESS"].ToString().IsEmpty())
                    {
                        lblnEStatement = false;
                    }
                }
                ldtbParticipantCommunicationPreference.Dispose();


                ldtTempAddrDetails.Rows.Add(ldrTempAddrdetails);
                ldtTempAddrDetails.AcceptChanges();
                ldtTempAddrDetails.TableName = "ReportTable01";

                if (lcdoAnnualStatementBatchData.addr_category_value == "BAD")
                {
                    AddressFlag = "BAD_ADDRESS";
                }
                else if (lcdoAnnualStatementBatchData.addr_category_value == "INTR")
                {
                    AddressFlag = "FOREIGN_ADDRESS";
                }
                else
                {
                    AddressFlag = "DOMESTIC_ADDRESS";
                }
            }

            int lComputationYear = 0;
            lComputationYear = lcdoAnnualStatementBatchData.computational_year;
            DataTable ldtTempDataForHours = new DataTable();

            ldtReportResult.Tables.Add(ldtTempAddrDetails.Copy());

            //Commenting condition becaue in pay statement also using new template now.
            //if (!(lcdoAnnualStatementBatchData.md_flag == busConstant.FLAG_YES || lcdoAnnualStatementBatchData.retr_special_account_flag == busConstant.FLAG_YES || lcdoAnnualStatementBatchData.reemployed_under_65_flag == busConstant.FLAG_YES))
            //{
                #region Create Temp Table to store IAP Balance History
                //RID#106354 /* this code was added to create chart example, not approved yet.*/
                DataTable ldtTempIAPBalHistory = new DataTable();

                ldtTempIAPBalHistory.Columns.Add("MPI_PERSON_ID", typeof(string));
                ldtTempIAPBalHistory.Columns.Add("PERSON_NAME", typeof(string));
                ldtTempIAPBalHistory.Columns.Add("PLAN_YEAR", typeof(int));
                ldtTempIAPBalHistory.Columns.Add("TOTAL_IAP_BALANCE", typeof(decimal));
                ldtTempIAPBalHistory.Columns.Add("YEARBAR", typeof(int));

                if (lcdoAnnualStatementBatchData != null)
                {
                    DataTable ldtbIAPBalHistorySingleParticipant = busBase.Select("cdoDataExtractionBatchInfo.AnnualStatementIAPHistoryForSinglePerson", new object[1] { lcdoAnnualStatementBatchData.person_id });
                    if (ldtbIAPBalHistorySingleParticipant != null && ldtbIAPBalHistorySingleParticipant.Rows.Count > 0)
                    {
                        DataRow ldrTempIAPBalHistory;
                        foreach (DataRow ldr in ldtbIAPBalHistorySingleParticipant.Rows)
                        {
                            ldrTempIAPBalHistory = ldtTempIAPBalHistory.NewRow();
                            ldrTempIAPBalHistory["MPI_PERSON_ID"] = ldr["MPI_PERSON_ID"];
                            ldrTempIAPBalHistory["PERSON_NAME"] = ldr["PERSON_NAME"];
                            ldrTempIAPBalHistory["PLAN_YEAR"] = ldr["PLAN_YEAR"];
                            ldrTempIAPBalHistory["TOTAL_IAP_BALANCE"] = ldr["TOTAL_IAP_BALANCE"];
                            ldrTempIAPBalHistory["YEARBAR"] = ldr["YEARBAR"];
                            ldtTempIAPBalHistory.Rows.Add(ldrTempIAPBalHistory);
                        }
                        ldtTempIAPBalHistory.AcceptChanges();
                        ldtTempIAPBalHistory.TableName = "ChartTable01";
                    }
                    ldtbIAPBalHistorySingleParticipant.Dispose();
                }
                if (ldtTempIAPBalHistory != null && ldtTempIAPBalHistory.Rows.Count > 0)
                {
                    ldtReportResult.Tables.Add(ldtTempIAPBalHistory.Copy());
                }

                #endregion
            //}
            List<string> llstGeneratedReports = new List<string>();
            busCreateReports lobjCreateReports = new busCreateReports();

            if (ablnIsBatchFlag == false)
            {
                //Annual Statement Report Changes PIR 960
                if (lcdoAnnualStatementBatchData.md_flag == busConstant.FLAG_YES || lcdoAnnualStatementBatchData.retr_special_account_flag == busConstant.FLAG_YES || lcdoAnnualStatementBatchData.reemployed_under_65_flag == busConstant.FLAG_YES)
                {
                    lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt19_AnnualReportGenerated_InPay", "", lcdoAnnualStatementBatchData.MPI_PERSON_ID);
                }
                //Ticket##128387 
                else if (lcdoAnnualStatementBatchData.pension_only_flag == busConstant.FLAG_YES)
                 {
                    lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt19_AnnualReportGenerated_PensionOnly", "", lcdoAnnualStatementBatchData.MPI_PERSON_ID);
                }
                else
                    lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt19_AnnualReportGenerated", "", lcdoAnnualStatementBatchData.MPI_PERSON_ID);

                llstGeneratedReports.Add(lstrReportPath);
                System.Diagnostics.Process.Start(lstrReportPath);
            }
            else
            {
                if (lcdoAnnualStatementBatchData.md_flag == busConstant.FLAG_YES || lcdoAnnualStatementBatchData.retr_special_account_flag == busConstant.FLAG_YES || lcdoAnnualStatementBatchData.reemployed_under_65_flag == busConstant.FLAG_YES)
                {
                    lstrReportPath = lobjCreateReports.CreateAnnualStatmentPDFReport(rvViewer, ldtReportResult, "rpt19_AnnualReportGenerated_InPay", "", lcdoAnnualStatementBatchData.MPI_PERSON_ID, AddressFlag, lstrPostPrefixName, ablnCorrectedFlag, lblnEStatement);
                }
                //Ticket##128387 
                else if (lcdoAnnualStatementBatchData.pension_only_flag == busConstant.FLAG_YES)
                {
                    lstrReportPath = lobjCreateReports.CreateAnnualStatmentPDFReport(rvViewer, ldtReportResult, "rpt19_AnnualReportGenerated_PensionOnly", "", lcdoAnnualStatementBatchData.MPI_PERSON_ID, AddressFlag, lstrPostPrefixName, ablnCorrectedFlag, lblnEStatement);


                }
                else
                    lstrReportPath = lobjCreateReports.CreateAnnualStatmentPDFReport(rvViewer, ldtReportResult, "rpt19_AnnualReportGenerated", "", lcdoAnnualStatementBatchData.MPI_PERSON_ID, AddressFlag, lstrPostPrefixName, ablnCorrectedFlag, lblnEStatement);
                llstGeneratedReports.Add(lstrReportPath);
            }

            ldtReportResult.Dispose();
            ldtTempDataForHours.Dispose();

        }
        #endregion

        #region Create Annual Statement for Single Person & from MSS
        //Annual Statement Report Changes PIR 960
        public DataSet CreateAnnualStatementReport(cdoAnnualStatementBatchData lcdoAnnualStatementBatchData, int aintComputationYear, bool ablnCorrectedFlag = false)//PIR 882
        {
            DataSet ldtReportResult = new DataSet();
            string lstrReportPath = string.Empty;
            string AddressFlag = "";

            #region Create Temp Table to store REGULAR HOURS INFO
            DataTable ldtTempAddrDetails = new DataTable();

            ldtTempAddrDetails.Columns.Add("PERSON_DOB", typeof(DateTime));
            ldtTempAddrDetails.Columns.Add("MPI_PERSON_ID", typeof(string));
            ldtTempAddrDetails.Columns.Add("person_name", typeof(string));
            ldtTempAddrDetails.Columns.Add("ADDR_LINE_1", typeof(string));
            ldtTempAddrDetails.Columns.Add("ADDR_LINE_2", typeof(string));
            ldtTempAddrDetails.Columns.Add("ADDR_CITY", typeof(string));
            ldtTempAddrDetails.Columns.Add("ADDR_STATE_VALUE", typeof(string));
            ldtTempAddrDetails.Columns.Add("ADDR_ZIP_CODE", typeof(string));
            ldtTempAddrDetails.Columns.Add("ADDR_COUNTRY_VALUE", typeof(string));
            ldtTempAddrDetails.Columns.Add("END_DATE", typeof(DateTime));
            ldtTempAddrDetails.Columns.Add("YEAR_BEFORE_LAST_YEAR", typeof(int));
            ldtTempAddrDetails.Columns.Add("LAST_YEAR", typeof(int));
            ldtTempAddrDetails.Columns.Add("STATUS_CODE_VALUE", typeof(string));

            ldtTempAddrDetails.Columns.Add("LOCAL_52_PREMERGER_TOTAL_QUALIFIED_YEARS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_161_PREMERGER_TOTAL_QUALIFIED_YEARS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_600_PREMERGER_TOTAL_QUALIFIED_YEARS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_666_PREMERGER_TOTAL_QUALIFIED_YEARS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_700_PREMERGER_TOTAL_QUALIFIED_YEARS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("IAP_HOURS_A2", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_52_CREDITED_HOURS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_600_CREDITED_HOURS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_666_CREDITED_HOURS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_700_CREDITED_HOURS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_161_CREDITED_HOURS", typeof(decimal));

            ldtTempAddrDetails.Columns.Add("LOCAL_52_PREMERGER_BENEFIT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_161_PREMERGER_BENEFIT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_600_PREMERGER_BENEFIT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_666_PREMERGER_BENEFIT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_700_PREMERGER_BENEFIT", typeof(decimal));

            ldtTempAddrDetails.Columns.Add("QUALIFIED_YRS_BEFORE_PRIOR_YR", typeof(int));
            ldtTempAddrDetails.Columns.Add("VESTED_YRS_BEFORE_PRIOR_YR", typeof(int));

            ldtTempAddrDetails.Columns.Add("MPI_QUALIFIED_YEARS_IN_CUREENT_YEAR", typeof(int));
            ldtTempAddrDetails.Columns.Add("MPI_VESTED_YEARS_IN_CUREENT_YEAR", typeof(int));

            ldtTempAddrDetails.Columns.Add("CREDITED_HRS_BEFORE_PRIOR_YR", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("EE_AMT_PRIOR_YEAR", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("ACCRUED_BEN_BEFORE_PRIOR_YR", typeof(decimal));

            ldtTempAddrDetails.Columns.Add("LATE_HOUR_REPORTED_SUM", typeof(decimal));

            ldtTempAddrDetails.Columns.Add("YTD_HOURS_FOR_LAST_COMP_YEAR", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("EE_CONTRIBUTION_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("ACCRUED_BENEFIT_FOR_PRIOR_YEAR", typeof(decimal));

            ldtTempAddrDetails.Columns.Add("PRIOR_YEAR_IAP_ACCOUNT_BALANCE_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("PRIOR_YEAR_LOCAL52_SPECIAL_ACC_ACCOUNT_BALANCE_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("PRIOR_YEAR_LOCAL161_SPECIAL_ACC_ACCOUNT_BALANCE_AMT", typeof(decimal));

            ldtTempAddrDetails.Columns.Add("LATE_IAP_ADJUSTMENT_AMOUNT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LATE_LOCAL52_SPECIAL_ACCOUNT_ADJUSTMENT_AMOUNT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LATE_LOCAL161_SPECIAL_ACCOUNT_ADJUSTMENT_AMOUNT", typeof(decimal));

            ldtTempAddrDetails.Columns.Add("IAP_ALLOCATION1_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("lOCAL52_SPECIAL_ACCOUNT_ALLOCATION1_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("lOCAL161_SPECIAL_ACCOUNT_ALLOCATION1_AMT", typeof(decimal));

            ldtTempAddrDetails.Columns.Add("IAP_ALLOCATION2_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("IAP_ALLOCATION2_INVESTMENT_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("IAP_ALLOCATION2_FORFIETURE_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("IAP_ALLOCATION4_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("IAP_ALLOCATION4_INVESTMENT_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("IAP_ALLOCATION4_FORFIETURE_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("CURRENT_PAYMENT_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("L52_PAYOUTS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("L161_PAYOUTS", typeof(decimal));

            ldtTempAddrDetails.Columns.Add("COMPUTATION_YEAR_BEGIN_DATE", typeof(DateTime));
            ldtTempAddrDetails.Columns.Add("COMPUTATION_YEAR_BEGIN_DATE_FORMAT", typeof(string));
            ldtTempAddrDetails.Columns.Add("DATE_BEFORE_COMPUTATION_YR_BEGIN", typeof(string));
            ldtTempAddrDetails.Columns.Add("COMPUTATION_YEAR_END_DATE", typeof(DateTime));
            ldtTempAddrDetails.Columns.Add("COMPUTATION_YEAR_END_DATE_FORMAT", typeof(string));
            ldtTempAddrDetails.Columns.Add("COUNTRY_DESCRIPTION", typeof(string));
            ldtTempAddrDetails.Columns.Add("MPI_VESTED_DATE", typeof(DateTime));
            ldtTempAddrDetails.Columns.Add("MPI_TOTAL_VESTED_YEARS", typeof(int));
            //Annual statement report changes PIR 960
            ldtTempAddrDetails.Columns.Add("CORRECTED_FLAG", typeof(bool));
            //ChangeID: 57284
            ldtTempAddrDetails.Columns.Add("ELIGIBLE_ACTIVE_INCR_FLAG", typeof(string));
            ldtTempAddrDetails.Columns.Add("MD_FLAG", typeof(string));
            ldtTempAddrDetails.Columns.Add("RETR_SPECIAL_ACCOUNT_FLAG", typeof(string));
            //RequestID: 59768
            ldtTempAddrDetails.Columns.Add("REEMPLOYED_UNDER_65_FLAG", typeof(string));
            //RequestID: 61146
            ldtTempAddrDetails.Columns.Add("MPI_RETIREE_FLAG", typeof(string));
            ldtTempAddrDetails.Columns.Add("LOCAL600_RETIREE_FLAG", typeof(string));
            ldtTempAddrDetails.Columns.Add("LOCAL666_RETIREE_FLAG", typeof(string));
            ldtTempAddrDetails.Columns.Add("LOCAL700_RETIREE_FLAG", typeof(string));
            ldtTempAddrDetails.Columns.Add("LOCAL52_RETIREE_FLAG", typeof(string));
            ldtTempAddrDetails.Columns.Add("LOCAL161_RETIREE_FLAG", typeof(string));
            //RID#106354 FOR 2022 STATEMENT CHANGES
            ldtTempAddrDetails.Columns.Add("IAP_LUMSUM_BALANCE", typeof(string));
            ldtTempAddrDetails.Columns.Add("EST_IAP_LIFE_ANNUITY", typeof(string));
            ldtTempAddrDetails.Columns.Add("EST_IAP_JS100_ANNUITY", typeof(string));
            //Ticket##128387 
            ldtTempAddrDetails.Columns.Add("PENSION_ONLY_FLAG", typeof(string));
            ldtTempAddrDetails.Columns.Add("REGISTERED_EMAIL_ADDRESS", typeof(string));
            #endregion

            if (lcdoAnnualStatementBatchData != null)
            {
                DataRow ldrTempAddrdetails = ldtTempAddrDetails.NewRow();
                ldrTempAddrdetails["PERSON_DOB"] = lcdoAnnualStatementBatchData.PERSON_DOB;
                ldrTempAddrdetails["COUNTRY_DESCRIPTION"] = lcdoAnnualStatementBatchData.country_description;
                ldrTempAddrdetails["MPI_PERSON_ID"] = lcdoAnnualStatementBatchData.MPI_PERSON_ID;
                ldrTempAddrdetails["person_name"] = lcdoAnnualStatementBatchData.person_name;
                ldrTempAddrdetails["ADDR_LINE_1"] = lcdoAnnualStatementBatchData.addr_line_1;
                ldrTempAddrdetails["ADDR_LINE_2"] = lcdoAnnualStatementBatchData.addr_line_2;
                ldrTempAddrdetails["ADDR_CITY"] = lcdoAnnualStatementBatchData.addr_city;
                ldrTempAddrdetails["ADDR_STATE_VALUE"] = lcdoAnnualStatementBatchData.addr_state_value;
                ldrTempAddrdetails["ADDR_ZIP_CODE"] = lcdoAnnualStatementBatchData.addr_zip_code;
                ldrTempAddrdetails["ADDR_COUNTRY_VALUE"] = lcdoAnnualStatementBatchData.addr_country_value;
                //ChangeID: 57284
                //ldrTempAddrdetails["END_DATE"] = lcdoAnnualStatementBatchData.end_date;
                ldrTempAddrdetails["MPI_VESTED_DATE"] = lcdoAnnualStatementBatchData.mpi_vested_date;

                ldrTempAddrdetails["YEAR_BEFORE_LAST_YEAR"] = aintComputationYear - 1;//PIR 882
                ldrTempAddrdetails["LAST_YEAR"] = aintComputationYear;//PIR 882
                ldrTempAddrdetails["MPI_TOTAL_VESTED_YEARS"] = lcdoAnnualStatementBatchData.mpi_total_vested_years;
                ldrTempAddrdetails["LOCAL_52_PREMERGER_TOTAL_QUALIFIED_YEARS"] = lcdoAnnualStatementBatchData.l52_vested_years;
                ldrTempAddrdetails["LOCAL_161_PREMERGER_TOTAL_QUALIFIED_YEARS"] = lcdoAnnualStatementBatchData.l161_vested_years;
                ldrTempAddrdetails["LOCAL_600_PREMERGER_TOTAL_QUALIFIED_YEARS"] = lcdoAnnualStatementBatchData.l600_vested_years;
                ldrTempAddrdetails["LOCAL_666_PREMERGER_TOTAL_QUALIFIED_YEARS"] = lcdoAnnualStatementBatchData.l666_vested_years;
                ldrTempAddrdetails["LOCAL_700_PREMERGER_TOTAL_QUALIFIED_YEARS"] = lcdoAnnualStatementBatchData.l700_vested_years;

                ldrTempAddrdetails["iap_hours_a2"] = lcdoAnnualStatementBatchData.iap_hours_a2;
                ldrTempAddrdetails["LOCAL_52_CREDITED_HOURS"] = lcdoAnnualStatementBatchData.l52_credited_hours;
                ldrTempAddrdetails["LOCAL_600_CREDITED_HOURS"] = lcdoAnnualStatementBatchData.l600_credited_hours;
                ldrTempAddrdetails["LOCAL_666_CREDITED_HOURS"] = lcdoAnnualStatementBatchData.l666_credited_hours;
                ldrTempAddrdetails["LOCAL_700_CREDITED_HOURS"] = lcdoAnnualStatementBatchData.l700_credited_hours;
                ldrTempAddrdetails["LOCAL_161_CREDITED_HOURS"] = lcdoAnnualStatementBatchData.l161_credited_hours;

                ldrTempAddrdetails["LOCAL_52_PREMERGER_BENEFIT"] = lcdoAnnualStatementBatchData.l52_frozen_benefits;
                ldrTempAddrdetails["LOCAL_161_PREMERGER_BENEFIT"] = lcdoAnnualStatementBatchData.l161_frozen_benefits;
                ldrTempAddrdetails["LOCAL_600_PREMERGER_BENEFIT"] = lcdoAnnualStatementBatchData.l600_frozen_benefits;
                ldrTempAddrdetails["LOCAL_666_PREMERGER_BENEFIT"] = lcdoAnnualStatementBatchData.l666_frozen_benefits;
                ldrTempAddrdetails["LOCAL_700_PREMERGER_BENEFIT"] = lcdoAnnualStatementBatchData.l700_frozen_benefits;

                ldrTempAddrdetails["MPI_QUALIFIED_YEARS_IN_CUREENT_YEAR"] = lcdoAnnualStatementBatchData.mpi_qualified_years_in_cureent_year;
                ldrTempAddrdetails["MPI_VESTED_YEARS_IN_CUREENT_YEAR"] = lcdoAnnualStatementBatchData.mpi_vested_years_in_cureent_year;

                ldrTempAddrdetails["QUALIFIED_YRS_BEFORE_PRIOR_YR"] = lcdoAnnualStatementBatchData.mpi_qualified_years_as_of_prior_year;
                ldrTempAddrdetails["VESTED_YRS_BEFORE_PRIOR_YR"] = lcdoAnnualStatementBatchData.mpi_vested_years_as_of_prior_year;
                ldrTempAddrdetails["CREDITED_HRS_BEFORE_PRIOR_YR"] = lcdoAnnualStatementBatchData.mpi_credited_hours_as_of_prior_year;
                ldrTempAddrdetails["EE_AMT_PRIOR_YEAR"] = lcdoAnnualStatementBatchData.mpi_ee_contributions_as_of_prior_year;
                ldrTempAddrdetails["ACCRUED_BEN_BEFORE_PRIOR_YR"] = lcdoAnnualStatementBatchData.mpi_accrued_benefit_as_of_prior_year;

                ldrTempAddrdetails["LATE_HOUR_REPORTED_SUM"] = lcdoAnnualStatementBatchData.mpi_late_credited_hours_in_cureent_year;

                ldrTempAddrdetails["YTD_HOURS_FOR_LAST_COMP_YEAR"] = lcdoAnnualStatementBatchData.mpi_credited_hours_in_cureent_year;
                // ldrTempAddrdetails["EE_CONTRIBUTION_AMT"] = lcdoAnnualStatementBatchData.mpi_ee_contributions_in_cureent_year;
                ldrTempAddrdetails["ACCRUED_BENEFIT_FOR_PRIOR_YEAR"] = lcdoAnnualStatementBatchData.mpi_accrued_benefits_in_cureent_year;

                ldrTempAddrdetails["PRIOR_YEAR_IAP_ACCOUNT_BALANCE_AMT"] = lcdoAnnualStatementBatchData.iap_ending_balance_for_prior_year;
                ldrTempAddrdetails["PRIOR_YEAR_LOCAL52_SPECIAL_ACC_ACCOUNT_BALANCE_AMT"] = lcdoAnnualStatementBatchData.l52_ending_balance_for_prior_year;
                ldrTempAddrdetails["PRIOR_YEAR_LOCAL161_SPECIAL_ACC_ACCOUNT_BALANCE_AMT"] = lcdoAnnualStatementBatchData.l161_ending_balance_for_prior_year;

                ldrTempAddrdetails["LATE_IAP_ADJUSTMENT_AMOUNT"] = lcdoAnnualStatementBatchData.iap_prior_adjustment;
                ldrTempAddrdetails["LATE_LOCAL52_SPECIAL_ACCOUNT_ADJUSTMENT_AMOUNT"] = lcdoAnnualStatementBatchData.l52_prior_adjustment;
                ldrTempAddrdetails["LATE_LOCAL161_SPECIAL_ACCOUNT_ADJUSTMENT_AMOUNT"] = lcdoAnnualStatementBatchData.l161_prior_adjustment;

                ldrTempAddrdetails["IAP_ALLOCATION1_AMT"] = lcdoAnnualStatementBatchData.iap_net_investment_income;
                ldrTempAddrdetails["lOCAL52_SPECIAL_ACCOUNT_ALLOCATION1_AMT"] = lcdoAnnualStatementBatchData.l52_net_investment_income;
                ldrTempAddrdetails["lOCAL161_SPECIAL_ACCOUNT_ALLOCATION1_AMT"] = lcdoAnnualStatementBatchData.l161_net_investment_income;

                ldrTempAddrdetails["IAP_ALLOCATION2_AMT"] = lcdoAnnualStatementBatchData.iap_hourly_contributions_iaphoura2;
                ldrTempAddrdetails["IAP_ALLOCATION2_INVESTMENT_AMT"] = lcdoAnnualStatementBatchData.iap_hourly_contributions_iaphoura2_ialc;
                ldrTempAddrdetails["IAP_ALLOCATION2_FORFIETURE_AMT"] = lcdoAnnualStatementBatchData.iap_hourly_contributions_iaphoura2_falc;

                ldrTempAddrdetails["IAP_ALLOCATION4_AMT"] = lcdoAnnualStatementBatchData.iap_percentage_of_compensation;
                ldrTempAddrdetails["IAP_ALLOCATION4_INVESTMENT_AMT"] = lcdoAnnualStatementBatchData.iap_percentage_of_compensation_ialc;
                ldrTempAddrdetails["IAP_ALLOCATION4_FORFIETURE_AMT"] = lcdoAnnualStatementBatchData.iap_percentage_of_compensation_falc;
                ldrTempAddrdetails["CURRENT_PAYMENT_AMT"] = lcdoAnnualStatementBatchData.iap_payouts;
                ldrTempAddrdetails["L52_PAYOUTS"] = lcdoAnnualStatementBatchData.l52_payouts;
                ldrTempAddrdetails["L161_PAYOUTS"] = lcdoAnnualStatementBatchData.l161_payouts;

                //Annual Statement Report Changes PIR 960
                ldrTempAddrdetails["CORRECTED_FLAG"] = ablnCorrectedFlag;
                //ChangeID: 57284
                ldrTempAddrdetails["ELIGIBLE_ACTIVE_INCR_FLAG"] = lcdoAnnualStatementBatchData.eligible_active_incr_flag;
                ldrTempAddrdetails["MD_FLAG"] = lcdoAnnualStatementBatchData.md_flag;
                ldrTempAddrdetails["RETR_SPECIAL_ACCOUNT_FLAG"] = lcdoAnnualStatementBatchData.retr_special_account_flag;
                //RequestID: 59768
                ldrTempAddrdetails["REEMPLOYED_UNDER_65_FLAG"] = lcdoAnnualStatementBatchData.reemployed_under_65_flag;
                //RequestID: 61146
                ldrTempAddrdetails["MPI_RETIREE_FLAG"] = lcdoAnnualStatementBatchData.mpi_retiree_flag;
                ldrTempAddrdetails["LOCAL600_RETIREE_FLAG"] = lcdoAnnualStatementBatchData.local600_retiree_flag;
                ldrTempAddrdetails["LOCAL666_RETIREE_FLAG"] = lcdoAnnualStatementBatchData.local666_retiree_flag;
                ldrTempAddrdetails["LOCAL700_RETIREE_FLAG"] = lcdoAnnualStatementBatchData.local700_retiree_flag;
                ldrTempAddrdetails["LOCAL52_RETIREE_FLAG"] = lcdoAnnualStatementBatchData.local52_retiree_flag;
                ldrTempAddrdetails["LOCAL161_RETIREE_FLAG"] = lcdoAnnualStatementBatchData.local161_retiree_flag;
                //RID#106354 FOR 2022 STATEMENT CHANGES
                ldrTempAddrdetails["IAP_LUMSUM_BALANCE"] = lcdoAnnualStatementBatchData.iap_lumsum_balance;
                ldrTempAddrdetails["EST_IAP_LIFE_ANNUITY"] = lcdoAnnualStatementBatchData.est_iap_life_annuity;
                ldrTempAddrdetails["EST_IAP_JS100_ANNUITY"] = lcdoAnnualStatementBatchData.est_iap_js100_annuity;
                //Ticket##128387 
                ldrTempAddrdetails["PENSION_ONLY_FLAG"] = lcdoAnnualStatementBatchData.pension_only_flag;

                DateTime ldtComputationYearBeginDate = new DateTime();
                DateTime ldtDateBeforeComputationYrBegin = new DateTime();
                ldtComputationYearBeginDate = busGlobalFunctions.GetFirstDateOfComputationYear(aintComputationYear);//PIR 882
                ldrTempAddrdetails["COMPUTATION_YEAR_BEGIN_DATE"] = ldtComputationYearBeginDate;

                string lstrCompYearBeginDtFormat = string.Empty;
                lstrCompYearBeginDtFormat = busGlobalFunctions.ConvertDateIntoDifFormat(ldtComputationYearBeginDate);
                ldrTempAddrdetails["COMPUTATION_YEAR_BEGIN_DATE_FORMAT"] = lstrCompYearBeginDtFormat;

                string lstrBeforeComputationYrBegin = string.Empty;
                ldtDateBeforeComputationYrBegin = ldtComputationYearBeginDate.AddDays(-1);
                lstrBeforeComputationYrBegin = busGlobalFunctions.ConvertDateIntoDifFormat(ldtDateBeforeComputationYrBegin);
                ldrTempAddrdetails["DATE_BEFORE_COMPUTATION_YR_BEGIN"] = lstrBeforeComputationYrBegin;

                DateTime ldtComputationYearEndDate = new DateTime();
                ldtComputationYearEndDate = busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear);//PIR 882
                ldrTempAddrdetails["COMPUTATION_YEAR_END_DATE"] = ldtComputationYearEndDate;

                string lstrCompYearEndDtFormat = string.Empty;
                lstrCompYearEndDtFormat = busGlobalFunctions.ConvertDateIntoDifFormat(ldtComputationYearEndDate);
                ldrTempAddrdetails["COMPUTATION_YEAR_END_DATE_FORMAT"] = lstrCompYearEndDtFormat;

                DataTable ldtbParticipantCommunicationPreference = busBase.Select("cdoPerson.GetCommunicationPreferences", new object[1] { lcdoAnnualStatementBatchData.person_id });
                if (ldtbParticipantCommunicationPreference != null && ldtbParticipantCommunicationPreference.Rows.Count > 0)
                {
                    ldrTempAddrdetails["REGISTERED_EMAIL_ADDRESS"] = ldtbParticipantCommunicationPreference.Rows[0]["REGISTERED_EMAIL_ADDRESS"];
                }
                ldtbParticipantCommunicationPreference.Dispose();

                ldtTempAddrDetails.Rows.Add(ldrTempAddrdetails);
                ldtTempAddrDetails.AcceptChanges();
                ldtTempAddrDetails.TableName = "ReportTable01";

                if (lcdoAnnualStatementBatchData.addr_category_value == "BAD")
                {
                    AddressFlag = "BAD_ADDRESS";
                }
                else if (lcdoAnnualStatementBatchData.addr_category_value == "INTR")
                {
                    AddressFlag = "FOREIGN_ADDRESS";
                }
                else
                {
                    AddressFlag = "DOMESTIC_ADDRESS";
                }
            }

            int lComputationYear = 0;
            lComputationYear = lcdoAnnualStatementBatchData.computational_year;
            DataTable ldtTempDataForHours = new DataTable();

            ldtReportResult.Tables.Add(ldtTempAddrDetails.Copy());
            //Commenting condition becaue in pay statement also using new template now.
            //if (!(lcdoAnnualStatementBatchData.md_flag == busConstant.FLAG_YES || lcdoAnnualStatementBatchData.retr_special_account_flag == busConstant.FLAG_YES || lcdoAnnualStatementBatchData.reemployed_under_65_flag == busConstant.FLAG_YES) )
            //{
                #region Create Temp Table to store IAP Balance History
                //RID#106354 /* this code was added to create chart example, not approved yet.*/
                DataTable ldtTempIAPBalHistory = new DataTable();

                ldtTempIAPBalHistory.Columns.Add("MPI_PERSON_ID", typeof(string));
                ldtTempIAPBalHistory.Columns.Add("PERSON_NAME", typeof(string));
                ldtTempIAPBalHistory.Columns.Add("PLAN_YEAR", typeof(int));
                ldtTempIAPBalHistory.Columns.Add("TOTAL_IAP_BALANCE", typeof(decimal));
                ldtTempIAPBalHistory.Columns.Add("YEARBAR", typeof(int));

                if (lcdoAnnualStatementBatchData != null)
                {
                    DataTable ldtbIAPBalHistorySingleParticipant = busBase.Select("cdoDataExtractionBatchInfo.AnnualStatementIAPHistoryForSinglePerson", new object[1] { lcdoAnnualStatementBatchData.person_id });
                    if (ldtbIAPBalHistorySingleParticipant != null && ldtbIAPBalHistorySingleParticipant.Rows.Count > 0)
                    {
                        DataRow ldrTempIAPBalHistory;
                        foreach (DataRow ldr in ldtbIAPBalHistorySingleParticipant.Rows)
                        {
                            ldrTempIAPBalHistory = ldtTempIAPBalHistory.NewRow();
                            ldrTempIAPBalHistory["MPI_PERSON_ID"] = ldr["MPI_PERSON_ID"];
                            ldrTempIAPBalHistory["PERSON_NAME"] = ldr["PERSON_NAME"];
                            ldrTempIAPBalHistory["PLAN_YEAR"] = ldr["PLAN_YEAR"];
                            ldrTempIAPBalHistory["TOTAL_IAP_BALANCE"] = ldr["TOTAL_IAP_BALANCE"];
                            ldrTempIAPBalHistory["YEARBAR"] = ldr["YEARBAR"];
                            ldtTempIAPBalHistory.Rows.Add(ldrTempIAPBalHistory);
                        }
                        ldtTempIAPBalHistory.AcceptChanges();
                        ldtTempIAPBalHistory.TableName = "ChartTable01";
                    }
                    ldtbIAPBalHistorySingleParticipant.Dispose();
                }
                if (ldtTempIAPBalHistory != null && ldtTempIAPBalHistory.Rows.Count > 0)
                {
                    ldtReportResult.Tables.Add(ldtTempIAPBalHistory.Copy());
                }

                #endregion
            //}
            return ldtReportResult;
        }
        #endregion

        #region GenerateAnnualStatements
        public byte[] GenerateAnnualStatements(int aintPersonID, int lintCompYear = 0)
        {
            if (iobjSystemManagement == null)
            {
                iobjSystemManagement = new busSystemManagement();
                iobjSystemManagement.FindSystemManagement();
            }
            if (lintCompYear == 0)
            {
                //PIR 878
                //lintCompYear = iobjSystemManagement.icdoSystemManagement.batch_date.Year - 1;                
                busAnnualStatementBatchData lbusAnnualStatementBatchData = new busAnnualStatementBatchData { icdoAnnualStatementBatchData = new cdoAnnualStatementBatchData() };
                DataTable ldtList = Select<cdoAnnualStatementBatchData>(
                                    new string[1] { enmAnnualStatementBatchData.person_id.ToString() },
                                    new object[1] { aintPersonID }, null, enmAnnualStatementBatchData.computational_year.ToString() + " " + "Desc");
                if (ldtList.Rows.Count > 0)
                {
                    lintCompYear = Convert.ToInt32(ldtList.Rows[0]["computational_year"]);
                }
                else
                {
                    lintCompYear = iobjSystemManagement.icdoSystemManagement.batch_date.Year - 1;
                }
            }

            DataTable ldtbAnnualStatementSingleParticipant = busBase.Select("cdoDataExtractionBatchInfo.GetAnnualStatementForPersonWithLatestAddress", new object[2] { lintCompYear, aintPersonID });
            cdoAnnualStatementBatchData lcdoAnnualStatementBatchData = new cdoAnnualStatementBatchData();

            if (ldtbAnnualStatementSingleParticipant != null && ldtbAnnualStatementSingleParticipant.Rows.Count > 0)
            {
                lcdoAnnualStatementBatchData.LoadData(ldtbAnnualStatementSingleParticipant.Rows[0]);

                bool ablnCorrectedFlag = false;

                if (lcdoAnnualStatementBatchData.corrected_flag == busConstant.FLAG_YES)
                {
                    ablnCorrectedFlag = true;
                }

                //Annual Statement Report Changes PIR 960
                DataSet ldtResult = CreateAnnualStatementReport(lcdoAnnualStatementBatchData, lintCompYear, ablnCorrectedFlag);//PIR 882
                if (ldtResult != null)
                {
                    byte[] lbyteFile = null;
                    busCreateReports lbusCreateReports = new busCreateReports();
                    //Annual Statement Report Changes PIR 960
                    if (lcdoAnnualStatementBatchData.md_flag == busConstant.FLAG_YES || lcdoAnnualStatementBatchData.retr_special_account_flag == busConstant.FLAG_YES || lcdoAnnualStatementBatchData.reemployed_under_65_flag == busConstant.FLAG_YES)
                        lbyteFile = lbusCreateReports.CreateDynamicReport(ldtResult, "rpt19_AnnualReportGenerated_InPay");
                    //Ticket##128387 
                    else if (lcdoAnnualStatementBatchData.pension_only_flag == busConstant.FLAG_YES)
                    {
                        lbyteFile = lbusCreateReports.CreateDynamicReport(ldtResult, "rpt19_AnnualReportGenerated_PensionOnly");
                    }
                    else
                        lbyteFile = lbusCreateReports.CreateDynamicReport(ldtResult, "rpt19_AnnualReportGenerated");

                    return lbyteFile;
                }
                lcdoAnnualStatementBatchData = null;
                ldtResult.Dispose();
            }

            ldtbAnnualStatementSingleParticipant.Dispose();
            return null;
        }
        #endregion

        //Annual Statement Report Changes PIR 960
        public void CreateAnnualStatementReport(DataTable adtbAnnualStatementParticipant, bool ablnIsBatchFlag = false, bool ablnCorrectedFlag = false)
        {
            DataSet ldtReportResult = new DataSet();
            string lstrReportPath = string.Empty;

            //Create Temp Table to store REGULAR HOURS INFO
            DataTable ldtTempAddrDetails = new DataTable();
            ldtTempAddrDetails.Columns.Add("person_name", typeof(string));
            ldtTempAddrDetails.Columns.Add("ADDR_LINE_1", typeof(string));
            ldtTempAddrDetails.Columns.Add("ADDR_LINE_2", typeof(string));
            ldtTempAddrDetails.Columns.Add("ADDR_CITY", typeof(string));
            ldtTempAddrDetails.Columns.Add("ADDR_STATE_VALUE", typeof(string));
            ldtTempAddrDetails.Columns.Add("ADDR_ZIP_CODE", typeof(string));
            ldtTempAddrDetails.Columns.Add("LOCAL_52_PREMERGER_BENEFIT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_161_PREMERGER_BENEFIT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_600_PREMERGER_BENEFIT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_666_PREMERGER_BENEFIT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_700_PREMERGER_BENEFIT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("YTD_HOURS_BEFORE_LAST_COMP_YEAR", typeof(decimal));

            ldtTempAddrDetails.Columns.Add("ACCRUED_BENEFIT_TILL_LAST_COMP_YEAR", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LATE_HOUR_REPORTED", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("TOTAL_QUALIFIED_YEARS", typeof(int));
            ldtTempAddrDetails.Columns.Add("EE_AMT_PRIOR_YEAR", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("ACCRUED_BENEFIT_FOR_PRIOR_YEAR", typeof(decimal));

            ldtTempAddrDetails.Columns.Add("YEAR_BEFORE_LAST_YEAR", typeof(int));
            ldtTempAddrDetails.Columns.Add("LAST_YEAR", typeof(int));
            ldtTempAddrDetails.Columns.Add("YTD_HOURS_FOR_LAST_COMP_YEAR", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("PERSON_DOB", typeof(DateTime));
            ldtTempAddrDetails.Columns.Add("MPI_PERSON_ID", typeof(string));

            ldtTempAddrDetails.Columns.Add("LOCAL_52_FLAG", typeof(string));
            ldtTempAddrDetails.Columns.Add("LOCAL_161_FLAG", typeof(string));
            ldtTempAddrDetails.Columns.Add("LOCAL_666_FLAG", typeof(string));
            ldtTempAddrDetails.Columns.Add("LOCAL_700_FLAG", typeof(string));
            ldtTempAddrDetails.Columns.Add("LOCAL_600_FLAG", typeof(string));
            ldtTempAddrDetails.Columns.Add("IAP_ALLOCATION1_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("IAP_ALLOCATION2_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("IAP_ALLOCATION2_INVESTMENT_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("IAP_ALLOCATION2_FORFIETURE_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("IAP_ALLOCATION4_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("IAP_ALLOCATION4_INVESTMENT_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("IAP_ALLOCATION4_FORFIETURE_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("CURRENT_PAYMENT_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("PRIOR_YEAR_IAP_ACCOUNT_BALANCE_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("PRIOR_YEAR_LOCAL52_SPECIAL_ACC_ACCOUNT_BALANCE_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("lOCAL52_SPECIAL_ACCOUNT_ALLOCATION1_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("lOCAL52_SPECIAL_ACCOUNT_ALLOCATION2_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("lOCAL52_SPECIAL_ACCOUNT_ALLOCATION4_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("PRIOR_YEAR_LOCAL161_SPECIAL_ACC_ACCOUNT_BALANCE_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("lOCAL161_SPECIAL_ACCOUNT_ALLOCATION1_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("lOCAL161_SPECIAL_ACCOUNT_ALLOCATION2_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("lOCAL161_SPECIAL_ACCOUNT_ALLOCATION4_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_52_PREMERGER_TOTAL_QUALIFIED_YEARS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_161_PREMERGER_TOTAL_QUALIFIED_YEARS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_600_PREMERGER_TOTAL_QUALIFIED_YEARS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_666_PREMERGER_TOTAL_QUALIFIED_YEARS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_700_PREMERGER_TOTAL_QUALIFIED_YEARS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("EE_CONTRIBUTION_AMT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("STATUS_CODE_VALUE", typeof(string));

            ldtTempAddrDetails.Columns.Add("LOCAL_52_PENSION_CREDITS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_52_CREDITED_HOURS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_600_PENSION_CREDITS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_600_CREDITED_HOURS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_666_PENSION_CREDITS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_666_CREDITED_HOURS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_700_PENSION_CREDITS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_700_CREDITED_HOURS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_161_PENSION_CREDITS", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LOCAL_161_CREDITED_HOURS", typeof(decimal));

            ldtTempAddrDetails.Columns.Add("COMPUTATION_YEAR_BEGIN_DATE", typeof(DateTime));
            ldtTempAddrDetails.Columns.Add("COMPUTATION_YEAR_BEGIN_DATE_FORMAT", typeof(string));
            ldtTempAddrDetails.Columns.Add("DATE_BEFORE_COMPUTATION_YR_BEGIN", typeof(string));
            ldtTempAddrDetails.Columns.Add("COMPUTATION_YEAR_END_DATE", typeof(DateTime));
            ldtTempAddrDetails.Columns.Add("COMPUTATION_YEAR_END_DATE_FORMAT", typeof(string));

            ldtTempAddrDetails.Columns.Add("LATE_IAP_ADJUSTMENT_AMOUNT", typeof(decimal));

            ldtTempAddrDetails.Columns.Add("LATE_LOCAL52_SPECIAL_ACCOUNT_ADJUSTMENT_AMOUNT", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LATE_LOCAL161_SPECIAL_ACCOUNT_ADJUSTMENT_AMOUNT", typeof(decimal));

            ldtTempAddrDetails.Columns.Add("DIFF_ACCRUED_BENFIT_FOR_LATE_HOUR", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("LATE_HOUR_REPORTED_SUM", typeof(decimal));
            ldtTempAddrDetails.Columns.Add("NEGATIVE_QUALIFIED_YEARS", typeof(int));
            ldtTempAddrDetails.Columns.Add("LATE_EE_CONTRIBUTION", typeof(decimal));
            //Annual statement report changes PIR 960
            ldtTempAddrDetails.Columns.Add("CORRECTED_FLAG", typeof(bool));
            ldtTempAddrDetails.Columns.Add("ELIGIBLE_ACTIVE_INCR_FLAG", typeof(string));
            ldtTempAddrDetails.Columns.Add("MD_FLAG", typeof(string));
            ldtTempAddrDetails.Columns.Add("RETR_SPECIAL_ACCOUNT_FLAG", typeof(string));
            //RequestID: 59768
            ldtTempAddrDetails.Columns.Add("REEMPLOYED_UNDER_65_FLAG", typeof(string));
            //Ticket##128387 
            ldtTempAddrDetails.Columns.Add("PENSION_ONLY_FLAG", typeof(string));
            ldtTempAddrDetails.Columns.Add("REGISTERED_EMAIL_ADDRESS", typeof(string));

            if (adtbAnnualStatementParticipant != null && adtbAnnualStatementParticipant.Rows.Count > 0)
            {
                DataRow ldrTempAddrdetails = ldtTempAddrDetails.NewRow();
                ldrTempAddrdetails["person_name"] = adtbAnnualStatementParticipant.Rows[0]["person_name"];
                ldrTempAddrdetails["ADDR_LINE_1"] = adtbAnnualStatementParticipant.Rows[0]["ADDR_LINE_1"];
                ldrTempAddrdetails["ADDR_LINE_2"] = adtbAnnualStatementParticipant.Rows[0]["ADDR_LINE_2"];
                ldrTempAddrdetails["ADDR_CITY"] = adtbAnnualStatementParticipant.Rows[0]["ADDR_CITY"];
                ldrTempAddrdetails["ADDR_STATE_VALUE"] = adtbAnnualStatementParticipant.Rows[0]["ADDR_STATE_VALUE"];
                ldrTempAddrdetails["ADDR_ZIP_CODE"] = adtbAnnualStatementParticipant.Rows[0]["ADDR_ZIP_CODE"];
                ldrTempAddrdetails["LOCAL_52_PREMERGER_BENEFIT"] = adtbAnnualStatementParticipant.Rows[0]["LOCAL_52_PREMERGER_BENEFIT"];
                ldrTempAddrdetails["LOCAL_161_PREMERGER_BENEFIT"] = adtbAnnualStatementParticipant.Rows[0]["LOCAL_161_PREMERGER_BENEFIT"];
                ldrTempAddrdetails["LOCAL_600_PREMERGER_BENEFIT"] = adtbAnnualStatementParticipant.Rows[0]["LOCAL_600_PREMERGER_BENEFIT"];
                ldrTempAddrdetails["LOCAL_666_PREMERGER_BENEFIT"] = adtbAnnualStatementParticipant.Rows[0]["LOCAL_666_PREMERGER_BENEFIT"];
                ldrTempAddrdetails["LOCAL_700_PREMERGER_BENEFIT"] = adtbAnnualStatementParticipant.Rows[0]["LOCAL_700_PREMERGER_BENEFIT"];
                ldrTempAddrdetails["YTD_HOURS_BEFORE_LAST_COMP_YEAR"] = adtbAnnualStatementParticipant.Rows[0]["YTD_HOURS_BEFORE_LAST_COMP_YEAR"];
                ldrTempAddrdetails["ACCRUED_BENEFIT_TILL_LAST_COMP_YEAR"] = adtbAnnualStatementParticipant.Rows[0]["ACCRUED_BENEFIT_TILL_LAST_COMP_YEAR"];
                ldrTempAddrdetails["LATE_HOUR_REPORTED"] = adtbAnnualStatementParticipant.Rows[0]["LATE_HOUR_REPORTED"];
                ldrTempAddrdetails["TOTAL_QUALIFIED_YEARS"] = adtbAnnualStatementParticipant.Rows[0]["TOTAL_QUALIFIED_YEARS"];
                ldrTempAddrdetails["EE_AMT_PRIOR_YEAR"] = adtbAnnualStatementParticipant.Rows[0]["EE_AMT_PRIOR_YEAR"];
                ldrTempAddrdetails["ACCRUED_BENEFIT_FOR_PRIOR_YEAR"] = adtbAnnualStatementParticipant.Rows[0]["ACCRUED_BENEFIT_FOR_PRIOR_YEAR"];

                ldrTempAddrdetails["YEAR_BEFORE_LAST_YEAR"] = adtbAnnualStatementParticipant.Rows[0]["YEAR_BEFORE_LAST_YEAR"];
                ldrTempAddrdetails["LAST_YEAR"] = adtbAnnualStatementParticipant.Rows[0]["LAST_YEAR"];
                ldrTempAddrdetails["YTD_HOURS_FOR_LAST_COMP_YEAR"] = adtbAnnualStatementParticipant.Rows[0]["YTD_HOURS_FOR_LAST_COMP_YEAR"];
                ldrTempAddrdetails["PERSON_DOB"] = adtbAnnualStatementParticipant.Rows[0]["PERSON_DOB"];
                ldrTempAddrdetails["MPI_PERSON_ID"] = adtbAnnualStatementParticipant.Rows[0]["MPI_PERSON_ID"];

                ldrTempAddrdetails["LOCAL_52_FLAG"] = adtbAnnualStatementParticipant.Rows[0]["LOCAL_52_FLAG"];
                ldrTempAddrdetails["LOCAL_161_FLAG"] = adtbAnnualStatementParticipant.Rows[0]["LOCAL_161_FLAG"];
                ldrTempAddrdetails["LOCAL_666_FLAG"] = adtbAnnualStatementParticipant.Rows[0]["LOCAL_666_FLAG"];
                ldrTempAddrdetails["LOCAL_700_FLAG"] = adtbAnnualStatementParticipant.Rows[0]["LOCAL_700_FLAG"];
                ldrTempAddrdetails["LOCAL_600_FLAG"] = adtbAnnualStatementParticipant.Rows[0]["LOCAL_600_FLAG"];
                ldrTempAddrdetails["IAP_ALLOCATION1_AMT"] = adtbAnnualStatementParticipant.Rows[0]["IAP_ALLOCATION1_AMT"];
                ldrTempAddrdetails["IAP_ALLOCATION2_AMT"] = adtbAnnualStatementParticipant.Rows[0]["IAP_ALLOCATION2_AMT"];
                ldrTempAddrdetails["IAP_ALLOCATION2_INVESTMENT_AMT"] = adtbAnnualStatementParticipant.Rows[0]["IAP_ALLOCATION2_INVESTMENT_AMT"];
                ldrTempAddrdetails["IAP_ALLOCATION2_FORFIETURE_AMT"] = adtbAnnualStatementParticipant.Rows[0]["IAP_ALLOCATION2_FORFIETURE_AMT"];
                ldrTempAddrdetails["IAP_ALLOCATION4_AMT"] = adtbAnnualStatementParticipant.Rows[0]["IAP_ALLOCATION4_AMT"];
                ldrTempAddrdetails["IAP_ALLOCATION4_INVESTMENT_AMT"] = adtbAnnualStatementParticipant.Rows[0]["IAP_ALLOCATION4_INVESTMENT_AMT"];
                ldrTempAddrdetails["IAP_ALLOCATION4_FORFIETURE_AMT"] = adtbAnnualStatementParticipant.Rows[0]["IAP_ALLOCATION4_FORFIETURE_AMT"];
                ldrTempAddrdetails["CURRENT_PAYMENT_AMT"] = adtbAnnualStatementParticipant.Rows[0]["CURRENT_PAYMENT_AMT"];
                ldrTempAddrdetails["PRIOR_YEAR_IAP_ACCOUNT_BALANCE_AMT"] = adtbAnnualStatementParticipant.Rows[0]["PRIOR_YEAR_IAP_ACCOUNT_BALANCE_AMT"];
                ldrTempAddrdetails["PRIOR_YEAR_LOCAL52_SPECIAL_ACC_ACCOUNT_BALANCE_AMT"] = adtbAnnualStatementParticipant.Rows[0]["PRIOR_YEAR_LOCAL52_SPECIAL_ACC_ACCOUNT_BALANCE_AMT"];
                ldrTempAddrdetails["lOCAL52_SPECIAL_ACCOUNT_ALLOCATION1_AMT"] = adtbAnnualStatementParticipant.Rows[0]["lOCAL52_SPECIAL_ACCOUNT_ALLOCATION1_AMT"];
                ldrTempAddrdetails["lOCAL52_SPECIAL_ACCOUNT_ALLOCATION2_AMT"] = adtbAnnualStatementParticipant.Rows[0]["lOCAL52_SPECIAL_ACCOUNT_ALLOCATION2_AMT"];
                ldrTempAddrdetails["lOCAL52_SPECIAL_ACCOUNT_ALLOCATION4_AMT"] = adtbAnnualStatementParticipant.Rows[0]["lOCAL52_SPECIAL_ACCOUNT_ALLOCATION4_AMT"];
                ldrTempAddrdetails["PRIOR_YEAR_LOCAL161_SPECIAL_ACC_ACCOUNT_BALANCE_AMT"] = adtbAnnualStatementParticipant.Rows[0]["PRIOR_YEAR_LOCAL161_SPECIAL_ACC_ACCOUNT_BALANCE_AMT"];
                ldrTempAddrdetails["lOCAL161_SPECIAL_ACCOUNT_ALLOCATION1_AMT"] = adtbAnnualStatementParticipant.Rows[0]["lOCAL161_SPECIAL_ACCOUNT_ALLOCATION1_AMT"];
                ldrTempAddrdetails["lOCAL161_SPECIAL_ACCOUNT_ALLOCATION2_AMT"] = adtbAnnualStatementParticipant.Rows[0]["lOCAL161_SPECIAL_ACCOUNT_ALLOCATION2_AMT"];
                ldrTempAddrdetails["lOCAL161_SPECIAL_ACCOUNT_ALLOCATION4_AMT"] = adtbAnnualStatementParticipant.Rows[0]["lOCAL161_SPECIAL_ACCOUNT_ALLOCATION4_AMT"];
                ldrTempAddrdetails["LOCAL_52_PREMERGER_TOTAL_QUALIFIED_YEARS"] = adtbAnnualStatementParticipant.Rows[0]["LOCAL_52_PREMERGER_TOTAL_QUALIFIED_YEARS"];
                ldrTempAddrdetails["LOCAL_161_PREMERGER_TOTAL_QUALIFIED_YEARS"] = adtbAnnualStatementParticipant.Rows[0]["LOCAL_161_PREMERGER_TOTAL_QUALIFIED_YEARS"];
                ldrTempAddrdetails["LOCAL_600_PREMERGER_TOTAL_QUALIFIED_YEARS"] = adtbAnnualStatementParticipant.Rows[0]["LOCAL_600_PREMERGER_TOTAL_QUALIFIED_YEARS"];
                ldrTempAddrdetails["LOCAL_666_PREMERGER_TOTAL_QUALIFIED_YEARS"] = adtbAnnualStatementParticipant.Rows[0]["LOCAL_666_PREMERGER_TOTAL_QUALIFIED_YEARS"];
                ldrTempAddrdetails["LOCAL_700_PREMERGER_TOTAL_QUALIFIED_YEARS"] = adtbAnnualStatementParticipant.Rows[0]["LOCAL_700_PREMERGER_TOTAL_QUALIFIED_YEARS"];
                ldrTempAddrdetails["EE_CONTRIBUTION_AMT"] = adtbAnnualStatementParticipant.Rows[0]["EE_CONTRIBUTION_AMT"];
                ldrTempAddrdetails["STATUS_CODE_VALUE"] = adtbAnnualStatementParticipant.Rows[0]["STATUS_CODE_VALUE"];

                ldrTempAddrdetails["LOCAL_52_PENSION_CREDITS"] = adtbAnnualStatementParticipant.Rows[0]["LOCAL_52_PENSION_CREDITS"];
                ldrTempAddrdetails["LOCAL_52_CREDITED_HOURS"] = adtbAnnualStatementParticipant.Rows[0]["LOCAL_52_CREDITED_HOURS"];
                ldrTempAddrdetails["LOCAL_600_PENSION_CREDITS"] = adtbAnnualStatementParticipant.Rows[0]["LOCAL_600_PENSION_CREDITS"];
                ldrTempAddrdetails["LOCAL_600_CREDITED_HOURS"] = adtbAnnualStatementParticipant.Rows[0]["LOCAL_600_CREDITED_HOURS"];
                ldrTempAddrdetails["LOCAL_666_PENSION_CREDITS"] = adtbAnnualStatementParticipant.Rows[0]["LOCAL_666_PENSION_CREDITS"];
                ldrTempAddrdetails["LOCAL_666_CREDITED_HOURS"] = adtbAnnualStatementParticipant.Rows[0]["LOCAL_666_CREDITED_HOURS"];
                ldrTempAddrdetails["LOCAL_700_PENSION_CREDITS"] = adtbAnnualStatementParticipant.Rows[0]["LOCAL_700_PENSION_CREDITS"];
                ldrTempAddrdetails["LOCAL_700_CREDITED_HOURS"] = adtbAnnualStatementParticipant.Rows[0]["LOCAL_700_CREDITED_HOURS"];
                ldrTempAddrdetails["LOCAL_161_PENSION_CREDITS"] = adtbAnnualStatementParticipant.Rows[0]["LOCAL_161_PENSION_CREDITS"];
                ldrTempAddrdetails["LOCAL_161_CREDITED_HOURS"] = adtbAnnualStatementParticipant.Rows[0]["LOCAL_161_CREDITED_HOURS"];

                //Annual Statement Report Changes PIR 960
                ldrTempAddrdetails["CORRECTED_FLAG"] = ablnCorrectedFlag;
                //ChangeID: 57284
                ldrTempAddrdetails["ELIGIBLE_ACTIVE_INCR_FLAG"] = adtbAnnualStatementParticipant.Rows[0]["ELIGIBLE_ACTIVE_INCR_FLAG"];
                ldrTempAddrdetails["MD_FLAG"] = adtbAnnualStatementParticipant.Rows[0]["MD_FLAG"];
                ldrTempAddrdetails["RETR_SPECIAL_ACCOUNT_FLAG"] = adtbAnnualStatementParticipant.Rows[0]["RETR_SPECIAL_ACCOUNT_FLAG"];
                //RequestID: 59768
                ldrTempAddrdetails["REEMPLOYED_UNDER_65_FLAG"] = adtbAnnualStatementParticipant.Rows[0]["REEMPLOYED_UNDER_65_FLAG"];
                //Ticket##128387 
                ldrTempAddrdetails["PENSION_ONLY_FLAG"] = adtbAnnualStatementParticipant.Rows[0]["PENSION_ONLY_FLAG"];

                if (ablnIsBatchFlag == true && iobjSystemManagement == null)
                {
                    iobjSystemManagement = new busSystemManagement();
                    iobjSystemManagement.FindSystemManagement();
                }

                DateTime ldtComputationYearBeginDate = new DateTime();
                DateTime ldtDateBeforeComputationYrBegin = new DateTime();
                ldtComputationYearBeginDate = busGlobalFunctions.GetFirstDateOfComputationYear(iobjSystemManagement.icdoSystemManagement.batch_date.Year - 1);
                ldrTempAddrdetails["COMPUTATION_YEAR_BEGIN_DATE"] = ldtComputationYearBeginDate;

                string lstrCompYearBeginDtFormat = string.Empty;
                lstrCompYearBeginDtFormat = busGlobalFunctions.ConvertDateIntoDifFormat(ldtComputationYearBeginDate);
                ldrTempAddrdetails["COMPUTATION_YEAR_BEGIN_DATE_FORMAT"] = lstrCompYearBeginDtFormat;

                string lstrBeforeComputationYrBegin = string.Empty;
                ldtDateBeforeComputationYrBegin = ldtComputationYearBeginDate.AddDays(-1);
                lstrBeforeComputationYrBegin = busGlobalFunctions.ConvertDateIntoDifFormat(ldtDateBeforeComputationYrBegin);
                ldrTempAddrdetails["DATE_BEFORE_COMPUTATION_YR_BEGIN"] = lstrBeforeComputationYrBegin;

                DateTime ldtComputationYearEndDate = new DateTime();
                ldtComputationYearEndDate = busGlobalFunctions.GetLastDateOfComputationYear(iobjSystemManagement.icdoSystemManagement.batch_date.Year - 1);
                ldrTempAddrdetails["COMPUTATION_YEAR_END_DATE"] = ldtComputationYearEndDate;

                string lstrCompYearEndDtFormat = string.Empty;
                lstrCompYearEndDtFormat = busGlobalFunctions.ConvertDateIntoDifFormat(ldtComputationYearEndDate);
                ldrTempAddrdetails["COMPUTATION_YEAR_END_DATE_FORMAT"] = lstrCompYearEndDtFormat;

                ldrTempAddrdetails["LATE_IAP_ADJUSTMENT_AMOUNT"] = adtbAnnualStatementParticipant.Rows[0]["LATE_IAP_ADJUSTMENT_AMOUNT"];
                ldrTempAddrdetails["LATE_LOCAL52_SPECIAL_ACCOUNT_ADJUSTMENT_AMOUNT"] = adtbAnnualStatementParticipant.Rows[0]["LATE_LOCAL52_SPECIAL_ACCOUNT_ADJUSTMENT_AMOUNT"];
                ldrTempAddrdetails["LATE_LOCAL161_SPECIAL_ACCOUNT_ADJUSTMENT_AMOUNT"] = adtbAnnualStatementParticipant.Rows[0]["LATE_LOCAL161_SPECIAL_ACCOUNT_ADJUSTMENT_AMOUNT"];

                ldrTempAddrdetails["DIFF_ACCRUED_BENFIT_FOR_LATE_HOUR"] = adtbAnnualStatementParticipant.Rows[0]["DIFF_ACCRUED_BENFIT_FOR_LATE_HOUR"];
                ldrTempAddrdetails["LATE_HOUR_REPORTED_SUM"] = adtbAnnualStatementParticipant.Rows[0]["LATE_HOUR_REPORTED_SUM"];
                ldrTempAddrdetails["NEGATIVE_QUALIFIED_YEARS"] = adtbAnnualStatementParticipant.Rows[0]["NEGATIVE_QUALIFIED_YEARS"];
                ldrTempAddrdetails["LATE_EE_CONTRIBUTION"] = adtbAnnualStatementParticipant.Rows[0]["LATE_EE_CONTRIBUTION"];

                DataTable ldtbParticipantCommunicationPreference = busBase.Select("cdoPerson.GetCommunicationPreferences", new object[1] { adtbAnnualStatementParticipant.Rows[0]["PERSON_ID"] });
                if (ldtbParticipantCommunicationPreference != null && ldtbParticipantCommunicationPreference.Rows.Count > 0)
                {
                    ldrTempAddrdetails["REGISTERED_EMAIL_ADDRESS"] = ldtbParticipantCommunicationPreference.Rows[0]["REGISTERED_EMAIL_ADDRESS"];
                }
                ldtbParticipantCommunicationPreference.Dispose();

                ldtTempAddrDetails.Rows.Add(ldrTempAddrdetails);
                ldtTempAddrDetails.AcceptChanges();
                ldtTempAddrDetails.TableName = "ReportTable01";
            }

            int lComputationYear = 0;
            lComputationYear = iobjSystemManagement.icdoSystemManagement.batch_date.Year - 1;
            DataTable ldtTempDataForHours = new DataTable();

            DataTable ldtTempDataExtractionTable = new DataTable();
            ldtTempDataExtractionTable.Columns.Add(enmDataExtractionBatchHourInfo.employer_no.ToString(), typeof(string));
            ldtTempDataExtractionTable.Columns.Add(enmDataExtractionBatchHourInfo.employer_name.ToString(), typeof(string));
            ldtTempDataExtractionTable.Columns.Add(enmYearEndDataExtractionHeader.year.ToString(), typeof(int));
            ldtTempDataExtractionTable.Columns.Add(enmDataExtractionBatchHourInfo.hours_reported.ToString(), typeof(decimal));
            ldtTempDataExtractionTable.Columns.Add("LAST_YEAR", typeof(int));
            ldtTempDataExtractionTable.TableName = "ReportTable02";

            //CREATE TEMP TABLE TO STORE LATE HOURS

            DataTable ldtTempDataLateHoursTable = new DataTable();
            ldtTempDataLateHoursTable.Columns.Add(new DataColumn("employer_no_for_late_hrs", Type.GetType("System.String")));
            ldtTempDataLateHoursTable.Columns.Add(new DataColumn("employer_name_for_late_hrs", Type.GetType("System.String")));
            ldtTempDataLateHoursTable.Columns.Add(enmDataExtractionBatchHourInfo.computation_year.ToString(), typeof(int));
            ldtTempDataLateHoursTable.Columns.Add(enmDataExtractionBatchHourInfo.late_hour_reported.ToString(), typeof(decimal));
            ldtTempDataLateHoursTable.Columns.Add("LAST_YEAR", typeof(int));
            ldtTempDataLateHoursTable.TableName = "ReportTable03";

            DataTable ldtTotalHoursTable = new DataTable();
            ldtTotalHoursTable.Columns.Add("Total_Reported_Hours", typeof(decimal));
            ldtTotalHoursTable.TableName = "ReportTable04";

            var CheckIfCompYrexistsRow = (from DataRow dRow in adtbAnnualStatementParticipant.Rows
                                          where Convert.ToInt32(dRow["computation_year"]) == lComputationYear
                                          select Convert.ToInt32(dRow["computation_year"]));

            if (!CheckIfCompYrexistsRow.IsEmpty())
            {
                ldtTempDataForHours = adtbAnnualStatementParticipant.FilterTable(utlDataType.Numeric, enmDataExtractionBatchHourInfo.computation_year.ToString(), lComputationYear).CopyToDataTable();

                DataRow ldrTempDataExtractionTable;

                foreach (DataRow ldr in ldtTempDataForHours.Rows)
                {
                    ldrTempDataExtractionTable = ldtTempDataExtractionTable.NewRow();
                    ldrTempDataExtractionTable[enmDataExtractionBatchHourInfo.employer_no.ToString()] = ldr[enmDataExtractionBatchHourInfo.employer_no.ToString()];
                    ldrTempDataExtractionTable[enmDataExtractionBatchHourInfo.employer_name.ToString()] = ldr[enmDataExtractionBatchHourInfo.employer_name.ToString()];
                    ldrTempDataExtractionTable[enmYearEndDataExtractionHeader.year.ToString()] = ldr[enmYearEndDataExtractionHeader.year.ToString()];
                    ldrTempDataExtractionTable[enmDataExtractionBatchHourInfo.hours_reported.ToString()] = ldr[enmDataExtractionBatchHourInfo.hours_reported.ToString()];
                    ldrTempDataExtractionTable["LAST_YEAR"] = lComputationYear;

                    ldtTempDataExtractionTable.Rows.Add(ldrTempDataExtractionTable);
                }
            }
            DataRow ldrTempDataLateHoursTable;

            //foreach (DataRow ldr in ldtTempDataForHours.Rows)
            foreach (DataRow ldr in adtbAnnualStatementParticipant.Rows)
            {
                if ((ldr[enmDataExtractionBatchHourInfo.late_hour_reported.ToString()] != DBNull.Value))
                {
                    if (Convert.ToDecimal(ldr[enmDataExtractionBatchHourInfo.late_hour_reported.ToString()]) != 0.0M)
                    {
                        ldrTempDataLateHoursTable = ldtTempDataLateHoursTable.NewRow();
                        ldrTempDataLateHoursTable["employer_no_for_late_hrs"] = ldr[enmDataExtractionBatchHourInfo.employer_no.ToString()];
                        ldrTempDataLateHoursTable["employer_name_for_late_hrs"] = ldr[enmDataExtractionBatchHourInfo.employer_name.ToString()];
                        ldrTempDataLateHoursTable[enmDataExtractionBatchHourInfo.computation_year.ToString()] = ldr[enmDataExtractionBatchHourInfo.computation_year.ToString()];
                        ldrTempDataLateHoursTable[enmDataExtractionBatchHourInfo.late_hour_reported.ToString()] = ldr[enmDataExtractionBatchHourInfo.late_hour_reported.ToString()];
                        ldrTempDataLateHoursTable["LAST_YEAR"] = lComputationYear;
                        ldtTempDataLateHoursTable.Rows.Add(ldrTempDataLateHoursTable);
                    }
                }
            }

            #region Total of Regular and adjusted Hours

            DataRow ldtTotalHoursRow = ldtTotalHoursTable.NewRow();

            decimal idtRegularHours = decimal.Zero;
            decimal idtLateHours = decimal.Zero;

            if ((from obj in ldtTempDataExtractionTable.AsEnumerable()
                 select obj.Field<decimal>(enmDataExtractionBatchHourInfo.hours_reported.ToString())).Count() > 0)

                idtRegularHours = (from obj in ldtTempDataExtractionTable.AsEnumerable()
                                   select obj.Field<decimal>(enmDataExtractionBatchHourInfo.hours_reported.ToString())).Sum();

            if ((from obj in ldtTempDataLateHoursTable.AsEnumerable()
                 select obj.Field<decimal>(enmDataExtractionBatchHourInfo.late_hour_reported.ToString())).Count() > 0)

                idtLateHours = (from obj in ldtTempDataLateHoursTable.AsEnumerable()
                                select obj.Field<decimal>(enmDataExtractionBatchHourInfo.late_hour_reported.ToString())).Sum();

            ldtTotalHoursRow["Total_Reported_Hours"] = idtRegularHours + idtLateHours;
            ldtTotalHoursTable.Rows.Add(ldtTotalHoursRow);

            #endregion
            // }

            if (adtbAnnualStatementParticipant.Rows.Count > 1)
            {
                adtbAnnualStatementParticipant.Rows.Remove(adtbAnnualStatementParticipant.Rows[0]);
                adtbAnnualStatementParticipant.AcceptChanges();
            }
            ldtReportResult.Tables.Add(ldtTempAddrDetails.Copy());
            ldtReportResult.Tables.Add(ldtTempDataExtractionTable.Copy());
            ldtReportResult.Tables.Add(ldtTempDataLateHoursTable.Copy());
            ldtReportResult.Tables.Add(ldtTotalHoursTable.Copy());

            List<string> llstGeneratedReports = new List<string>();
            busCreateReports lobjCreateReports = new busCreateReports();

            if (ablnIsBatchFlag == false)
            {
                //Annual Statement Report Changes PIR 960
                lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt19_AnnualReportGenerated", "", adtbAnnualStatementParticipant.Rows[0]["MPI_PERSON_ID"].ToString());
                llstGeneratedReports.Add(lstrReportPath);
                System.Diagnostics.Process.Start(lstrReportPath);
            }
            else
            {
                lstrReportPath = lobjCreateReports.CreatePDFReport(ldtReportResult, "rpt19_AnnualReportGenerated", "", adtbAnnualStatementParticipant.Rows[0]["MPI_PERSON_ID"].ToString());
                llstGeneratedReports.Add(lstrReportPath);
            }
        }

        #region GetTrueUnionCodeBySSN
        public string GetTrueUnionCodeBySSN(string astrSSN)
        {
            IDbConnection lconLegacy = null;
            if (lconLegacy == null)
            {
                lconLegacy = DBFunction.GetDBConnection("Legacy");
            }

            Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();

            IDbDataParameter lobjParameter = DBFunction.GetDBParameter();
            lobjParameter.ParameterName = "@SSN";
            lobjParameter.DbType = DbType.String;
            lobjParameter.Value = astrSSN.ToLower();
            lcolParameters.Add(lobjParameter);

            DateTime ToDate = busGlobalFunctions.GetLastDateOfComputationYear(DateTime.Now.Year);
            DateTime FromDate = busGlobalFunctions.GetLastDateOfComputationYear(DateTime.Now.Year - 1);
            FromDate = FromDate.AddDays(1);

            IDbDataParameter lobjParameter2 = DBFunction.GetDBParameter();
            lobjParameter2.ParameterName = "@FromDate";
            lobjParameter2.DbType = DbType.DateTime;
            lobjParameter2.Value = FromDate;
            lcolParameters.Add(lobjParameter2);

            IDbDataParameter lobjParameter3 = DBFunction.GetDBParameter();
            lobjParameter3.ParameterName = "@ToDate";
            lobjParameter3.DbType = DbType.DateTime;
            lobjParameter3.Value = ToDate;
            lcolParameters.Add(lobjParameter3);

            IDbDataParameter lobjParameter1 = DBFunction.GetDBParameter();
            lobjParameter1.ParameterName = "@TrueUnion";
            lobjParameter1.DbType = DbType.Int32;
            lobjParameter1.Direction = ParameterDirection.ReturnValue;
            lcolParameters.Add(lobjParameter1);

            DBFunction.DBExecuteProcedure("fn_GetTrueUnionBy_SSN_N_Date_OldWay", lcolParameters, lconLegacy, null);

            lconLegacy.Close();

            return lcolParameters[3].Value.ToString();
        }
        #endregion


        #region GetEmployerNameBySSN
        public void GetEmployerNameBySSN(string astrSSN, ref string astrEmployerName, ref int aintUnionCode)
        {
            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;
            Collection<IDbDataParameter> lcolParameters1 = new Collection<IDbDataParameter>();
            SqlParameter[] parameters = new SqlParameter[1];
            SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
            param1.Value = astrSSN.ToLower();
            parameters[0] = param1;

            DataTable ldtbIAPInfo = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetWorkDataForSSNMerge", astrLegacyDBConnetion, null, parameters);
            if (ldtbIAPInfo != null)
            {
                var CorrectedRows = (from c in ldtbIAPInfo.AsEnumerable()
                                     where c.Field<string>("EmployerName").IsNotNullOrEmpty()
                                     && c.Field<DateTime?>("ToDate") != null
                                     select new
                                     {
                                         EmployerName = c.Field<string>("EmployerName"),
                                         ToDate = c.Field<DateTime>("ToDate"),
                                         UnionCode = c.Field<int>("UnionCode"),

                                     });
                //CorrectedRows = CorrectedRows.OrderBy(o => o.ToDate);
                if (CorrectedRows.Count() != 0)
                {
                    astrEmployerName = CorrectedRows.FirstOrDefault().EmployerName;
                    aintUnionCode = CorrectedRows.FirstOrDefault().UnionCode;
                }
            }
            //return null;


        }
        #endregion

        #region GetMailingAddress

        public string GetMailingAddress(int aintPersonId)
        {
            DataTable ldtbParticipantAddress = busBase.Select("cdoPerson.GetMailingAddrByPersonID", new object[1] { aintPersonId });
            if (ldtbParticipantAddress.Rows.Count > 0 && !Convert.ToBoolean(ldtbParticipantAddress.Rows[0][0].IsDBNull()))
            {
                return ldtbParticipantAddress.Rows[0]["istrParticipantAddress"].ToString();
            }
            return null;
        }
        #endregion

        public Collection<busBenefitApplication> iclbDocumentsReceived { get; set; }

        public Collection<busBenefitApplication> iclbDocumentsPending { get; set; }

        #region MSS

        public void LoadActiveAddressOfMember()
        {
            bool blnCheckMail = false;
            ibusPersonAddressForCorr = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() };
            iblnMssCheckForActiveAddress = false;
            if (this.iclbPersonAddress.IsNullOrEmpty())
            {
                this.iclbPersonAddress = new Collection<busPersonAddress>();
                this.LoadPersonAddresss();
            }
            if (!this.iclbPersonAddress.IsNullOrEmpty())
            {
                foreach (busPersonAddress objbusPersonAddress in this.iclbPersonAddress)
                {
                    if ((objbusPersonAddress.icdoPersonAddress.end_date >= DateTime.Now || objbusPersonAddress.icdoPersonAddress.end_date == DateTime.MinValue) &&
                        objbusPersonAddress.icdoPersonAddress.start_date <= DateTime.Now && objbusPersonAddress.icdoPersonAddress.start_date != objbusPersonAddress.icdoPersonAddress.end_date)
                    {
                        iblnMssCheckForActiveAddress = true;
                        if (Convert.ToInt32(objbusPersonAddress.icdoPersonAddress.addr_country_value) == busConstant.AUSTRALIA || Convert.ToInt32(objbusPersonAddress.icdoPersonAddress.addr_country_value) == busConstant.CANADA || Convert.ToInt32(objbusPersonAddress.icdoPersonAddress.addr_country_value) == busConstant.MEXICO
                   || Convert.ToInt32(objbusPersonAddress.icdoPersonAddress.addr_country_value) == busConstant.NewZealand || Convert.ToInt32(objbusPersonAddress.icdoPersonAddress.addr_country_value) == busConstant.OTHER_PROVINCE)
                        {
                            cdoCodeValue lobjcdoCodeValue =
                            HelperUtil.GetCodeValueDetails(152,
                                                           objbusPersonAddress.icdoPersonAddress.addr_state_value);
                            objbusPersonAddress.icdoPersonAddress.addr_state_description = lobjcdoCodeValue.description;
                        }
                        ibusPersonAddressForCorr = objbusPersonAddress;

                        if (!objbusPersonAddress.iclcPersonAddressChklist.IsNullOrEmpty())
                        {
                            foreach (cdoPersonAddressChklist lcdoPersonAddressChklist in objbusPersonAddress.iclcPersonAddressChklist)
                            {
                                if (lcdoPersonAddressChklist.address_type_value == "MAIL")
                                {
                                    blnCheckMail = true;
                                }

                            }
                        }
                    }
                    if (blnCheckMail && iblnMssCheckForActiveAddress) //rid 83291
                    {
                        break;
                    }
                }
                if (ibusPersonAddressForCorr.icdoPersonAddress.address_id == 0)
                {
                    ibusPersonAddressForCorr = this.iclbPersonAddress.OrderByDescending(item => item.icdoPersonAddress.start_date).FirstOrDefault();
                    if (Convert.ToInt32(ibusPersonAddressForCorr.icdoPersonAddress.addr_country_value) == busConstant.AUSTRALIA || Convert.ToInt32(ibusPersonAddressForCorr.icdoPersonAddress.addr_country_value) == busConstant.CANADA || Convert.ToInt32(ibusPersonAddressForCorr.icdoPersonAddress.addr_country_value) == busConstant.MEXICO
                  || Convert.ToInt32(ibusPersonAddressForCorr.icdoPersonAddress.addr_country_value) == busConstant.NewZealand || Convert.ToInt32(ibusPersonAddressForCorr.icdoPersonAddress.addr_country_value) == busConstant.OTHER_PROVINCE)
                    {
                        cdoCodeValue lobjcdoCodeValue =
                        HelperUtil.GetCodeValueDetails(152,
                                                       ibusPersonAddressForCorr.icdoPersonAddress.addr_state_value);
                        ibusPersonAddressForCorr.icdoPersonAddress.addr_state_description = lobjcdoCodeValue.description;
                    }
                }
            }
        }

        public DataTable LoadMemberWorkHistory()
        {
            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

            SqlParameter[] parameters = new SqlParameter[1];
            SqlParameter param1 = new SqlParameter("@SSN", DbType.String);

            param1.Value = icdoPerson.ssn;
            parameters[0] = param1;

            DataTable ldtPersonWorkHistory = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetWeeklyWorkData", astrLegacyDBConnetion, null, parameters);

            return ldtPersonWorkHistory;
        }

        public byte[] CreatePDFReport(string astrReportName)
        {
            ReportViewer rvViewer = new ReportViewer();
            Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;
            string labsRptDefPath = string.Empty;

            DataTable ldtbReportTable = LoadMemberWorkHistory();
            ldtbReportTable.TableName = astrReportName;

            rvViewer.ProcessingMode = ProcessingMode.Local;
            labsRptDefPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION);

            rvViewer.LocalReport.ReportPath = labsRptDefPath + astrReportName + ".rdlc";
            ReportDataSource lrdsReport = new ReportDataSource(ldtbReportTable.TableName, ldtbReportTable);

            rvViewer.LocalReport.DataSources.Add(lrdsReport);
            byte[] bytes = rvViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

            return bytes;
        }

        public void CheckIFRetireeForMss()
        {
            DataTable ldtblist = busPerson.Select("cdoPersonAccount.GetPlanForPersonOverview", new object[1] { this.icdoPerson.person_id });
            if (ldtblist.IsNotNull() && ldtblist.Rows.Count > 0)
            {
                if (!(ldtblist.AsEnumerable().Where(row => Convert.ToString(row[enmPersonAccount.status_value.ToString()]) == "ACTV").Count() > 0))
                {
                    this.istrRetiree = busConstant.FLAG_YES;
                    DataTable ldtbApplications = busPerson.Select("cdoMssBenefitCalculationHeader.GetApprovedApplications", new object[1] { this.icdoPerson.person_id });
                    if (ldtbApplications.AsEnumerable().Where(row => Convert.ToDateTime(row[enmBenefitApplication.retirement_date.ToString()]).Year == DateTime.Now.Year).Count() > 0)
                    {
                        this.istrRetiredInCurrentYear = busConstant.FLAG_YES;
                    }

                }
            }
        }

        public void CheckIfRetiredInCurrentYear()
        {

        }
        #endregion
        public Collection<busReturnedMail> iclbReturnedMail { get; set; }
        public void LoadReturnedMail()
        {
            DataTable ldtblist = busPerson.Select("cdoReturnedMail.LoadReturnMailForPerson", new object[1] { this.icdoPerson.person_id });
            if (ldtblist != null && ldtblist.Rows.Count > 0)
            {
                iclbReturnedMail = new Collection<busReturnedMail>();
                foreach (DataRow dr in ldtblist.Rows)
                {
                    busReturnedMail lbusReturnedMail = new busReturnedMail { icdoReturnedMail = new cdoReturnedMail() };
                    lbusReturnedMail.icdoReturnedMail.LoadData(dr);
                    lbusReturnedMail.ibusPersonAddress = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() };
                    lbusReturnedMail.ibusPersonAddress.icdoPersonAddress.LoadData(dr);
                    lbusReturnedMail.ibusDocument = new busDocument { icdoDocument = new cdoDocument() };
                    lbusReturnedMail.ibusDocument.icdoDocument.LoadData(dr);
                    iclbReturnedMail.Add(lbusReturnedMail);
                }
            }
            // iclbReturnedMail = GetCollection<busReturnedMail>(ldtblist, "icdoReturnedMail");
        }


        //Ticket : 55015
        public bool CheckIfVIPManagerLogin()
        {
            busUserRoles lbusUserRoles = new busUserRoles { icdoRoles = new cdoRoles() };
            if (lbusUserRoles.FindUserRoles(iobjPassInfo.iintUserSerialID, busConstant.Role.VIP_MANAGER_ROLE))
                return true;
            else
                return false;

        }
        // LA Sunset 
        public bool CheckIfManager()
        {
            busUserRoles lbusUserRoles = new busUserRoles { icdoRoles = new cdoRoles() };
            if (lbusUserRoles.FindUserRoles(iobjPassInfo.iintUserSerialID, busConstant.Role.MANAGER_ROLE))
            {
                return true;
            }
            return false;
        }

        public ArrayList btn_DeleteDependent(int aintPersonRelationshipId)
        {
            utlError lobjErr = new utlError();
            ArrayList larr = new ArrayList();

            try
            {
                if (this.iclbPersonDependent != null && iclbPersonDependent.Count() > 0
                    && iclbPersonDependent.Where(item => item.icdoRelationship.person_relationship_id == aintPersonRelationshipId).Count() > 0)
                {
                    busPersonDependent lbusPersonDependent = new busPersonDependent { icdoRelationship = new cdoRelationship() };
                    lbusPersonDependent = iclbPersonDependent.Where(item => item.icdoRelationship.person_relationship_id == aintPersonRelationshipId).FirstOrDefault();
                    busPerson lbusDependent = new busPerson { icdoPerson = new cdoPerson() };
                    lbusDependent.FindPerson(lbusPersonDependent.icdoRelationship.dependent_person_id);
                    bool lblnIsParticipant = lbusDependent.CheckMemberIsParticipant();


                    var transactionOptions = new TransactionOptions();
                    transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                    transactionOptions.Timeout = TransactionManager.MaximumTimeout;

                    using (TransactionScope ds = new TransactionScope(TransactionScopeOption.Required, transactionOptions)) // Distributed Transcation Co-ordinator should be SWITCHED ON on the server
                    {
                        //if (lobjLegacyPassInfoHEDB.IsNull())
                        //{
                        //    lobjLegacyPassInfoHEDB = new utlPassInfo();
                        //}

                        //Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();

                        //lobjLegacyPassInfoHEDB.iconFramework = DBFunction.GetDBConnection("HELegacy");


                        //if (lobjLegacyPassInfoHEDB.iconFramework != null)
                        //{
                        //    IDbDataParameter lobjParameter1 = DBFunction.GetDBParameter();
                        //    lobjParameter1.ParameterName = "@participantID";
                        //    lobjParameter1.DbType = DbType.String;
                        //    lobjParameter1.Value = this.icdoPerson.mpi_person_id;
                        //    lcolParameters.Add(lobjParameter1);


                        //    IDbDataParameter lobjParameter2 = DBFunction.GetDBParameter();
                        //    lobjParameter2.ParameterName = "@dependentID";
                        //    lobjParameter2.DbType = DbType.String;
                        //    lobjParameter2.Value = lbusDependent.icdoPerson.mpi_person_id;
                        //    lcolParameters.Add(lobjParameter2);

                        //    DBFunction.DBExecuteProcedure("usp_OPUS_Dependent_del", lcolParameters, lobjLegacyPassInfoHEDB.iconFramework, lobjLegacyPassInfoHEDB.itrnFramework);
                        //}

                        
                        if (lbusPersonDependent.icdoRelationship.beneficiary_person_id > 0 || lbusPersonDependent.icdoRelationship.beneficiary_org_id > 0)
                        {
                            lbusPersonDependent.icdoRelationship.dependent_person_id = 0;
                            lbusPersonDependent.icdoRelationship.Update();
                        }
                        else
                        {
                            lbusPersonDependent.icdoRelationship.Delete();
                        }

                        int lintCount = (int)DBFunction.DBExecuteScalar("cdoPerson.CheckIfPersonIsOnlyDependent", new object[1] { lbusPersonDependent.icdoRelationship.dependent_person_id },
                                                                                  iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                        if (!lblnIsParticipant && lintCount > 0)
                        {
                            lbusDependent.icdoPerson.is_person_deleted_flag = busConstant.FLAG_YES;
                            lbusDependent.icdoPerson.Update();
                        }

                        iclbPersonDependent.Remove(iclbPersonDependent.Where(item => item.icdoRelationship.person_relationship_id == aintPersonRelationshipId).FirstOrDefault());

                        ds.Complete();
                    }                 
                }
            }
            catch (Exception e)
            {
                if (iobjPassInfo.itrnFramework.IsNotNull())
                {
                    
                    utlError lError = new utlError();
                    if (e.Message.IsNotNullOrEmpty())
                    {
                        int lintStartIndex = e.Message.IndexOf('\'');
                        int lintEndIndex = e.Message.LastIndexOf('\'');

                        if (lintStartIndex > 0 && lintEndIndex > lintStartIndex)
                        {
                            lError.istrErrorMessage = e.Message.Substring(lintStartIndex + 1, lintEndIndex - lintStartIndex -1).ToString();
                        }
                        else
                            lError.istrErrorMessage = e.Message.ToString();
                    }
                    else
                        lError.istrErrorMessage = "Error Occured/All transactions have been RollBacked";

                    larr.Add(lError);
                    return larr;

                    throw (e);
                }
            }
            finally
            {

                //if (lobjLegacyPassInfoHEDB.IsNotNull() && lobjLegacyPassInfoHEDB.iconFramework != null &&
                //    lobjLegacyPassInfoHEDB.iconFramework.State == ConnectionState.Open)
                //{
                //    lobjLegacyPassInfoHEDB.iconFramework.Close();
                //    lobjLegacyPassInfoHEDB.iconFramework.Dispose();
                //}
            }

            larr.Add(this);
            return larr;
        }

        public Collection<busCorTracking> LoadPacketCorrespondences()
        {
            DataTable ldtbList = Select("cdoCorPacketContent.GetCorTrackingDetails", new object[1] { icdoPerson.person_id });
            iclbCorTracking = GetCollection<busCorTracking>(ldtbList, "icdoCorTracking");

            if (iclbCorTracking != null && iclbCorTracking.Count > 0)
            {
                foreach (busCorTracking lbusCorTracking in iclbCorTracking)
                {
                    lbusCorTracking.ibusCorTemplates = new busCorTemplates { icdoCorTemplates = new cdoCorTemplates() };
                    lbusCorTracking.ibusCorTemplates.FindCorTemplates(lbusCorTracking.icdoCorTracking.template_id);

                    lbusCorTracking.ibusCorPacketContentTracking = new busCorPacketContentTracking { icdoCorPacketContentTracking = new cdoCorPacketContentTracking() };
                    lbusCorTracking.ibusCorPacketContentTracking.FindCorPacketContentTracking(lbusCorTracking.icdoCorTracking.tracking_id);

                    //if (lbusCorTracking.icdoCorTracking.packet_status_value.IsNotNullOrEmpty())
                    //    lbusCorTracking.icdoCorTracking.packet_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(7094, lbusCorTracking.icdoCorTracking.packet_status_value);
                }
            }

            return iclbCorTracking;
        }
        //Ticket#76267
        public bool CheckHealthEligibilityEffectiveDate()
        {
            int leligilibiltyAge = 0;

            busPersonOverview lbusPersonOverview = new busPersonOverview { icdoPerson = new cdoPerson() };
           
            lbusPersonOverview.FindPerson(this.icdoPerson.person_id);
            //lbusPersonOverview.idecAge = busGlobalFunctions.CalculatePersonAgeInDec(lbusPersonOverview.icdoPerson.idtDateofBirth, DateTime.Now);
            lbusPersonOverview.lbusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            lbusPersonOverview.lbusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lbusPersonOverview.lbusBenefitApplication.ibusPerson.icdoPerson = lbusPersonOverview.icdoPerson;
            lbusPersonOverview.lbusBenefitApplication.ibusPerson.LoadPersonAccounts();
            DateTime ldtEffectivedate = Convert.ToDateTime(this.icdoPerson.retirement_health_date).Date;
            decimal ldecAgeAsOfRetirement = Math.Floor(busGlobalFunctions.CalculatePersonAgeInDec(this.icdoPerson.idtDateofBirth, Convert.ToDateTime(this.icdoPerson.retirement_health_date)));
            //Ticket#96556
            lbusPersonOverview.lbusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans();
            // lbusPersonOverview.GetRetireeHealthHours();
            lbusPersonOverview.iclbHealthWorkHistory = lbusPersonOverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI;
            var iflagRule = lbusPersonOverview.CheckForHealthEligibility();
           
            if (lbusPersonOverview.icdoPerson.istrRule == busConstant.RULE_1)
            {
                leligilibiltyAge = 62;
                if (ldecAgeAsOfRetirement >= leligilibiltyAge)
                {
                    if (lbusPersonOverview.icdoPerson.idtDateofBirth.AddYears(leligilibiltyAge).Day != 1)
                        ldtEffectivedate = lbusPersonOverview.icdoPerson.idtDateofBirth.AddYears(leligilibiltyAge).GetLastDayofMonth().AddDays(1);
                    else
                        ldtEffectivedate = lbusPersonOverview.icdoPerson.idtDateofBirth.AddYears(leligilibiltyAge);
                    if (this.icdoPerson.retirement_health_date >= ldtEffectivedate)
                    {
                        return true;
                    }
                }
             }
            else if (lbusPersonOverview.icdoPerson.istrRule == busConstant.RULE_2)
            {
                leligilibiltyAge = 62;
                if (ldecAgeAsOfRetirement >= leligilibiltyAge)
                {
                    if (lbusPersonOverview.icdoPerson.idtDateofBirth.AddYears(leligilibiltyAge).Day != 1)
                        ldtEffectivedate = lbusPersonOverview.icdoPerson.idtDateofBirth.AddYears(leligilibiltyAge).GetLastDayofMonth().AddDays(1);
                    else
                        ldtEffectivedate = lbusPersonOverview.icdoPerson.idtDateofBirth.AddYears(leligilibiltyAge);

                    if (this.icdoPerson.retirement_health_date >= ldtEffectivedate)
                    {
                        return true;
                    }
                }
            }
            else if (lbusPersonOverview.icdoPerson.istrRule == busConstant.RULE_3)
            {
                leligilibiltyAge = 61;
                if (ldecAgeAsOfRetirement >= leligilibiltyAge)
                {
                    if (lbusPersonOverview.icdoPerson.idtDateofBirth.AddYears(leligilibiltyAge).Day != 1)
                        ldtEffectivedate = lbusPersonOverview.icdoPerson.idtDateofBirth.AddYears(leligilibiltyAge).GetLastDayofMonth().AddDays(1);
                    else
                        ldtEffectivedate = lbusPersonOverview.icdoPerson.idtDateofBirth.AddYears(leligilibiltyAge);

                    if (this.icdoPerson.retirement_health_date >= ldtEffectivedate)
                    {
                        return true;
                    }
                }
            }
            else if (lbusPersonOverview.icdoPerson.istrRule == busConstant.RULE_4)
            {
                leligilibiltyAge = 60;
                if (ldecAgeAsOfRetirement >= leligilibiltyAge)
                {
                    if (lbusPersonOverview.icdoPerson.idtDateofBirth.AddYears(leligilibiltyAge).Day != 1)
                        ldtEffectivedate = lbusPersonOverview.icdoPerson.idtDateofBirth.AddYears(leligilibiltyAge).GetLastDayofMonth().AddDays(1);
                    else
                        ldtEffectivedate = lbusPersonOverview.icdoPerson.idtDateofBirth.AddYears(leligilibiltyAge);

                    if (this.icdoPerson.retirement_health_date >= ldtEffectivedate)
                    {
                        return true;
                    }
                }
            }
            else if (lbusPersonOverview.icdoPerson.istrRule == busConstant.RULE_5)
            {
                leligilibiltyAge = 62;
                if (ldecAgeAsOfRetirement >= leligilibiltyAge)
                {
                    if (lbusPersonOverview.icdoPerson.idtDateofBirth.AddYears(leligilibiltyAge).Day != 1)
                        ldtEffectivedate = lbusPersonOverview.icdoPerson.idtDateofBirth.AddYears(leligilibiltyAge).GetLastDayofMonth().AddDays(1);
                    else
                        ldtEffectivedate = lbusPersonOverview.icdoPerson.idtDateofBirth.AddYears(leligilibiltyAge);

                    if (this.icdoPerson.retirement_health_date >= ldtEffectivedate)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public Collection<cdoPerson> GetAnalystNames()
        {
            Collection<cdoPerson> lclcAnalystNames = null;
           
                DataTable ldtbList = Select("cdoPerson.GetCaseAnalyst", new object[0] );
                if (ldtbList.Rows.Count > 0)
                {
                lclcAnalystNames = doBase.GetCollection<cdoPerson>(ldtbList);
                }
 
            return lclcAnalystNames;
        }

        public void GetCaseAnalystById(int person_id)
        {
            iintAnalystId = 0;
            DataTable ldtbList = Select("cdoPerson.GetCaseAnalystById", new object[1] { person_id });
            if (ldtbList.Rows.Count > 0)
            {
               iintAnalystId = Convert.ToInt32(ldtbList.Rows[0][1]);
               this.icdoPerson.analyst_id = Convert.ToInt32(ldtbList.Rows[0][1]);

            }

           
        }

        public Collection<cdoPerson> GetMDOption()
        {
            Collection<cdoPerson> lclMDAge = null;

            DataTable ldtbList = Select("cdoPerson.GetMDOption", new object[0]);
            if (ldtbList.Rows.Count > 0)
            {
                lclMDAge = doBase.GetCollection<cdoPerson>(ldtbList);

                //if(this.icdoPerson.md_age_opt_id == 0)
                //{
                //    this.icdoPerson.md_age_opt_id = 1;
                //}
            }

            return lclMDAge;
        }

        public void GetMDAgeById(int person_id)
        {
            iintAnalystId = 0;
            DataTable ldtbList = Select("cdoPerson.GetMDAgeById", new object[1] { person_id });
            if (ldtbList.Rows.Count > 0)
            {
              //  iintAnalystId = Convert.ToInt32(ldtbList.Rows[0]["MD_AGE_OPT_ID"]);
                this.icdoPerson.md_age_opt_id = Convert.ToInt32(ldtbList.Rows[0]["MD_AGE_OPT_ID"]);

            }

            //busGlobalFunctions.CalculatePersonAge(this.icdoPerson.date_of_birth, DateTime.Today);
            //HelperUtil.CalculateAgeForGivenDateUsingAgeCalculationMethodology(this.icdoPerson.date_of_birth, DateTime.Today);

        }
        public void BtnSubmitClick(int aintPersonId)
        {
            DataTable ldtProcessID = busPerson.Select("entSolBpmRequest.GetProcessIdByCaseName", new object[1] { busConstant.PersonAccountMaintenance.APPLICATION_SERVICE_RETIREMENT_BPM });
            int ProcessId = 0; 
            if (ldtProcessID.Rows.Count > 0)
            {
                ProcessId = Convert.ToInt32(ldtProcessID.Rows[0]["PROCESS_ID"]);
            }
            UpdateQDROLegalReviewStatus(aintPersonId);

            DataTable ldtblist = busPerson.Select("cdoBenefitApplication.LoadBenefitApplications", new object[1] { aintPersonId });
            if (ldtblist.Rows.Count > 0)
            {
                foreach (DataRow row in ldtblist.Rows)
                {
                    Hashtable lhstRequestParameters = new Hashtable();
                    lhstRequestParameters.Add("ReferenceId", Convert.ToInt32(row["BENEFIT_APPLICATION_DETAIL_ID"]));
                    lhstRequestParameters.Add(busConstant.PersonAccountMaintenance.APPLICATION_ID, Convert.ToInt32(row["BENEFIT_APPLICATION_ID"]));
                    lhstRequestParameters.Add(busConstant.PersonAccountMaintenance.PERSON_ID, aintPersonId);
                    lhstRequestParameters.Add(busConstant.PersonAccountMaintenance.PLAN_ID, Convert.ToInt32(row["iintPlanId"]));
                    lhstRequestParameters.Add(busConstant.PersonAccountMaintenance.PLAN_DESCRIPTION, Convert.ToString(row["istrPlanDescription"]));
                    busSolBpmRequest lbusRequest = new busSolBpmRequest();
                    lbusRequest.icdoBpmRequest.person_id = aintPersonId;
                    lbusRequest.icdoBpmRequest.reference_id = Convert.ToInt32(row["BENEFIT_APPLICATION_DETAIL_ID"]);
                    lbusRequest.icdoBpmRequest.process_id = ProcessId;
                    lbusRequest.AddRequest(lhstRequestParameters);   
                }
            }
        }

        public void UpdateQDROLegalReviewStatus(int aintPersonId)
        {
            DataTable ldtblist = busPerson.Select("entPersonAccount.QDROLegalReviewRequiredForApplicationMaintenance", new object[1] { aintPersonId });

            if (ldtblist.Rows.Count > 0)
            {
                var PersonAccountMPIPlan = ldtblist.AsEnumerable().Where(row => row.Field<int>("PLAN_ID") == busConstant.MPIPP_PLAN_ID
                       && row.Field<string>("QDRO_LEGAL_REVIEW_REQUIRED") == busConstant.FLAG_YES).Select(row => row.Field<int>("PERSON_ACCOUNT_ID")).FirstOrDefault();

                var qdroDate = ldtblist.AsEnumerable().Where(row => row.Field<int>("PLAN_ID") == busConstant.MPIPP_PLAN_ID
                               && row.Field<string>("QDRO_LEGAL_REVIEW_REQUIRED") == busConstant.FLAG_YES)
                              .Select(row => row.Field<DateTime?>("QDRO_REVIEW_COMPLETED_DATE")).FirstOrDefault();

                var PersonAccountIAPPlan = ldtblist.AsEnumerable().Where(row => row.Field<int>("PLAN_ID") == busConstant.IAP_PLAN_ID
                      && row.Field<string>("QDRO_LEGAL_REVIEW_REQUIRED") == busConstant.FLAG_YES).Select(row => row.Field<int>("PERSON_ACCOUNT_ID")).FirstOrDefault();

                var PersonAccountLocal52Plan = ldtblist.AsEnumerable().Where(row => row.Field<int>("PLAN_ID") == busConstant.LOCAL_52_PLAN_ID
                     && row.Field<string>("QDRO_LEGAL_REVIEW_REQUIRED") == busConstant.FLAG_YES).Select(row => row.Field<int>("PERSON_ACCOUNT_ID")).FirstOrDefault();

                var PersonAccountLocal161Plan = ldtblist.AsEnumerable().Where(row => row.Field<int>("PLAN_ID") == busConstant.LOCAL_161_PLAN_ID
                    && row.Field<string>("QDRO_LEGAL_REVIEW_REQUIRED") == busConstant.FLAG_YES).Select(row => row.Field<int>("PERSON_ACCOUNT_ID")).FirstOrDefault();

                var PersonAccountLocal600Plan = ldtblist.AsEnumerable().Where(row => row.Field<int>("PLAN_ID") == busConstant.LOCAL_600_PLAN_ID
                   && row.Field<string>("QDRO_LEGAL_REVIEW_REQUIRED") == busConstant.FLAG_YES).Select(row => row.Field<int>("PERSON_ACCOUNT_ID")).FirstOrDefault();

                var PersonAccountLocal700Plan = ldtblist.AsEnumerable().Where(row => row.Field<int>("PLAN_ID") == busConstant.LOCAL_700_PLAN_ID
                  && row.Field<string>("QDRO_LEGAL_REVIEW_REQUIRED") == busConstant.FLAG_YES).Select(row => row.Field<int>("PERSON_ACCOUNT_ID")).FirstOrDefault();


                if (PersonAccountMPIPlan > 0 && qdroDate != DateTime.MinValue)
                {
                    busPersonAccount lbusPersonAccount = new busPersonAccount();
                    if (PersonAccountMPIPlan > 0)
                    {
                        lbusPersonAccount.FindPersonAccount(PersonAccountMPIPlan);
                        lbusPersonAccount.icdoPersonAccount.Select();
                        lbusPersonAccount.icdoPersonAccount.qdro_review_completed_date = DateTime.MinValue;
                        lbusPersonAccount.icdoPersonAccount.Update();
                    }
                    var PersonAccountIAPId = ldtblist.AsEnumerable().Where(row => row.Field<int>("PLAN_ID") == busConstant.IAP_PLAN_ID).Select(row => row.Field<int>("PERSON_ACCOUNT_ID")).FirstOrDefault();
                    if (PersonAccountIAPId > 0)
                    {
                        lbusPersonAccount.FindPersonAccount(PersonAccountIAPId);
                        lbusPersonAccount.icdoPersonAccount.Select();
                        lbusPersonAccount.icdoPersonAccount.qdro_legal_review_required = busConstant.Flag_Yes;
                        lbusPersonAccount.icdoPersonAccount.Update();
                    }
                }
                if (PersonAccountIAPPlan > 0)
                {
                    busPersonAccount lbusPersonAccount = new busPersonAccount();
                    lbusPersonAccount.FindPersonAccount(PersonAccountIAPPlan);
                    lbusPersonAccount.icdoPersonAccount.Select();
                    lbusPersonAccount.icdoPersonAccount.qdro_review_completed_date = DateTime.MinValue;
                    lbusPersonAccount.icdoPersonAccount.qdro_legal_review_required = busConstant.Flag_Yes;
                    lbusPersonAccount.icdoPersonAccount.Update();
                }
                if (PersonAccountLocal52Plan > 0)
                {
                    busPersonAccount lbusPersonAccount = new busPersonAccount();
                    lbusPersonAccount.FindPersonAccount(PersonAccountLocal52Plan);
                    lbusPersonAccount.icdoPersonAccount.Select();
                    lbusPersonAccount.icdoPersonAccount.qdro_review_completed_date = DateTime.MinValue;
                    lbusPersonAccount.icdoPersonAccount.qdro_legal_review_required = busConstant.Flag_Yes;
                    lbusPersonAccount.icdoPersonAccount.Update();
                }
                if (PersonAccountLocal161Plan > 0)
                {
                    busPersonAccount lbusPersonAccount = new busPersonAccount();
                    lbusPersonAccount.FindPersonAccount(PersonAccountLocal161Plan);
                    lbusPersonAccount.icdoPersonAccount.Select();
                    lbusPersonAccount.icdoPersonAccount.qdro_review_completed_date = DateTime.MinValue;
                    lbusPersonAccount.icdoPersonAccount.qdro_legal_review_required = busConstant.Flag_Yes;
                    lbusPersonAccount.icdoPersonAccount.Update();
                }
                if (PersonAccountLocal600Plan > 0)
                {
                    busPersonAccount lbusPersonAccount = new busPersonAccount();
                    lbusPersonAccount.FindPersonAccount(PersonAccountLocal600Plan);
                    lbusPersonAccount.icdoPersonAccount.Select();
                    lbusPersonAccount.icdoPersonAccount.qdro_review_completed_date = DateTime.MinValue;
                    lbusPersonAccount.icdoPersonAccount.qdro_legal_review_required = busConstant.Flag_Yes;
                    lbusPersonAccount.icdoPersonAccount.Update();
                }
                if (PersonAccountLocal700Plan > 0)
                {
                    busPersonAccount lbusPersonAccount = new busPersonAccount();
                    lbusPersonAccount.FindPersonAccount(PersonAccountLocal700Plan);
                    lbusPersonAccount.icdoPersonAccount.Select();
                    lbusPersonAccount.icdoPersonAccount.qdro_review_completed_date = DateTime.MinValue;
                    lbusPersonAccount.icdoPersonAccount.qdro_legal_review_required = busConstant.Flag_Yes;
                    lbusPersonAccount.icdoPersonAccount.Update();
                }
            }
        }
    }
}



