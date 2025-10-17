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
using System.Windows.Forms;
using System.Transactions;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busPerson:
    /// Inherited from busPersonGen, the class is used to customize the business object busPersonGen.
    /// </summary>
    [Serializable]
    public class busSSNMerge : busPerson
    {
        public busPerson ibusDuplicateRecord { get; set; }

        public Collection<busPerson> iclbOldPersonToBeMerged { get; set; }
        public Collection<busPerson> iclbNewPerson { get; set; }
        public Collection<busPerson> iclbMergedPersonReord { get; set; }
        public Collection<busPayeeAccount> iclbPayeeAccounts { get; set; }
        public Collection<busPayeeAccount> iclbPayeeAccountsForPossibleDuplicates { get; set; }
        public busIapAllocationSummary ibusPrevYearAllocationSummary { get; set; }
        public Collection<busPersonAccountEligibility> iclbPersonAccountEligibility { get; set; }

        //utlPassInfo lobjLegacyPassInfoEADB {get; set;}
        //utlPassInfo lobjLegacyPassInfoHEDB {get; set;}
        //utlPassInfo lobjLegacy { get; set; }

        [NonSerialized()]
        private utlPassInfo _lobjLegacyPassInfoEADB;
        [NonSerialized()]
        private utlPassInfo _lobjLegacyPassInfoHEDB;
        [NonSerialized()]
        private utlPassInfo _lobjLegacy;

        public utlPassInfo lobjLegacyPassInfoEADB { get { return _lobjLegacyPassInfoEADB; } set { _lobjLegacyPassInfoEADB = value; } }
        public utlPassInfo lobjLegacyPassInfoHEDB { get { return _lobjLegacyPassInfoHEDB; } set { _lobjLegacyPassInfoHEDB = value; } }
        public utlPassInfo lobjLegacy { get { return _lobjLegacy; } set { _lobjLegacy = value; } }

        public Collection<busPerson> iclbPreview { get; set; }
        public decimal idecEEContribnAmt { get; set; }
        public decimal idecEEInterestAmt { get; set; }
        public decimal idecUVHPAmount { get; set; }
        public decimal idecUVHPInterestAmt { get; set; }
        public decimal idecIAPBalanceAmount { get; set; }

        public string istrMPID { get; set; }

        IDbConnection lconLegacy = null;
        DateTime ldtMinVal = new DateTime(1753, 01, 01);

        public bool IsPossibleSSNMergeRecord(busPerson abusNewPerson)
        {
            bool flag = false;

            if (abusNewPerson.icdoPerson.istrSSNNonEncrypted != null)
            {
                DataTable ldtbResult;

                if (abusNewPerson.icdoPerson.ienuObjectState == ObjectState.Insert)
                {
                    ldtbResult = Select("cdoPerson.LoadPersonInfoForSSNByPersonId", new object[3] {
                            abusNewPerson.icdoPerson.first_name , abusNewPerson.icdoPerson.last_name,
                            abusNewPerson.icdoPerson.idtDateofBirth});
                }
                else
                {
                    ldtbResult = Select("cdoPerson.LoadPersonInfoForSSNMerge", new object[4] {
                            abusNewPerson.icdoPerson.first_name , abusNewPerson.icdoPerson.last_name,
                            abusNewPerson.icdoPerson.idtDateofBirth.ToString(),
                            abusNewPerson.icdoPerson.person_id});
                }


                foreach (DataRow ldr in ldtbResult.Rows)
                {
                    busPerson lbusExistingPersonRecord = new busPerson() { icdoPerson = new cdoPerson() };
                    lbusExistingPersonRecord.icdoPerson.LoadData(ldr);

                    if (abusNewPerson.icdoPerson.ienuObjectState == ObjectState.Insert)
                    {
                        //lstrOldSSN = Sagitec.Common.HelperFunction.SagitecDecryptAES(lbusExistingPersonRecord.icdoPerson.ssn);
                        if (abusNewPerson.icdoPerson.first_name.Equals(lbusExistingPersonRecord.icdoPerson.first_name)
                            && (abusNewPerson.icdoPerson.last_name.Equals(lbusExistingPersonRecord.icdoPerson.last_name)) &&
                            lbusExistingPersonRecord.icdoPerson.idtDateofBirth == abusNewPerson.icdoPerson.idtDateofBirth)
                        {
                            for (int lintSSNIndex = 0; lintSSNIndex <= 4; lintSSNIndex++)
                            {
                                string temp = lbusExistingPersonRecord.icdoPerson.istrSSNNonEncrypted.Substring(lintSSNIndex, 5);
                                if (abusNewPerson.icdoPerson.istrSSNNonEncrypted.Contains(temp))
                                {
                                    flag = true;
                                    break;
                                }

                            }
                        }

                    }
                    else if (abusNewPerson.icdoPerson.ienuObjectState == ObjectState.Update && lbusExistingPersonRecord.icdoPerson.istrSSNNonEncrypted.IsNotNullOrEmpty() &&
                             abusNewPerson.icdoPerson.ihstOldValues.Count > 0 &&
                             Convert.ToString(abusNewPerson.icdoPerson.ihstOldValues["ssn"]) != abusNewPerson.icdoPerson.istrSSNNonEncrypted)
                    {
                        DateTime ldtOldDOB = Convert.ToDateTime(Convert.ToString(abusNewPerson.icdoPerson.ihstOldValues["date_of_birth"]));

                        if (Convert.ToString(abusNewPerson.icdoPerson.ihstOldValues["first_name"]).IsNotNullOrEmpty() &&
                                Convert.ToString(abusNewPerson.icdoPerson.ihstOldValues["first_name"]).Equals(abusNewPerson.icdoPerson.first_name)
                                && (Convert.ToString(abusNewPerson.icdoPerson.ihstOldValues["last_name"]).IsNotNullOrEmpty() &&
                                Convert.ToString(abusNewPerson.icdoPerson.ihstOldValues["last_name"]).Equals(abusNewPerson.icdoPerson.last_name))
                                && (ldtOldDOB != DateTime.MinValue && ldtOldDOB == abusNewPerson.icdoPerson.idtDateofBirth))
                        {
                            string lstrSSNToBeCompared = string.Empty;

                            for (int lintSSNIndex = 0; lintSSNIndex <= 4; lintSSNIndex++)
                            {
                                lstrSSNToBeCompared = abusNewPerson.icdoPerson.istrSSNNonEncrypted.Substring(lintSSNIndex, 5);
                                if (lbusExistingPersonRecord.icdoPerson.istrSSNNonEncrypted.Contains(lstrSSNToBeCompared))
                                {
                                    flag = true;
                                    break;
                                }

                            }
                        }

                    }

                    if (flag)
                    {
                        break;
                    }
                }
            }

            return flag;
        }

        public Collection<busPayeeAccount> GetPayeeAccounts(int aintNewPersonID)
        {
            iclbPayeeAccounts = new Collection<busPayeeAccount>();
            DataTable ldtblist = Select("cdoPayeeAccount.GetLatestPayeeAccounts", new object[1] { aintNewPersonID });

            //if (icdoPerson.date_of_birth == ldtMinVal)
            //    icdoPerson.date_of_birth = DateTime.MinValue; 
            //if(ldtblist.Rows.Count>0 && (icdoPerson.date_of_birth!=DateTime.MinValue) && (icdoPerson.first_name.IsNotNull()) && (icdoPerson.last_name.IsNotNull())&& (icdoPerson.ssn.IsNotNull()))

            if (ldtblist.Rows.Count > 0)
                iclbPayeeAccounts = GetCollection<busPayeeAccount>(ldtblist, "icdoPayeeAccount");
            return iclbPayeeAccounts;
        }

        public void UpdateBeneficiaryAndDependentID(int aintOldPersonID, int aintNewPersonId)
        {
            DataTable ldtblist = Select("cdoRelationship.GetBeneficiaryID", new object[1] { aintOldPersonID });

            foreach (DataRow ldr in ldtblist.Rows)
            {
                busRelationship lbusRelationship = new busRelationship() { icdoRelationship = new cdoRelationship() };
                lbusRelationship.icdoRelationship.LoadData(ldr);


                if (ldr[enmRelationship.beneficiary_person_id.ToString()].ToString().IsNotNullOrEmpty() &&
                     Convert.ToInt32(ldr[enmRelationship.beneficiary_person_id.ToString()]) == aintOldPersonID)
                {
                    lbusRelationship.icdoRelationship.beneficiary_person_id = aintNewPersonId;
                    lbusRelationship.icdoRelationship.Update();
                }

                if (ldr[enmRelationship.dependent_person_id.ToString()].ToString().IsNotNullOrEmpty() &&
                      Convert.ToInt32(ldr[enmRelationship.dependent_person_id.ToString()]) == aintOldPersonID)
                {
                    lbusRelationship.icdoRelationship.dependent_person_id = aintNewPersonId;
                    lbusRelationship.icdoRelationship.Update();
                }
                if (ldr[enmRelationship.beneficiary_of.ToString()].ToString().IsNotNullOrEmpty() &&
                    Convert.ToInt32(ldr[enmRelationship.beneficiary_of.ToString()]) == aintOldPersonID)
                {
                    lbusRelationship.icdoRelationship.beneficiary_of = aintNewPersonId;
                    lbusRelationship.icdoRelationship.Update();
                }
            }

        }

        public void MergePersonContacts(int aintOldPersonID, int aintNewPersonID)
        {
            DataTable ldtbPersonContactList = busBase.Select<cdoPersonContact>(
                  new string[1] { enmPersonContact.person_id.ToString() },
                  new object[1] { aintOldPersonID }, null, null);

            foreach (DataRow ldr in ldtbPersonContactList.Rows)
            {
                busPersonContact lbusPersonContact = new busPersonContact() { icdoPersonContact = new cdoPersonContact() };
                lbusPersonContact.icdoPersonContact.LoadData(ldr);
                lbusPersonContact.icdoPersonContact.person_id = aintNewPersonID;
                lbusPersonContact.icdoPersonContact.created_by = iobjPassInfo.istrUserID;
                lbusPersonContact.icdoPersonContact.created_date = DateTime.Now;
                lbusPersonContact.icdoPersonContact.modified_by = iobjPassInfo.istrUserID;
                lbusPersonContact.icdoPersonContact.modified_date = DateTime.Now;
                lbusPersonContact.icdoPersonContact.update_seq = 0;
                lbusPersonContact.icdoPersonContact.Insert();

            }
        }

        private void MergeSuspendibleMonths(int aintOldPersonID, int aintNewPersonID)
        {
            int lintPlanYear = 0;
            string lstrSuspendibleMonth = string.Empty;
            string lstrStatus = string.Empty;
            busPersonSuspendibleMonth lbusPersonSuspendibleMonth = new busPersonSuspendibleMonth() { icdoPersonSuspendibleMonth = new cdoPersonSuspendibleMonth() };

            DataTable ldtbExistingSuspendibleMonths = busBase.Select<cdoPersonSuspendibleMonth>(
                new string[1] { enmPersonSuspendibleMonth.person_id.ToString() },
                new object[1] { aintOldPersonID }, null, null);
            DataTable ldtbNewSuspendibleMonths = busBase.Select<cdoPersonSuspendibleMonth>(
                new string[1] { enmPersonSuspendibleMonth.person_id.ToString() },
                new object[1] { aintNewPersonID }, null, null);

            foreach (DataRow ldr in ldtbExistingSuspendibleMonths.Rows)
            {
                if (ldr[enmPersonSuspendibleMonth.plan_year.ToString()] != DBNull.Value)
                    lintPlanYear = Convert.ToInt32(ldr[enmPersonSuspendibleMonth.plan_year.ToString()]);
                DataRow[] ldrExistingSuspendibleMonth = ldtbNewSuspendibleMonths.FilterTable(utlDataType.Numeric, enmPersonSuspendibleMonth.plan_year.ToString(),
                                      lintPlanYear);

                lstrSuspendibleMonth = Convert.ToString(ldr[enmPersonSuspendibleMonth.suspendible_month_value.ToString()]);
                lstrStatus = Convert.ToString(ldr[enmPersonSuspendibleMonth.status_value.ToString()]);

                if (ldrExistingSuspendibleMonth.Count() == 0)
                    lbusPersonSuspendibleMonth.InsertSuspendibleMonths(aintNewPersonID, lintPlanYear, lstrSuspendibleMonth, lstrStatus);
            }



        }

        public void MergeGeneratedCorrespondences(int aintOldPersonID, int aintNewPersonID)
        {
            DataTable ldtbResult = busBase.Select("cdoCorTracking.LoadCorTrackingByPersonID", new object[1] { aintOldPersonID });
            if (ldtbResult.IsNotNull() && ldtbResult.Rows.Count > 0)
            {
                foreach (DataRow ldr in ldtbResult.Rows)
                {
                    busCorTracking lbusCorTracking = new busCorTracking() { icdoCorTracking = new cdoCorTracking() };
                    lbusCorTracking.icdoCorTracking.LoadData(ldr);
                    lbusCorTracking.icdoCorTracking.person_id = aintNewPersonID;
                    lbusCorTracking.icdoCorTracking.Insert();
                }
            }
        }

        public void MergeNotes(int aintOldPersonID, int aintNewPersonID)
        {

            int lintorgid = 0;
            string lstrformvalue = string.Empty;
            string lstrnotes = string.Empty;
            DataTable ldtbResult = Select<cdoNotes>(
                new string[1] { enmNotes.person_id.ToString() },
                new object[1] { aintOldPersonID }, null, null);
            if (ldtbResult.Rows.Count > 0)
            {
                foreach (DataRow ldr in ldtbResult.Rows)
                {
                    busNotes lbusNotes = new busNotes() { icdoNotes = new cdoNotes() };
                    lbusNotes.icdoNotes.LoadData(ldr);

                    if (ldtbResult.Rows[0][enmNotes.org_id.ToString()].ToString().IsNotNullOrEmpty())
                        lintorgid = Convert.ToInt32(ldtbResult.Rows[0][enmNotes.org_id.ToString()]);

                    lstrformvalue = ldtbResult.Rows[0][enmNotes.form_value.ToString()].ToString();
                    lstrnotes = ldtbResult.Rows[0][enmNotes.notes.ToString()].ToString();
                    lbusNotes.InsertNotes(aintNewPersonID, lstrformvalue, lstrnotes, lintorgid);


                }

            }

        }

        public void MergeContributionAmount(int aintOldPersonID, int aintNewPersonID, int aintPlanID)
        {
            DataTable ldtbGetContributions = Select("cdoPerson.GetContribnsAndInterestByPersonID",
                                            new object[2] { aintOldPersonID, aintPlanID });
            iclbNewPerson[0].LoadPersonAccounts();

            if (ldtbGetContributions.Rows.Count > 0)
            {

                Collection<busPersonAccountRetirementContribution> lclbRetirementContribution = new Collection<busPersonAccountRetirementContribution>();

                int lintComputationYear = 0;

                DateTime ldtEffectiveDate = DateTime.MinValue;

                string lstrContributionType = null;
                string lstrContributionSubtype = null;
                lclbRetirementContribution = GetCollection<busPersonAccountRetirementContribution>(ldtbGetContributions, "icdoPersonAccountRetirementContribution");

                idecEEContribnAmt = (from item in lclbRetirementContribution select item.icdoPersonAccountRetirementContribution.ee_contribution_amount).Sum();

                idecEEInterestAmt = (from item in lclbRetirementContribution select item.icdoPersonAccountRetirementContribution.ee_int_amount).Sum();

                idecUVHPAmount = (from item in lclbRetirementContribution select item.icdoPersonAccountRetirementContribution.uvhp_amount).Sum();

                idecUVHPInterestAmt = (from item in lclbRetirementContribution select item.icdoPersonAccountRetirementContribution.uvhp_int_amount).Sum();

                busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution() { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

                //Insert negative entries for old person

                //EE Amounts
                if (idecEEContribnAmt > 0 || idecEEInterestAmt > 0)
                {
                    lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(
                                       Convert.ToInt32(ldtbGetContributions.Rows[0][enmPersonAccount.person_account_id.ToString()]), DateTime.Now, DateTime.Now,
                                            DateTime.Now.Year, 0.0M, 0.0M, 0.0M, busConstant.TRANSACTION_TYPE_SSN_ADJS, -idecEEContribnAmt,
                                            0.0M, -idecEEInterestAmt, 0.0M, 0.0M, busConstant.CONTRIBUTION_TYPE_EE, null, 0.0M, 0.0M);
                }
                //UVHP Amounts
                if (idecUVHPAmount > 0 || idecUVHPInterestAmt > 0)
                {
                    lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(
                                       Convert.ToInt32(ldtbGetContributions.Rows[0][enmPersonAccount.person_account_id.ToString()]), DateTime.Now, DateTime.Now,
                                            DateTime.Now.Year, 0.0M, 0.0M, 0.0M, busConstant.TRANSACTION_TYPE_SSN_ADJS, 0.0M,
                                            -idecUVHPAmount, 0.0M, -idecUVHPInterestAmt, 0.0M, busConstant.CONTRIBUTION_TYPE_UVHP, null, 0.0M, 0.0M);
                }

                foreach (DataRow ldr in ldtbGetContributions.Rows)
                {
                    lintComputationYear = 0;
                    idecEEContribnAmt = Decimal.Zero;
                    idecEEInterestAmt = Decimal.Zero;
                    idecUVHPAmount = Decimal.Zero;
                    idecUVHPInterestAmt = Decimal.Zero;
                    lstrContributionSubtype = String.Empty;
                    lstrContributionSubtype = String.Empty;
                    ldtEffectiveDate = DateTime.MinValue;

                    if (ldr[enmPersonAccountRetirementContribution.computational_year.ToString()].ToString().IsNotNullOrEmpty())
                    {
                        lintComputationYear = Convert.ToInt32(ldr[enmPersonAccountRetirementContribution.computational_year.ToString()]);
                    }
                    if (ldr[enmPersonAccountRetirementContribution.ee_contribution_amount.ToString()].ToString().IsNotNullOrEmpty())
                    {
                        idecEEContribnAmt = Convert.ToDecimal(ldr[enmPersonAccountRetirementContribution.ee_contribution_amount.ToString()]);
                    }
                    if (ldr[enmPersonAccountRetirementContribution.uvhp_amount.ToString()].ToString().IsNotNullOrEmpty())
                    {
                        idecUVHPAmount = Convert.ToDecimal(ldr[enmPersonAccountRetirementContribution.uvhp_amount.ToString()]);
                    }
                    if (ldr[enmPersonAccountRetirementContribution.ee_int_amount.ToString()].ToString().IsNotNullOrEmpty())
                    {
                        idecEEInterestAmt = Convert.ToDecimal(ldr[enmPersonAccountRetirementContribution.ee_int_amount.ToString()]);
                    }
                    if (ldr[enmPersonAccountRetirementContribution.uvhp_int_amount.ToString()].ToString().IsNotNullOrEmpty())
                    {
                        idecUVHPInterestAmt = Convert.ToDecimal(ldr[enmPersonAccountRetirementContribution.uvhp_int_amount.ToString()]);
                    }
                    if (ldr[enmPersonAccountRetirementContribution.effective_date.ToString()].ToString().IsNotNullOrEmpty())
                    {
                        ldtEffectiveDate = Convert.ToDateTime(ldr[enmPersonAccountRetirementContribution.effective_date.ToString()]);
                    }
                    lstrContributionType = Convert.ToString(ldr[enmPersonAccountRetirementContribution.contribution_type_value.ToString()]);
                    lstrContributionSubtype = Convert.ToString(ldr[enmPersonAccountRetirementContribution.contribution_subtype_value.ToString()]);

                    //entries for new person
                    if (iclbNewPerson[0].iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
                    {
                        lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution();
                        lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(iclbNewPerson[0].iclbPersonAccount.Where(
                       item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id,
                            ldtEffectiveDate, DateTime.Now, lintComputationYear, 0.0M, 0.0M,
                        0.0M, busConstant.TRANSACTION_TYPE_SSN_ADJS, idecEEContribnAmt, idecUVHPAmount, idecEEInterestAmt, idecUVHPInterestAmt,
                        0.0M, lstrContributionType, lstrContributionSubtype, 0.0M, 0.0M);
                    }

                }
            }
        }

        public void UpdateBenefitApplications(int aintOldPersonID, int aintNewPersonID)
        {
            DataTable ldtbGetBenApplcn = busBase.Select<cdoBenefitApplication>(
                new string[1] { enmBenefitApplication.person_id.ToString() },
                new object[1] { aintOldPersonID }, null, null);

            foreach (DataRow ldr in ldtbGetBenApplcn.Rows)
            {
                busBenefitApplication lbusBenefitApplication = new busBenefitApplication() { icdoBenefitApplication = new cdoBenefitApplication() };
                lbusBenefitApplication.icdoBenefitApplication.LoadData(ldr);
                if ((Convert.ToInt32(ldr[enmBenefitApplication.person_id.ToString()]).ToString().IsNotNullOrEmpty()) && (Convert.ToInt32(ldr[enmBenefitApplication.person_id.ToString()])) == aintOldPersonID)
                {
                    lbusBenefitApplication.icdoBenefitApplication.person_id = aintNewPersonID;
                    lbusBenefitApplication.icdoBenefitApplication.Update();
                }

                if ((ldr[enmBenefitApplication.alternate_payee_id.ToString()]) != DBNull.Value)
                {
                    if ((Convert.ToInt32(ldr[enmBenefitApplication.alternate_payee_id.ToString()])) == aintOldPersonID)
                    {
                        lbusBenefitApplication.icdoBenefitApplication.alternate_payee_id = aintNewPersonID;
                        lbusBenefitApplication.icdoBenefitApplication.Update();
                    }
                }
            }

        }

        public void UpdateBenefitCalculations(int aintOldPersonID, int aintNewPersonID)
        {
            DataTable ldtbOldBenefitDetails = Select("cdoBenefitCalculationDetail.GetBenefitDetailsByPersonID",
                                         new object[1] { aintOldPersonID });


            foreach (DataRow ldr in ldtbOldBenefitDetails.Rows)
            {
                busBenefitCalculationHeader lbusBenefitCalculationHeader = new busBenefitCalculationHeader() { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
                lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.LoadData(ldr);

                if (ldr[enmBenefitCalculationHeader.beneficiary_person_id.ToString()].ToString().IsNotNullOrEmpty() && Convert.ToInt32(ldr[enmBenefitCalculationHeader.beneficiary_person_id.ToString()]) == aintOldPersonID)
                {
                    lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.beneficiary_person_id = aintNewPersonID;
                    lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.Update();
                }
                if (ldr[enmBenefitCalculationHeader.person_id.ToString()].ToString().IsNotNullOrEmpty() && Convert.ToInt32(ldr[enmBenefitCalculationHeader.person_id.ToString()]) == aintOldPersonID)
                {
                    lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.person_id = aintNewPersonID;
                    lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.Update();
                }

                busBenefitCalculationDetail lbusBenefitCalculationDetail = new busBenefitCalculationDetail() { icdoBenefitCalculationDetail = new cdoBenefitCalculationDetail() };
                lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.LoadData(ldr);

                if (iclbNewPerson[0].iclbPersonAccount != null && iclbNewPerson[0].iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id ==
                                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id).Count() > 0)
                {
                    int lintPersonAccountId = iclbNewPerson[0].iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id ==
                                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id).First().icdoPersonAccount.person_account_id;
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.person_account_id = lintPersonAccountId;
                    lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.Update();
                }

            }



        }

        public void UpdatePayeeAccounts(int aintOldPersonID, int aintNewPersonID)
        {
            int lintNewPersonAccountID = 0;

            DataTable ldtbOldPayeeAccounts = busBase.Select<cdoPayeeAccount>(
                new string[1] { enmPayeeAccount.person_id.ToString() },
                new object[1] { aintOldPersonID }, null, null);

            DataTable ldtbOldPayeeBenefitAccount = Select("cdoPayeeBenefitAccount.GetPayeeBenefitAndPlanDetailsByPersonID", new object[1] { aintOldPersonID });

            if (ldtbOldPayeeAccounts != null && ldtbOldPayeeAccounts.Rows.Count > 0)
            {
                foreach (DataRow ldr in ldtbOldPayeeAccounts.Rows)
                {
                    busPayeeAccount lbusPayeeAccount = new busPayeeAccount() { icdoPayeeAccount = new cdoPayeeAccount() };
                    lbusPayeeAccount.icdoPayeeAccount.LoadData(ldr);
                    if ((Convert.ToInt32(ldr[enmPayeeAccount.person_id.ToString()]).ToString().IsNotNullOrEmpty()) &&
                        (Convert.ToInt32(ldr[enmPayeeAccount.person_id.ToString()])) == aintOldPersonID)
                    {
                        lbusPayeeAccount.icdoPayeeAccount.person_id = aintNewPersonID;
                        lbusPayeeAccount.icdoPayeeAccount.Update();
                    }

                    //PIR 926
                    DataTable ldtbOldPaymentHistoryHeaders = busBase.Select<cdoPaymentHistoryHeader>(
                    new string[2] { enmPaymentHistoryHeader.person_id.ToString(), enmPaymentHistoryHeader.payee_account_id.ToString() },
                    new object[2] { aintOldPersonID, lbusPayeeAccount.icdoPayeeAccount.payee_account_id }, null, null);
                    if (ldtbOldPaymentHistoryHeaders != null && ldtbOldPaymentHistoryHeaders.Rows.Count > 0)
                    {
                        foreach (DataRow ldrPhh in ldtbOldPaymentHistoryHeaders.Rows)
                        {
                            busPaymentHistoryHeader lbusPaymentHistoryHeader = new busPaymentHistoryHeader() { icdoPaymentHistoryHeader = new cdoPaymentHistoryHeader() };
                            lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.LoadData(ldrPhh);
                            if ((Convert.ToInt32(ldr[enmPayeeAccount.person_id.ToString()]).ToString().IsNotNullOrEmpty()) &&
                                      (Convert.ToInt32(ldr[enmPayeeAccount.person_id.ToString()])) == aintOldPersonID)
                            {
                                lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.person_id = aintNewPersonID;
                                lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.Update();
                            }
                        }
                    }

                }
            }
            if (ldtbOldPayeeBenefitAccount != null && ldtbOldPayeeBenefitAccount.Rows.Count > 0)
            {
                foreach (DataRow ldr in ldtbOldPayeeBenefitAccount.Rows)
                {
                    busPayeeBenefitAccount lbusPayeeBenefitAccount = new busPayeeBenefitAccount() { icdoPayeeBenefitAccount = new cdoPayeeBenefitAccount() };
                    lbusPayeeBenefitAccount.icdoPayeeBenefitAccount.LoadData(ldr);

                    if (iclbNewPerson[0].iclbPersonAccount != null && iclbNewPerson[0].iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id ==
                                     Convert.ToInt32(ldr[enmPersonAccount.plan_id.ToString()])).Count() > 0)
                    {
                        lintNewPersonAccountID = iclbNewPerson[0].iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id ==
                                        Convert.ToInt32(ldr[enmPersonAccount.plan_id.ToString()])).FirstOrDefault().icdoPersonAccount.person_account_id;

                        lbusPayeeBenefitAccount.icdoPayeeBenefitAccount.person_id = aintNewPersonID;
                        lbusPayeeBenefitAccount.icdoPayeeBenefitAccount.person_account_id = lintNewPersonAccountID;
                        lbusPayeeBenefitAccount.icdoPayeeBenefitAccount.Update();

                    }
                }
            }


        }

        private Collection<busPersonAccount> GetPersonAccountInfoToBeUpdated(string aintOldMPID, string aintNewMPID, string astrOldSSN, string astrNewSSN,
                                                                            int aintNewPersonID, Collection<busPersonAccount> aclbPersonAccountToBeInserted,
                                                                            Collection<busPersonAccount> aclbPersonAccountToBeUpdated, ref DataTable ldtbworkInfo)
        {
            LoadPersonAccounts();// Old person accounts
            iclbNewPerson[0].LoadPersonAccounts(); //New Person Account

            //IDbConnection lconEADBLegacy = DBFunction.GetDBConnection("Legacy");
            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            string lstrEADBLegacy = utlLegacyDBConnetion.istrConnectionString;

            if (lobjLegacyPassInfoEADB.iconFramework != null)
            {
                #region Merge SSN on EADB

                Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();

                IDbDataParameter lobjParameter1 = DBFunction.GetDBParameter();
                lobjParameter1.ParameterName = "@OldPID";
                lobjParameter1.DbType = DbType.String;
                lobjParameter1.Value = aintOldMPID;
                lcolParameters.Add(lobjParameter1);

                IDbDataParameter lobjParameter2 = DBFunction.GetDBParameter();
                lobjParameter2.ParameterName = "@OldSSN";
                lobjParameter2.DbType = DbType.String;
                if (astrOldSSN.IsNull())
                    astrOldSSN = "";
                lobjParameter2.Value = astrOldSSN;
                lcolParameters.Add(lobjParameter2);

                IDbDataParameter lobjParameter3 = DBFunction.GetDBParameter();
                lobjParameter3.ParameterName = "@NewPID";
                lobjParameter3.DbType = DbType.String;
                lobjParameter3.Value = aintNewMPID;
                lcolParameters.Add(lobjParameter3);

                IDbDataParameter lobjParameter4 = DBFunction.GetDBParameter();
                lobjParameter4.ParameterName = "@NewSSN";
                lobjParameter4.DbType = DbType.String;
                lobjParameter4.Value = astrNewSSN;
                lcolParameters.Add(lobjParameter4);

                DBFunction.DBExecuteProcedure("USP_PID_MERGE", lcolParameters, lobjLegacyPassInfoEADB.iconFramework, lobjLegacyPassInfoEADB.itrnFramework);

                #endregion

                SqlParameter[] parameters = new SqlParameter[1];
                SqlParameter param1 = new SqlParameter("@SSN", DbType.String);

                param1.Value = astrNewSSN;
                parameters[0] = param1;

                ldtbworkInfo = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_PensionInterface4OPUS", lstrEADBLegacy, lobjLegacyPassInfoEADB, parameters);

                if (ldtbworkInfo.Rows.Count > 0 && this.iclbPersonAccount.IsNotNull() && this.iclbPersonAccount.Count > 0)
                {
                    DateTime ldtPlanReportedDate = new DateTime();

                    busPersonAccount lbusPersonAccount = new busPersonAccount();

                    if (ldtbworkInfo.FilterTable(utlDataType.Numeric, "PensionPlan", busConstant.MPIPP_PLAN_ID).Count() > 0)
                    {
                        ldtPlanReportedDate = Convert.ToDateTime(ldtbworkInfo.FilterTable(utlDataType.Numeric, "PensionPlan", busConstant.MPIPP_PLAN_ID)[0]["FromDate"]);
                        lbusPersonAccount = lbusPersonAccount.GetPersonAccountInfo(aintNewPersonID, busConstant.MPIPP_PLAN_ID, ldtPlanReportedDate);

                        if (lbusPersonAccount != null)
                        {
                            if (!iclbNewPerson[0].CheckIfPersonHasMPI())
                            {
                                // if (this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.status_value.IsNotNullOrEmpty())
                                if (this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP && item.icdoPersonAccount.status_value != null).Count() > 0)
                                    lbusPersonAccount.icdoPersonAccount.status_value = this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.status_value;

                                // if (this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.created_by.IsNotNullOrEmpty())
                                if (this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP && item.icdoPersonAccount.created_by != null).Count() > 0)
                                    lbusPersonAccount.icdoPersonAccount.created_by = this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.created_by;

                                iclbNewPerson[0].iclbPersonAccount.Add(lbusPersonAccount);
                                aclbPersonAccountToBeInserted.Add(lbusPersonAccount);
                            }
                            else
                            {
                                iclbNewPerson[0].iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.start_date = ldtPlanReportedDate;
                                aclbPersonAccountToBeUpdated.Add(iclbNewPerson[0].iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First());
                            }
                        }
                    }
                    if (ldtbworkInfo.FilterTable(utlDataType.Numeric, "PensionPlan", busConstant.LOCAL_161_PLAN_ID).Count() > 0)
                    {
                        ldtPlanReportedDate = Convert.ToDateTime(ldtbworkInfo.FilterTable(utlDataType.Numeric, "PensionPlan", busConstant.LOCAL_161_PLAN_ID)[0]["FromDate"]);
                        lbusPersonAccount = lbusPersonAccount.GetPersonAccountInfo(aintNewPersonID, busConstant.LOCAL_161_PLAN_ID, ldtPlanReportedDate);

                        if (lbusPersonAccount != null)
                        {
                            if (!iclbNewPerson[0].CheckIfPersonHasLocal161())
                            {
                                // if (this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.status_value.IsNotNullOrEmpty())
                                if (this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_161 && item.icdoPersonAccount.status_value != null).Count() > 0)
                                    lbusPersonAccount.icdoPersonAccount.status_value = this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.status_value;

                                // if (this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.created_by.IsNotNullOrEmpty())
                                if (this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_161 && item.icdoPersonAccount.created_by != null).Count() > 0)
                                    lbusPersonAccount.icdoPersonAccount.created_by = this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.created_by;

                                iclbNewPerson[0].iclbPersonAccount.Add(lbusPersonAccount);
                                aclbPersonAccountToBeInserted.Add(lbusPersonAccount);
                            }
                            else
                            {
                                iclbNewPerson[0].iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First().icdoPersonAccount.start_date = ldtPlanReportedDate;
                                aclbPersonAccountToBeUpdated.Add(iclbNewPerson[0].iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_161).First());
                            }
                        }
                    }
                    if (ldtbworkInfo.FilterTable(utlDataType.Numeric, "PensionPlan", busConstant.LOCAL_666_PLAN_ID).Count() > 0)
                    {
                        ldtPlanReportedDate = Convert.ToDateTime(ldtbworkInfo.FilterTable(utlDataType.Numeric, "PensionPlan", busConstant.LOCAL_666_PLAN_ID)[0]["FromDate"]);
                        lbusPersonAccount = lbusPersonAccount.GetPersonAccountInfo(aintNewPersonID, busConstant.LOCAL_666_PLAN_ID, ldtPlanReportedDate);

                        if (lbusPersonAccount != null)
                        {
                            if (!iclbNewPerson[0].CheckIfPersonHasLocal666())
                            {
                                // if (this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.status_value.IsNotNullOrEmpty())
                                if (this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_666 && item.icdoPersonAccount.status_value != null).Count() > 0)
                                    lbusPersonAccount.icdoPersonAccount.status_value = this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.status_value;

                                // if (this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.created_by.IsNotNullOrEmpty())
                                if (this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_666 && item.icdoPersonAccount.created_by != null).Count() > 0)
                                    lbusPersonAccount.icdoPersonAccount.created_by = this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.created_by;

                                iclbNewPerson[0].iclbPersonAccount.Add(lbusPersonAccount);
                                aclbPersonAccountToBeInserted.Add(lbusPersonAccount);
                            }
                            else
                            {
                                iclbNewPerson[0].iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First().icdoPersonAccount.start_date = ldtPlanReportedDate;
                                aclbPersonAccountToBeUpdated.Add(iclbNewPerson[0].iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_666).First());
                            }
                        }
                    }
                    if (ldtbworkInfo.FilterTable(utlDataType.Numeric, "PensionPlan", busConstant.LOCAL_600_PLAN_ID).Count() > 0)
                    {
                        ldtPlanReportedDate = Convert.ToDateTime(ldtbworkInfo.FilterTable(utlDataType.Numeric, "PensionPlan", busConstant.LOCAL_600_PLAN_ID)[0]["FromDate"]);

                        lbusPersonAccount = lbusPersonAccount.GetPersonAccountInfo(aintNewPersonID, busConstant.LOCAL_600_PLAN_ID, ldtPlanReportedDate);

                        if (lbusPersonAccount != null)
                        {
                            if (!iclbNewPerson[0].CheckIfPersonHasLocal600())
                            {
                                //if (this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_600).FirstOrDefault().icdoPersonAccount.status_value.IsNotNullOrEmpty())
                                if (this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_600 && item.icdoPersonAccount.status_value != null).Count() > 0)
                                    lbusPersonAccount.icdoPersonAccount.status_value = this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.status_value;

                                //  if (this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.created_by.IsNotNullOrEmpty())
                                if (this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_600 && item.icdoPersonAccount.created_by != null).Count() > 0)
                                    lbusPersonAccount.icdoPersonAccount.created_by = this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.created_by;

                                iclbNewPerson[0].iclbPersonAccount.Add(lbusPersonAccount);
                                aclbPersonAccountToBeInserted.Add(lbusPersonAccount);
                            }
                            else
                            {
                                iclbNewPerson[0].iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First().icdoPersonAccount.start_date = ldtPlanReportedDate;
                                aclbPersonAccountToBeUpdated.Add(iclbNewPerson[0].iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.Local_600).First());
                            }
                        }
                    }
                    if (ldtbworkInfo.FilterTable(utlDataType.Numeric, "PensionPlan", busConstant.LOCAL_700_PLAN_ID).Count() > 0)
                    {
                        ldtPlanReportedDate = Convert.ToDateTime(ldtbworkInfo.FilterTable(utlDataType.Numeric, "PensionPlan", busConstant.LOCAL_700_PLAN_ID)[0]["FromDate"]);

                        lbusPersonAccount = lbusPersonAccount.GetPersonAccountInfo(aintNewPersonID, busConstant.LOCAL_700_PLAN_ID, ldtPlanReportedDate);

                        if (!iclbNewPerson[0].CheckIfPersonHasLocal700())
                        {
                            // if (this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.status_value.IsNotNullOrEmpty())
                            if (this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700 && item.icdoPersonAccount.status_value != null).Count() > 0)
                                lbusPersonAccount.icdoPersonAccount.status_value = this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.status_value;

                            // if (this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.created_by.IsNotNullOrEmpty())
                            if (this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700 && item.icdoPersonAccount.created_by != null).Count() > 0)
                                lbusPersonAccount.icdoPersonAccount.created_by = this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.created_by;

                            iclbNewPerson[0].iclbPersonAccount.Add(lbusPersonAccount);
                            aclbPersonAccountToBeInserted.Add(lbusPersonAccount);
                        }
                        else
                        {
                            iclbNewPerson[0].iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First().icdoPersonAccount.start_date = ldtPlanReportedDate;
                            aclbPersonAccountToBeUpdated.Add(iclbNewPerson[0].iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.LOCAL_700).First());
                        }
                    }
                    if ((from item in ldtbworkInfo.AsEnumerable() select item.Field<decimal?>("IAPHours")).Sum() > 0)
                    {
                        ldtPlanReportedDate = ldtbworkInfo.AsEnumerable().Where(item => item.Field<decimal?>("IAPHours") != 0).First().Field<DateTime>("FromDate");

                        lbusPersonAccount = lbusPersonAccount.GetPersonAccountInfo(aintNewPersonID, busConstant.IAP_PLAN_ID, ldtPlanReportedDate);

                        if (!iclbNewPerson[0].CheckIfPersonHasIAP())
                        {

                            //if (this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.status_value.IsNotNullOrEmpty())
                            if (this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP && item.icdoPersonAccount.status_value != null).Count() > 0)
                                lbusPersonAccount.icdoPersonAccount.status_value = this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.status_value;

                            // if (this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.created_by.IsNotNullOrEmpty())
                            if (this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP && item.icdoPersonAccount.created_by != null).Count() > 0)
                                lbusPersonAccount.icdoPersonAccount.created_by = this.iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.created_by;

                            iclbNewPerson[0].iclbPersonAccount.Add(lbusPersonAccount);
                            aclbPersonAccountToBeInserted.Add(lbusPersonAccount);
                        }
                        else
                        {
                            iclbNewPerson[0].iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).First().icdoPersonAccount.start_date = ldtPlanReportedDate;
                            aclbPersonAccountToBeUpdated.Add(iclbNewPerson[0].iclbPersonAccount.Where(item => item.icdoPersonAccount.istrPlanCode == busConstant.IAP).First());
                        }
                    }
                    //}
                }

            }
            return aclbPersonAccountToBeInserted;
        }

        public void MergeBeneficiariesAndDependents(int aintOldPersonID, int aintNewPersonId)
        {
            DataTable ldtbOldBeneficiaries = Select("cdoRelationship.GetListOfBeneficiariesAndDependents", new object[1] { aintOldPersonID });

            DataTable ldtbNewBeneficiaries = Select("cdoRelationship.GetListOfBeneficiariesAndDependents", new object[1] { aintNewPersonId });

            foreach (DataRow ldr in ldtbOldBeneficiaries.Rows)
            {
                DataRow[] ldrBenOrDepExists = null;

                if (Convert.ToString(ldr[enmRelationship.beneficiary_person_id.ToString()]).IsNotNullOrEmpty())
                {
                    ldrBenOrDepExists = ldtbNewBeneficiaries.FilterTable(utlDataType.Numeric,
                      enmRelationship.beneficiary_person_id.ToString(), Convert.ToInt32(ldr[enmRelationship.beneficiary_person_id.ToString()]));
                }
                else if (Convert.ToString(ldr[enmRelationship.dependent_person_id.ToString()]).IsNotNullOrEmpty())
                {
                    ldrBenOrDepExists = ldtbNewBeneficiaries.FilterTable(utlDataType.Numeric,
                      enmRelationship.dependent_person_id.ToString(), Convert.ToInt32(ldr[enmRelationship.dependent_person_id.ToString()]));
                }


                if (ldrBenOrDepExists.IsNotNull() && ldrBenOrDepExists.Count() == 0)
                {
                    DateTime ldtMarriageDate = new DateTime();
                    DateTime ldtStartDate = new DateTime();
                    DateTime ldtEndDate = new DateTime();
                    DateTime ldtAccountBeneficiaryStartDate = new DateTime();
                    DateTime ldtAccountBeneficiaryEndDate = new DateTime();
                    int lintBeneficiaryOf = 0, lintDependentID = 0, lintBeneficiaryID = 0;
                    decimal ldecPercent = 0;

                    if (ldr[enmRelationship.date_of_marriage.ToString()].ToString().IsNotNullOrEmpty())
                    {
                        ldtMarriageDate = Convert.ToDateTime(ldr[enmRelationship.date_of_marriage.ToString()]);
                    }
                    if (ldr[enmRelationship.beneficiary_of.ToString()].ToString().IsNotNullOrEmpty())
                    {
                        lintBeneficiaryOf = Convert.ToInt32(ldr[enmRelationship.beneficiary_of.ToString()]);
                    }
                    if (ldr[enmRelationship.effective_start_date.ToString()].ToString().IsNotNullOrEmpty())
                    {
                        ldtStartDate = Convert.ToDateTime(ldr[enmRelationship.effective_start_date.ToString()]);
                    }
                    if (ldr[enmRelationship.effective_end_date.ToString()].ToString().IsNotNullOrEmpty())
                    {
                        ldtEndDate = Convert.ToDateTime(ldr[enmRelationship.effective_end_date.ToString()]);
                    }

                    if (ldr[enmRelationship.dependent_person_id.ToString()].ToString().IsNotNullOrEmpty())
                        lintDependentID = Convert.ToInt32(ldr[enmRelationship.dependent_person_id.ToString()]);

                    if (ldr[enmRelationship.beneficiary_person_id.ToString()].ToString().IsNotNullOrEmpty())
                        lintBeneficiaryID = Convert.ToInt32(ldr[enmRelationship.beneficiary_person_id.ToString()]);

                    //if (ldr[enmPersonAccountBeneficiary.dist_percent.ToString()].ToString().IsNotNullOrEmpty())
                    //{
                    //    ldecPercent = Convert.ToDecimal(ldr[enmPersonAccountBeneficiary.dist_percent.ToString()]);
                    //}
                    if (ldr[enmPersonAccountBeneficiary.start_date.ToString()].ToString().IsNotNullOrEmpty())
                    {
                        ldtAccountBeneficiaryStartDate = Convert.ToDateTime(ldr[enmPersonAccountBeneficiary.start_date.ToString()]);
                    }
                    if (ldr[enmPersonAccountBeneficiary.end_date.ToString()].ToString().IsNotNullOrEmpty())
                    {
                        ldtAccountBeneficiaryEndDate = Convert.ToDateTime(ldr[enmPersonAccountBeneficiary.end_date.ToString()]);
                    }

                    busRelationship lbusRelationship = new busRelationship();
                    int lintRelationshipId = lbusRelationship.InsertPersonRelationship(aintNewPersonId, lintBeneficiaryID,
                    lintDependentID, ldr[enmRelationship.relationship_value.ToString()].ToString(), ldr[enmRelationship.addr_same_as_participant_flag.ToString()].ToString(),
                    ldtMarriageDate, ldr[enmRelationship.beneficiary_from_value.ToString()].ToString(), lintBeneficiaryOf, ldtStartDate, ldtEndDate);

                    if (lintBeneficiaryID != 0)
                    {
                        int lintNewPersonAccountId = 0;

                        if (ldr[enmPersonAccount.plan_id.ToString()] != DBNull.Value)
                            lintNewPersonAccountId = GetPersonAccountIdByPlanAndPersonId(aintNewPersonId, Convert.ToInt32(ldr[enmPersonAccount.plan_id.ToString()]));
                        busPersonAccountBeneficiary lbusPersonAccountBeneficiary = new busPersonAccountBeneficiary();

                        if (lintNewPersonAccountId != 0)
                        {
                            lbusPersonAccountBeneficiary.InsertValuesInPersonAccBeneficiary(lintRelationshipId, lintNewPersonAccountId, ldtAccountBeneficiaryStartDate,
                            ldtAccountBeneficiaryEndDate, ldecPercent, Convert.ToString(ldr[enmPersonAccountBeneficiary.beneficiary_type_value.ToString()]),
                            Convert.ToString(ldr[enmPersonAccountBeneficiary.status_value.ToString()]));
                        }

                        lbusPersonAccountBeneficiary = new busPersonAccountBeneficiary
                        {
                            icdoPersonAccountBeneficiary = new cdoPersonAccountBeneficiary()
                        };
                        //lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.LoadData(ldr);
                        //lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.dist_percent = 0;
                        //lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.Update();
                    }
                }
            }
        }

        public bool CheckSSNExistsInHistory(busPerson abusNewPerson)
        {
            if (abusNewPerson.icdoPerson.istrSSNNonEncrypted != null)
            {
                DataTable ldtbList = Select<cdoSsnMergeHistory>(
                new string[1] { enmSsnMergeHistory.old_ssn.ToString() },
                new object[1] { abusNewPerson.icdoPerson.istrSSNNonEncrypted }, null, null);

                if (ldtbList.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
                return false;
        }

        private bool CheckIFPlanAlreadyExists(Collection<busPersonAccount> aclbPersonAccount, int aintPlanId)
        {
            if (aclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.MPIPP_PLAN_ID).Count() > 0)
                return true;

            return false;
        }

        public ArrayList btn_SearchWithMPID(string astrMPID)
        {

            ArrayList larr = new ArrayList();
            utlError lobjError = null;

            busPerson lbusPerson = new busPerson();

            if (string.IsNullOrEmpty(astrMPID))
            {
                lobjError = AddError(6214, "");
                larr.Add(lobjError);
                return larr;
            }

            DataTable ldtbResult = busBase.Select("cdoPerson.GetPersonDetailsByMPID", new object[1] { istrMPID });


            if (ldtbResult.Rows.Count > 0)
            {

                //if ((!(ldtbResult.Rows[0]["DATE_OF_BIRTH"] != DBNull.Value && Convert.ToDateTime(ldtbResult.Rows[0]["DATE_OF_BIRTH"]).Date != ldtMinVal)) ||
                //    (ldtbResult.Rows[0]["SSN"].ToString()).IsNullOrEmpty()
                //       || (ldtbResult.Rows[0]["FIRST_NAME"].ToString()).IsNullOrEmpty() || (ldtbResult.Rows[0]["LAST_NAME"].ToString()).IsNullOrEmpty())
                //{
                //    lobjError = AddError(6209, "");
                //    larr.Add(lobjError);
                //    return larr;
                //}

                lbusPerson.icdoPerson = new cdoPerson();
                lbusPerson.icdoPerson.LoadData(ldtbResult.Rows[0]);

                ldtbResult.Columns.Add("istrPersonType", Type.GetType("System.String"));
                ldtbResult.Columns.Add("UnionCode", Type.GetType("System.String"));
                ldtbResult.Columns.Add("istrEmployerName", Type.GetType("System.String"));
                ldtbResult.Columns.Add("istrParticipantAddress", Type.GetType("System.String"));

                if (ldtbResult.Rows[0]["MPI_PERSON_ID"].ToString() == icdoPerson.mpi_person_id)
                {
                    lobjError = AddError(6138, "");
                    larr.Add(lobjError);
                    return larr;
                }
                ldtbResult.Rows[0]["istrParticipantAddress"] = GetMailingAddress(Convert.ToInt32(ldtbResult.Rows[0]["PERSON_ID"]));

                string astrSSN = ldtbResult.Rows[0]["SSN"].ToString();

                if (!String.IsNullOrEmpty(astrSSN))
                {
                    string lstrEmployerName = string.Empty;
                    int lintUnionCode = 0;
                    //ldtbResult.Rows[0]["UnionCode"] = GetTrueUnionCodeBySSN(astrSSN);
                    //ldtbResult.Rows[0]["istrEmployerName"] = 
                    GetEmployerNameBySSN(astrSSN, ref lstrEmployerName, ref lintUnionCode);
                    ldtbResult.Rows[0]["UnionCode"] = Convert.ToString(lintUnionCode);
                    ldtbResult.Rows[0]["istrEmployerName"] = lstrEmployerName;
                }

                lbusPerson.LoadInitialData();
                if (lbusPerson.iblnBeneficiary == busConstant.YES)
                {
                    ldtbResult.Rows[0]["istrPersonType"] = busGlobalFunctions.GetCodeValueDescriptionByValue(1509, busConstant.PERSON_TYPE_SURVIVOR).description;
                }
                if (lbusPerson.iblnAlternatePayee == busConstant.YES)
                {
                    ldtbResult.Rows[0]["istrPersonType"] = busGlobalFunctions.GetCodeValueDescriptionByValue(1509, busConstant.PERSON_TYPE_ALTERNATE_PAYEE).description;
                }
                if (lbusPerson.iblnParticipant == busConstant.YES)
                {
                    ldtbResult.Rows[0]["istrPersonType"] = busGlobalFunctions.GetCodeValueDescriptionByValue(1509, busConstant.PERSON_TYPE_PARTICIPANT).description;
                }

                if (iclbOldPersonToBeMerged != null)
                    iclbOldPersonToBeMerged.Clear();
                iclbOldPersonToBeMerged = GetCollection<busPerson>(ldtbResult, "icdoPerson");

                foreach (busPerson lbusOldPersonToBeMerged in iclbOldPersonToBeMerged)
                {
                    lbusOldPersonToBeMerged.icdoPerson.iintNewMergedMPIID = icdoPerson.mpi_person_id;
                }

                if (lbusPerson.icdoPerson.person_id > 0)
                {
                    GetLatestPayeeAccountsForPossibleDuplicatesOnSearch(lbusPerson);
                }

                larr.Add(this);
            }
            else
            {
                lobjError = AddError(6213, "");
                larr.Add(lobjError);
                return larr;

            }
            return larr;
        }

        public ArrayList btn_MergeClick()
        {
            ArrayList larr = new ArrayList();
            utlError lobjError1 = null;

            try
            {
                if ((iobjPassInfo.iconFramework).Database != "MPI" && (iobjPassInfo.iconFramework).Database != "MPIPHP")
                {
                    //IDbConnection lconQNXTLegacy = null;
                    Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();

                    //@NewPID varchar(9) ,@OldPID varchar(9) ,@NewPIDClmCnt int output ,@OldPIDClmCnt int output ,@Debug bit = 0

                    //if (lconQNXTLegacy == null)
                    //{
                    //    lconQNXTLegacy = DBFunction.GetDBConnection("QNXTLegacy");
                    //}

                    IDbDataParameter lobjParameter1 = DBFunction.GetDBParameter();
                    lobjParameter1.ParameterName = "@NewPID";
                    lobjParameter1.DbType = DbType.String;
                    lobjParameter1.Value = iclbNewPerson[0].icdoPerson.mpi_person_id;
                    lcolParameters.Add(lobjParameter1);

                    IDbDataParameter lobjParameter2 = DBFunction.GetDBParameter();
                    lobjParameter2.ParameterName = "@OldPID";
                    lobjParameter2.DbType = DbType.String;
                    lobjParameter2.Value = iclbOldPersonToBeMerged[0].icdoPerson.mpi_person_id;
                    lcolParameters.Add(lobjParameter2);

                    IDbDataParameter lobjParameter3 = DBFunction.GetDBParameter();
                    lobjParameter3.ParameterName = "@NewPIDClmCnt";
                    lobjParameter3.DbType = DbType.Int32;
                    lobjParameter3.Value = 0;
                    lobjParameter3.Direction = ParameterDirection.InputOutput;
                    lcolParameters.Add(lobjParameter3);

                    IDbDataParameter lobjParameter4 = DBFunction.GetDBParameter();
                    lobjParameter4.ParameterName = "@OldPIDClmCnt";
                    lobjParameter4.DbType = DbType.Int32;
                    lobjParameter4.Value = 0;
                    lobjParameter4.Direction = ParameterDirection.InputOutput;
                    lcolParameters.Add(lobjParameter4);

                    //DBFunction.DBExecuteProcedure("spm_Opus_MemberClaimCnt", lcolParameters, lconQNXTLegacy, null);
                    //lconQNXTLegacy.Close();

                    if (Convert.ToInt32(lcolParameters[3].Value) > 0)
                    {
                        lobjError1 = AddError(0, "MPID:-" + iclbNewPerson[0].icdoPerson.mpi_person_id + " has " + Convert.ToInt32(lcolParameters[2].Value).ToString() + " and MPID:-" + iclbOldPersonToBeMerged[0].icdoPerson.mpi_person_id + " has " + Convert.ToInt32(lcolParameters[3].Value).ToString() + " claims associated with it.");
                        larr.Add(lobjError1);
                        return larr;
                    }
                }

                if (this.iclbMergedPersonReord.Count == 0)
                {
                    lobjError1 = AddError(6158, " ");
                    larr.Add(lobjError1);
                    return larr;
                }

                //if (iclbNewPerson[0].icdoPerson.idtDateofBirth != iclbOldPersonToBeMerged[0].icdoPerson.idtDateofBirth)
                //&&
                //iclbMergedPersonReord[0].icdoPerson.idtDateofBirth == iclbOldPersonToBeMerged[0].icdoPerson.idtDateofBirth) 
                //{
                //  int lintCountPayees = (int)DBFunction.DBExecuteScalar("cdoPerson.GetPayeeAccountWithStatusNotCncld", new object[1] { this.icdoPerson.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework, iobjPassInfo.isrvMetaDataCache);
                //if (lintCountPayees > 0)
                //{
                //lobjError1 = AddError(6218, "");
                //larr.Add(lobjError1);
                //return larr;
                //}
                //}

                this.iclbOldPersonToBeMerged[0].icdoPerson.ssn = this.iclbOldPersonToBeMerged[0].icdoPerson.istrSSNNonEncrypted;
                this.iclbNewPerson[0].icdoPerson.ssn = this.iclbNewPerson[0].icdoPerson.istrSSNNonEncrypted;
                this.iclbMergedPersonReord[0].icdoPerson.ssn = this.iclbMergedPersonReord[0].icdoPerson.istrSSNNonEncrypted;

                //ChangeID: 53632
                //Checking if the old SSN has any payee account other than Cancelled or Payments Completed status. Reused the method GetPayeeAccountStatusForDeathNotification for this purpose.
                if (this.iclbNewPerson[0].icdoPerson.ssn != this.iclbOldPersonToBeMerged[0].icdoPerson.ssn)
                {
                    int lintCountPayees = (int)DBFunction.DBExecuteScalar("cdoPerson.GetPayeeAccountWithStatusNotCncldCmpl", new object[2] { this.iclbNewPerson[0].icdoPerson.person_id, this.iclbOldPersonToBeMerged[0].icdoPerson.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                    bool IsManager = false;
                    busUserRoles lbusUserRoles = new busUserRoles { icdoRoles = new cdoRoles() };
                    if (lbusUserRoles.FindUserRoles(iobjPassInfo.iintUserSerialID, busConstant.Role.MANAGER_ROLE))
                    {
                        IsManager = true;
                    }

                    if (lintCountPayees > 0 && !IsManager)
                    {
                        lobjError1 = AddError(6288, "");
                        larr.Add(lobjError1);
                        return larr;
                    }

                }

                var transactionOptions = new TransactionOptions();
                transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                transactionOptions.Timeout = TransactionManager.MaximumTimeout;

                using (TransactionScope ds = new TransactionScope(TransactionScopeOption.Required, transactionOptions)) // Distributed Transcation Co-ordinator should be SWITCHED ON on the server
                {
                    Collection<busPersonAccount> lclbPersonAccountInfotoBeInserted = new Collection<busPersonAccount>();
                    Collection<busPersonAccount> lclbPersonAccountToBeUpdated = new Collection<busPersonAccount>();

                    busSsnMergeHistory lbusSsnMergeHistory = new busSsnMergeHistory();
                    utlError lobjError = null;

                    if (lobjLegacyPassInfoEADB.IsNull())
                    {
                        lobjLegacyPassInfoEADB = new utlPassInfo();
                    }
                    //if (lobjLegacyPassInfoHEDB.IsNull())
                    //{
                    //    lobjLegacyPassInfoHEDB = new utlPassInfo();
                    //}
                    if (lobjLegacy.IsNull())
                    {
                        lobjLegacy = new utlPassInfo();
                    }
                    lobjLegacyPassInfoEADB.iconFramework = DBFunction.GetDBConnection("Legacy");
                    // lobjLegacyPassInfoHEDB.iconFramework = DBFunction.GetDBConnection("HELegacy");
                    lobjLegacy.iconFramework = DBFunction.GetDBConnection("LookupDB");

                    #region Code for SSn Merge

                    DataTable ldtbWorkInfo = new DataTable();
                    DataTable ldtbIAPWorkInfo = new DataTable();

                    string lstrOldSSN = string.Empty;

                    if (iclbOldPersonToBeMerged[0].icdoPerson.istrSSNNonEncrypted == this.iclbMergedPersonReord[0].icdoPerson.istrSSNNonEncrypted)
                    {
                        lstrOldSSN = iclbNewPerson[0].icdoPerson.istrSSNNonEncrypted;
                    }
                    else
                    {
                        lstrOldSSN = iclbOldPersonToBeMerged[0].icdoPerson.istrSSNNonEncrypted;
                    }


                    GetPersonAccountInfoToBeUpdated(iclbOldPersonToBeMerged[0].icdoPerson.mpi_person_id, iclbNewPerson[0].icdoPerson.mpi_person_id,
                                                    lstrOldSSN, this.iclbMergedPersonReord[0].icdoPerson.istrSSNNonEncrypted,
                                                    iclbNewPerson[0].icdoPerson.person_id, lclbPersonAccountInfotoBeInserted, lclbPersonAccountToBeUpdated, ref ldtbWorkInfo);

                    if (iclbPersonAccount != null && iclbPersonAccount.Count > 0)
                    {
                        foreach (busPersonAccount lbusOldPersonAccount in iclbPersonAccount)
                        {
                            if (iclbNewPerson[0].iclbPersonAccount != null &&
                                iclbNewPerson[0].iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == lbusOldPersonAccount.icdoPersonAccount.plan_id).Count() == 0)
                            {
                                lobjError = AddError(6177, " ");
                                larr.Add(lobjError);
                                return larr;
                            }
                        }
                    }
                    // decommissioning demographics informations, since HEDB is retiring.
                    //   UpdateHEDBPersonAndAddressInfo();

                    //  UpdateHEDBInfo(lstrOldSSN, this.iclbMergedPersonReord[0].icdoPerson.istrSSNNonEncrypted);

                    //158717
                    //UpdateHoursOnImagingSystem(lstrOldSSN, this.iclbMergedPersonReord[0].icdoPerson.istrSSNNonEncrypted);


                    int lintMergeHistoryId = lbusSsnMergeHistory.InsertSSNMergeHistoryOfPerson(iclbOldPersonToBeMerged[0].icdoPerson.person_id,
                    lstrOldSSN, iclbOldPersonToBeMerged[0].icdoPerson.mpi_person_id,
                    iclbNewPerson[0].icdoPerson.person_id, iclbMergedPersonReord[0].icdoPerson.istrSSNNonEncrypted,
                    iclbMergedPersonReord[0].icdoPerson.mpi_person_id);

                    foreach (busPersonAccount lbusOldPersonAccount in lclbPersonAccountInfotoBeInserted)
                    {
                        lbusOldPersonAccount.icdoPersonAccount.Insert();
                    }

                    foreach (busPersonAccount lbusPersonAccountToBeUpdated in lclbPersonAccountToBeUpdated)
                    {
                        lbusPersonAccountToBeUpdated.icdoPersonAccount.Update();
                    }

                    //Inserting records in person account eligibility table to get the local qualified years and frozen hours

                    foreach (busPersonAccount lbusPersonAccountOld in iclbPersonAccount)
                    {

                        if (lclbPersonAccountInfotoBeInserted.Where(i => i.icdoPersonAccount.plan_id == lbusPersonAccountOld.icdoPersonAccount.plan_id).Count() > 0)
                        {
                            busPersonAccountEligibility lbusPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };
                            DataTable ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { lbusPersonAccountOld.icdoPersonAccount.person_account_id });
                            iclbPersonAccountEligibility = GetCollection<busPersonAccountEligibility>(ldtbPersonAccountEligibility, "icdoPersonAccountEligibility");

                            if (iclbPersonAccountEligibility.Count > 0)
                            {
                                foreach (busPersonAccountEligibility lbusPersonAccountEligibilityOld in iclbPersonAccountEligibility)
                                {
                                    lbusPersonAccountEligibility.icdoPersonAccountEligibility.person_account_id = lclbPersonAccountInfotoBeInserted.Where(i => i.icdoPersonAccount.plan_id == lbusPersonAccountOld.icdoPersonAccount.plan_id).Select(y => y.icdoPersonAccount.person_account_id).SingleOrDefault();
                                    lbusPersonAccountEligibility.icdoPersonAccountEligibility.vested_date = Convert.ToDateTime(lbusPersonAccountEligibilityOld.icdoPersonAccountEligibility.vested_date);
                                    lbusPersonAccountEligibility.icdoPersonAccountEligibility.vesting_rule = Convert.ToString(lbusPersonAccountEligibilityOld.icdoPersonAccountEligibility.vesting_rule);
                                    lbusPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date = Convert.ToDateTime(lbusPersonAccountEligibilityOld.icdoPersonAccountEligibility.forfeiture_date);
                                    lbusPersonAccountEligibility.icdoPersonAccountEligibility.last_evaluated_date = Convert.ToDateTime(lbusPersonAccountEligibilityOld.icdoPersonAccountEligibility.last_evaluated_date);
                                    lbusPersonAccountEligibility.icdoPersonAccountEligibility.status_id = lbusPersonAccountEligibilityOld.icdoPersonAccountEligibility.status_id;
                                    lbusPersonAccountEligibility.icdoPersonAccountEligibility.status_value = Convert.ToString(lbusPersonAccountEligibilityOld.icdoPersonAccountEligibility.status_value);
                                    lbusPersonAccountEligibility.icdoPersonAccountEligibility.update_seq = lbusPersonAccountEligibilityOld.icdoPersonAccountEligibility.update_seq;
                                    lbusPersonAccountEligibility.icdoPersonAccountEligibility.pension_credits = lbusPersonAccountEligibilityOld.icdoPersonAccountEligibility.pension_credits;
                                    lbusPersonAccountEligibility.icdoPersonAccountEligibility.local_qualified_years = lbusPersonAccountEligibilityOld.icdoPersonAccountEligibility.local_qualified_years;
                                    lbusPersonAccountEligibility.icdoPersonAccountEligibility.local_frozen_hours = lbusPersonAccountEligibilityOld.icdoPersonAccountEligibility.local_frozen_hours;
                                    lbusPersonAccountEligibility.icdoPersonAccountEligibility.Insert();
                                }

                            }

                        }

                        if (lclbPersonAccountToBeUpdated.Where(i => i.icdoPersonAccount.plan_id == lbusPersonAccountOld.icdoPersonAccount.plan_id).Count() > 0)
                        {
                            busPersonAccountEligibility lbusPersonAccountEligibilityUpdate = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };
                            DataTable ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { lbusPersonAccountOld.icdoPersonAccount.person_account_id });
                            iclbPersonAccountEligibility = GetCollection<busPersonAccountEligibility>(ldtbPersonAccountEligibility, "icdoPersonAccountEligibility");

                            if (iclbPersonAccountEligibility.Count > 0)
                            {
                                foreach (busPersonAccountEligibility lbusPersonAccountEligibilityOld in iclbPersonAccountEligibility)
                                {
                                    lbusPersonAccountEligibilityUpdate.icdoPersonAccountEligibility.person_account_eligibility_id = lbusPersonAccountEligibilityOld.icdoPersonAccountEligibility.person_account_eligibility_id;
                                    lbusPersonAccountEligibilityUpdate.icdoPersonAccountEligibility.person_account_id = lbusPersonAccountOld.icdoPersonAccount.person_account_id;
                                    lbusPersonAccountEligibilityUpdate.icdoPersonAccountEligibility.vested_date = Convert.ToDateTime(lbusPersonAccountEligibilityOld.icdoPersonAccountEligibility.vested_date);
                                    lbusPersonAccountEligibilityUpdate.icdoPersonAccountEligibility.vesting_rule = Convert.ToString(lbusPersonAccountEligibilityOld.icdoPersonAccountEligibility.vesting_rule);
                                    lbusPersonAccountEligibilityUpdate.icdoPersonAccountEligibility.forfeiture_date = Convert.ToDateTime(lbusPersonAccountEligibilityOld.icdoPersonAccountEligibility.forfeiture_date);
                                    lbusPersonAccountEligibilityUpdate.icdoPersonAccountEligibility.last_evaluated_date = Convert.ToDateTime(lbusPersonAccountEligibilityOld.icdoPersonAccountEligibility.last_evaluated_date);
                                    lbusPersonAccountEligibilityUpdate.icdoPersonAccountEligibility.status_id = lbusPersonAccountEligibilityOld.icdoPersonAccountEligibility.status_id;
                                    lbusPersonAccountEligibilityUpdate.icdoPersonAccountEligibility.status_value = Convert.ToString(lbusPersonAccountEligibilityOld.icdoPersonAccountEligibility.status_value);
                                    lbusPersonAccountEligibilityUpdate.icdoPersonAccountEligibility.update_seq = lbusPersonAccountEligibilityOld.icdoPersonAccountEligibility.update_seq;
                                    lbusPersonAccountEligibilityUpdate.icdoPersonAccountEligibility.pension_credits = lbusPersonAccountEligibilityOld.icdoPersonAccountEligibility.pension_credits;
                                    lbusPersonAccountEligibilityUpdate.icdoPersonAccountEligibility.local_qualified_years = lbusPersonAccountEligibilityOld.icdoPersonAccountEligibility.local_qualified_years;
                                    lbusPersonAccountEligibilityUpdate.icdoPersonAccountEligibility.local_frozen_hours = lbusPersonAccountEligibilityOld.icdoPersonAccountEligibility.local_frozen_hours;
                                    lbusPersonAccountEligibilityUpdate.icdoPersonAccountEligibility.Update();
                                }

                            }

                        }



                    }

                    MergeContributionAmount(iclbOldPersonToBeMerged[0].icdoPerson.person_id, iclbNewPerson[0].icdoPerson.person_id, busConstant.MPIPP_PLAN_ID);
                    if (ldtbWorkInfo.Rows.Count > 0)
                    {
                        //ldtbIAPWorkInfo = ldtbWorkInfo.AsEnumerable().Where(item => item.Field<decimal>("IAPHoursA2") != 0).CopyToDataTable();
                        //Sid Jain 04052013
                        ldtbIAPWorkInfo = ldtbWorkInfo.AsEnumerable().CopyToDataTable();

                        Collection<cdoDummyWorkData> lclbWorkData = new Collection<cdoDummyWorkData>();

                        busBenefitApplication lbusBenefitApplication = new busBenefitApplication();
                        lbusBenefitApplication.icdoBenefitApplication = new cdoBenefitApplication();

                        int lintNewIAPPersonAccountId = 0, lintOldIAPPersonAccountId = 0;

                        if (iclbNewPerson[0].iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).Count() > 0)
                        {
                            lintNewIAPPersonAccountId = iclbNewPerson[0].iclbPersonAccount.Where(
                                item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id;
                            lbusBenefitApplication.ibusPerson = iclbMergedPersonReord[0];
                            lbusBenefitApplication.ibusPerson.iclbPersonAccount =
                                    iclbNewPerson[0].iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).ToList().ToCollection();
                            lbusBenefitApplication.ibusPerson.iclbPersonAccount[0].icdoPersonAccount.istrPlanCode = busConstant.IAP;
                            lbusBenefitApplication.LoadandProcessWorkHistory_ForAllPlans(true, lobjLegacyPassInfoEADB);
                            lclbWorkData = lbusBenefitApplication.aclbPersonWorkHistory_IAP;
                        }

                        if (iclbOldPersonToBeMerged[0].iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).Count() > 0)
                        {
                            lintOldIAPPersonAccountId = iclbOldPersonToBeMerged[0].iclbPersonAccount.Where(
                                item => item.icdoPersonAccount.plan_id == busConstant.IAP_PLAN_ID).FirstOrDefault().icdoPersonAccount.person_account_id;
                        }

                        SetParametersForIAPAllocationMerging(ldtbIAPWorkInfo, lclbWorkData, lintNewIAPPersonAccountId, lintOldIAPPersonAccountId);
                    }
                    UpdateBeneficiaryAndDependentID(iclbOldPersonToBeMerged[0].icdoPerson.person_id, iclbNewPerson[0].icdoPerson.person_id);
                    MergeBeneficiariesAndDependents(iclbOldPersonToBeMerged[0].icdoPerson.person_id, iclbNewPerson[0].icdoPerson.person_id);
                    MergeNotes(iclbOldPersonToBeMerged[0].icdoPerson.person_id, iclbNewPerson[0].icdoPerson.person_id);
                    MergeGeneratedCorrespondences(iclbOldPersonToBeMerged[0].icdoPerson.person_id, iclbNewPerson[0].icdoPerson.person_id);
                    MergePersonContacts(iclbOldPersonToBeMerged[0].icdoPerson.person_id, iclbNewPerson[0].icdoPerson.person_id);
                    MergeSuspendibleMonths(iclbOldPersonToBeMerged[0].icdoPerson.person_id, iclbNewPerson[0].icdoPerson.person_id);
                    MergeBridgedHours();

                    UpdateBenefitApplications(iclbOldPersonToBeMerged[0].icdoPerson.person_id, iclbNewPerson[0].icdoPerson.person_id);
                    UpdateBenefitCalculations(iclbOldPersonToBeMerged[0].icdoPerson.person_id, iclbNewPerson[0].icdoPerson.person_id);
                    UpdatePayeeAccounts(iclbOldPersonToBeMerged[0].icdoPerson.person_id, iclbNewPerson[0].icdoPerson.person_id);
                    MergePersonDetails();

                    this.icdoPerson.is_person_deleted_flag = busConstant.FLAG_YES;
                    this.icdoPerson.Update();

                    iclbNewPerson[0].icdoPerson.recalculate_vesting_flag = busConstant.FLAG_YES;
                    iclbNewPerson[0].icdoPerson.Update();

                    larr.Add(this);
                    EvaluateInitialLoadRules();
                    #endregion

                    ds.Complete();

                    //PIR-258-Initiate Workflow
                    busWorkflowHelper.InitializeWorkflowIfNotExists(busConstant.SSN_MERGE_IAP_RECALCULATION, iclbNewPerson[0].icdoPerson.person_id,
                       0, iclbNewPerson[0].icdoPerson.person_id, null);
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
                    larr.Add(lError);
                    throw (e);
                }
            }
            finally
            {
                //if (lobjLegacyPassInfoEADB.IsNotNull() && lobjLegacyPassInfoEADB.itrnFramework.IsNotNull())
                if (lobjLegacyPassInfoEADB.IsNotNull() && lobjLegacyPassInfoEADB.iconFramework != null
                    && lobjLegacyPassInfoEADB.iconFramework.State == ConnectionState.Open)
                {
                    lobjLegacyPassInfoEADB.iconFramework.Close();
                    lobjLegacyPassInfoEADB.iconFramework.Dispose();
                }

                //if (lobjLegacyPassInfoHEDB.IsNotNull() && lobjLegacyPassInfoHEDB.itrnFramework.IsNotNull())
                //if (lobjLegacyPassInfoHEDB.IsNotNull() && lobjLegacyPassInfoHEDB.iconFramework != null
                //    && lobjLegacyPassInfoHEDB.iconFramework.State == ConnectionState.Open)
                //{
                //    lobjLegacyPassInfoHEDB.iconFramework.Close();
                //    lobjLegacyPassInfoHEDB.iconFramework.Dispose();
                //}

                //if (lobjLegacy.IsNotNull() && lobjLegacy.itrnFramework.IsNotNull())
                if (lobjLegacy.IsNotNull() && lobjLegacy.iconFramework != null &&
                    lobjLegacy.iconFramework.State == ConnectionState.Open)
                {
                    lobjLegacy.iconFramework.Close();
                    lobjLegacy.iconFramework.Dispose();
                }
            }
            return larr;
        }

        //  private void UpdateHEDBInfo(string astrOldSSN, string astrNewSSN)
        //   {

        //IDbConnection lconEADBLegacy = DBFunction.GetDBConnection("HELegacy");
        //utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("HELegacy");

        //if (lobjLegacyPassInfoHEDB.iconFramework != null)
        //{
        //    #region Merge SSN on HEADB

        //    Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();

        //    IDbDataParameter lobjParameter1 = DBFunction.GetDBParameter();
        //    lobjParameter1.ParameterName = "@OldPID";
        //    lobjParameter1.DbType = DbType.String;
        //    lobjParameter1.Value = iclbOldPersonToBeMerged[0].icdoPerson.mpi_person_id;
        //    lcolParameters.Add(lobjParameter1);

        //    IDbDataParameter lobjParameter2 = DBFunction.GetDBParameter();
        //    lobjParameter2.ParameterName = "@OldSSN";
        //    lobjParameter2.DbType = DbType.String;
        //    if (astrOldSSN.IsNull())
        //        astrOldSSN = "";
        //    lobjParameter2.Value = astrOldSSN;
        //    lcolParameters.Add(lobjParameter2);

        //    IDbDataParameter lobjParameter3 = DBFunction.GetDBParameter();
        //    lobjParameter3.ParameterName = "@NewPID";
        //    lobjParameter3.DbType = DbType.String;
        //    lobjParameter3.Value = iclbNewPerson[0].icdoPerson.mpi_person_id;
        //    lcolParameters.Add(lobjParameter3);

        //    IDbDataParameter lobjParameter4 = DBFunction.GetDBParameter();
        //    lobjParameter4.ParameterName = "@NewSSN";
        //    lobjParameter4.DbType = DbType.String;
        //    lobjParameter4.Value = astrNewSSN;
        //    lcolParameters.Add(lobjParameter4);

        //    IDbDataParameter lobjParameter5 = DBFunction.GetDBParameter();
        //    lobjParameter5.ParameterName = "@Notes";
        //    lobjParameter5.DbType = DbType.String;
        //    lobjParameter5.Value = "SSN Merged";
        //    lcolParameters.Add(lobjParameter5);

        //    IDbDataParameter lobjParameter6 = DBFunction.GetDBParameter();
        //    lobjParameter6.ParameterName = "@ADH";
        //    lobjParameter6.DbType = DbType.String;
        //    lobjParameter6.Value = DBNull.Value; //f/w Framework Upgrade Fix 
        //                                        //Previously null was getting passsed to this parameter.This null value was getting converted into char(1) blank string
        //                                        //which was causing issues in SP. Now null has been changed to DBNull.Value
        //    lcolParameters.Add(lobjParameter6);

        //    IDbDataParameter lobjParameter7 = DBFunction.GetDBParameter();
        //    lobjParameter7.ParameterName = "@MergeDate";
        //    lobjParameter7.DbType = DbType.DateTime;
        //    lobjParameter7.Value = DateTime.Now;
        //    lcolParameters.Add(lobjParameter7);

        //    DBFunction.DBExecuteProcedure("USP_PID_Merge", lcolParameters, lobjLegacyPassInfoHEDB.iconFramework, lobjLegacyPassInfoHEDB.itrnFramework);

        //    #endregion
        //}
        //  }

        private void UpdateHoursOnImagingSystem(string astrOldSSN, string astrNewSSN)
        {
            //IDbConnection lconEADBLegacy = DBFunction.GetDBConnection("LookupDB");
            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("LookupDB");

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

        //public void UpdateHEDBPersonAndAddressInfo()
        //{

        //    if (iclbMergedPersonReord.Count > 0 && iclbMergedPersonReord.First() != null)
        //    {
        //        busPerson lbusPerson = new busPerson { icdoPerson = new cdoPerson() };
        //        lbusPerson.FindPerson(iclbNewPerson[0].icdoPerson.person_id);
        //        //Ticket : 55015
        //        bool lboolVipFlag = false;
        //        if (lbusPerson != null)
        //        {
        //            string lstrVipFlag = this.icdoPerson.vip_flag;
        //            string lUpperlstrVipFlag = lstrVipFlag.ToUpper();

        //            if (lUpperlstrVipFlag == "Y")
        //            {
        //                lboolVipFlag = true;
        //            }
        //        }

        //        if (lobjLegacyPassInfoHEDB.iconFramework != null)
        //        {
        //            Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();

        //            IDbDataParameter lobjParameter1 = DBFunction.GetDBParameter();
        //            lobjParameter1.ParameterName = "@PID";
        //            lobjParameter1.DbType = DbType.String;
        //            lobjParameter1.Value = iclbMergedPersonReord.First().icdoPerson.mpi_person_id.ToLower();
        //            lcolParameters.Add(lobjParameter1);

        //            if (iclbNewPerson.First().icdoPerson.istrSSNNonEncrypted.IsNotNullOrEmpty())
        //            {
        //                IDbDataParameter lobjParameter2 = DBFunction.GetDBParameter();
        //                lobjParameter2.ParameterName = "@SSN";
        //                lobjParameter2.DbType = DbType.String;
        //                lobjParameter2.Value = iclbNewPerson.First().icdoPerson.istrSSNNonEncrypted.ToLower();
        //                lcolParameters.Add(lobjParameter2);
        //            }

        //            IDbDataParameter lobjParameter4 = DBFunction.GetDBParameter();
        //            lobjParameter4.ParameterName = "@EntityTypeCode";
        //            lobjParameter4.DbType = DbType.String;
        //            lobjParameter4.Value = "P";                 //for now we will always use Person
        //            lcolParameters.Add(lobjParameter4);


        //            IDbDataParameter lobjParameter6 = DBFunction.GetDBParameter();
        //            lobjParameter6.ParameterName = "@FirstName";
        //            lobjParameter6.DbType = DbType.String;
        //            lobjParameter6.Value = iclbMergedPersonReord.First().icdoPerson.first_name;
        //            lcolParameters.Add(lobjParameter6);

        //            IDbDataParameter lobjParameter7 = DBFunction.GetDBParameter();
        //            lobjParameter7.ParameterName = "@MiddleName";
        //            lobjParameter7.DbType = DbType.String;
        //            lobjParameter7.Value = iclbMergedPersonReord.First().icdoPerson.middle_name;
        //            lcolParameters.Add(lobjParameter7);

        //            IDbDataParameter lobjParameter8 = DBFunction.GetDBParameter();
        //            lobjParameter8.ParameterName = "@LastName";
        //            lobjParameter8.DbType = DbType.String;
        //            lobjParameter8.Value = iclbMergedPersonReord.First().icdoPerson.last_name;
        //            lcolParameters.Add(lobjParameter8);

        //            IDbDataParameter lobjParameter9 = DBFunction.GetDBParameter();
        //            lobjParameter9.ParameterName = "@Gender";
        //            lobjParameter9.DbType = DbType.String;
        //            lobjParameter9.Value = lbusPerson.icdoPerson.gender_value;
        //            lcolParameters.Add(lobjParameter9);


        //            IDbDataParameter lobjParameter10 = DBFunction.GetDBParameter();
        //            lobjParameter10.ParameterName = "@DateOfBirth";
        //            lobjParameter10.DbType = DbType.DateTime;
        //            if (iclbMergedPersonReord.First().icdoPerson.idtDateofBirth != DateTime.MinValue)
        //            {
        //                lobjParameter10.Value = iclbMergedPersonReord.First().icdoPerson.idtDateofBirth;
        //            }
        //            else
        //            {
        //                lobjParameter10.Value = DBNull.Value;
        //            }
        //            lcolParameters.Add(lobjParameter10);


        //            IDbDataParameter lobjParameter11 = DBFunction.GetDBParameter();
        //            lobjParameter11.ParameterName = "@DateOfDeath";
        //            lobjParameter11.DbType = DbType.DateTime;

        //            if (iclbMergedPersonReord.First().icdoPerson.date_of_death != DateTime.MinValue)
        //            {
        //                lobjParameter11.Value = iclbMergedPersonReord.First().icdoPerson.date_of_death;
        //            }
        //            else
        //            {
        //                lobjParameter11.Value = DBNull.Value;
        //            }
        //            lcolParameters.Add(lobjParameter11);

        //            IDbDataParameter lobjParameter12 = DBFunction.GetDBParameter();
        //            lobjParameter12.ParameterName = "@HomePhone";
        //            lobjParameter12.DbType = DbType.String;
        //            lobjParameter12.Value = lbusPerson.icdoPerson.home_phone_no;
        //            lcolParameters.Add(lobjParameter12);

        //            IDbDataParameter lobjParameter13 = DBFunction.GetDBParameter();
        //            lobjParameter13.ParameterName = "@CellPhone";
        //            lobjParameter13.DbType = DbType.String;
        //            lobjParameter13.Value = lbusPerson.icdoPerson.cell_phone_no;
        //            lcolParameters.Add(lobjParameter13);

        //            IDbDataParameter lobjParameter14 = DBFunction.GetDBParameter();
        //            lobjParameter14.ParameterName = "@Fax";
        //            lobjParameter14.DbType = DbType.String;
        //            lobjParameter14.Value = lbusPerson.icdoPerson.fax_no;
        //            lcolParameters.Add(lobjParameter14);

        //            IDbDataParameter lobjParameter15 = DBFunction.GetDBParameter();
        //            lobjParameter15.ParameterName = "@Email";
        //            lobjParameter15.DbType = DbType.String;
        //            lobjParameter15.Value = lbusPerson.icdoPerson.email_address_1;
        //            lcolParameters.Add(lobjParameter15);

        //            IDbDataParameter lobjParameter16 = DBFunction.GetDBParameter();
        //            lobjParameter16.ParameterName = "@AuditUser";
        //            lobjParameter16.DbType = DbType.String;
        //            lobjParameter16.Value = iobjPassInfo.istrUserID;
        //            lcolParameters.Add(lobjParameter16);
        //            //Ticket : 55015
        //            IDbDataParameter lobjParameter17 = DBFunction.GetDBParameter();
        //            lobjParameter17.ParameterName = "@VipFlag";
        //            lobjParameter17.DbType = DbType.Boolean;
        //            lobjParameter17.Value = lboolVipFlag;
        //            lcolParameters.Add(lobjParameter17);
        //            DBFunction.DBExecuteProcedure("USP_PID_Person_UPD", lcolParameters, lobjLegacyPassInfoHEDB.iconFramework, lobjLegacyPassInfoHEDB.itrnFramework);


        //        }


        //        if (lobjLegacyPassInfoHEDB.iconFramework != null)
        //        {

        //            string lstrContactName = string.Empty;
        //            string lstrAddrline1 = string.Empty;
        //            string lstrAddrline2 = string.Empty;
        //            string lstrCity = string.Empty;
        //            string lstrState = string.Empty;
        //            string lstrPostalCode = string.Empty;
        //            string lstrCountry = string.Empty;
        //            string lstrCountryValue = string.Empty;
        //            int lintForeignAddrFlag = 0;
        //            int lintDoNotUpdate = 0;
        //            bool lbnIsContact = false;
        //            string lstrAuditUser = string.Empty;
        //            string lstrPersonMpiID = string.Empty;
        //            DateTime ldtAddressEndDate = DateTime.MinValue;
        //            string lstrAddressSource = string.Empty;
        //            string lstrBadAddressFlag = string.Empty;
        //            DateTime ldtAddressStartDate = DateTime.MinValue;

        //            busPersonAddress lbusPersonAddress = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() };
        //            if (iclbNewPerson[0].IsToAddressChecked.IsNotNull() && iclbNewPerson[0].IsToAddressChecked == busConstant.FLAG_YES && iclbNewPerson[0].istrPersonAddress != null)
        //            {
        //                lbusPersonAddress = iclbNewPerson[0].ibusPersonAddress.ibusMainParticipantAddress;
        //            }
        //            else
        //            {
        //                lbusPersonAddress = iclbOldPersonToBeMerged[0].ibusPersonAddress.ibusMainParticipantAddress;
        //            }

        //            if (lbusPersonAddress != null && lbusPersonAddress.icdoPersonAddress.address_id > 0)
        //            {
        //                lstrAddrline1 = lbusPersonAddress.icdoPersonAddress.addr_line_1;
        //                lstrAddrline2 = lbusPersonAddress.icdoPersonAddress.addr_line_2;
        //                lstrCity = lbusPersonAddress.icdoPersonAddress.addr_city;

        //                if (Convert.ToInt32(lbusPersonAddress.icdoPersonAddress.addr_country_value) == busConstant.USA || Convert.ToInt32(lbusPersonAddress.icdoPersonAddress.addr_country_value) == busConstant.AUSTRALIA
        //                    || Convert.ToInt32(lbusPersonAddress.icdoPersonAddress.addr_country_value) == busConstant.CANADA || Convert.ToInt32(lbusPersonAddress.icdoPersonAddress.addr_country_value) == busConstant.MEXICO
        //                    || Convert.ToInt32(lbusPersonAddress.icdoPersonAddress.addr_country_value) == busConstant.NewZealand)
        //                {
        //                    lstrState = lbusPersonAddress.icdoPersonAddress.addr_state_value;
        //                }
        //                else
        //                {
        //                    lstrState = lbusPersonAddress.icdoPersonAddress.foreign_province;
        //                }

        //                if (Convert.ToInt32(lbusPersonAddress.icdoPersonAddress.addr_country_value) == busConstant.USA)
        //                {

        //                    lstrPostalCode = lbusPersonAddress.icdoPersonAddress.addr_zip_code + lbusPersonAddress.icdoPersonAddress.addr_zip_4_code;
        //                }
        //                else
        //                {
        //                    lstrPostalCode = lbusPersonAddress.icdoPersonAddress.foreign_postal_code;
        //                }

        //                lstrCountry = lbusPersonAddress.icdoPersonAddress.addr_country_description;
        //                if (!string.IsNullOrEmpty(lbusPersonAddress.icdoPersonAddress.addr_country_value))
        //                {
        //                    lstrCountryValue = HelperUtil.GetData1ByCodeValue(lbusPersonAddress.icdoPersonAddress.addr_country_id, lbusPersonAddress.icdoPersonAddress.addr_country_value);
        //                }
        //                if (Convert.ToInt32(lbusPersonAddress.icdoPersonAddress.addr_country_value) == busConstant.USA)
        //                {
        //                    lintForeignAddrFlag = 0;
        //                }
        //                else
        //                {
        //                    lintForeignAddrFlag = 1;
        //                }

        //                if (lbusPersonAddress.icdoPersonAddress.secured_flag == "Y")
        //                {
        //                    lintDoNotUpdate = 1;
        //                }
        //                else
        //                {
        //                    lintDoNotUpdate = 0;
        //                }

        //                lstrAuditUser = iobjPassInfo.istrUserID;

        //                ldtAddressStartDate = lbusPersonAddress.icdoPersonAddress.start_date;

        //                if (lbusPersonAddress.icdoPersonAddress.end_date.IsNotNull() && lbusPersonAddress.icdoPersonAddress.end_date != DateTime.MinValue)
        //                    ldtAddressEndDate = lbusPersonAddress.icdoPersonAddress.end_date;
        //                else
        //                    ldtAddressEndDate = DateTime.MinValue;

        //                lstrAddressSource = lbusPersonAddress.icdoPersonAddress.addr_source_description;
        //                lstrBadAddressFlag = lbusPersonAddress.icdoPersonAddress.bad_address_flag;


        //                lstrPersonMpiID = iclbMergedPersonReord.First().icdoPerson.mpi_person_id;


        //                Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();

        //                IDbDataParameter lobjParameter1 = DBFunction.GetDBParameter();
        //                lobjParameter1.ParameterName = "@PID";
        //                lobjParameter1.DbType = DbType.String;
        //                lobjParameter1.Value = lstrPersonMpiID.ToLower();
        //                lcolParameters.Add(lobjParameter1);

        //                IDbDataParameter lobjParameter4 = DBFunction.GetDBParameter();
        //                lobjParameter4.ParameterName = "@Address1";
        //                lobjParameter4.DbType = DbType.String;
        //                lobjParameter4.Value = lstrAddrline1;
        //                lcolParameters.Add(lobjParameter4);

        //                IDbDataParameter lobjParameter5 = DBFunction.GetDBParameter();
        //                lobjParameter5.ParameterName = "@Address2";
        //                lobjParameter5.DbType = DbType.String;
        //                lobjParameter5.Value = lstrAddrline2;
        //                lcolParameters.Add(lobjParameter5);

        //                IDbDataParameter lobjParameter6 = DBFunction.GetDBParameter();
        //                lobjParameter6.ParameterName = "@City";
        //                lobjParameter6.DbType = DbType.String;
        //                lobjParameter6.Value = lstrCity;
        //                lcolParameters.Add(lobjParameter6);

        //                IDbDataParameter lobjParameter7 = DBFunction.GetDBParameter();
        //                lobjParameter7.ParameterName = "@State";
        //                lobjParameter7.DbType = DbType.String;
        //                lobjParameter7.Value = lstrState;
        //                lcolParameters.Add(lobjParameter7);

        //                IDbDataParameter lobjParameter8 = DBFunction.GetDBParameter();
        //                lobjParameter8.ParameterName = "@PostalCode";
        //                lobjParameter8.DbType = DbType.String;
        //                lobjParameter8.Value = lstrPostalCode;
        //                lcolParameters.Add(lobjParameter8);

        //                IDbDataParameter lobjParameter9 = DBFunction.GetDBParameter();
        //                lobjParameter9.ParameterName = "@Country";
        //                lobjParameter9.DbType = DbType.String;
        //                lobjParameter9.Value = lstrCountry;
        //                lcolParameters.Add(lobjParameter9);

        //                IDbDataParameter lobjParameter10 = DBFunction.GetDBParameter();
        //                lobjParameter10.ParameterName = "@CountryCode";
        //                lobjParameter10.DbType = DbType.String;
        //                lobjParameter10.Value = lstrCountryValue;
        //                lcolParameters.Add(lobjParameter10);

        //                IDbDataParameter lobjParameter11 = DBFunction.GetDBParameter();
        //                lobjParameter11.ParameterName = "@ForeignAddr";
        //                lobjParameter11.DbType = DbType.String;
        //                lobjParameter11.Value = lintForeignAddrFlag;
        //                lcolParameters.Add(lobjParameter11);


        //                IDbDataParameter lobjParameter12 = DBFunction.GetDBParameter();
        //                lobjParameter12.ParameterName = "@ReturnedMail";
        //                lobjParameter12.DbType = DbType.DateTime;
        //                if (ldtAddressEndDate.IsNotNull() && lstrBadAddressFlag.IsNotNull() && ldtAddressEndDate != DateTime.MinValue && lstrBadAddressFlag == busConstant.FLAG_YES)
        //                    lobjParameter12.Value = ldtAddressEndDate;
        //                else
        //                    lobjParameter12.Value = DBNull.Value;

        //                lcolParameters.Add(lobjParameter12);

        //                if (!lbnIsContact)
        //                {
        //                    IDbDataParameter lobjParameter13 = DBFunction.GetDBParameter();
        //                    lobjParameter13.ParameterName = "@DoNotUpdate";
        //                    lobjParameter13.DbType = DbType.String;
        //                    lobjParameter13.Value = lintDoNotUpdate;
        //                    lcolParameters.Add(lobjParameter13);
        //                }

        //                IDbDataParameter lobjParameter14 = DBFunction.GetDBParameter();
        //                lobjParameter14.ParameterName = "@AuditUser";
        //                lobjParameter14.DbType = DbType.String;
        //                lobjParameter14.Value = lstrAuditUser;
        //                lcolParameters.Add(lobjParameter14);

        //                IDbDataParameter lobjParameter15 = DBFunction.GetDBParameter();
        //                lobjParameter15.ParameterName = "@ReceivedFrom";
        //                lobjParameter15.DbType = DbType.String;
        //                lobjParameter15.Value = lstrAddressSource;
        //                lcolParameters.Add(lobjParameter15);

        //                IDbDataParameter lobjParameter16 = DBFunction.GetDBParameter();
        //                lobjParameter16.ParameterName = "@AddressStartDate";
        //                lobjParameter16.DbType = DbType.DateTime;
        //                lobjParameter16.Value = ldtAddressStartDate;
        //                lcolParameters.Add(lobjParameter16);

        //                DBFunction.DBExecuteProcedure("USP_PID_PersonAddress_UPD", lcolParameters, lobjLegacyPassInfoHEDB.iconFramework, lobjLegacyPassInfoHEDB.itrnFramework);
        //            }
        //        }
        //    }
        //}

        public Collection<busPayeeAccount> GetLatestPayeeAccountsForPossibleDuplicatesOnSearch(busPerson abusPerson)
        {

            iclbPayeeAccountsForPossibleDuplicates = new Collection<busPayeeAccount>();
            DataTable ldtblist = Select("cdoPayeeAccount.GetLatestPayeeAccounts", new object[1] { abusPerson.icdoPerson.person_id });
            iclbPayeeAccountsForPossibleDuplicates = GetCollection<busPayeeAccount>(ldtblist, "icdoPayeeAccount");
            return iclbPayeeAccountsForPossibleDuplicates;
        }

        public Collection<busPayeeAccount> GetLatestPayeeAccountsForPossibleDuplicates(busPerson abusNewPerson)
        {
            iclbPayeeAccountsForPossibleDuplicates = new Collection<busPayeeAccount>();
            DataTable ldtTempDataTable = new DataTable();
            DataTable ldtbResult = new DataTable();
            if (abusNewPerson.icdoPerson.date_of_birth != DateTime.MinValue && abusNewPerson.icdoPerson.first_name.IsNotNullOrEmpty() && abusNewPerson.icdoPerson.last_name.IsNotNullOrEmpty()
                && abusNewPerson.icdoPerson.istrSSNNonEncrypted.IsNotNullOrEmpty())
            {
                ldtbResult = Select("cdoPerson.GetPossibleDuplicateSSNs", new object[5] { abusNewPerson.icdoPerson.first_name , abusNewPerson.icdoPerson.last_name,
                            abusNewPerson.icdoPerson.date_of_birth,abusNewPerson.icdoPerson.istrSSNNonEncrypted, abusNewPerson.icdoPerson.person_id});
            }
            if (ldtbResult != null && ldtbResult.Rows.Count > 0)
            {

                ldtTempDataTable.Columns.Add(enmPayeeAccount.payee_account_id.ToString(), typeof(int));
                ldtTempDataTable.Columns.Add("istrMPID", typeof(string));
                ldtTempDataTable.Columns.Add("iintPlanId", typeof(int));
                ldtTempDataTable.Columns.Add("istrPlanDescription", typeof(string));
                ldtTempDataTable.Columns.Add("istrStatusDesc", typeof(string));

                DataRow ldrTempDataTable;

                DataTable ldtblist = new DataTable();
                foreach (DataRow dr in ldtbResult.Rows)
                {
                    ldtblist = Select("cdoPayeeAccount.GetLatestPayeeAccounts", new object[1] { dr["PERSON_ID"] });
                    if (ldtblist.Rows.Count > 0)
                    {
                        foreach (DataRow drr in ldtblist.Rows)
                        {
                            ldrTempDataTable = ldtTempDataTable.NewRow();

                            ldrTempDataTable[enmPayeeAccount.payee_account_id.ToString()] = drr[enmPayeeAccount.payee_account_id.ToString()];
                            ldrTempDataTable["istrMPID"] = Convert.ToString(drr["istrMPID"]);
                            ldrTempDataTable["iintPlanId"] = Convert.ToInt32(drr["iintPlanId"]);
                            ldrTempDataTable["istrPlanDescription"] = Convert.ToString(drr["istrPlanDescription"]);
                            ldrTempDataTable["istrStatusDesc"] = Convert.ToString(drr["istrStatusDesc"]);

                            ldtTempDataTable.Rows.Add(ldrTempDataTable);
                        }
                    }
                }
            }
            iclbPayeeAccountsForPossibleDuplicates = GetCollection<busPayeeAccount>(ldtTempDataTable, "icdoPayeeAccount");
            return iclbPayeeAccountsForPossibleDuplicates;
        }

        public Collection<busPerson> LoadPossibleSSNRecords(busPerson abusNewPerson)
        {
            busPerson lbusPerson = new busPerson();
            DataTable ldtbResult = new DataTable();
            iclbOldPersonToBeMerged = new Collection<busPerson>();
            if (abusNewPerson.icdoPerson.date_of_birth != DateTime.MinValue && abusNewPerson.icdoPerson.first_name.IsNotNullOrEmpty() && abusNewPerson.icdoPerson.last_name.IsNotNullOrEmpty()
              && abusNewPerson.icdoPerson.istrSSNNonEncrypted.IsNotNullOrEmpty())
            {
                ldtbResult = Select("cdoPerson.GetPossibleDuplicateSSNs", new object[5] { abusNewPerson.icdoPerson.first_name , abusNewPerson.icdoPerson.last_name,
                            abusNewPerson.icdoPerson.date_of_birth,abusNewPerson.icdoPerson.istrSSNNonEncrypted, abusNewPerson.icdoPerson.person_id});

                DataColumn ldcPersonType = new DataColumn("istrPersonType", Type.GetType("System.String"));
                ldtbResult.Columns.Add(ldcPersonType);
                DataColumn ldcUnionCode = new DataColumn("UnionCode", Type.GetType("System.String"));
                ldtbResult.Columns.Add(ldcUnionCode);
                DataColumn ldcEmployeeName = new DataColumn("istrEmployerName", Type.GetType("System.String"));
                ldtbResult.Columns.Add(ldcEmployeeName);
                DataColumn ldcMailingAddres = new DataColumn("istrParticipantAddress", Type.GetType("System.String"));
                ldtbResult.Columns.Add(ldcMailingAddres);

            }
            if (ldtbResult != null && ldtbResult.Rows.Count > 0)
            {
                foreach (DataRow dr in ldtbResult.Rows)
                {
                    string astrSSN = dr["SSN"].ToString();

                    if (!String.IsNullOrEmpty(astrSSN))
                    {
                        string lstrEmployerName = string.Empty;
                        int lintUnionCode = 0;
                        //ldtbResult.Rows[0]["UnionCode"] = GetTrueUnionCodeBySSN(astrSSN);
                        //ldtbResult.Rows[0]["istrEmployerName"] = 
                        GetEmployerNameBySSN(astrSSN, ref lstrEmployerName, ref lintUnionCode);
                        ldtbResult.Rows[0]["UnionCode"] = Convert.ToString(lintUnionCode);
                        ldtbResult.Rows[0]["istrEmployerName"] = lstrEmployerName;

                        if (lbusPerson.FindPerson(dr["MPI_PERSON_ID"].ToString()))
                        {
                            lbusPerson.LoadInitialData();
                            if (lbusPerson.iblnBeneficiary == busConstant.YES)
                            {
                                dr["istrPersonType"] = busGlobalFunctions.GetCodeValueDescriptionByValue(1509, busConstant.PERSON_TYPE_SURVIVOR).description;
                            }
                            if (lbusPerson.iblnAlternatePayee == busConstant.YES)
                            {
                                dr["istrPersonType"] = busGlobalFunctions.GetCodeValueDescriptionByValue(1509, busConstant.PERSON_TYPE_ALTERNATE_PAYEE).description;
                            }
                            if (lbusPerson.iblnParticipant == busConstant.YES)
                            {
                                dr["istrPersonType"] = busGlobalFunctions.GetCodeValueDescriptionByValue(1509, busConstant.PERSON_TYPE_PARTICIPANT).description;
                            }
                        }

                        int lintPersonId = lbusPerson.icdoPerson.person_id;
                        dr["istrParticipantAddress"] = GetMailingAddress(lintPersonId);
                    }

                }
                if (iclbOldPersonToBeMerged != null)
                    iclbOldPersonToBeMerged = GetCollection<busPerson>(ldtbResult, "icdoPerson");
                foreach (busPerson lbusOldPersonToBeMerged in iclbOldPersonToBeMerged)
                {
                    lbusOldPersonToBeMerged.icdoPerson.iintNewMergedMPIID = abusNewPerson.istrNewMergedMPIID;
                }
            }

            return iclbOldPersonToBeMerged;

        }

        private void MergePersonDetails()
        {
            //store values for old person

            if (iclbOldPersonToBeMerged[0].icdoPerson.istrSSNNonEncrypted == iclbMergedPersonReord[0].icdoPerson.istrSSNNonEncrypted)
            {
                iclbOldPersonToBeMerged[0].icdoPerson.ssn = iclbNewPerson[0].icdoPerson.ssn;
                iclbOldPersonToBeMerged[0].icdoPerson.istrSSNNonEncrypted = iclbNewPerson[0].icdoPerson.ssn;
            }
            if (iclbOldPersonToBeMerged[0].icdoPerson.first_name == iclbMergedPersonReord[0].icdoPerson.first_name)
                iclbOldPersonToBeMerged[0].icdoPerson.first_name = iclbNewPerson[0].icdoPerson.first_name;
            if (iclbOldPersonToBeMerged[0].icdoPerson.middle_name == iclbMergedPersonReord[0].icdoPerson.middle_name)
                iclbOldPersonToBeMerged[0].icdoPerson.middle_name = iclbNewPerson[0].icdoPerson.middle_name;
            if (iclbOldPersonToBeMerged[0].icdoPerson.last_name == iclbMergedPersonReord[0].icdoPerson.last_name)
                iclbOldPersonToBeMerged[0].icdoPerson.last_name = iclbNewPerson[0].icdoPerson.last_name;
            if (iclbOldPersonToBeMerged[0].icdoPerson.name_prefix_value == iclbMergedPersonReord[0].icdoPerson.name_prefix_value)
                iclbOldPersonToBeMerged[0].icdoPerson.name_prefix_value = iclbNewPerson[0].icdoPerson.name_prefix_value;
            if (iclbOldPersonToBeMerged[0].icdoPerson.name_suffix == iclbMergedPersonReord[0].icdoPerson.name_suffix)
                iclbOldPersonToBeMerged[0].icdoPerson.name_suffix = iclbNewPerson[0].icdoPerson.name_suffix;
            if (iclbOldPersonToBeMerged[0].icdoPerson.idtDateofBirth == iclbMergedPersonReord[0].icdoPerson.idtDateofBirth)
            {
                iclbOldPersonToBeMerged[0].icdoPerson.idtDateofBirth = iclbNewPerson[0].icdoPerson.date_of_birth;
                iclbOldPersonToBeMerged[0].icdoPerson.date_of_birth = iclbNewPerson[0].icdoPerson.date_of_birth;
            }
            if (iclbOldPersonToBeMerged[0].icdoPerson.date_of_death == iclbMergedPersonReord[0].icdoPerson.date_of_death)
                iclbOldPersonToBeMerged[0].icdoPerson.date_of_death = iclbNewPerson[0].icdoPerson.date_of_death;

            if (iclbOldPersonToBeMerged[0].istrPersonAddress == iclbMergedPersonReord[0].istrPersonAddress)
                iclbOldPersonToBeMerged[0].istrPersonAddress = iclbNewPerson[0].istrPersonAddress;
            if (!iclbMergedPersonReord.IsNullOrEmpty())
            {
                iclbNewPerson[0].icdoPerson.ssn = iclbMergedPersonReord[0].icdoPerson.istrSSNNonEncrypted;
                iclbNewPerson[0].icdoPerson.istrSSNNonEncrypted = iclbMergedPersonReord[0].icdoPerson.istrSSNNonEncrypted;
                iclbNewPerson[0].icdoPerson.last_name = iclbMergedPersonReord[0].icdoPerson.last_name;
                iclbNewPerson[0].icdoPerson.first_name = iclbMergedPersonReord[0].icdoPerson.first_name;
                iclbNewPerson[0].icdoPerson.middle_name = iclbMergedPersonReord[0].icdoPerson.middle_name;
                iclbNewPerson[0].icdoPerson.name_prefix_value = iclbMergedPersonReord[0].icdoPerson.name_prefix_value;
                iclbNewPerson[0].icdoPerson.name_suffix = iclbMergedPersonReord[0].icdoPerson.name_suffix;
                iclbNewPerson[0].icdoPerson.date_of_birth = iclbMergedPersonReord[0].icdoPerson.idtDateofBirth;
                iclbNewPerson[0].icdoPerson.idtDateofBirth = iclbMergedPersonReord[0].icdoPerson.idtDateofBirth;
                iclbNewPerson[0].icdoPerson.date_of_death = iclbMergedPersonReord[0].icdoPerson.date_of_death;
                iclbNewPerson[0].istrPersonAddress = iclbMergedPersonReord[0].istrPersonAddress;


                if (ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress != null)
                {

                    if (iclbOldPersonToBeMerged[0].IsFromAddressChecked == busConstant.FLAG_YES)
                    {
                        if (ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.address_id != 0)
                        {
                            ///TO SWAP VALUES

                            int lintFromAddresID = ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.address_id;
                            int lintFromPersonID = ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.person_id;

                            int lintNewAddressID = ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.address_id;
                            int lintNewPersonID = ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.person_id;


                            busPersonAddress lbusPersonAdd = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() };


                            #region Taking from 'TO' Inserting in 'From'


                            lbusPersonAdd.icdoPersonAddress.addr_line_1 = ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_line_1;
                            lbusPersonAdd.icdoPersonAddress.addr_line_2 = ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_line_2;
                            lbusPersonAdd.icdoPersonAddress.addr_zip_code = ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_zip_code;
                            lbusPersonAdd.icdoPersonAddress.addr_state_id = ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_state_id;
                            lbusPersonAdd.icdoPersonAddress.addr_state_value = ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_state_value;
                            lbusPersonAdd.icdoPersonAddress.addr_city = ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_city;
                            lbusPersonAdd.icdoPersonAddress.addr_country_id = ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_country_id;
                            lbusPersonAdd.icdoPersonAddress.addr_country_value = ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_country_value;
                            lbusPersonAdd.icdoPersonAddress.addr_source_id = ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_source_id;
                            lbusPersonAdd.icdoPersonAddress.addr_source_value = ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_source_value;
                            lbusPersonAdd.icdoPersonAddress.start_date = ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.start_date;
                            lbusPersonAdd.icdoPersonAddress.person_id = lintFromPersonID;
                            lbusPersonAdd.icdoPersonAddress.Insert();

                            busPersonAddressChklist lbusPersonAddressChklist = new busPersonAddressChklist();
                            Collection<busPersonAddressChklist> lclbusPersonAddressChklist = new Collection<busPersonAddressChklist>();
                            lclbusPersonAddressChklist = lbusPersonAddressChklist.LoadChecklistByAddressId(
                                                    ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.address_id);

                            foreach (busPersonAddressChklist lPersonAddressChklist in lclbusPersonAddressChklist)
                            {
                                lbusPersonAddressChklist.InsertDataInPersonAddressChecklist(
                                    lbusPersonAdd.icdoPersonAddress.address_id,
                                    lPersonAddressChklist.icdoPersonAddressChklist.address_type_value);
                            }

                            #endregion

                            #region UPDATE 'TO'
                            ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_line_1 = ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_line_1;
                            ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_line_2 = ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_line_2;
                            ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_zip_code = ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_zip_code;
                            ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_state_id = ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_state_id;
                            ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_state_value = ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_state_value;
                            ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_city = ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_city;
                            ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_country_id = ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_country_id;
                            ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_country_value = ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_country_value;
                            ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_source_id = ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_source_id;
                            ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_source_value = ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_source_value;
                            ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.start_date = ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.start_date;
                            ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.Update();

                            busPersonAddressChklist lbusPersonAddressChklistNew = new busPersonAddressChklist();
                            Collection<busPersonAddressChklist> lclbusPersonAddressChklistNew = new Collection<busPersonAddressChklist>();
                            lclbusPersonAddressChklistNew = lbusPersonAddressChklistNew.LoadChecklistByAddressId(
                                               ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.address_id);
                            if (lclbusPersonAddressChklistNew != null)
                            {
                                foreach (busPersonAddressChklist lPersonAddressChklist in lclbusPersonAddressChklistNew)
                                {
                                    lPersonAddressChklist.icdoPersonAddressChklist.Delete();
                                }
                            }

                            lclbusPersonAddressChklistNew = lbusPersonAddressChklistNew.LoadChecklistByAddressId(
                                               ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.address_id);
                            if (lclbusPersonAddressChklistNew != null)
                            {
                                foreach (busPersonAddressChklist lPersonAddressChklist in lclbusPersonAddressChklistNew)
                                {
                                    lbusPersonAddressChklist.InsertDataInPersonAddressChecklist(
                                     ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.address_id,
                                    lPersonAddressChklist.icdoPersonAddressChklist.address_type_value);
                                }
                            }
                            #endregion

                            #region DELETE ORGINAL CHECKLIST OF 'FROM'

                            if (ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.address_id > 0)
                            {
                                busPersonAddressChklist lbusPersonAddressChklistOld = new busPersonAddressChklist();
                                Collection<busPersonAddressChklist> lclbusPersonAddressChklistOld = new Collection<busPersonAddressChklist>();

                                ibusPersonAddress.ibusMainParticipantAddress.iclbPersonAddressHistory = new Collection<busPersonAddressHistory>();
                                ibusPersonAddress.ibusMainParticipantAddress.LoadPersonAddressHistorys();


                                if (ibusPersonAddress.ibusMainParticipantAddress.IsNotNull())
                                {
                                    foreach (busPersonAddressHistory add in ibusPersonAddress.ibusMainParticipantAddress.iclbPersonAddressHistory)
                                    {
                                        add.iclcPersonAddressChklistHistory = new utlCollection<cdoPersonAddressChklistHistory>();
                                        add.LoadPersonAddressChklists();
                                        foreach (cdoPersonAddressChklistHistory item in add.iclcPersonAddressChklistHistory)
                                        {
                                            item.Delete();
                                        }
                                        add.icdoPersonAddressHistory.Delete();
                                    }
                                }


                                lclbusPersonAddressChklistOld = lbusPersonAddressChklist.LoadChecklistByAddressId(
                                                       ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.address_id);
                                if (lclbusPersonAddressChklistOld != null)
                                {
                                    foreach (busPersonAddressChklist lPersonAddressChklist in lclbusPersonAddressChklistOld)
                                    {
                                        lPersonAddressChklist.icdoPersonAddressChklist.Delete();
                                    }
                                }


                                ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.Delete();
                            }
                            #endregion

                        }
                        else
                        {
                            ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress = new cdoPersonAddress();
                            ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.person_id = ibusDuplicateRecord.icdoPerson.person_id;
                            ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_line_1 = ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_line_1;
                            ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_line_2 = ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_line_2;
                            ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_zip_code = ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_zip_code;
                            ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_state_id = ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_state_id;
                            ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_state_value = ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_state_value;
                            ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_city = ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_city;
                            ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_country_id = ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_country_id;
                            ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_country_value = ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_country_value;
                            ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_source_id = ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_source_id;
                            ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_source_value = ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_source_value;
                            ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.start_date = ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.start_date;
                            ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.Insert();

                            ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.ibusPersonAddressChklist =
                                                            new busPersonAddressChklist { icdoPersonAddressChklist = new cdoPersonAddressChklist() };

                            busPersonAddressChklist lbusPersonAddressChklist = new busPersonAddressChklist();
                            Collection<busPersonAddressChklist> lclbusPersonAddressChklist = new Collection<busPersonAddressChklist>();
                            lclbusPersonAddressChklist = lbusPersonAddressChklist.LoadChecklistByAddressId(
                                                    iclbOldPersonToBeMerged[0].ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.address_id);

                            foreach (busPersonAddressChklist lPersonAddressChklist in lclbusPersonAddressChklist)
                            {
                                lbusPersonAddressChklist.InsertDataInPersonAddressChecklist(
                                    ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.address_id,
                                    lPersonAddressChklist.icdoPersonAddressChklist.address_type_value);
                            }
                        }

                    }

                }
            }

        }

        public ArrayList btnMergedPersonPreview()
        {
            ArrayList larr = new ArrayList();
            utlError lobjError = null;
            busPerson lbusMergedPersonRecord = new busPerson { icdoPerson = new cdoPerson() };
            if (this.iclbMergedPersonReord.Count > 0)
                this.iclbMergedPersonReord.Clear();

            #region Hard Errors

            if (iclbNewPerson[0].IsToSSNChecked == busConstant.FLAG_YES && iclbNewPerson[0].icdoPerson.istrSSNNonEncrypted.IsNullOrEmpty())
            {
                lobjError = AddError(1109, "");
                larr.Add(lobjError);
                return larr;

            }
            if (iclbOldPersonToBeMerged[0].IsFromSSNChecked == null)
                iclbOldPersonToBeMerged[0].IsFromSSNChecked = busConstant.FLAG_NO;
            if (iclbOldPersonToBeMerged[0].IsFromSSNChecked == busConstant.FLAG_YES && iclbOldPersonToBeMerged[0].icdoPerson.istrSSNNonEncrypted.IsNullOrEmpty())
            {
                lobjError = AddError(1109, "");
                larr.Add(lobjError);
                return larr;

            }


            if (iclbNewPerson[0].IsToMPIIDChecked == busConstant.FLAG_YES && iclbOldPersonToBeMerged[0].IsFromMPIIDChecked == busConstant.FLAG_YES)
            {
                lobjError = AddError(6122, "");
                larr.Add(lobjError);
                return larr;

            }
            if (iclbNewPerson[0].IsToMPIIDChecked == busConstant.FLAG_NO && iclbOldPersonToBeMerged[0].IsFromMPIIDChecked == busConstant.FLAG_NO)
            {
                lobjError = AddError(6147, " ");
                larr.Add(lobjError);
                return larr;

            }


            if (iclbNewPerson[0].IsToSSNChecked == busConstant.FLAG_YES && iclbOldPersonToBeMerged[0].IsFromSSNChecked == busConstant.FLAG_YES)
            {
                lobjError = AddError(6123, "");
                larr.Add(lobjError);
                return larr;


            }

            if (iclbNewPerson[0].IsToSSNChecked == busConstant.FLAG_NO && iclbOldPersonToBeMerged[0].IsFromSSNChecked == busConstant.FLAG_NO)
            {

                lobjError = AddError(6148, " ");
                larr.Add(lobjError);
                return larr;

            }

            if (iclbOldPersonToBeMerged[0].IsFromLastNameChecked == null)
                iclbOldPersonToBeMerged[0].IsFromLastNameChecked = busConstant.FLAG_NO;
            if (iclbNewPerson[0].IsToLastNameChecked == busConstant.FLAG_YES && iclbOldPersonToBeMerged[0].IsFromLastNameChecked == busConstant.FLAG_YES)
            {

                lobjError = AddError(6124, "");
                larr.Add(lobjError);
                return larr;

            }


            if (iclbNewPerson[0].IsToLastNameChecked == busConstant.FLAG_NO && iclbOldPersonToBeMerged[0].IsFromLastNameChecked == busConstant.FLAG_NO)
            {

                lobjError = AddError(6149, " ");
                larr.Add(lobjError);
                return larr;

            }

            if (iclbOldPersonToBeMerged[0].IsFromFirstNameChecked == null)
                iclbOldPersonToBeMerged[0].IsFromFirstNameChecked = busConstant.FLAG_NO;
            if (iclbNewPerson[0].IsToFirstNameChecked == busConstant.FLAG_YES && iclbOldPersonToBeMerged[0].IsFromFirstNameChecked == busConstant.FLAG_YES)
            {

                lobjError = AddError(6125, "");
                larr.Add(lobjError);
                return larr;

            }

            if (iclbNewPerson[0].IsToFirstNameChecked == busConstant.FLAG_NO && iclbOldPersonToBeMerged[0].IsFromFirstNameChecked == busConstant.FLAG_NO)
            {

                lobjError = AddError(6150, " ");
                larr.Add(lobjError);
                return larr;

            }

            if (iclbOldPersonToBeMerged[0].IsFromMiddleNameChecked == null)
                iclbOldPersonToBeMerged[0].IsFromMiddleNameChecked = busConstant.FLAG_NO;
            if (iclbNewPerson[0].IsToMiddleNameChecked == busConstant.FLAG_YES && iclbOldPersonToBeMerged[0].IsFromMiddleNameChecked == busConstant.FLAG_YES)
            {
                lobjError = AddError(6126, "");
                larr.Add(lobjError);
                return larr;

            }
            if (iclbNewPerson[0].IsToMiddleNameChecked == busConstant.FLAG_NO && iclbOldPersonToBeMerged[0].IsFromMiddleNameChecked == busConstant.FLAG_NO)
            {
                lobjError = AddError(6151, " ");
                larr.Add(lobjError);
                return larr;

            }

            if (iclbOldPersonToBeMerged[0].IsFromPrefixNameChecked == null)
                iclbOldPersonToBeMerged[0].IsFromPrefixNameChecked = busConstant.FLAG_NO;
            if (iclbNewPerson[0].IsToPrefixNameChecked == busConstant.FLAG_YES && iclbOldPersonToBeMerged[0].IsFromPrefixNameChecked == busConstant.FLAG_YES)
            {
                lobjError = AddError(6127, "");
                larr.Add(lobjError);
                return larr;

            }
            if (iclbNewPerson[0].IsToPrefixNameChecked == busConstant.FLAG_NO && iclbOldPersonToBeMerged[0].IsFromPrefixNameChecked == busConstant.FLAG_NO)
            {
                lobjError = AddError(6152, " ");
                larr.Add(lobjError);
                return larr;

            }

            if (iclbOldPersonToBeMerged[0].IsFromSuffixNameChecked == null)
                iclbOldPersonToBeMerged[0].IsFromSuffixNameChecked = busConstant.FLAG_NO;
            if (iclbNewPerson[0].IsToSuffixNameChecked == busConstant.FLAG_YES && iclbOldPersonToBeMerged[0].IsFromSuffixNameChecked == busConstant.FLAG_YES)
            {
                lobjError = AddError(6128, "");
                larr.Add(lobjError);
                return larr;

            }

            if (iclbNewPerson[0].IsToSuffixNameChecked == busConstant.FLAG_NO && iclbOldPersonToBeMerged[0].IsFromSuffixNameChecked == busConstant.FLAG_NO)
            {
                lobjError = AddError(6153, " ");
                larr.Add(lobjError);
                return larr;
            }

            if (iclbOldPersonToBeMerged[0].IsFromDOBChecked == null)
                iclbOldPersonToBeMerged[0].IsFromDOBChecked = busConstant.FLAG_NO;
            if (iclbNewPerson[0].IsToDOBChecked == busConstant.FLAG_YES && iclbOldPersonToBeMerged[0].IsFromDOBChecked == busConstant.FLAG_YES)
            {

                lobjError = AddError(6129, "");
                larr.Add(lobjError);
                return larr;
            }
            if (iclbNewPerson[0].IsToDOBChecked == busConstant.FLAG_NO && iclbOldPersonToBeMerged[0].IsFromDOBChecked == busConstant.FLAG_NO)
            {
                lobjError = AddError(6154, " ");
                larr.Add(lobjError);
                return larr;

            }

            if (iclbOldPersonToBeMerged[0].IsFromDODChecked == null)
                iclbOldPersonToBeMerged[0].IsFromDODChecked = busConstant.FLAG_NO;
            if (iclbNewPerson[0].IsToDODChecked == busConstant.FLAG_YES && iclbOldPersonToBeMerged[0].IsFromDODChecked == busConstant.FLAG_YES)
            {
                lobjError = AddError(6130, "");
                larr.Add(lobjError);
                return larr;

            }
            if (iclbNewPerson[0].IsToDODChecked == busConstant.FLAG_NO && iclbOldPersonToBeMerged[0].IsFromDODChecked == busConstant.FLAG_NO)
            {
                lobjError = AddError(6155, " ");
                larr.Add(lobjError);
                return larr;

            }

            if (iclbOldPersonToBeMerged[0].IsFromAddressChecked == null)
                iclbOldPersonToBeMerged[0].IsFromAddressChecked = busConstant.FLAG_NO;
            if (iclbNewPerson[0].IsToAddressChecked == busConstant.FLAG_YES && iclbOldPersonToBeMerged[0].IsFromAddressChecked == busConstant.FLAG_YES)
            {
                lobjError = AddError(6131, "");
                larr.Add(lobjError);
                return larr;

            }
            if (iclbNewPerson[0].IsToAddressChecked == busConstant.FLAG_NO && iclbOldPersonToBeMerged[0].IsFromAddressChecked == busConstant.FLAG_NO)
            {
                lobjError = AddError(6156, " ");
                larr.Add(lobjError);
                return larr;


            }
            #endregion

            if ((iclbNewPerson != null && iclbNewPerson.Count > 0 && iclbNewPerson[0] != null) && (iclbOldPersonToBeMerged != null && iclbOldPersonToBeMerged.Count > 0 && iclbOldPersonToBeMerged[0] != null))
            {
                lbusMergedPersonRecord.icdoPerson.mpi_person_id = iclbNewPerson[0].icdoPerson.mpi_person_id;

                if (iclbNewPerson[0].IsToSSNChecked.IsNotNull() && iclbNewPerson[0].IsToSSNChecked == busConstant.FLAG_YES)
                    lbusMergedPersonRecord.icdoPerson.istrSSNNonEncrypted = iclbNewPerson[0].icdoPerson.istrSSNNonEncrypted;

                else
                    lbusMergedPersonRecord.icdoPerson.istrSSNNonEncrypted = iclbOldPersonToBeMerged[0].icdoPerson.istrSSNNonEncrypted;


                if (iclbNewPerson[0].IsToLastNameChecked.IsNotNull() && iclbNewPerson[0].IsToLastNameChecked == busConstant.FLAG_YES)
                    lbusMergedPersonRecord.icdoPerson.last_name = iclbNewPerson[0].icdoPerson.last_name;
                else
                    lbusMergedPersonRecord.icdoPerson.last_name = iclbOldPersonToBeMerged[0].icdoPerson.last_name;


                if (iclbNewPerson[0].IsToFirstNameChecked.IsNotNull() && iclbNewPerson[0].IsToFirstNameChecked == busConstant.FLAG_YES)
                    lbusMergedPersonRecord.icdoPerson.first_name = iclbNewPerson[0].icdoPerson.first_name;
                else
                    lbusMergedPersonRecord.icdoPerson.first_name = iclbOldPersonToBeMerged[0].icdoPerson.first_name;


                if (iclbNewPerson[0].IsToMiddleNameChecked.IsNotNull() && iclbNewPerson[0].IsToMiddleNameChecked == busConstant.FLAG_YES)
                    lbusMergedPersonRecord.icdoPerson.middle_name = iclbNewPerson[0].icdoPerson.middle_name;
                else
                    lbusMergedPersonRecord.icdoPerson.middle_name = iclbOldPersonToBeMerged[0].icdoPerson.middle_name;


                if (iclbNewPerson[0].IsToPrefixNameChecked.IsNotNull() && iclbNewPerson[0].IsToPrefixNameChecked == busConstant.FLAG_YES)
                {
                    lbusMergedPersonRecord.icdoPerson.name_prefix_value = iclbNewPerson[0].icdoPerson.name_prefix_value;
                    lbusMergedPersonRecord.icdoPerson.name_prefix_description = iclbNewPerson[0].icdoPerson.name_prefix_description;
                }
                else
                {
                    lbusMergedPersonRecord.icdoPerson.name_prefix_value = iclbOldPersonToBeMerged[0].icdoPerson.name_prefix_value;
                    lbusMergedPersonRecord.icdoPerson.name_prefix_description = iclbOldPersonToBeMerged[0].icdoPerson.name_prefix_description;
                }


                if (iclbNewPerson[0].IsToSuffixNameChecked.IsNotNull() && iclbNewPerson[0].IsToSuffixNameChecked == busConstant.FLAG_YES)
                    lbusMergedPersonRecord.icdoPerson.name_suffix = iclbNewPerson[0].icdoPerson.name_suffix;
                else
                    lbusMergedPersonRecord.icdoPerson.name_suffix = iclbOldPersonToBeMerged[0].icdoPerson.name_suffix;


                if (iclbNewPerson[0].IsToDOBChecked.IsNotNull() && iclbNewPerson[0].IsToDOBChecked == busConstant.FLAG_YES)
                {
                    lbusMergedPersonRecord.icdoPerson.idtDateofBirth = iclbNewPerson[0].icdoPerson.idtDateofBirth;

                }
                else
                {
                    lbusMergedPersonRecord.icdoPerson.idtDateofBirth = iclbOldPersonToBeMerged[0].icdoPerson.idtDateofBirth;

                }

                if (iclbNewPerson[0].IsToDODChecked.IsNotNull() && iclbNewPerson[0].IsToDODChecked == busConstant.FLAG_YES)
                    lbusMergedPersonRecord.icdoPerson.date_of_death = iclbNewPerson[0].icdoPerson.date_of_death;
                else
                    lbusMergedPersonRecord.icdoPerson.date_of_death = iclbOldPersonToBeMerged[0].icdoPerson.date_of_death;


                if (iclbNewPerson[0].IsToAddressChecked.IsNotNull() && iclbNewPerson[0].IsToAddressChecked == busConstant.FLAG_YES && iclbNewPerson[0].istrPersonAddress != null)
                {
                    lbusMergedPersonRecord.istrPersonAddress = iclbNewPerson[0].istrPersonAddress;
                }
                else
                {
                    lbusMergedPersonRecord.istrPersonAddress = iclbOldPersonToBeMerged[0].istrPersonAddress;
                }



            }


            lbusMergedPersonRecord.idecEEContributionAmount = iclbNewPerson[0].idecEEContributionAmount + iclbOldPersonToBeMerged[0].idecEEContributionAmount;
            lbusMergedPersonRecord.idecEEContributionInterest = iclbNewPerson[0].idecEEContributionInterest + iclbOldPersonToBeMerged[0].idecEEContributionInterest;
            lbusMergedPersonRecord.idecUVHPContributionAmount = iclbNewPerson[0].idecUVHPContributionAmount + iclbOldPersonToBeMerged[0].idecUVHPContributionAmount;
            lbusMergedPersonRecord.idecUVHPContributionInterest = iclbNewPerson[0].idecUVHPContributionInterest + iclbOldPersonToBeMerged[0].idecUVHPContributionInterest;
            lbusMergedPersonRecord.idecIAPContributionAmount = iclbNewPerson[0].idecIAPContributionAmount;

            iclbMergedPersonReord.Add(lbusMergedPersonRecord);
            larr.Add(this);

            //  EvaluateInitialLoadRules();
            return larr;
        }

        private void MergeBridgedHours()
        {
            busPersonBridgeHours lbusPersonBridgeHours = new busPersonBridgeHours();

            DataTable ldtbPersonBridgeHoursDetailOld = lbusPersonBridgeHours.LoadBridgeHoursDetailsByPersonId(iclbOldPersonToBeMerged[0].icdoPerson.person_id);
            DataTable ldtbPersonBridgeHoursDetailNew = lbusPersonBridgeHours.LoadBridgeHoursDetailsByPersonId(iclbNewPerson[0].icdoPerson.person_id);

            decimal ldecTotalHoursAdded = 0;
            ArrayList larrDataEnteredInHoursTable = new ArrayList();
            busPersonBridgeHours lbusPersonBridgeHoursNew = new busPersonBridgeHours();

            foreach (DataRow ldrOldData in ldtbPersonBridgeHoursDetailOld.Rows)
            {
                #region If there is no data for bridge type and computation year then make entry in both the tables

                if (ldtbPersonBridgeHoursDetailNew.AsEnumerable().Where(
                    item => item.Field<string>(enmPersonBridgeHours.bridge_type_value.ToString()) == Convert.ToString(ldrOldData[enmPersonBridgeHours.bridge_type_value.ToString()])).Count() == 0)
                {
                    if (!larrDataEnteredInHoursTable.Contains(Convert.ToInt32(ldrOldData[enmPersonBridgeHours.person_bridge_id.ToString()])))
                    {
                        DateTime ldtEndDate = new DateTime();
                        DateTime ldtToDate = new DateTime();

                        if (Convert.ToString(ldrOldData[enmPersonBridgeHours.bridge_end_date.ToString()]).IsNotNullOrEmpty())
                        {
                            ldtEndDate = Convert.ToDateTime(ldrOldData[enmPersonBridgeHours.bridge_end_date.ToString()]);
                        }

                        larrDataEnteredInHoursTable.Add(Convert.ToInt32(ldrOldData[enmPersonBridgeHours.person_bridge_id.ToString()]));
                        lbusPersonBridgeHoursNew = new busPersonBridgeHours() { icdoPersonBridgeHours = new cdoPersonBridgeHours() };
                        //        if(ldrOldData[enmPersonBridgeHours.hours_reported.ToString()]!=DBNull.Value )
                        lbusPersonBridgeHoursNew.InsertPersonBridgeHours(iclbNewPerson[0].icdoPerson.person_id, Convert.ToString(ldrOldData[enmPersonBridgeHours.bridge_type_value.ToString()]),
                                   Convert.ToDecimal(ldrOldData[enmPersonBridgeHours.hours_reported.ToString()]), Convert.ToDateTime(ldrOldData[enmPersonBridgeHours.bridge_start_date.ToString()]),
                                   ldtEndDate);


                        DataRow[] ldrRecordForSameBridgeValuesandCompYear = ldtbPersonBridgeHoursDetailOld.FilterTable(utlDataType.Numeric, enmPersonBridgeHoursDetail.person_bridge_id.ToString(),
                                                                            Convert.ToInt32(ldrOldData[enmPersonBridgeHoursDetail.person_bridge_id.ToString()]));

                        foreach (DataRow ldr in ldrRecordForSameBridgeValuesandCompYear)
                        {
                            if (Convert.ToString(ldr[enmPersonBridgeHoursDetail.to_date.ToString()]).IsNotNullOrEmpty())
                            {
                                ldtToDate = Convert.ToDateTime(ldr[enmPersonBridgeHoursDetail.to_date.ToString()]);
                            }

                            busPersonBridgeHoursDetail lbusPersonBridgeHoursDetail = new busPersonBridgeHoursDetail() { icdoPersonBridgeHoursDetail = new cdoPersonBridgeHoursDetail() };
                            lbusPersonBridgeHoursDetail.InsertDataInPersonBridgedTable(lbusPersonBridgeHoursNew.icdoPersonBridgeHours.person_bridge_id,
                             Convert.ToInt32(ldr[enmPersonBridgeHoursDetail.computation_year.ToString()]), Convert.ToDecimal(ldr[enmPersonBridgeHoursDetail.hours.ToString()]),
                             Convert.ToDateTime(ldr[enmPersonBridgeHoursDetail.from_date.ToString()]), ldtToDate);
                        }
                    }
                }

                #endregion

                #region If bridge type match but year dosen't match

                else if (ldtbPersonBridgeHoursDetailNew.AsEnumerable().Where(item => item.Field<int>(enmPersonBridgeHoursDetail.computation_year.ToString()) != Convert.ToInt32(ldrOldData[enmPersonBridgeHoursDetail.computation_year.ToString()]) &&
                    item.Field<string>(enmPersonBridgeHours.bridge_type_value.ToString()) == Convert.ToString(ldrOldData[enmPersonBridgeHours.bridge_type_value.ToString()])).Count() > 0)
                {
                    if (!larrDataEnteredInHoursTable.Contains(Convert.ToInt32(ldrOldData[enmPersonBridgeHours.person_bridge_id.ToString()])))
                    {
                        DateTime ldtEndDate = new DateTime();

                        if (Convert.ToString(ldrOldData[enmPersonBridgeHours.bridge_end_date.ToString()]).IsNotNullOrEmpty())
                        {
                            ldtEndDate = Convert.ToDateTime(ldrOldData[enmPersonBridgeHours.bridge_end_date.ToString()]);
                        }
                        //if (Convert.ToString(ldrOldData[enmPersonBridgeHoursDetail.to_date.ToString()]).IsNotNullOrEmpty())
                        //{
                        //    ldtToDate = Convert.ToDateTime(ldrOldData[enmPersonBridgeHoursDetail.to_date.ToString()]);
                        //}


                        DataRow[] ldrRecordForSameBridgeValuesandCompYear = ldtbPersonBridgeHoursDetailOld.FilterTable(utlDataType.Numeric, enmPersonBridgeHoursDetail.person_bridge_id.ToString(),
                                                                              Convert.ToInt32(ldrOldData[enmPersonBridgeHoursDetail.person_bridge_id.ToString()]));
                        DataRow[] ldrRecordForSameBridgeValuesForNewPerson = ldtbPersonBridgeHoursDetailNew.FilterTable(utlDataType.String, enmPersonBridgeHours.bridge_type_value.ToString(),
                                                                             Convert.ToString(ldrOldData[enmPersonBridgeHours.bridge_type_value.ToString()]));

                        foreach (DataRow ldr in ldrRecordForSameBridgeValuesandCompYear)
                        {
                            DateTime ldtToDate = new DateTime();
                            if (Convert.ToString(ldr[enmPersonBridgeHoursDetail.to_date.ToString()]).IsNotNullOrEmpty())
                            {
                                ldtToDate = Convert.ToDateTime(ldr[enmPersonBridgeHoursDetail.to_date.ToString()]);
                            }

                            busPersonBridgeHoursDetail lbusPersonBridgeHoursDetail = new busPersonBridgeHoursDetail() { icdoPersonBridgeHoursDetail = new cdoPersonBridgeHoursDetail() };
                            lbusPersonBridgeHoursDetail.InsertDataInPersonBridgedTable(Convert.ToInt32(ldrRecordForSameBridgeValuesForNewPerson[0][enmPersonBridgeHours.person_bridge_id.ToString()]),
                             Convert.ToInt32(ldr[enmPersonBridgeHoursDetail.computation_year.ToString()]), Convert.ToDecimal(ldr[enmPersonBridgeHoursDetail.hours.ToString()]),
                             Convert.ToDateTime(ldr[enmPersonBridgeHoursDetail.from_date.ToString()]), ldtToDate);
                            ldecTotalHoursAdded += Convert.ToDecimal(ldr[enmPersonBridgeHoursDetail.hours.ToString()]);
                        }

                        lbusPersonBridgeHours.icdoPersonBridgeHours = new cdoPersonBridgeHours();
                        lbusPersonBridgeHours.icdoPersonBridgeHours.LoadData(ldrRecordForSameBridgeValuesForNewPerson[0]);
                        lbusPersonBridgeHours.icdoPersonBridgeHours.hours_reported = lbusPersonBridgeHours.icdoPersonBridgeHours.hours_reported + ldecTotalHoursAdded;
                        lbusPersonBridgeHours.icdoPersonBridgeHours.Update();
                        larrDataEnteredInHoursTable.Add(Convert.ToInt32(ldrOldData[enmPersonBridgeHours.person_bridge_id.ToString()]));
                    }
                }

                #endregion
            }

        }

        public bool CheckIsPersonMerged()
        {
            if (iclbOldPersonToBeMerged != null && iclbOldPersonToBeMerged.Count > 0)
            {
                DataTable ldtbResult = Select("cdoSsnMergeHistory.GetOldPersonByMPID", new object[1] { iclbOldPersonToBeMerged[0].icdoPerson.mpi_person_id });
                //  DataTable ldtbResult = Select("cdoSsnMergeHistory.GetOldPersonBySSN", new object[1] { iclbOldPersonToBeMerged[0].icdoPerson.istrSSNNonEncrypted });
                if (ldtbResult.Rows.Count == 0)
                    return false;
            }

            return true;
        }

        private void SetParametersForIAPAllocationMerging(DataTable adtWorkHistory, Collection<cdoDummyWorkData> aclbPersonWorkHistory_IAP,
                                                        int aintPersonAccountID, int aintOldPersonAccountId)
        {
            #region CheckIfThePersonIsRetiree
            //Prod PIR 261
            DateTime ldtRetirementDate = new DateTime();
            DateTime ldtAwardedOnDate = new DateTime();
            DateTime ldtRetEffectiveDate = new DateTime();
            iclbPayeeAccounts = new Collection<busPayeeAccount>();
            DataTable ldtblistPayeeAcnt = busPerson.Select("cdoBenefitApplication.CheckIfParticipantIsRetired", new object[1] { this.iclbNewPerson.FirstOrDefault().icdoPerson.person_id });
            if (ldtblistPayeeAcnt.IsNotNull() && ldtblistPayeeAcnt.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(ldtblistPayeeAcnt.Rows[0][enmBenefitApplication.retirement_date.ToString()])))
                {
                    ldtRetirementDate = Convert.ToDateTime(ldtblistPayeeAcnt.Rows[0][enmBenefitApplication.retirement_date.ToString()]);
                    ldtRetEffectiveDate = ldtRetirementDate;
                }
                if (!string.IsNullOrEmpty(Convert.ToString(ldtblistPayeeAcnt.Rows[0][enmBenefitApplication.awarded_on_date.ToString()])))
                {
                    ldtAwardedOnDate = Convert.ToDateTime(ldtblistPayeeAcnt.Rows[0][enmBenefitApplication.awarded_on_date.ToString()]);
                    if (ldtAwardedOnDate > ldtRetirementDate)
                    {
                        ldtRetEffectiveDate = ldtAwardedOnDate;
                    }
                }

            }
            #endregion

            #region Calculate IAP Allocation start date

            int lintIAPAllocationStartYear = 0, lintLocal52AllocationStartYear = 0, lintLocal161AllocationStartYear = 0;
            decimal ldecPreviousYearIAPBalance = 0, ldecPreviousYearLocal52Balance = 0, ldecPreviousYearLocal161Balance = 0;

            DataTable ldtbGetContributionsofOldPersonByPlanYear = Select("cdoPersonAccountRetirementContribution.LoadIAPContributionforsinglePerson", new object[1] { aintOldPersonAccountId });

            DataTable ldtbGetContributionforOldPerson = Select("cdoPersonAccountRetirementContribution.GetIAPContributionStartYear",
                                            new object[2] { iclbOldPersonToBeMerged[0].icdoPerson.person_id, busConstant.IAP_PLAN_ID });

            DataTable ldtbGetContributionforNewPerson = Select("cdoPersonAccountRetirementContribution.GetIAPContributionStartYear",
                                           new object[2] { iclbNewPerson[0].icdoPerson.person_id, busConstant.IAP_PLAN_ID });

            #region Calculate IAP Allocation End Date

            int lintIAPAllocationMergeEndYear = 0;
            LoadPreviousYearAllocationSummary();

            if (ibusPrevYearAllocationSummary.icdoIapAllocationSummary != null)
            {
                lintIAPAllocationMergeEndYear = ibusPrevYearAllocationSummary.icdoIapAllocationSummary.computation_year;
            }

            #endregion

            #region Calculate Thru79 hours

            busIapAllocationSummary lbusIapAllocationSummary = new busIapAllocationSummary();
            decimal ldecThru79Hours = 0;



            //if (aclbPersonWorkHistory_IAP != null && aclbPersonWorkHistory_IAP.Count() > 0
            //    && aclbPersonWorkHistory_IAP.Where(t => t.year <= 1979).Count() > 0)
            //{
            //    ldecThru79Hours = aclbPersonWorkHistory_IAP.Where(t => t.year <= 1979).Sum(t => t.qualified_hours);
            //}


            #region IF participant was on BIS as of 1979, then prior hours should not be counted towards Thru1979

            //Remove history for any forfieture year 1979
            if (aclbPersonWorkHistory_IAP != null && aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979).Count() > 0)
            {
                if (aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979 && o.istrForfietureFlag == busConstant.FLAG_YES).Count() > 0)
                {
                    int lintMaxForfietureYearBefore1979 = aclbPersonWorkHistory_IAP.Where(item => item.year <= busConstant.BenefitCalculation.YEAR_1979 && item.istrForfietureFlag == busConstant.FLAG_YES).Max(t => t.year);
                    aclbPersonWorkHistory_IAP = aclbPersonWorkHistory_IAP.Where(item => item.year > lintMaxForfietureYearBefore1979).ToList().ToCollection();
                }
            }

            if (aclbPersonWorkHistory_IAP != null && aclbPersonWorkHistory_IAP.Where(o => o.year == busConstant.BenefitCalculation.YEAR_1979).Count() > 0)
            {
                decimal ldecPreviousYearPaidIAPAccountBalance = 0M;
                cdoDummyWorkData lcdoWorkData1979 = aclbPersonWorkHistory_IAP.Where(o => o.year == busConstant.BenefitCalculation.YEAR_1979).FirstOrDefault();
                //IF participant was on BIS as of 1979, then prior hours should not be counted towards Thru1979
                if (lcdoWorkData1979 != null && lcdoWorkData1979.bis_years_count < 2)
                {
                    int lintPaymentYear = 0;
                    DataTable ldtblPaymentYear = busBase.Select("cdoPersonAccountRetirementContribution.GetMaxPaymentYearOnOrBefore1979", new object[1] { iclbMergedPersonReord[0].icdoPerson.person_id });
                    if (ldtblPaymentYear != null && ldtblPaymentYear.Rows.Count > 0 && Convert.ToString(ldtblPaymentYear.Rows[0][0]).IsNotNullOrEmpty())
                    {
                        lintPaymentYear = Convert.ToInt32(ldtblPaymentYear.Rows[0][0]);
                    }
                    if (lintPaymentYear == 0)
                    {

                        ldecThru79Hours = aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979).Sum(o => o.qualified_hours);

                    }
                    else
                    {
                        if (aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979 && o.year > lintPaymentYear).Count() > 0)
                        {
                            ldecThru79Hours = aclbPersonWorkHistory_IAP.Where(o => o.year <= busConstant.BenefitCalculation.YEAR_1979 && o.year > lintPaymentYear).Sum(o => o.qualified_hours);
                        }
                    }

                    ldecThru79Hours += ldecPreviousYearPaidIAPAccountBalance;
                    if (ldecThru79Hours < 0)
                        ldecThru79Hours = 0;
                }
            }

            if (aclbPersonWorkHistory_IAP != null && aclbPersonWorkHistory_IAP.Where(o => o.year >= busConstant.BenefitCalculation.YEAR_1979).Count() > 0)
            {
                aclbPersonWorkHistory_IAP = aclbPersonWorkHistory_IAP.Where(o => o.year >= busConstant.BenefitCalculation.YEAR_1979).ToList().ToCollection();
            }

            #endregion


            //foreach (DataRow ldr in adtWorkHistory.Rows)
            //{
            //    if (Convert.ToInt32(ldr["ComputationYear"]) <= 1979)
            //    {
            //        ldecThru79Hours += (ldr["iaphoursa2"] == DBNull.Value ? 0.0M : Convert.ToDecimal(ldr["iaphoursa2"]));

            //        if (Convert.ToInt32(ldr["ComputationYear"]) == 1979)
            //        {
            //            break;
            //        }
            //    }
            //}

            #endregion

            int lintOldPersonIAPContriStartDate = 0, lintNewPersonIAPContriStartDate = 0, lintOldPersonLocal52ContriStartDate = 0, lintNewPersonLocal52ContriStartDate = 0,
                   lintOldPersonLocal161ContriStartDate = 0, lintNewPersonLocal161ContriStartDate = 0;

            if (ldtbGetContributionforOldPerson.Rows.Count > 0)
            {
                if (ldtbGetContributionforOldPerson.AsEnumerable().Where(item => item.Field<decimal>(enmPersonAccountRetirementContribution.iap_balance_amount.ToString()) > 0).Count() > 0)
                {
                    lintOldPersonIAPContriStartDate = Convert.ToInt32(ldtbGetContributionforOldPerson.AsEnumerable().Where(item => item.Field<decimal>(enmPersonAccountRetirementContribution.iap_balance_amount.ToString()) > 0).
                                               FirstOrDefault().Field<decimal>(enmPersonAccountRetirementContribution.computational_year.ToString()));
                }

                if (ldtbGetContributionforOldPerson.AsEnumerable().Where(item => item.Field<decimal>(enmPersonAccountRetirementContribution.local52_special_acct_bal_amount.ToString()) > 0).Count() > 0)
                {
                    lintOldPersonLocal52ContriStartDate = Convert.ToInt32(ldtbGetContributionforOldPerson.AsEnumerable().Where(item => item.Field<decimal>(enmPersonAccountRetirementContribution.local52_special_acct_bal_amount.ToString()) > 0).
                                             FirstOrDefault().Field<decimal>(enmPersonAccountRetirementContribution.computational_year.ToString()));
                }

                if (ldtbGetContributionforOldPerson.AsEnumerable().Where(item => item.Field<decimal>(enmPersonAccountRetirementContribution.local161_special_acct_bal_amount.ToString()) > 0).Count() > 0)
                {
                    lintOldPersonLocal161ContriStartDate = Convert.ToInt32(ldtbGetContributionforOldPerson.AsEnumerable().Where(item => item.Field<decimal>(enmPersonAccountRetirementContribution.local161_special_acct_bal_amount.ToString()) > 0).
                                            First().Field<decimal>(enmPersonAccountRetirementContribution.computational_year.ToString()));
                }
            }

            if (ldtbGetContributionforNewPerson.Rows.Count > 0)
            {
                if (ldtbGetContributionforNewPerson.AsEnumerable().Where(item => item.Field<decimal>(enmPersonAccountRetirementContribution.iap_balance_amount.ToString()) > 0).Count() > 0)
                {
                    lintNewPersonIAPContriStartDate = Convert.ToInt32(ldtbGetContributionforNewPerson.AsEnumerable().Where(item => item.Field<decimal>(enmPersonAccountRetirementContribution.iap_balance_amount.ToString()) > 0).
                                            FirstOrDefault().Field<decimal>(enmPersonAccountRetirementContribution.computational_year.ToString()));
                }

                if (ldtbGetContributionforNewPerson.AsEnumerable().Where(item => item.Field<decimal>(enmPersonAccountRetirementContribution.local52_special_acct_bal_amount.ToString()) > 0).Count() > 0)
                {
                    lintNewPersonLocal52ContriStartDate = Convert.ToInt32(ldtbGetContributionforNewPerson.AsEnumerable().Where(item => item.Field<decimal>(enmPersonAccountRetirementContribution.local52_special_acct_bal_amount.ToString()) > 0).
                                            First().Field<decimal>(enmPersonAccountRetirementContribution.computational_year.ToString()));
                }

                if (ldtbGetContributionforNewPerson.AsEnumerable().Where(item => item.Field<decimal>(enmPersonAccountRetirementContribution.local161_special_acct_bal_amount.ToString()) > 0).Count() > 0)
                {
                    lintNewPersonLocal161ContriStartDate = Convert.ToInt32(ldtbGetContributionforNewPerson.AsEnumerable().Where(item => item.Field<decimal>(enmPersonAccountRetirementContribution.local161_special_acct_bal_amount.ToString()) > 0).
                                            First().Field<decimal>(enmPersonAccountRetirementContribution.computational_year.ToString()));
                }
            }

            #region Merge IAP Allocations

            if ((lintNewPersonIAPContriStartDate != 0 && lintOldPersonIAPContriStartDate != 0 && lintNewPersonIAPContriStartDate <= lintOldPersonIAPContriStartDate)
                || lintOldPersonIAPContriStartDate == 0)
            {
                //lintIAPAllocationStartYear = lintOldPersonIAPContriStartDate;
                //ldecPreviousYearIAPBalance = ldtbGetContributionforNewPerson.AsEnumerable().Where(
                //    item => item.Field<decimal>(enmPersonAccountRetirementContribution.computational_year.ToString()) <= lintIAPAllocationStartYear - 1).Sum(
                //    item => item.Field<decimal>(enmPersonAccountRetirementContribution.iap_balance_amount.ToString())); 

                lintIAPAllocationStartYear = lintNewPersonIAPContriStartDate;
                ldecPreviousYearIAPBalance = ldtbGetContributionforNewPerson.AsEnumerable().Where(
                    item => item.Field<decimal>(enmPersonAccountRetirementContribution.computational_year.ToString()) <= lintIAPAllocationStartYear - 1).Sum(
                    item => item.Field<decimal>(enmPersonAccountRetirementContribution.iap_balance_amount.ToString()));
            }
            else if ((lintOldPersonIAPContriStartDate != 0 && lintNewPersonIAPContriStartDate != 0 && lintNewPersonIAPContriStartDate > lintOldPersonIAPContriStartDate)
                || lintNewPersonIAPContriStartDate == 0)
            {
                lintIAPAllocationStartYear = lintOldPersonIAPContriStartDate;
                ldecPreviousYearIAPBalance = ldtbGetContributionforOldPerson.AsEnumerable().Where(
                    item => item.Field<decimal>(enmPersonAccountRetirementContribution.computational_year.ToString()) <= lintIAPAllocationStartYear - 1).Sum(
                    item => item.Field<decimal>(enmPersonAccountRetirementContribution.iap_balance_amount.ToString()));

                //DataTable ldtbIAPContribution = new DataTable();

                //if (ldtbGetContributionsofOldPersonByPlanYear.AsEnumerable().Where(
                //                                     item => item.Field<decimal>(enmPersonAccountRetirementContribution.computational_year.ToString()) <= lintIAPAllocationStartYear - 1).Count() > 0)
                //{
                //    //ldtbIAPContribution = ldtbGetContributionsofOldPersonByPlanYear.AsEnumerable().Where(
                //    //                                 item => item.Field<decimal>(enmPersonAccountRetirementContribution.computational_year.ToString()) <= lintIAPAllocationStartYear - 1).CopyToDataTable();

                //    //foreach (DataRow ldr in ldtbIAPContribution.Rows)
                //    //{
                //    //    if (Convert.ToString(ldr[enmPersonAccountRetirementContribution.iap_balance_amount.ToString()]).IsNotNullOrEmpty() &&
                //    //            Convert.ToInt32(ldr[enmPersonAccountRetirementContribution.iap_balance_amount.ToString()]) != 0)
                //    //    {
                //    //        busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution();
                //    //        lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID,
                //    //                                                Convert.ToDateTime(ldr[enmPersonAccountRetirementContribution.effective_date.ToString()]),
                //    //                                                DateTime.Now, Convert.ToInt32(ldr[enmPersonAccountRetirementContribution.computational_year.ToString()]),
                //    //                                                Convert.ToDecimal(ldr[enmPersonAccountRetirementContribution.iap_balance_amount.ToString()]),
                //    //                                                0, 0, busConstant.TRANSACTION_TYPE_SSN_MERGE, 0, 0, 0, 0, 0, Convert.ToString(ldr[enmPersonAccountRetirementContribution.contribution_type_value.ToString()]),
                //    //                                                Convert.ToString(ldr[enmPersonAccountRetirementContribution.contribution_subtype_value.ToString()]), 0, 0, 0);
                //    //    }
                //    //}
                //}
            }

            //PIR-258-TusharT-Post all the RC entries of ols to new
            if (ldtbGetContributionsofOldPersonByPlanYear != null && ldtbGetContributionsofOldPersonByPlanYear.Rows.Count > 0)
            {
                foreach (DataRow ldr in ldtbGetContributionsofOldPersonByPlanYear.Rows)
                {
                    //For IAP Bal Amount
                    if (Convert.ToString(ldr[enmPersonAccountRetirementContribution.iap_balance_amount.ToString()]).IsNotNullOrEmpty())
                    //&& Convert.ToInt32(ldr[enmPersonAccountRetirementContribution.iap_balance_amount.ToString()]) != 0) //PIR 885
                    {
                        busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution();
                        lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID,
                                                                Convert.ToDateTime(ldr[enmPersonAccountRetirementContribution.effective_date.ToString()]),
                                                                Convert.ToDateTime(ldr[enmPersonAccountRetirementContribution.transaction_date.ToString()]),
                                                                Convert.ToInt32(ldr[enmPersonAccountRetirementContribution.computational_year.ToString()]),
                                                                Convert.ToDecimal(ldr[enmPersonAccountRetirementContribution.iap_balance_amount.ToString()]),
                                                                0, 0, busConstant.TRANSACTION_TYPE_SSN_ADJS, 0, 0, 0, 0, 0, Convert.ToString(ldr[enmPersonAccountRetirementContribution.contribution_type_value.ToString()]),
                                                                Convert.ToString(ldr[enmPersonAccountRetirementContribution.contribution_subtype_value.ToString()]), 0, 0, 0);
                    }
                    //For Loacal52
                    if (Convert.ToString(ldr[enmPersonAccountRetirementContribution.local52_special_acct_bal_amount.ToString()]).IsNotNullOrEmpty())
                    //&& Convert.ToInt32(ldr[enmPersonAccountRetirementContribution.local52_special_acct_bal_amount.ToString()]) != 0) //PIR 885
                    {
                        busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution();
                        lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID,
                                                                Convert.ToDateTime(ldr[enmPersonAccountRetirementContribution.effective_date.ToString()]),
                                                                Convert.ToDateTime(ldr[enmPersonAccountRetirementContribution.transaction_date.ToString()]),
                                                                Convert.ToInt32(ldr[enmPersonAccountRetirementContribution.computational_year.ToString()]), 0,
                                                                 Convert.ToDecimal(ldr[enmPersonAccountRetirementContribution.local52_special_acct_bal_amount.ToString()]), 0, busConstant.TRANSACTION_TYPE_SSN_ADJS,
                                                                 0, 0, 0, 0, 0, Convert.ToString(ldr[enmPersonAccountRetirementContribution.contribution_type_value.ToString()]),
                                                                Convert.ToString(ldr[enmPersonAccountRetirementContribution.contribution_subtype_value.ToString()]), 0, 0, 0);
                    }
                    //For Local161
                    if (Convert.ToString(ldr[enmPersonAccountRetirementContribution.local161_special_acct_bal_amount.ToString()]).IsNotNullOrEmpty())
                    //&& Convert.ToInt32(ldr[enmPersonAccountRetirementContribution.local161_special_acct_bal_amount.ToString()]) != 0)//PIR 885
                    {
                        busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution();
                        lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID,
                                                                Convert.ToDateTime(ldr[enmPersonAccountRetirementContribution.effective_date.ToString()]),
                                                                Convert.ToDateTime(ldr[enmPersonAccountRetirementContribution.transaction_date.ToString()]), Convert.ToInt32(ldr[enmPersonAccountRetirementContribution.computational_year.ToString()]), 0,
                                                                 0, Convert.ToDecimal(ldr[enmPersonAccountRetirementContribution.local161_special_acct_bal_amount.ToString()]),
                                                                 busConstant.TRANSACTION_TYPE_SSN_ADJS, 0, 0, 0, 0, 0,
                                                                 Convert.ToString(ldr[enmPersonAccountRetirementContribution.contribution_type_value.ToString()]),
                                                                Convert.ToString(ldr[enmPersonAccountRetirementContribution.contribution_subtype_value.ToString()]), 0, 0, 0);
                    }
                }
            }
            if (lintIAPAllocationStartYear != 0)
            {
                for (int i = lintIAPAllocationStartYear; i <= lintIAPAllocationMergeEndYear; i++)
                {
                    decimal ldecNewPreviousYearBalance = MergeIAPAllocations(adtWorkHistory, i, aintPersonAccountID, ldecThru79Hours, ldecPreviousYearIAPBalance,
                                          aclbPersonWorkHistory_IAP, lintIAPAllocationMergeEndYear, aintOldPersonAccountId, ldtRetEffectiveDate);

                    ldecPreviousYearIAPBalance = ldecNewPreviousYearBalance;
                }
            }

            #endregion

            #region Local52 special account merge logic

            if ((lintNewPersonLocal52ContriStartDate != 0 && lintOldPersonLocal52ContriStartDate != 0 && lintNewPersonLocal52ContriStartDate <= lintOldPersonLocal52ContriStartDate)
                || lintOldPersonLocal52ContriStartDate == 0)
            {
                lintLocal52AllocationStartYear = lintNewPersonLocal52ContriStartDate;
                ldecPreviousYearLocal52Balance = ldtbGetContributionforNewPerson.AsEnumerable().Where(
                    item => item.Field<decimal>(enmPersonAccountRetirementContribution.computational_year.ToString()) <= lintLocal52AllocationStartYear - 1).Sum(
                    item => item.Field<decimal>(enmPersonAccountRetirementContribution.local52_special_acct_bal_amount.ToString()));
            }
            else if ((lintOldPersonLocal52ContriStartDate != 0 && lintNewPersonLocal52ContriStartDate != 0 && lintNewPersonLocal52ContriStartDate > lintOldPersonLocal52ContriStartDate) ||
                       (lintNewPersonLocal52ContriStartDate == 0))
            {
                lintLocal52AllocationStartYear = lintOldPersonLocal52ContriStartDate;

                ldecPreviousYearLocal52Balance = ldtbGetContributionforOldPerson.AsEnumerable().Where(
                    item => item.Field<decimal>(enmPersonAccountRetirementContribution.computational_year.ToString()) <= lintLocal52AllocationStartYear - 1).Sum(
                    item => item.Field<decimal>(enmPersonAccountRetirementContribution.local52_special_acct_bal_amount.ToString()));

                //DataRow[] ldrIAPContribution = ldtbGetContributionsofOldPersonByPlanYear.FilterTable(utlDataType.Numeric, enmPersonAccountRetirementContribution.computational_year.ToString(), lintNewPersonLocal52ContriStartDate - 1);

                //foreach (DataRow ldr in ldrIAPContribution)
                //{
                //    if (Convert.ToString(ldr[enmPersonAccountRetirementContribution.local52_special_acct_bal_amount.ToString()]).IsNotNullOrEmpty() &&
                //            Convert.ToInt32(ldr[enmPersonAccountRetirementContribution.local52_special_acct_bal_amount.ToString()]) != 0)
                //    {
                //        busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution();
                //        lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID,
                //                                                Convert.ToDateTime(ldr[enmPersonAccountRetirementContribution.effective_date.ToString()]),
                //                                                DateTime.Now, Convert.ToInt32(ldr[enmPersonAccountRetirementContribution.computational_year.ToString()]), 0,
                //                                                 Convert.ToDecimal(ldr[enmPersonAccountRetirementContribution.local52_special_acct_bal_amount.ToString()]), 0, busConstant.TRANSACTION_TYPE_SSN_MERGE,
                //                                                 0, 0, 0, 0, 0, Convert.ToString(ldr[enmPersonAccountRetirementContribution.contribution_type_value.ToString()]),
                //                                                Convert.ToString(ldr[enmPersonAccountRetirementContribution.contribution_subtype_value.ToString()]), 0, 0, 0);
                //    }
                //}
            }

            if (lintLocal52AllocationStartYear != 0)
            {
                for (int i = lintLocal52AllocationStartYear; i <= lintIAPAllocationMergeEndYear; i++)
                {
                    decimal ldecNewPreviousYearBalance = MergeLocal52SpecialAccountBalance(i, ldecPreviousYearLocal52Balance, aintPersonAccountID);
                    ldecPreviousYearLocal52Balance = ldecNewPreviousYearBalance;
                }
            }

            #endregion

            #region Local161 special account merge logic

            if ((lintNewPersonLocal161ContriStartDate != 0 && lintOldPersonLocal161ContriStartDate != 0 && lintNewPersonLocal161ContriStartDate <= lintOldPersonLocal161ContriStartDate)
                || lintOldPersonLocal161ContriStartDate == 0)
            {
                lintLocal161AllocationStartYear = lintNewPersonLocal161ContriStartDate;
                ldecPreviousYearLocal161Balance = ldtbGetContributionforNewPerson.AsEnumerable().Where(
                   item => item.Field<decimal>(enmPersonAccountRetirementContribution.computational_year.ToString()) <= lintLocal161AllocationStartYear - 1).Sum(
                   item => item.Field<decimal>(enmPersonAccountRetirementContribution.local161_special_acct_bal_amount.ToString()));
            }
            else if ((lintNewPersonLocal161ContriStartDate != 0 && lintOldPersonLocal161ContriStartDate != 0 && lintNewPersonLocal161ContriStartDate > lintOldPersonLocal161ContriStartDate)
                || lintNewPersonLocal161ContriStartDate == 0)
            {
                lintLocal161AllocationStartYear = lintOldPersonLocal161ContriStartDate;
                ldecPreviousYearLocal161Balance = ldtbGetContributionforOldPerson.AsEnumerable().Where(
                   item => item.Field<decimal>(enmPersonAccountRetirementContribution.computational_year.ToString()) <= lintLocal161AllocationStartYear - 1).Sum(
                   item => item.Field<decimal>(enmPersonAccountRetirementContribution.local161_special_acct_bal_amount.ToString()));


                //DataRow[] ldrIAPContribution = ldtbGetContributionsofOldPersonByPlanYear.FilterTable(utlDataType.Numeric, enmPersonAccountRetirementContribution.computational_year.ToString(), lintNewPersonLocal52ContriStartDate - 1);

                //foreach (DataRow ldr in ldrIAPContribution)
                //{
                //    if (Convert.ToString(ldr[enmPersonAccountRetirementContribution.local161_special_acct_bal_amount.ToString()]).IsNotNullOrEmpty() &&
                //            Convert.ToInt32(ldr[enmPersonAccountRetirementContribution.local161_special_acct_bal_amount.ToString()]) != 0)
                //    {
                //        busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution();
                //        lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID,
                //                                                Convert.ToDateTime(ldr[enmPersonAccountRetirementContribution.effective_date.ToString()]),
                //                                                DateTime.Now, Convert.ToInt32(ldr[enmPersonAccountRetirementContribution.computational_year.ToString()]), 0,
                //                                                 0, Convert.ToDecimal(ldr[enmPersonAccountRetirementContribution.local161_special_acct_bal_amount.ToString()]),
                //                                                 busConstant.TRANSACTION_TYPE_SSN_MERGE, 0, 0, 0, 0, 0,
                //                                                 Convert.ToString(ldr[enmPersonAccountRetirementContribution.contribution_type_value.ToString()]),
                //                                                Convert.ToString(ldr[enmPersonAccountRetirementContribution.contribution_subtype_value.ToString()]), 0, 0, 0);
                //    }
                //}
            }

            if (lintLocal161AllocationStartYear != 0)
            {
                for (int i = lintLocal161AllocationStartYear; i <= lintIAPAllocationMergeEndYear; i++)
                {
                    decimal ldecNewPreviousYearBalance = MergeLocal161SpecialAccountBalance(i, ldecPreviousYearLocal161Balance, aintPersonAccountID);
                    ldecPreviousYearLocal161Balance = ldecNewPreviousYearBalance;
                }
            }
            // }

            #endregion

            #endregion

            //#region Calculate IAP Allocation End Date

            //int lintIAPAllocationMergeEndYear = 0;
            //LoadPreviousYearAllocationSummary();

            //if (ibusPrevYearAllocationSummary.icdoIapAllocationSummary != null)
            //{
            //    lintIAPAllocationMergeEndYear = ibusPrevYearAllocationSummary.icdoIapAllocationSummary.computation_year;
            //}

            //#endregion

            #region Do IAP allocation for new person

            //busIapAllocationSummary lbusIapAllocationSummary = new busIapAllocationSummary();
            //decimal ldecThru79Hours = 0;

            //foreach (DataRow ldr in adtWorkHistory.Rows)
            //{
            //    if (Convert.ToInt32(ldr["ComputationYear"]) <= 1979)
            //    {
            //        ldecThru79Hours += (ldr["iaphoursa2"] == DBNull.Value ? 0.0M : Convert.ToDecimal(ldr["iaphoursa2"]));

            //        if (Convert.ToInt32(ldr["ComputationYear"]) == 1979)
            //        {
            //            break;
            //        }
            //    }
            //}

            //if (lintIAPAllocationStartYear != 0)
            //{
            //    for (int i = lintIAPAllocationStartYear; i <= lintIAPAllocationMergeEndYear; i++)
            //    {
            //        decimal ldecNewPreviousYearBalance = MergeIAPAllocations(adtWorkHistory, i, aintPersonAccountID, ldecThru79Hours, ldecPreviousYearIAPBalance,
            //                              aclbPersonWorkHistory_IAP, lintIAPAllocationMergeEndYear, aintOldPersonAccountId);

            //        ldecPreviousYearIAPBalance = ldecNewPreviousYearBalance;
            //    }
            //}

            //if (lintLocal52AllocationStartYear != 0)
            //{
            //    for (int i = lintLocal52AllocationStartYear; i <= lintIAPAllocationMergeEndYear; i++)
            //    {
            //        decimal ldecNewPreviousYearBalance = MergeLocal52SpecialAccountBalance(i, ldecPreviousYearLocal52Balance, aintPersonAccountID);
            //        ldecPreviousYearLocal52Balance = ldecNewPreviousYearBalance;
            //    }
            //}

            //if (lintLocal161AllocationStartYear != 0)
            //{
            //    for (int i = lintLocal161AllocationStartYear; i <= lintIAPAllocationMergeEndYear; i++)
            //    {
            //        decimal ldecNewPreviousYearBalance = MergeLocal161SpecialAccountBalance(i, ldecPreviousYearLocal161Balance, aintPersonAccountID);
            //        ldecPreviousYearLocal161Balance = ldecNewPreviousYearBalance;
            //    }
            //}

            #endregion

            #region  set negative entries for old person

            //DataTable ldtbGetContributions = Select("cdoPersonAccountRetirementContribution.LoadIAPAllocationForPerson",
            //                                new object[2] { iclbOldPersonToBeMerged[0].icdoPerson.person_id, busConstant.IAP_PLAN_ID });

            foreach (DataRow ldrIAPContributions in ldtbGetContributionsofOldPersonByPlanYear.Rows)
            {
                DateTime ldtEffectiveDate = new DateTime();
                int lintComputationalYear = 0;
                string lstrContributionType = string.Empty, lstrContributionSubType = string.Empty;
                decimal ldecIAPBalalnce = 0, ldecLocal52Balance = 0, ldecLocal161Balance = 0;

                if (Convert.ToString(ldrIAPContributions[enmPersonAccountRetirementContribution.effective_date.ToString()]).IsNotNullOrEmpty())
                {
                    ldtEffectiveDate = Convert.ToDateTime(ldrIAPContributions[enmPersonAccountRetirementContribution.effective_date.ToString()]);
                }

                if (Convert.ToString(ldrIAPContributions[enmPersonAccountRetirementContribution.computational_year.ToString()]).IsNotNullOrEmpty())
                {
                    lintComputationalYear = Convert.ToInt32(ldrIAPContributions[enmPersonAccountRetirementContribution.computational_year.ToString()]);
                }

                if (Convert.ToString(ldrIAPContributions[enmPersonAccountRetirementContribution.contribution_type_value.ToString()]).IsNotNullOrEmpty())
                {
                    lstrContributionType = ldrIAPContributions[enmPersonAccountRetirementContribution.contribution_type_value.ToString()].ToString();
                }

                if (Convert.ToString(ldrIAPContributions[enmPersonAccountRetirementContribution.contribution_subtype_value.ToString()]).IsNotNullOrEmpty())
                {
                    lstrContributionSubType = ldrIAPContributions[enmPersonAccountRetirementContribution.contribution_subtype_value.ToString()].ToString();
                }

                if (Convert.ToString(ldrIAPContributions[enmPersonAccountRetirementContribution.iap_balance_amount.ToString()]).IsNotNullOrEmpty())
                {
                    ldecIAPBalalnce = Convert.ToDecimal(ldrIAPContributions[enmPersonAccountRetirementContribution.iap_balance_amount.ToString()]);
                }

                if (Convert.ToString(ldrIAPContributions[enmPersonAccountRetirementContribution.local52_special_acct_bal_amount.ToString()]).IsNotNullOrEmpty())
                {
                    ldecLocal52Balance = Convert.ToDecimal(ldrIAPContributions[enmPersonAccountRetirementContribution.local52_special_acct_bal_amount.ToString()]);
                }

                if (Convert.ToString(ldrIAPContributions[enmPersonAccountRetirementContribution.local161_special_acct_bal_amount.ToString()]).IsNotNullOrEmpty())
                {
                    ldecLocal161Balance = Convert.ToDecimal(ldrIAPContributions[enmPersonAccountRetirementContribution.local161_special_acct_bal_amount.ToString()]);
                }

                busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution();
                lbusPersonAccountRetirementContribution.InsertPersonAccountRetirementContirbution(aintOldPersonAccountId, ldtEffectiveDate, DateTime.Now, lintComputationalYear,
                   -(ldecIAPBalalnce), -(ldecLocal52Balance), -(ldecLocal161Balance), busConstant.TRANSACTION_TYPE_SSN_ADJS, 0, 0, 0, 0, 0, lstrContributionType, lstrContributionSubType, 0, 0, 0);
            }

            #endregion
        }

        private decimal MergeIAPAllocations(DataTable adtWorkHistory, int aintIAPAllocationStartYear, int aintNewPersonAccountID, decimal adecThru79Hours,
                                            decimal adecPreviousYearIAPBalance, Collection<cdoDummyWorkData> aclbPersonWorkHistory_IAP, int aintIAPAllocationMergeEndYear,
                                            int aintOldPersonAccountId, DateTime ldtRetEffectiveDate)
        {
            decimal ldecIAPAlloc1Amount = 0, ldecAlloc2Amount = 0, ldecAlloc2InvstAmount = 0,
                        ldecAlloc2FrftAmount = 0, ldecAlloc3Amount = 0, ldecAlloc4InvstAmount = 0, ldecAllocation4Amount = 0, ldecAlloc4FrftAmount = 0,
                        ldecAllocationFactor = 0, ldecAlloc5AfflAmount = 0, ldecAlloc5NonAfflAmount = 0, ldecAlloc5BothAmount = 0, ldecTotalYTDHours = 0;

            DataTable ldtIAPFiltered = new DataTable();

            if (adtWorkHistory.AsEnumerable().Where(o => o.Field<Int16>("computationyear") == aintIAPAllocationStartYear).Count() > 0)
            {
                ldtIAPFiltered = adtWorkHistory.AsEnumerable().Where(o => o.Field<Int16>("computationyear") == aintIAPAllocationStartYear).CopyToDataTable();
            }

            foreach (DataRow ldr in ldtIAPFiltered.Rows)
            {
                ldecTotalYTDHours += (ldr["iaphoursa2"] == DBNull.Value ? 0.0M : Convert.ToDecimal(ldr["iaphoursa2"]));
            }


            DataTable ldtIAPContributions = LoadIAPContributions(aintNewPersonAccountID, aintIAPAllocationStartYear);
            DataRow[] ldrIAPContribution = ldtIAPContributions.FilterTable(utlDataType.Numeric, "computational_year", aintIAPAllocationStartYear);

            int lintAllocFactorQuarter = 4;
            bool lblnIsRetirementYear = false;
            //prod pir 261
            if (ldtRetEffectiveDate != DateTime.MinValue)
            {
                if (ldtRetEffectiveDate.Year == aintIAPAllocationStartYear)
                {
                    lblnIsRetirementYear = true;
                    if (ldtRetEffectiveDate.Month > 3)
                    {
                        lintAllocFactorQuarter = busGlobalFunctions.GetPreviousQuarter(ldtRetEffectiveDate);
                    }
                    else
                    {
                        lintAllocFactorQuarter = 0;
                    }
                }
            }

            busIAPAllocationHelper lbusIAPAllocationHelper = new busIAPAllocationHelper();
            ldecIAPAlloc1Amount = lbusIAPAllocationHelper.CalculateAllocation1Amount(aintIAPAllocationStartYear,
                                adecPreviousYearIAPBalance, lintAllocFactorQuarter, ref ldecAllocationFactor);



            if (ldecTotalYTDHours >= Convert.ToDecimal(HelperUtil.GetData1ByCodeValue(52, busConstant.QualifiedYearHours)))
            {
                //method to calculate allocation 2 amount
                ldecAlloc2Amount = lbusIAPAllocationHelper.CalculateAllocation2Amount(aintIAPAllocationStartYear, adecThru79Hours, ldecTotalYTDHours, DateTime.MinValue,
                                                                        DateTime.MinValue, DateTime.MinValue);
                //method to calculate allocation 2 investment amount
                if (!lblnIsRetirementYear)
                {
                    ldecAlloc2InvstAmount = lbusIAPAllocationHelper.CalculateAllocation2InvstOrFrftAmount(aintIAPAllocationStartYear, ldecTotalYTDHours, DateTime.MinValue,
                                                                            DateTime.MinValue, DateTime.MinValue, busConstant.IAPAllocationInvestmentFlag);
                    //method to calculate allocation 2 forfeiture amount
                    ldecAlloc2FrftAmount = lbusIAPAllocationHelper.CalculateAllocation2InvstOrFrftAmount(aintIAPAllocationStartYear, ldecTotalYTDHours, DateTime.MinValue,
                                                                            DateTime.MinValue, DateTime.MinValue, busConstant.IAPAllocationForfeitureFlag);
                    //method to calculate allocation 3 amount
                    ldecAlloc3Amount = lbusIAPAllocationHelper.CalculateAllocation3Amount(aintIAPAllocationStartYear, adecThru79Hours, ldecTotalYTDHours);

                    //method to calculate allocation 4 investment amount
                    ldecAlloc4InvstAmount = lbusIAPAllocationHelper.CalculateAllocation4InvstOrFrftAmount(aintIAPAllocationStartYear, ldecAllocation4Amount, busConstant.IAPAllocationInvestmentFlag);
                    //method to calculate allocation 4 forfeiture amount
                    ldecAlloc4FrftAmount = lbusIAPAllocationHelper.CalculateAllocation4InvstOrFrftAmount(aintIAPAllocationStartYear, ldecAllocation4Amount, busConstant.IAPAllocationForfeitureFlag);

                }
                //Fix for UAT PIR 1023
                ldecAllocation4Amount = lbusIAPAllocationHelper.CalculateAllocation4Amount(aintIAPAllocationStartYear, ldtIAPFiltered);

                //Block to calculate allocation 5 amount
                if (aintIAPAllocationStartYear >= 1996 && aintIAPAllocationStartYear <= 2001 && ldecAllocation4Amount != 0.00M && !lblnIsRetirementYear)
                {
                    bool lblnAgeFlag = busGlobalFunctions.CalculatePersonAge(iclbNewPerson[0].icdoPerson.idtDateofBirth, Convert.ToDateTime(HelperUtil.GetData1ByCodeValue(52, busConstant.IAPInceptionDate))) < 55 ? true : false;
                    if (lbusIAPAllocationHelper.CheckParticipantIsAffiliate(aintIAPAllocationStartYear, iclbNewPerson[0].icdoPerson.istrSSNNonEncrypted, lobjLegacyPassInfoEADB))
                        ldecAlloc5AfflAmount = lbusIAPAllocationHelper.CalcuateAllocation5AffliatesAmount(aintIAPAllocationStartYear, aclbPersonWorkHistory_IAP, lblnAgeFlag);
                    else
                        ldecAlloc5NonAfflAmount = lbusIAPAllocationHelper.CalcuateAllocation5NonAffOrBothAmount(aintIAPAllocationStartYear, ldecTotalYTDHours, busConstant.IAPAllocationNonAffiliatesFlag);
                    ldecAlloc5BothAmount = lbusIAPAllocationHelper.CalcuateAllocation5NonAffOrBothAmount(aintIAPAllocationStartYear, ldecTotalYTDHours, busConstant.IAPAllocationBothAffAndNonAffFlag);
                }
            }
            //Method to post the difference amount into contribution amount
            //Prod PIR 261
            //Prod PIR 258-Do not recalculate
            //PostDifferenceAmountIntoContribution(aintIAPAllocationStartYear, aintNewPersonAccountID, ldrIAPContribution, ldecIAPAlloc1Amount, ldecAlloc2Amount, ldecAlloc2InvstAmount, ldecAlloc2FrftAmount, ldecAlloc3Amount, ldecAllocation4Amount,
            // ldecAlloc4InvstAmount, ldecAlloc4FrftAmount, ldecAlloc5AfflAmount, ldecAlloc5NonAfflAmount, ldecAlloc5BothAmount, 0, 0, ldtRetEffectiveDate);
            //updating IAP account balance with the latest allocation amounts
            adecPreviousYearIAPBalance = adecPreviousYearIAPBalance + ldecIAPAlloc1Amount + ldecAlloc2Amount + ldecAlloc2InvstAmount + ldecAlloc2FrftAmount + ldecAlloc3Amount + ldecAllocation4Amount +
                                            ldecAlloc4InvstAmount + ldecAlloc4FrftAmount + ldecAlloc5AfflAmount + ldecAlloc5NonAfflAmount + ldecAlloc5BothAmount;
            return adecPreviousYearIAPBalance;
        }

        private decimal MergeLocal52SpecialAccountBalance(int aintLocal52AllocationStartYear, decimal adecPreviousYearLocal52Balance, int aintPersonAccountID)
        {
            decimal ldecLocal52Alloc1Amount = 0, ldecAllocationFactor = 0;

            DataTable ldtIAPContributions = LoadIAPContributions(aintPersonAccountID, aintLocal52AllocationStartYear);
            DataRow[] ldrIAPContribution = ldtIAPContributions.FilterTable(utlDataType.Numeric, "computational_year", aintLocal52AllocationStartYear);

            busIAPAllocationHelper lbusIAPAllocationHelper = new busIAPAllocationHelper();
            ldecLocal52Alloc1Amount = lbusIAPAllocationHelper.CalculateAllocation1Amount(aintLocal52AllocationStartYear + 1,
                                    adecPreviousYearLocal52Balance, 4, ref ldecAllocationFactor);

            adecPreviousYearLocal52Balance = adecPreviousYearLocal52Balance + ldecLocal52Alloc1Amount;
            //Prod PIR 261
            //Prod PIR 258-Do not recalculate
            //PostDifferenceAmountIntoContribution(aintLocal52AllocationStartYear, aintPersonAccountID, ldrIAPContribution, 0, 0, 0, 0, 0, 0,
            //0, 0, 0, 0, 0, ldecLocal52Alloc1Amount, 0, DateTime.MinValue);

            return adecPreviousYearLocal52Balance;
        }

        private decimal MergeLocal161SpecialAccountBalance(int aintLocal161AllocationStartYear, decimal adecPreviousYearLocal161Balance, int aintPersonAccountID)
        {
            decimal ldecLocal52Alloc1Amount = 0, ldecAllocationFactor = 0;

            DataTable ldtIAPContributions = LoadIAPContributions(aintPersonAccountID, aintLocal161AllocationStartYear);
            DataRow[] ldrIAPContribution = ldtIAPContributions.FilterTable(utlDataType.Numeric, "computational_year", aintLocal161AllocationStartYear);

            busIAPAllocationHelper lbusIAPAllocationHelper = new busIAPAllocationHelper();
            ldecLocal52Alloc1Amount = lbusIAPAllocationHelper.CalculateAllocation1Amount(aintLocal161AllocationStartYear + 1,
                                    adecPreviousYearLocal161Balance, 4, ref ldecAllocationFactor);

            adecPreviousYearLocal161Balance = adecPreviousYearLocal161Balance + ldecLocal52Alloc1Amount;
            //Prod PIR 261 
            //Prod PIR 258-Do not recalculate
            //PostDifferenceAmountIntoContribution(aintLocal161AllocationStartYear, aintPersonAccountID, ldrIAPContribution, 0, 0, 0, 0, 0, 0,
            // 0, 0, 0, 0, 0, 0, ldecLocal52Alloc1Amount, DateTime.MinValue);

            return adecPreviousYearLocal161Balance;
        }

        private void LoadPreviousYearAllocationSummary()
        {
            ibusPrevYearAllocationSummary = new busIapAllocationSummary();
            ibusPrevYearAllocationSummary.LoadLatestAllocationSummary();
        }

        public void PostDifferenceAmountIntoContribution(int aintComputationYear, int aintPersonAccountID, DataRow[] adrIAPContribution, decimal adecAlloc1Amount, decimal adecAlloc2Amount, decimal adecAlloc2InvstAmount, decimal adecAlloc2FrftAmount,
           decimal adecAlloc3Amount, decimal adecAllocation4Amount, decimal adecAlloc4InvstAmount, decimal adecAlloc4FrftAmount, decimal adecAlloc5AfflAmount, decimal adecAlloc5NonAfflAmount, decimal adecAlloc5BothAmount,
           decimal adecLocal52SpecialAccountBalance, decimal adecLocal161SpecialAccountBalance, DateTime ldtRetirementEffectiveDate)
        {
            DateTime ldtEffectiveDate = new DateTime();
            if (aintComputationYear == ldtRetirementEffectiveDate.Year && ldtRetirementEffectiveDate != DateTime.MinValue)
            {
                ldtEffectiveDate = ldtRetirementEffectiveDate;
            }
            else
            {
                ldtEffectiveDate = busGlobalFunctions.GetLastDateOfComputationYear(aintComputationYear);
            }
            busPersonAccountRetirementContribution lobjRetrContribution;
            //block to insert the allocation 1 difference amount            
            lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

            if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc1"].IsDBNull()))
                adecAlloc1Amount -= Convert.ToDecimal(adrIAPContribution[0]["alloc1"]);
            if (adecAlloc1Amount != 0)
            {
                lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, ldtEffectiveDate, DateTime.Now, aintComputationYear, adecIAPBalanceAmount: adecAlloc1Amount,
                astrTransactionType: busConstant.TRANSACTION_TYPE_SSN_ADJS, astrContributionType: busConstant.RCContributionTypeAllocation1);
            }
            //block to insert the allocation 2 difference amount            
            lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

            if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc2"].IsDBNull()))
                adecAlloc2Amount -= Convert.ToDecimal(adrIAPContribution[0]["alloc2"]);

            if (adecAlloc2Amount != 0)
            {
                lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, ldtEffectiveDate, DateTime.Now, aintComputationYear, adecIAPBalanceAmount: adecAlloc2Amount,
                    astrTransactionType: busConstant.TRANSACTION_TYPE_SSN_ADJS, astrContributionType: busConstant.RCContributionTypeAllocation2);
            }
            //block to insert the allocation 2 invst difference amount            
            lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

            if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc2_invt"].IsDBNull()))
                adecAlloc2InvstAmount -= Convert.ToDecimal(adrIAPContribution[0]["alloc2_invt"]);
            if (adecAlloc2InvstAmount != 0)
            {
                lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, ldtEffectiveDate, DateTime.Now, aintComputationYear, adecIAPBalanceAmount: adecAlloc2InvstAmount,
                astrTransactionType: busConstant.TRANSACTION_TYPE_SSN_ADJS, astrContributionType: busConstant.RCContributionTypeAllocation2, astrContributionSubtype: busConstant.RCContributionSubTypeInvestment);
            }
            //block to insert the allocation 2 frft difference amount            
            lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

            if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc2_frft"].IsDBNull()))
                adecAlloc2FrftAmount -= Convert.ToDecimal(adrIAPContribution[0]["alloc2_frft"]);
            if (adecAlloc2FrftAmount != 0)
            {
                lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, ldtEffectiveDate, DateTime.Now, aintComputationYear, adecIAPBalanceAmount: adecAlloc2FrftAmount,
                astrTransactionType: busConstant.TRANSACTION_TYPE_SSN_ADJS, astrContributionType: busConstant.RCContributionTypeAllocation2, astrContributionSubtype: busConstant.RCContributionSubTypeForfeited);
            }
            //block to insert the allocation 3 difference amount            
            lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

            if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc3"].IsDBNull()))
                adecAlloc3Amount -= Convert.ToDecimal(adrIAPContribution[0]["alloc3"]);
            if (adecAlloc3Amount != 0)
            {
                lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, ldtEffectiveDate, DateTime.Now, aintComputationYear, adecIAPBalanceAmount: adecAlloc3Amount,
                astrTransactionType: busConstant.TRANSACTION_TYPE_SSN_ADJS, astrContributionType: busConstant.RCContributionTypeAllocation3);
            }
            //block to insert the allocation 4 difference amount            
            lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

            if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc4"].IsDBNull()))
                adecAllocation4Amount -= Convert.ToDecimal(adrIAPContribution[0]["alloc4"]);
            if (adecAllocation4Amount != 0)
            {
                lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, ldtEffectiveDate, DateTime.Now, aintComputationYear, adecIAPBalanceAmount: adecAllocation4Amount,
                astrTransactionType: busConstant.TRANSACTION_TYPE_SSN_ADJS, astrContributionType: busConstant.RCContributionTypeAllocation4);
            }
            //block to insert the allocation 4 invt difference amount            
            lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

            if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc4_invt"].IsDBNull()))
                adecAlloc4InvstAmount -= Convert.ToDecimal(adrIAPContribution[0]["alloc4_invt"]);
            if (adecAlloc4InvstAmount != 0)
            {
                lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, ldtEffectiveDate, DateTime.Now, aintComputationYear, adecIAPBalanceAmount: adecAlloc4InvstAmount,
                astrTransactionType: busConstant.TRANSACTION_TYPE_SSN_ADJS, astrContributionType: busConstant.RCContributionTypeAllocation4, astrContributionSubtype: busConstant.RCContributionSubTypeInvestment);
            }
            //block to insert the allocation 4 forft difference amount            
            lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };

            if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc4_frft"].IsDBNull()))
                adecAlloc4FrftAmount -= Convert.ToDecimal(adrIAPContribution[0]["alloc4_frft"]);
            if (adecAlloc4FrftAmount != 0)
            {
                lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, ldtEffectiveDate, DateTime.Now, aintComputationYear, adecIAPBalanceAmount: adecAlloc4FrftAmount,
                astrTransactionType: busConstant.TRANSACTION_TYPE_SSN_ADJS, astrContributionType: busConstant.RCContributionTypeAllocation4, astrContributionSubtype: busConstant.RCContributionSubTypeForfeited);
            }

            //block to insert the allocation 5 affl & non affl difference amount            
            lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
            decimal ldecTotalAlloc5 = adecAlloc5AfflAmount + adecAlloc5NonAfflAmount + adecAlloc5BothAmount;
            if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["alloc5"].IsDBNull()))
                ldecTotalAlloc5 -= Convert.ToDecimal(adrIAPContribution[0]["alloc5"]);
            if (ldecTotalAlloc5 != 0)
            {
                lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, ldtEffectiveDate, DateTime.Now, aintComputationYear, adecIAPBalanceAmount: ldecTotalAlloc5,
                astrTransactionType: busConstant.TRANSACTION_TYPE_SSN_ADJS, astrContributionType: busConstant.RCContributionTypeAllocation5);
            }


            lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
            if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["LOCAL161_SPECIAL_ACCT_BAL_AMOUNT"].IsDBNull()))
                adecLocal161SpecialAccountBalance -= Convert.ToDecimal(adrIAPContribution[0]["LOCAL161_SPECIAL_ACCT_BAL_AMOUNT"]);
            if (adecLocal161SpecialAccountBalance != 0)
            {
                lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, ldtEffectiveDate, DateTime.Now, aintComputationYear,
                adec161SplAccountBalance: adecLocal161SpecialAccountBalance, astrTransactionType: busConstant.TRANSACTION_TYPE_SSN_ADJS, astrContributionType: busConstant.RCContributionTypeAllocation1);
            }

            lobjRetrContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
            if (adrIAPContribution.Length > 0 && !Convert.ToBoolean(adrIAPContribution[0]["LOCAL52_SPECIAL_ACCT_BAL_AMOUNT"].IsDBNull()))
                adecLocal52SpecialAccountBalance -= Convert.ToDecimal(adrIAPContribution[0]["LOCAL52_SPECIAL_ACCT_BAL_AMOUNT"]);
            if (adecLocal52SpecialAccountBalance != 0)
            {
                lobjRetrContribution.InsertPersonAccountRetirementContirbution(aintPersonAccountID, ldtEffectiveDate, DateTime.Now, aintComputationYear,
                adec52SplAccountBalance: adecLocal52SpecialAccountBalance, astrTransactionType: busConstant.TRANSACTION_TYPE_SSN_ADJS, astrContributionType: busConstant.RCContributionTypeAllocation1);
            }
        }

        private DataTable LoadIAPContributions(int aintPersonAccountID, int aintComputationYear)
        {
            return busBase.Select("cdoPersonAccountRetirementContribution.GetIAPAllocationsForPersonAccount", new object[2] { aintPersonAccountID, aintComputationYear });
        }

    }

}
