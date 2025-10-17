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

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busPersonBase:
	/// Inherited from busPersonBaseGen, the class is used to customize the business object busPersonBaseGen.
	/// </summary>
	[Serializable]
	public class busPersonBase : busPersonBaseGen
	{
        #region Properties
        public string iblnParticipant { get; set; }
        public string iblnBeneficiary { get; set; }
        public string iblnAlternatePayee { get; set; }
        public string iblnDependent { get; set; }
        public string istrRetiree { get; set; }
        public int iintSelectedParticipantId { get; set; }
        public decimal idecAge { get; set; }
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

        public string istrCurrentDate { get; set; }
        public string istrRetirementDate { get; set; }
        public string istrPersonAddress { get; set; }
        public int iintCurrentYear { get; set; }

        public Collection<busPersonDependent> iclbPersonDependent { get; set; }
        public Collection<busPersonAccount> iclbPersonAccount { get; set; }
        public Collection<busPersonBridgeHours> iclbPersonBridgeHours { get; set; }
        public Collection<busPersonAccountRetirementContribution> iclbPersonAccountRetirementContribution { get; set; }
        public Collection<busPersonSuspendibleMonth> iclbPersonSuspendibleMonth { get; set; }

        public busBenefitApplication ibusBenefitApplication { get; set; }

        public busPersonAddress ibusPersonAddressForCorr { get; set; }

        public busPerson ibusBeneficiaryPerson { get; set; }//Will load Address directly in this one.

        public busPersonContact ibusPersonCourtContact { get; set; }
        public busPersonAddress ibusCourtAddressForCorr { get; set; }   // address required for participant for DRO-0008
        public busPersonAddress ibusAlternatePayeeAddressForCorr { get; set; }    // address required for alternatePayee for DRO-0008
        public busPersonContact ibusPersonContactForCor { get; set; }

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

        #endregion


        #region Public Methods

        //Loading Person Account Beneficiaries Collection For all Beneficiaries
        public void LoadBeneficiaries()
        {
            // DataTable ldtblist = busPerson.Select("cdoPerson.LoadAllPlans&Beneficiaries", new object[1] { this.icdoPerson.person_id });
            DataTable ldtblist = busPerson.Select("cdoPerson.LoadAllBeneficiaries", new object[1] { this.icdoPersonBase.person_id });
            iclbPersonAccountBeneficiary = GetCollection<busPersonAccountBeneficiary>(ldtblist, "icdoPersonAccountBeneficiary");
        }

        public void LoadRetirementContributionsbyPersonId(int aintPersonId)
        {
            DataTable ldtbList = busBase.Select("cdoPerson.GetUVHPAmounByPersonID", new object[1] { aintPersonId });
            iclbPersonAccountRetirementContribution = GetCollection<busPersonAccountRetirementContribution>(ldtbList, "icdoPersonAccountRetirementContribution");
        }

        public void LoadBeneficiariesForDeath()
        {
            // DataTable ldtblist = busPerson.Select("cdoPerson.LoadAllPlans&Beneficiaries", new object[1] { this.icdoPerson.person_id });
            DataTable ldtblist = busPerson.Select("cdoDeathNotification.LoadAllBeneficiaries", new object[1] { this.icdoPersonBase.person_id });
            iclbPersonAccountBeneficiary = GetCollection<busPersonAccountBeneficiary>(ldtblist, "icdoPersonAccountBeneficiary");
        }

        public void LoadPersonBridgedService()
        {
            DataTable ldtblist = busPerson.Select("cdoPerson.LoadPersonBridgedService", new object[1] { this.icdoPersonBase.person_id });
            iclbPersonBridgeHours = GetCollection<busPersonBridgeHours>(ldtblist, "icdoPersonBridgeHours");
        }

        public override busBase GetCorPerson()
        {
            return this;
        }

        public void LoadPersonAccounts()
        {
            DataTable ldtblist = busPerson.Select("cdoPerson.LoadPersonAccounts", new object[1] { this.icdoPersonBase.person_id });
            iclbPersonAccount = GetCollection<busPersonAccount>(ldtblist, "icdoPersonAccount");
        }

        /// <summary>
        /// Loading Person Account : IF Person is a beneficiary
        /// </summary>
        public void LoadPersonAccountsForSurvivor()
        {
            DataTable ldtblist = busPerson.Select("cdoPerson.LoadPersonAccountsForSurvivor", new object[1] { this.icdoPersonBase.person_id });
            iclbPersonAccount = GetCollection<busPersonAccount>(ldtblist, "icdoPersonAccount");
        }

        public void LoadPersonAccountsByPlanId(int aintPlanId)
        {
            DataTable ldtblist = busPerson.Select("cdoPersonAccount.LoadPersonAccountbyPlanId", new object[2] { this.icdoPersonBase.person_id, aintPlanId });
            iclbPersonAccount = GetCollection<busPersonAccount>(ldtblist, "icdoPersonAccount");
        }

        public void LoadPersonDependents()
        {
            DataTable ldtblist = busPerson.Select("cdoPerson.GetDependentName", new object[1] { this.icdoPersonBase.person_id });
            iclbPersonDependent = GetCollection<busPersonDependent>(ldtblist, "icdoRelationship");
        }

        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            if (this is busPersonOverview)
                return;
            utlError lobjError = null;
            base.ValidateHardErrors(aenmPageMode);
            if (this.ibusPersonAddress != null && PersonAddressEntered())
            {
                if (this.ibusPersonAddress.iarrErrors != null)
                {
                    this.ibusPersonAddress.iarrErrors.Clear();
                }
                this.ibusPersonAddress.ValidateHardErrors(utlPageMode.All);
                if (this.iarrErrors.Count == 0)
                {
                    this.iarrErrors = this.ibusPersonAddress.iarrErrors;
                }
            }

            if (string.IsNullOrEmpty(this.icdoPersonBase.communication_preference_value) && (!string.IsNullOrEmpty(this.icdoPersonBase.email_address_1) || blnCheckActiveAddress))
            {
                lobjError = AddError(1166, " ");
                this.iarrErrors.Add(lobjError);
            }

            if (this.icdoPersonBase.communication_preference_value == "MAIL")
            {
                if ((this.iclbPersonAddress != null && this.iclbPersonAddress.Count == 0) || this.iclbPersonAddress == null)
                {
                    lobjError = AddError(1163, " ");
                    this.iarrErrors.Add(lobjError);
                }
            }

            if (iobjPassInfo.ienmPageMode == utlPageMode.Update)
            {
                if (Convert.ToString(icdoPersonBase.ihstOldValues["date_of_birth"]) != string.Empty)
                {
                    DateTime ldtOldDateOfBirth = new DateTime();
                    ldtOldDateOfBirth = Convert.ToDateTime(Convert.ToString(icdoPersonBase.ihstOldValues["date_of_birth"]));

                    if (ldtOldDateOfBirth != icdoPersonBase.idtDateofBirth)
                    {
                        int lintCountPayees = (int)DBFunction.DBExecuteScalar("cdoPerson.GetPayeeAccountWithStatusNotCncld", new object[1] { this.icdoPersonBase.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                        if (lintCountPayees > 0)
                        {
                            lobjError = AddError(6067, " ");
                            this.iarrErrors.Add(lobjError);
                        }

                        busRetirementApplication lbusRetirementApplication = new busRetirementApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                        lbusRetirementApplication.icdoBenefitApplication.person_id = icdoPersonBase.person_id;
                        lbusRetirementApplication.GetRetirementDate();

                        if (lbusRetirementApplication.GetRetirementDate() != DateTime.MinValue)
                        {
                            lobjError = AddError(6090, " ");
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
                  //  CheckErrorOnAddButton(this, lhstParams, ref iarrErrors, true);
                  //  CheckOverlappingPeriod(this, lhstParams, ref iarrErrors, true);
                }
            }
        }

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


                    decimal ldtHoursReportedInparticularYear = Convert.ToDecimal(lintWorkingDays * 8);
                    lbusPersonBridgeHoursDetail.icdoPersonBridgeHoursDetail.hours = ldtHoursReportedInparticularYear;
                    aobjPersonBridgeHours.icdoPersonBridgeHours.hours_reported += ldtHoursReportedInparticularYear;

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
            ibusBenefitApplication.ibusPerson.FindPerson(this.icdoPersonBase.person_id);
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
            if (icdoPersonBase.ssn != null)
            {
                DataTable ldtblist = Select<cdoPerson>(
                    new string[1] { enmPerson.ssn.ToString() },
                    new object[1] { icdoPersonBase.ssn }, null, null);

                if (ldtblist.Rows.Count > 0)
                {

                    if (this.icdoPersonBase.ienuObjectState.Equals(ObjectState.Insert))
                    {
                        return true;
                    }
                    else if (icdoPersonBase.ihstOldValues.Count > 0 && icdoPersonBase.ihstOldValues[enmPerson.ssn.ToString()].ToString() != icdoPersonBase.ssn)
                    {
                        return true;
                    }
                }

            }
            return false;
        }

        public bool IsEmailValid()
        {
            return busGlobalFunctions.IsValidEmail(icdoPersonBase.email_address_1);
        }

        public void GetCurrentAge()
        {
            DateTime ldtDOB = icdoPersonBase.idtDateofBirth;
            idecAge = busGlobalFunctions.CalculatePersonAge(ldtDOB, DateTime.Today);
        }

        public bool CheckMemberIsBeneficiary()
        {
            DataTable ldtbBeneficiary = Select<cdoRelationship>(
               new string[1] { enmRelationship.beneficiary_person_id.ToString() },
               new object[1] { this.icdoPersonBase.person_id }, null, null);

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
            DataTable ldtbAlternatePayee = Select("cdoDroApplication.CheckCountApprovedApplication", new object[1] { this.icdoPersonBase.person_id });
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
            #region Commented Code
            // int lintcount = 0;
            //IDbConnection lconLegacy = DBFunction.GetDBConnection("Legacy");

            //if (lconLegacy != null)
            //{                              
            //    if (!String.IsNullOrEmpty(this.icdoPerson.ssn))
            //    {
            //        Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();

            //        IDbDataParameter lobjParameter = DBFunction.GetDBParameter();
            //        lobjParameter.ParameterName = "@SSN";
            //        lobjParameter.DbType = DbType.String;
            //        lobjParameter.Value = this.icdoPerson.ssn.ToString();
            //        lcolParameters.Add(lobjParameter);

            //        IDbDataParameter lobjParameter1 = DBFunction.GetDBParameter();
            //        lobjParameter1.ParameterName = "@COUNT";
            //        lobjParameter1.DbType = DbType.Int32;
            //        lobjParameter1.Direction = ParameterDirection.ReturnValue;
            //        lcolParameters.Add(lobjParameter1);

            //        //DBFunction.DBExecuteProcedure("usp_CheckPersonIsParticipant", lcolParameters, lconLegacy, null);
            //        lintcount = Convert.ToInt32(lcolParameters[1].Value);
            #endregion

            //Check IF HOURS EXIST FOR A PERSON            
            if (!string.IsNullOrEmpty(this.icdoPersonBase.istrSSNNonEncrypted))
            {
                //IDbConnection EADB = DBFunction.GetDBConnection("Legacy");
                //if (EADB != null)
                //{
                //    lintHoursExist = (int)DBFunction.DBExecuteScalar("cdoLegacy.CheckPersonIsParticipant", new object[1] { this.icdoPerson.istrSSNNonEncrypted }, EADB, null, iobjPassInfo.isrvMetaDataCache);
                //}
                //EADB.Close();
                utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
                string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

                DataTable ldtbCheckIfParticipant = new DataTable();

                SqlParameter[] parameters = new SqlParameter[1];
                SqlParameter param1 = new SqlParameter("@SSN", DbType.String);


                param1.Value = this.icdoPersonBase.istrSSNNonEncrypted;
                parameters[0] = param1;

                ldtbCheckIfParticipant = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_PensionInterface4OPUS", astrLegacyDBConnetion, null, parameters);
                if (ldtbCheckIfParticipant.Rows.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckMemberIsDependent()
        {
            DataTable ldtbDependent = Select<cdoRelationship>(
                new string[1] { enmRelationship.dependent_person_id.ToString() },
                new object[1] { this.icdoPersonBase.person_id }, null, null);

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
            DataTable ldtbBenefitApplcation = busBase.Select("cdoBenefitApplication.GetApprovedRetirement", new object[] { this.icdoPersonBase.person_id });
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



        //public Collection<cdoPersonBridgeHours> PopulateYears()
        //{
        //    Collection<cdoPersonBridgeHours> lclbYears = new Collection<cdoPersonBridgeHours>();
        //    lclbYears =busGlobalFunctions.PopulateYearsInDropDown();
        //    return lclbYears;
        //}

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

        // Wasim: Load Participant Plan Details

        public void LoadParticipantPlan()
        {
            DataTable ldtblist = busPerson.Select("cdoPersonAccount.GetPlanForPersonOverview", new object[1] { this.icdoPersonBase.person_id });
            iclbPersonAccount = GetCollection<busPersonAccount>(ldtblist, "icdoPersonAccount");
        }

        #region Load Spouse Details
        public void LoadSpouseDetails()
        {
            DataTable ldtbSpousalDetails = busBase.Select("cdoRelationship.GetSpousalDetails", new object[] { this.icdoPersonBase.person_id });
            if (ldtbSpousalDetails != null && ldtbSpousalDetails.Rows.Count > 0)
            {
                DataRow dtrow = ldtbSpousalDetails.Rows[0];
                istrSpousalFirstName = Convert.ToString(dtrow[enmPerson.first_name.ToString()]);
                istrSpousalLastName = Convert.ToString(dtrow[enmPerson.last_name.ToString()]);
                istrSpousalMiddleName = Convert.ToString(dtrow[enmPerson.middle_name.ToString()]);
                istrSpousalPrefix = Convert.ToString(dtrow[enmPerson.name_prefix_value.ToString()]);
                istrSpousalSSN = HelperFunction.FormatData(this.icdoPersonBase.istrSSNNonEncrypted, "{0:000-##-####}");
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
                   new object[1] { this.icdoPersonBase.person_id }, null, null);
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
                            foreach (cdoPersonAddressChklist lcdoPersonAddressChklist in objbusPersonAddress.iclcPersonAddressChklist)
                            {
                                if (lcdoPersonAddressChklist.address_type_value == "MAIL")
                                {
                                    blnCheckMail = true;
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

        public void LoadInitialData()
        {
            this.iblnBeneficiary = busConstant.NO;
            this.iblnAlternatePayee = busConstant.NO;
            this.iblnParticipant = busConstant.NO;
            this.iblnDependent = busConstant.NO;
            this.istrRetiree = busConstant.NO;
            this.lbusRetirementApplication = new busRetirementApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            this.lbusRetirementApplication.ibusPersonBase = this;
            if (this.CheckMemberIsBeneficiary())
            {
                this.iblnBeneficiary = busConstant.YES;
            }
            if (this.CheckMemberIsAlternatePayee())
            {
                this.iblnAlternatePayee = busConstant.YES;
            }
            if (!string.IsNullOrEmpty(this.icdoPersonBase.ssn))
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
            //DataTable ldtbSpousalDetails = busBase.Select("cdoRelationship.GetSpousalDetails", new object[] { this.icdoPerson.person_id });
            //if (ldtbSpousalDetails != null && ldtbSpousalDetails.Rows.Count > 0)
            //{
            //    DataRow dtrow = ldtbSpousalDetails.Rows[0];
            //    istrSpousalFirstName = Convert.ToString(dtrow[enmPerson.first_name.ToString()]);
            //    istrSpousalLastName = Convert.ToString(dtrow[enmPerson.last_name.ToString()]);
            //    istrSpousalMiddleName = Convert.ToString(dtrow[enmPerson.middle_name.ToString()]);
            //    istrSpousalPrefix = Convert.ToString(dtrow[enmPerson.name_prefix_value.ToString()]);
            //    istrSpousalSSN = HelperFunction.FormatData(this.icdoPerson.istrSSNNonEncrypted, "{0:000-##-####}");
            //}
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
            return lbusPersonAddress;
        }

        #endregion

        #region Overriden Methods

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (!string.IsNullOrEmpty(this.icdoPersonBase.istrSSNNonEncrypted) && this.icdoPersonBase.istrSSNNonEncrypted.Contains("-"))
            {
                this.icdoPersonBase.istrSSNNonEncrypted = this.icdoPersonBase.istrSSNNonEncrypted.Replace("-", string.Empty);
            }
            this.icdoPersonBase.ssn =this.icdoPersonBase.istrSSNNonEncrypted;
            //if (!string.IsNullOrEmpty(Convert.ToString(this.icdoPerson.idtDateofBirth)))
            //{
            //    this.icdoPerson.date_of_birth = HelperFunction.SagitecEncryptAES(Convert.ToString(this.icdoPerson.idtDateofBirth));
            //}
            if (this.icdoPersonBase.communication_preference_value == busConstant.MAIL)
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
            if (!blnCheckActiveAddress)
            {
                if (!string.IsNullOrEmpty(this.icdoPersonBase.email_address_1))
                {
                    this.icdoPersonBase.communication_preference_value = busConstant.EMAL;
                }
                else
                {
                    this.icdoPersonBase.communication_preference_value = string.Empty;
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

                //lbusPersonAccountBeneficiary.ibusParticipant = new busPerson();
                //confirm : why are we doing this ?
                lbusPersonAccountBeneficiary.ibusDuplicateParticipant = this;

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

                DataTable ldtblist = busPerson.Select("cdoPerson.LoadBridgingDetails", new object[1] { this.icdoPersonBase.person_id });
                lbusPersonBridgeHours.iclbPersonBridgeHoursDetails = GetCollection<busPersonBridgeHoursDetail>(ldtblist, "icdoPersonBridgeHoursDetail");
            }

            base.LoadOtherObjects(adtrRow, aobjBus);
        }

        public override void BeforePersistChanges()
        {
            if (this is busPersonOverview)
                return;
            //EncryptDOB();

            base.BeforePersistChanges();
            //WI 23234 Secure Document submission
            if (this.icdoPersonBase.document_upload_flag.IsNullOrEmpty())
                this.icdoPersonBase.document_upload_flag = busConstant.FLAG_NO;  

            if (this.icdoPersonBase.vip_flag.IsNullOrEmpty())
                this.icdoPersonBase.vip_flag = busConstant.FLAG_NO;  //Default the VIP flag to No if the use doesnt check it as YES while creating the Person

            //U24074
            if (this.icdoPersonBase.adverse_interest_flag.IsNullOrEmpty())
                this.icdoPersonBase.adverse_interest_flag = busConstant.FLAG_NO;

            if (istrAddressSameAsParticipant == busConstant.FLAG_YES)
            {
                iarrChangeLog.Remove(this.ibusPersonAddress.icdoPersonAddress);
            }


            //When we create new address from Beneficiary maintennace screen
            //if same as participant checkbox is checked then need to load participant address and insert a new record. 
            //Need to change the object state to None since in this method it is making blank entry in database

            //if (ibusPersonAddress!= null && ibusPersonAddress.icdoPersonAddress.addr_same_as_person == busConstant.FLAG_YES && iintSelectedParticipantId != 0)
            //{
            //    ibusPersonAddress.icdoPersonAddress.ienuObjectState = ObjectState.None;
            //}
            if (icdoPersonBase.idtDateofBirth != null)
            {
                idecAge = busGlobalFunctions.CalculatePersonAge(icdoPersonBase.idtDateofBirth, DateTime.Today);
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
                    item.icdoNotes.person_id = this.icdoPersonBase.person_id;
                item.icdoNotes.form_id = busConstant.Form_ID;
                item.icdoNotes.form_value = busConstant.PERSON_MAINTAINENCE_FORM;
            });
        }


        public override int PersistChanges()   //ABHSIHEK-- ASK QUESTION TO DURGESH 
        {
            string old_vip_flag_value = String.Empty;
            string old_adverse_interest_flag = String.Empty;
            if (!(this is busPersonOverview))
            {
                if (iarrChangeLog.Count > 0 && iarrChangeLog[0].ihstOldValues.Count > 0 && iarrChangeLog[0].ihstOldValues.ContainsKey(enmPerson.vip_flag.ToString()))
                {
                    old_vip_flag_value = Convert.ToString(iarrChangeLog[0].ihstOldValues[enmPerson.vip_flag.ToString()]);
                }
                if (iarrChangeLog.Count > 0 && iarrChangeLog[0].ihstOldValues.Count > 0 && iarrChangeLog[0].ihstOldValues.ContainsKey(enmPerson.adverse_interest_flag.ToString()))
                {
                    old_adverse_interest_flag = Convert.ToString(iarrChangeLog[0].ihstOldValues[enmPerson.adverse_interest_flag.ToString()]);
                    if (old_adverse_interest_flag.IsNullOrEmpty())
                        old_adverse_interest_flag = busConstant.FLAG_NO;
                }

                #region DOB change
                DateTime ldtOldDOB = new DateTime();
                if (iarrChangeLog.Count > 0 && iarrChangeLog[0].ihstOldValues.Count > 0 && iarrChangeLog[0].ihstOldValues.ContainsKey(enmPerson.date_of_birth.ToString()))
                {
                    string lstrOldDOB = Convert.ToString(iarrChangeLog[0].ihstOldValues[enmPerson.date_of_birth.ToString()]);
                    if (!string.IsNullOrEmpty(lstrOldDOB))
                    {
                        ldtOldDOB = Convert.ToDateTime(lstrOldDOB);
                        if (ldtOldDOB != this.icdoPersonBase.idtDateofBirth)
                        {
                            this.icdoPersonBase.recalculate_vesting_flag = busConstant.FLAG_YES;
                        }
                    }
                    else
                    {
                        this.icdoPersonBase.recalculate_vesting_flag = busConstant.FLAG_YES;
                    }
                }
                else
                {
                    this.icdoPersonBase.recalculate_vesting_flag = busConstant.FLAG_NO;
                }
                #endregion

                #region Bridging Hours Added or Changed
                if (!this.iclbPersonBridgeHours.IsNullOrEmpty())
                    this.icdoPersonBase.recalculate_vesting_flag = busConstant.FLAG_YES;

                #endregion

            }

            int return_value = base.PersistChanges();

            if (this.icdoPersonBase.vip_flag != old_vip_flag_value && !(this is busPersonOverview))
            {
                ChangeVIPFlag();
            }

            //WI 23234 Secure Document submission
            if (iarrChangeLog.Count > 0 && iarrChangeLog[0].ihstOldValues.Count > 0 && iarrChangeLog[0].ihstOldValues.ContainsKey(enmPerson.document_upload_flag.ToString()))
            {
                string old_document_upload_flag;
                old_document_upload_flag = Convert.ToString(iarrChangeLog[0].ihstOldValues[enmPerson.document_upload_flag.ToString()]);
                if (this.icdoPersonBase.document_upload_flag != old_document_upload_flag && icdoPersonBase.document_upload_flag == busConstant.FLAG_YES)
                {
                    //Call webapi to trigger email.
                }

            }
            //U24074
            if (iarrChangeLog.Count > 0 && iarrChangeLog[0].ihstOldValues.Count > 0 && iarrChangeLog[0].ihstOldValues.ContainsKey(enmPerson.adverse_interest_flag.ToString()))
            {
                if (this.icdoPersonBase.adverse_interest_flag != old_adverse_interest_flag)
                {
                    string lstrNotes = string.Empty;
                    if (this.icdoPersonBase.adverse_interest_flag == "Y")
                        lstrNotes = "Potential Adverse Interest";
                    else
                        lstrNotes = "Adverse Interest Removed";
                    cdoNotes lcdoNotes = new cdoNotes();
                    lcdoNotes.person_id = this.icdoPersonBase.person_id;
                    lcdoNotes.form_value = busConstant.PERSON_MAINTAINENCE_FORM;
                    lcdoNotes.notes = lstrNotes;
                    lcdoNotes.created_by = iobjPassInfo.istrUserID;
                    lcdoNotes.created_date = DateTime.Now;
                    lcdoNotes.modified_by = iobjPassInfo.istrUserID;
                    lcdoNotes.modified_date = DateTime.Now;
                    lcdoNotes.Insert();
                    this.iclbNotes = busGlobalFunctions.LoadNotes(this.icdoPersonBase.person_id, 0, busConstant.PERSON_MAINTAINENCE_FORM);
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
            if (string.IsNullOrEmpty(this.icdoPersonBase.mpi_person_id))
            {
                this.icdoPersonBase.mpi_person_id = "M" + this.icdoPersonBase.person_id.ToString("D8");
                this.icdoPersonBase.Update();
            }
            LoadInitialData();

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

            //DataTable ldtbSysMgmt = iobjPassInfo.isrvDBCache.GetSystemManagement();
            //if (ldtbSysMgmt.Rows.Count > 0 && ldtbSysMgmt.Rows[0]["REGION_VALUE"].ToString() == "DEVL")
            //{
            //    return;
            //}

            if ((iobjPassInfo.iconFramework).Database == "MPI" || (iobjPassInfo.iconFramework).Database == "MPIPHP")
            {
                return;
            }
            //OPUS data push to Health Eligibility for any person Update  //Commented - Rohan Code For data Push to HEDB (Do not delete this)

            // decommissioning demographics informations, since HEDB is retiring.
            //if (iobjPassInfo.ienmPageMode == utlPageMode.Update)
            //{

            if (iobjPassInfo.ienmPageMode == utlPageMode.Update)
            {


            //    utlPassInfo lobjPassInfo1 = new utlPassInfo();
            //    lobjPassInfo1.idictParams["ID"] = "OPUS_INTEGRATION";
            //    lobjPassInfo1.iconFramework = DBFunction.GetDBConnection("HELegacy");

            //    if (lobjPassInfo1.iconFramework != null)
            //    {

            //        string lstrMPIPersonId = this.icdoPersonBase.mpi_person_id;
            //        string lstrPersonSSN = this.icdoPersonBase.istrSSNNonEncrypted;
            //        string lstrParticipantMPIID = string.Empty;
            //        string lstrRelationshipType = string.Empty;


            //        try
            //        {
            //            string strQuery = "select * from person where ssn = (select ssn from Eligibility_PID_Reference where PID = '" + this.icdoPersonBase.mpi_person_id + "')";
            //            DataTable ldtbResult = DBFunction.DBSelect(strQuery, lobjPassInfo1.iconFramework);
            //            if (ldtbResult.Rows.Count == 0)
            //            {
            //                return;
            //            }

            //        }
            //        catch
            //        {

            //        }

            //        int CountDependent = (int)DBFunction.DBExecuteScalar("cdoPerson.ChechPersonIsDependent", new object[1] { this.icdoPersonBase.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            //        int CountBeneficiary = (int)DBFunction.DBExecuteScalar("cdoPerson.CheckPersonIsBeneficiary", new object[1] { this.icdoPersonBase.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            //        if (CountDependent > 0)
            //        {
            //            lstrRelationshipType = "D";
            //        }
            //        else if (CountBeneficiary > 0)
            //        {
            //            lstrRelationshipType = "B";
            //        }
            //        else
            //        {
            //            lstrRelationshipType = null;
            //        }

            //        string lstrFirstName = this.icdoPersonBase.first_name;
            //        string lstrMiddleName = this.icdoPersonBase.middle_name;
            //        string lstrlastName = this.icdoPersonBase.last_name;
            //        string lstrGender = this.icdoPersonBase.gender_value;
            //        DateTime lstrDOB = DateTime.MinValue;

                    //string lstrFirstName = this.icdoPersonBase.first_name;
                    //string lstrMiddleName = this.icdoPersonBase.middle_name;
                    //string lstrlastName = this.icdoPersonBase.last_name;
                    //string lstrGender = this.icdoPersonBase.gender_value;
                    //DateTime lstrDOB = DateTime.MinValue;


            //        if (lstrFirstName.IsNotNullOrEmpty())
            //        {
            //            lstrFirstName = lstrFirstName.ToUpper();
            //        }

            //        if (lstrMiddleName.IsNotNullOrEmpty())
            //        {
            //            lstrMiddleName = lstrMiddleName.ToUpper();
            //        }

            //        if (lstrlastName.IsNotNullOrEmpty())
            //        {
            //            lstrlastName = lstrlastName.ToUpper();
            //        }

            //        lstrDOB = this.icdoPersonBase.idtDateofBirth;

            //        DateTime lstrDOD = DateTime.MinValue;

            //        lstrDOD = this.icdoPersonBase.date_of_death;


            //        string lstrHomePhone = this.icdoPersonBase.home_phone_no;
            //        string lstrCellPhone = this.icdoPersonBase.cell_phone_no;
            //        string lstrFax = this.icdoPersonBase.fax_no;
            //        string lstrEmail = this.icdoPersonBase.email_address_1;
            //        string lstrCreatedBy = this.icdoPersonBase.modified_by;
            //        //Ticket : 55015
            //        string lstrVipFlag = this.icdoPersonBase.vip_flag;
            //        string lUpperlstrVipFlag = lstrVipFlag.ToUpper();
            //        bool lboolVipFlag = false;
            //        if (lUpperlstrVipFlag == "Y")
            //        {
            //            lboolVipFlag = true;
            //        }
            //        if (iobjPassInfo.ienmPageMode == utlPageMode.Update)
            //        {

            //            //if (!String.IsNullOrEmpty(lstrPersonSSN))
            //            //{
            //                Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();

                        //if (!String.IsNullOrEmpty(lstrPersonSSN))
                        //{
                            Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();

            //                IDbDataParameter lobjParameter1 = DBFunction.GetDBParameter();
            //                lobjParameter1.ParameterName = "@PID";
            //                lobjParameter1.DbType = DbType.String;
            //                lobjParameter1.Value = lstrMPIPersonId.ToLower();
            //                lcolParameters.Add(lobjParameter1);

            //                IDbDataParameter lobjParameter2 = DBFunction.GetDBParameter();
            //                lobjParameter2.ParameterName = "@SSN";
            //                lobjParameter2.DbType = DbType.String;

            //                if (lstrPersonSSN.IsNullOrEmpty())
            //                    lobjParameter2.Value = DBNull.Value;
            //                else
            //                    lobjParameter2.Value = lstrPersonSSN.ToLower();
            //                lcolParameters.Add(lobjParameter2);

            //                IDbDataParameter lobjParameter3 = DBFunction.GetDBParameter();
            //                lobjParameter3.ParameterName = "@ParticipantPID";
            //                lobjParameter3.DbType = DbType.String;
            //                lobjParameter3.Value = DBNull.Value;    //Need to change            
            //                lcolParameters.Add(lobjParameter3);

            //                IDbDataParameter lobjParameter4 = DBFunction.GetDBParameter();
            //                lobjParameter4.ParameterName = "@EntityTypeCode";
            //                lobjParameter4.DbType = DbType.String;
            //                lobjParameter4.Value = "P";                 //for now we will always use Person
            //                lcolParameters.Add(lobjParameter4);

            //                IDbDataParameter lobjParameter5 = DBFunction.GetDBParameter();
            //                lobjParameter5.ParameterName = "@RelationType";
            //                lobjParameter5.DbType = DbType.String;
            //                lobjParameter5.Value = lstrRelationshipType;                //Need to change
            //                lcolParameters.Add(lobjParameter5);

            //                IDbDataParameter lobjParameter6 = DBFunction.GetDBParameter();
            //                lobjParameter6.ParameterName = "@FirstName";
            //                lobjParameter6.DbType = DbType.String;
            //                lobjParameter6.Value = lstrFirstName;
            //                lcolParameters.Add(lobjParameter6);

            //                IDbDataParameter lobjParameter7 = DBFunction.GetDBParameter();
            //                lobjParameter7.ParameterName = "@MiddleName";
            //                lobjParameter7.DbType = DbType.String;
            //                lobjParameter7.Value = lstrMiddleName;
            //                lcolParameters.Add(lobjParameter7);

            //                IDbDataParameter lobjParameter8 = DBFunction.GetDBParameter();
            //                lobjParameter8.ParameterName = "@LastName";
            //                lobjParameter8.DbType = DbType.String;
            //                lobjParameter8.Value = lstrlastName;
            //                lcolParameters.Add(lobjParameter8);

            //                IDbDataParameter lobjParameter9 = DBFunction.GetDBParameter();
            //                lobjParameter9.ParameterName = "@Gender";
            //                lobjParameter9.DbType = DbType.String;
            //                lobjParameter9.Value = lstrGender;
            //                lcolParameters.Add(lobjParameter9);


            //                IDbDataParameter lobjParameter10 = DBFunction.GetDBParameter();
            //                lobjParameter10.ParameterName = "@DateOfBirth";
            //                lobjParameter10.DbType = DbType.DateTime;
            //                if (lstrDOB != DateTime.MinValue)
            //                {
            //                    lobjParameter10.Value = lstrDOB;
            //                }
            //                else
            //                {
            //                    lobjParameter10.Value = DBNull.Value;
            //                }
            //                lcolParameters.Add(lobjParameter10);


            //                IDbDataParameter lobjParameter11 = DBFunction.GetDBParameter();
            //                lobjParameter11.ParameterName = "@DateOfDeath";
            //                lobjParameter11.DbType = DbType.DateTime;

            //                if (lstrDOD != DateTime.MinValue)
            //                {
            //                    lobjParameter11.Value = lstrDOD;
            //                }
            //                else
            //                {
            //                    lobjParameter11.Value = DBNull.Value;
            //                }
            //                lcolParameters.Add(lobjParameter11);

            //                IDbDataParameter lobjParameter12 = DBFunction.GetDBParameter();
            //                lobjParameter12.ParameterName = "@HomePhone";
            //                lobjParameter12.DbType = DbType.String;
            //                lobjParameter12.Value = lstrHomePhone;
            //                lcolParameters.Add(lobjParameter12);

            //                IDbDataParameter lobjParameter13 = DBFunction.GetDBParameter();
            //                lobjParameter13.ParameterName = "@CellPhone";
            //                lobjParameter13.DbType = DbType.String;
            //                lobjParameter13.Value = lstrCellPhone;
            //                lcolParameters.Add(lobjParameter13);

            //                IDbDataParameter lobjParameter14 = DBFunction.GetDBParameter();
            //                lobjParameter14.ParameterName = "@Fax";
            //                lobjParameter14.DbType = DbType.String;
            //                lobjParameter14.Value = lstrFax;
            //                lcolParameters.Add(lobjParameter14);

            //                IDbDataParameter lobjParameter15 = DBFunction.GetDBParameter();
            //                lobjParameter15.ParameterName = "@Email";
            //                lobjParameter15.DbType = DbType.String;
            //                lobjParameter15.Value = lstrEmail;
            //                lcolParameters.Add(lobjParameter15);

            //                IDbDataParameter lobjParameter16 = DBFunction.GetDBParameter();
            //                lobjParameter16.ParameterName = "@AuditUser";
            //                lobjParameter16.DbType = DbType.String;
            //                lobjParameter16.Value = lstrCreatedBy;
            //                lcolParameters.Add(lobjParameter16);
            //                //Ticket : 55015
            //                IDbDataParameter lobjParameter17 = DBFunction.GetDBParameter();
            //                lobjParameter17.ParameterName = "@VipFlag";
            //                lobjParameter17.DbType = DbType.Boolean;
            //                lobjParameter17.Value = lboolVipFlag;
            //                lcolParameters.Add(lobjParameter17);


            //            try
            //                {
            //                    lobjPassInfo1.BeginTransaction();
            //                    DBFunction.DBExecuteProcedure("USP_PID_Person_UPD", lcolParameters, lobjPassInfo1.iconFramework, lobjPassInfo1.itrnFramework);
            //                    lobjPassInfo1.Commit();
            //                }
            //                catch (Exception e)
            //                {
            //                    lobjPassInfo1.Rollback();
            //                    throw e;
            //                }
            //                finally
            //                {
            //                    lobjPassInfo1.iconFramework.Close();
            //                }
            //            //}
            //        }
            //    }
           }
        }

        public override ArrayList ValidateNew(Hashtable ahstParam)
        {
            ArrayList larrErrors = new ArrayList();
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

            if (!string.IsNullOrEmpty(this.iblnBeneficiary))
            {
                //Will be called from batch
                if ((this.iblnBeneficiary.ToUpper() == busConstant.YES.ToUpper() || this.iblnDependent.ToUpper() == busConstant.YES.ToUpper()) && this.iblnParticipant.ToUpper() != busConstant.YES.ToUpper())
                {
                    DataTable ldtbBeneficaryID = Select("cdoPersonAddress.GetMainParticipantAddress", new object[1] { this.icdoPersonBase.person_id });
                    if (ldtbBeneficaryID.Rows.Count > 0)
                    {
                        DataRow dtrBeneficaryID = ldtbBeneficaryID.Rows[0];
                        DataTable ldtbList = Select<cdoPersonAddress>(new string[1] { enmPersonAddress.person_id.ToString() }, new object[1] { Convert.ToInt32(dtrBeneficaryID[enmRelationship.person_id.ToString()]) }, null, null);
                        foreach (DataRow ldtRow in ldtbList.Rows)
                        {
                            busPersonAddress lbusPersonAddress = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() };
                            lbusPersonAddress.icdoPersonAddress.LoadData(ldtRow);
                            lbusPersonAddress.LoadPersonAddressChklists();
                            lbusPersonAddress.SetPhysivcalMailingFlagValue(lbusPersonAddress.iclcPersonAddressChklist, lbusPersonAddress);
                            lbusPersonAddress.icdoPersonAddress.iaintMainParticipantAddressID = lbusPersonAddress.icdoPersonAddress.address_id;
                            lbusPersonAddress.icdoPersonAddress.person_id = this.icdoPersonBase.person_id;
                            lbusPersonAddress.icdoPersonAddress.istrAddSameAsParticipantFlag = busConstant.FLAG_YES;
                            lbusPersonAddress.LoadStateDescription();
                            iclbPersonAddress.Add(lbusPersonAddress);
                        }
                    }
                }
            }

            foreach (busPersonAddress lbusPersonAddress in iclbPersonAddress)
            {
                if (lbusPersonAddress.icdoPersonAddress.iaintMainParticipantAddressID == 0)
                {
                    lbusPersonAddress.LoadPersonAddressChklists();
                    lbusPersonAddress.LoadStateDescription();
                    lbusPersonAddress.SetPhysivcalMailingFlagValue(lbusPersonAddress.iclcPersonAddressChklist, lbusPersonAddress);
                }
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

            if (astrTemplateName == busConstant.WORK_HISTORY_REQUEST)
            {
                this.LoadWorkHistory();
                if (this.ibusBenefitApplication.icdoBenefitApplication.retirement_date != DateTime.MinValue)
                {
                    DateTime lRetirementDt = this.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
                    istrRetirementDate = busGlobalFunctions.ConvertDateIntoDifFormat(lRetirementDt);
                }
            }
        }

        public void LoadCorrespondenceProperties()
        {
            DateTime ldtDob = icdoPersonBase.idtDateofBirth;
            ldtDob = ldtDob.AddYears(70);
            ldtDob = ldtDob.AddMonths(6);
            iintYear = Convert.ToInt32(ldtDob.Year);

            istrMinDistriDate = busGlobalFunctions.CalculateMinDistributionDate(icdoPersonBase.idtDateofBirth);
            istrFebDate = busGlobalFunctions.CalculateFebDate(icdoPersonBase.idtDateofBirth);

            if (this.icdoPersonBase.marital_status_value != null)
            {
                if (this.icdoPersonBase.marital_status_value != "M")
                    istrBenefitOption = "Life Annuity";
                else
                    istrBenefitOption = "Qualified Joint & 50% Survivor Annuity";
            }
        }

        public bool FindPerson(string astrPersonMpiId)
        {
            bool lblnResult = false;
            if (icdoPersonBase == null)
            {
                icdoPersonBase = new cdoPersonBase();
            }

            DataTable ldtPerson = busBase.Select<cdoPerson>(new string[1] { enmPerson.mpi_person_id.ToString() }, new object[1] { astrPersonMpiId }, null, null);
            if (ldtPerson != null && ldtPerson.Rows.Count > 0)
            {
                icdoPersonBase.LoadData(ldtPerson.Rows[0]);
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
            DataTable ldtbBeneficariesDependants = Select("cdoPerson.GetVIPforBeneficaryandDependent", new object[1] { icdoPersonBase.person_id });

            if (ldtbBeneficariesDependants.Rows.Count > 0)
            {
                Collection<busPerson> lclbBenDepList = new Collection<busPerson>();
                lclbBenDepList = GetCollection<busPerson>(ldtbBeneficariesDependants, "icdoPerson");

                lclbBenDepList.ForEach(item =>
                {
                    item.icdoPerson.vip_flag = this.icdoPersonBase.vip_flag;
                    item.icdoPerson.Update();
                });
            }
        }

        private bool PersonAddressEntered()
        {
            bool lblnAddressEntered = false;
            if (this.iarrChangeLog.Contains(this.ibusPersonAddress.icdoPersonAddress))
            {
                lblnAddressEntered = true;
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
            if (this.icdoPersonBase.gender_value == busConstant.MALE)
            {
                if (this.icdoPersonBase.name_prefix_value == busConstant.MISS || this.icdoPersonBase.name_prefix_value == busConstant.MRS)
                {
                    return true;
                }
            }
            else if (this.icdoPersonBase.gender_value == busConstant.FEMALE)
            {
                if (this.icdoPersonBase.name_prefix_value == busConstant.MR)
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
        #endregion


        /// <summary>
        /// Method to negate the forfeiture amounts for IAP when bridging is done
        /// </summary>
        public void NegateIAPForfeitureAmounts()
        {
            busBenefitApplication lobjApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            cdoDummyWorkData lcdoWorkData = new cdoDummyWorkData();

            lobjApplication.ibusPerson = new busPerson();
            lobjApplication.ibusPerson.FindPerson(this.icdoPersonBase.person_id);
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
            lobjApplication.ibusPerson.FindPerson(this.icdoPersonBase.person_id);
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

        public void LoadRetirementContributionsForEEAndUVHP(int aintPersonId, DateTime adtTransactionDate)
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

        public void LoadRetirementContributionsForIAP(int aintPersonAccountID)
        {
            this.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();
            DataTable ldtbIAPContribution = busBase.Select("cdoPersonAccountRetirementContribution.GetRetirementContributionbyAccountId", new object[1] { aintPersonAccountID });
            if (ldtbIAPContribution.Rows.Count > 0)
            {
                iclbPersonAccountRetirementContribution = GetCollection<busPersonAccountRetirementContribution>(ldtbIAPContribution, "icdoPersonAccountRetirementContribution");
            }
            this.idecIAPContributionAmount = iclbPersonAccountRetirementContribution.Sum(item => item.icdoPersonAccountRetirementContribution.iap_balance_amount);
            this.idecLocal52SpecialAcctBalanceAmt = iclbPersonAccountRetirementContribution.Sum(item => item.icdoPersonAccountRetirementContribution.local52_special_acct_bal_amount);
            this.idecLocal161SpecialAcctBalanceAmt = iclbPersonAccountRetirementContribution.Sum(item => item.icdoPersonAccountRetirementContribution.local161_special_acct_bal_amount);
        }

        public void LoadPersonSuspendibleMonth()
        {
            DataTable ldtblist = busPerson.Select("cdoPerson.LoadPersonSuspendibleMonth", new object[1] { this.icdoPersonBase.person_id });
            iclbPersonSuspendibleMonth = GetCollection<busPersonSuspendibleMonth>(ldtblist, "icdoPersonSuspendibleMonth");
        }

    }
}



