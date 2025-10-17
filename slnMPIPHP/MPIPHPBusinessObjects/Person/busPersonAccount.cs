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
using NeoSpin.BusinessObjects;
using System.Linq;
using Sagitec.Bpm;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busPersonAccount:
	/// Inherited from busPersonAccountGen, the class is used to customize the business object busPersonAccountGen.
	/// </summary>
	[Serializable]
    public class busPersonAccount : busPersonAccountGen
    {
        public DateTime idtRetirementDate { get; set; }

        public int iintActivityInstanceId { get; set; }

        public Collection<busPersonAccountBeneficiary> iclbPersonAccountBeneficiary { get; set; }

        public Collection<busPayeeAccount> iclbPayeeAccount { get; set; }

        public bool IsSubmitButtonVisible()
        {
            if (ibusBaseActivityInstance != null)
            {
                busBpmActivityInstance lbusBpmActivityInstance = ibusBaseActivityInstance as busBpmActivityInstance;
                if (lbusBpmActivityInstance != null)
                {
                    this.iintActivityInstanceId = lbusBpmActivityInstance.icdoBpmActivityInstance.activity_instance_id;
                    if (lbusBpmActivityInstance.ibusBpmActivity.icdoBpmActivity.name == busConstant.PersonAccountMaintenance.PERSON_ACCOUNT_RETIREMENT_INTAKE_ACTIVITY)
                    {
                        if (this.icdoPersonAccount.application_form_sent == busConstant.Flag_Yes || this.icdoPersonAccount.election_packet_sent == busConstant.Flag_Yes
                            || this.icdoPersonAccount.election_packet_received == busConstant.Flag_Yes || this.icdoPersonAccount.signed_application_forn_received_date != DateTime.MinValue)
                        {
                            return true;
                        }
                    }
                    else if (lbusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.ibusBpmCase.icdoBpmCase.name != busConstant.PersonAccountMaintenance.PERSON_ACCOUNT_RETIREMENT_INTAKE_ACTIVITY)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        public void InsertDataInPersonAccount(int aintPersonId, int aintPlanID, DateTime adtStartDate)
        {
            if(icdoPersonAccount == null)
            {
                icdoPersonAccount = new cdoPersonAccount() ;
            }

            this.icdoPersonAccount.person_id = aintPersonId;
            this.icdoPersonAccount.plan_id = aintPlanID;
            this.icdoPersonAccount.start_date = adtStartDate;
            this.icdoPersonAccount.status_id = busConstant.PERSON_ACCOUNT_STATUS_ID;
            this.icdoPersonAccount.status_value = busConstant.PERSON_ACCOUNT_STATUS_ACTIVE;
            this.icdoPersonAccount.created_by = iobjPassInfo.istrUserID;
            this.icdoPersonAccount.created_date = DateTime.Now;
            this.icdoPersonAccount.modified_by = iobjPassInfo.istrUserID;
            this.icdoPersonAccount.modified_date = DateTime.Now;
            this.icdoPersonAccount.update_seq = 0;
            this.icdoPersonAccount.Insert();
        }

        public busPersonAccount GetPersonAccountInfo(int aintPersonId, int aintPlanID, DateTime adtStartDate)
        {
            busPersonAccount lbusPersonAccount = new busPersonAccount();
            if (lbusPersonAccount.icdoPersonAccount == null)
            {
                lbusPersonAccount.icdoPersonAccount = new cdoPersonAccount();                
            }

            lbusPersonAccount.icdoPersonAccount.person_id = aintPersonId;
            lbusPersonAccount.icdoPersonAccount.plan_id = aintPlanID;
            lbusPersonAccount.icdoPersonAccount.start_date = adtStartDate;
            lbusPersonAccount.icdoPersonAccount.status_id = busConstant.PERSON_ACCOUNT_STATUS_ID;
            lbusPersonAccount.icdoPersonAccount.status_value = busConstant.PERSON_ACCOUNT_STATUS_ACTIVE;
            lbusPersonAccount.icdoPersonAccount.created_by = iobjPassInfo.istrUserID;
            lbusPersonAccount.icdoPersonAccount.created_date = DateTime.Now;
            lbusPersonAccount.icdoPersonAccount.modified_by = iobjPassInfo.istrUserID;
            lbusPersonAccount.icdoPersonAccount.modified_date = DateTime.Now;
            lbusPersonAccount.icdoPersonAccount.update_seq = 0;

            return lbusPersonAccount;
        }

        public override void BeforePersistChanges()
        {
            base.BeforePersistChanges();
            SetBPMActivityInstanceParameters();
            if (iobjPassInfo.istrUserID != null)
            {
                busUser lbusUser = new busUser { icdoUser = new MPIPHP.CustomDataObjects.cdoUser() };
                if (!lbusUser.FindUserByUserName(this.icdoPersonAccount.created_by))
                {
                    if (this.icdoPersonAccount.created_by != this.iobjPassInfo.istrUserID)
                    {
                        DBFunction.DBNonQuery("entPersonAccount.UpdateCreatedBy",
                        new object[2] { this.iobjPassInfo.istrUserID, this.icdoPersonAccount.person_account_id },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                    }
                }
            }
        }
        public void LoadActivityInstance(object obj)
        {
            this.ibusBaseActivityInstance = (busSolBpmActivityInstance)obj;
        }

        public void SetBPMActivityInstanceParameters()
        {
            if (ibusBaseActivityInstance != null)
            {
                busSolBpmActivityInstance lbusBpmActivityInstance = ibusBaseActivityInstance as busSolBpmActivityInstance;
                if (lbusBpmActivityInstance != null)
                {
                    busUser lbusUser = new busUser();
                    if (lbusUser.FindUser(lbusBpmActivityInstance.icdoBpmActivityInstance.checked_out_user))
                    {
                        lbusBpmActivityInstance.UpdateParameterValue("PreviousCheckOutUser", lbusUser.icdoUser.user_serial_id);
                    }
                    if (lbusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.ibusBpmCase.icdoBpmCase.name == busConstant.PersonAccountMaintenance.SERVICE_RETIREMENT_BPM)
                    {
                        if (lbusBpmActivityInstance.ibusBpmActivity.icdoBpmActivity.name == busConstant.PersonAccountMaintenance.PERSON_ACCOUNT_RETIREMENT_INTAKE_ACTIVITY)
                        {
                            lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.SIGNED_APPLICATION_FORM_RECEIVED_DATE, busConstant.FLAG_NO);
                            lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.APPLICATION_FORM_SENT, busConstant.FLAG_NO);
                            lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.QDRO_LEGAL_REVIEW_REQUIRED, busConstant.FLAG_NO);
                            lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.ELECTION_PACKET_SENT, busConstant.FLAG_NO);
                            lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.PLAN_DESCRIPTION, this.icdoPersonAccount.istrPlan);
                            lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.RETIREMENT_DATE, LoadServiceRetirementDate(lbusBpmActivityInstance));
                            lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.ELECTION_PACKET_RECEIVED, busConstant.FLAG_NO);
                            lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.RETIREMENT_DATE_DAY_BEFORE, idtRetirementDate.AddDays(-1));
                            lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.SIGNED_DOCUMENT_RECEIVED, IsSignedDocumentReceived());
                            lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.SOURCE_OF_CANCELLATION, busConstant.PersonAccountMaintenance.BPM_TIMER_EXPIRTU_BEFORE_APPLICATION);

                            lbusBpmActivityInstance.UpdateParameterValue("LastActivityName", busConstant.PersonAccountMaintenance.PERSON_ACCOUNT_RETIREMENT_INTAKE_ACTIVITY);
                            lbusBpmActivityInstance.UpdateParameterValue("LastCheckOutUser", lbusBpmActivityInstance.icdoBpmActivityInstance.checked_out_user);

                            if (this.icdoPersonAccount.application_form_sent == busConstant.FLAG_YES && this.icdoPersonAccount.election_packet_sent != busConstant.FLAG_YES && this.icdoPersonAccount.qdro_legal_review_required != busConstant.FLAG_YES
                                && this.icdoPersonAccount.election_packet_received != busConstant.FLAG_YES && this.icdoPersonAccount.signed_application_forn_received_date == DateTime.MinValue
                                && this.icdoPersonAccount.qdro_review_completed_date == DateTime.MinValue)
                            {
                                lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.APPLICATION_FORM_SENT, busConstant.FLAG_YES);
                            }
                            else if (this.icdoPersonAccount.signed_application_forn_received_date != DateTime.MinValue && this.icdoPersonAccount.election_packet_sent != busConstant.FLAG_YES && this.icdoPersonAccount.qdro_legal_review_required != busConstant.FLAG_YES
                                && this.icdoPersonAccount.election_packet_received != busConstant.FLAG_YES && this.icdoPersonAccount.qdro_review_completed_date == DateTime.MinValue)
                            {
                                lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.APPLICATION_FORM_SENT, busConstant.FLAG_NO);
                                lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.SIGNED_APPLICATION_FORM_RECEIVED_DATE, busConstant.FLAG_YES);
                            }
                            else if (this.icdoPersonAccount.signed_application_forn_received_date != DateTime.MinValue && this.icdoPersonAccount.election_packet_received != busConstant.FLAG_YES && this.icdoPersonAccount.qdro_review_completed_date == DateTime.MinValue)
                            {
                                lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.SIGNED_APPLICATION_FORM_RECEIVED_DATE, busConstant.FLAG_YES);
                            }
                            else if (this.icdoPersonAccount.signed_application_forn_received_date != DateTime.MinValue && this.icdoPersonAccount.qdro_legal_review_required == busConstant.FLAG_YES)
                            {
                                lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.SIGNED_APPLICATION_FORM_RECEIVED_DATE, busConstant.FLAG_YES);
                            }
                            else if (this.icdoPersonAccount.election_packet_sent == busConstant.Flag_Yes)
                            {
                                lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.SIGNED_APPLICATION_FORM_RECEIVED_DATE, busConstant.FLAG_YES);
                            }
                            else if (this.icdoPersonAccount.election_packet_received == busConstant.FLAG_YES)
                            {
                                lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.ELECTION_PACKET_RECEIVED, busConstant.FLAG_YES);
                            }
                            else if (this.icdoPersonAccount.application_form_sent == busConstant.FLAG_YES && this.icdoPersonAccount.qdro_legal_review_required == busConstant.FLAG_YES)
                            {
                                lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.APPLICATION_FORM_SENT, busConstant.FLAG_YES);
                                lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.SIGNED_APPLICATION_FORM_RECEIVED_DATE, busConstant.FLAG_YES);
                            }
                            else if (this.icdoPersonAccount.application_form_sent == busConstant.FLAG_YES && this.icdoPersonAccount.qdro_review_completed_date != DateTime.MinValue)
                            {
                                lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.APPLICATION_FORM_SENT, busConstant.FLAG_YES);
                                lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.SIGNED_APPLICATION_FORM_RECEIVED_DATE, busConstant.FLAG_YES);
                            }
                            else if (this.icdoPersonAccount.qdro_legal_review_required == busConstant.FLAG_YES && this.icdoPersonAccount.application_form_sent != busConstant.Flag_Yes)
                            {
                                lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.SIGNED_APPLICATION_FORM_RECEIVED_DATE, busConstant.FLAG_YES);
                            }
                        }
                        else if (lbusBpmActivityInstance.ibusBpmActivity.icdoBpmActivity.name == busConstant.PersonAccountMaintenance.GENERATE_RETIREMENT_APPLICATION_CANCELLATION_NOTICE_ACTIVITY)
                        {
                            lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.GENERATE_CANCELLATION_NOTICE, IsCancellationNoticeGenerated());
                            lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.PAYEE_ACCOUNT_EXISTS, LoadPayeeAccount());
                            lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.PAYMENT_BEGIN_DATE_PROCESSED, IsRequirePayeeAudit());
                            lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.SERVICE_RETIREMENT_APPLICATION_EXISTS, IsApplicationExist());
                            lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.APPLICATION_ID, GetApplicationID());
                        }
                        else if (lbusBpmActivityInstance.ibusBpmActivity.icdoBpmActivity.name == busConstant.PersonAccountMaintenance.QDRO_LEAGL_REVIEW_BENEFIT_ELECTION_ACTIVITY)
                        {
                            lbusBpmActivityInstance.UpdateParameterValue("LastActivityName", busConstant.PersonAccountMaintenance.QDRO_LEAGL_REVIEW_BENEFIT_ELECTION_ACTIVITY);
                            lbusBpmActivityInstance.UpdateParameterValue("LastCheckOutUser", lbusBpmActivityInstance.icdoBpmActivityInstance.checked_out_user);

                            lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.SOURCE_OF_CANCELLATION, busConstant.PersonAccountMaintenance.BPM_TIMER_EXPIRTU_BEFORE_ELECTION);

                            if (this.icdoPersonAccount.qdro_legal_review_required == busConstant.FLAG_YES && this.icdoPersonAccount.qdro_review_completed_date == DateTime.MinValue)
                            {
                                lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.QDRO_LEGAL_REVIEW_REQUIRED, busConstant.FLAG_YES);
                            }
                            else if (this.icdoPersonAccount.qdro_legal_review_required == busConstant.FLAG_YES && this.icdoPersonAccount.election_packet_sent == busConstant.FLAG_YES && this.icdoPersonAccount.qdro_review_completed_date != DateTime.MinValue)
                            {
                                lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.QDRO_LEGAL_REVIEW_REQUIRED, busConstant.FLAG_NO);
                            }
                            else if (this.icdoPersonAccount.qdro_legal_review_required == busConstant.FLAG_YES && this.icdoPersonAccount.election_packet_sent == busConstant.FLAG_YES)
                            {
                                lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.QDRO_LEGAL_REVIEW_REQUIRED, busConstant.FLAG_YES);
                            }
                            else if (this.icdoPersonAccount.qdro_review_completed_date != DateTime.MinValue)
                            {
                                lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.QDRO_LEGAL_REVIEW_REQUIRED, busConstant.FLAG_NO);
                            }
                            else if (this.icdoPersonAccount.election_packet_sent == busConstant.Flag_Yes)
                            {
                                lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.QDRO_LEGAL_REVIEW_REQUIRED, busConstant.FLAG_NO);
                            }
                        }
                        else if (lbusBpmActivityInstance.ibusBpmActivity.icdoBpmActivity.name == busConstant.PersonAccountMaintenance.LEGAL_DOCUMENT_REVIEW_ACTIVITY)
                        {
                            if (this.icdoPersonAccount.qdro_legal_review_required == busConstant.FLAG_YES && this.icdoPersonAccount.election_packet_sent == busConstant.FLAG_YES)
                            {
                                lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.QDRO_LEGAL_REVIEW_REQUIRED, busConstant.FLAG_NO);
                            }
                            else if (this.icdoPersonAccount.qdro_legal_review_required == busConstant.FLAG_YES && this.icdoPersonAccount.qdro_review_completed_date == DateTime.MinValue)
                            {
                                lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.QDRO_LEGAL_REVIEW_REQUIRED, busConstant.FLAG_YES);
                            }
                            else if (this.icdoPersonAccount.qdro_review_completed_date != DateTime.MinValue)
                            {
                                lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.QDRO_LEGAL_REVIEW_REQUIRED, busConstant.FLAG_NO);
                            }
                            else if (this.icdoPersonAccount.election_packet_received ==busConstant.Flag_Yes)
                            {
                                lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.QDRO_LEGAL_REVIEW_REQUIRED, busConstant.FLAG_NO);
                            }
                        }
                    }
                    else if (lbusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.ibusBpmCase.icdoBpmCase.name == busConstant.PersonAccountMaintenance.CANCEL_SERVICE_RETIREMENT_BPM)
                    {
                        lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.GENERATE_CANCELLATION_NOTICE, IsCancellationNoticeGenerated());
                        lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.PAYEE_ACCOUNT_EXISTS, LoadPayeeAccount());
                        lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.PAYMENT_BEGIN_DATE_PROCESSED, IsRequirePayeeAudit());
                        lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.SERVICE_RETIREMENT_APPLICATION_EXISTS, IsApplicationExist());
                        lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.APPLICATION_ID, GetApplicationID());
                    }
                    else if (lbusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.ibusBpmCase.icdoBpmCase.name == busConstant.PersonAccountMaintenance.APPLICATION_SERVICE_RETIREMENT_BPM)
                    {
                        if (lbusBpmActivityInstance.ibusBpmActivity.icdoBpmActivity.name == busConstant.PersonAccountMaintenance.LEGAL_DOCUMENT_REVIEW_ACTIVITY)
                        {
                            this.icdoPersonAccount.qdro_auditor_name = this.iobjPassInfo.istrUserID;
                            if (this.icdoPersonAccount.qdro_legal_review_required == busConstant.FLAG_YES && this.icdoPersonAccount.qdro_review_completed_date == DateTime.MinValue)
                            {
                                lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.QDRO_LEGAL_REVIEW_REQUIRED, busConstant.FLAG_YES);
                            }
                            else if (this.icdoPersonAccount.qdro_review_completed_date != DateTime.MinValue)
                            {
                                lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.QDRO_LEGAL_REVIEW_REQUIRED, busConstant.FLAG_NO);
                            }
                        }
                    }
                }
            }
        }
        public ArrayList ValidateQDROLegalReviewRequired()
        {
            ArrayList larrResult = new ArrayList();
            if (ibusBaseActivityInstance != null)
            {
                busSolBpmActivityInstance lbusBpmActivityInstance = ibusBaseActivityInstance as busSolBpmActivityInstance;
                if (lbusBpmActivityInstance != null)
                {
                    string istrExSpouse = busConstant.NO;
                    string IsActiveLegalDocument = string.Empty;

                    busPerson ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                    ibusPerson.FindPerson(icdoPersonAccount.person_id);
                    DataTable ldtblist = busPerson.Select("cdoPerson.LoadBeneficaryForOverview", new object[1] { icdoPersonAccount.person_id });
                    iclbPersonAccountBeneficiary = GetCollection<busPersonAccountBeneficiary>(ldtblist, "icdoPersonAccountBeneficiary");
                    foreach (busPersonAccountBeneficiary objbusPersonAccountBeneficiary in iclbPersonAccountBeneficiary)
                    {
                        busRelationship objbusRelationship = new busRelationship();
                        objbusRelationship.FindRelationship(objbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.person_relationship_id);
                        objbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.dtdateOfMarriage = objbusRelationship.icdoRelationship.date_of_marriage;
                        if (objbusRelationship.icdoRelationship.relationship_value== busConstant.BENEFICIARY_RELATIONSHIP_EXSPOUSE)
                        {
                            istrExSpouse = busConstant.FLAG_YES;
                        }
                    }
                    DataTable ldtblist1 = busPerson.Select("entDroApplication.CheckLegalDocument", new object[1] { this.icdoPersonAccount.person_id });
                    if (ldtblist1.Rows.Count > 0)
                    {
                        IsActiveLegalDocument = busConstant.FLAG_YES;
                    }
                    if ((ibusPerson.IsNotNull() && ibusPerson.icdoPerson.marital_status_value == busConstant.MARITAL_STATUS_DIVORCED) || istrExSpouse == busConstant.FLAG_YES || IsActiveLegalDocument == busConstant.FLAG_YES)
                    {
                        lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.QDRO_LEGAL_REVIEW_REQUIRED, busConstant.FLAG_YES);
                        this.icdoPersonAccount.qdro_legal_review_required = busConstant.FLAG_YES;
                        this.icdoPersonAccount.Update();
                    }
                    else
                    {
                        lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.QDRO_LEGAL_REVIEW_REQUIRED, busConstant.FLAG_NO);
                    }
                }
            }
            return larrResult;
        }
        public void LoadBeneficiaries()
        {
            DataTable ldtblist = busPerson.Select("cdoPerson.LoadAllBeneficiaries", new object[1] { this.icdoPersonAccount.person_id });
            iclbPersonAccountBeneficiary = GetCollection<busPersonAccountBeneficiary>(ldtblist, "icdoPersonAccountBeneficiary");
        }
        public DateTime LoadServiceRetirementDate(busSolBpmActivityInstance lbusBpmActivityInstance)
        {
            int iintCalculationId = Convert.ToInt32(lbusBpmActivityInstance.GetBpmParameterValue(busConstant.PersonAccountMaintenance.BENEFIT_CALCULATION_ID));
            DateTime itdRetrementDate = Convert.ToDateTime(lbusBpmActivityInstance.GetBpmParameterValue(busConstant.PersonAccountMaintenance.BENEFIT_RETIREMENT_DATE));


            DataTable ldtblist = busPerson.Select("entBenefitCalculationHeader.ServiceRetirementDate", new object[3] { this.icdoPersonAccount.person_id,this.icdoPersonAccount.plan_id, iintCalculationId });
            if (ldtblist.Rows.Count > 0)
            {
                DateTime fullDateTime = Convert.ToDateTime(ldtblist.Rows[0][0]);
                this.idtRetirementDate = fullDateTime;
                return GetDateBeforeRetirement(idtRetirementDate);
            }
            else
            {
                DateTime today = DateTime.Today;
                DateTime resultDate = today.AddDays(60);
                DateTime eligibleDate = new DateTime(resultDate.Year, resultDate.Month, 1).AddMonths(1);
                this.idtRetirementDate = eligibleDate;
                return eligibleDate;
            }
        }
        public DateTime GetDateBeforeRetirement(DateTime retirementDate)
        {
            return retirementDate.AddDays(-50);
        }
        public ArrayList BtnSubmitClick()
        {
            ArrayList larrResult = new ArrayList();
            utlError lobjError = new utlError();
            busSolBpmActivityInstance lbusBpmActivityInstance = ibusBaseActivityInstance as busSolBpmActivityInstance;
            if (lbusBpmActivityInstance != null && lbusBpmActivityInstance.ibusBpmActivity.icdoBpmActivity.name != busConstant.PersonAccountMaintenance.GENERATE_RETIREMENT_APPLICATION_CANCELLATION_NOTICE_ACTIVITY)
            {
                SetBPMActivityInstanceParameters();
            }
            if (this.icdoPersonAccount.qdro_review_completed_date !=DateTime.MinValue)
            {
                this.icdoPersonAccount.Update();
            }
            if (lbusBpmActivityInstance != null && lbusBpmActivityInstance.ibusBpmActivity.icdoBpmActivity.name == busConstant.PersonAccountMaintenance.QDRO_LEAGL_REVIEW_BENEFIT_ELECTION_ACTIVITY)
            {
                if (this.icdoPersonAccount.qdro_legal_review_required != busConstant.FLAG_YES && this.icdoPersonAccount.election_packet_sent != busConstant.FLAG_YES)
                {
                    lobjError = AddError(7014, "");
                    larrResult.Add(lobjError);
                    return larrResult;
                }
            }
            else if (lbusBpmActivityInstance != null && lbusBpmActivityInstance.ibusBpmActivity.icdoBpmActivity.name == busConstant.PersonAccountMaintenance.LEGAL_DOCUMENT_REVIEW_ACTIVITY)
            {
                if (this.icdoPersonAccount.qdro_legal_review_required == busConstant.FLAG_YES && this.icdoPersonAccount.qdro_review_completed_date == DateTime.MinValue)
                {
                    lobjError = AddError(7013, "");
                    larrResult.Add(lobjError);
                    return larrResult;
                }
                else if (this.icdoPersonAccount.qdro_legal_review_required == busConstant.FLAG_YES && this.icdoPersonAccount.qdro_review_completed_date != DateTime.MinValue)
                {
                    lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.QDRO_LEGAL_REVIEW_FLAG, busConstant.FLAG_NO);

                }
            }
            else if (lbusBpmActivityInstance != null && lbusBpmActivityInstance.ibusBpmActivity.icdoBpmActivity.name == busConstant.PersonAccountMaintenance.GENERATE_RETIREMENT_APPLICATION_CANCELLATION_NOTICE_ACTIVITY)
            {
                lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.GENERATE_CANCELLATION_NOTICE, IsCancellationNoticeGenerated());
                lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.PAYEE_ACCOUNT_EXISTS, LoadPayeeAccount());
                lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.PAYMENT_BEGIN_DATE_PROCESSED, IsRequirePayeeAudit());
                lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.SERVICE_RETIREMENT_APPLICATION_EXISTS, IsApplicationExist());
                lbusBpmActivityInstance.UpdateParameterValue(busConstant.PersonAccountMaintenance.APPLICATION_ID, GetApplicationID());
            }
            larrResult.Add(this);
            return larrResult;
        }

        public string LoadPayeeAccount()
        {
            DataTable ldtbPayeeAccount = busBase.Select("entPersonAccount.IsPayeeAccountExits", new object[2] { this.icdoPersonAccount.person_id, this.icdoPersonAccount.plan_id });
            iclbPayeeAccount = GetCollection<busPayeeAccount>(ldtbPayeeAccount, "icdoPayeeAccount");

            if (iclbPayeeAccount.Count() > 0)
            {
                return busConstant.FLAG_YES;
            }
            return busConstant.FLAG_NO;
        }
        public string IsRequirePayeeAudit()
        {
            DataTable ldtbPayeeAccount = busBase.Select("entPersonAccount.IsPayeeAuditRequired", new object[2] { this.icdoPersonAccount.person_id, this.icdoPersonAccount.plan_id });
            iclbPayeeAccount = GetCollection<busPayeeAccount>(ldtbPayeeAccount, "icdoPayeeAccount");
            
            if (iclbPayeeAccount.Count() > 0)
            {
                return busConstant.FLAG_YES;
            }
            return busConstant.FLAG_NO;
        }
        public string IsApplicationExist()
        {
            DataTable ldtbApplicationExists = busBase.Select("entPersonAccount.ApplicationExists", new object[2] { this.icdoPersonAccount.person_id, this.icdoPersonAccount.istrPlan });

            if (ldtbApplicationExists.Rows.Count > 0)
            {
                if (ldtbApplicationExists.AsEnumerable().Any(row => row.Field<string>("APPLICATION_STATUS_VALUE") == busConstant.PersonAccountMaintenance.APPLICATION_APPROVE_STATUS))
                {
                    return busConstant.FLAG_YES;
                }
            }
            return busConstant.FLAG_NO;
        }
        public int GetApplicationID()
        {
            DataTable ldtbApplicationExists = busBase.Select("entPersonAccount.ApplicationExists", new object[2] { this.icdoPersonAccount.person_id, this.icdoPersonAccount.istrPlan });

            if (ldtbApplicationExists.Rows.Count > 0)
            {
                var ApplicationId = ldtbApplicationExists.AsEnumerable().Select(row => row.Field<int?>("BENEFIT_APPLICATION_ID")).FirstOrDefault();

                return Convert.ToInt32(ApplicationId);
            }
            return 0;
        }

        public string IsSignedDocumentReceived()
        {
            if (this.icdoPersonAccount.signed_application_forn_received_date != DateTime.MinValue)
            {
                return busConstant.FLAG_YES;
            }
            return busConstant.FLAG_NO;
        }
        public string IsCancellationNoticeGenerated()
        {
            DataTable ldtbApplicationExists = busBase.Select("entPersonAccount.LoadGeneratedCancellationNotice", new object[1] { this.icdoPersonAccount.person_id });

            if (ldtbApplicationExists.Rows.Count > 0)
            {
                return busConstant.FLAG_YES;
            }
            return busConstant.FLAG_NO;
        }
        public void CancelApplicationNote(int aintPersonAccountId, string aintSourceOfApplication)
        {
            busPersonAccount lobjPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            lobjPersonAccount.FindPersonAccount(aintPersonAccountId);
            lobjPersonAccount.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lobjPersonAccount.ibusPerson.FindPerson(lobjPersonAccount.icdoPersonAccount.person_id);
            busNotes lobjNotes = new busNotes { icdoNotes = new cdoNotes() };
            lobjNotes.icdoNotes.person_id = lobjPersonAccount.icdoPersonAccount.person_id;
            lobjNotes.icdoNotes.form_id = busConstant.Form_ID;
            lobjNotes.icdoNotes.form_value = busConstant.PersonAccountMaintenance.SERVICE_RETIREMENT;
            if (aintSourceOfApplication == busConstant.PersonAccountMaintenance.BPM_TIMER_EXPIRTU_BEFORE_APPLICATION)
            {
                lobjNotes.icdoNotes.notes = "Retirement Application Cancellation Notice sent. to the " + lobjPersonAccount.ibusPerson.icdoPerson.mpi_person_id;
            }
            else if (aintSourceOfApplication == busConstant.PersonAccountMaintenance.BPM_TIMER_EXPIRTU_BEFORE_ELECTION)
            {
                lobjNotes.icdoNotes.notes = "Retirement Packet Cancellation Notice sent. to the " + lobjPersonAccount.ibusPerson.icdoPerson.mpi_person_id;
            }
            else
            {
                lobjNotes.icdoNotes.notes = "Retirement  Application Cancellation Notice  sent. to the " + lobjPersonAccount.ibusPerson.icdoPerson.mpi_person_id;
            }
            lobjNotes.icdoNotes.created_by = this.iobjPassInfo.istrUserID;
            lobjNotes.icdoNotes.created_date = DateTime.Now;
            lobjNotes.icdoNotes.modified_date = DateTime.Now;
            lobjNotes.icdoNotes.modified_by = this.iobjPassInfo.istrUserID;
            lobjNotes.icdoNotes.ienuObjectState = ObjectState.Insert;
            lobjNotes.icdoNotes.Insert();
        }

        public override ArrayList OnBpmSubmit()
        {
            return base.OnBpmSubmit();
        }
    }
}
