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
    /// Class MPIPHP.BusinessObjects.busReturnedMail:
    /// Inherited from busReturnedMailGen, the class is used to customize the business object busReturnedMailGen.
    /// </summary>
    [Serializable]
    public class busReturnedMail : busReturnedMailGen
    {
        public busPerson ibusPerson { get; set; }
        public busPerson ibusLookupPerson { get; set; }
        public busOrganization ibusLookupOrganization { get; set; }
        public busPersonAddress ibusPersonAddress { get; set; }
        public busOrganization ibusOrganization { get; set; }
        public busOrgAddress ibusOrgAddress { get; set; }
        public busDocument ibusDocument { get; set; }
        public string astrVipAcc { get; set; }
        public Collection<busOrgAddress> iclbOrgAddress { get; set; }
        public Collection<busPerson> iclbPerson { get; set; }
        public Collection<busOrganization> iclbOrganization { get; set; }
        public bool iblnAddressChangeError { get; set; }
        public bool iblnisLastStep { get; set; }
        public bool iblnchanged { get; set; }

        public virtual void LoadAllPersonAddresss()
        {
            DataTable ldtbList = Select<cdoPersonAddress>(
                new string[1] { enmPersonAddress.person_id.ToString() },
                new object[1] { ibusPerson.icdoPerson.person_id }, null, null);
            iclbPersonAddress = GetCollection<busPersonAddress>(ldtbList, "icdoPersonAddress");
        }
        public virtual void LoadPersonAddress()
        {
            ibusPersonAddress = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() };
            ibusPersonAddress.FindPersonAddress(icdoReturnedMail.address_id);
        }

        public virtual void LoadAllOrgAddress()
        {
            DataTable ldtbList = Select<cdoOrgAddress>(
                new string[1] { enmOrgAddress.org_id.ToString() },
                new object[1] { ibusOrganization.icdoOrganization.org_id }, null, null);
            iclbOrgAddress = GetCollection<busOrgAddress>(ldtbList, "icdoOrgAddress");
        }
        public virtual void LoadOrgAddress()
        {
            ibusOrgAddress = new busOrgAddress { icdoOrgAddress = new cdoOrgAddress() };
            ibusOrgAddress.FindOrgAddress(icdoReturnedMail.org_address_id);
        }
        public Boolean CheckReturFlagPerson()
        {
            Boolean result = false;
            if (iclbPersonAddress != null && iclbPersonAddress.Count != 0)
            {
                int rescount = iclbPersonAddress.Where(obj => obj.IsReturnFlag == busConstant.FLAG_YES).Count();
                if (rescount == 1)
                {
                    result = true;
                }
            }
            else
            {
                result = true;
            }
            return result;
        }
        public Boolean CheckSelectedFlagPerson()
        {
            Boolean result = false;
            if (iclbPerson != null)
            {
                int rescount = iclbPerson.Where(obj => obj.icdoPerson.istrSelectedFlag == busConstant.FLAG_YES).Count();
                if (rescount == 1)
                {
                    result = true;
                }
            }

            return result;
        }
        public Boolean CheckVIPFlagPerson()
        {
            Boolean result = true;
            if (iclbPerson != null)
            {
                int rescount = iclbPerson.Where(obj => obj.icdoPerson.vip_flag == busConstant.FLAG_YES && obj.icdoPerson.istrSelectedFlag == busConstant.FLAG_YES).Count();
                if (astrVipAcc != busConstant.FLAG_YES && rescount > 0)
                {
                    result = false;
                }
            }

            return result;
        }


        // CHECK IF RETURN MAIL DATE IS FUTURE DATE
        public Boolean CheckreturnMailDatePerson()
        {
            Boolean result = true;
            if (iclbPersonAddress != null && iclbPersonAddress.Count != 0)
            {
                busPersonAddress lbusPersonAddress = iclbPersonAddress.Where(obj => obj.IsReturnFlag == busConstant.FLAG_YES).FirstOrDefault();
                if (lbusPersonAddress != null)
                {

                    if (lbusPersonAddress.icdoPersonAddress.end_date > DateTime.Now)
                    {
                        result = false;
                    }
                }
            }

            return result;
        }
        //PIR 1000
        public Boolean CheckOnlyOneReturnMailFlag()
        {
            Boolean result = false;
            if (iclbPersonAddress != null && iclbPersonAddress.Count != 0 &&
                iclbPersonAddress.Where(obj => obj.IsReturnFlag == busConstant.FLAG_YES).Count() == 1)
            {
                result = true;
            }
            else if ((iclbPersonAddress == null || (iclbPersonAddress != null && iclbPersonAddress.Count == 0)
                || (iclbPersonAddress != null && iclbPersonAddress.Count > 0 && iclbPersonAddress.Where(obj => obj.IsReturnFlag == busConstant.FLAG_YES).Count() == 0))
                && !string.IsNullOrEmpty(icdoReturnedMail.notes))
            {
                result = true;
            }

            return result;
        }


        //PIR 1050
        public Boolean IsManagerApprovalRequiredForAddressChange()
        {
            //iclbPersonAddress = iclbPersonAddress.Where(obj => obj.IsReturnFlag == busConstant.FLAG_YES).ToList().ToCollection();

            ibusPersonAddress = iclbPersonAddress.Where(obj => obj.IsReturnFlag == busConstant.FLAG_YES).FirstOrDefault();
            ibusPerson.ibusPersonAddress = iclbPersonAddress.Where(obj => obj.IsReturnFlag == busConstant.FLAG_YES).FirstOrDefault();
            if (ibusPersonAddress != null)
            {
                if (Convert.ToDateTime(ibusPersonAddress.icdoPersonAddress.ihstOldValues["end_date"]) == DateTime.MinValue && ibusPersonAddress.icdoPersonAddress.end_date != DateTime.MinValue)
                {
                    if (Convert.ToDateTime(ibusPerson.ibusPersonAddress.icdoPersonAddress.ihstOldValues["end_date"]) == DateTime.MinValue && ibusPerson.ibusPersonAddress.icdoPersonAddress.end_date != DateTime.MinValue)
                    {
                        DataTable ldtbPayeeAccountIDs = Select("cdoPersonAddress.GetPayeeAccounts", new object[1] { ibusPerson.ibusPersonAddress.icdoPersonAddress.person_id });
                        if (ldtbPayeeAccountIDs.Rows.Count > 0)
                        {
                            foreach (DataRow ldrPayeeAccountId in ldtbPayeeAccountIDs.Rows)
                            {
                                bool lblnCheckToChangeStatus;
                                busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                                if (Convert.ToString(ldrPayeeAccountId[enmPayeeAccount.payee_account_id.ToString().ToUpper()]).IsNotNullOrEmpty() && lbusPayeeAccount.FindPayeeAccount(Convert.ToInt32(ldrPayeeAccountId[enmPayeeAccount.payee_account_id.ToString().ToUpper()])))
                                {

                                    lbusPayeeAccount.LoadBenefitDetails();
                                    lbusPayeeAccount.LoadDRODetails();

                                    lbusPayeeAccount.FindPayeeAccount(Convert.ToInt32(ldrPayeeAccountId[enmPayeeAccountStatus.payee_account_id.ToString().ToUpper()]));
                                    lblnCheckToChangeStatus = lbusPayeeAccount.CheckACHDetails();
                                    if (lblnCheckToChangeStatus == busConstant.BOOL_TRUE)
                                        continue;

                                    string istrScheduleType = string.Empty;
                                    if (lbusPayeeAccount.icdoPayeeAccount.iintPlanId != 1)
                                        istrScheduleType = busConstant.PaymentScheduleTypeMonthly;
                                    else
                                        istrScheduleType = busConstant.PaymentScheduleTypeWeekly;

                                    bool IsManager = false;
                                    busUserRoles lbusUserRoles = new busUserRoles { icdoRoles = new cdoRoles() };
                                    if (lbusUserRoles.FindUserRoles(iobjPassInfo.iintUserSerialID, busConstant.Role.MANAGER_ROLE))
                                    {
                                        IsManager = true;
                                    }

                                    int lintCountSchedule = (int)DBFunction.DBExecuteScalar("cdoPerson.CheckPaymentSchedulebeforeApprovestatus", new object[1] { istrScheduleType }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                                    if (lintCountSchedule > 0 && !IsManager)
                                    {
                                        //utlError lutlError = new utlError();
                                        //lutlError = AddError(6223, "");
                                        //this.iarrErrors.Add(lutlError);
                                        return true;
                                    }
                                }
                            }

                        }
                    }
                }
            }
            return false;
        }

        public Boolean CheckReturFlagOrg()
        {
            Boolean result = false;
            if (iclbOrgAddress != null && iclbOrgAddress.Count != 0)
            {
                int rescount = iclbOrgAddress.Where(obj => obj.IsReturnFlag == busConstant.FLAG_YES).Count();
                if (rescount == 1)
                {
                    result = true;
                }
            }
            else
            {
                result = true;
            }
            return result;
        }
        public Boolean CheckSelectedFlagOrg()
        {
            Boolean result = false;
            if (iclbPerson != null)
            {
                int rescount = iclbOrganization.Where(obj => obj.icdoOrganization.istrSelectedFlag == busConstant.FLAG_YES).Count();
                if (rescount == 1)
                {
                    result = true;
                }
            }

            return result;
        }
        public override void BeforePersistChanges()
        {
            if (iclbPerson != null)
            {
                if (ibusLookupPerson != null)
                {
                    if (this.iarrChangeLog.Contains(ibusLookupPerson.icdoPerson))
                    {
                        this.iarrChangeLog.Remove(ibusLookupPerson.icdoPerson);
                    }
                }

                ibusPerson = iclbPerson.Where(obj => obj.icdoPerson.istrSelectedFlag == busConstant.FLAG_YES).FirstOrDefault();

            }
            if (iclbOrganization != null)
            {
                if (this.iarrChangeLog.Contains(ibusLookupOrganization.icdoOrganization))
                {
                    this.iarrChangeLog.Remove(ibusLookupOrganization.icdoOrganization);
                }

                ibusOrganization = iclbOrganization.Where(obj => obj.icdoOrganization.istrSelectedFlag == busConstant.FLAG_YES).FirstOrDefault();

            }
            if (!string.IsNullOrEmpty(icdoReturnedMail.doc_Id_Source) && icdoReturnedMail.doc_Id_Source.Contains(','))
            {
                string[] spl = icdoReturnedMail.doc_Id_Source.Split(",");
                icdoReturnedMail.doc_id = Convert.ToInt32(spl[0]);
                icdoReturnedMail.doc_type_source_value = Convert.ToString(spl[1]);
            }
            //if (ibusPerson!=null && ibusPerson.icdoPerson.person_id!=0)
            //{
            //    ibusPerson.FindPerson(ibusPerson.icdoPerson.person_id);
            //}
            //if (ibusOrganization != null && ibusOrganization.icdoOrganization.org_id != 0)
            //{
            //    ibusOrganization.FindOrganization(ibusOrganization.icdoOrganization.org_id);
            //}


            if (iclbPersonAddress != null)
            {
                //PIR 1000
                iclbPersonAddress = iclbPersonAddress.Where(obj => obj.IsReturnFlag == busConstant.FLAG_YES).ToList().ToCollection();

                ibusPersonAddress = iclbPersonAddress.Where(obj => obj.IsReturnFlag == busConstant.FLAG_YES).FirstOrDefault();
                ibusPerson.ibusPersonAddress = iclbPersonAddress.Where(obj => obj.IsReturnFlag == busConstant.FLAG_YES).FirstOrDefault();
                if (ibusPersonAddress != null)
                {
                    icdoReturnedMail.address_id = ibusPersonAddress.icdoPersonAddress.address_id;
                    icdoReturnedMail.Update();

                    if (Convert.ToDateTime(ibusPersonAddress.icdoPersonAddress.ihstOldValues[enmPersonAddress.end_date.ToString()]) == DateTime.MinValue &&
                        Convert.ToDateTime(ibusPersonAddress.icdoPersonAddress.ihstOldValues[enmPersonAddress.end_date.ToString()]) != ibusPersonAddress.icdoPersonAddress.end_date
                        && ibusPersonAddress.icdoPersonAddress.secured_flag != busConstant.FLAG_YES)//PIR 1000
                    {
                        iblnchanged = true;
                    }
                    //PIR 1000
                    else
                    {
                        this.iarrChangeLog.Clear();
                    }

                    if (ibusPerson.ibusPersonAddress.icdoPersonAddress.end_date != DateTime.MinValue)
                        ibusPerson.ibusPersonAddress.icdoPersonAddress.bad_address_flag = busConstant.FLAG_YES;
                }
            }


            if (iclbOrgAddress != null)
            {
                ibusOrgAddress = iclbOrgAddress.Where(obj => obj.IsReturnFlag == busConstant.FLAG_YES).FirstOrDefault();
                if (ibusOrgAddress != null)
                {
                    icdoReturnedMail.org_address_id = ibusOrgAddress.icdoOrgAddress.org_address_id;
                    icdoReturnedMail.Update();
                }
            }
            if (iblnisLastStep)
            {
                if (this.ibusPerson.IsNotNull() && this.ibusPerson.icdoPerson.IsNotNull() && this.icdoReturnedMail.person_id <= 0)
                {
                    if (this.ibusPerson.icdoPerson.person_id > 0)
                    {
                        this.icdoReturnedMail.person_id = this.ibusPerson.icdoPerson.person_id;
                        icdoReturnedMail.Update();
                    }
                }
                if (this.ibusOrganization.IsNotNull() && this.ibusOrganization.icdoOrganization.IsNotNull() && this.icdoReturnedMail.org_id <= 0)
                {
                    if (this.ibusOrganization.icdoOrganization.org_id > 0)
                    {
                        this.icdoReturnedMail.org_id = this.ibusOrganization.icdoOrganization.org_id;
                        icdoReturnedMail.Update();
                    }
                }
            }

            base.BeforePersistChanges();


        }
        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            if (iclbPersonAddress != null)
            {
                if (ibusPersonAddress != null && iblnchanged)
                {
                    #region //PIR 1050
                    //if (Convert.ToDateTime(ibusPersonAddress.icdoPersonAddress.ihstOldValues["end_date"]) == DateTime.MinValue && ibusPersonAddress.icdoPersonAddress.end_date != DateTime.MinValue)
                    //{
                    DataTable ldtbPayeeAccountIDs = Select("cdoPersonAddress.GetPayeeAccounts", new object[1] { ibusPersonAddress.icdoPersonAddress.person_id });
                    if (ldtbPayeeAccountIDs.Rows.Count > 0)
                    {
                        bool lblnFlg = busConstant.BOOL_FALSE, lblnCheckToChangeStatus;
                        foreach (DataRow ldrPayeeAccountId in ldtbPayeeAccountIDs.Rows)
                        {
                            busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                            lbusPayeeAccount.FindPayeeAccount(Convert.ToInt32(ldrPayeeAccountId[enmPayeeAccountStatus.payee_account_id.ToString().ToUpper()]));
                            lblnCheckToChangeStatus = lbusPayeeAccount.CheckACHDetails();
                            if (lblnCheckToChangeStatus != busConstant.BOOL_TRUE)
                            {
                                cdoPayeeAccountStatus lcdoPayeeAccountStatus = new cdoPayeeAccountStatus();
                                //RID 79754 Approved and Receiving payee account should be marked suspended not review
                                if (ldrPayeeAccountId[1].ToString() == busConstant.PAYEE_ACCOUNT_STATUS_RECEIVING || ldrPayeeAccountId[1].ToString() == busConstant.PAYEE_ACCOUNT_STATUS_APPROVED)
                                {

                                    lcdoPayeeAccountStatus.payee_account_id = Convert.ToInt32(ldrPayeeAccountId[enmPayeeAccountStatus.payee_account_id.ToString().ToUpper()]);
                                    lcdoPayeeAccountStatus.status_id = busConstant.Payee_Account_Status_ID;
                                    //lcdoPayeeAccountStatus.status_value = busConstant.PAYEE_ACCOUNT_STATUS_REVIEW;
                                    lcdoPayeeAccountStatus.status_value = busConstant.PAYEE_ACCOUNT_STATUS_SUSPENDED;
                                    lcdoPayeeAccountStatus.status_effective_date = DateTime.Now;
                                    lcdoPayeeAccountStatus.Insert();
                                    lblnFlg = busConstant.BOOL_TRUE;
                                }
                                //PROD PIR 269
                                if (Convert.ToInt32(ldrPayeeAccountId[enmPayeeAccountStatus.payee_account_id.ToString().ToUpper()]) > 0)
                                {
                                    busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
                                    if (lobjPayeeAccount.FindPayeeAccount(Convert.ToInt32(ldrPayeeAccountId[enmPayeeAccountStatus.payee_account_id.ToString().ToUpper()])))
                                    {
                                        lobjPayeeAccount.CreateReactivatioRetroPaymentItem();
                                    }
                                }
                            }
                        }
                        if (lblnFlg == busConstant.BOOL_TRUE)
                        {
                            cdoNotes lcdoNotes = new cdoNotes();
                            lcdoNotes.person_id = ibusPerson.ibusPersonAddress.icdoPersonAddress.person_id;
                            //RID 79754 Approved and Receiving payee account should be marked suspended not review
                            //lcdoNotes.notes = "Payee account status changed to review as Mailing address is end dated via Returned Mail";
                            lcdoNotes.notes = "Payee account status changed to suspended as Mailing address is end dated via Returned Mail";
                            lcdoNotes.Insert();
                        }
                    }
                    //}
                    #endregion //PIR 1050

                    ibusPerson.ibusPersonAddress.icdoPersonAddress.Update();//PIR 1000
                  // decommissioning demographics informations, since HEDB is retiring.
                  //  UpdateAddress();
                }

            }
            if (ibusPerson != null && ibusPerson.icdoPerson.person_id != 0)
            {
                this.LoadAllPersonAddresss();
                //PIR 515
                if (this.iclbPersonAddress != null)
                {
                    //PIR 1000
                    iclbPersonAddress.ForEach(i => i.iblnReturnedMail = true);
                    ibusPersonAddress = iclbPersonAddress.Where(item => item.icdoPersonAddress.end_date == DateTime.MinValue).FirstOrDefault();
                    if (ibusPersonAddress != null && this.icdoReturnedMail.returned_mail_date != DateTime.MinValue)
                    {
                        ibusPersonAddress.icdoPersonAddress.end_date = DateTime.Now.AddDays(-1);
                    }
                }
            }
            if (ibusOrganization != null && ibusOrganization.icdoOrganization.org_id != 0)
            {
                this.LoadAllOrgAddress();
            }
        }

        //FM upgrade: 6.0.2.1 changes - method signature changed - added 4th param
        public override void BeforeWizardStepValidate(utlPageMode aenmPageMode, string astrWizardName, string astrWizardStepName, utlWizardNavigationEventArgs we = null)
        {
            iblnisLastStep = false;
            if (this.iarrBusChangeLog.Count > 0)
            {
                if (iclbPerson != null)
                {
                    ArrayList arr = new ArrayList();
                    foreach (var obj in this.iarrBusChangeLog)
                    {
                        arr.Add(obj);
                    }
                    foreach (var obj in arr)
                    {
                        if (obj is busPerson)
                        {
                            this.iarrBusChangeLog.Remove(obj);
                        }
                    }
                    foreach (busPerson lbusPerson in iclbPerson)
                    {
                        if (this.iarrBusChangeLog.Contains(lbusPerson))
                        {
                            this.iarrBusChangeLog.Remove(lbusPerson);
                        }
                    }
                }
                if (iclbPersonAddress != null)
                {
                    foreach (busPersonAddress lbusPersonAddress in iclbPersonAddress)
                    {
                        if (!lbusPersonAddress.icdoPersonAddress.ihstOldValues.IsNullOrEmpty())
                        {
                            if (Convert.ToString(lbusPersonAddress.icdoPersonAddress.ihstOldValues["end_date"]).IsNotNullOrEmpty())
                            {
                                if (Convert.ToDateTime(lbusPersonAddress.icdoPersonAddress.ihstOldValues["end_date"]) != DateTime.MinValue)
                                {
                                    if (Convert.ToDateTime(lbusPersonAddress.icdoPersonAddress.ihstOldValues["end_date"]).Date != lbusPersonAddress.icdoPersonAddress.end_date.Date)
                                    {
                                        iblnAddressChangeError = true;
                                    }
                                }
                            }
                        }
                    }
                }
                //if (this.iarrBusChangeLog.Contains(ibusLookupOrganization))
                //{
                //    this.iarrBusChangeLog.Remove(ibusLookupOrganization);
                //}
                //if (this.iarrBusChangeLog.Contains(ibusLookupPerson))
                //{
                //    this.iarrBusChangeLog.Remove(ibusLookupPerson);
                //}
                //if (this.iarrBusChangeLog.Contains(ibusPerson))
                //{
                //    this.iarrBusChangeLog.Remove(ibusPerson);
                //}
                //if (this.iarrBusChangeLog.Contains(ibusOrganization))
                //{
                //    this.iarrBusChangeLog.Remove(ibusOrganization);
                //}
                //if (ibusLookupPerson != null)
                //{
                //    if (this.iarrChangeLog.Contains(ibusLookupPerson.icdoPerson))
                //    {
                //        this.iarrChangeLog.Remove(ibusLookupPerson.icdoPerson);
                //    }
                //}
                if (iclbOrganization != null)
                {
                    ArrayList arr = new ArrayList();
                    foreach (var obj in this.iarrBusChangeLog)
                    {
                        arr.Add(obj);
                    }
                    foreach (var obj in arr)
                    {
                        if (obj is busOrganization)
                        {
                            this.iarrBusChangeLog.Remove(obj);
                        }
                    }
                    foreach (busOrganization lbusOrganization in iclbOrganization)
                    {
                        if (this.iarrBusChangeLog.Contains(lbusOrganization))
                        {
                            this.iarrBusChangeLog.Remove(lbusOrganization);
                        }
                    }
                }
            }
            if (astrWizardStepName == "wzsReviewUpdateAddress")
            {
                iblnisLastStep = true;
            }
            base.BeforeWizardStepValidate(aenmPageMode, astrWizardName, astrWizardStepName);
        }

        public Boolean LoadDoucument()
        {
            Boolean res = false;
            ibusDocument = new busDocument { icdoDocument = new cdoDocument() };
            DataTable ldtbres = busBase.Select("cdoDocument.LoadDocumentTypeBasedOnSource", new object[2] { icdoReturnedMail.doc_id, icdoReturnedMail.doc_type_source_value });
            if (icdoReturnedMail.doc_id != 0)
            {
                if (ldtbres != null && ldtbres.Rows.Count > 0)
                {
                    ibusDocument.icdoDocument.LoadData(ldtbres.Rows[0]);
                }
                res = true;
            }
            else
            {
                ibusDocument.icdoDocument.doc_description = icdoReturnedMail.other_document_type;
                res = true;
            }
            return res;
        }
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            base.LoadOtherObjects(adtrRow, aobjBus);
        }

        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            base.ValidateHardErrors(aenmPageMode);

        }

        #region Update Address
        public void UpdateAddress()
        {
            #region Update address on HEDB
            if (ibusPerson.ibusPersonAddress.icdoPersonAddress.start_date <= DateTime.Now)
            {
                string lstrContactName = string.Empty;
                string lstrAddrline1 = string.Empty;
                string lstrAddrline2 = string.Empty;
                string lstrCity = string.Empty;
                string lstrState = string.Empty;
                string lstrPostalCode = string.Empty;
                string lstrCountry = string.Empty;
                string lstrCountryValue = string.Empty;
                int lintForeignAddrFlag = 0;
                int lintDoNotUpdate = 0;
                bool lbnIsContact = false;
                string lstrAuditUser = string.Empty;
                string lstrPersonMpiID = string.Empty;
                DateTime ldtAddressEndDate = DateTime.MinValue;
                string lstrAddressSource = string.Empty;
                string lstrBadAddressFlag = string.Empty;
                DateTime ldtAddressStartDate = DateTime.MinValue;


                int lintPersonID = ibusPerson.ibusPersonAddress.icdoPersonAddress.person_id;
                ibusPerson.iclbPersonContact = new Collection<busPersonContact>();

                DataTable ldtbPersonContactList = busBase.Select<cdoPersonContact>(
                    new string[1] { enmPersonContact.person_id.ToString() },
                    new object[1] { lintPersonID }, null, enmPersonContact.effective_start_date.ToString());

                foreach (DataRow ldtRow in ldtbPersonContactList.Rows)
                {
                    busPersonContact lbusPersonContact = new busPersonContact { icdoPersonContact = new cdoPersonContact() };
                    lbusPersonContact.icdoPersonContact.LoadData(ldtRow);


                    if (lbusPersonContact.icdoPersonContact.effective_end_date == DateTime.MinValue || lbusPersonContact.icdoPersonContact.effective_end_date > DateTime.Now)
                    {
                        if ((lbusPersonContact.icdoPersonContact.contact_type_value == "POAP" || lbusPersonContact.icdoPersonContact.contact_type_value == "GRDN"
                            || lbusPersonContact.icdoPersonContact.contact_type_value == "COAP")
                                && lbusPersonContact.icdoPersonContact.correspondence_addr_flag == "Y")
                        {
                            lbnIsContact = true;
                            lstrContactName = lbusPersonContact.icdoPersonContact.contact_name;
                            lstrAddrline1 = lbusPersonContact.icdoPersonContact.addr_line_1;
                            lstrAddrline2 = lbusPersonContact.icdoPersonContact.addr_line_2;
                            lstrCity = lbusPersonContact.icdoPersonContact.addr_city;

                            if (Convert.ToInt32(lbusPersonContact.icdoPersonContact.addr_country_value) == busConstant.USA || Convert.ToInt32(lbusPersonContact.icdoPersonContact.addr_country_value) == busConstant.AUSTRALIA
                                   || Convert.ToInt32(lbusPersonContact.icdoPersonContact.addr_country_value) == busConstant.CANADA || Convert.ToInt32(lbusPersonContact.icdoPersonContact.addr_country_value) == busConstant.MEXICO
                                   || Convert.ToInt32(lbusPersonContact.icdoPersonContact.addr_country_value) == busConstant.NewZealand)
                            {
                                lstrState = lbusPersonContact.icdoPersonContact.addr_state_value;
                            }
                            else
                            {
                                lstrState = lbusPersonContact.icdoPersonContact.foreign_province;
                            }

                            if (Convert.ToInt32(lbusPersonContact.icdoPersonContact.addr_country_value) == busConstant.USA)
                            {

                                lstrPostalCode = lbusPersonContact.icdoPersonContact.addr_zip_code + lbusPersonContact.icdoPersonContact.addr_zip_4_code;
                            }
                            else
                            {
                                lstrPostalCode = lbusPersonContact.icdoPersonContact.foreign_postal_code;
                            }

                            lstrCountry = lbusPersonContact.icdoPersonContact.addr_country_description;
                            if (!string.IsNullOrEmpty(lbusPersonContact.icdoPersonContact.addr_country_value))
                                lstrCountryValue = HelperUtil.GetData1ByCodeValue(lbusPersonContact.icdoPersonContact.addr_country_id, lbusPersonContact.icdoPersonContact.addr_country_value);

                            if (Convert.ToInt32(lbusPersonContact.icdoPersonContact.addr_country_value) == busConstant.USA)
                            {
                                lintForeignAddrFlag = 0;
                            }
                            else
                            {
                                lintForeignAddrFlag = 1;
                            }

                            lstrAuditUser = iobjPassInfo.istrUserID;

                            ldtAddressStartDate = lbusPersonContact.icdoPersonContact.effective_start_date;

                            if (lbusPersonContact.icdoPersonAddress.end_date.IsNotNull() && lbusPersonContact.icdoPersonAddress.end_date != DateTime.MinValue)
                                ldtAddressEndDate = lbusPersonContact.icdoPersonAddress.end_date;
                            else
                                ldtAddressEndDate = DateTime.MinValue;

                        }
                    }

                }

                if (!lbnIsContact)
                {
                    lstrAddrline1 = ibusPerson.ibusPersonAddress.icdoPersonAddress.addr_line_1;
                    lstrAddrline2 = ibusPerson.ibusPersonAddress.icdoPersonAddress.addr_line_2;
                    lstrCity = ibusPerson.ibusPersonAddress.icdoPersonAddress.addr_city;

                    if (Convert.ToInt32(ibusPerson.ibusPersonAddress.icdoPersonAddress.addr_country_value) == busConstant.USA || Convert.ToInt32(ibusPerson.ibusPersonAddress.icdoPersonAddress.addr_country_value) == busConstant.AUSTRALIA
                        || Convert.ToInt32(ibusPerson.ibusPersonAddress.icdoPersonAddress.addr_country_value) == busConstant.CANADA || Convert.ToInt32(ibusPerson.ibusPersonAddress.icdoPersonAddress.addr_country_value) == busConstant.MEXICO
                        || Convert.ToInt32(ibusPerson.ibusPersonAddress.icdoPersonAddress.addr_country_value) == busConstant.NewZealand)
                    {
                        lstrState = ibusPerson.ibusPersonAddress.icdoPersonAddress.addr_state_value;
                    }
                    else
                    {
                        lstrState = ibusPerson.ibusPersonAddress.icdoPersonAddress.foreign_province;
                    }

                    if (Convert.ToInt32(ibusPerson.ibusPersonAddress.icdoPersonAddress.addr_country_value) == busConstant.USA)
                    {

                        lstrPostalCode = ibusPerson.ibusPersonAddress.icdoPersonAddress.addr_zip_code + ibusPerson.ibusPersonAddress.icdoPersonAddress.addr_zip_4_code;
                    }
                    else
                    {
                        lstrPostalCode = ibusPerson.ibusPersonAddress.icdoPersonAddress.foreign_postal_code;
                    }

                    lstrCountry = ibusPerson.ibusPersonAddress.icdoPersonAddress.addr_country_description;
                    if (!string.IsNullOrEmpty(ibusPerson.ibusPersonAddress.icdoPersonAddress.addr_country_value))
                    {
                        lstrCountryValue = HelperUtil.GetData1ByCodeValue(ibusPerson.ibusPersonAddress.icdoPersonAddress.addr_country_id, ibusPerson.ibusPersonAddress.icdoPersonAddress.addr_country_value);
                    }
                    if (Convert.ToInt32(ibusPerson.ibusPersonAddress.icdoPersonAddress.addr_country_value) == busConstant.USA)
                    {
                        lintForeignAddrFlag = 0;
                    }
                    else
                    {
                        lintForeignAddrFlag = 1;
                    }

                    if (ibusPerson.ibusPersonAddress.icdoPersonAddress.secured_flag == "Y")
                    {
                        lintDoNotUpdate = 1;
                    }
                    else
                    {
                        lintDoNotUpdate = 0;
                    }

                    lstrAuditUser = iobjPassInfo.istrUserID;

                    ldtAddressStartDate = ibusPerson.ibusPersonAddress.icdoPersonAddress.start_date;

                    if (ibusPerson.ibusPersonAddress.icdoPersonAddress.end_date.IsNotNull() && ibusPerson.ibusPersonAddress.icdoPersonAddress.end_date != DateTime.MinValue)
                        ldtAddressEndDate = ibusPerson.ibusPersonAddress.icdoPersonAddress.end_date;
                    else
                        ldtAddressEndDate = DateTime.MinValue;

                    lstrAddressSource = ibusPerson.ibusPersonAddress.icdoPersonAddress.addr_source_description;
                    lstrBadAddressFlag = ibusPerson.ibusPersonAddress.icdoPersonAddress.bad_address_flag;
                }
                ////On EADB

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

                //    lstrPersonMpiID = this.ibusPerson.icdoPerson.mpi_person_id;

                //    Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();

                //    IDbDataParameter lobjParameter1 = DBFunction.GetDBParameter();
                //    lobjParameter1.ParameterName = "@PID";
                //    lobjParameter1.DbType = DbType.String;
                //    lobjParameter1.Value = lstrPersonMpiID.ToLower();
                //    lcolParameters.Add(lobjParameter1);

                //    if (lbnIsContact)
                //    {
                //        IDbDataParameter lobjParameter3 = DBFunction.GetDBParameter();
                //        lobjParameter3.ParameterName = "@Attention";
                //        lobjParameter3.DbType = DbType.String;
                //        lobjParameter3.Value = lstrContactName;
                //        lcolParameters.Add(lobjParameter3);

                //    }

                //    IDbDataParameter lobjParameter4 = DBFunction.GetDBParameter();
                //    lobjParameter4.ParameterName = "@Address1";
                //    lobjParameter4.DbType = DbType.String;
                //    lobjParameter4.Value = lstrAddrline1;
                //    lcolParameters.Add(lobjParameter4);

                //    IDbDataParameter lobjParameter5 = DBFunction.GetDBParameter();
                //    lobjParameter5.ParameterName = "@Address2";
                //    lobjParameter5.DbType = DbType.String;
                //    lobjParameter5.Value = lstrAddrline2;
                //    lcolParameters.Add(lobjParameter5);

                //    IDbDataParameter lobjParameter6 = DBFunction.GetDBParameter();
                //    lobjParameter6.ParameterName = "@City";
                //    lobjParameter6.DbType = DbType.String;
                //    lobjParameter6.Value = lstrCity;
                //    lcolParameters.Add(lobjParameter6);

                //    IDbDataParameter lobjParameter7 = DBFunction.GetDBParameter();
                //    lobjParameter7.ParameterName = "@State";
                //    lobjParameter7.DbType = DbType.String;
                //    lobjParameter7.Value = lstrState;
                //    lcolParameters.Add(lobjParameter7);

                //    IDbDataParameter lobjParameter8 = DBFunction.GetDBParameter();
                //    lobjParameter8.ParameterName = "@PostalCode";
                //    lobjParameter8.DbType = DbType.String;
                //    lobjParameter8.Value = lstrPostalCode;
                //    lcolParameters.Add(lobjParameter8);

                //    IDbDataParameter lobjParameter9 = DBFunction.GetDBParameter();
                //    lobjParameter9.ParameterName = "@Country";
                //    lobjParameter9.DbType = DbType.String;
                //    lobjParameter9.Value = lstrCountry;
                //    lcolParameters.Add(lobjParameter9);

                //    IDbDataParameter lobjParameter10 = DBFunction.GetDBParameter();
                //    lobjParameter10.ParameterName = "@CountryCode";
                //    lobjParameter10.DbType = DbType.String;
                //    lobjParameter10.Value = lstrCountryValue;
                //    lcolParameters.Add(lobjParameter10);
                    IDbDataParameter lobjParameter10 = DBFunction.GetDBParameter();
                    //lobjParameter10.ParameterName = "@CountryCode";
                    //lobjParameter10.DbType = DbType.String;
                    //lobjParameter10.Value = lstrCountryValue;
                    //lcolParameters.Add(lobjParameter10);

                //    IDbDataParameter lobjParameter11 = DBFunction.GetDBParameter();
                //    lobjParameter11.ParameterName = "@ForeignAddr";
                //    lobjParameter11.DbType = DbType.String;
                //    lobjParameter11.Value = lintForeignAddrFlag;
                //    lcolParameters.Add(lobjParameter11);


                //    IDbDataParameter lobjParameter12 = DBFunction.GetDBParameter();
                //    lobjParameter12.ParameterName = "@ReturnedMail";
                //    lobjParameter12.DbType = DbType.DateTime;
                //    if (ldtAddressEndDate.IsNotNull() && lstrBadAddressFlag.IsNotNull() && ldtAddressEndDate != DateTime.MinValue && lstrBadAddressFlag == busConstant.FLAG_YES)
                //        lobjParameter12.Value = ldtAddressEndDate;
                //    else
                //        lobjParameter12.Value = DBNull.Value;

                //    lcolParameters.Add(lobjParameter12);

                //    if (!lbnIsContact)
                //    {
                //        IDbDataParameter lobjParameter13 = DBFunction.GetDBParameter();
                //        lobjParameter13.ParameterName = "@DoNotUpdate";
                //        lobjParameter13.DbType = DbType.String;
                //        lobjParameter13.Value = lintDoNotUpdate;
                //        lcolParameters.Add(lobjParameter13);
                //    }

                //    IDbDataParameter lobjParameter14 = DBFunction.GetDBParameter();
                //    lobjParameter14.ParameterName = "@AuditUser";
                //    lobjParameter14.DbType = DbType.String;
                //    lobjParameter14.Value = lstrAuditUser;
                //    lcolParameters.Add(lobjParameter14);

                //    IDbDataParameter lobjParameter15 = DBFunction.GetDBParameter();
                //    lobjParameter15.ParameterName = "@ReceivedFrom";
                //    lobjParameter15.DbType = DbType.String;
                //    lobjParameter15.Value = lstrAddressSource;
                //    lcolParameters.Add(lobjParameter15);

                //    IDbDataParameter lobjParameter16 = DBFunction.GetDBParameter();
                //    lobjParameter16.ParameterName = "@AddressStartDate";
                //    lobjParameter16.DbType = DbType.DateTime;
                //    lobjParameter16.Value = ldtAddressStartDate;
                //    lcolParameters.Add(lobjParameter16);


                //    try
                //    {
                //        lobjPassInfo1.BeginTransaction();
                //        DBFunction.DBExecuteProcedure("USP_PID_PersonAddress_UPD", lcolParameters, lobjPassInfo1.iconFramework, lobjPassInfo1.itrnFramework);
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
                //    }
                // }
            }
            #endregion
        }
        #endregion
    }
}
