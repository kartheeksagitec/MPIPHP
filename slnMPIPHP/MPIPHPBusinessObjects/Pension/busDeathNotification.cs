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

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busDeathNotification:
    /// Inherited from busDeathNotificationGen, the class is used to customize the business object busDeathNotificationGen.
    /// </summary>
    [Serializable]
    public class busDeathNotification : busDeathNotificationGen
    {

        public Collection<busPersonAccountBeneficiary> iclbBeneficiaryOf { get; set; }
        public Collection<busRelationship> iclbDependentOf { get; set; }
        public Collection<busQdroApplication> iclbDroApplicationDetails { get; set; }
        public Collection<busBenefitApplication> iclbBenefitApplicationDetails { get; set; }
        public Collection<busDeathNotification> iclbDeathNotificationDetails { get; set; }
        public Collection<busPayeeAccountStatus> iclbPayeeAccountStatusForDeathNotification { get; set; }
        public Collection<busPayeeAccount> iclbPayeeAccount { get; set; }

        public string istrBenName { get; set; }
        public string istrBenNamePrefix { get; set; }
        public string istrBeneLastName { get; set; }

        public busDocumentProcessCrossref ibusDocumentProcessCrossref { get; set; }
        public string istrBenefitOptionPercent { get; set; }
		//Ticket - 68031
        public Collection<busPersonAccountStatusBeforeDeath> iclbPersonAccountStatusBeforeDeath { get; set; }


        public override void BeforePersistChanges()
        {
            base.BeforePersistChanges();

            this.ValidateHardErrors(utlPageMode.All);

            //PIR 806
            this.ibusPerson.icdoPerson.date_of_death = this.icdoDeathNotification.date_of_death;
            this.ibusPerson.icdoPerson.Update();

            if (string.IsNullOrEmpty(this.icdoDeathNotification.death_notification_status_value))
            {
                this.icdoDeathNotification.death_notification_status_value = busConstant.NOTIFICATION_STATUS_IN_PROGRESS;
            }

            this.icdoDeathNotification.person_id = this.ibusPerson.icdoPerson.person_id;
            this.ibusPerson.iclbNotes.ForEach(item =>
            {
                if (item.icdoNotes.person_id == 0)
                    item.icdoNotes.person_id = this.icdoDeathNotification.person_id;
                item.icdoNotes.form_id = busConstant.Form_ID;
                item.icdoNotes.form_value = busConstant.DEATH_NOTIFICATION_MAINTANENCE_FORM;
            });

            #region for PIR-58

            DataTable ldtbCheckIfParticipantIsRetiree = busBase.Select("cdoDeathNotification.GetPayeeAccountToCheckRetireeStatus", new object[1] { this.icdoDeathNotification.person_id });
            if (ldtbCheckIfParticipantIsRetiree != null && ldtbCheckIfParticipantIsRetiree.Rows.Count > 0)
            {
                //initiate WF only if Person is Participant
                int lintCheckParticipant = (int)DBFunction.DBExecuteScalar("cdoDeathNotification.GetParticipantCount", new object[1] { this.icdoDeathNotification.person_id },
                                                            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                if (lintCheckParticipant > 0)
                {
                    DataTable ldtblPaymentsAfterDateOfDeath = Select("cdoDeathNotification.GetPaymentsAfterDateOfDeath", new object[3] { icdoDeathNotification.person_id,
                    icdoDeathNotification.date_of_death,DateTime.Now });

                    if (ldtblPaymentsAfterDateOfDeath != null && ldtblPaymentsAfterDateOfDeath.Rows.Count > 0)
                    {
                        foreach (DataRow ldrPayments in ldtblPaymentsAfterDateOfDeath.Rows)
                        {
                            int lintPayeeAccountId = 0;
                            if (Convert.ToString(ldrPayments[enmPaymentHistoryHeader.payee_account_id.ToString().ToUpper()]).IsNotNullOrEmpty())
                                lintPayeeAccountId = Convert.ToInt32(ldrPayments[enmPaymentHistoryHeader.payee_account_id.ToString().ToUpper()]);
                            ///Code to initiate new workflow
                            Hashtable lhstRequestParams = new Hashtable();
                            lhstRequestParams.Add("PayeeAccountId", lintPayeeAccountId);

                            if (icdoDeathNotification.date_of_death.Day == 1)
                            {
                                lhstRequestParams.Add("PaymentDateFrom", string.Format("{0:MM/dd/yyyy}", icdoDeathNotification.date_of_death.AddMonths(1)));
                            }
                            else
                            {
                                lhstRequestParams.Add("PaymentDateFrom", string.Format("{0:MM/dd/yyyy}", icdoDeathNotification.date_of_death));
                            }

                            lhstRequestParams.Add("PaymentDateTo", string.Format("{0:MM/dd/yyyy}", DateTime.Now));

                            busWorkflowHelper.InitializeWorkflow(busConstant.PROCESS_STOP_REISSUE_OR_RECLAMATION, icdoDeathNotification.person_id,
                                                                        0, 0, lhstRequestParams);
                        }
                    }
                    AuditLogHistoryForPerson(this.icdoDeathNotification.date_of_death.ToString());
                }
            }
            #endregion
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();


            DataTable ldtbPayeeAccountIDs = Select("cdoPayeeAccount.GetPayeeAccountForSuspensionDeathNotification", new object[1] { this.icdoDeathNotification.person_id });
            if (ldtbPayeeAccountIDs.Rows.Count > 0)
            {
                foreach (DataRow ldrPayeeAccountId in ldtbPayeeAccountIDs.Rows)
                {
                    cdoPayeeAccountStatus lcdoPayeeAccountStatus = new cdoPayeeAccountStatus();
                    lcdoPayeeAccountStatus.payee_account_id = Convert.ToInt32(ldrPayeeAccountId["PAYEE_ACCOUNT_ID"]);
                    lcdoPayeeAccountStatus.status_value = busConstant.PAYEE_ACCOUNT_STATUS_SUSPENDED;
                    lcdoPayeeAccountStatus.terminated_status_reason_value = busConstant.PayeeAccountTerminationReasonDeath;
                    //Ticket#69506
                   // lcdoPayeeAccountStatus.suspension_status_reason_value = busConstant.PayeeAccountSuspensionReasonDeath;
                    lcdoPayeeAccountStatus.status_effective_date = DateTime.Now;
                    lcdoPayeeAccountStatus.Insert();
                }
                //PIR-893
                DataTable ldtbAlternatePayeeAccountIDs = Select("cdoPayeeAccount.GetAlternatePayeeAccountForSuspensionDeathNotification", new object[1] { this.icdoDeathNotification.person_id });
                if (ldtbAlternatePayeeAccountIDs.Rows.Count > 0)
                {

                    foreach (DataRow ldrAlternatePayeeAccountId in ldtbAlternatePayeeAccountIDs.Rows)
                    {
                        cdoPayeeAccountStatus lcdoPayeeAccountStatus = new cdoPayeeAccountStatus();
                        lcdoPayeeAccountStatus.payee_account_id = Convert.ToInt32(ldrAlternatePayeeAccountId["PAYEE_ACCOUNT_ID"]);
                        lcdoPayeeAccountStatus.status_value = busConstant.PAYEE_ACCOUNT_STATUS_SUSPENDED;
                        lcdoPayeeAccountStatus.suspension_status_reason_value = "OTHR";
                        lcdoPayeeAccountStatus.suspension_reason_description = "Participant's Death";
                        lcdoPayeeAccountStatus.status_effective_date = DateTime.Now;
                        lcdoPayeeAccountStatus.Insert();

                    }
                }
            }


            LoadPayeeAccount();
            // decommissioning demographics informations, since HEDB is retiring.
            //  UpdateHEDB(busConstant.POTENTIAL_DEATH_CERTIFICATION);
            //When Death Notification Gets SAVED we start DEATH PRE-RETIREMENT WORKFLOW 
            //busWorkflowHelper.InitializeWorkflowIfNotExists(busConstant.PRERETIREMENT_DEATH_WORKFLOW_NAME, this.ibusPerson.icdoPerson.person_id, 0, 0, null);
            if (this.iarrChangeLog.Count > 0)
            {
                this.icdoDeathNotification.modified_by = iobjPassInfo.istrUserID;
            }
        }

        public override busBase GetCorPerson()
        {
            this.ibusPerson.LoadPersonAddresss();
            this.ibusPerson.LoadPersonContacts();
            this.ibusPerson.LoadCorrAddress();
            return this.ibusPerson;
        }

        public override void LoadCorresProperties(string astrTemplateName)
        {
            base.LoadCorresProperties(astrTemplateName);
            DateTime ldtCurrentDate = System.DateTime.Now;
            this.ibusPerson.istrCurrentDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtCurrentDate);

            //this.ibusPerson.LoadBeneficiaries();
            //foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in this.ibusPerson.iclbPersonAccountBeneficiary)
            //{
            //    if (lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.iaintPlan == busConstant.MPIPP_PLAN_ID 
            //        && lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.beneficiary_type_value == "PRIM")
            //    {
            //        if (lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.idtBenDateOfDeath != DateTime.MinValue)
            //        {
            //            istrBenName = lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.istrBenFullName;
            //            if (lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.istrNamePrefixValue.IsNotNullOrEmpty())
            //                istrBenNamePrefix = lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.istrNamePrefixValue;
            //            istrBeneLastName = lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.istrBenLastName;
            //            break;
            //        }
            //    }
            //}
            if (astrTemplateName == busConstant.RETIREE_DEATH_LETTER_TO_SURVIVING_SPOUSE_TO_START_ANNUITY)
            {
                foreach (busPayeeAccount lbusPayeeAccount in this.iclbPayeeAccount)
                {
                    if ((lbusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_RETIREMENT ||
                        lbusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_DISABILITY) && (lbusPayeeAccount.icdoPayeeAccount.retiree_incr_flag == busConstant.FLAG_NO)
                        && (lbusPayeeAccount.icdoPayeeAccount.istrPayeeAccountCurrentStatus == "Payments Completed") && (lbusPayeeAccount.icdoPayeeAccount.istrTerminationReason == busConstant.PayeeAccountTerminationReasonDeath))
                    {
                        busPlanBenefitXr lbusPlanBenefitXr = new busPlanBenefitXr { icdoPlanBenefitXr = new cdoPlanBenefitXr() };
                        lbusPlanBenefitXr.FindPlanBenefitXr(lbusPayeeAccount.icdoPayeeAccount.plan_benefit_id);

                        if (lbusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value.Contains("50"))
                            istrBenefitOptionPercent = "50%";
                        else if (lbusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value.Contains("75"))
                            istrBenefitOptionPercent = "75%";
                        else if (lbusPlanBenefitXr.icdoPlanBenefitXr.benefit_option_value.Contains("100"))
                            istrBenefitOptionPercent = "100%";
                        else
                            istrBenefitOptionPercent = string.Empty;
                    }

                }

            }
        }

        public void sample(string astrBenName, string astrBenNamePrefix, string astrBeneLastName)
        {
            //base.LoadCorresProperties(astrTemplateName);
            DateTime ldtCurrentDate = System.DateTime.Now;
            this.ibusPerson.istrCurrentDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtCurrentDate);

            istrBenName = astrBenName;
            istrBenNamePrefix = astrBenNamePrefix;
            if (!string.IsNullOrEmpty(istrBenNamePrefix))
            {
                if (istrBenNamePrefix.ToUpper() == "MISS")
                    istrBenNamePrefix = "Ms.";
                else if (istrBenNamePrefix.ToUpper() == "DR")
                    istrBenNamePrefix = "Dr.";
                else if (istrBenNamePrefix.ToUpper() == "MRS")
                    istrBenNamePrefix = "Mrs.";
                else if (istrBenNamePrefix.ToUpper() == "MR")
                    istrBenNamePrefix = "Mr.";
            }
            istrBeneLastName = astrBeneLastName;
        }

        public void UpdateHEDB(string astrNotificationStatus)
        {
            //OPUS data push to Health Eligibility for any person Update  //Commented - Rohan Code For data Push to HEDB (Do not delete this)


            //DataTable ldtbSysMgmt = iobjPassInfo.isrvDBCache.GetSystemManagement();
            //if (ldtbSysMgmt.Rows.Count > 0 && ldtbSysMgmt.Rows[0]["REGION_VALUE"].ToString() == "DEVL")
            //{
            //    return;
            //}
            if ((iobjPassInfo.iconFramework).Database == "MPI" || (iobjPassInfo.iconFramework).Database == "MPIPHP")
            {
                return;
            }
            // decommissioning demographics informations, since HEDB is retiring.
            //utlPassInfo lobjPassInfo1 = new utlPassInfo();
            //lobjPassInfo1.idictParams["ID"] = "OPUS_INTEGRATION";
            //lobjPassInfo1.iconFramework = DBFunction.GetDBConnection("HELegacy");

            //if (lobjPassInfo1.iconFramework != null)
            //{
            //    string lstrMPIPersonId = this.ibusPerson.icdoPerson.mpi_person_id;
            //    string lstrPersonSSN = this.ibusPerson.icdoPerson.ssn;//PIR 806
            //    string lstrParticipantMPIID = string.Empty;
            //    string lstrRelationshipType = string.Empty;


            //    #region Commented Code -Abhishek (I don't see a reason why we need it)
            //    //try
            //    //{
            //    //    string strQuery = "select * from person where ssn = (select ssn from Eligibility_PID_Reference where PID = '" + this.ibusPerson.icdoPerson.mpi_person_id + "')";
            //    //    DataTable ldtbResult = DBFunction.DBSelect(strQuery, lconHELegacy);
            //    //    if (ldtbResult.Rows.Count == 0)
            //    //    {
            //    //        return;
            //    //    }

            //    //}
            //    //catch
            //    //{

            //    //}
            //    #endregion

            //    int CountDependent = (int)DBFunction.DBExecuteScalar("cdoPerson.ChechPersonIsDependent", new object[1] { this.ibusPerson.icdoPerson.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            //    int CountBeneficiary = (int)DBFunction.DBExecuteScalar("cdoPerson.CheckPersonIsBeneficiary", new object[1] { this.ibusPerson.icdoPerson.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

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

            //    string lstrFirstName = this.ibusPerson.icdoPerson.first_name;
            //    string lstrMiddleName = this.ibusPerson.icdoPerson.middle_name;
            //    string lstrlastName = this.ibusPerson.icdoPerson.last_name;
            //    string lstrGender = this.ibusPerson.icdoPerson.gender_value;
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

            //    lstrDOB = this.ibusPerson.icdoPerson.date_of_birth;//PIR 806

            //    DateTime lstrDOD = DateTime.MinValue;

            //    lstrDOD = this.icdoDeathNotification.date_of_death;


            //    string lstrHomePhone = this.ibusPerson.icdoPerson.home_phone_no;
            //    string lstrCellPhone = this.ibusPerson.icdoPerson.cell_phone_no;
            //    string lstrFax = this.ibusPerson.icdoPerson.fax_no;
            //    string lstrEmail = this.ibusPerson.icdoPerson.email_address_1;
            //    string lstrCreatedBy = this.icdoDeathNotification.modified_by;
            //    //Ticket : 55015
            //    string lstrVipFlag = this.ibusPerson.icdoPerson.vip_flag;
            //    string lUpperlstrVipFlag = lstrVipFlag.ToUpper();
            //    bool lboolVipFlag = false;
            //    if (lUpperlstrVipFlag == "Y")
            //    {
            //        lboolVipFlag = true;
            //    }
            //    //if (!String.IsNullOrEmpty(lstrPersonSSN))
            //    //{
            //    Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();

            //    IDbDataParameter lobjParameter1 = DBFunction.GetDBParameter();
            //    lobjParameter1.ParameterName = "@PID";
            //    lobjParameter1.DbType = DbType.String;
            //    lobjParameter1.Value = lstrMPIPersonId.ToLower();
            //    lcolParameters.Add(lobjParameter1);

            //    IDbDataParameter lobjParameter2 = DBFunction.GetDBParameter();
            //    lobjParameter2.ParameterName = "@SSN";
            //    lobjParameter2.DbType = DbType.String;
            //    if (lstrPersonSSN.IsNullOrEmpty())
            //        lobjParameter2.Value = DBNull.Value;
            //    else
            //        lobjParameter2.Value = lstrPersonSSN.ToLower();
            //    lcolParameters.Add(lobjParameter2);

            //    IDbDataParameter lobjParameter3 = DBFunction.GetDBParameter();
            //    lobjParameter3.ParameterName = "@ParticipantPID";
            //    lobjParameter3.DbType = DbType.String;
            //    lobjParameter3.Value = DBNull.Value;    //Need to change            
            //    lcolParameters.Add(lobjParameter3);

            //    IDbDataParameter lobjParameter4 = DBFunction.GetDBParameter();
            //    lobjParameter4.ParameterName = "@EntityTypeCode";
            //    lobjParameter4.DbType = DbType.String;
            //    lobjParameter4.Value = "P";                 //for now we will always use Person
            //    lcolParameters.Add(lobjParameter4);

            //    IDbDataParameter lobjParameter5 = DBFunction.GetDBParameter();
            //    lobjParameter5.ParameterName = "@RelationType";
            //    lobjParameter5.DbType = DbType.String;
            //    lobjParameter5.Value = lstrRelationshipType;                //Need to change
            //    lcolParameters.Add(lobjParameter5);

            //    IDbDataParameter lobjParameter6 = DBFunction.GetDBParameter();
            //    lobjParameter6.ParameterName = "@FirstName";
            //    lobjParameter6.DbType = DbType.String;
            //    lobjParameter6.Value = lstrFirstName;
            //    lcolParameters.Add(lobjParameter6);

            //    IDbDataParameter lobjParameter7 = DBFunction.GetDBParameter();
            //    lobjParameter7.ParameterName = "@MiddleName";
            //    lobjParameter7.DbType = DbType.String;
            //    lobjParameter7.Value = lstrMiddleName;
            //    lcolParameters.Add(lobjParameter7);

            //    IDbDataParameter lobjParameter8 = DBFunction.GetDBParameter();
            //    lobjParameter8.ParameterName = "@LastName";
            //    lobjParameter8.DbType = DbType.String;
            //    lobjParameter8.Value = lstrlastName;
            //    lcolParameters.Add(lobjParameter8);

            //    IDbDataParameter lobjParameter9 = DBFunction.GetDBParameter();
            //    lobjParameter9.ParameterName = "@Gender";
            //    lobjParameter9.DbType = DbType.String;
            //    lobjParameter9.Value = lstrGender;
            //    lcolParameters.Add(lobjParameter9);

            //    IDbDataParameter lobjParameter10 = DBFunction.GetDBParameter();
            //    lobjParameter10.ParameterName = "@DateOfBirth";
            //    lobjParameter10.DbType = DbType.DateTime;
            //    if (lstrDOB != DateTime.MinValue)
            //    {
            //        lobjParameter10.Value = lstrDOB;
            //    }
            //    lcolParameters.Add(lobjParameter10);


            //    IDbDataParameter lobjParameter11 = DBFunction.GetDBParameter();
            //    lobjParameter11.ParameterName = "@DateOfDeath";
            //    lobjParameter11.DbType = DbType.DateTime;

            //    if (lstrDOD != DateTime.MinValue && astrNotificationStatus == busConstant.POTENTIAL_DEATH_CERTIFICATION)
            //    {
            //        lobjParameter11.Value = lstrDOD;
            //    }
            //    else
            //    {
            //        lobjParameter11.Value = DBNull.Value;
            //    }
            //    lcolParameters.Add(lobjParameter11);


            //    IDbDataParameter lobjParameter12 = DBFunction.GetDBParameter();
            //    lobjParameter12.ParameterName = "@HomePhone";
            //    lobjParameter12.DbType = DbType.String;
            //    lobjParameter12.Value = lstrHomePhone;
            //    lcolParameters.Add(lobjParameter12);

            //    IDbDataParameter lobjParameter13 = DBFunction.GetDBParameter();
            //    lobjParameter13.ParameterName = "@CellPhone";
            //    lobjParameter13.DbType = DbType.String;
            //    lobjParameter13.Value = lstrCellPhone;
            //    lcolParameters.Add(lobjParameter13);

            //    IDbDataParameter lobjParameter14 = DBFunction.GetDBParameter();
            //    lobjParameter14.ParameterName = "@Fax";
            //    lobjParameter14.DbType = DbType.String;
            //    lobjParameter14.Value = lstrFax;
            //    lcolParameters.Add(lobjParameter14);

            //    IDbDataParameter lobjParameter15 = DBFunction.GetDBParameter();
            //    lobjParameter15.ParameterName = "@Email";
            //    lobjParameter15.DbType = DbType.String;
            //    lobjParameter15.Value = lstrEmail;
            //    lcolParameters.Add(lobjParameter15);

            //    IDbDataParameter lobjParameter16 = DBFunction.GetDBParameter();
            //    lobjParameter16.ParameterName = "@AuditUser";
            //    lobjParameter16.DbType = DbType.String;
            //    lobjParameter16.Value = lstrCreatedBy;
            //    lcolParameters.Add(lobjParameter16);
            //    //Ticket : 55015
            //    IDbDataParameter lobjParameter17 = DBFunction.GetDBParameter();
            //    lobjParameter17.ParameterName = "@VipFlag";
            //    lobjParameter17.DbType = DbType.Boolean;
            //    lobjParameter17.Value = lboolVipFlag;
            //    lcolParameters.Add(lobjParameter17);
            //    try
            //    {
            //        lobjPassInfo1.BeginTransaction();
            //        DBFunction.DBExecuteProcedure("USP_PID_Person_UPD", lcolParameters, lobjPassInfo1.iconFramework, lobjPassInfo1.itrnFramework);
            //        lobjPassInfo1.Commit();
            //    }
            //    catch (Exception e)
            //    {
            //        lobjPassInfo1.Rollback();
            //        throw e;
            //    }
            //    finally
            //    {
            //        lobjPassInfo1.iconFramework.Close();
            //        lobjPassInfo1.iconFramework.Dispose();
            //    }

            //    //}
            //}
        }


        public ArrayList btn_Certified()
        {
            ArrayList larrList = new ArrayList();
            bool lblnflag = true;
            utlError lobjError = null;
            #region Conditions To Handle HardErrors
            this.ValidateHardErrors(utlPageMode.All);


            DataTable ldtbDeathNotification = busBase.Select("cdoDeathNotification.GetCertifiedNotification", new object[] { this.icdoDeathNotification.person_id });
            if (ldtbDeathNotification != null && ldtbDeathNotification.Rows.Count > 0)
            {
                int lintCount = Convert.ToInt32(ldtbDeathNotification.Rows[0]["COUNT"]);
                if (lintCount > 0)
                {
                    lobjError = AddError(5047, "");
                    larrList.Add(lobjError);
                    lblnflag = false;
                    return larrList;
                }

            }
            
            DataTable ldtbGetBenefitApplicationID = busBase.Select("cdoDeathNotification.GetBenefitApplicationByPersonID", new object[1] { this.icdoDeathNotification.person_id });


            foreach (busBenefitApplication lbusBenefitApplication in this.iclbBenefitApplicationDetails)
            {
                //if (lbusBenefitApplication.icdoBenefitApplication.cancellation_reason_value != busConstant.CANCELLATION_REASON_DECEASED)
                //{
                //    utlError lobjError = null;
                //    lobjError = AddError(5056, "");
                //    larrList.Add(lobjError);
                //    lblnflag = false;
                //    break;
                //}

                ///Hard Error Removed as per Debra mails : BR : 29 Death Post Retirement.                    


                if (lbusBenefitApplication.icdoBenefitApplication.application_status_value != busConstant.BENEFIT_APPL_CANCELLED)
                {
                    DataRow[] ldr = null;
                    if (ldtbGetBenefitApplicationID.Rows.Count > 0)
                    {
                        ldr = ldtbGetBenefitApplicationID.Select(enmBenefitApplication.benefit_application_id.ToString().ToUpper() + "=" + lbusBenefitApplication.icdoBenefitApplication.benefit_application_id.ToString());
                    }
                    if (ldr != null && ldr.Count() > 0)
                    {
                        continue;
                    }
                    else
                    {
                        lobjError = AddError(5054, "");
                        larrList.Add(lobjError);
                        lblnflag = false;
                        break;
                    }
                }
            }
            foreach (busQdroApplication lbusQdroApplication in this.iclbDroApplicationDetails)
            {
                if (lbusQdroApplication.icdoDroApplication.dro_status_value != busConstant.DRO_CANCELLED)
                {
                    DataRow[] ldr = null;
                    if (ldtbGetBenefitApplicationID.Rows.Count > 0)
                    {
                        ldr = ldtbGetBenefitApplicationID.Select(enmDroApplication.dro_application_id.ToString().ToUpper() + "=" + lbusQdroApplication.icdoDroApplication.dro_application_id.ToString());
                    }
                    if (ldr != null && ldr.Count() > 0)
                    {
                        continue;
                    }
                    else
                    {
                        lobjError = AddError(5053, "");
                        larrList.Add(lobjError);
                        lblnflag = false;
                        break;
                    }
                }
            }


            foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in this.ibusPerson.iclbPersonAccountBeneficiary)
            {
                if (lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.status_value != busConstant.STATUS_VALID && lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.status_value != "INAC")
                {
                    lobjError = AddError(5472, " ");
                    larrList.Add(lobjError);
                    lblnflag = false;
                    break;
                }
                if (lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.istrPlan == string.Empty || lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.istrPlan == null)
                {

                    lobjError = AddError(5159, "");
                    larrList.Add(lobjError);
                    lblnflag = false;
                    break;
                }
                if (lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.dist_percent == 0 && lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.istrPlan != busConstant.LIFE_PLAN)
                {

                    lobjError = AddError(5160, "");
                    larrList.Add(lobjError);
                    lblnflag = false;
                    break;
                }
                if (lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.start_date == DateTime.MinValue && lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.istrPlan != busConstant.LIFE_PLAN)
                {

                    lobjError = AddError(5161, "");
                    larrList.Add(lobjError);
                    lblnflag = false;
                    break;
                }

                if (lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.start_date > this.icdoDeathNotification.date_of_death && lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.istrPlan != busConstant.LIFE_PLAN)
                {
                    // utlError lobjError = null;
                    lobjError = AddError(1168, " ");
                    larrList.Add(lobjError);
                    lblnflag = false;
                    break;
                }
            }


            foreach (busPersonContact lbusPersonContact in this.ibusPerson.iclbPersonContact)
            {
                if (lbusPersonContact.icdoPersonContact.effective_start_date > this.icdoDeathNotification.date_of_death)
                {
                    // utlError lobjError = null;
                    lobjError = AddError(1167, " ");
                    larrList.Add(lobjError);
                    lblnflag = false;
                    break;
                }
            }
            foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in this.iclbBeneficiaryOf)
            {
                if (lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.istrPlan == string.Empty || lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.istrPlan == null)
                {

                    lobjError = AddError(5182, "");
                    larrList.Add(lobjError);
                    lblnflag = false;
                    break;
                }
                if (lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.dist_percent == 0 && lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.istrPlan != busConstant.LIFE_PLAN)
                {

                    lobjError = AddError(5183, "");
                    larrList.Add(lobjError);
                    lblnflag = false;
                    break;
                }
                if (lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.start_date == DateTime.MinValue && lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.istrPlan != busConstant.LIFE_PLAN)
                {

                    lobjError = AddError(5184, "");
                    larrList.Add(lobjError);
                    lblnflag = false;
                    break;
                }
                if (lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.start_date > this.icdoDeathNotification.date_of_death && lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.istrPlan != busConstant.LIFE_PLAN)
                {
                    //utlError lobjError = null;
                    lobjError = AddError(1169, " ");
                    larrList.Add(lobjError);
                    lblnflag = false;
                    break;
                }
            }



            #endregion

            if (lblnflag)
            {

                icdoDeathNotification.death_notification_status_value = busConstant.NOTIFICATION_STATUS_CERTIFIED;
                icdoDeathNotification.Update();
                this.EvaluateInitialLoadRules(utlPageMode.Update);
                larrList.Add(this);

                this.ibusPerson.icdoPerson.date_of_death = this.icdoDeathNotification.date_of_death;
                this.ibusPerson.icdoPerson.Update();


                foreach (busPersonAccount lbusPersonAccount in this.ibusPerson.iclbPersonAccount)
                {

					//Ticket - 68031
                    cdoPersonAccountStatusBeforeDeath lcdoPersonAccountStatusBeforeDeath = new cdoPersonAccountStatusBeforeDeath();
                    lcdoPersonAccountStatusBeforeDeath.death_notification_id = icdoDeathNotification.death_notification_id;
                    lcdoPersonAccountStatusBeforeDeath.start_date = lbusPersonAccount.icdoPersonAccount.start_date;
                    lcdoPersonAccountStatusBeforeDeath.end_date = lbusPersonAccount.icdoPersonAccount.end_date;
                    lcdoPersonAccountStatusBeforeDeath.plan_id = lbusPersonAccount.icdoPersonAccount.plan_id;
                    lcdoPersonAccountStatusBeforeDeath.status_value = lbusPersonAccount.icdoPersonAccount.status_value;
                    lcdoPersonAccountStatusBeforeDeath.created_by = iobjPassInfo.istrUserID;
                    lcdoPersonAccountStatusBeforeDeath.created_date = DateTime.Now;
                    lcdoPersonAccountStatusBeforeDeath.modified_by = iobjPassInfo.istrUserID;
                    lcdoPersonAccountStatusBeforeDeath.modified_date = DateTime.Now;
                    lcdoPersonAccountStatusBeforeDeath.update_seq = 0;
                    lcdoPersonAccountStatusBeforeDeath.Insert();

                    lbusPersonAccount.icdoPersonAccount.status_id = 6035;
                    lbusPersonAccount.icdoPersonAccount.status_value = busConstant.PERSON_ACCOUNT_STATUS_DECEASED;

                    if (lbusPersonAccount.icdoPersonAccount.end_date == DateTime.MinValue)
                    {
                        lbusPersonAccount.icdoPersonAccount.end_date = icdoDeathNotification.date_of_death;
                    }
                    else
                    {
                        if (lbusPersonAccount.icdoPersonAccount.end_date > icdoDeathNotification.date_of_death)
                            lbusPersonAccount.icdoPersonAccount.end_date = icdoDeathNotification.date_of_death;
                    }

                    lbusPersonAccount.icdoPersonAccount.Update();
                }

                foreach (busPersonContact lbusPersonContact in this.ibusPerson.iclbPersonContact)
                {
                    lbusPersonContact.icdoPersonContact.effective_end_date = this.icdoDeathNotification.date_of_death;
                    lbusPersonContact.icdoPersonContact.Update();

                }

                foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in this.ibusPerson.iclbPersonAccountBeneficiary)
                {

                    lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date = this.icdoDeathNotification.date_of_death;
                    lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.Update();
                    lbusPersonAccountBeneficiary.ibusParticipant = this.ibusPerson;
                    lbusPersonAccountBeneficiary.iaintMainPersonID = this.ibusPerson.icdoPerson.person_id;
                    lbusPersonAccountBeneficiary.ValidateSoftErrors();
                    lbusPersonAccountBeneficiary.UpdateValidateStatus();
                }
                this.ibusPerson.LoadBeneficiariesForDeath();
                foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in this.iclbBeneficiaryOf)
                {
                    lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date = this.icdoDeathNotification.date_of_death;
                    lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.Update();

                    // --- To validate Soft Error of all its Participants
                    Collection<busPersonAccountBeneficiary> iclbPersonAccountParticipantOfBeneficiary = new Collection<busPersonAccountBeneficiary>();
                    DataTable ldtblist = busPerson.Select("cdoPerson.LoadAllBeneficiaries", new object[1] { lbusPersonAccountBeneficiary.ibusPerson.icdoPerson.person_id });
                    iclbPersonAccountParticipantOfBeneficiary = GetCollection<busPersonAccountBeneficiary>(ldtblist, "icdoPersonAccountBeneficiary");

                    foreach (busPersonAccountBeneficiary lbusPersonAccountPartBen in iclbPersonAccountParticipantOfBeneficiary)
                    {
                        busPersonAccount lbusPersonAccount = new busPersonAccount();
                        if (lbusPersonAccount.FindPersonAccount(lbusPersonAccountPartBen.icdoPersonAccountBeneficiary.person_account_id))
                        {
                            lbusPersonAccountPartBen.iaintMainPersonID = lbusPersonAccountBeneficiary.ibusPerson.icdoPerson.person_id;
                            lbusPersonAccountPartBen.ibusParticipant = lbusPersonAccountBeneficiary.ibusPerson;
                            lbusPersonAccountPartBen.icdoPersonAccountBeneficiary.iaintPlan = lbusPersonAccount.icdoPersonAccount.plan_id;
                            lbusPersonAccountPartBen.ValidateSoftErrors();
                            lbusPersonAccountPartBen.FindPersonAccountBeneficiary(lbusPersonAccountPartBen.icdoPersonAccountBeneficiary.person_account_beneficiary_id);
                            lbusPersonAccountPartBen.UpdateValidateStatus();
                        }
                    }
                }

                #region If Participant dies and has shared DRO thn AP's benefits will get revert back to participant if not eligible for continuance

                RevertAlternatePayeeSharedBenefitsToParticipant();

                #endregion


                //For Changing Payee Account status as Payment Completed & termination Reason DEATH
                DataTable ldtbPayeeAccountIDs = Select("cdoPayeeAccount.GetPayeeAccountStatusForDeathNotification", new object[1] { this.icdoDeathNotification.person_id });
                if (ldtbPayeeAccountIDs.Rows.Count > 0)
                {
                    foreach (DataRow ldrPayeeAccountId in ldtbPayeeAccountIDs.Rows)
                    {
                        cdoPayeeAccountStatus lcdoPayeeAccountStatus = new cdoPayeeAccountStatus();
                        lcdoPayeeAccountStatus.payee_account_id = Convert.ToInt32(ldrPayeeAccountId["PAYEE_ACCOUNT_ID"]);
                        lcdoPayeeAccountStatus.status_value = busConstant.PAYEE_ACCOUNT_STATUS_COMPLETED;
                        lcdoPayeeAccountStatus.terminated_status_reason_value = busConstant.PayeeAccountTerminationReasonDeath;
                        lcdoPayeeAccountStatus.status_effective_date = DateTime.Now;
                        lcdoPayeeAccountStatus.Insert();

                        //Need to set the benefit end date as the last day of the month of Date of death
                        busPayeeAccount lbusPayeeAccount = new busPayeeAccount();
                        lbusPayeeAccount.FindPayeeAccount(Convert.ToInt32(ldrPayeeAccountId["PAYEE_ACCOUNT_ID"]));

                        if (this.icdoDeathNotification.date_of_death.Day == 1)
                        {
                            lbusPayeeAccount.icdoPayeeAccount.benefit_end_date = this.icdoDeathNotification.date_of_death.GetLastDayofMonth();
                        }
                        else
                        {
                            lbusPayeeAccount.icdoPayeeAccount.benefit_end_date = this.icdoDeathNotification.date_of_death.GetLastDayofMonth();
                        }

                        lbusPayeeAccount.icdoPayeeAccount.Update();
                    }
                    //PIR-893
                    DataTable ldtbAlternatePayeeAccountIDs = Select("cdoPayeeAccount.GetAlternatePayeeAccountStatusForDeathNotification", new object[1] { this.icdoDeathNotification.person_id });
                    if (ldtbAlternatePayeeAccountIDs.Rows.Count > 0)
                    {
                        foreach (DataRow ldrAlternatePayeeAccountId in ldtbAlternatePayeeAccountIDs.Rows)
                        {
                            cdoPayeeAccountStatus lcdoPayeeAccountStatus = new cdoPayeeAccountStatus();
                            lcdoPayeeAccountStatus.payee_account_id = Convert.ToInt32(ldrAlternatePayeeAccountId["PAYEE_ACCOUNT_ID"]);
                            lcdoPayeeAccountStatus.status_value = busConstant.PAYEE_ACCOUNT_STATUS_COMPLETED;
                            lcdoPayeeAccountStatus.terminated_status_reason_value = "OTHR";
                            lcdoPayeeAccountStatus.termination_reason_description = "Participant's Death";
                            lcdoPayeeAccountStatus.status_effective_date = DateTime.Now;
                            lcdoPayeeAccountStatus.Insert();

                            //Need to set the benefit end date as the last day of the month of Date of death
                            busPayeeAccount lbusPayeeAccount = new busPayeeAccount();
                            lbusPayeeAccount.FindPayeeAccount(Convert.ToInt32(ldrAlternatePayeeAccountId["PAYEE_ACCOUNT_ID"]));

                            if (this.icdoDeathNotification.date_of_death.Day == 1)
                            {
                                lbusPayeeAccount.icdoPayeeAccount.benefit_end_date = this.icdoDeathNotification.date_of_death.GetLastDayofMonth();
                            }
                            else
                            {
                                lbusPayeeAccount.icdoPayeeAccount.benefit_end_date = this.icdoDeathNotification.date_of_death.GetLastDayofMonth();
                            }

                            lbusPayeeAccount.icdoPayeeAccount.Update();
                        }

                    }
                }



                //foreach (busRelationship lbusRelationship in this.iclbDependentOf)
                //{
                //    lbusRelationship.icdoRelationship.effective_end_date = this.icdoDeathNotification.date_of_death;
                //    lbusRelationship.icdoRelationship.Update();
                //}

                //Commented because we need to send Date Of Death on Hit of Save - PIR 1039 
                //UpdateHEDB(busConstant.NOTIFICATION_STATUS_CERTIFIED);

                //OPUS must start Pop Up payee account workflow - On death notification certification, if person is a Joint annuitant to a Retiree
                DataTable ldtblJointAnnuitantOfRetiree = Select("cdoDeathNotification.GetJointAnnuitantOfRetiree", new object[1] { this.icdoDeathNotification.person_id });
                if (ldtblJointAnnuitantOfRetiree.Rows.Count > 0)
                {
                    if (Convert.ToString(ldtblJointAnnuitantOfRetiree.Rows[0][0]).IsNotNullOrEmpty() && Convert.ToString(ldtblJointAnnuitantOfRetiree.Rows[0][1]).IsNotNullOrEmpty())
                    {
                        //busWorkflowHelper.InitializeWorkflowIfNotExists(busConstant.POP_UP_PAYEE_ACCOUNT_WORKFLOW_NAME, Convert.ToInt32(ldtblJointAnnuitantOfRetiree.Rows[0][0]), 0, Convert.ToInt32(ldtblJointAnnuitantOfRetiree.Rows[0][1]), null);
                        //PIR 521 fix
                        busWorkflowHelper.InitializeWorkflow(busConstant.POP_UP_PAYEE_ACCOUNT_WORKFLOW_NAME, Convert.ToInt32(ldtblJointAnnuitantOfRetiree.Rows[0][0]), 0,
                            Convert.ToInt32(ldtblJointAnnuitantOfRetiree.Rows[0][1]), null);
                    }
                }


                //In case of DRO if Alternate payee Pre deceases participant and Eligible for continuance flag is not checked than benefits will get reverted back to 
                //participant else Post Retirement workflow will get initiated for Alternate payee,and Benefits will be given to Beneficiary of Alternate payee
                DataTable ldtblAlternatePayeePayeeAccounts = Select("cdoDroApplication.GetAlternatePayeesPayeeAccounts",
                    new object[1] { icdoDeathNotification.person_id });

                if (ldtblAlternatePayeePayeeAccounts.Rows.Count > 0)
                {
                    foreach (DataRow ldrAlternatePayeePayeeAccounts in ldtblAlternatePayeePayeeAccounts.Rows)
                    {
                        if (Convert.ToString(ldrAlternatePayeePayeeAccounts[enmDroApplication.eligible_for_continuance_flag.ToString().ToUpper()]).IsNullOrEmpty() ||
                            Convert.ToString(ldrAlternatePayeePayeeAccounts[enmDroApplication.eligible_for_continuance_flag.ToString().ToUpper()]) != busConstant.FLAG_YES)
                        {
                            DataTable ldtblPayeeAccountId = Select("cdoDroApplication.GetRTMT&WDRL&DISLPayeeAccountOfParticipant", new object[1] { Convert.ToInt32(ldrAlternatePayeePayeeAccounts[enmPayeeBenefitAccount.person_id.ToString().ToUpper()]) });

                            if (Convert.ToString(ldrAlternatePayeePayeeAccounts[enmPayeeAccount.benefit_account_type_value.ToString().ToUpper()]).IsNotNullOrEmpty() &&
                                Convert.ToString(ldrAlternatePayeePayeeAccounts[enmPayeeAccount.benefit_account_type_value.ToString().ToUpper()]) == busConstant.BENEFIT_TYPE_QDRO)
                            {
                                if (ldtblPayeeAccountId != null && ldtblPayeeAccountId.Rows.Count > 0)
                                {
                                    foreach (DataRow ldrPayeeAcctId in ldtblPayeeAccountId.Rows)
                                    {
                                        if (Convert.ToString(ldrPayeeAcctId[enmPayeeAccount.benefit_account_type_value.ToString().ToUpper()]) == busConstant.BENEFIT_TYPE_RETIREMENT
                                            || Convert.ToString(ldrPayeeAcctId[enmPayeeAccount.benefit_account_type_value.ToString().ToUpper()]) == busConstant.BENEFIT_TYPE_DISABILITY)
                                        {

                                            DataTable ldtblPaymentsAfterDateOfDeath = Select("cdoDeathNotification.GetPaymentsAfterDateOfDeath", new object[3] { icdoDeathNotification.person_id,
                                             icdoDeathNotification.date_of_death,DateTime.Now });

                                            if (ldtblPaymentsAfterDateOfDeath != null && ldtblPaymentsAfterDateOfDeath.Rows.Count > 0)
                                            {
                                                foreach (DataRow ldrPayments in ldtblPaymentsAfterDateOfDeath.Rows)
                                                {
                                                    int lintPayeeAccountId = 0;
                                                    if (Convert.ToString(ldrPayments[enmPaymentHistoryHeader.payee_account_id.ToString().ToUpper()]).IsNotNullOrEmpty())
                                                        lintPayeeAccountId = Convert.ToInt32(ldrPayments[enmPaymentHistoryHeader.payee_account_id.ToString().ToUpper()]);
                                                    ///Code to initiate new workflow
                                                    Hashtable lhstRequestParams = new Hashtable();
                                                    lhstRequestParams.Add("PayeeAccountId", lintPayeeAccountId);
                                                    if (icdoDeathNotification.date_of_death.Day == 1)
                                                    {
                                                        lhstRequestParams.Add("PaymentDateFrom", string.Format("{0:MM/dd/yyyy}", icdoDeathNotification.date_of_death.AddMonths(1)));
                                                    }
                                                    else
                                                    {
                                                        lhstRequestParams.Add("PaymentDateFrom", string.Format("{0:MM/dd/yyyy}", icdoDeathNotification.date_of_death));
                                                    }

                                                    lhstRequestParams.Add("PaymentDateTo", string.Format("{0:MM/dd/yyyy}", DateTime.Now));

                                                    busWorkflowHelper.InitializeWorkflow(busConstant.PROCESS_STOP_REISSUE_OR_RECLAMATION, icdoDeathNotification.person_id,
                                                                                                0, 0, lhstRequestParams);
                                                }
                                            }

                                            //PROD PIR 798
                                            if (Convert.ToString(ldrAlternatePayeePayeeAccounts[enmDroBenefitDetails.dro_model_value.ToString().ToUpper()]) != "STAF")
                                            {
                                                UpdateParticipantsPayeeAccount(Convert.ToInt32(ldrPayeeAcctId[enmPayeeAccount.payee_account_id.ToString().ToUpper()]),
                                                    Convert.ToInt32(ldrAlternatePayeePayeeAccounts[enmPayeeAccount.payee_account_id.ToString().ToUpper()]));
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (ldtblPayeeAccountId.Rows.Count > 0)
                                {
                                    foreach (DataRow ldrPayeeAcctId in ldtblPayeeAccountId.Rows)
                                    {
                                        if (Convert.ToString(ldrPayeeAcctId[enmPayeeAccount.benefit_account_type_value.ToString().ToUpper()]) == busConstant.BENEFIT_TYPE_WITHDRAWAL)
                                        {
                                            UpdateParticipantsPayeeAccount(Convert.ToInt32(ldrPayeeAcctId[enmPayeeAccount.payee_account_id.ToString().ToUpper()]),
                                                Convert.ToInt32(ldrAlternatePayeePayeeAccounts[enmPayeeAccount.payee_account_id.ToString().ToUpper()]));
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            bool lblnIsTermEnd = false;

                            if (Convert.ToString(ldrAlternatePayeePayeeAccounts[enmPayeeAccount.term_certain_end_date.ToString().ToUpper()]).IsNotNullOrEmpty()
                                && icdoDeathNotification.date_of_death >= Convert.ToDateTime(ldrAlternatePayeePayeeAccounts[enmPayeeAccount.term_certain_end_date.ToString().ToUpper()]))
                            {
                                lblnIsTermEnd = true;
                            }

                            if (!lblnIsTermEnd)
                                busWorkflowHelper.InitializeWorkflowIfNotExists(busConstant.POSTRETIREMENT_DEATH_WORKFLOW_NAME, this.icdoDeathNotification.person_id, 0, Convert.ToInt32(ldrAlternatePayeePayeeAccounts[enmPayeeAccount.payee_account_id.ToString().ToUpper()]), null);
                        }
                    }
                }



                //initiate WF only if Person is Participant
                int lintCheckParticipant = (int)DBFunction.DBExecuteScalar("cdoDeathNotification.GetParticipantCount", new object[1] { this.icdoDeathNotification.person_id },
                                                            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);


                //Start Post Retirement Death Workflow
                DataTable ldtbenficiaryPayee = busBase.Select("cdoDeathNotification.GetbenficiaryPayeePostRetr", new object[1] { icdoDeathNotification.person_id });

                if (ldtbenficiaryPayee.Rows.Count > 0)
                {
                    foreach (DataRow ldrPayee in ldtbenficiaryPayee.AsEnumerable())
                    {
                        busPayeeAccount lbusDeadPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                        lbusDeadPayeeAccount.icdoPayeeAccount.LoadData(ldrPayee);
                        DivideBenefitsIntoBeneficiary(lbusDeadPayeeAccount.icdoPayeeAccount.payee_account_id, lbusDeadPayeeAccount);
                    }
                }
                else
                {
                    DataTable ldtblRetirementDate = Select("cdoDeathNotification.GetRetirementDate", new object[1] { this.icdoDeathNotification.person_id });
                    if (ldtblRetirementDate.Rows.Count > 0)
                    {
                        foreach (DataRow ldrRetirementDate in ldtblRetirementDate.Rows)
                        {
                            if (ldtbPayeeAccountIDs.Rows.Count > 0)
                            {
                                foreach (DataRow ldrPayeeAccountId in ldtbPayeeAccountIDs.Rows)
                                {
                                    if (Convert.ToString(ldrPayeeAccountId[enmPayeeAccount.plan_benefit_id.ToString().ToUpper()]).IsNotNullOrEmpty() &&
                                        Convert.ToString(ldrRetirementDate[enmBenefitApplicationDetail.plan_benefit_id.ToString().ToUpper()]).IsNotNullOrEmpty() &&
                                        Convert.ToInt32(ldrPayeeAccountId[enmPayeeAccount.plan_benefit_id.ToString().ToUpper()])
                                        == Convert.ToInt32(ldrRetirementDate[enmBenefitApplicationDetail.plan_benefit_id.ToString().ToUpper()])
                                        && Convert.ToString(ldrPayeeAccountId[enmPayeeAccount.benefit_application_detail_id.ToString().ToUpper()]).IsNotNullOrEmpty()
                                         && Convert.ToInt32(ldrPayeeAccountId[enmPayeeAccount.benefit_application_detail_id.ToString().ToUpper()]) ==
                                        Convert.ToInt32(ldrRetirementDate[enmBenefitApplicationDetail.benefit_application_detail_id.ToString().ToUpper()]))
                                    {
                                        if (this.icdoDeathNotification.date_of_death > Convert.ToDateTime(ldrRetirementDate[enmBenefitApplication.retirement_date.ToString().ToUpper()]))
                                        {
                                            bool lblnIsTermEnd = false;
                                            if (Convert.ToString(ldrPayeeAccountId[enmPayeeAccount.term_certain_end_date.ToString().ToUpper()]).IsNotNullOrEmpty()
                                                  && icdoDeathNotification.date_of_death >= Convert.ToDateTime(ldrPayeeAccountId[enmPayeeAccount.term_certain_end_date.ToString().ToUpper()]))
                                            {
                                                lblnIsTermEnd = true;
                                            }

                                            if (!lblnIsTermEnd)
                                                busWorkflowHelper.InitializeWorkflowIfNotExists(busConstant.POSTRETIREMENT_DEATH_WORKFLOW_NAME, this.icdoDeathNotification.person_id, 0, Convert.ToInt32(ldrPayeeAccountId["PAYEE_ACCOUNT_ID"]), null);
                                        }
                                        else
                                        {
                                            //When Death Notification Gets SAVED we start DEATH PRE-RETIREMENT WORKFLOW 
                                            busWorkflowHelper.InitializeWorkflowIfNotExists(busConstant.PRERETIREMENT_DEATH_WORKFLOW_NAME, this.ibusPerson.icdoPerson.person_id, 0, 0, null);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //When Death Notification Gets SAVED we start DEATH PRE-RETIREMENT WORKFLOW 
                                busWorkflowHelper.InitializeWorkflowIfNotExists(busConstant.PRERETIREMENT_DEATH_WORKFLOW_NAME, this.ibusPerson.icdoPerson.person_id, 0, 0, null);
                            }
                        }

                    }
                }



                if (lintCheckParticipant > 0)
                {
                    DataTable ldtblPaymentsAfterDateOfDeath = Select("cdoDeathNotification.GetPaymentsAfterDateOfDeath", new object[3] { icdoDeathNotification.person_id,
                    icdoDeathNotification.date_of_death,DateTime.Now });

                    if (ldtblPaymentsAfterDateOfDeath != null && ldtblPaymentsAfterDateOfDeath.Rows.Count > 0)
                    {
                        foreach (DataRow ldrPayments in ldtblPaymentsAfterDateOfDeath.Rows)
                        {
                            int lintPayeeAccountId = 0;
                            if (Convert.ToString(ldrPayments[enmPaymentHistoryHeader.payee_account_id.ToString().ToUpper()]).IsNotNullOrEmpty())
                                lintPayeeAccountId = Convert.ToInt32(ldrPayments[enmPaymentHistoryHeader.payee_account_id.ToString().ToUpper()]);
                            ///Code to initiate new workflow
                            Hashtable lhstRequestParams = new Hashtable();
                            lhstRequestParams.Add("PayeeAccountId", lintPayeeAccountId);

                            if (icdoDeathNotification.date_of_death.Day == 1)
                            {
                                lhstRequestParams.Add("PaymentDateFrom", string.Format("{0:MM/dd/yyyy}", icdoDeathNotification.date_of_death.AddMonths(1)));
                            }
                            else
                            {
                                lhstRequestParams.Add("PaymentDateFrom", string.Format("{0:MM/dd/yyyy}", icdoDeathNotification.date_of_death));
                            }

                            lhstRequestParams.Add("PaymentDateTo", string.Format("{0:MM/dd/yyyy}", DateTime.Now));

                            busWorkflowHelper.InitializeWorkflow(busConstant.PROCESS_STOP_REISSUE_OR_RECLAMATION, icdoDeathNotification.person_id,
                                                                        0, 0, lhstRequestParams);
                        }
                    }

                    //DataTable ldtblRetirementDate = Select("cdoDeathNotification.GetRetirementDate", new object[1] { this.icdoDeathNotification.person_id });
                    //if (ldtblRetirementDate.Rows.Count > 0)
                    //{
                    //    if (this.icdoDeathNotification.date_of_death > Convert.ToDateTime(ldtblRetirementDate.Rows[0][0]))
                    //    {
                    //        lblnIsInitiatePreRtmtWorkFlow = false;
                    //        if (ldtbPayeeAccountIDs.Rows.Count > 0)
                    //        {
                    //            foreach (DataRow ldrPayeeAccountId in ldtbPayeeAccountIDs.Rows)
                    //            {
                    //                busWorkflowHelper.InitializeWorkflowIfNotExists(busConstant.POSTRETIREMENT_DEATH_WORKFLOW_NAME, this.icdoDeathNotification.person_id, 0, Convert.ToInt32(ldrPayeeAccountId["PAYEE_ACCOUNT_ID"]), null);
                    //            }
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    //DataTable ldtbenficiaryPayee = busBase.Select("cdoDeathNotification.GetbenficiaryPayeePostRetr", new object[1] { icdoDeathNotification.person_id });

                    //    //if (ldtbenficiaryPayee.Rows.Count > 0)
                    //    //{
                    //    //    foreach (DataRow ldrPayee in ldtbenficiaryPayee.AsEnumerable())
                    //    //    {
                    //    //        busPayeeAccount lbusDeadPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                    //    //        lbusDeadPayeeAccount.icdoPayeeAccount.LoadData(ldrPayee);
                    //    //        DevideBenifitIntoBeneficiary(lbusDeadPayeeAccount.icdoPayeeAccount.payee_account_id, lbusDeadPayeeAccount);
                    //    //    }
                    //    //}
                    //}

                    //if (lblnIsInitiatePreRtmtWorkFlow && lintCheckParticipant > 0)
                    //{
                    //    //When Death Notification Gets SAVED we start DEATH PRE-RETIREMENT WORKFLOW 
                    //    busWorkflowHelper.InitializeWorkflowIfNotExists(busConstant.PRERETIREMENT_DEATH_WORKFLOW_NAME, this.ibusPerson.icdoPerson.person_id, 0, 0, null);
                    //}
                }
                AuditLogHistoryForPerson(this.icdoDeathNotification.date_of_death.ToString());
                return larrList;
            }
            EvaluateInitialLoadRules();
            AuditLogHistoryForPerson(this.icdoDeathNotification.date_of_death.ToString());

            //When Death Notification Gets SAVED we start DEATH PRE-RETIREMENT WORKFLOW 
            //busWorkflowHelper.InitializeWorkflowIfNotExists(busConstant.PRERETIREMENT_DEATH_WORKFLOW_NAME, this.ibusPerson.icdoPerson.person_id, 0, 0, null);
            return larrList;
        }


        #region If Participant dies and has shared DRO thn AP's benefits will get revert back to participant if not eligible for continuance
        //Shared Interest DRO : death for a participant who has an Alternate Payee we need to set the Alternate Payee's account to Payments Complete
        //if the flag Eligible for Continuance is not checked. Once the Alternate Payees account is set to Payments
        //Complete the deceased participants benefit will revert back to the full amount and no longer be 
        //offset by the QDRO offset being paid to the Alternate Payee. This will allow for the Survivor
        //to receive the appropriate amount of benefit 
        public void RevertAlternatePayeeSharedBenefitsToParticipant()
        {
            DataTable ldtbPayeeAccountsWithJSOption = Select("cdoDroApplication.GetParticipantsPayeeAccountWithJSOptions", new object[1] { this.icdoDeathNotification.person_id });
            if (ldtbPayeeAccountsWithJSOption != null && ldtbPayeeAccountsWithJSOption.Rows.Count > 0)
            {
                if (Convert.ToString(ldtbPayeeAccountsWithJSOption.Rows[0][enmPayeeAccount.payee_account_id.ToString().ToUpper()]).IsNotNullOrEmpty())
                {
                    int lintJointAnnuitantId = Convert.ToInt32(ldtbPayeeAccountsWithJSOption.Rows[0][enmBenefitApplicationDetail.joint_annuitant_id.ToString().ToUpper()]);
                    int lintParticipantsPayeeAccountId = Convert.ToInt32(ldtbPayeeAccountsWithJSOption.Rows[0][enmPayeeAccount.payee_account_id.ToString().ToUpper()]);

                    busPayeeAccount lbusParticipantPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                    lbusParticipantPayeeAccount.FindPayeeAccount(lintParticipantsPayeeAccountId);

                    decimal ldecParticipantPayeeTotalTaxableAmount = 0M;
                    decimal ldecParticipantTotalNonTaxableAmount = 0M;
                    decimal ldecParticipantTotalRemainingMG = 0M;
                    decimal ldecParticipantTotalNonTaxableBeginningBalance = 0M;


                    lbusParticipantPayeeAccount.LoadBenefitDetails();
                    lbusParticipantPayeeAccount.LoadNextBenefitPaymentDate();


                    lbusParticipantPayeeAccount.LoadPaymentItemType();
                    lbusParticipantPayeeAccount.LoadBenefitDetails();
                    lbusParticipantPayeeAccount.LoadPayeeAccountPaymentItemType();
                    lbusParticipantPayeeAccount.LoadNextBenefitPaymentDate();
                    lbusParticipantPayeeAccount.iclbPayeeAccountPaymentItemTypeActive = (from item in lbusParticipantPayeeAccount.iclbPayeeAccountPaymentItemType
                                                                                         where busGlobalFunctions.CheckDateOverlapping(lbusParticipantPayeeAccount.idtNextBenefitPaymentDate,
                                                             item.icdoPayeeAccountPaymentItemType.start_date, item.icdoPayeeAccountPaymentItemType.end_date)
                                                                                         select item).ToList().ToCollection<busPayeeAccountPaymentItemType>();

                    ldecParticipantTotalNonTaxableBeginningBalance += lbusParticipantPayeeAccount.GetRemainingNonTaxableBeginningBalanaceTillDate(this.icdoDeathNotification.date_of_death);
                    ldecParticipantTotalRemainingMG += lbusParticipantPayeeAccount.GetRemainingMinimumGuaranteeTillDate(this.icdoDeathNotification.date_of_death);

                    if (lbusParticipantPayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Where(item => item.icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                     lbusParticipantPayeeAccount.iclbPaymentItemType.Where(t => t.icdoPaymentItemType.item_type_code == busConstant.ITEM1).FirstOrDefault().icdoPaymentItemType.payment_item_type_id).Count() > 0)
                    {
                        ldecParticipantPayeeTotalTaxableAmount = lbusParticipantPayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Where(item => item.icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                            lbusParticipantPayeeAccount.iclbPaymentItemType.Where(t => t.icdoPaymentItemType.item_type_code == busConstant.ITEM1).FirstOrDefault().icdoPaymentItemType.payment_item_type_id).FirstOrDefault().icdoPayeeAccountPaymentItemType.amount;

                    }

                    if (lbusParticipantPayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Where(item => item.icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                        lbusParticipantPayeeAccount.iclbPaymentItemType.Where(t => t.icdoPaymentItemType.item_type_code == busConstant.ITEM2).FirstOrDefault().icdoPaymentItemType.payment_item_type_id).Count() > 0)
                    {
                        ldecParticipantTotalNonTaxableAmount = lbusParticipantPayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Where(item => item.icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                            lbusParticipantPayeeAccount.iclbPaymentItemType.Where(t => t.icdoPaymentItemType.item_type_code == busConstant.ITEM2).FirstOrDefault().icdoPaymentItemType.payment_item_type_id).FirstOrDefault().icdoPayeeAccountPaymentItemType.amount;

                    }

                    decimal ldecAlternatePayeeTotalTaxableAmount = 0M;
                    decimal ldecAlternatePayeeTotalNonTaxableAmount = 0M;
                    decimal ldecAPTotalRemainingMG = 0M;
                    decimal ldecAPTotalNonTaxableBeginningBalance = 0M;

                    DataTable ldtbParticipantsDROPayeeAccounts = Select("cdoDroApplication.GetParticipantsDROPayeeAccounts", new object[1] { this.icdoDeathNotification.person_id });
                    if (ldtbParticipantsDROPayeeAccounts != null && ldtbParticipantsDROPayeeAccounts.Rows.Count > 0)
                    {
                        Collection<busPayeeAccount> lclbAlternatePayeeAccounts = new Collection<busPayeeAccount>();
                        lclbAlternatePayeeAccounts = GetCollection<busPayeeAccount>(ldtbParticipantsDROPayeeAccounts, "icdoPayeeAccount");

                        foreach (busPayeeAccount lbusAlternatePayeeAccount in lclbAlternatePayeeAccounts)
                        {

                            lbusAlternatePayeeAccount.LoadBenefitDetails();
                            lbusAlternatePayeeAccount.LoadNextBenefitPaymentDate();

                            lbusAlternatePayeeAccount.LoadPaymentItemType();
                            lbusAlternatePayeeAccount.LoadBenefitDetails();
                            lbusAlternatePayeeAccount.LoadPayeeAccountPaymentItemType();
                            lbusAlternatePayeeAccount.LoadNextBenefitPaymentDate();
                            lbusAlternatePayeeAccount.iclbPayeeAccountPaymentItemTypeActive = (from item in lbusAlternatePayeeAccount.iclbPayeeAccountPaymentItemType
                                                                                               where busGlobalFunctions.CheckDateOverlapping(lbusAlternatePayeeAccount.idtNextBenefitPaymentDate,
                                                                                             item.icdoPayeeAccountPaymentItemType.start_date, item.icdoPayeeAccountPaymentItemType.end_date)
                                                                                               select item).ToList().ToCollection<busPayeeAccountPaymentItemType>();

                            ldecAPTotalNonTaxableBeginningBalance += lbusAlternatePayeeAccount.GetRemainingNonTaxableBeginningBalanaceTillDate(this.icdoDeathNotification.date_of_death);
                            ldecAPTotalRemainingMG += lbusAlternatePayeeAccount.GetRemainingMinimumGuaranteeTillDate(this.icdoDeathNotification.date_of_death);

                            if (lbusAlternatePayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Where(item => item.icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                                    lbusAlternatePayeeAccount.iclbPaymentItemType.Where(t => t.icdoPaymentItemType.item_type_code == busConstant.ITEM1).FirstOrDefault().icdoPaymentItemType.payment_item_type_id).Count() > 0)
                            {
                                ldecAlternatePayeeTotalTaxableAmount += lbusAlternatePayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Where(item => item.icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                                    lbusAlternatePayeeAccount.iclbPaymentItemType.Where(t => t.icdoPaymentItemType.item_type_code == busConstant.ITEM1).FirstOrDefault().icdoPaymentItemType.payment_item_type_id).FirstOrDefault().icdoPayeeAccountPaymentItemType.amount;
                            }


                            if (lbusAlternatePayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Where(item => item.icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                                   lbusAlternatePayeeAccount.iclbPaymentItemType.Where(t => t.icdoPaymentItemType.item_type_code == busConstant.ITEM2).FirstOrDefault().icdoPaymentItemType.payment_item_type_id).Count() > 0)
                            {
                                ldecAlternatePayeeTotalNonTaxableAmount += lbusAlternatePayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Where(item => item.icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                                    lbusAlternatePayeeAccount.iclbPaymentItemType.Where(t => t.icdoPaymentItemType.item_type_code == busConstant.ITEM2).FirstOrDefault().icdoPaymentItemType.payment_item_type_id).FirstOrDefault().icdoPayeeAccountPaymentItemType.amount;
                            }


                            cdoPayeeAccountStatus lcdoPayeeAccountStatus = new cdoPayeeAccountStatus();
                            lcdoPayeeAccountStatus.payee_account_id = Convert.ToInt32(lbusAlternatePayeeAccount.icdoPayeeAccount.payee_account_id);
                            lcdoPayeeAccountStatus.status_value = busConstant.PAYEE_ACCOUNT_STATUS_COMPLETED;
                            lcdoPayeeAccountStatus.status_effective_date = DateTime.Now;
                            lcdoPayeeAccountStatus.Insert();


                            if (lintJointAnnuitantId == lbusAlternatePayeeAccount.icdoPayeeAccount.person_id)
                            {
                                DataTable ldtbExSpouseForGivenPlan = Select("cdoDroApplication.GetExSpouseForGivenPlan", new object[3] { this.icdoDeathNotification.person_id, lbusAlternatePayeeAccount.icdoPayeeAccount.person_id, lbusParticipantPayeeAccount.icdoPayeeAccount.iintPlanId });
                                if (ldtbExSpouseForGivenPlan != null && ldtbExSpouseForGivenPlan.Rows.Count > 0)
                                {
                                    foreach (DataRow ldrExSpouseForGivenPlan in ldtbExSpouseForGivenPlan.Rows)
                                    {
                                        if (Convert.ToString(ldrExSpouseForGivenPlan[enmRelationship.person_relationship_id.ToString().ToUpper()]).IsNotNullOrEmpty())
                                        {
                                            busRelationship lbusRelationship = new busRelationship { icdoRelationship = new cdoRelationship() };
                                            lbusRelationship.icdoRelationship.LoadData(ldrExSpouseForGivenPlan);
                                            lbusRelationship.icdoRelationship.relationship_value = busConstant.BENEFICIARY_RELATIONSHIP_SPOUSE;
                                            lbusRelationship.icdoRelationship.Update();
                                        }
                                    }
                                }
                            }
                        }

                        //Create Payee Account Minimum Guarantee History
                        if (lbusParticipantPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance > 0)
                            lbusParticipantPayeeAccount.CreatePayeeAccountMinimumGuaranteeHistory(lbusParticipantPayeeAccount.icdoPayeeAccount.payee_account_id, lbusParticipantPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount,
                                lbusParticipantPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance, this.icdoDeathNotification.date_of_death);
                        else
                            lbusParticipantPayeeAccount.CreatePayeeAccountMinimumGuaranteeHistory(lbusParticipantPayeeAccount.icdoPayeeAccount.payee_account_id, lbusParticipantPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount,
                          lbusParticipantPayeeAccount.icdoPayeeAccount.remaining_non_taxable_from_conversion, this.icdoDeathNotification.date_of_death);


                        if (lbusParticipantPayeeAccount.icdoPayeeAccount.iintPlanId == busConstant.MPIPP_PLAN_ID)
                        {
                            //Minimum Gaurentee Amount
                            lbusParticipantPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount =
                                 ldecParticipantTotalRemainingMG + ldecAPTotalRemainingMG;

                            //Non Taxable Beginning Balance
                            if (lbusParticipantPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance > 0)
                                lbusParticipantPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance =
                                    ldecParticipantTotalNonTaxableBeginningBalance + ldecAPTotalNonTaxableBeginningBalance;
                            else
                                lbusParticipantPayeeAccount.icdoPayeeAccount.remaining_non_taxable_from_conversion =
                                    ldecParticipantTotalNonTaxableBeginningBalance + ldecAPTotalNonTaxableBeginningBalance;

                        }


                        if ((ldecParticipantPayeeTotalTaxableAmount + ldecAlternatePayeeTotalTaxableAmount) > 0)
                        {
                            lbusParticipantPayeeAccount.CreatePayeeAccountPaymentItemType("ITEM1", ldecParticipantPayeeTotalTaxableAmount + ldecAlternatePayeeTotalTaxableAmount, "0", 0,
                                lbusParticipantPayeeAccount.idtNextBenefitPaymentDate, DateTime.MinValue, "N", false);
                        }

                        if ((ldecParticipantTotalNonTaxableAmount + ldecAlternatePayeeTotalNonTaxableAmount) > 0)
                        {
                            lbusParticipantPayeeAccount.CreatePayeeAccountPaymentItemType("ITEM2", ldecParticipantTotalNonTaxableAmount + ldecAlternatePayeeTotalNonTaxableAmount, "0", 0,
                             lbusParticipantPayeeAccount.idtNextBenefitPaymentDate, DateTime.MinValue, "N", false);
                        }


                        lbusParticipantPayeeAccount.icdoPayeeAccount.Update();

                    }
                }
            }

        }

        #endregion

        public void UpdateParticipantsPayeeAccount(int aintParticipantPayeeAccountId, int aintAlternatePayeeAccountId)
        {
            busPayeeAccount lbusParticipantPayeeAccount = new busPayeeAccount();
            busPayeeAccount lbusAlternatePayeeAccount = new busPayeeAccount();

            lbusParticipantPayeeAccount.FindPayeeAccount(aintParticipantPayeeAccountId);
            lbusAlternatePayeeAccount.FindPayeeAccount(aintAlternatePayeeAccountId);

            lbusParticipantPayeeAccount.LoadBenefitDetails();
            lbusAlternatePayeeAccount.LoadBenefitDetails();


            lbusParticipantPayeeAccount.LoadNextBenefitPaymentDate();
            lbusAlternatePayeeAccount.LoadNextBenefitPaymentDate();


            //Create Payee Account Minimum Guarantee History
            if (lbusParticipantPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance > 0)
                lbusParticipantPayeeAccount.CreatePayeeAccountMinimumGuaranteeHistory(lbusParticipantPayeeAccount.icdoPayeeAccount.payee_account_id, lbusParticipantPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount,
                    lbusParticipantPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance, this.icdoDeathNotification.date_of_death);
            else
                lbusParticipantPayeeAccount.CreatePayeeAccountMinimumGuaranteeHistory(lbusParticipantPayeeAccount.icdoPayeeAccount.payee_account_id, lbusParticipantPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount,
              lbusParticipantPayeeAccount.icdoPayeeAccount.remaining_non_taxable_from_conversion, this.icdoDeathNotification.date_of_death);


            //Minimum Gaurentee Amount
            if (lbusParticipantPayeeAccount.icdoPayeeAccount.iintPlanId == busConstant.MPIPP_PLAN_ID)
            {
                lbusParticipantPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount =
                     lbusParticipantPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount + lbusAlternatePayeeAccount.GetRemainingMinimumGuaranteeTillDate(icdoDeathNotification.date_of_death);

                //Non Taxable Beginning Balance
                if (lbusParticipantPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance > 0)
                    lbusParticipantPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance =
                        lbusParticipantPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance + lbusAlternatePayeeAccount.GetRemainingNonTaxableBeginningBalanaceTillDate(icdoDeathNotification.date_of_death);
                else
                    lbusParticipantPayeeAccount.icdoPayeeAccount.remaining_non_taxable_from_conversion =
                   lbusParticipantPayeeAccount.icdoPayeeAccount.remaining_non_taxable_from_conversion + lbusAlternatePayeeAccount.GetRemainingNonTaxableBeginningBalanaceTillDate(icdoDeathNotification.date_of_death);

            }

            lbusParticipantPayeeAccount.LoadPaymentItemType();
            lbusParticipantPayeeAccount.LoadBenefitDetails();
            lbusParticipantPayeeAccount.LoadPayeeAccountPaymentItemType();
            lbusParticipantPayeeAccount.LoadNextBenefitPaymentDate();
            lbusParticipantPayeeAccount.iclbPayeeAccountPaymentItemTypeActive = (from item in lbusParticipantPayeeAccount.iclbPayeeAccountPaymentItemType
                                                                                 where busGlobalFunctions.CheckDateOverlapping(lbusParticipantPayeeAccount.idtNextBenefitPaymentDate,
                                                     item.icdoPayeeAccountPaymentItemType.start_date, item.icdoPayeeAccountPaymentItemType.end_date)
                                                                                 select item).ToList().ToCollection<busPayeeAccountPaymentItemType>();


            lbusAlternatePayeeAccount.LoadPaymentItemType();
            lbusAlternatePayeeAccount.LoadBenefitDetails();
            lbusAlternatePayeeAccount.LoadPayeeAccountPaymentItemType();
            lbusAlternatePayeeAccount.LoadNextBenefitPaymentDate();
            lbusAlternatePayeeAccount.iclbPayeeAccountPaymentItemTypeActive = (from item in lbusAlternatePayeeAccount.iclbPayeeAccountPaymentItemType
                                                                               where busGlobalFunctions.CheckDateOverlapping(lbusAlternatePayeeAccount.idtNextBenefitPaymentDate,
                                                   item.icdoPayeeAccountPaymentItemType.start_date, item.icdoPayeeAccountPaymentItemType.end_date)
                                                                               select item).ToList().ToCollection<busPayeeAccountPaymentItemType>();


            decimal ldecParticipantsTaxableAmt = 0M;
            decimal ldecParticipantsNonTaxableAmt = 0M;

            decimal ldecAlternatePayeesTaxableAmt = 0M;
            decimal ldecAlternatePayeesNonTaxableAmt = 0M;

            if (lbusParticipantPayeeAccount.icdoPayeeAccount.iintPlanId == busConstant.MPIPP_PLAN_ID)
            {
                if (lbusParticipantPayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Where(item => item.icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                     lbusParticipantPayeeAccount.iclbPaymentItemType.Where(t => t.icdoPaymentItemType.item_type_code == busConstant.ITEM1).FirstOrDefault().icdoPaymentItemType.payment_item_type_id).Count() > 0)
                {
                    ldecParticipantsTaxableAmt = lbusParticipantPayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Where(item => item.icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                        lbusParticipantPayeeAccount.iclbPaymentItemType.Where(t => t.icdoPaymentItemType.item_type_code == busConstant.ITEM1).FirstOrDefault().icdoPaymentItemType.payment_item_type_id).FirstOrDefault().icdoPayeeAccountPaymentItemType.amount;

                }

                if (lbusAlternatePayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Where(item => item.icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                        lbusAlternatePayeeAccount.iclbPaymentItemType.Where(t => t.icdoPaymentItemType.item_type_code == busConstant.ITEM1).FirstOrDefault().icdoPaymentItemType.payment_item_type_id).Count() > 0)
                {
                    ldecAlternatePayeesTaxableAmt = lbusAlternatePayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Where(item => item.icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                        lbusAlternatePayeeAccount.iclbPaymentItemType.Where(t => t.icdoPaymentItemType.item_type_code == busConstant.ITEM1).FirstOrDefault().icdoPaymentItemType.payment_item_type_id).FirstOrDefault().icdoPayeeAccountPaymentItemType.amount;
                }

                if ((ldecParticipantsTaxableAmt + ldecAlternatePayeesTaxableAmt) > 0)
                {
                    lbusParticipantPayeeAccount.CreatePayeeAccountPaymentItemType("ITEM1", ldecParticipantsTaxableAmt + ldecAlternatePayeesTaxableAmt, "0", 0,
                        lbusParticipantPayeeAccount.idtNextBenefitPaymentDate, DateTime.MinValue, "N", false);
                }


                if (lbusParticipantPayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Where(item => item.icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                    lbusParticipantPayeeAccount.iclbPaymentItemType.Where(t => t.icdoPaymentItemType.item_type_code == busConstant.ITEM2).FirstOrDefault().icdoPaymentItemType.payment_item_type_id).Count() > 0)
                {
                    ldecParticipantsNonTaxableAmt = lbusParticipantPayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Where(item => item.icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                        lbusParticipantPayeeAccount.iclbPaymentItemType.Where(t => t.icdoPaymentItemType.item_type_code == busConstant.ITEM2).FirstOrDefault().icdoPaymentItemType.payment_item_type_id).FirstOrDefault().icdoPayeeAccountPaymentItemType.amount;

                }


                if (lbusAlternatePayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Where(item => item.icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                       lbusAlternatePayeeAccount.iclbPaymentItemType.Where(t => t.icdoPaymentItemType.item_type_code == busConstant.ITEM2).FirstOrDefault().icdoPaymentItemType.payment_item_type_id).Count() > 0)
                {
                    ldecAlternatePayeesNonTaxableAmt = lbusAlternatePayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Where(item => item.icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                        lbusAlternatePayeeAccount.iclbPaymentItemType.Where(t => t.icdoPaymentItemType.item_type_code == busConstant.ITEM2).FirstOrDefault().icdoPaymentItemType.payment_item_type_id).FirstOrDefault().icdoPayeeAccountPaymentItemType.amount;
                }


                if ((ldecParticipantsNonTaxableAmt + ldecAlternatePayeesNonTaxableAmt) > 0)
                {
                    lbusParticipantPayeeAccount.CreatePayeeAccountPaymentItemType("ITEM2", ldecParticipantsNonTaxableAmt + ldecAlternatePayeesNonTaxableAmt, "0", 0,
                     lbusParticipantPayeeAccount.idtNextBenefitPaymentDate, DateTime.MinValue, "N", false);
                }


            }
            else
            {
                if (lbusParticipantPayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Where(item => item.icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                     lbusParticipantPayeeAccount.iclbPaymentItemType.Where(t => t.icdoPaymentItemType.item_type_code == busConstant.ITEM1).FirstOrDefault().icdoPaymentItemType.payment_item_type_id).Count() > 0)
                {
                    ldecParticipantsTaxableAmt = lbusParticipantPayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Where(item => item.icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                        lbusParticipantPayeeAccount.iclbPaymentItemType.Where(t => t.icdoPaymentItemType.item_type_code == busConstant.ITEM1).FirstOrDefault().icdoPaymentItemType.payment_item_type_id).FirstOrDefault().icdoPayeeAccountPaymentItemType.amount;

                }

                if (lbusAlternatePayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Where(item => item.icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                        lbusAlternatePayeeAccount.iclbPaymentItemType.Where(t => t.icdoPaymentItemType.item_type_code == busConstant.ITEM1).FirstOrDefault().icdoPaymentItemType.payment_item_type_id).Count() > 0)
                {
                    ldecAlternatePayeesTaxableAmt = lbusAlternatePayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Where(item => item.icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                        lbusAlternatePayeeAccount.iclbPaymentItemType.Where(t => t.icdoPaymentItemType.item_type_code == busConstant.ITEM1).FirstOrDefault().icdoPaymentItemType.payment_item_type_id).FirstOrDefault().icdoPayeeAccountPaymentItemType.amount;
                }


                if ((ldecParticipantsNonTaxableAmt + ldecAlternatePayeesNonTaxableAmt) > 0)
                {
                    lbusParticipantPayeeAccount.CreatePayeeAccountPaymentItemType("ITEM1", ldecParticipantsTaxableAmt + ldecAlternatePayeesTaxableAmt, "0", 0,
                            lbusParticipantPayeeAccount.idtNextBenefitPaymentDate, DateTime.MinValue, "N", false);
                }
            }
            //Ticket#73504
            lbusParticipantPayeeAccount.CreateReviewStatus();
            lbusParticipantPayeeAccount.icdoPayeeAccount.Update();

            busCalculation lbusCalculation = new busCalculation();
            Collection<busBenefitMonthwiseAdjustmentDetail> lclbBenefitMonthwiseAdjustmentDetail = new Collection<busBenefitMonthwiseAdjustmentDetail>();

            if (icdoDeathNotification.date_of_death.Day == 1)
            {
                lclbBenefitMonthwiseAdjustmentDetail = lbusCalculation.GetAmountActuallyPaid(lbusParticipantPayeeAccount, icdoDeathNotification.date_of_death.AddMonths(1), lbusParticipantPayeeAccount.idtLastBenefitPaymentDate);
            }
            else
            {
                lclbBenefitMonthwiseAdjustmentDetail = lbusCalculation.GetAmountActuallyPaid(lbusParticipantPayeeAccount, icdoDeathNotification.date_of_death.GetLastDayofMonth().AddDays(1), lbusParticipantPayeeAccount.idtLastBenefitPaymentDate);
            }

            lbusCalculation.CalculateAmountShouldHaveBeenPaid(lbusParticipantPayeeAccount, ref lclbBenefitMonthwiseAdjustmentDetail);

            lbusCalculation.CreateOverpaymentUnderPayment(lbusParticipantPayeeAccount, lclbBenefitMonthwiseAdjustmentDetail, busConstant.RETRO_PAYMENT_DEATH);

        }

        public ArrayList btn_IncorrectlyReported()
        {

            //Rohan - Eligibility PIR 806
            this.ibusPerson.icdoPerson.date_of_death = DateTime.MinValue;
            this.ibusPerson.icdoPerson.Update();

            ArrayList larrList = new ArrayList();
            if (this.icdoDeathNotification.death_notification_status_value == busConstant.NOTIFICATION_STATUS_CERTIFIED)
            {
                //foreach (busQdroApplication lbusQdroApplication in this.iclbDroApplicationDetails)
                //{
                //    if (lbusQdroApplication.icdoDroApplication.dro_status_value == busConstant.DRO_CANCELLED
                //        && lbusQdroApplication.icdoDroApplication.cancellation_reason_value == busConstant.CANCELLATION_REASON_DECEASED)
                //    {
                //        DataTable ldtblist = busPerson.Select("cdoDeathNotification.GetPreviousDroStatus", new object[1] { lbusQdroApplication.icdoDroApplication.dro_application_id });
                //        if (ldtblist.Rows.Count == 2)
                //        {
                //            lbusQdroApplication.icdoDroApplication.dro_status_value = Convert.ToString(ldtblist.Rows[1]["STATUS_VALUE"]);
                //            lbusQdroApplication.icdoDroApplication.cancellation_reason_value = "";
                //            lbusQdroApplication.icdoDroApplication.Update();
                //        }
                //    }
                //}

                foreach (busBenefitApplication lbusBenefitApplication in this.iclbBenefitApplicationDetails)
                {
                    if (lbusBenefitApplication.icdoBenefitApplication.application_status_value == busConstant.BENEFIT_APPL_CANCELLED
                        && lbusBenefitApplication.icdoBenefitApplication.cancellation_reason_value == busConstant.CANCELLATION_REASON_DECEASED)
                    {
                        DataTable ldtblist = busPerson.Select("cdoDeathNotification.GetPreviousApplStatus", new object[1] { lbusBenefitApplication.icdoBenefitApplication.benefit_application_id });
                        if (ldtblist.Rows.Count == 2)
                        {
                            lbusBenefitApplication.icdoBenefitApplication.application_status_value = Convert.ToString(ldtblist.Rows[1]["STATUS_VALUE"]);
                            lbusBenefitApplication.icdoBenefitApplication.cancellation_reason_value = "";

                            lbusBenefitApplication.icdoBenefitApplication.Update();
                        }
                    }
                }

                //this.ibusPerson.icdoPerson.date_of_death = DateTime.MinValue;
                //this.ibusPerson.icdoPerson.Update();

                AuditLogHistoryForPerson();

                foreach (busPersonAccount lbusPersonAccount in this.ibusPerson.iclbPersonAccount)
                {
                    lbusPersonAccount.icdoPersonAccount.status_value = busConstant.PERSON_ACCOUNT_STATUS_ACTIVE;
                    lbusPersonAccount.icdoPersonAccount.Update();
                }

                foreach (busPersonContact lbusPersonContact in this.ibusPerson.iclbPersonContact)
                {
                    lbusPersonContact.icdoPersonContact.effective_end_date = DateTime.MinValue;
                    lbusPersonContact.icdoPersonContact.Update();

                }
                foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in this.ibusPerson.iclbPersonAccountBeneficiary)
                {
                    lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date = DateTime.MinValue;
                    lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.Update();
                    lbusPersonAccountBeneficiary.ibusParticipant = this.ibusPerson;
                    lbusPersonAccountBeneficiary.iaintMainPersonID = lbusPersonAccountBeneficiary.ibusParticipant.icdoPerson.person_id;
                    lbusPersonAccountBeneficiary.ValidateSoftErrors();
                    lbusPersonAccountBeneficiary.UpdateValidateStatus();

                }

                //foreach (busRelationship lbusRelationship in this.ibusPerson.iclbPersonDependent)
                //{
                //    lbusRelationship.icdoRelationship.effective_end_date = DateTime.MinValue;
                //    lbusRelationship.icdoRelationship.Update();

                //}

                foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in this.iclbBeneficiaryOf)
                {
                    lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date = DateTime.MinValue;
                    lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.Update();
                    lbusPersonAccountBeneficiary.ibusParticipant = lbusPersonAccountBeneficiary.ibusPerson;
                    lbusPersonAccountBeneficiary.iaintMainPersonID = lbusPersonAccountBeneficiary.ibusPerson.icdoPerson.person_id;
                    lbusPersonAccountBeneficiary.ValidateSoftErrors();
                    lbusPersonAccountBeneficiary.UpdateValidateStatus();
                }

                //foreach (busRelationship lbusRelationship in this.iclbDependentOf)
                //{
                //    lbusRelationship.icdoRelationship.effective_end_date = DateTime.MinValue;
                //    lbusRelationship.icdoRelationship.Update();
                //}

                DataTable ldtbPreRetirementDeath = busPerson.Select("cdoDeathNotification.GetPreDeathRetirementApplication", new object[1] { this.icdoDeathNotification.person_id });
                if (ldtbPreRetirementDeath.Rows.Count > 0)
                {
                    Collection<busDeathPreRetirement> lclbDeathPreRetirement = new Collection<busDeathPreRetirement>();
                    lclbDeathPreRetirement = GetCollection<busDeathPreRetirement>(ldtbPreRetirementDeath, "icdoBenefitApplication");

                    foreach (busDeathPreRetirement lbusDeathPreRetirement in lclbDeathPreRetirement)
                    {
                        lbusDeathPreRetirement.icdoBenefitApplication.application_status_value = busConstant.BENEFIT_APPL_CANCELLED;
                        lbusDeathPreRetirement.icdoBenefitApplication.Update();
                    }
                }
            }

            //For Changing Payee Account status
            DataTable ldtbPayeeAccountIDs = Select("cdoPayeeAccount.GetPreviousStatusForDeathNotification", new object[1] { this.icdoDeathNotification.person_id });
            if (ldtbPayeeAccountIDs.Rows.Count > 0)
            {
                foreach (DataRow ldrPayeeAccountId in ldtbPayeeAccountIDs.Rows)
                {
                    cdoPayeeAccountStatus lcdoPayeeAccountStatus = new cdoPayeeAccountStatus();
                    lcdoPayeeAccountStatus.payee_account_id = Convert.ToInt32(ldrPayeeAccountId["PAYEE_ACCOUNT_ID"]);
                    lcdoPayeeAccountStatus.status_value = busConstant.PAYEE_ACCOUNT_STATUS_REVIEW;//Convert.ToString(ldrPayeeAccountId["STATUS_VALUE"]);
                    lcdoPayeeAccountStatus.status_effective_date = DateTime.Now;
                    lcdoPayeeAccountStatus.Insert();

                    //Payment Adjustments
                    CreateReactivatioRetroPayment(Convert.ToInt32(ldrPayeeAccountId["PAYEE_ACCOUNT_ID"]));
                }

                LoadPayeeAccount();
            }

            icdoDeathNotification.notification_change_date = DateTime.Now;
            icdoDeathNotification.death_notification_status_value = busConstant.NOTIFICATION_STATUS_INCORRECTLY_REPORTED;
            icdoDeathNotification.Update();
            // decommissioning demographics informations, since HEDB is retiring.
            //  UpdateHEDB(busConstant.NOTIFICATION_STATUS_INCORRECTLY_REPORTED);
            larrList.Add(this);
            EvaluateInitialLoadRules();
            return larrList;
        }

        public ArrayList btn_NotDeceased()
        {

            //Rohan - Eligibility PIR 806
            this.ibusPerson.icdoPerson.date_of_death = DateTime.MinValue;
            this.ibusPerson.icdoPerson.Update();

            ArrayList larrList = new ArrayList();

            if (this.icdoDeathNotification.death_notification_status_value == busConstant.NOTIFICATION_STATUS_CERTIFIED)
            {                
                foreach (busBenefitApplication lbusBenefitApplication in this.iclbBenefitApplicationDetails)
                {
                    if (lbusBenefitApplication.icdoBenefitApplication.application_status_value == busConstant.BENEFIT_APPL_CANCELLED
                        && lbusBenefitApplication.icdoBenefitApplication.cancellation_reason_value == busConstant.CANCELLATION_REASON_DECEASED)
                    {
                        DataTable ldtblist = busPerson.Select("cdoDeathNotification.GetPreviousApplStatus", new object[1] { lbusBenefitApplication.icdoBenefitApplication.benefit_application_id });
                        if (ldtblist.Rows.Count == 2)
                        {
                            lbusBenefitApplication.icdoBenefitApplication.application_status_value = Convert.ToString(ldtblist.Rows[1]["STATUS_VALUE"]);
                            lbusBenefitApplication.icdoBenefitApplication.cancellation_reason_value = "";

                            lbusBenefitApplication.icdoBenefitApplication.Update();
                        }
                    }
                }

                //this.ibusPerson.icdoPerson.date_of_death = DateTime.MinValue;
                //this.ibusPerson.icdoPerson.Update();

                AuditLogHistoryForPerson();
                
				//Ticket - 68031
                LoadPersonAccountStatusBeforeDeath(icdoDeathNotification.death_notification_id);

                foreach (busPersonAccount lbusPersonAccount in this.ibusPerson.iclbPersonAccount)
                {
                    lbusPersonAccount.icdoPersonAccount.status_id = 6035;

					//Ticket - 68031
                    if (iclbPersonAccountStatusBeforeDeath != null && iclbPersonAccountStatusBeforeDeath.Count() > 0
                        && iclbPersonAccountStatusBeforeDeath.Where(t => t.icdoPersonAccountStatusBeforeDeath.plan_id == lbusPersonAccount.icdoPersonAccount.plan_id).Count() > 0)
                    {
                        busPersonAccountStatusBeforeDeath lbusPersonAccountStatusBeforeDeath =
                            iclbPersonAccountStatusBeforeDeath.Where(t => t.icdoPersonAccountStatusBeforeDeath.plan_id == lbusPersonAccount.icdoPersonAccount.plan_id).OrderByDescending(t => t.icdoPersonAccountStatusBeforeDeath.modified_date).FirstOrDefault();

                        lbusPersonAccount.icdoPersonAccount.end_date = lbusPersonAccountStatusBeforeDeath.icdoPersonAccountStatusBeforeDeath.end_date;
                        lbusPersonAccount.icdoPersonAccount.status_value = lbusPersonAccountStatusBeforeDeath.icdoPersonAccountStatusBeforeDeath.status_value;
                        lbusPersonAccount.icdoPersonAccount.Update();
                    }
                    else if (lbusPersonAccount.icdoPersonAccount.end_date == icdoDeathNotification.date_of_death)
                    {
                        lbusPersonAccount.icdoPersonAccount.end_date = DateTime.MinValue;
                        lbusPersonAccount.icdoPersonAccount.status_value = busConstant.PERSON_ACCOUNT_STATUS_ACTIVE;
                        lbusPersonAccount.icdoPersonAccount.Update();
                    }
                }


                foreach (busPersonContact lbusPersonContact in this.ibusPerson.iclbPersonContact)
                {
                    lbusPersonContact.icdoPersonContact.effective_end_date = DateTime.MinValue;
                    lbusPersonContact.icdoPersonContact.Update();

                }

                foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in this.ibusPerson.iclbPersonAccountBeneficiary)
                {
                    lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date = DateTime.MinValue;
                    lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.Update();
                    lbusPersonAccountBeneficiary.ibusParticipant = this.ibusPerson;
                    lbusPersonAccountBeneficiary.iaintMainPersonID = this.ibusPerson.icdoPerson.person_id;
                    lbusPersonAccountBeneficiary.ValidateSoftErrors();
                    lbusPersonAccountBeneficiary.UpdateValidateStatus();
                }
               
                foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in this.iclbBeneficiaryOf)
                {
                    lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date = DateTime.MinValue;
                    lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.Update();
                    lbusPersonAccountBeneficiary.ibusParticipant = lbusPersonAccountBeneficiary.ibusPerson;
                    lbusPersonAccountBeneficiary.iaintMainPersonID = lbusPersonAccountBeneficiary.ibusPerson.icdoPerson.person_id;
                    lbusPersonAccountBeneficiary.ValidateSoftErrors();
                    lbusPersonAccountBeneficiary.UpdateValidateStatus();
                }

                
                DataTable ldtbPreRetirementDeath = busPerson.Select("cdoDeathNotification.GetPreDeathRetirementApplication", new object[1] { this.icdoDeathNotification.person_id });
                if (ldtbPreRetirementDeath.Rows.Count > 0)
                {
                    Collection<busDeathPreRetirement> lclbDeathPreRetirement = new Collection<busDeathPreRetirement>();
                    lclbDeathPreRetirement = GetCollection<busDeathPreRetirement>(ldtbPreRetirementDeath, "icdoBenefitApplication");

                    foreach (busDeathPreRetirement lbusDeathPreRetirement in lclbDeathPreRetirement)
                    {
                        lbusDeathPreRetirement.icdoBenefitApplication.application_status_value = busConstant.BENEFIT_APPL_CANCELLED;
                        lbusDeathPreRetirement.icdoBenefitApplication.Update();
                    }
                }
            }


            //For Changing Payee Account status
            DataTable ldtbPayeeAccountIDs = Select("cdoPayeeAccount.GetPreviousStatusForDeathNotification", new object[1] { this.icdoDeathNotification.person_id });
            if (ldtbPayeeAccountIDs.Rows.Count > 0)
            {
                foreach (DataRow ldrPayeeAccountId in ldtbPayeeAccountIDs.Rows)
                {
                    cdoPayeeAccountStatus lcdoPayeeAccountStatus = new cdoPayeeAccountStatus();
                    lcdoPayeeAccountStatus.payee_account_id = Convert.ToInt32(ldrPayeeAccountId["PAYEE_ACCOUNT_ID"]);
                    lcdoPayeeAccountStatus.status_value = busConstant.PAYEE_ACCOUNT_STATUS_REVIEW;//Convert.ToString(ldrPayeeAccountId["STATUS_VALUE"]);
                    lcdoPayeeAccountStatus.status_effective_date = DateTime.Now;
                    lcdoPayeeAccountStatus.Insert();

                    //Payment Adjustments
                    CreateReactivatioRetroPayment(Convert.ToInt32(ldrPayeeAccountId["PAYEE_ACCOUNT_ID"]));
                }
                LoadPayeeAccount();
            }

            icdoDeathNotification.notification_change_date = DateTime.Now;
            icdoDeathNotification.death_notification_status_value = busConstant.NOTIFICATION_STATUS_NOT_DECEASED;
            icdoDeathNotification.Update();
          //  UpdateHEDB(busConstant.NOTIFICATION_STATUS_NOT_DECEASED);
            larrList.Add(this);
            EvaluateInitialLoadRules();
            return larrList;

        }

        //Payment Adjustments
        public void CreateReactivatioRetroPayment(int aintPayeeAccountId)
        {

            busPayeeAccount lbusPayeeAccount = new busPayeeAccount();
            if (lbusPayeeAccount.FindPayeeAccount(aintPayeeAccountId))
            {
                lbusPayeeAccount.LoadPayeeAccountStatuss();
                lbusPayeeAccount.LoadBenefitDetails();
                lbusPayeeAccount.LoadDRODetails();
                lbusPayeeAccount.LoadPayeeAccountPaymentItemType();

                if (lbusPayeeAccount.icdoPayeeAccount.iintPlanId != busConstant.IAP_PLAN_ID)
                    lbusPayeeAccount.CreateReactivatioRetroPaymentItem();
            }
        }

        //payment Adjustments--In Case Of Shared Interest DRO after the Death of Alternate Payee 
        //,Add shared interest in participants Account
        private void UpdateParticipantsPayeeAccount()
        {
            DataTable ldtAltPayeeAccountId =
                Select("cdoDeathNotification.GetPayeeAccountIdOfDeceasedAltPayee", new object[1] { icdoDeathNotification.person_id });

            if (ldtAltPayeeAccountId.Rows.Count > 0)
            {

                busPayeeAccount lbusAlternatePayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };

            }

        }

        public void LoadBenficiaryOf()
        {
            DataTable ldtblist = busPerson.Select("cdoDeathNotification.BeneficiaryOf", new object[1] { this.ibusPerson.icdoPerson.person_id });
            iclbBeneficiaryOf = GetCollection<busPersonAccountBeneficiary>(ldtblist, "icdoPersonAccountBeneficiary");

        }


        public void LoadDependentOf()
        {
            DataTable ldtblist = busPerson.Select("cdoDeathNotification.DependentOf", new object[1] { this.ibusPerson.icdoPerson.person_id });
            iclbDependentOf = GetCollection<busRelationship>(ldtblist, "icdoRelationship");
        }

        public void LoadDroApplicationDetails()
        {
            DataTable ldtblist = busPerson.Select("cdoDeathNotification.LoadDroApplicationDetails", new object[1] { this.icdoDeathNotification.person_id });
            iclbDroApplicationDetails = GetCollection<busQdroApplication>(ldtblist, "icdoDroApplication");
        }

        public void LoadBenefitApplicationDetails()
        {
            DataTable ldtblist = busPerson.Select("cdoDeathNotification.LoadBenefitApplicationDetails", new object[1] { this.icdoDeathNotification.person_id });
            iclbBenefitApplicationDetails = GetCollection<busBenefitApplication>(ldtblist, "icdoBenefitApplication");
        }

        public void LoadPayeeAccount()
        {
            DataTable ldtblist = busPerson.Select("cdoDeathNotification.LoadPayeeAccount", new object[1] { this.icdoDeathNotification.person_id });
            iclbPayeeAccount = GetCollection<busPayeeAccount>(ldtblist, "icdoPayeeAccount");
        }

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            base.LoadOtherObjects(adtrRow, aobjBus);


            if (aobjBus is busPersonAccountBeneficiary && adtrRow[enmPerson.person_id.ToString()] != DBNull.Value && adtrRow.Table.Columns.Contains(enmPerson.person_id.ToString()))
            {
                busPersonAccountBeneficiary lobjPersonAccountBeneficiary = (busPersonAccountBeneficiary)aobjBus;
                lobjPersonAccountBeneficiary.ibusPerson = new busPerson();
                lobjPersonAccountBeneficiary.ibusRelationship = new busRelationship();
                lobjPersonAccountBeneficiary.ibusPerson.FindPerson(Convert.ToInt32(adtrRow[enmPerson.person_id.ToString()]));
                lobjPersonAccountBeneficiary.ibusRelationship.FindRelationship(Convert.ToInt32(adtrRow[enmPersonAccountBeneficiary.person_relationship_id.ToString()]));
            }

            if (aobjBus is busRelationship && adtrRow[enmPerson.person_id.ToString()] != DBNull.Value)
            {
                busRelationship lobjRelationship = (busRelationship)aobjBus;
                lobjRelationship.ibusPerson = new busPerson();
                lobjRelationship.ibusPerson.FindPerson(Convert.ToInt32(adtrRow[enmPerson.person_id.ToString()]));
            }

            if (aobjBus is busQdroApplication)
            {
                busQdroApplication lobjQdroApplication = (busQdroApplication)aobjBus;
                lobjQdroApplication.ibusParticipant = new busPerson();
                lobjQdroApplication.ibusParticipant.FindPerson(lobjQdroApplication.icdoDroApplication.person_id);

                lobjQdroApplication.ibusAlternatePayee = new busPerson();
                lobjQdroApplication.ibusAlternatePayee.FindPerson(lobjQdroApplication.icdoDroApplication.alternate_payee_id);
            }

            if (aobjBus is busBenefitApplication)
            {
                busBenefitApplication lobjBenefitApplication = (busBenefitApplication)aobjBus;
                lobjBenefitApplication.ibusPerson = new busPerson();
                lobjBenefitApplication.ibusPerson.FindPerson(lobjBenefitApplication.icdoBenefitApplication.person_id);
            }
        }


        public override ArrayList ValidateNew(Hashtable ahstParam)
        {
            ArrayList larrErrors = new ArrayList();
            utlError lobjError = null;
            int lintPersonId = 0;
            string lstrBenefitTypeValue = string.Empty;
            string lstrMpiPersonId = Convert.ToString(ahstParam["m_p_i__p_e_r_s_o_n__id"]).Trim();
            if (string.IsNullOrEmpty(lstrMpiPersonId))
            {
                lobjError = AddError(4075, "");
                larrErrors.Add(lobjError);
                return larrErrors;
            }
            else
            {
                lstrMpiPersonId = lstrMpiPersonId.Trim();
                if (iobjPassInfo.idictParams.ContainsKey("UserID"))
                {
                    string astrUserSerialID = iobjPassInfo.idictParams["UserID"].ToString();
                    DataTable ldtbPer = Select("cdoPerson.GetVIPFlagInfo", new object[2] { astrUserSerialID, lstrMpiPersonId });
                    if (ldtbPer.Rows.Count > 0)
                    {
                        if (ldtbPer.Rows[0]["istr_IS_LOGGED_IN_USER_VIP"].ToString() == "N" && ldtbPer.Rows[0]["istrRelativeVipFlag"].ToString() == "Y")
                        {
                            lobjError = AddError(6175, "");
                            larrErrors.Add(lobjError);
                            return larrErrors;
                        }
                    }

                }
                DataTable ldtbParticipantId = Select("cdoBenefitApplication.CheckParticipantExistance", new object[1] { lstrMpiPersonId });
                if (ldtbParticipantId.Rows.Count > 0)
                {
                    lintPersonId = Convert.ToInt32(ldtbParticipantId.Rows[0][enmPerson.person_id.ToString()]);
                }

                if (ldtbParticipantId.Rows.Count <= 0)
                {
                    lobjError = AddError(5016, "");
                    larrErrors.Add(lobjError);
                    return larrErrors;

                }
                else
                {
                    lintPersonId = Convert.ToInt32(ldtbParticipantId.Rows[0][enmPerson.person_id.ToString()]);
                    DataTable ldtbDeathNotification = busBase.Select("cdoDeathNotification.GetCertifiedNotification", new object[] { lintPersonId });
                    if (ldtbDeathNotification != null && ldtbDeathNotification.Rows.Count > 0)
                    {
                        int lintCount = Convert.ToInt32(ldtbDeathNotification.Rows[0]["COUNT"]);
                        if (lintCount > 0)
                        {
                            lobjError = AddError(5047, "");
                            larrErrors.Add(lobjError);
                            return larrErrors;
                        }

                    }

                }
            }

            // We should not be checking for any plan related  information for death notification, 
            // because Death Notification will be entered for Beneficiary and Dependents too.
            //if (lintPersonId != 0)          // To check if plan is not enrolled.
            //{
            //    DataTable ldtbList = Select("cdoPersonAccountBeneficiary.GetPlan", new object[1] { lintPersonId });
            //    if (ldtbList.Rows.Count == 0)
            //    {
            //        lobjError = AddError(5092, "");
            //        larrErrors.Add(lobjError);
            //        return larrErrors;
            //    }
            //}

            return larrErrors;
        }


        public bool IsDateOfDeathNull()
        {
            if (this.icdoDeathNotification.date_of_death == DateTime.MinValue)
            {
                return true;
            }
            return false;
        }

        public bool IsDeathNotificationReceivedDateNull()
        {
            if (this.icdoDeathNotification.death_notification_received_date == DateTime.MinValue)
            {
                return true;
            }
            return false;
        }

        public bool IsDeathNotificationReceivedDateFutureDate()
        {
            if (this.icdoDeathNotification.death_notification_received_date > DateTime.Now)
            {
                return true;
            }
            return false;
        }

        public bool DateOfDeathIsFutureDate()
        {
            if (this.icdoDeathNotification.date_of_death > DateTime.Now)
            {
                return true;
            }
            return false;
        }

        public bool DODGreatherThnNotificationRcvDate()
        {
            if (this.icdoDeathNotification.date_of_death > this.icdoDeathNotification.death_notification_received_date)
            {
                return true;
            }
            return false;
        }

        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            utlError lobjError = null;
            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();
            int lintPersonId = 0;
            bool lblnCheck = false;
            if (this.icdoDeathNotification.person_id.IsNotNull())
            {
                DataTable ldtbDeathNotification = busBase.Select("cdoDeathNotification.GetDeathNotificationStatusDetails", new object[] { this.icdoDeathNotification.person_id, this.icdoDeathNotification.death_notification_id });
                if (ldtbDeathNotification != null && ldtbDeathNotification.Rows.Count > 0)
                {
                    int lintCount = Convert.ToInt32(ldtbDeathNotification.Rows[0]["COUNT"]);
                    if (lintCount > 0)
                    {
                        lobjError = AddError(5404, "");
                        this.iarrErrors.Add(lobjError);
                        lblnCheck = true;
                    }
                }
            }
            if (lblnCheck)
                return;

            foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in this.ibusPerson.iclbPersonAccountBeneficiary)
            {
                if (lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.start_date > this.icdoDeathNotification.date_of_death && lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.istrPlan != busConstant.LIFE_PLAN)
                {
                    lobjError = AddError(1168, " ");
                    this.iarrErrors.Add(lobjError);
                    break;
                }

            }
            foreach (busPersonContact lbusPersonContact in this.ibusPerson.iclbPersonContact)
            {
                if (lbusPersonContact.icdoPersonContact.effective_start_date > this.icdoDeathNotification.date_of_death)
                {
                    lobjError = AddError(1167, " ");
                    this.iarrErrors.Add(lobjError);
                    break;
                }
            }
            foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in this.iclbBeneficiaryOf)
            {
                if (lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.start_date > this.icdoDeathNotification.date_of_death)
                {
                    lobjError = AddError(1169, " ");
                    this.iarrErrors.Add(lobjError);
                    break;
                }
            }

            base.ValidateHardErrors(aenmPageMode);
        }


        public string ReturnRetirementStatus(int aintPersonId)
        {
            string lstrRetireeStatus = "A";
            DataTable ldtbBenefitApplcation = busBase.Select("cdoDeathNotification.GetApprovedReirementorDisability", new object[1] { aintPersonId });
            if (ldtbBenefitApplcation != null && ldtbBenefitApplcation.Rows.Count > 0)
            {
                int lintCount = Convert.ToInt32(ldtbBenefitApplcation.Rows[0]["COUNT"]);
                if (lintCount > 0)
                {
                    lstrRetireeStatus = "R";
                }
            }
            return lstrRetireeStatus;

        }
        public DataTable LoadDeathNotificationBatch()
        {
            DataTable ldtblist = null;
            IDbConnection lconLegacy = DBFunction.GetDBConnection("Legacy");
            if (lconLegacy != null)
            {
                ldtblist = Select("cdoDeathNotification.LoadDeathOutboundFile", new object[0] { });
                foreach (DataRow ldtRow in ldtblist.Rows)
                {
                    //Get Union Code
                    Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();

                    IDbDataParameter lobjParameter = DBFunction.GetDBParameter();
                    lobjParameter.ParameterName = "@SSN";
                    lobjParameter.DbType = DbType.String;
                    lobjParameter.Value = ldtRow["SSN"].ToString();
                    lcolParameters.Add(lobjParameter);
                    IDataReader lDataRedaer = DBFunction.DBExecuteProcedureResult("sp_GetTrueUnions", lcolParameters, lconLegacy, null);
                    DataTable ldataTable = new DataTable();

                    if (lDataRedaer != null)
                    {
                        ldataTable.Load(lDataRedaer);
                        if (ldataTable.Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(ldataTable.Rows[0][0])))
                            {
                                ldtRow["UNION_CODE"] = Convert.ToInt32(ldataTable.Rows[0][0]);
                            }
                            ldtRow["UNION_CODE_DESC"] = ldataTable.Rows[0][1].ToString();

                        }
                    }

                    //Status Depending on Death
                    if (Convert.ToString(ldtRow[enmDeathNotification.death_notification_status_value.ToString()]) == busConstant.NOTIFICATION_STATUS_INCORRECTLY_REPORTED ||
                    Convert.ToString(ldtRow[enmDeathNotification.death_notification_status_value.ToString()]) == busConstant.NOTIFICATION_STATUS_NOT_DECEASED)
                    {
                        ldtRow["APP_STATUS"] = busConstant.INC_REPORT_DEATH_REPORT;
                        ldtRow["PREV_DATE_DEATH"] = ldtRow[enmDeathNotification.date_of_death.ToString()].ToString();
                        ldtRow[enmDeathNotification.date_of_death.ToString()] = DBNull.Value;
                    }
                    else
                    {
                        ldtRow["APP_STATUS"] = busConstant.NEW_REPORT_DEATH_REPORT;
                    }

                    //Status Active or Retiree
                    if (!string.IsNullOrEmpty(Convert.ToString(ldtRow[enmPerson.person_id.ToString()])))
                    {
                        ldtRow["STATUS"] = ReturnRetirementStatus(Convert.ToInt32(ldtRow[enmPerson.person_id.ToString()]));
                    }
                }
            }
            return ldtblist;

        }

        public busDeathNotification LoadDetailsForInboundFile(string mpi_person_id)
        {
            busDeathNotification lbusDeathNotification = new busDeathNotification { icdoDeathNotification = new cdoDeathNotification() };
            DataTable ldtblist = busPerson.Select("cdoDeathNotification.GetDeathNotificationForInbound", new object[1] { mpi_person_id });
            if (ldtblist.Rows.Count > 0)
            {
                lbusDeathNotification.icdoDeathNotification.LoadData(ldtblist.Rows[0]);
            }
            return lbusDeathNotification;
        }
        public void DivideBenefitsIntoBeneficiary(int aintDeadBeneficiaryPayeeAccountId, busPayeeAccount lbusDeadPayeeAccount)
        {
            DataTable ldtbOthrPayeeAccount = busBase.Select("cdoPayeeBenefitAccount.GetOthActiveBeneficiary", new object[1] { aintDeadBeneficiaryPayeeAccountId });
            //busPayeeAccount lbusDeadPayeeAccount=new busPayeeAccount{icdoPayeeAccount=new cdoPayeeAccount()};
            if (lbusDeadPayeeAccount.icdoPayeeAccount.payee_account_id != 0)
            {
                //Moving paymnet item into other benificiary accounts
                lbusDeadPayeeAccount.LoadPayeeAccountPaymentItemType();
                lbusDeadPayeeAccount.LoadNextBenefitPaymentDate();
                lbusDeadPayeeAccount.iclbPayeeAccountPaymentItemTypeActive = (from item in lbusDeadPayeeAccount.iclbPayeeAccountPaymentItemType
                                                                              where busGlobalFunctions.CheckDateOverlapping(lbusDeadPayeeAccount.idtNextBenefitPaymentDate,
                                                          item.icdoPayeeAccountPaymentItemType.start_date, item.icdoPayeeAccountPaymentItemType.end_date)
                                                                              select item).ToList().ToCollection<busPayeeAccountPaymentItemType>();
                Collection<busPayeeAccount> iclbOthPayeeAccount = new Collection<busPayeeAccount>();
                foreach (DataRow ldrPayee in ldtbOthrPayeeAccount.AsEnumerable())
                {
                    busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                    lbusPayeeAccount.icdoPayeeAccount.LoadData(ldrPayee);
                    iclbOthPayeeAccount.Add(lbusPayeeAccount);
                }
                int aintDevidfactor = iclbOthPayeeAccount.Count();
                foreach (busPayeeAccount lOthbusPayeeAccount in iclbOthPayeeAccount)
                {
                    lOthbusPayeeAccount.LoadPayeeAccountPaymentItemType();
                    lOthbusPayeeAccount.LoadNextBenefitPaymentDate();
                    lOthbusPayeeAccount.LoadLastPaymentDate();
                    if (lOthbusPayeeAccount.idtNextBenefitPaymentDate != DateTime.MinValue)
                    {
                        // Active Payee Account Payment Item type
                        lOthbusPayeeAccount.iclbPayeeAccountPaymentItemTypeActive = (from item in lOthbusPayeeAccount.iclbPayeeAccountPaymentItemType
                                                                                     where busGlobalFunctions.CheckDateOverlapping(lOthbusPayeeAccount.idtNextBenefitPaymentDate,
                                                                                       item.icdoPayeeAccountPaymentItemType.start_date, item.icdoPayeeAccountPaymentItemType.end_date)
                                                                                       && item.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == 1
                                                                                     select item).ToList().ToCollection<busPayeeAccountPaymentItemType>();
                        foreach (busPayeeAccountPaymentItemType lDeadPayeeAccountPaymentItemType in lbusDeadPayeeAccount.iclbPayeeAccountPaymentItemTypeActive)
                        {
                            decimal ldecnewAmount = 0M;
                            string lstrAccountNo = string.Empty;
                            int lintVendor = 0;
                            busPayeeAccountPaymentItemType ollbPaymentItemtype = null;

                            if (lOthbusPayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Where
                                                                                (obj => obj.icdoPayeeAccountPaymentItemType.payment_item_type_id == lDeadPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.payment_item_type_id).Count() > 0)
                            {
                                ollbPaymentItemtype = lOthbusPayeeAccount.iclbPayeeAccountPaymentItemTypeActive.Where
                                                                                    (obj => obj.icdoPayeeAccountPaymentItemType.payment_item_type_id == lDeadPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.payment_item_type_id).FirstOrDefault();

                                if (ollbPaymentItemtype != null)
                                {
                                    ldecnewAmount = ollbPaymentItemtype.icdoPayeeAccountPaymentItemType.amount + lDeadPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.amount / aintDevidfactor;
                                    lstrAccountNo = ollbPaymentItemtype.icdoPayeeAccountPaymentItemType.account_number;
                                    lintVendor = ollbPaymentItemtype.icdoPayeeAccountPaymentItemType.vendor_org_id;
                                }
                                else
                                {
                                    ldecnewAmount = lDeadPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.amount / aintDevidfactor;
                                    lstrAccountNo = lDeadPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.account_number;
                                    lintVendor = lDeadPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.vendor_org_id;
                                }
                                ldecnewAmount = Math.Round(ldecnewAmount, 2);
                                lOthbusPayeeAccount.CreatePayeeAccountPaymentItemType(lDeadPayeeAccountPaymentItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_code, ldecnewAmount, lstrAccountNo, lintVendor,
                                                                                        lOthbusPayeeAccount.idtNextBenefitPaymentDate, DateTime.MinValue, "N", false);
                            }
                        }

                        lOthbusPayeeAccount.LoadPayeeAccountPaymentItemType();
                        busCalculation lbusCalculation = new busCalculation();
                        Collection<busBenefitMonthwiseAdjustmentDetail> lclbBenefitMonthwiseAdjustmentDetail = new Collection<busBenefitMonthwiseAdjustmentDetail>();

                        if (icdoDeathNotification.date_of_death.Day == 1)
                        {
                            lclbBenefitMonthwiseAdjustmentDetail = lbusCalculation.GetAmountActuallyPaid(lOthbusPayeeAccount, icdoDeathNotification.date_of_death.AddMonths(1), lOthbusPayeeAccount.idtLastBenefitPaymentDate);
                        }
                        else
                        {
                            lclbBenefitMonthwiseAdjustmentDetail = lbusCalculation.GetAmountActuallyPaid(lOthbusPayeeAccount, icdoDeathNotification.date_of_death.GetLastDayofMonth().AddDays(1), lOthbusPayeeAccount.idtLastBenefitPaymentDate);
                        }

                        lbusCalculation.CalculateAmountShouldHaveBeenPaid(lOthbusPayeeAccount, ref lclbBenefitMonthwiseAdjustmentDetail);

                        lbusCalculation.CreateOverpaymentUnderPayment(lOthbusPayeeAccount, lclbBenefitMonthwiseAdjustmentDetail, busConstant.RETRO_PAYMENT_DEATH);

                    }
                }
            }

        }

        public void AuditLogHistoryForPerson(string astrDateofDeath = "")
        {
            cdoFullAuditLog lcdoFullAuditLog = new cdoFullAuditLog();
            lcdoFullAuditLog.person_id = this.icdoDeathNotification.person_id;
            lcdoFullAuditLog.primary_key = this.icdoDeathNotification.person_id;
            lcdoFullAuditLog.form_name = "wfmPersonMaintenance";
            lcdoFullAuditLog.table_name = "sgt_person";
            //lcdoFullAuditLog.Insert();

            //cdoFullAuditLogDetail lcdoFullAuditLogDetail = new cdoFullAuditLogDetail();
            //lcdoFullAuditLogDetail.audit_log_id = lcdoFullAuditLog.audit_log_id;
            //if (!string.IsNullOrEmpty(astrDateofDeath))
            //{
            //    lcdoFullAuditLogDetail.old_value = string.Empty;
            //}
            //else
            //{
            //    lcdoFullAuditLogDetail.old_value = Convert.ToString(this.icdoDeathNotification.date_of_death);
            //}
            //lcdoFullAuditLogDetail.new_value = astrDateofDeath;
            //lcdoFullAuditLogDetail.column_name = "date_of_death";
            //lcdoFullAuditLogDetail.Insert();

            //Fw upgrade: PIR ID : 28660: New implementation of Audit History using audit_details
            string old_value = null;
            if (!string.IsNullOrEmpty(astrDateofDeath))
            {
                old_value = string.Empty;
            }
            else
            {
                old_value = Convert.ToString(this.icdoDeathNotification.date_of_death);
            }

            var lcdoFullAuditLogDetail = new
            {
                column_name = "date_of_death",
                old_value = old_value,
                new_value = astrDateofDeath,
            };

            string lsrtJSONAuditDetails = Newtonsoft.Json.JsonConvert.SerializeObject(lcdoFullAuditLogDetail);
            lcdoFullAuditLog.audit_details = lsrtJSONAuditDetails;
            lcdoFullAuditLog.Insert();
        }
        
		//Ticket - 68031
        public Collection<busPersonAccountStatusBeforeDeath> LoadPersonAccountStatusBeforeDeath(int aintDeathNotificationID)
        {
            iclbPersonAccountStatusBeforeDeath = new Collection<busPersonAccountStatusBeforeDeath>();

            DataTable ldtbList = Select<cdoPersonAccountStatusBeforeDeath>(
                new string[1] { enmPersonAccountStatusBeforeDeath.death_notification_id.ToString().ToUpper() },
                new object[1] { aintDeathNotificationID }, null, "MODIFIED_DATE DESC");
            iclbPersonAccountStatusBeforeDeath = GetCollection<busPersonAccountStatusBeforeDeath>(ldtbList, "icdoPersonAccountStatusBeforeDeath");

            return iclbPersonAccountStatusBeforeDeath;
        }

    }
}
