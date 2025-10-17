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
using System.Reflection;
using MPIPHP.Common;
using Sagitec.CustomDataObjects;
using System.Linq;
using System.Text.RegularExpressions;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busPersonAddress:
    /// Inherited from busPersonAddressGen, the class is used to customize the business object busPersonAddressGen.
    /// </summary>
    [Serializable]
    public class busPersonAddress : busPersonAddressGen
    {
        #region Properties

        public busPerson ibusPerson { get; set; }
        public busPersonAddressHistory ibusPersonAddressHistory { get; set; }
        public string istrAddressType { get; set; }
        public bool iblnIsNewMode { get; set; }
        public utlCollection<cdoPersonAddressChklist> iclcPersonAddressChklistOld { get; set; }
        public busPersonAddress ibusMainParticipantAddress { get; set; }
        public busPersonAddressChklist ibusPersonAddressChklist { get; set; }
        public busRelationship ibusRelationship { get; set; }
        public string istrOtherProvince { get; set; }
        public bool iblnAddressSourceReadOnly { get; set; }

        public string old_state_val { get; set; }
        public string istrIsNoStateTax { get; set; }
        #endregion
        public string IsReturnFlag { get; set; }

        public bool iblnReturnedMail { get; set; }//PIR 1000


        //PIR 430
        public bool iblnIsUpdateEndDate = false;
        public cdoPersonAddress iobjcdoPersonAddress { get; set; }

        public override long iintPrimaryKey
        {
            get
            {
                long LintPrimaryKey=0;
                if (iobjPassInfo?.idictParams != null  && iobjPassInfo.istrSenderID == "btnSave")
                {
                    if (this.icdoPersonAddress.ihstOldValues.ContainsKey("address_id"))
                    {
                        LintPrimaryKey=Convert.ToInt32(this.icdoPersonAddress.ihstOldValues["address_id"]);
                    }
                    else
                    {
                        LintPrimaryKey=icdoPersonAddress.iintPrimaryKey;
                    }
                    return LintPrimaryKey;
                }
                else { return icdoPersonAddress.iintPrimaryKey; }
            }
        }

        public bool IsPersonMerged()
        {
            DataTable ldtbResult = busBase.Select<cdoPerson>(
              new string[1] { enmPerson.person_id.ToString() },
              new object[1] { this.ibusPerson.icdoPerson.person_id }, null, null);
            if (ldtbResult.Rows.Count > 0)
                return false;
            else
                return true;

        }
        #region Public Overriden Methods

        public Collection<cdoCodeValue> LoadAddressSource()
        {
            Collection<cdoCodeValue> iclbCodeValue = new Collection<cdoCodeValue>();
            DataTable ldtbList = null;

            if (iblnAddressSourceReadOnly == true)
                ldtbList = Select<cdoCodeValue>(new string[1] { "code_id" },
                new object[1] { "6031" }, null, "code_value_order, description");
            else
                ldtbList = Select<cdoCodeValue>(new string[2] { "code_id", "DATA1" },
                new object[2] { "6031", "1" }, null, "code_value_order, description");

            if (ldtbList.IsNotNull())
                iclbCodeValue = doBase.GetCollection<cdoCodeValue>(ldtbList);

            return iclbCodeValue;
        }


        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            if (iblnReturnedMail)//PIR 1000
                return;

            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();
            if (this.ibusRelationship.IsNotNull())
            {
                if (this.ibusMainParticipantAddress.IsNull() && this.ibusRelationship.icdoRelationship.person_id == 0 && this.ibusRelationship.icdoRelationship.addr_same_as_participant_flag == busConstant.Flag_Yes)
                {
                    utlError lutlError = new utlError();
                    lutlError = AddError(1181, "Participant does not have an Address");
                    this.iarrErrors.Add(lutlError);
                    return;
                }

                else if ((this.ibusMainParticipantAddress.IsNotNull() && this.ibusRelationship.icdoRelationship.person_id > 0 && this.ibusRelationship.icdoRelationship.addr_same_as_participant_flag == busConstant.Flag_Yes && this.ibusMainParticipantAddress.icdoPersonAddress.bad_address_flag.IsNotNull()
                         && this.ibusMainParticipantAddress.icdoPersonAddress.bad_address_flag == busConstant.FLAG_YES) ||
                        (this.ibusMainParticipantAddress.IsNotNull() && this.ibusRelationship.IsNotNull() && this.ibusMainParticipantAddress.icdoPersonAddress.person_id == 0 && this.ibusRelationship.icdoRelationship.addr_same_as_participant_flag == busConstant.Flag_Yes && this.ibusMainParticipantAddress.iclcPersonAddressChklist.IsNullOrEmpty())
                         )
                {

                    utlError lutlError = new utlError();
                    lutlError = AddError(1182, "Participant does not have an Valid Address");
                    this.iarrErrors.Add(lutlError);
                    return;
                }
            }

            if (this.icdoPersonAddress.bad_address_flag.IsNotNull() && this.icdoPersonAddress.bad_address_flag == busConstant.FLAG_YES && (this.icdoPersonAddress.end_date == DateTime.MinValue || this.icdoPersonAddress.end_date.IsNull()))
            {
                utlError lutlError = new utlError();
                lutlError = AddError(1183, "It is a BAD address Please Enter End Date");
                this.iarrErrors.Add(lutlError);
                return;
            }
            if (this.ibusRelationship.icdoRelationship.addr_same_as_participant_flag == null)
                this.ibusRelationship.icdoRelationship.addr_same_as_participant_flag = busConstant.FLAG_NO;
            if ((this.icdoPersonAddress.bad_address_flag.IsNotNull() && this.icdoPersonAddress.bad_address_flag == busConstant.FLAG_YES && (this.icdoPersonAddress.end_date == DateTime.MinValue || this.icdoPersonAddress.end_date.IsNull()) && this.icdoPersonAddress.start_date > DateTime.Now)
                || (this.icdoPersonAddress.bad_address_flag.IsNotNull() && this.icdoPersonAddress.bad_address_flag == busConstant.FLAG_YES && this.icdoPersonAddress.end_date != DateTime.MinValue && this.icdoPersonAddress.start_date > DateTime.Now
                    && this.icdoPersonAddress.end_date > this.icdoPersonAddress.start_date))
            {
                utlError lutlError = new utlError();
                lutlError = AddError(1184, "You cannot have a future dated Bad Address");
                this.iarrErrors.Add(lutlError);
                return;
            }

            else if ((this.ibusRelationship != null && this.ibusRelationship.icdoRelationship.addr_same_as_participant_flag == busConstant.FLAG_NO))
            {
                base.ValidateHardErrors(aenmPageMode);
            }

            if (this.icdoPersonAddress.addr_country_value == "0001" && !string.IsNullOrEmpty(Convert.ToString(this.icdoPersonAddress.addr_zip_code)) && this.icdoPersonAddress.addr_zip_code.Length != 5)
            {
                utlError lutlError = new utlError();
                lutlError = AddError(1171, "");
                this.iarrErrors.Add(lutlError);
            }

            if (CheckDuplicateAddressType() && iobjPassInfo.ienmPageMode == utlPageMode.Update)//PIR 525
            {
                utlError lutlError = new utlError();
                lutlError = AddError(1136, "");
                this.iarrErrors.Add(lutlError);
            }

            #region PIR 354 Enhancement
            if (iobjPassInfo.ienmPageMode == utlPageMode.Update && icdoPersonAddress.ihstOldValues.Count > 0)
            {
                //if (Convert.ToString(icdoPersonAddress.ihstOldValues["addr_state_value"]).IsNotNullOrEmpty() && Convert.ToString(icdoPersonAddress.ihstOldValues["addr_state_value"]) != icdoPersonAddress.addr_state_value)
                //PIR 1050
                if (icdoPersonAddress.ihstOldValues != null)
                {
                    DataTable ldtbPayeeAccountIDs = Select("cdoPersonAddress.GetPayeeAccounts", new object[1] { this.icdoPersonAddress.person_id });
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

                                lblnCheckToChangeStatus = lbusPayeeAccount.CheckACHDetails();

                                if (lblnCheckToChangeStatus != busConstant.BOOL_TRUE)
                                {
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
                                    utlError lutlError = new utlError();
                                    lutlError = AddError(6223, "");
                                    this.iarrErrors.Add(lutlError);
                                    return;
                                }
                            }
                        }
                    }
                }
                }
                //if (Convert.ToDateTime(icdoPersonAddress.ihstOldValues["end_date"]) == DateTime.MinValue && icdoPersonAddress.end_date != DateTime.MinValue)
                //{
                //    DataTable ldtbPayeeAccountIDs = Select("cdoPersonAddress.GetPayeeAccounts", new object[1] { this.icdoPersonAddress.person_id });
                //    if (ldtbPayeeAccountIDs.Rows.Count > 0)
                //    {
                //        foreach (DataRow ldrPayeeAccountId in ldtbPayeeAccountIDs.Rows)
                //        {
                //            busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                //            if (Convert.ToString(ldrPayeeAccountId[enmPayeeAccount.payee_account_id.ToString().ToUpper()]).IsNotNullOrEmpty() && lbusPayeeAccount.FindPayeeAccount(Convert.ToInt32(ldrPayeeAccountId[enmPayeeAccount.payee_account_id.ToString().ToUpper()])))
                //            {

                //                lbusPayeeAccount.LoadBenefitDetails();
                //                lbusPayeeAccount.LoadDRODetails();

                //                string istrScheduleType = string.Empty;
                //                if (lbusPayeeAccount.icdoPayeeAccount.iintPlanId != 1)
                //                    istrScheduleType = busConstant.PaymentScheduleTypeMonthly;
                //                else
                //                    istrScheduleType = busConstant.PaymentScheduleTypeWeekly;

                //                bool IsManager = false;
                //                busUserRoles lbusUserRoles = new busUserRoles { icdoRoles = new cdoRoles() };
                //                if (lbusUserRoles.FindUserRoles(iobjPassInfo.iintUserSerialID, busConstant.Role.MANAGER_ROLE))
                //                {
                //                    IsManager = true;
                //                }

                //                int lintCountSchedule = (int)DBFunction.DBExecuteScalar("cdoPerson.CheckPaymentSchedulebeforeApprovestatus", new object[1] { istrScheduleType }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework, iobjPassInfo.isrvMetaDataCache);
                //                if (lintCountSchedule > 0 && !IsManager)
                //                {
                //                    utlError lutlError = new utlError();
                //                    lutlError = AddError(6223, "");
                //                    this.iarrErrors.Add(lutlError);
                //                    return;
                //                }
                //            }
                //        }

                //    }
                //}
            }
            #endregion PIR 354 Enhancement

            //PIR 430
            if (iobjPassInfo.ienmPageMode == utlPageMode.Update && icdoPersonAddress.end_date == DateTime.MinValue && !this.icdoPersonAddress.ihstOldValues.IsNullOrEmpty()
                && !(this.ibusRelationship != null && this.ibusRelationship.icdoRelationship.addr_same_as_participant_flag == busConstant.Flag_Yes))
            {
                DateTime ldtOldStartDate = Convert.ToDateTime(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.start_date.ToString()]);
                if (Convert.ToDateTime(ldtOldStartDate.Date) < Convert.ToDateTime(DateTime.Now.Date) &&
                    Convert.ToDateTime(ldtOldStartDate.Date) != Convert.ToDateTime(icdoPersonAddress.start_date.Date))
                {
                    utlError lutlError = new utlError();
                    lutlError = AddError(0, "Start date cannot be changed.");
                    this.iarrErrors.Add(lutlError);
                }
            }

            //PIR 430 PIR 515
            //PIR 525
            if (icdoPersonAddress.end_date != DateTime.MinValue
                && icdoPersonAddress.end_date.Date != DateTime.Now.AddDays(-1).Date)
            {
                utlError lutlError = new utlError();
                lutlError = AddError(0, "End date cannot be any date before/after yesterday's date.");
                this.iarrErrors.Add(lutlError);
            }

            //PIR 525
            if (iobjPassInfo.ienmPageMode == utlPageMode.New
                || (iobjPassInfo.istrFormName == "wfmBeneficiaryMaintenance" && //PIR RID 63893 added OR condition for Beneficiary/Dependent Maintenance screen address add.
                    iobjPassInfo.ienmPageMode == utlPageMode.Update && this.icdoPersonAddress.ihstOldValues != null &&
                   (Convert.ToString(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.addr_line_1.ToString()]).IsNullOrEmpty() && icdoPersonAddress.addr_line_1.IsNotNullOrEmpty() ))
                )
            {
                if (icdoPersonAddress.start_date.Date != DateTime.Today.Date)
                {
                    utlError lutlError = new utlError();
                    lutlError = AddError(0, "Start Date should be Today's Date");
                    this.iarrErrors.Add(lutlError);
                }
            }

            //PIR 525
            if ((iobjPassInfo.ienmPageMode == utlPageMode.Update && this.icdoPersonAddress.ihstOldValues.Count > 0
                && Convert.ToString(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.secured_flag.ToString()]) == busConstant.FLAG_YES &&
                icdoPersonAddress.secured_flag == Convert.ToString(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.secured_flag.ToString()]))
                ||
                (iobjPassInfo.ienmPageMode == utlPageMode.New && ibusPerson != null && ibusPerson.iclbPersonAddress != null &&
                ibusPerson.iclbPersonAddress.Where(item => item.icdoPersonAddress.end_date == DateTime.MinValue).Count() > 0
                && ibusPerson.iclbPersonAddress.Where(item => item.icdoPersonAddress.end_date == DateTime.MinValue).FirstOrDefault().icdoPersonAddress.secured_flag == busConstant.FLAG_YES))
            {
                utlError lutlError = new utlError();
                lutlError = AddError(0, "Cannot add or update the address as current address is secured.");
                this.iarrErrors.Add(lutlError);
            }

            //PIR 1050 //RID 51428
            if (icdoPersonAddress.addr_line_1.IsNotNullOrEmpty() && icdoPersonAddress.addr_line_2.IsNotNullOrEmpty() && icdoPersonAddress.addr_city.IsNotNullOrEmpty())
            {
                if ((iobjPassInfo.ienmPageMode == utlPageMode.New && (Regex.IsMatch(icdoPersonAddress.addr_line_1 == null ? string.Empty : icdoPersonAddress.addr_line_1, "[^a-zA-Z0-9/  ]")
                      || Regex.IsMatch(icdoPersonAddress.addr_line_2 == null ? string.Empty : icdoPersonAddress.addr_line_2, "[^a-zA-Z0-9/  ]")
                              || Regex.IsMatch(icdoPersonAddress.addr_city == null ? string.Empty : icdoPersonAddress.addr_city, "[^a-zA-Z0-9/  ]")))
                  ||
                  (iobjPassInfo.ienmPageMode == utlPageMode.Update && this.icdoPersonAddress.ihstOldValues != null &&
                   ((Convert.ToString(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.addr_line_1.ToString()]) != icdoPersonAddress.addr_line_1 &&
                                      Regex.IsMatch(icdoPersonAddress.addr_line_1 == null ? string.Empty : icdoPersonAddress.addr_line_1, "[^a-zA-Z0-9/  ]"))
                   || (Convert.ToString(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.addr_line_2.ToString()]) != icdoPersonAddress.addr_line_2 &&
                                      Regex.IsMatch(icdoPersonAddress.addr_line_2 == null ? string.Empty : icdoPersonAddress.addr_line_2, "[^a-zA-Z0-9/  ]"))
                   || (Convert.ToString(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.addr_city.ToString()]) != icdoPersonAddress.addr_city &&
                                      Regex.IsMatch(icdoPersonAddress.addr_city == null ? string.Empty : icdoPersonAddress.addr_city, "[^a-zA-Z0-9/  ]")))))
                {
                    utlError lutlError = new utlError();
                    lutlError = AddError(0, "Please remove special characters from address line 1 / address line 2");
                    this.iarrErrors.Add(lutlError);
                }
            }
             //RID#80581
            if (!string.IsNullOrEmpty(this.icdoPersonAddress.addr_country_value))
            {
                int aintCountryValue = Convert.ToInt32(this.icdoPersonAddress.addr_country_value);
                if (aintCountryValue == busConstant.AUSTRALIA || aintCountryValue == busConstant.CANADA || aintCountryValue == busConstant.MEXICO
               || aintCountryValue == busConstant.NewZealand || aintCountryValue == busConstant.OTHER_PROVINCE)
                {
                    if (string.IsNullOrEmpty(this.icdoPersonAddress.addr_state_value))
                    {
                        utlError lutlError = new utlError();
                        lutlError = AddError(0, "Please select foreign province.");
                        this.iarrErrors.Add(lutlError);
                    }
                }

            }
        }


        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            base.BeforeValidate(aenmPageMode);
            if ((this.ibusRelationship != null && this.ibusRelationship.icdoRelationship.addr_same_as_participant_flag == busConstant.Flag_Yes))//PIR 525
            //&& iarrChangeLog.Count > 0 && ((iarrChangeLog.Contains(icdoPersonAddress) && this.icdoPersonAddress.ienuObjectState == ObjectState.Insert) ||
            //(iarrChangeLog.Contains(this.ibusRelationship.icdoRelationship) && this.ibusRelationship.icdoRelationship.ienuObjectState == ObjectState.Update)))
            {
                //iarrChangeLog.Remove(icdoPersonAddress);
                //this.icdoPersonAddress.ienuObjectState = ObjectState.None;
                LoadMainPaddToBenificaryAddress(this.ibusMainParticipantAddress, this);
                this.icdoPersonAddress.person_id = ibusPerson.icdoPerson.person_id;
                this.iblnAddressSourceReadOnly = true;
                LoadAddressSource();
                if (iarrChangeLog.Contains(icdoPersonAddress) && this.icdoPersonAddress.ienuObjectState == ObjectState.Insert)
                {
                    this.icdoPersonAddress.update_seq = 0;
                }
                if (this.iclcPersonAddressChklist.Count == 0)
                {
                    foreach (cdoPersonAddressChklist tbusPersonAddressChklist in this.ibusMainParticipantAddress.iclcPersonAddressChklist)
                    {
                        this.LoadPersonAddressChklists();

                        busPersonAddressChklist lbusPersonAddressChklist = new busPersonAddressChklist { icdoPersonAddressChklist = new cdoPersonAddressChklist() };
                        lbusPersonAddressChklist.icdoPersonAddressChklist.address_id = this.icdoPersonAddress.address_id;
                        lbusPersonAddressChklist.icdoPersonAddressChklist.address_type_value = tbusPersonAddressChklist.address_type_value;
                        lbusPersonAddressChklist.icdoPersonAddressChklist.address_type_id = busConstant.ADDRESS_TYPE_ID;
                        lbusPersonAddressChklist.icdoPersonAddressChklist.ienuObjectState = ObjectState.Insert;
                        this.iarrChangeLog.Add(lbusPersonAddressChklist.icdoPersonAddressChklist);
                        this.iclcPersonAddressChklist.Add(lbusPersonAddressChklist.icdoPersonAddressChklist);
                    }

                }
            }
        }


        public override void BeforePersistChanges()
        {
            //PIR 430
            iblnIsUpdateEndDate = busConstant.BOOL_FALSE;
            if (iobjPassInfo.ienmPageMode == utlPageMode.Update && icdoPersonAddress.end_date == DateTime.MinValue)
            {
                if (Convert.ToDateTime(icdoPersonAddress.start_date.Date) < Convert.ToDateTime(DateTime.Now.Date))
                {
                    LoadCurrentAddressValues();
                    UpdateOldAddressWithExisting();
                    icdoPersonAddress.end_date = DateTime.Now.AddDays(-1);
                    iblnIsUpdateEndDate = busConstant.BOOL_TRUE;
                }
            }

            if (!IsNewMode())
            {
                //Loading Address History Object.
                LoadOldAddressInAddressHistory();
            }
            if (string.IsNullOrEmpty(this.icdoPersonAddress.secured_flag))
            {
                this.icdoPersonAddress.secured_flag = busConstant.FLAG_NO;
            }
            if (this.icdoPersonAddress.end_date != DateTime.MinValue)
            {
                this.icdoPersonAddress.bad_address_flag = busConstant.FLAG_YES;
            }
            if (this.icdoPersonAddress.addr_country_value != "0001")
                this.icdoPersonAddress.addr_state_id = 152;
            else
                this.icdoPersonAddress.addr_state_id = 150;
            //Changing Same As Address Flag From Yes To No
            this.icdoPersonAddress.person_id = ibusPerson.icdoPerson.person_id;

            if (this.ibusRelationship != null && this.ibusRelationship.icdoRelationship.addr_same_as_participant_flag == busConstant.Flag_Yes && this.icdoPersonAddress.address_id != 0)
            {
                //this.icdoPersonAddress.end_date = DateTime.Today;

            }
            //if ((this.ibusRelationship != null && this.ibusRelationship.icdoRelationship.addr_same_as_participant_flag == busConstant.Flag_Yes)
            //    && iarrChangeLog.Count > 0 && iarrChangeLog.Contains(icdoPersonAddress) && this.icdoPersonAddress.ienuObjectState == ObjectState.Insert)
            //{
            //    //iarrChangeLog.Remove(icdoPersonAddress);
            //    //this.icdoPersonAddress.ienuObjectState = ObjectState.None;
            //    LoadMainPaddToBenificaryAddress(this.ibusMainParticipantAddress, this);
            //    this.icdoPersonAddress.person_id = ibusPerson.icdoPerson.person_id;
            //    this.iblnAddressSourceReadOnly = true;
            //    LoadAddressSource();
            //    this.icdoPersonAddress.update_seq = 0;
            //}
            if (!string.IsNullOrEmpty(istrOtherProvince))
            {
                this.icdoPersonAddress.foreign_province = istrOtherProvince;
            }

            if (iobjPassInfo.ienmPageMode == utlPageMode.Update && icdoPersonAddress.ihstOldValues.Count > 0)
            {
                if (Convert.ToString(icdoPersonAddress.ihstOldValues["addr_state_value"]).IsNotNullOrEmpty() && Convert.ToString(icdoPersonAddress.ihstOldValues["addr_state_value"]) != icdoPersonAddress.addr_state_value)
                {
                    DataTable ldtbPayeeAccountIDs = Select("cdoPersonAddress.GetPayeeAccounts", new object[1] { this.icdoPersonAddress.person_id });
                    if (ldtbPayeeAccountIDs.Rows.Count > 0)
                    {
                        foreach (DataRow ldrPayeeAccountId in ldtbPayeeAccountIDs.Rows)
                        {
                            cdoPayeeAccountStatus lcdoPayeeAccountStatus = new cdoPayeeAccountStatus();
                            lcdoPayeeAccountStatus.payee_account_id = Convert.ToInt32(ldrPayeeAccountId[enmPayeeAccountStatus.payee_account_id.ToString().ToUpper()]);
                            lcdoPayeeAccountStatus.status_id = busConstant.Payee_Account_Status_ID;
                            lcdoPayeeAccountStatus.status_value = busConstant.PAYEE_ACCOUNT_STATUS_REVIEW;
                            lcdoPayeeAccountStatus.status_effective_date = DateTime.Now;
                            lcdoPayeeAccountStatus.Insert();
                            //PROD PIR 269
                            if (Convert.ToInt32(ldrPayeeAccountId[enmPayeeAccountStatus.payee_account_id.ToString().ToUpper()]) > 0)
                            {
                                busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
                                if (lobjPayeeAccount.FindPayeeAccount(Convert.ToInt32(ldrPayeeAccountId[enmPayeeAccountStatus.payee_account_id.ToString().ToUpper()])))
                                {
                                    lobjPayeeAccount.CreateReactivatioRetroPaymentItem();
                                    lobjPayeeAccount.LoadPayeeAccountTaxWithholdings();
                                    if (lobjPayeeAccount.iclbPayeeAccountTaxWithholding != null && lobjPayeeAccount.iclbPayeeAccountTaxWithholding.Count > 0)
                                    {
                                        foreach (busPayeeAccountTaxWithholding lbusPayeeAccountTaxWithholding in lobjPayeeAccount.iclbPayeeAccountTaxWithholding)
                                        {
                                            lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.end_date = DateTime.Now;
                                            lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.Update();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (Convert.ToDateTime(icdoPersonAddress.ihstOldValues["end_date"]) == DateTime.MinValue && icdoPersonAddress.end_date != DateTime.MinValue)
                {
                    DataTable ldtbPayeeAccountIDs = Select("cdoPersonAddress.GetPayeeAccounts", new object[1] { this.icdoPersonAddress.person_id });
                    if (ldtbPayeeAccountIDs.Rows.Count > 0)
                    {
                        //PIR-79 (If payee's type of payment is ACH, DO NOT change the payee account status.)
                        bool lblnFlg = busConstant.BOOL_FALSE, lblnCheckToChangeStatus;
                        foreach (DataRow ldrPayeeAccountId in ldtbPayeeAccountIDs.Rows)
                        {
                            busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                            lbusPayeeAccount.FindPayeeAccount(Convert.ToInt32(ldrPayeeAccountId[enmPayeeAccountStatus.payee_account_id.ToString().ToUpper()]));
                            lblnCheckToChangeStatus = lbusPayeeAccount.CheckACHDetails();
                            if (lblnCheckToChangeStatus != busConstant.BOOL_TRUE)
                            {
                                cdoPayeeAccountStatus lcdoPayeeAccountStatus = new cdoPayeeAccountStatus();
                              
                                //Ticket 69506
                                if(ldrPayeeAccountId[1].ToString() == busConstant.PAYEE_ACCOUNT_STATUS_RECEIVING)
                                {
                                    lcdoPayeeAccountStatus.payee_account_id = Convert.ToInt32(ldrPayeeAccountId[enmPayeeAccountStatus.payee_account_id.ToString().ToUpper()]);
                                    lcdoPayeeAccountStatus.status_id = busConstant.Payee_Account_Status_ID;
                                    lcdoPayeeAccountStatus.status_value = busConstant.PAYEE_ACCOUNT_STATUS_SUSPENDED;
                                    
                                }else
                                {
                                    lcdoPayeeAccountStatus.payee_account_id = Convert.ToInt32(ldrPayeeAccountId[enmPayeeAccountStatus.payee_account_id.ToString().ToUpper()]);
                                    lcdoPayeeAccountStatus.status_id = busConstant.Payee_Account_Status_ID;
                                    lcdoPayeeAccountStatus.status_value = busConstant.PAYEE_ACCOUNT_STATUS_REVIEW;
                              
                                }

                                lcdoPayeeAccountStatus.status_effective_date = DateTime.Now;
                                lcdoPayeeAccountStatus.Insert();
                                lblnFlg = busConstant.BOOL_TRUE;


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
                            lcdoNotes.person_id = this.icdoPersonAddress.person_id;
                            lcdoNotes.notes = "Payee account status changed to review as Mailing address is end dated";
                            lcdoNotes.form_value = busConstant.PERSON_ADDRESS_MAINTAINANCE_FORM;
                            lcdoNotes.Insert();
                        }
                    }
                }
            }

            #region PIR-498
            if (iobjPassInfo.ienmPageMode == utlPageMode.New)
            {
                bool lblnCheckToChangeStatus;
                if (this.ibusPerson.iclbPersonAddress.Count > 0)
                {
                    if (this.ibusPerson.iclbPersonAddress[this.ibusPerson.iclbPersonAddress.Count - 1].icdoPersonAddress.addr_state_value == "CA")
                    {
                        if ((this.iclcPersonAddressChklist[0].address_type_value == busConstant.MAILING_ADDRESS) && (this.icdoPersonAddress.addr_state_value != busConstant.CALIFORNIA))
                        {
                            DataTable ldtbPayeeAccountIDs = Select("cdoPersonAddress.GetPayeeAccountsForAddingNewAddress", new object[1] { this.icdoPersonAddress.person_id });
                            if (ldtbPayeeAccountIDs.Rows.Count > 0)
                            {
                                foreach (DataRow ldrPayeeAccountId in ldtbPayeeAccountIDs.Rows)
                                {
                                    if (Convert.ToInt32(ldrPayeeAccountId[enmPayeeAccountStatus.payee_account_id.ToString().ToUpper()]) > 0)
                                    {
                                        busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
                                        if (lobjPayeeAccount.FindPayeeAccount(Convert.ToInt32(ldrPayeeAccountId[enmPayeeAccountStatus.payee_account_id.ToString().ToUpper()])))
                                        {
                                            //lobjPayeeAccount.CreateReactivatioRetroPaymentItem();
                                            lobjPayeeAccount.LoadPayeeAccountTaxWithholdings();
                                            if (lobjPayeeAccount.iclbPayeeAccountTaxWithholding != null && lobjPayeeAccount.iclbPayeeAccountTaxWithholding.Count > 0)
                                            {
                                                foreach (busPayeeAccountTaxWithholding lbusPayeeAccountTaxWithholding in lobjPayeeAccount.iclbPayeeAccountTaxWithholding)
                                                {
                                                    if ((lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_option_value != busConstant.NO_STATE_TAX)
                                                        && ((lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.end_date == DateTime.MinValue) || (lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.end_date > DateTime.Now))
                                                        && (lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_identifier_value != busConstant.FEDRAL_STATE_TAX))
                                                    {
                                                        lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.end_date = this.icdoPersonAddress.start_date.AddDays(-1);
                                                        lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.Update();

                                                        busPayeeAccountTaxWithholding tbusPayeeAccountTaxWithholding = new busPayeeAccountTaxWithholding();
                                                        lobjPayeeAccount.LoadNextBenefitPaymentDate();
                                                        lbusPayeeAccountTaxWithholding.InsertValuesInTaxWithHolding(Convert.ToInt32(ldrPayeeAccountId[enmPayeeAccountStatus.payee_account_id.ToString().ToUpper()]),
                                                            busConstant.CA_STATE_TAX, lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value,
                                                            this.icdoPersonAddress.start_date, DateTime.MinValue, busConstant.NO_STATE_TAX, 0, this.ibusPerson.icdoPerson.marital_status_value, 0, 0);

                                                        //PIR-79 (If payee's type of payment is ACH, DO NOT change the payee account status.)                                                        
                                                        if (Convert.ToString(ldrPayeeAccountId[enmPayeeAccountStatus.status_value.ToString()]) != busConstant.PAYEE_ACCOUNT_STATUS_REVIEW)
                                                        {
                                                            lblnCheckToChangeStatus = lobjPayeeAccount.CheckACHDetails();
                                                            if (lblnCheckToChangeStatus != busConstant.BOOL_TRUE)
                                                            {
                                                                cdoPayeeAccountStatus lcdoPayeeAccountStatus = new cdoPayeeAccountStatus();
                                                                lcdoPayeeAccountStatus.payee_account_id = Convert.ToInt32(ldrPayeeAccountId[enmPayeeAccountStatus.payee_account_id.ToString().ToUpper()]);
                                                                lcdoPayeeAccountStatus.status_id = busConstant.Payee_Account_Status_ID;
                                                                lcdoPayeeAccountStatus.status_value = busConstant.PAYEE_ACCOUNT_STATUS_REVIEW;
                                                                lcdoPayeeAccountStatus.status_effective_date = DateTime.Now;
                                                                lcdoPayeeAccountStatus.Insert();
                                                            }
                                                        }
                                                    }
                                                    lobjPayeeAccount.LoadPayeeAccountTaxWithholdings();
                                                    lobjPayeeAccount.ProcessTaxWithHoldingDetails();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            //PIR RID 61485
            if (iobjPassInfo.ienmPageMode == utlPageMode.Update)
            {
               
                if (this.iclcPersonAddressChklist != null && this.iclcPersonAddressChklist.Count > 0)
                {
                    foreach (cdoPersonAddressChklist lcdoPersonAddressChklist in this.iclcPersonAddressChklist.ToList())
                    {
                        if (lcdoPersonAddressChklist.address_chklist_id == 0)
                        {
                            this.iclcPersonAddressChklist.Remove(lcdoPersonAddressChklist);

                            if (iarrChangeLog != null && this.iarrChangeLog.Count > 0)
                            {
                                if (iarrChangeLog.Contains(lcdoPersonAddressChklist))
                                {
                                    iarrChangeLog.Remove(lcdoPersonAddressChklist);
                                }

                            }
                        }
                    }
                }
            }
            //end
            if (iobjPassInfo.ienmPageMode == utlPageMode.Update && (iblnIsUpdateEndDate || (this.ibusRelationship != null && this.ibusRelationship.icdoRelationship.addr_same_as_participant_flag == busConstant.Flag_Yes && this.icdoPersonAddress.address_id != 0)))
            {
                icdoPersonAddress.Update();
            }
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            if (iarrChangeLog.Count > 0)
            {
                if (!iblnIsNewMode)
                {
                    SaveOldAddressInAddressHistory();
                }

                //PIR 525
                ////For Benificiary
                //addressChangeForBenificiary();

                //PIR 430
                if (iobjPassInfo.ienmPageMode == utlPageMode.Update && iblnIsUpdateEndDate)
                {
                    #region OPUS data push to Health Eligibility for any Address
                    if (this.icdoPersonAddress.start_date <= DateTime.Now)
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


                        int lintPersonID = this.icdoPersonAddress.person_id;
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
                            lstrAddrline1 = this.icdoPersonAddress.addr_line_1;
                            lstrAddrline2 = this.icdoPersonAddress.addr_line_2;
                            lstrCity = this.icdoPersonAddress.addr_city;

                            if (Convert.ToInt32(this.icdoPersonAddress.addr_country_value) == busConstant.USA || Convert.ToInt32(this.icdoPersonAddress.addr_country_value) == busConstant.AUSTRALIA
                                || Convert.ToInt32(this.icdoPersonAddress.addr_country_value) == busConstant.CANADA || Convert.ToInt32(this.icdoPersonAddress.addr_country_value) == busConstant.MEXICO
                                || Convert.ToInt32(this.icdoPersonAddress.addr_country_value) == busConstant.NewZealand)
                            {
                                lstrState = this.icdoPersonAddress.addr_state_value;
                            }
                            else
                            {
                                lstrState = this.icdoPersonAddress.foreign_province;
                            }

                            if (Convert.ToInt32(this.icdoPersonAddress.addr_country_value) == busConstant.USA)
                            {

                                lstrPostalCode = this.icdoPersonAddress.addr_zip_code + this.icdoPersonAddress.addr_zip_4_code;
                            }
                            else
                            {
                                lstrPostalCode = this.icdoPersonAddress.foreign_postal_code;
                            }

                            lstrCountry = this.icdoPersonAddress.addr_country_description;
                            if (!string.IsNullOrEmpty(this.icdoPersonAddress.addr_country_value))
                            {
                                lstrCountryValue = HelperUtil.GetData1ByCodeValue(this.icdoPersonAddress.addr_country_id, this.icdoPersonAddress.addr_country_value);
                            }
                            if (Convert.ToInt32(this.icdoPersonAddress.addr_country_value) == busConstant.USA)
                            {
                                lintForeignAddrFlag = 0;
                            }
                            else
                            {
                                lintForeignAddrFlag = 1;
                            }

                            if (this.icdoPersonAddress.secured_flag == "Y")
                            {
                                lintDoNotUpdate = 1;
                            }
                            else
                            {
                                lintDoNotUpdate = 0;
                            }

                            lstrAuditUser = iobjPassInfo.istrUserID;

                            ldtAddressStartDate = icdoPersonAddress.start_date;

                            if (this.icdoPersonAddress.end_date.IsNotNull() && this.icdoPersonAddress.end_date != DateTime.MinValue)
                                ldtAddressEndDate = this.icdoPersonAddress.end_date;
                            else
                                ldtAddressEndDate = DateTime.MinValue;

                            lstrAddressSource = icdoPersonAddress.addr_source_description;
                            lstrBadAddressFlag = icdoPersonAddress.bad_address_flag;
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
                        //}
                    }   //              //Commented - Rohan Code For data Push to HEDB 
                    #endregion OPUS data push to Health Eligibility for any Address
                }


                //PIR 430
                if (iobjPassInfo.ienmPageMode == utlPageMode.Update && iblnIsUpdateEndDate)
                {
                    //Insert new address when update existing
                    UpdateNewAddressWithExisting();
                    if (this.iclcPersonAddressChklist != null && this.iclcPersonAddressChklist.Count > 0)
                    {
                        //PIR RID 61485 setting Address type mailing only. commented physical and mailing check.
                        icdoPersonAddress.istrMailingAddressType = busConstant.YES;
                      /*  foreach (cdoPersonAddressChklist lcdoPersonAddressChklist in this.iclcPersonAddressChklist)
                        {
                            if (lcdoPersonAddressChklist.address_type_value == busConstant.PHYSICAL_ADDRESS)
                            {
                                icdoPersonAddress.istrPhysicalAddressType = busConstant.YES;
                            }
                            else if (lcdoPersonAddressChklist.address_type_value == busConstant.MAILING_ADDRESS)
                            {
                                icdoPersonAddress.istrMailingAddressType = busConstant.YES;
                            }
                        }  */
                    }
                    icdoPersonAddress.Insert();
                    // Insert record in SGT_PERSON_ADDRESS_CHKLIST table with new address id

                    //PIR RID 61485 setting Address type mailing only. commented looping address type check list.
                    cdoPersonAddressChklist lcdoPersonAddressCheckList = new cdoPersonAddressChklist();
                    lcdoPersonAddressCheckList.address_id = this.icdoPersonAddress.address_id;
                    lcdoPersonAddressCheckList.address_type_value = busConstant.MAILING_ADDRESS;
                    if (!this.iobjPassInfo.istrUserID.IsNullOrEmpty())
                    {
                        lcdoPersonAddressCheckList.created_by = this.iobjPassInfo.istrUserID;
                        lcdoPersonAddressCheckList.modified_by = this.iobjPassInfo.istrUserID;
                    }
                    lcdoPersonAddressCheckList.created_date = DateTime.Now;
                    lcdoPersonAddressCheckList.modified_date = DateTime.Now;
                    lcdoPersonAddressCheckList.Insert();

                    /*
                    if (this.iclcPersonAddressChklist != null && this.iclcPersonAddressChklist.Count > 0)
                    {
                        foreach (cdoPersonAddressChklist lcdoPersonAddressChklist in this.iclcPersonAddressChklist)
                        {
                            if (lcdoPersonAddressChklist is cdoPersonAddressChklist)
                            {
                                (lcdoPersonAddressChklist as cdoPersonAddressChklist).address_id = this.icdoPersonAddress.address_id;
                                if (!this.iobjPassInfo.istrUserID.IsNullOrEmpty())
                                {
                                    (lcdoPersonAddressChklist as cdoPersonAddressChklist).created_by = this.iobjPassInfo.istrUserID;
                                    (lcdoPersonAddressChklist as cdoPersonAddressChklist).modified_by = this.iobjPassInfo.istrUserID;
                                }
                                (lcdoPersonAddressChklist as cdoPersonAddressChklist).created_date = DateTime.Now;
                                (lcdoPersonAddressChklist as cdoPersonAddressChklist).modified_date = DateTime.Now;

                                (lcdoPersonAddressChklist as cdoPersonAddressChklist).Insert();
                            }
                        }


                    }*/
                }

                // OPUS data push to Health Eligibility for any new Address     //Commented - Rohan Code For data Push to HEDB  (Do not delete this)

                //PIR 430
                if (!iblnIsUpdateEndDate)
                    this.UpdateEndDate();

                #region OPUS data push to Health Eligibility for any Address
                if (this.icdoPersonAddress.start_date <= DateTime.Now)
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


                    int lintPersonID = this.icdoPersonAddress.person_id;
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
                        lstrAddrline1 = this.icdoPersonAddress.addr_line_1;
                        lstrAddrline2 = this.icdoPersonAddress.addr_line_2;
                        lstrCity = this.icdoPersonAddress.addr_city;

                        if (Convert.ToInt32(this.icdoPersonAddress.addr_country_value) == busConstant.USA || Convert.ToInt32(this.icdoPersonAddress.addr_country_value) == busConstant.AUSTRALIA
                            || Convert.ToInt32(this.icdoPersonAddress.addr_country_value) == busConstant.CANADA || Convert.ToInt32(this.icdoPersonAddress.addr_country_value) == busConstant.MEXICO
                            || Convert.ToInt32(this.icdoPersonAddress.addr_country_value) == busConstant.NewZealand)
                        {
                            lstrState = this.icdoPersonAddress.addr_state_value;
                        }
                        else
                        {
                            lstrState = this.icdoPersonAddress.foreign_province;
                        }

                        if (Convert.ToInt32(this.icdoPersonAddress.addr_country_value) == busConstant.USA)
                        {

                            lstrPostalCode = this.icdoPersonAddress.addr_zip_code + this.icdoPersonAddress.addr_zip_4_code;
                        }
                        else
                        {
                            lstrPostalCode = this.icdoPersonAddress.foreign_postal_code;
                        }

                        lstrCountry = this.icdoPersonAddress.addr_country_description;
                        if (!string.IsNullOrEmpty(this.icdoPersonAddress.addr_country_value))
                        {
                            lstrCountryValue = HelperUtil.GetData1ByCodeValue(this.icdoPersonAddress.addr_country_id, this.icdoPersonAddress.addr_country_value);
                        }
                        if (Convert.ToInt32(this.icdoPersonAddress.addr_country_value) == busConstant.USA)
                        {
                            lintForeignAddrFlag = 0;
                        }
                        else
                        {
                            lintForeignAddrFlag = 1;
                        }

                        if (this.icdoPersonAddress.secured_flag == "Y")
                        {
                            lintDoNotUpdate = 1;
                        }
                        else
                        {
                            lintDoNotUpdate = 0;
                        }

                        lstrAuditUser = iobjPassInfo.istrUserID;

                        ldtAddressStartDate = icdoPersonAddress.start_date;

                        if (this.icdoPersonAddress.end_date.IsNotNull() && this.icdoPersonAddress.end_date != DateTime.MinValue)
                            ldtAddressEndDate = this.icdoPersonAddress.end_date;
                        else
                            ldtAddressEndDate = DateTime.MinValue;

                        lstrAddressSource = icdoPersonAddress.addr_source_description;
                        lstrBadAddressFlag = icdoPersonAddress.bad_address_flag;
                    }


                    ////On EADB

                    if ((iobjPassInfo.iconFramework).Database == "MPI" || (iobjPassInfo.iconFramework).Database == "MPIPHP")
                    {
                        return;
                    }


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
                    //}
                }   //              //Commented - Rohan Code For data Push to HEDB 
                #endregion OPUS data push to Health Eligibility for any Address
            }

        
            //PIR 525
            if (this.ibusRelationship.icdoRelationship.person_id == 0)
            {
                DataTable ldtbRelation = Select<cdoRelationship>(
                  new string[1] { enmRelationship.person_id.ToString() },
                  new object[1] { this.ibusPerson.icdoPerson.person_id }, null, null);

                if (ldtbRelation.Rows.Count > 0)
                {
                    Collection<busRelationship> lclbRelationship = new Collection<busRelationship>();
                    lclbRelationship = GetCollection<busRelationship>(ldtbRelation, "icdoRelationship");

                    foreach (busRelationship lbusRelationship in lclbRelationship)
                    {
                        if (lbusRelationship.icdoRelationship.addr_same_as_participant_flag == busConstant.FLAG_YES)
                        {
                            lbusRelationship.icdoRelationship.addr_same_as_participant_flag = busConstant.FLAG_NO;
                            lbusRelationship.icdoRelationship.Update();
                        }
                    }
                }
            }
            else
            {
                if (this.ibusRelationship.icdoRelationship.addr_same_as_participant_flag == busConstant.FLAG_YES)
                {
                    this.ibusRelationship.icdoRelationship.addr_same_as_participant_flag = busConstant.FLAG_NO;
                    this.ibusRelationship.icdoRelationship.Update();
                }
            }
        }

        public override void SetParentKey(Sagitec.DataObjects.doBase aobjBase)
        {
            if (aobjBase is cdoPersonAddressChklist)
            {
                (aobjBase as cdoPersonAddressChklist).address_id = this.icdoPersonAddress.address_id;
            }
        }


        public override void LoadPersonAddressHistorys()
        {
            base.LoadPersonAddressHistorys();
            foreach (busPersonAddressHistory lbusPersonAddressHistory in iclbPersonAddressHistory)
            {
                lbusPersonAddressHistory.LoadPersonAddressChklists();
                lbusPersonAddressHistory.icdoPersonAddressHistory.istrPhysicalAddressType = busConstant.NO;
                lbusPersonAddressHistory.icdoPersonAddressHistory.istrMailingAddressType = busConstant.NO;
                if (lbusPersonAddressHistory.iclcPersonAddressChklistHistory.Count > 0)
                {
                    foreach (cdoPersonAddressChklistHistory lcdoPersonAddressChklistHistory in lbusPersonAddressHistory.iclcPersonAddressChklistHistory)
                    {
                        if (lcdoPersonAddressChklistHistory.address_type_value == busConstant.PHYSICAL_ADDRESS)
                        {
                            lbusPersonAddressHistory.icdoPersonAddressHistory.istrPhysicalAddressType = busConstant.YES;
                        }
                        else if (lcdoPersonAddressChklistHistory.address_type_value == busConstant.MAILING_ADDRESS)
                        {
                            lbusPersonAddressHistory.icdoPersonAddressHistory.istrMailingAddressType = busConstant.YES;
                        }
                    }
                }
            }
        }

        public override busBase GetCorPerson()
        {
            return this.ibusPerson;
        }

        public override void LoadCorresProperties(string astrTemplateName)
        {
            base.LoadCorresProperties(astrTemplateName);
            DateTime ldtCurrentDate = System.DateTime.Now;
            this.ibusPerson.istrCurrentDate = busGlobalFunctions.ConvertDateIntoDifFormat(ldtCurrentDate);
        }

        #endregion

        #region Public Methods
        //PIR 430
        private void UpdateOldAddressWithExisting()
        {
            if (!this.icdoPersonAddress.ihstOldValues.IsNullOrEmpty())
            {
                this.icdoPersonAddress.addr_line_1 = Convert.ToString(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.addr_line_1.ToString()]);
                this.icdoPersonAddress.addr_line_2 = Convert.ToString(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.addr_line_2.ToString()]);
                this.icdoPersonAddress.addr_city = Convert.ToString(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.addr_city.ToString()]);
                this.icdoPersonAddress.addr_state_id = Convert.ToInt32(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.addr_state_id.ToString()]);
                this.icdoPersonAddress.addr_state_value = Convert.ToString(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.addr_state_value.ToString()]);
                this.icdoPersonAddress.addr_country_value = Convert.ToString(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.addr_country_value.ToString()]);
                this.icdoPersonAddress.addr_zip_code = Convert.ToString(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.addr_zip_code.ToString()]);
                this.icdoPersonAddress.addr_zip_4_code = Convert.ToString(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.addr_zip_4_code.ToString()]);
                this.icdoPersonAddress.foreign_province = Convert.ToString(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.foreign_province.ToString()]);
                this.icdoPersonAddress.foreign_postal_code = Convert.ToString(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.foreign_postal_code.ToString()]);
                this.icdoPersonAddress.start_date = Convert.ToDateTime(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.start_date.ToString()]);
                //this.icdoPersonAddress.end_date = Convert.ToDateTime(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.end_date.ToString()]);
                this.icdoPersonAddress.secured_flag = Convert.ToString(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.secured_flag.ToString()]);
                //this.icdoPersonAddress.bad_address_flag = Convert.ToString(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.bad_address_flag.ToString()]);
                if (!string.IsNullOrEmpty(Convert.ToString(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.addr_source_value.ToString()])))
                {
                    DataTable ldtblList = busBase.Select<cdoCodeValue>(new string[2] { "code_id", "code_value" },
                        new object[2] {Convert.ToString(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.addr_source_id.ToString()]),
                    this.icdoPersonAddress.ihstOldValues[enmPersonAddress.addr_source_value.ToString()]}, null, null);

                    if (ldtblList != null && ldtblList.Rows.Count > 0)
                    {
                        this.icdoPersonAddress.addr_source_description = Convert.ToString(ldtblList.Rows[0]["description"]);
                    }
                }
            }
        }
        //PIR 430
        private void LoadCurrentAddressValues()
        {
            iobjcdoPersonAddress = new cdoPersonAddress();
            iobjcdoPersonAddress.addr_line_1 = this.icdoPersonAddress.addr_line_1;
            iobjcdoPersonAddress.addr_line_2 = this.icdoPersonAddress.addr_line_2;
            iobjcdoPersonAddress.addr_city = this.icdoPersonAddress.addr_city;
            iobjcdoPersonAddress.addr_state_id = this.icdoPersonAddress.addr_state_id;
            iobjcdoPersonAddress.addr_state_value = this.icdoPersonAddress.addr_state_value;
            iobjcdoPersonAddress.addr_country_value = this.icdoPersonAddress.addr_country_value;
            iobjcdoPersonAddress.addr_zip_code = this.icdoPersonAddress.addr_zip_code;
            iobjcdoPersonAddress.addr_zip_4_code = this.icdoPersonAddress.addr_zip_4_code;
            iobjcdoPersonAddress.foreign_province = this.icdoPersonAddress.foreign_province;
            iobjcdoPersonAddress.foreign_postal_code = this.icdoPersonAddress.foreign_postal_code;
            iobjcdoPersonAddress.start_date = DateTime.Now;
            iobjcdoPersonAddress.end_date = DateTime.MinValue;
            iobjcdoPersonAddress.secured_flag = this.icdoPersonAddress.secured_flag;
            iobjcdoPersonAddress.bad_address_flag = busConstant.FLAG_NO;
            iobjcdoPersonAddress.addr_source_description = this.icdoPersonAddress.addr_source_description;
        }
        //PIR 430
        private void UpdateNewAddressWithExisting()
        {
            this.icdoPersonAddress.addr_line_1 = iobjcdoPersonAddress.addr_line_1;
            this.icdoPersonAddress.addr_line_2 = iobjcdoPersonAddress.addr_line_2;
            this.icdoPersonAddress.addr_city = iobjcdoPersonAddress.addr_city;
            this.icdoPersonAddress.addr_state_id = iobjcdoPersonAddress.addr_state_id;
            this.icdoPersonAddress.addr_state_value = iobjcdoPersonAddress.addr_state_value;
            this.icdoPersonAddress.addr_country_value = iobjcdoPersonAddress.addr_country_value;
            this.icdoPersonAddress.addr_zip_code = iobjcdoPersonAddress.addr_zip_code;
            this.icdoPersonAddress.addr_zip_4_code = iobjcdoPersonAddress.addr_zip_4_code;
            this.icdoPersonAddress.foreign_province = iobjcdoPersonAddress.foreign_province;
            this.icdoPersonAddress.foreign_postal_code = iobjcdoPersonAddress.foreign_postal_code;
            this.icdoPersonAddress.start_date = iobjcdoPersonAddress.start_date;
            this.icdoPersonAddress.end_date = iobjcdoPersonAddress.end_date;
            this.icdoPersonAddress.secured_flag = iobjcdoPersonAddress.secured_flag;
            this.icdoPersonAddress.bad_address_flag = iobjcdoPersonAddress.bad_address_flag;
            this.icdoPersonAddress.addr_source_description = iobjcdoPersonAddress.addr_source_description;
            if (!this.iobjPassInfo.istrUserID.IsNullOrEmpty())
            {
                this.icdoPersonAddress.created_by = this.iobjPassInfo.istrUserID;
                this.icdoPersonAddress.modified_by = this.iobjPassInfo.istrUserID;
            }
            this.icdoPersonAddress.created_date = DateTime.Now;
            this.icdoPersonAddress.modified_date = DateTime.Now;
        }

        public void LoadStateDescription()
        {
            if (!string.IsNullOrEmpty(this.icdoPersonAddress.addr_country_value))
            {
                int aintCountryValue = Convert.ToInt32(this.icdoPersonAddress.addr_country_value);
                if (aintCountryValue == busConstant.AUSTRALIA || aintCountryValue == busConstant.CANADA || aintCountryValue == busConstant.MEXICO
               || aintCountryValue == busConstant.NewZealand || aintCountryValue == busConstant.OTHER_PROVINCE)
                {
                    DataTable ldtbCodeValue = busBase.Select("cdoPersonAddress.GetStateDescForRest", new object[2] { this.icdoPersonAddress.addr_state_value, aintCountryValue });
                    if (ldtbCodeValue.Rows.Count > 0)
                    {
                        this.icdoPersonAddress.addr_state_description = Convert.ToString(ldtbCodeValue.Rows[0]["DESCRIPTION"]);
                    }
                }
                if (aintCountryValue == busConstant.USA)
                {
                    DataTable ldtbCodeValue = busBase.Select("cdoPersonAddress.GetStateDescUSA", new object[1] { this.icdoPersonAddress.addr_state_value });
                    if (ldtbCodeValue.Rows.Count > 0)
                    {
                        this.icdoPersonAddress.addr_state_description = Convert.ToString(ldtbCodeValue.Rows[0]["DESCRIPTION"]);
                    }
                }
            }
        }
        /// <summary>
        /// Load Primary Address of a Person
        /// </summary>
        /// <param name="PersonID"></param>
        /// 

        public void LoadMailingAddress()
        {/*
            DataTable ldtbMailingAddress = Select<cdoPersonAddress>(
               new string[2] { enmPersonAddress.person_id.ToString(), enmPersonAddress.address_type_value.ToString() },
               new object[2] { icdoPersonAddress.person_id, busConstant.PHYSICAL_AND_MAILING_ADDRESS }, null, null);

            if (ldtbMailingAddress.Rows.Count > 0 && (ldtbMailingAddress.Rows[0]["End_Date"].ToString() == string.Empty
                || Convert.ToDateTime(ldtbMailingAddress.Rows[0]["End_Date"]) > DateTime.Today))
            {
                icdoPersonAddress.LoadData(ldtbMailingAddress.Rows[0]);
            }
            else
            {
                 ldtbMailingAddress = Select<cdoPersonAddress>(
                  new string[2] { enmPersonAddress.person_id.ToString(), enmPersonAddress.address_type_value.ToString() },
                  new object[2] { icdoPersonAddress.person_id, busConstant.MAILING_ADDRESS }, null, null);

                 if (ldtbMailingAddress.Rows.Count > 0 && ( ldtbMailingAddress.Rows[0]["End_Date"].ToString() == string.Empty
                      || Convert.ToDateTime(ldtbMailingAddress.Rows[0]["End_Date"]) > DateTime.Today))                //should not there be end date or greater than present date ,need an active address
                 {
                     icdoPersonAddress.LoadData(ldtbMailingAddress.Rows[0]);
                 }
                 else
                 {
                     ldtbMailingAddress = Select<cdoPersonAddress>(
                     new string[2] { enmPersonAddress.person_id.ToString(), enmPersonAddress.address_type_value.ToString() },
                     new object[2] { icdoPersonAddress.person_id, busConstant.PHYSICAL_ADDRESS }, null, null);
                     if (ldtbMailingAddress.Rows.Count > 0 && (ldtbMailingAddress.Rows[0]["End_Date"].ToString() == string.Empty
                          || Convert.ToDateTime(ldtbMailingAddress.Rows[0]["End_Date"]) > DateTime.Today))
                     {
                         icdoPersonAddress.LoadData(ldtbMailingAddress.Rows[0]);
                     }
                 }
            }*/
        }

        /// <summary>
        /// Update Person Address
        /// </summary>
        /// <param name="aintPersonId"></param>
        public void UpdatePersonAddress(int aintAddressID)
        {
            busPersonAddress lbusPersonAddress = new busPersonAddress();
            if (lbusPersonAddress.FindPersonAddress(aintAddressID))
            {
                lbusPersonAddress.icdoPersonAddress.addr_line_1 = icdoPersonAddress.addr_line_1;
                lbusPersonAddress.icdoPersonAddress.addr_line_2 = icdoPersonAddress.addr_line_2;
                lbusPersonAddress.icdoPersonAddress.addr_city = icdoPersonAddress.addr_city;
                lbusPersonAddress.icdoPersonAddress.addr_state_value = icdoPersonAddress.addr_state_value;
                lbusPersonAddress.icdoPersonAddress.addr_country_value = icdoPersonAddress.addr_country_value;
                lbusPersonAddress.icdoPersonAddress.addr_zip_code = icdoPersonAddress.addr_zip_code;
                lbusPersonAddress.icdoPersonAddress.addr_zip_4_code = icdoPersonAddress.addr_zip_4_code;
                lbusPersonAddress.icdoPersonAddress.foreign_province = icdoPersonAddress.foreign_province;
                lbusPersonAddress.icdoPersonAddress.foreign_postal_code = icdoPersonAddress.foreign_postal_code;
                lbusPersonAddress.icdoPersonAddress.start_date = icdoPersonAddress.start_date;
                lbusPersonAddress.icdoPersonAddress.end_date = icdoPersonAddress.end_date;
                lbusPersonAddress.icdoPersonAddress.Update();
            }
        }

        public void LoadInitialData()
        {
            CheckAlternateContact();
            //If Status is changed from No To Yes we are giving end date, but its not making an entry in iarrchangelog
            this.icdoPersonAddress.addr_line_1 = this.icdoPersonAddress.addr_line_1 + " ";
            this.istrOtherProvince = this.icdoPersonAddress.foreign_province;
        }

        /// <summary>
        /// Alternate Correspondence Address  will display “Yes” if “Correspondence Address” checkbox is checked on Contact Maintenance
        /// </summary>
        public void CheckAlternateContact()
        {
            DataTable ldtbAlternateCorp = Select("cdoPersonAddress.GetAlternateCorrespondence", new object[1] { this.icdoPersonAddress.person_id });
            if (ldtbAlternateCorp.Rows.Count > 0)
            {
                this.icdoPersonAddress.istrAlternateCorrespondenceAddress = busConstant.YES;
            }
            else
            {
                this.icdoPersonAddress.istrAlternateCorrespondenceAddress = busConstant.NO;
            }
        }

        public void UpdateEndDate()
        {
            bool lblnCheckBoth = false;
            cdoPersonAddressChklist lcdoAddressChklist = new cdoPersonAddressChklist();
            if (this.iclcPersonAddressChklist.Count == 2)
            {
                if (iclcPersonAddressChklist[0].ienuObjectState != ObjectState.CheckListDelete &&
                    iclcPersonAddressChklist[1].ienuObjectState != ObjectState.CheckListDelete)
                {
                    lblnCheckBoth = true;
                }
            }
            else
            {
                lblnCheckBoth = false;
            }
            if (lblnCheckBoth)
            {
                UpdateEndDateInExistAdd(true, string.Empty);
            }
            else
            {
                if (this.iclcPersonAddressChklist.Count > 0)
                {
                    UpdateEndDateInExistAdd(false, iclcPersonAddressChklist[0].address_type_value);
                }
            }
        }

        public void UpdateEndDateInExistAdd(bool lblnCheckBoth, string strAddressType)
        {

            foreach (busPersonAddress lbusPersonAddress in this.ibusPerson.iclbPersonAddress)
            {
                if (lbusPersonAddress.icdoPersonAddress.address_id != this.icdoPersonAddress.address_id)
                {
                    if (lbusPersonAddress.icdoPersonAddress.end_date == DateTime.MinValue || lbusPersonAddress.icdoPersonAddress.end_date > this.icdoPersonAddress.start_date)
                    {
                        if (lblnCheckBoth || (lbusPersonAddress.iclcPersonAddressChklist.Count == 1 && strAddressType == lbusPersonAddress.iclcPersonAddressChklist[0].address_type_value))
                        {
                            if (lbusPersonAddress.icdoPersonAddress.start_date < this.icdoPersonAddress.start_date && (lbusPersonAddress.icdoPersonAddress.end_date == DateTime.MinValue || lbusPersonAddress.icdoPersonAddress.end_date > this.icdoPersonAddress.start_date))
                            {
                                lbusPersonAddress.icdoPersonAddress.end_date = this.icdoPersonAddress.start_date.Subtract(new TimeSpan(1, 0, 0, 0));
                                lbusPersonAddress.icdoPersonAddress.Update();
                            }
                            else if (lbusPersonAddress.icdoPersonAddress.start_date == this.icdoPersonAddress.start_date && lbusPersonAddress.icdoPersonAddress.end_date != lbusPersonAddress.icdoPersonAddress.start_date)
                            {
                                lbusPersonAddress.icdoPersonAddress.end_date = lbusPersonAddress.icdoPersonAddress.start_date;
                                lbusPersonAddress.icdoPersonAddress.Update();
                            }
                        }
                        else if (lbusPersonAddress.iclcPersonAddressChklist.Count == 2)
                        {
                            lbusPersonAddress.icdoPersonAddress.iblnActualChange = true;
                            lbusPersonAddress.LoadOldAddressInAddressHistory();
                            lbusPersonAddress.SaveOldAddressInAddressHistory();
                            foreach (cdoPersonAddressChklist lcdoPersonAddressChklist in lbusPersonAddress.iclcPersonAddressChklist)
                            {
                                if (lcdoPersonAddressChklist.address_type_value == strAddressType)
                                {
                                    lcdoPersonAddressChklist.Delete();
                                    break;
                                }
                            }
                        }

                    }

                }
            }
        }

        public bool CheckDuplicateActiveAddressType()
        {

            if (this.ibusPerson == null)
            {
                return false;
            }
            if (this.icdoPersonAddress.start_date != DateTime.MinValue)
            {
                DateTime ldtCurrentEnd = this.icdoPersonAddress.end_date;
                if (ldtCurrentEnd == DateTime.MinValue)
                {
                    ldtCurrentEnd = DateTime.MaxValue;
                }
                foreach (cdoPersonAddressChklist lcdoPersonAddressChklist in this.iclcPersonAddressChklist)
                {
                    if (lcdoPersonAddressChklist.address_type_value == "MAIL" && lcdoPersonAddressChklist.ienuObjectState != ObjectState.CheckListDelete)
                    {

                        foreach (busPersonAddress lbusPersonAddress in this.ibusPerson.iclbPersonAddress)
                        {
                            if (lbusPersonAddress.icdoPersonAddress.address_id != this.icdoPersonAddress.address_id && this.icdoPersonAddress.istrAddSameAsParticipantFlag != busConstant.FLAG_YES)
                            {
                                DateTime ldtExist = lbusPersonAddress.icdoPersonAddress.end_date;
                                if (ldtExist == DateTime.MinValue)
                                {
                                    ldtExist = DateTime.MaxValue;
                                }
                                foreach (cdoPersonAddressChklist lChklist in lbusPersonAddress.iclcPersonAddressChklist)
                                {
                                    if (lChklist.address_type_value == "MAIL")
                                    {
                                        if (lbusPersonAddress.icdoPersonAddress.start_date > this.icdoPersonAddress.start_date &&
                                            ldtCurrentEnd > lbusPersonAddress.icdoPersonAddress.start_date)
                                        {
                                            return true;
                                        }

                                    }
                                }
                            }
                        }
                    }
                    if (lcdoPersonAddressChklist.address_type_value == "PYSL" && lcdoPersonAddressChklist.ienuObjectState != ObjectState.CheckListDelete)
                    {
                        foreach (busPersonAddress lbusPersonAddress in this.ibusPerson.iclbPersonAddress)
                        {
                            if (lbusPersonAddress.icdoPersonAddress.address_id != this.icdoPersonAddress.address_id && this.icdoPersonAddress.istrAddSameAsParticipantFlag != busConstant.FLAG_YES)
                            {
                                DateTime ldtExist = lbusPersonAddress.icdoPersonAddress.end_date;
                                if (ldtExist == DateTime.MinValue)
                                {
                                    ldtExist = DateTime.MaxValue;
                                }
                                foreach (cdoPersonAddressChklist lChklist in lbusPersonAddress.iclcPersonAddressChklist)
                                {
                                    if (lChklist.address_type_value == "PYSL")
                                    {
                                        if (lbusPersonAddress.icdoPersonAddress.start_date > this.icdoPersonAddress.start_date &&
                                            ldtCurrentEnd > lbusPersonAddress.icdoPersonAddress.start_date)
                                        {
                                            return true;
                                        }

                                    }
                                }
                            }
                        }
                    }

                }

            }
            return false;
        }


        public bool CheckDuplicateAddressType()
        {

            if (this.ibusPerson == null)
            {
                return false;
            }
            if (this.icdoPersonAddress.start_date != DateTime.MinValue)
            {
                DateTime ldtCurrentEnd = this.icdoPersonAddress.end_date;
                if (ldtCurrentEnd == DateTime.MinValue)
                {
                    ldtCurrentEnd = DateTime.MaxValue;
                }
                foreach (cdoPersonAddressChklist lcdoPersonAddressChklist in this.iclcPersonAddressChklist)
                {
                    if (lcdoPersonAddressChklist.address_type_value == "MAIL" && lcdoPersonAddressChklist.ienuObjectState != ObjectState.CheckListDelete)
                    {

                        foreach (busPersonAddress lbusPersonAddress in this.ibusPerson.iclbPersonAddress)
                        {
                            if (lbusPersonAddress.icdoPersonAddress.address_id != this.icdoPersonAddress.address_id && this.icdoPersonAddress.istrAddSameAsParticipantFlag != busConstant.FLAG_YES)
                            {
                                DateTime ldtExist = lbusPersonAddress.icdoPersonAddress.end_date;
                                if (ldtExist == DateTime.MinValue)
                                {
                                    ldtExist = DateTime.MaxValue;
                                }
                                foreach (cdoPersonAddressChklist lChklist in lbusPersonAddress.iclcPersonAddressChklist)
                                {
                                    if (lChklist.address_type_value == "MAIL")
                                    {
                                        if (lbusPersonAddress.icdoPersonAddress.start_date <= this.icdoPersonAddress.start_date &&
                                    this.icdoPersonAddress.start_date <= ldtExist)
                                        {
                                            return true;
                                        }
                                        else if (lbusPersonAddress.icdoPersonAddress.start_date >= this.icdoPersonAddress.start_date &&
                                            ldtCurrentEnd >= lbusPersonAddress.icdoPersonAddress.start_date)
                                        {
                                            return true;
                                        }

                                    }
                                }
                            }
                        }
                    }
                    if (lcdoPersonAddressChklist.address_type_value == "PYSL" && lcdoPersonAddressChklist.ienuObjectState != ObjectState.CheckListDelete)
                    {
                        foreach (busPersonAddress lbusPersonAddress in this.ibusPerson.iclbPersonAddress)
                        {
                            if (lbusPersonAddress.icdoPersonAddress.address_id != this.icdoPersonAddress.address_id && this.icdoPersonAddress.istrAddSameAsParticipantFlag != busConstant.FLAG_YES)
                            {
                                DateTime ldtExist = lbusPersonAddress.icdoPersonAddress.end_date;
                                if (ldtExist == DateTime.MinValue)
                                {
                                    ldtExist = DateTime.MaxValue;
                                }
                                foreach (cdoPersonAddressChklist lChklist in lbusPersonAddress.iclcPersonAddressChklist)
                                {
                                    if (lChklist.address_type_value == "PYSL")
                                    {
                                        if (lbusPersonAddress.icdoPersonAddress.start_date <= this.icdoPersonAddress.start_date &&
                                    this.icdoPersonAddress.start_date <= ldtExist)
                                        {
                                            return true;
                                        }
                                        else if (lbusPersonAddress.icdoPersonAddress.start_date >= this.icdoPersonAddress.start_date &&
                                            ldtCurrentEnd >= lbusPersonAddress.icdoPersonAddress.start_date)
                                        {
                                            return true;
                                        }

                                    }
                                }
                            }
                        }
                    }

                }

            }
            return false;
        }

        public void LoadPersonAddressChklistsOld()
        {
            iclcPersonAddressChklistOld = GetCollection<cdoPersonAddressChklist>(
                new string[1] { enmPersonAddressChklist.address_id.ToString() },
                new object[1] { icdoPersonAddress.address_id }, null, null);
        }

        public void SaveOldAddressInAddressHistory()
        {

            if (this.icdoPersonAddress.iblnActualChange || CheckChangeinAddressType())
            {
                this.ibusPersonAddressHistory.icdoPersonAddressHistory.Insert();
                foreach (cdoPersonAddressChklist lcdoPersonAddressCheckListold in iclcPersonAddressChklistOld)
                {
                    cdoPersonAddressChklistHistory lcdoPersonAddressChklistHistory = new cdoPersonAddressChklistHistory();
                    lcdoPersonAddressChklistHistory.person_address_history_id = ibusPersonAddressHistory.icdoPersonAddressHistory.person_address_history_id;
                    lcdoPersonAddressChklistHistory.address_type_id = lcdoPersonAddressCheckListold.address_type_id;
                    lcdoPersonAddressChklistHistory.address_type_value = lcdoPersonAddressCheckListold.address_type_value;
                    lcdoPersonAddressChklistHistory.Insert();
                }
            }
        }

        public void LoadOldAddressInAddressHistory()
        {
            // Loading Checklist Items : Old
            LoadPersonAddressChklistsOld();

            this.ibusPersonAddressHistory = new busPersonAddressHistory { icdoPersonAddressHistory = new cdoPersonAddressHistory(), iclcPersonAddressChklistHistory = new utlCollection<cdoPersonAddressChklistHistory>() };
            ibusPersonAddressHistory.icdoPersonAddressHistory.address_id = Convert.ToInt32(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.address_id.ToString()]);

            ibusPersonAddressHistory.icdoPersonAddressHistory.addr_line_1 = Convert.ToString(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.addr_line_1.ToString()]);
            ibusPersonAddressHistory.icdoPersonAddressHistory.addr_line_1 = Convert.ToString(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.addr_line_1.ToString()]);
            ibusPersonAddressHistory.icdoPersonAddressHistory.addr_line_2 = Convert.ToString(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.addr_line_2.ToString()]);
            ibusPersonAddressHistory.icdoPersonAddressHistory.addr_city = Convert.ToString(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.addr_city.ToString()]);
            ibusPersonAddressHistory.icdoPersonAddressHistory.addr_state_id = Convert.ToInt32(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.addr_state_id.ToString()]);
            ibusPersonAddressHistory.icdoPersonAddressHistory.addr_state_value = Convert.ToString(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.addr_state_value.ToString()]);
            ibusPersonAddressHistory.icdoPersonAddressHistory.addr_country_value = Convert.ToString(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.addr_country_value.ToString()]);
            ibusPersonAddressHistory.icdoPersonAddressHistory.addr_zip_code = Convert.ToString(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.addr_zip_code.ToString()]);
            ibusPersonAddressHistory.icdoPersonAddressHistory.addr_zip_4_code = Convert.ToString(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.addr_zip_4_code.ToString()]);
            //ibusPersonAddressHistory.icdoPersonAddressHistory.address_type_value = Convert.ToString(iarrChangeLog[0].ihstOldValues[enmPersonAddress.address_type_value.ToString()]);
            ibusPersonAddressHistory.icdoPersonAddressHistory.foreign_province = Convert.ToString(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.foreign_province.ToString()]);
            ibusPersonAddressHistory.icdoPersonAddressHistory.foreign_postal_code = Convert.ToString(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.foreign_postal_code.ToString()]);
            ibusPersonAddressHistory.icdoPersonAddressHistory.start_date = Convert.ToDateTime(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.start_date.ToString()]);
            ibusPersonAddressHistory.icdoPersonAddressHistory.end_date = Convert.ToDateTime(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.end_date.ToString()]);
            ibusPersonAddressHistory.icdoPersonAddressHistory.secured_flag = Convert.ToString(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.secured_flag.ToString()]);
            ibusPersonAddressHistory.icdoPersonAddressHistory.bad_address_flag = Convert.ToString(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.bad_address_flag.ToString()]);
            if (!string.IsNullOrEmpty(Convert.ToString(this.icdoPersonAddress.ihstOldValues[enmPersonAddress.addr_source_value.ToString()])))
            {
                DataTable ldtblList = busBase.Select<cdoCodeValue>(new string[2] { "code_id", "code_value" },
                 new object[2] { this.icdoPersonAddress.ihstOldValues[enmPersonAddress.addr_source_id.ToString()],
            this.icdoPersonAddress.ihstOldValues[enmPersonAddress.addr_source_value.ToString()]}, null, null);

                if (ldtblList != null && ldtblList.Rows.Count > 0)
                {
                    ibusPersonAddressHistory.icdoPersonAddressHistory.addr_source_desc = Convert.ToString(ldtblList.Rows[0]["description"]);
                }
            }
            ibusPersonAddressHistory.icdoPersonAddressHistory.PopulateDescriptions();

        }

        public bool CheckChangeinAddressType()
        {
            foreach (doBase ldoBase in iarrChangeLog)
            {
                if (ldoBase is cdoPersonAddressChklist)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsNewMode()
        {
            iblnIsNewMode = false;
            if (this.icdoPersonAddress.ienuObjectState == ObjectState.Insert)
            {
                iblnIsNewMode = true;
            }
            return iblnIsNewMode;
        }

        public void LoadMainParticipantsAddress()
        {
            bool lblnIsbeneficary = this.ibusPerson.CheckMemberIsBeneficiary();
            bool lblnIsDependent = this.ibusPerson.CheckMemberIsDependent();

            DataTable ldtbRelation = Select<cdoRelationship>(
               new string[1] { enmRelationship.beneficiary_person_id.ToString() },
               new object[1] { this.ibusPerson.icdoPerson.person_id }, null, null);
            if (ldtbRelation.Rows.Count == 0)
            {
                ldtbRelation = Select<cdoRelationship>(
               new string[1] { enmRelationship.dependent_person_id.ToString() },
               new object[1] { this.ibusPerson.icdoPerson.person_id }, null, null);
            }

            if (ldtbRelation.Rows.Count > 0)
            {
                //this.ibusRelationship = new busRelationship { icdoRelationship = new cdoRelationship() };
                int lintPersonId = Convert.ToInt32(ldtbRelation.Rows[0][enmRelationship.person_id.ToString()]);
                this.ibusRelationship.icdoRelationship.LoadData(ldtbRelation.Rows[0]);
                LoadActiveAddress(lintPersonId);
            }

        }

        public void LoadActiveAddress(int aintPersonID)
        {
            ibusMainParticipantAddress = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() };
            DataTable ldtbActiveAddress = Select("cdoPersonContact.GetActiveAddress", new object[1] { aintPersonID });
            if (ldtbActiveAddress.Rows.Count > 0)
            {
                if (ldtbActiveAddress.Rows.Count == 1)
                {
                    ibusMainParticipantAddress.icdoPersonAddress.LoadData(ldtbActiveAddress.Rows[0]);
                }
                else
                {
                    foreach (DataRow dtRow in ldtbActiveAddress.Rows)
                    {
                        if (dtRow[enmPersonAddressChklist.address_type_value.ToString()].ToString() == busConstant.MAILING_ADDRESS)
                        {
                            ibusMainParticipantAddress.icdoPersonAddress.LoadData(dtRow);
                            break;
                        }

                    }
                }
                ibusMainParticipantAddress.LoadPersonAddressChklists();
                SetPhysivcalMailingFlagValue(ibusMainParticipantAddress.iclcPersonAddressChklist, ibusMainParticipantAddress);
            }
        }

        public void SetPhysivcalMailingFlagValue(utlCollection<cdoPersonAddressChklist> aiclcPersonAddressChklist, busPersonAddress abusPersonAddress)
        {
            abusPersonAddress.icdoPersonAddress.istrPhysicalAddressType = busConstant.NO;
            abusPersonAddress.icdoPersonAddress.istrMailingAddressType = busConstant.NO;
            foreach (cdoPersonAddressChklist lcdoPersonAddressChklist in aiclcPersonAddressChklist)
            {
                if (lcdoPersonAddressChklist.address_type_value == busConstant.PHYSICAL_ADDRESS)
                {
                    abusPersonAddress.icdoPersonAddress.istrPhysicalAddressType = busConstant.YES;
                }
                else if (lcdoPersonAddressChklist.address_type_value == busConstant.MAILING_ADDRESS)
                {
                    abusPersonAddress.icdoPersonAddress.istrMailingAddressType = busConstant.YES;
                }
            }
        }

        /// <summary>
        /// When Same as Participant's flag is checked
        /// </summary>
        public void LoadBeneficiaryDependentData()
        {

            //if (astrType.ToUpper() == "BEN")
            //{
            this.ibusRelationship = new busRelationship { icdoRelationship = new cdoRelationship() };
            DataTable ldtbBenefeciary = Select("cdoPersonAddress.LoadRelationshipData", new object[1] { this.ibusPerson.icdoPerson.person_id });
            if (ldtbBenefeciary.Rows.Count > 0)
            {
                this.ibusRelationship.icdoRelationship.LoadData(ldtbBenefeciary.Rows[0]);
            }
            //}
            //else if (astrType.ToUpper() == "DEP")
            //{
            //    this.ibusPersonDependent = new busPersonDependent { icdoPersonDependent = new cdoPersonDependent() };
            //    DataTable ldtbDependent = Select("cdoPersonAddress.LoadDependentData", new object[1] { this.ibusPerson.icdoPerson.person_id });
            //    if (ldtbDependent.Rows.Count > 0)
            //    {
            //        this.ibusPersonDependent.icdoPersonDependent.LoadData(ldtbDependent.Rows[0]);
            //    }
            //}
        }

        public void LoadMainParticipantAddress(int aintParticipantID)
        {
            this.ibusMainParticipantAddress = new busPersonAddress();
            this.ibusMainParticipantAddress.FindPersonAddress(aintParticipantID);
            this.ibusMainParticipantAddress.LoadStateDescription();
            this.ibusMainParticipantAddress.LoadPersonAddressChklists();
            this.SetPhysivcalMailingFlagValue(this.ibusMainParticipantAddress.iclcPersonAddressChklist, this.ibusMainParticipantAddress);

        }

        public bool CheckAddressTypeMandatory()
        {
            bool blnType = false;
            int lcount = 0;
            foreach (cdoPersonAddressChklist lcdoPersonAddressChklist in this.iclcPersonAddressChklist)
            {
                if (lcdoPersonAddressChklist.ienuObjectState == ObjectState.CheckListDelete)
                {
                    lcount++;
                }
            }
            if (lcount == iclcPersonAddressChklist.Count)
            {
                blnType = true;
            }
            return blnType;
        }

        public Collection<cdoCodeValue> GetStateOrProvinceBasedOnCountry(int aintCountryValue)
        {
            Collection<cdoCodeValue> lclcStates = null;
            DataTable ldtbResult = null;
            if (aintCountryValue == busConstant.USA)
            {
                ldtbResult = busBase.Select("cdoPersonAddress.GetStatesForUSA", new object[0] { });
                lclcStates = doBase.GetCollection<cdoCodeValue>(ldtbResult);
            }
            else if (aintCountryValue == busConstant.AUSTRALIA || aintCountryValue == busConstant.CANADA || aintCountryValue == busConstant.MEXICO
                || aintCountryValue == busConstant.NewZealand || aintCountryValue == busConstant.OTHER_PROVINCE)
            {
                ldtbResult = busBase.Select("cdoPersonAddress.GetProvinces", new object[1] { aintCountryValue });
                lclcStates = doBase.GetCollection<cdoCodeValue>(ldtbResult);
            }

            return lclcStates;
        }

        public ArrayList ShowHistory()
        {
            ArrayList larrArrayList = new ArrayList();
            this.LoadPersonAddressHistorys();
            larrArrayList.Add(this);
            return larrArrayList;
        }
        void addressChangeForBenificiary()
        {
            busBase lobjBase = new busBase();
            this.ibusPerson.LoadBeneficiaries();
            DataTable ldtbBenificiaryPerson = Select("cdoPerson.GetBenificiaryPerson", new object[1] { this.ibusPerson.icdoPerson.person_id });
            Collection<busPerson> iclbBenificiaryPerson = new Collection<busPerson>();
            iclbBenificiaryPerson = lobjBase.GetCollection<busPerson>(ldtbBenificiaryPerson, "icdoPerson");
            foreach (busPerson tbusPerson in iclbBenificiaryPerson)
            {
                tbusPerson.LoadPersonAddresss();
                foreach (busPersonAddress tbusPersonAddress in tbusPerson.iclbPersonAddress)
                {
                    //int MailingAddress=this.iclcPersonAddressChklis
                    busPersonAddress lbusPersonAddress = tbusPersonAddress;
                    foreach (cdoPersonAddressChklist lChklist in this.iclcPersonAddressChklist)
                    {
                        if (lChklist.address_type_value == "MAIL")
                        {
                            int personaddid = lbusPersonAddress.icdoPersonAddress.person_id;
                            LoadMainPaddToBenificaryAddress(this, lbusPersonAddress);
                            //lbusPersonAddress = this;
                            lbusPersonAddress.icdoPersonAddress.person_id = personaddid;
                            lbusPersonAddress.icdoPersonAddress.Update();
                        }
                    }
                }

            }
            //this.ibusPerson.ic
        }
        void LoadMainPaddToBenificaryAddress(busPersonAddress lbussource, busPersonAddress lbusDestination)
        {
            lbusDestination.icdoPersonAddress.addr_line_1 = lbussource.icdoPersonAddress.addr_line_1;
            lbusDestination.icdoPersonAddress.addr_line_2 = lbussource.icdoPersonAddress.addr_line_2;
            lbusDestination.icdoPersonAddress.addr_city = lbussource.icdoPersonAddress.addr_city;
            lbusDestination.icdoPersonAddress.addr_state_id = lbussource.icdoPersonAddress.addr_state_id;
            lbusDestination.icdoPersonAddress.addr_state_value = lbussource.icdoPersonAddress.addr_state_value;
            lbusDestination.icdoPersonAddress.addr_state_value = lbussource.icdoPersonAddress.addr_state_value;
            lbusDestination.icdoPersonAddress.addr_country_id = lbussource.icdoPersonAddress.addr_country_id;
            lbusDestination.icdoPersonAddress.addr_country_value = lbussource.icdoPersonAddress.addr_country_value;
            lbusDestination.icdoPersonAddress.addr_zip_code = lbussource.icdoPersonAddress.addr_zip_code;
            lbusDestination.icdoPersonAddress.start_date = lbussource.icdoPersonAddress.start_date;
            lbusDestination.icdoPersonAddress.end_date = lbussource.icdoPersonAddress.end_date;
            lbusDestination.icdoPersonAddress.secured_flag = lbussource.icdoPersonAddress.secured_flag;
            lbusDestination.icdoPersonAddress.addr_source_id = lbussource.icdoPersonAddress.addr_source_id;
            lbusDestination.icdoPersonAddress.addr_source_value = lbussource.icdoPersonAddress.addr_source_value;
            lbusDestination.icdoPersonAddress.county = lbussource.icdoPersonAddress.county;
        }

        #endregion
    }
}
