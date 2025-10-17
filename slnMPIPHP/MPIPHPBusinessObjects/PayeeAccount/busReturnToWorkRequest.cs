#region Using directives

using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
using NeoSpin.BusinessObjects;
using Sagitec.Bpm;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.CorBuilder;
using Sagitec.DBUtility;
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

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busReturnToWork:
    /// Inherited from busReturnToWorkGen, the class is used to customize the business object busReturnToWorkGen.
    /// </summary>
    [Serializable]
    public partial class busReturnToWorkRequest : busReturnToWorkRequestGen
    {
        public string istrPayeeAccountActive { get; set; }

        public string istrParticipantRetiree { get; set; }

        public string istrParticipantHoursInPayrollMonth { get; set; }

        public string istrIsEligibleToRTW { get; set; }

        public string istrIsWithinPaymentThreshold { get; set; }

        public DateTime idtPaymentAccountSuspensionDate { get; set; }

        public string istrIsSecondAuditRequired { get; set; }

        public string istrIsAllPayeeAccountApproved { get; set; }

        public bool IsSubmitButtonVisible()
        {
            if (this.icdoReturnToWorkRequest.request_type_value == busConstant.ReturnToWorkRequest.REQUEST_TYPE_RTW)
            {
                if (ibusBaseActivityInstance != null)
                {
                    busBpmActivityInstance lbusBpmActivityInstance = ibusBaseActivityInstance as busBpmActivityInstance;

                    if (lbusBpmActivityInstance != null && lbusBpmActivityInstance.ibusBpmActivity.icdoBpmActivity.name == busConstant.ReturnToWorkRequest.BPM_PRINT_REEMPLOYMENT_NOTIFICATION_FORM
                        && this.icdoReturnToWorkRequest.status_value == busConstant.ReturnToWorkRequest.STATUS_REVEIW)
                    {
                        return true;
                    }
                    else if (this.icdoReturnToWorkRequest.status_value == busConstant.ReturnToWorkRequest.STATUS_VALID || this.icdoReturnToWorkRequest.status_value == busConstant.ReturnToWorkRequest.STATUS_CANCEL)
                    {
                        return true;
                    }
                }
            }
            else if (this.icdoReturnToWorkRequest.request_type_value == busConstant.ReturnToWorkRequest.REQUEST_TYPE_ERTW)
            {
                if (this.icdoReturnToWorkRequest.status_value.In(new string[] { busConstant.ReturnToWorkRequest.STATUS_REVEIW, busConstant.ReturnToWorkRequest.STATUS_VALID, busConstant.ReturnToWorkRequest.STATUS_CANCEL }))
                { 
                    return true; 
                }
            }
            return false;
        }

        public bool IsCancelButtonVisible()
        {
            if (ibusBaseActivityInstance != null)
            {
                busBpmActivityInstance lbusBpmActivityInstance = ibusBaseActivityInstance as busBpmActivityInstance;
                if (lbusBpmActivityInstance != null && !(lbusBpmActivityInstance.ibusBpmActivity is busBpmServiceTask))
                    return true;
            }
            return false;
        }

        public bool IsFormReadOnly()
        {
            if (ibusBaseActivityInstance == null || this.icdoReturnToWorkRequest.status_value == busConstant.ReturnToWorkRequest.STATUS_PROCESSED || this.icdoReturnToWorkRequest.status_value == busConstant.ReturnToWorkRequest.STATUS_CANCEL)
            {
                return true;
            }
            return false;
        }
        public bool IsPaymentAccountSuspensionDateVisible()
        {
            if ((this.icdoReturnToWorkRequest.payment_account_suspension_date != null && DateTime.MinValue < this.icdoReturnToWorkRequest.payment_account_suspension_date && this.icdoReturnToWorkRequest.payment_account_suspension_date < DateTime.MaxValue))
            {
                return true;
            }
            return false;
        }

        public bool IsEndOfReturnToWork()
        {
            if (this.icdoReturnToWorkRequest.request_type_value == busConstant.ReturnToWorkRequest.REQUEST_TYPE_ERTW)
                return true;
            return false;
        }

        public void LoadPayeeAccount()
        {
            this.istrPayeeAccountActive = busConstant.ReturnToWorkRequest.NOT_MET;
            DataTable ldtblist = null;
            if (this.icdoReturnToWorkRequest.request_type_value == busConstant.ReturnToWorkRequest.REQUEST_TYPE_RTW)
                ldtblist = busPerson.Select("entReturnToWorkRequest.LoadPayeeAccounts", new object[1] { this.ibusPayee.icdoPerson.person_id });
            else if (this.icdoReturnToWorkRequest.request_type_value == busConstant.ReturnToWorkRequest.REQUEST_TYPE_ERTW)
                ldtblist = busPerson.Select("entReturnToWorkRequest.LoadPayeeAccountsERTW", new object[1] { this.ibusPayee.icdoPerson.person_id });

            iclbPayeeAccount = GetCollection<busPayeeAccount>(ldtblist, "icdoPayeeAccount");
            if (iclbPayeeAccount.Count() > 0)
            {
                if (iclbPayeeAccount.Any(p =>p.icdoPayeeAccount.istrStatus == busConstant.ReturnToWorkRequest.STATUS_RECEIVING ))
                {
                    this.istrPayeeAccountActive = busConstant.ReturnToWorkRequest.MET;
                }
            }
        }
        public void LoadCorTracking()
        {
            DataTable ldtblist = busPerson.Select("entCorTracking.LoadReturnToWorkRequestTracking", new object[2] { this.ibusPayee.icdoPerson.person_id, this.icdoReturnToWorkRequest.reemployment_notification_id });
            iclbCorTracking = GetCollection<busCorTracking>(ldtblist, "icdoCorTracking");
            if (iclbCorTracking != null && iclbCorTracking.Count > 0)
            {
                foreach (busCorTracking lbusCorTracking in iclbCorTracking)
                {
                    lbusCorTracking.ibusCorTemplates = new busCorTemplates { icdoCorTemplates = new cdoCorTemplates() };
                    lbusCorTracking.ibusCorTemplates.FindCorTemplates(lbusCorTracking.icdoCorTracking.template_id);

                    lbusCorTracking.ibusCorPacketContentTracking = new busCorPacketContentTracking { icdoCorPacketContentTracking = new cdoCorPacketContentTracking() };
                    lbusCorTracking.ibusCorPacketContentTracking.FindCorPacketContentTracking(lbusCorTracking.icdoCorTracking.tracking_id);

                }
            }
        }
        public override void BeforePersistChanges()
        {

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

            if (this.iclbNotes != null)
            {
                this.iclbNotes.ForEach(item =>
                {
                    if (item.icdoNotes.org_id == 0)
                        item.icdoNotes.person_id = this.ibusPayee.icdoPerson.person_id;
                    item.icdoNotes.form_id = busConstant.Form_ID;
                    item.icdoNotes.form_value = busConstant.ReturnToWorkRequest.RETURN_TO_WORK_REQUEST_FORM;
                    item.icdoNotes.reference_id = this.icdoReturnToWorkRequest.reemployment_notification_id;

                });
            }

            if (this.icdoReturnToWorkRequest.request_type_value == busConstant.ReturnToWorkRequest.REQUEST_TYPE_RTW)
            {
                ValidateReturnToWorkRequestSoftErrors();
                ValidateReturnToWorkRequest();
            }
            else if (this.icdoReturnToWorkRequest.request_type_value == busConstant.ReturnToWorkRequest.REQUEST_TYPE_ERTW)
            {
                ValidateEndOfReturnToWorkRequestSoftErrors();
            }
            if (ibusBaseActivityInstance != null)
            {
                busBpmActivityInstance lbusBpmActivityInstance = ibusBaseActivityInstance as busBpmActivityInstance;

                if (lbusBpmActivityInstance != null && lbusBpmActivityInstance.ibusBpmActivity.icdoBpmActivity.name == busConstant.ReturnToWorkRequest.BPM_CAPTURE_AND_VALIDATION_ACTIVITY_NAME)
                {
                   
                    if (this.istrIsEligibleToRTW == busConstant.ReturnToWorkRequest.YES)
                    {
                        DateTime ldtCurrentDate = DateTime.Now;
                        DateTime ldtBlackOutStartDate = new DateTime(ldtCurrentDate.Year, ldtCurrentDate.Month, busConstant.ReturnToWorkRequest.BLACK_OUT_START_DAY);
                        DateTime ldtBlackOutEndDate = new DateTime(ldtCurrentDate.Year, ldtCurrentDate.Month, busConstant.ReturnToWorkRequest.BLACK_OUT_END_DAY);
                        int iintPayeeAccountSuspensionDay = busConstant.ReturnToWorkRequest.PAYEE_ACCOUNT_SUSPENSION_DAY;

                        List<utlCodeValue> codeValuesFromDict = base.iobjPassInfo.isrvDBCache.GetCodeValuesFromDict(busConstant.ReturnToWorkRequest.RTW_BLACKOUT_DAYS);
                        if (codeValuesFromDict.Count > 0)
                        {
                            ldtBlackOutStartDate = new DateTime(ldtCurrentDate.Year, ldtCurrentDate.Month, Convert.ToInt32(codeValuesFromDict[0].data1));
                            ldtBlackOutEndDate = new DateTime(ldtCurrentDate.Year, ldtCurrentDate.Month, Convert.ToInt32(codeValuesFromDict[0].data2));
                            iintPayeeAccountSuspensionDay = Convert.ToInt32(codeValuesFromDict[0].data3);
                        }

                        busReturnToWorkPayrollDetails lbusReturnToWorkPayrollDetails = new busReturnToWorkPayrollDetails();
                        busReturnToWorkPayrollDetails lReturnToworkPayrollDetails = lbusReturnToWorkPayrollDetails.GetPayrollPeriod(this.icdoReturnToWorkRequest.reemployment_start_date);

                        busReturnToWorkPayrollDetails lCurrentPayrollDetails = lbusReturnToWorkPayrollDetails.GetPayrollPeriod(ldtCurrentDate);
                        busReturnToWorkPayrollDetails lImmediateNextPayrollDetails = lbusReturnToWorkPayrollDetails.GetPayrollPeriod(lCurrentPayrollDetails.idtPayrollEndDate.AddDays(1));

                        if (this.icdoReturnToWorkRequest.reemployment_start_date != null
                            && (this.icdoReturnToWorkRequest.reemployment_start_date < ldtCurrentDate
                                || (lCurrentPayrollDetails.idtPayrollStartDate <= this.icdoReturnToWorkRequest.reemployment_start_date
                                    && this.icdoReturnToWorkRequest.reemployment_start_date <= lCurrentPayrollDetails.idtPayrollEndDate)
                                || (lImmediateNextPayrollDetails.idtPayrollStartDate <= this.icdoReturnToWorkRequest.reemployment_start_date
                                    && this.icdoReturnToWorkRequest.reemployment_start_date <= lImmediateNextPayrollDetails.idtPayrollEndDate)))
                        {
                            this.istrIsWithinPaymentThreshold = busConstant.ReturnToWorkRequest.YES;
                            this.idtPaymentAccountSuspensionDate = ldtCurrentDate;
                            this.icdoReturnToWorkRequest.payment_account_suspension_date = ldtCurrentDate;
                            if (ldtBlackOutStartDate <= ldtCurrentDate && ldtCurrentDate <= ldtBlackOutEndDate)
                            {
                                this.istrIsWithinPaymentThreshold = busConstant.ReturnToWorkRequest.NO;
                                this.idtPaymentAccountSuspensionDate = new DateTime(ldtCurrentDate.Year, ldtCurrentDate.Month, iintPayeeAccountSuspensionDay);
                                this.icdoReturnToWorkRequest.payment_account_suspension_date = new DateTime(ldtCurrentDate.Year, ldtCurrentDate.Month, iintPayeeAccountSuspensionDay);
                            }
                        }
                        if (this.icdoReturnToWorkRequest.reemployment_start_date > lImmediateNextPayrollDetails.idtPayrollEndDate)
                        {
                            this.istrIsWithinPaymentThreshold = busConstant.ReturnToWorkRequest.NO;
                            this.idtPaymentAccountSuspensionDate = lReturnToworkPayrollDetails.idtSuspensionDate;
                            this.icdoReturnToWorkRequest.payment_account_suspension_date = lReturnToworkPayrollDetails.idtSuspensionDate;
                        }
                    }
                }
            }
            base.BeforePersistChanges();
            LoadSoftErrors();
        }

        public void ValidateReturnToWorkRequest()
        {
            this.istrParticipantHoursInPayrollMonth = busConstant.ReturnToWorkRequest.NOT_MET;
            this.istrIsWithinPaymentThreshold = busConstant.ReturnToWorkRequest.NO;
            LoadPayeeAccount();
            CheckMemberIsRetiree();
            LoadSoftErrors();
            if (this.icdoReturnToWorkRequest.estimated_hours_per_payroll_month.IsNotNull() && this.icdoReturnToWorkRequest.estimated_hours_per_payroll_month >= 50)
            {
                this.istrParticipantHoursInPayrollMonth = busConstant.ReturnToWorkRequest.MET;
            }
            if (this.istrParticipantHoursInPayrollMonth == busConstant.ReturnToWorkRequest.MET && this.istrParticipantRetiree == busConstant.ReturnToWorkRequest.MET
                 && this.istrPayeeAccountActive == busConstant.ReturnToWorkRequest.MET && this.icdoReturnToWorkRequest.employer_name.IsNotNullOrEmpty()
                 && this.icdoReturnToWorkRequest.reemployment_start_date !=DateTime.MinValue && this.icdoReturnToWorkRequest.job_classfication.IsNotNullOrEmpty() 
                 && this.icdoReturnToWorkRequest.estimated_hours_per_payroll_month !=0)
            {
                this.icdoReturnToWorkRequest.status_value = busConstant.ReturnToWorkRequest.STATUS_VALID;
                this.icdoReturnToWorkRequest.eligible_flag = busConstant.ReturnToWorkRequest.YES;
                this.istrIsEligibleToRTW = busConstant.ReturnToWorkRequest.YES;
            }
            else
            {
                this.icdoReturnToWorkRequest.status_value = busConstant.ReturnToWorkRequest.STATUS_REVEIW;
                this.icdoReturnToWorkRequest.eligible_flag = busConstant.ReturnToWorkRequest.NO;
                this.istrIsEligibleToRTW = busConstant.ReturnToWorkRequest.NO;
            }
        }

        public void LoadActivityInstance(object obj)
        {
            this.ibusBaseActivityInstance = (busSolBpmActivityInstance)obj;
        }

        public void ValidateReturnToWorkRequestSoftErrors()
        {
            if (this.icdoReturnToWorkRequest.estimated_hours_per_payroll_month == busConstant.ZERO_INT)
            {
                DBFunction.DBNonQuery("entReturnToWorkRequest.InsertReturnToWorkRequestSoftErrors",
                       new object[4] { 7005, iobjPassInfo.istrUserID, icdoReturnToWorkRequest.person_id, icdoReturnToWorkRequest.reemployment_notification_id},
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }
            else
            {
                DBFunction.DBNonQuery("entReturnToWorkRequest.DeleteReturnToWorkRequest",
                       new object[2] { 7005, icdoReturnToWorkRequest.reemployment_notification_id },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }
            if (this.icdoReturnToWorkRequest.employer_name.IsNullOrEmpty())
            {
                DBFunction.DBNonQuery("entReturnToWorkRequest.InsertReturnToWorkRequestSoftErrors",
                       new object[4] { 7006, iobjPassInfo.istrUserID, icdoReturnToWorkRequest.person_id, icdoReturnToWorkRequest.reemployment_notification_id },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }
            else
            {
                DBFunction.DBNonQuery("entReturnToWorkRequest.DeleteReturnToWorkRequest",
                       new object[2] { 7006, icdoReturnToWorkRequest.reemployment_notification_id },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }
            if (this.icdoReturnToWorkRequest.reemployment_start_date == DateTime.MinValue)
            {
                DBFunction.DBNonQuery("entReturnToWorkRequest.InsertReturnToWorkRequestSoftErrors",
                       new object[4] { 7007, iobjPassInfo.istrUserID, icdoReturnToWorkRequest.person_id, icdoReturnToWorkRequest.reemployment_notification_id },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }
            else
            {
                DBFunction.DBNonQuery("entReturnToWorkRequest.DeleteReturnToWorkRequest",
                       new object[2] { 7007, icdoReturnToWorkRequest.reemployment_notification_id },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }
            if (this.icdoReturnToWorkRequest.job_classfication.IsNullOrEmpty())
            {
                DBFunction.DBNonQuery("entReturnToWorkRequest.InsertReturnToWorkRequestSoftErrors",
                       new object[4] { 7008, iobjPassInfo.istrUserID, icdoReturnToWorkRequest.person_id, icdoReturnToWorkRequest.reemployment_notification_id },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }
            else
            {
                DBFunction.DBNonQuery("entReturnToWorkRequest.DeleteReturnToWorkRequest",
                       new object[2] { 7008, icdoReturnToWorkRequest.reemployment_notification_id },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }

        }

        public void ValidateEndOfReturnToWorkRequestSoftErrors()
        {
            if (iclbPayeeAccount.Count == 0)
            {
                DBFunction.DBNonQuery("entReturnToWorkRequest.InsertReturnToWorkRequestSoftErrors",
                       new object[4] { 7017, iobjPassInfo.istrUserID, icdoReturnToWorkRequest.person_id, icdoReturnToWorkRequest.reemployment_notification_id },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }
            else
            {
                DBFunction.DBNonQuery("entReturnToWorkRequest.DeleteReturnToWorkRequest",
                       new object[2] { 7017, icdoReturnToWorkRequest.reemployment_notification_id },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }
            if (iclbPayeeAccount.Any(e => e.icdoPayeeAccount.istrStatus == "SPND"))
            {
                DBFunction.DBNonQuery("entReturnToWorkRequest.InsertReturnToWorkRequestSoftErrors",
                       new object[4] { 7016, iobjPassInfo.istrUserID, icdoReturnToWorkRequest.person_id, icdoReturnToWorkRequest.reemployment_notification_id },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }
            else
            {
                DBFunction.DBNonQuery("entReturnToWorkRequest.DeleteReturnToWorkRequest",
                       new object[2] { 7016, icdoReturnToWorkRequest.reemployment_notification_id },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }
        }

        public void ValidateReEmploymentNotification()
        {
            if (this.istrParticipantHoursInPayrollMonth == busConstant.ReturnToWorkRequest.MET && this.istrParticipantRetiree == busConstant.ReturnToWorkRequest.MET
                && this.istrPayeeAccountActive == busConstant.ReturnToWorkRequest.MET)
            {
                this.icdoReturnToWorkRequest.status_value = busConstant.STATUS_VALID;
                this.icdoReturnToWorkRequest.eligible_flag = busConstant.YES;
                this.istrParticipantHoursInPayrollMonth = busConstant.YES;
            }
        }
        public bool CheckMemberIsRetiree()
        {
            DataTable ldtbBenefitApplcation = busBase.Select("cdoBenefitApplication.MemberIsRetiree", new object[] { this.ibusPayee.icdoPerson.person_id });
            if (ldtbBenefitApplcation != null && ldtbBenefitApplcation.Rows.Count > 0)
            {
                int lintCount = Convert.ToInt32(ldtbBenefitApplcation.Rows[0]["COUNT"]);
                if (lintCount > 0)
                {
                    this.istrParticipantRetiree = busConstant.ReturnToWorkRequest.MET;
                }
            }
            return false;
        }

        public void LoadSoftErrors()
        {
            iclbReemploymentErrors = new Collection<busError>();
            DataTable ldtbError = Select("entReturnToWorkRequest.LoadSoftErrors", new object[1] { this.icdoReturnToWorkRequest.reemployment_notification_id });
            if (ldtbError.Rows.Count > 0)
            {

                foreach (DataRow ldtrError in ldtbError.Rows)
                {
                    busError lobjError = new busError();
                    sqlFunction.LoadQueryResult(lobjError, ldtrError, iobjPassInfo.iconFramework);
                    //lobjError.parameter_values = ldtrError[enmPersonAccountBeneficiary.person_account_beneficiary_id.ToString()].ToString();
                    string lstrParamValues = lobjError.parameter_values;

                    lobjError.display_message = FormatMessageParameters(lobjError.message_id, lobjError.display_message, lstrParamValues);
                    iclbReemploymentErrors.Add(lobjError);
                }
            }
        }

        public void GenerateCorrespondenceRETR0006(int aintReEmploymentNotificationId, int aintPersonId)
        {
            busReturnToWorkRequest lobjReturnToWork = new busReturnToWorkRequest { icdoReturnToWorkRequest = new cdoReturnToWorkRequest() };
            lobjReturnToWork.FindReturnToWorkRequest(aintReEmploymentNotificationId);
            DataTable ldtblist = busPerson.Select("entReturnToWorkRequest.LoadPayeeAccounts", new object[1] { lobjReturnToWork.icdoReturnToWorkRequest.person_id });
            Collection<busPayeeAccount> iclbPayeeAccount = GetCollection<busPayeeAccount>(ldtblist, "icdoPayeeAccount");
            busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
            lobjPayeeAccount.ibusPayee = new busPerson() { icdoPerson = new cdoPerson() };
            int payeeAccountID = 0;
            if (iclbPayeeAccount.Count() > 0)
            {
                payeeAccountID = iclbPayeeAccount.Where(p => p.icdoPayeeAccount.istrStatus == busConstant.ReturnToWorkRequest.STATUS_RECEIVING)
                                                        .Select(p => p.icdoPayeeAccount.payee_account_id).FirstOrDefault();
            }

            lobjPayeeAccount.FindPayeeAccount(payeeAccountID);
            lobjPayeeAccount.ibusPayee.FindPerson(lobjReturnToWork.icdoReturnToWorkRequest.person_id);
            lobjPayeeAccount.ibusPayee.LoadPersonAccounts();
            lobjPayeeAccount.ibusPayee.LoadCorrAddress();
            Hashtable ahtbQueryBkmarks = new Hashtable();
            ArrayList aarrResult = new ArrayList();
            aarrResult.Add(lobjPayeeAccount);
            utlCorresPondenceInfo lobjCorresPondenceInfo0006 = CreateCorrespondence(busConstant.RE_EMPLOYMENT_NOTIFICATION_FORM, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks);

            InsertRTWCorrMapping(aintReEmploymentNotificationId, lobjCorresPondenceInfo0006.iintCorrespondenceTrackingId);
        }

        public void GenerateCorrespondencePAYEE0001AndPAYEE0021(int aintReEmploymentNotificationId, int aintPersonId)
        {
            //DataTable ldtReemployedPayeeAccounts = busBase.Select("entPayeeAccount.ReEmpoymentParticipantInfo", new object[1] { aintPersonId });
            DataTable ldtReemployedPayeeAccounts = busPerson.Select("entReturnToWorkRequest.LoadPayeeAccounts", new object[1] { aintPersonId });
            Collection<busPayeeAccount> iclbPayeeAccount = GetCollection<busPayeeAccount>(ldtReemployedPayeeAccounts, "icdoPayeeAccount");
            Hashtable ahtbQueryBkmarks = new Hashtable();
            ArrayList aarrResult = null;

            if (iclbPayeeAccount.Count() > 0)
            {
                //foreach (busPayeeAccount payee in iclbPayeeAccount)
                //{
                busPayeeAccount objPayeeAccount = new busPayeeAccount();
                objPayeeAccount.FindPayeeAccount(iclbPayeeAccount[0].icdoPayeeAccount.payee_account_id);
                objPayeeAccount.ibusParticipant = new busPerson { icdoPerson = new cdoPerson() };
                objPayeeAccount.ibusParticipant.FindPerson(aintPersonId);
                objPayeeAccount.LoadNextBenefitPaymentDate();
                objPayeeAccount.ibusParticipant.LoadCorrAddress();
                aarrResult = new ArrayList();
                aarrResult.Add(objPayeeAccount);

                utlCorresPondenceInfo lobjCorresPondenceInfo0001 = CreateCorrespondence(busConstant.REEMPLOYMENT_NOTIFICATION_FORM, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks);
                InsertRTWCorrMapping(aintReEmploymentNotificationId, lobjCorresPondenceInfo0001.iintCorrespondenceTrackingId);
                utlCorresPondenceInfo lobjCorresPondenceInfo0021 = CreateCorrespondence(busConstant.STOP_WORKING_POST_RE_EMPLOYMENT_NOTIFICATION_FORM, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks);
                InsertRTWCorrMapping(aintReEmploymentNotificationId, lobjCorresPondenceInfo0021.iintCorrespondenceTrackingId);
                //}
            }
        }

        private void InsertRTWCorrMapping(int aintReEmploymentNotificationId, int aintCorrespondenceTrackingId)
        {
            busRTWCorrMapping lobjRTWCorrMapping = new busRTWCorrMapping { icdoRTWCorrMapping = new doRTWCorrMapping() };
            lobjRTWCorrMapping.icdoRTWCorrMapping.reemployment_notification_id = aintReEmploymentNotificationId;
            lobjRTWCorrMapping.icdoRTWCorrMapping.corr_tracking_id = aintCorrespondenceTrackingId;
            lobjRTWCorrMapping.icdoRTWCorrMapping.created_by = this.iobjPassInfo.istrUserID;
            lobjRTWCorrMapping.icdoRTWCorrMapping.created_date = DateTime.Today;
            lobjRTWCorrMapping.icdoRTWCorrMapping.modified_by = this.iobjPassInfo.istrUserID;
            lobjRTWCorrMapping.icdoRTWCorrMapping.modified_date = DateTime.Today;
            lobjRTWCorrMapping.icdoRTWCorrMapping.ienuObjectState = ObjectState.Insert;
            lobjRTWCorrMapping.icdoRTWCorrMapping.Insert();
        }

        public void UpdatePaymentAccountToSuspended(int aintReEmploymentNotificationId, int aintPersonId)
        {          
            DataTable ldtblist = busPerson.Select("entReturnToWorkRequest.LoadPayeeAccounts", new object[1] { aintPersonId });
            Collection<busPayeeAccount> iclbPayeeAccount = GetCollection<busPayeeAccount>(ldtblist, "icdoPayeeAccount");
    
            if (iclbPayeeAccount.Count() > 0)
            {
                var PayeeAccountfilteredList = iclbPayeeAccount.Where(p => p.icdoPayeeAccount.istrStatus == busConstant.PAYEE_ACCOUNT_STATUS_RECEIVING ).ToList();

                foreach (var item in PayeeAccountfilteredList)
                {
                    busPayeeAccount lobjPayeeAccount = new busPayeeAccount() { icdoPayeeAccount = new cdoPayeeAccount() };
                    lobjPayeeAccount.FindPayeeAccount(item.icdoPayeeAccount.payee_account_id);
                    lobjPayeeAccount.ibusParticipant = new busPerson { icdoPerson = new cdoPerson() };
                    lobjPayeeAccount.ibusParticipant.FindPerson(lobjPayeeAccount.icdoPayeeAccount.person_id);
                    busPayeeAccountStatus lbusPayeeAccountStatus = new busPayeeAccountStatus { icdoPayeeAccountStatus = new cdoPayeeAccountStatus() };
                    lbusPayeeAccountStatus.InsertValuesInPayeeAccountStatus(item.icdoPayeeAccount.payee_account_id, busConstant.PAYEE_ACCOUNT_STATUS_SUSPENDED, DateTime.Now, busConstant.ReturnToWorkRequest.PAYEE_ACCOUNT_SUSPENSION_REASON_RETURN_TO_WORK_BPM);
                }
            }
            busNotes lobjNotes = new busNotes { icdoNotes = new cdoNotes() };
            lobjNotes.icdoNotes.person_id = aintPersonId;
            lobjNotes.icdoNotes.form_id = busConstant.Form_ID;
            lobjNotes.icdoNotes.form_value = busConstant.ReturnToWorkRequest.RETURN_TO_WORK_REQUEST_FORM;
            lobjNotes.icdoNotes.notes = "Benefit suspended.";
            lobjNotes.icdoNotes.reference_id = this.icdoReturnToWorkRequest.reemployment_notification_id;
            lobjNotes.icdoNotes.created_by = this.iobjPassInfo.istrUserID;
            lobjNotes.icdoNotes.created_date = DateTime.Now;
            lobjNotes.icdoNotes.modified_date = DateTime.Now;
            lobjNotes.icdoNotes.modified_by = this.iobjPassInfo.istrUserID;
            lobjNotes.icdoNotes.ienuObjectState = ObjectState.Insert;
            lobjNotes.icdoNotes.Insert();
        }
        public void UpdateNotificationStatusToProcessed(int aintReEmploymentNotificationId, int aintPersonId)
        {
            busReturnToWorkRequest lobjReturnToWork = new busReturnToWorkRequest { icdoReturnToWorkRequest = new cdoReturnToWorkRequest() };
            lobjReturnToWork.FindReturnToWorkRequest(aintReEmploymentNotificationId);
            lobjReturnToWork.icdoReturnToWorkRequest.Select();
            lobjReturnToWork.icdoReturnToWorkRequest.status_value = busConstant.ReturnToWorkRequest.STATUS_PROCESSED;
            lobjReturnToWork.icdoReturnToWorkRequest.Update();
            busNotes lobjNotes = new busNotes { icdoNotes = new cdoNotes() };
            lobjNotes.icdoNotes.person_id = aintPersonId;
            lobjNotes.icdoNotes.form_id = busConstant.Form_ID;
            lobjNotes.icdoNotes.form_value = busConstant.ReturnToWorkRequest.RETURN_TO_WORK_REQUEST_FORM;
            lobjNotes.icdoNotes.notes = "End of re-employment and confirmation letter sent.";
            lobjNotes.icdoNotes.reference_id = this.icdoReturnToWorkRequest.reemployment_notification_id;
            lobjNotes.icdoNotes.created_by = this.iobjPassInfo.istrUserID;
            lobjNotes.icdoNotes.created_date = DateTime.Now;
            lobjNotes.icdoNotes.modified_date = DateTime.Now;
            lobjNotes.icdoNotes.modified_by = this.iobjPassInfo.istrUserID;
            lobjNotes.icdoNotes.ienuObjectState = ObjectState.Insert;
            lobjNotes.icdoNotes.Insert();

        }
        public void CancelReEmploymentNotification(int aintReEmploymentNotificationId, int aintPersonId, string astrCancelReason)
        {
            busReturnToWorkRequest lobjReturnToWork = new busReturnToWorkRequest { icdoReturnToWorkRequest = new cdoReturnToWorkRequest() };
            lobjReturnToWork.FindReturnToWorkRequest(aintReEmploymentNotificationId);
            lobjReturnToWork.icdoReturnToWorkRequest.Select();
            lobjReturnToWork.icdoReturnToWorkRequest.status_value = busConstant.ReturnToWorkRequest.STATUS_CANCEL;
            lobjReturnToWork.icdoReturnToWorkRequest.Update();
            busNotes lobjNotes = new busNotes { icdoNotes = new cdoNotes() };
            lobjNotes.icdoNotes.person_id = aintPersonId;
            lobjNotes.icdoNotes.form_id = busConstant.Form_ID;
            lobjNotes.icdoNotes.form_value = busConstant.ReturnToWorkRequest.RETURN_TO_WORK_REQUEST_FORM;
            lobjNotes.icdoNotes.notes = astrCancelReason;
            lobjNotes.icdoNotes.reference_id = this.icdoReturnToWorkRequest.reemployment_notification_id;
            lobjNotes.icdoNotes.created_by = this.iobjPassInfo.istrUserID;
            lobjNotes.icdoNotes.created_date = DateTime.Now;
            lobjNotes.icdoNotes.modified_date = DateTime.Now;
            lobjNotes.icdoNotes.modified_by = this.iobjPassInfo.istrUserID;
            lobjNotes.icdoNotes.ienuObjectState = ObjectState.Insert;
            lobjNotes.icdoNotes.Insert();

        }
        public utlCorresPondenceInfo CreateCorrespondence(string astrTemplateName, string astrUserID, int aintUserSerialID, ArrayList aarrResult, Hashtable ahtbQueryBkmarks, bool ablnIsPDF = false, string astrActiveAddr = busConstant.FLAG_YES)
        {
            utlCorresPondenceInfo lobjCorresPondenceInfo = busMPIPHPBase.SetCorrespondence(astrTemplateName,
                           iobjPassInfo.istrUserID, iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks);
            string lstrFileName = string.Empty;
            if (lobjCorresPondenceInfo == null)
            {
                throw new Exception("Unable to create correspondence, SetCorrespondence method not found in business solutions base object");
            }
            lobjCorresPondenceInfo.istrAutoPrintFlag = "N";
            string lstrLastName = string.Empty;
            string lstrMPID = string.Empty;
            CorBuilderXML iobjCorBuilder = null;
            string istrAddressType = string.Empty;
            foreach (utlBookmarkFieldInfo obj in lobjCorresPondenceInfo.icolBookmarkFieldInfo)
            {
                if (obj.istrDataType == "String" && !(string.IsNullOrEmpty(obj.istrValue)))
                {
                    if (obj.istrObjectField == "istrAddrLine1" || obj.istrObjectField == "istrAddrLine2" || obj.istrObjectField == "istrAddrLine3" || obj.istrObjectField == "istrCountryDescription" ||
                        obj.istrObjectField == "istrState" || obj.istrObjectField == "istrCity" || obj.istrObjectField == "istrZipCode" || obj.istrObjectField == "istrRecepientName" ||
                        obj.istrObjectField == "ibusPayee.icdoPerson.istrFullName" || obj.istrObjectField == "ibusPayeeAccount.istrBeneficiaryFullName" ||
                        obj.istrObjectField == "icdoPayeeAccount.istrParticipantName" || obj.istrObjectField == "ibusPayee.ibusPersonCourtContact.icdoPersonContact.contact_name" ||
                        obj.istrObjectField == "ibusAlternatePayee.ibusPersonAddressForCorr.icdoPersonAddress.addr_line_1" ||
                        obj.istrObjectField == "ibusAlternatePayee.ibusPersonAddressForCorr.icdoPersonAddress.addr_line_2" ||
                        obj.istrObjectField == "ibusAlternatePayee.ibusPersonAddressForCorr.icdoPersonAddress.addr_city" ||
                        obj.istrObjectField == "ibusAlternatePayee.ibusPersonAddressForCorr.icdoPersonAddress.addr_state_value" ||
                        obj.istrObjectField == "ibusAlternatePayee.ibusPersonAddressForCorr.icdoPersonAddress.istrCompleteZipCode" ||
                        obj.istrObjectField == "ibusAlternatePayee.ibusPersonAddressForCorr.icdoPersonAddress.foreign_postal_code" ||
                        obj.istrObjectField == "ibusAlternatePayee.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_description" ||
                        obj.istrObjectField == "ibusPayee.ibusPersonCourtContact.icdoPersonContact.addr_line_1" ||
                        obj.istrObjectField == "ibusPayee.ibusPersonCourtContact.icdoPersonContact.addr_line_2" ||
                        obj.istrObjectField == "ibusPayee.ibusPersonCourtContact.icdoPersonContact.addr_city" ||
                        obj.istrObjectField == "ibusPayee.ibusPersonCourtContact.icdoPersonContact.addr_state_value" ||
                        obj.istrObjectField == "ibusPayee.ibusPersonCourtContact.icdoPersonContact.istrCompleteZipCode" ||
                        obj.istrObjectField == "ibusPayee.ibusPersonCourtContact.icdoPersonContact.foreign_postal_code" ||
                        obj.istrObjectField == "ibusPayee.ibusPersonCourtContact.icdoPersonContact.addr_country_description" ||
                        obj.istrObjectField == "istrReportedBy" ||
                        obj.istrObjectField == "istrEmployerName" ||
                        obj.istrObjectField == "istrAddress1" ||
                        obj.istrObjectField == "istrCity1" ||
                        obj.istrObjectField == "istrState" ||
                        obj.istrObjectField == "istrPostalCode" ||
                        obj.istrObjectField == "istrStreet" ||
                        obj.istrObjectField == "istrAddress2" || obj.istrObjectField == "istrApprovedByUserInitials" ||
                        obj.istrObjectField == "ibusPayee.ibusPersonCourtContact.icdoPersonContact.county" ||
                        obj.istrObjectField == "icdoDroApplication.case_number" || obj.istrObjectField == "istrPayeeFullName"
                        )
                    {
                        obj.istrValue = obj.istrValue.ToUpper();
                    }
                    else
                        obj.istrValue = obj.istrValue.ToProperCase();
                }
                if (obj.istrName == "stdMbrLastName")
                {
                    lstrLastName = string.IsNullOrEmpty(obj.istrValue) ? string.Empty : obj.istrValue;
                }
                if (obj.istrName == "stdMbrParticipantMPID")
                {
                    lstrMPID = string.IsNullOrEmpty(obj.istrValue) ? string.Empty : obj.istrValue;
                }
            }

            if (astrTemplateName == busConstant.RE_EMPLOYMENT_GENERAL_INFORMATION)
            {
                utlBookmarkFieldInfo lobjField = new utlBookmarkFieldInfo();
                lobjField.istrName = "stdEndOfReemploymentDate";
                lobjField.istrValue = this.icdoReturnToWorkRequest.reemployment_start_date.ToShortDateString();
                lobjCorresPondenceInfo.icolBookmarkFieldInfo.Add(lobjField);
            }

            try
            {
                iobjCorBuilder = new CorBuilderXML();
                iobjCorBuilder.InstantiateWord();
                lstrFileName = iobjCorBuilder.CreateCorrespondenceFromTemplate(astrTemplateName,
                    lobjCorresPondenceInfo, astrUserID);

                if (ablnIsPDF)
                {
                    busSystemManagement iobjSystemManagement = new busSystemManagement();
                    iobjSystemManagement.FindSystemManagement();

                    string lstrFilepath = iobjSystemManagement.icdoSystemManagement.base_directory + "Correspondence\\Generated\\" + lstrFileName;
                    busGlobalFunctions.RenderWordAsPDF(lstrFilepath, astrActiveAddr);
                }

                iobjCorBuilder.CloseWord();
            }
            catch (Exception e)
            {
                if (iobjCorBuilder != null)
                {
                    iobjCorBuilder.CloseWord();
                }
            }

            return lobjCorresPondenceInfo;
        }
      
        public ArrayList BtnSubmitClick()
        {
            iarrErrors = new ArrayList();
            IsEffectiveDateEntered();
            CheckSuspendedAccountAndUpdateStatus();
            UpdateRTWparameter();
            UpdateERTWparameter();
            return iarrErrors;
        }

        public ArrayList BtnCancelClick()
        {
            ArrayList larrResult = new ArrayList();
            if (ibusBaseActivityInstance != null)
            {
                busSolBpmActivityInstance lbusBpmActivityInstance = ibusBaseActivityInstance as busSolBpmActivityInstance;
                if (lbusBpmActivityInstance != null)
                {
                    lbusBpmActivityInstance.istrTerminationReason = busConstant.ReturnToWorkRequest.TERMINATION_REASON;
                    larrResult = lbusBpmActivityInstance.InvokeWorkflowAction();
                    CancelReEmploymentNotification(icdoReturnToWorkRequest.reemployment_notification_id, icdoReturnToWorkRequest.person_id, lbusBpmActivityInstance.istrTerminationReason);
                }
            }
            return larrResult;
        }
        public  Collection<busNotes> LoadNotesForRTW(int aintPersonID,  string astrFormValue, int aintReEmploymentNotificationId)
        {
            Collection<busNotes> iclbNotes = new Collection<busNotes>();
            DataTable ldtbNotes = null;

            if (aintPersonID != 0)
            {
                //Query to load Person Notes
                ldtbNotes = busMPIPHPBase.Select("entNotes.FindNotesForReturnToWorkRequest", new object[3] { aintPersonID, astrFormValue, aintReEmploymentNotificationId });
                if (ldtbNotes.Rows.Count > 0)
                {
                    busMPIPHPBase lbusMPIPHPBase = new busMPIPHPBase();
                    iclbNotes = lbusMPIPHPBase.GetCollection<busNotes>(ldtbNotes, "icdoNotes");
                    if (iclbNotes != null)
                        iclbNotes = iclbNotes.OrderByDescending(obj => obj.icdoNotes.created_date).ToList().ToCollection<busNotes>();
                }
            }
            return iclbNotes;
        }
        public void LoadDefaultProperties()
        {
            this.istrIsEligibleToRTW = busConstant.ReturnToWorkRequest.NO;
            this.istrIsWithinPaymentThreshold = busConstant.ReturnToWorkRequest.NO;
            this.idtPaymentAccountSuspensionDate = DateTime.MaxValue;
            this.istrIsSecondAuditRequired = busConstant.ReturnToWorkRequest.NO;
            this.istrIsAllPayeeAccountApproved = busConstant.ReturnToWorkRequest.NO;
            this.istrPayeeAccountActive = busConstant.ReturnToWorkRequest.NOT_MET;
            this.istrParticipantRetiree = busConstant.ReturnToWorkRequest.NOT_MET;
            this.istrParticipantHoursInPayrollMonth = busConstant.ReturnToWorkRequest.NOT_MET;
        }
        
        public void LoadReturnToWorkHistory()
        {
            iclbReturnToWorkHistory = new Collection<busReturnToWorkRequest>();
            DataTable ldtblist = busReturnToWorkRequest.Select("entReturnToWorkRequest.LoadReturnToWorkHistory", new object[2] { this.icdoReturnToWorkRequest.person_id, this.icdoReturnToWorkRequest.reemployment_notification_id });
            iclbReturnToWorkHistory = GetCollection<busReturnToWorkRequest>(ldtblist, "icdoReturnToWorkRequest");
        }

        public void AddNotesForERTW(int aintReEmploymentNotificationId, int aintPersonId)
        {
            busNotes lobjNotes = new busNotes { icdoNotes = new cdoNotes() };
            lobjNotes.icdoNotes.person_id = aintPersonId;
            lobjNotes.icdoNotes.form_id = busConstant.Form_ID;
            lobjNotes.icdoNotes.form_value = busConstant.ReturnToWorkRequest.RETURN_TO_WORK_REQUEST_FORM;
            lobjNotes.icdoNotes.notes = "End Of Re-Employment Acknowledgement Form generated.";
            lobjNotes.icdoNotes.reference_id = aintReEmploymentNotificationId;
            lobjNotes.icdoNotes.created_by = this.iobjPassInfo.istrUserID;
            lobjNotes.icdoNotes.created_date = DateTime.Now;
            lobjNotes.icdoNotes.modified_by = this.iobjPassInfo.istrUserID;
            lobjNotes.icdoNotes.modified_date = DateTime.Now;
            lobjNotes.icdoNotes.ienuObjectState = ObjectState.Insert;
            lobjNotes.icdoNotes.Insert();
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            if (ibusBaseActivityInstance != null)
            {
                busBpmActivityInstance lbusBpmActivityInstance = ibusBaseActivityInstance as busBpmActivityInstance;
                if (lbusBpmActivityInstance?.ibusBpmActivity?.icdoBpmActivity?.name == busConstant.ReturnToWorkRequest.ACTIVITY_ERTW_GENERATE_CORR_AND_RESUME_PA)
                {
                    DeleteOldCorrAndRpt();
                    GenerateCorrespondencePER0012();
                    GenerateReportMonthOfSuspendibleService();
                }
            }
        }

        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            base.ValidateHardErrors(aenmPageMode);
            IsEffectiveDateEntered();
        }

        public void DeleteOldCorrAndRpt()
        {
            DataTable ldtbList = Select<doRTWCorrMapping>(
                new String[1] { enmRTWCorrMapping.reemployment_notification_id.ToString() },
                new object[1] { this.icdoReturnToWorkRequest.reemployment_notification_id }, null, null);
            Collection<busRTWCorrMapping> lclbRTWCorrMappings = GetCollection<busRTWCorrMapping>(ldtbList, "icdoRTWCorrMapping");

            if (lclbRTWCorrMappings.IsNotNull() && lclbRTWCorrMappings.Count > 0)
            {
                foreach (busRTWCorrMapping lbusRTWCorrMapping in lclbRTWCorrMappings)
                {
                    string lstrCorrFilePath = GetCorrFilePathFromTrackingId(lbusRTWCorrMapping.icdoRTWCorrMapping.corr_tracking_id);
                    if (File.Exists(lstrCorrFilePath))
                        File.Delete(lstrCorrFilePath);

                    string lstrRptFilePath = GetSuspendibleServiceReportPath(lbusRTWCorrMapping.icdoRTWCorrMapping.corr_tracking_id);
                    if (File.Exists(lstrRptFilePath))
                        File.Delete(lstrRptFilePath);

                    lbusRTWCorrMapping.icdoRTWCorrMapping.Delete();
                }
            }
        }

        private void GenerateCorrespondencePER0012()
        {
            busPerson lbusPerson = new busPerson() { icdoPerson = new cdoPerson() };
            lbusPerson.FindPerson(this.icdoReturnToWorkRequest.person_id);
            lbusPerson.LoadCorrAddress();

            Hashtable ahtbQueryBkmarks = new Hashtable();
            ArrayList aarrResult = new ArrayList();
            aarrResult.Add(lbusPerson);

            utlCorresPondenceInfo lobjCorrespondencePER0012 = CreateCorrespondence(busConstant.RE_EMPLOYMENT_GENERAL_INFORMATION, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks);
            InsertRTWCorrMapping(this.icdoReturnToWorkRequest.reemployment_notification_id, lobjCorrespondencePER0012.iintCorrespondenceTrackingId);
        }

        private string GetCorrFilePathFromTrackingId(int aintTrackingId)
        {
            string lstrFolderpath = iobjPassInfo.isrvDBCache.GetPathInfo("CorrGenr");
            string lstrTrackingID = aintTrackingId.ToString().PadLeft(10, '0');
            string lstrFileName = busConstant.RE_EMPLOYMENT_GENERAL_INFORMATION + "-" + lstrTrackingID + ".docx";
            string lstrFilePath = Path.Combine(lstrFolderpath, lstrFileName);
            return lstrFilePath;
        }

        private void GenerateReportMonthOfSuspendibleService()
        {
            byte[] lbyteFile = GenerateReport();
            int lintCorrespondenceTrackingId = InsertCorrTracking();

            string lstrFilePath = GetSuspendibleServiceReportPath(lintCorrespondenceTrackingId);
            File.WriteAllBytes(lstrFilePath, lbyteFile);

            InsertRTWCorrMapping(this.icdoReturnToWorkRequest.reemployment_notification_id, lintCorrespondenceTrackingId);
        }

        private byte[] GenerateReport()
        {
            busPerson lbusPerson = new busPerson() { icdoPerson = new cdoPerson() };
            lbusPerson.FindPerson(this.icdoReturnToWorkRequest.person_id);

            byte[] lbyteFile = null;

            DateTime ldtEffectiveDate = new DateTime();
            DataSet ldsResult = new DataSet();
            DataTable ldResult = new DataTable();

            DataTable ldtblRetirementDate = busBase.Select("cdoPerson.GetRetirementDate", new object[1] { lbusPerson.icdoPerson.person_id });
            if (ldtblRetirementDate != null && ldtblRetirementDate.Rows.Count > 0 &&
                Convert.ToString(ldtblRetirementDate.Rows[0][enmBenefitApplication.retirement_date.ToString().ToUpper()]).IsNotNullOrEmpty())
                ldtEffectiveDate = Convert.ToDateTime(ldtblRetirementDate.Rows[0][enmBenefitApplication.retirement_date.ToString().ToUpper()]);

            if (ldtEffectiveDate != DateTime.MinValue && ldtEffectiveDate > lbusPerson.icdoPerson.date_of_birth.AddYears(65))
                ldtEffectiveDate = lbusPerson.icdoPerson.date_of_birth.AddYears(65);

            if (ldtEffectiveDate != DateTime.MinValue)
                ldResult = busBase.Select("cdoPerson.MonthOfSuspendibleServiceReport", new object[2] { ldtEffectiveDate.Year, lbusPerson.icdoPerson.ssn });

            ldResult.TableName = "ReportTable01";
            ldsResult.Tables.Add(ldResult.Copy());

            if (ldsResult != null)
            {
                busCreateReports lbusCreateReports = new busCreateReports();
                lbyteFile = lbusCreateReports.CreateDynamicReport(ldsResult, "rpt_MonthOfSuspendibleServiceReport");
            }
            return lbyteFile;
        }

        private int InsertCorrTracking()
        {
            cdoCorTemplates lobjCorTemplate = new cdoCorTemplates();
            lobjCorTemplate.LoadByTemplateName(busConstant.ReturnToWorkRequest.MONTH_OF_SUSPENDIBLE_SERVICE_REPORT);

            cdoCorTracking lcdoCorTracking = new cdoCorTracking();
            lcdoCorTracking.template_id = lobjCorTemplate.template_id;
            lcdoCorTracking.cor_status_value = "GENR";
            lcdoCorTracking.generated_date = DateTime.Now;
            lcdoCorTracking.created_by = this.iobjPassInfo.istrUserID;
            lcdoCorTracking.modified_by = this.iobjPassInfo.istrUserID;
            lcdoCorTracking.comments = "";
            lcdoCorTracking.person_id = this.icdoReturnToWorkRequest.person_id;

            lcdoCorTracking.Insert();
            return lcdoCorTracking.tracking_id;
        }

        private string GetSuspendibleServiceReportPath(int aintTrackingId)
        {
            string lstrFolderpath = iobjPassInfo.isrvDBCache.GetPathInfo("BatchRptGN");
            string lstrTrackingID = aintTrackingId.ToString().PadLeft(10, '0');
            string lstrFileName = busConstant.ReturnToWorkRequest.MONTH_OF_SUSPENDIBLE_SERVICE_REPORT + "-" + lstrTrackingID + ".pdf";
            string lstrFilePath = Path.Combine(lstrFolderpath, lstrFileName);
            return lstrFilePath;
        }

        public byte[] OpenMonthOfSuspendibleServiceReport(int aintTrackingId)
        {
            string lstrFilePath = GetSuspendibleServiceReportPath(aintTrackingId);
            return File.ReadAllBytes(lstrFilePath);
        }

        private void IsEffectiveDateEntered()
        {
            if (icdoReturnToWorkRequest.request_type_value == busConstant.ReturnToWorkRequest.REQUEST_TYPE_ERTW
                && icdoReturnToWorkRequest.reemployment_start_date == DateTime.MinValue)
            {
                utlError utlError = AddError(7011, string.Empty);
                iarrErrors.Add(utlError);
            }
        }

        private void CheckSuspendedAccountAndUpdateStatus()
        {
            if (ibusBaseActivityInstance != null)
            {
                busBpmActivityInstance lbusBpmActivityInstance = ibusBaseActivityInstance as busBpmActivityInstance;
                if (icdoReturnToWorkRequest.status_value == busConstant.ReturnToWorkRequest.STATUS_REVEIW
                    && lbusBpmActivityInstance?.ibusBpmActivity?.icdoBpmActivity?.name == busConstant.ReturnToWorkRequest.ACTIVITY_ERTW_GENERATE_CORR_AND_RESUME_PA)
                {
                    if (iclbPayeeAccount.Any(e => e.icdoPayeeAccount.istrStatus == "SPND"))
                    {
                        utlError utlError = AddError(7012, string.Empty);
                        iarrErrors.Add(utlError);
                    }
                    else
                    {
                        icdoReturnToWorkRequest.status_value = busConstant.ReturnToWorkRequest.STATUS_VALID;
                        icdoReturnToWorkRequest.Update();
                    }
                }
                if (icdoReturnToWorkRequest.status_value == busConstant.ReturnToWorkRequest.STATUS_VALID
                    && lbusBpmActivityInstance?.ibusBpmActivity?.icdoBpmActivity?.name == busConstant.ReturnToWorkRequest.ACTIVITY_MAIL_CONFIRMATION_LETTER)
                {
                    icdoReturnToWorkRequest.status_value = busConstant.ReturnToWorkRequest.STATUS_PROCESSED;
                    icdoReturnToWorkRequest.Update();
                }
            }
        }

        private void UpdateERTWparameter()
        {
            if (ibusBaseActivityInstance != null && this.icdoReturnToWorkRequest.request_type_value == busConstant.ReturnToWorkRequest.REQUEST_TYPE_ERTW)
            {
                busBpmActivityInstance lbusBpmActivityInstance = ibusBaseActivityInstance as busBpmActivityInstance;

                //ACTIVITY_ERTW_GENERATE_CORR_AND_RESUME_PA
                if (lbusBpmActivityInstance?.ibusBpmActivity?.icdoBpmActivity?.name == busConstant.ReturnToWorkRequest.ACTIVITY_ERTW_GENERATE_CORR_AND_RESUME_PA)
                {
                    if (this.icdoReturnToWorkRequest.status_value == busConstant.ReturnToWorkRequest.STATUS_VALID)
                    {
                        busUser lbusUser = new busUser();
                        if (lbusUser.FindUser(lbusBpmActivityInstance.icdoBpmActivityInstance.checked_out_user))
                        {
                            lbusBpmActivityInstance.UpdateParameterValue("FirstActivityUser", lbusUser.icdoUser.user_serial_id);
                        }
                    }
                }
                //ACTIVITY_ERTW_CONDUCT_FIRST_AUDIT
                else if (lbusBpmActivityInstance?.ibusBpmActivity?.icdoBpmActivity?.name == busConstant.ReturnToWorkRequest.ACTIVITY_ERTW_CONDUCT_FIRST_AUDIT)
                {
                    busUser lbusUser = new busUser();
                    if (lbusUser.FindUser(lbusBpmActivityInstance.icdoBpmActivityInstance.checked_out_user))
                    {
                        lbusBpmActivityInstance.UpdateParameterValue("FirstAuditUser", lbusUser.icdoUser.user_serial_id);
                    }

                    bool lblnIsSecondAuditRequired = iclbPayeeAccount.Any(e => e.icdoPayeeAccount.verified_flag == busConstant.FLAG_YES);
                    if (lblnIsSecondAuditRequired)
                    {
                        lbusBpmActivityInstance.UpdateParameter("IsSecondAuditRequired", busConstant.ReturnToWorkRequest.YES);
                    }
                    else
                    {
                        lbusBpmActivityInstance.UpdateParameter("IsSecondAuditRequired", busConstant.ReturnToWorkRequest.NO);
                    }
                }

                //ACTIVITY_ERTW_CONDUCT_SECOND_AUDIT
                if (lbusBpmActivityInstance?.ibusBpmActivity?.icdoBpmActivity?.name == busConstant.ReturnToWorkRequest.ACTIVITY_ERTW_CONDUCT_FIRST_AUDIT
                    || lbusBpmActivityInstance?.ibusBpmActivity?.icdoBpmActivity?.name == busConstant.ReturnToWorkRequest.ACTIVITY_ERTW_CONDUCT_SECOND_AUDIT)
                {
                    //The System Activity "All Payee Accounts Approved?" will check if all Payee Accounts (except those are in completed, cancelled, Receiving status) are in Approved status and not in Review or Suspended status
                    List<busPayeeAccount> lstFilteredPayeeAccount = iclbPayeeAccount.Where(e => e.icdoPayeeAccount.istrStatus.NotIn(new string[] { "CNCL", "CMPL", "RECV" })).ToList();
                    bool lblnIsAllApproved = lstFilteredPayeeAccount.All(e => e.icdoPayeeAccount.istrStatus == "APRD");
                    bool lblnIsAnyRevwOrSusp = lstFilteredPayeeAccount.Any(e => e.icdoPayeeAccount.istrStatus.In(new string[] { "REVW", "SPND" }));

                    if (lblnIsAllApproved && !lblnIsAnyRevwOrSusp)
                    {
                        lbusBpmActivityInstance.UpdateParameter("IsAllPayeeAccountApproved", busConstant.ReturnToWorkRequest.YES);
                    }
                    else
                    {
                        lbusBpmActivityInstance.UpdateParameter("IsAllPayeeAccountApproved", busConstant.ReturnToWorkRequest.NO);
                    }
                }
            }
        }

        private void UpdateRTWparameter()
        {
            if (ibusBaseActivityInstance != null && this.icdoReturnToWorkRequest.request_type_value == busConstant.ReturnToWorkRequest.REQUEST_TYPE_RTW)
            {
                busBpmActivityInstance lbusBpmActivityInstance = ibusBaseActivityInstance as busBpmActivityInstance;
                busUser lbusUser = new busUser();
                if (lbusUser.FindUser(lbusBpmActivityInstance.icdoBpmActivityInstance.checked_out_user))
                {
                    lbusBpmActivityInstance.UpdateParameterValue("PreviousCheckoutUser", lbusUser.icdoUser.user_serial_id);
                }
            }
        }
        public void AddAuditingNotes()
        {
            if (this.icdoReturnToWorkRequest.request_type_value == busConstant.ReturnToWorkRequest.REQUEST_TYPE_ERTW)
            {
                this.iclbErtwAuditingNotes = new Collection<busErtwAuditingNotes>();

                this.ibusErtwSharedNotes = new busErtwSharedNotes() { icdoErtwSharedNotes = new doErtwSharedNotes() };
                this.ibusErtwSharedNotes.icdoErtwSharedNotes.reemployment_notification_id = this.icdoReturnToWorkRequest.reemployment_notification_id;
                this.ibusErtwSharedNotes.icdoErtwSharedNotes.created_by = this.iobjPassInfo.istrUserID;
                this.ibusErtwSharedNotes.icdoErtwSharedNotes.created_date = DateTime.Now;
                this.ibusErtwSharedNotes.icdoErtwSharedNotes.modified_by = this.iobjPassInfo.istrUserID;
                this.ibusErtwSharedNotes.icdoErtwSharedNotes.modified_date = DateTime.Now;
                this.ibusErtwSharedNotes.icdoErtwSharedNotes.ienuObjectState = ObjectState.Insert;
                this.ibusErtwSharedNotes.icdoErtwSharedNotes.Insert();
                //this.ibusErtwSharedNotes = lobjSharedNotes;

                for (int i = 0; i < 5; i++)
                {
                    busErtwAuditingNotes lobjAuditingNotes = new busErtwAuditingNotes() { icdoErtwAuditingNotes = new doErtwAuditingNotes() };
                    lobjAuditingNotes.icdoErtwAuditingNotes.reemployment_notification_id = this.icdoReturnToWorkRequest.reemployment_notification_id;
                    lobjAuditingNotes.icdoErtwAuditingNotes.created_by = this.iobjPassInfo.istrUserID;
                    lobjAuditingNotes.icdoErtwAuditingNotes.created_date = DateTime.Now;
                    lobjAuditingNotes.icdoErtwAuditingNotes.modified_by = this.iobjPassInfo.istrUserID;
                    lobjAuditingNotes.icdoErtwAuditingNotes.modified_date = DateTime.Now;
                    lobjAuditingNotes.icdoErtwAuditingNotes.ienuObjectState = ObjectState.Insert;
                    lobjAuditingNotes.icdoErtwAuditingNotes.Insert();
                    this.iclbErtwAuditingNotes.Add(lobjAuditingNotes);
                }
            }
        }

        public void LoadAuditingNotes()
        {
            if (this.icdoReturnToWorkRequest.request_type_value == busConstant.ReturnToWorkRequest.REQUEST_TYPE_ERTW)
            {
                ibusErtwSharedNotes = new busErtwSharedNotes();
                ibusErtwSharedNotes.FindByCustomKeys(
                    new string[1] { enmErtwAuditingNotes.reemployment_notification_id.ToString() },
                    new object[1] { this.icdoReturnToWorkRequest.reemployment_notification_id });

                DataTable ldtbList = Select<doErtwAuditingNotes>(
                new String[1] { enmErtwAuditingNotes.reemployment_notification_id.ToString() },
                new object[1] { this.icdoReturnToWorkRequest.reemployment_notification_id }, null, null);
                iclbErtwAuditingNotes = GetCollection<busErtwAuditingNotes>(ldtbList, "icdoErtwAuditingNotes");
            }
        }
        public busReturnToWorkRequest NewReturnToWorkRequest(string aintMPIPersonId, string aintRequestTypeValue, string aintSourceValue)
        {
            this.LoadDefaultProperties();
            this.ibusPayee = new busPerson() { icdoPerson = new cdoPerson() };
            DataTable ldtbPersonID = busBase.Select("cdoPerson.GetPersonDetails", new object[1] { aintMPIPersonId });
            if (ldtbPersonID.Rows.Count > 0)
            {
                this.ibusPayee.icdoPerson.LoadData(ldtbPersonID.Rows[0]);
                this.icdoReturnToWorkRequest.person_id = this.ibusPayee.icdoPerson.person_id;
            }
            if (aintSourceValue == busConstant.ReturnToWorkRequest.SOURCE_BATCH)
            {
                this.icdoReturnToWorkRequest.status_value = busConstant.ReturnToWorkRequest.STATUS_VALID;
            }
            else
            {
                this.icdoReturnToWorkRequest.status_value = busConstant.ReturnToWorkRequest.STATUS_REVEIW;
            }
            if (aintSourceValue.IsNullOrEmpty() && aintRequestTypeValue == busConstant.ReturnToWorkRequest.REQUEST_TYPE_ERTW)
            {
                aintSourceValue = busConstant.ReturnToWorkRequest.SOURCE_ONLINE;
            }
            this.icdoReturnToWorkRequest.source_value = aintSourceValue;
            this.icdoReturnToWorkRequest.request_type_value = aintRequestTypeValue;
            this.icdoReturnToWorkRequest.notification_form_created_on = DateTime.Today;
            this.icdoReturnToWorkRequest.created_by = this.iobjPassInfo.istrUserID;
            this.icdoReturnToWorkRequest.created_date = DateTime.Today;
            this.icdoReturnToWorkRequest.modified_by = this.iobjPassInfo.istrUserID;
            this.icdoReturnToWorkRequest.modified_date = DateTime.Today;
            this.icdoReturnToWorkRequest.PopulateDescriptions();
            this.icdoReturnToWorkRequest.ienuObjectState = ObjectState.Insert;
            this.icdoReturnToWorkRequest.Insert();
            this.ibusPayee.LoadCorrAddress();
            if (utlPassInfo.iobjPassInfo.itrnFramework == null || aintSourceValue == busConstant.ReturnToWorkRequest.SOURCE_BATCH)
            {
                BPMDBHelper.BeginTransaction(utlPassInfo.iobjPassInfo);
            }
            Dictionary<string, object> lhstRequestParameters = new Dictionary<string, object>();
            lhstRequestParameters.Add("PersonId", this.icdoReturnToWorkRequest.person_id);
            lhstRequestParameters.Add("Source", aintSourceValue);
            lhstRequestParameters.Add("ReemploymentNotificationId", this.icdoReturnToWorkRequest.reemployment_notification_id);

            ArrayList larrResult = new ArrayList();
            if (aintRequestTypeValue == busConstant.ReturnToWorkRequest.REQUEST_TYPE_RTW)
            {
                larrResult = BpmHelper.InitiateCaseInstance(busConstant.ReturnToWorkRequest.MAP_RETURN_TO_WORK, busConstant.ReturnToWorkRequest.BPM_RETURN_TO_WORK_PROCESS_NAME, this.icdoReturnToWorkRequest.person_id, 0, 0, utlPassInfo.iobjPassInfo, astrRequestSource: aintSourceValue, astrUserIdToWhomActivityToBeAssigned: iobjPassInfo.istrUserID, adctRequestParameters: lhstRequestParameters, ablnCheckForExistingInstance: true, ablnReturnCaseInstance: true, aenmActivityInitiateType: enmActivityInitiateType.Initiate);
            }
            else if (aintRequestTypeValue == busConstant.ReturnToWorkRequest.REQUEST_TYPE_ERTW)
            {
                larrResult = BpmHelper.InitiateCaseInstance(busConstant.ReturnToWorkRequest.MAP_END_RETURN_TO_WORK, busConstant.ReturnToWorkRequest.BPM_END_RETURN_TO_WORK_PROCESS_NAME, this.icdoReturnToWorkRequest.person_id, 0, 0, utlPassInfo.iobjPassInfo, astrRequestSource: aintSourceValue, astrUserIdToWhomActivityToBeAssigned: iobjPassInfo.istrUserID, adctRequestParameters: lhstRequestParameters, ablnCheckForExistingInstance: true, ablnReturnCaseInstance: true, aenmActivityInitiateType: enmActivityInitiateType.Initiate);
            }

            if (larrResult != null && larrResult.Count > 0 && !(larrResult[0] is utlError))
            {
                if (larrResult.Count == 2 && larrResult[0] is busSolBpmActivityInstance)
                {
                    this.LoadActivityInstance(larrResult[0] as busSolBpmActivityInstance);
                }
            }

            this.iclbNotes = new Collection<busNotes>();

            if (aintSourceValue == busConstant.ReturnToWorkRequest.SOURCE_INDEXING && aintRequestTypeValue == busConstant.ReturnToWorkRequest.REQUEST_TYPE_RTW)
            {
                busNotes lobjNotes = new busNotes { icdoNotes = new cdoNotes() };
                lobjNotes.icdoNotes.person_id = this.icdoReturnToWorkRequest.person_id;
                lobjNotes.icdoNotes.form_id = busConstant.Form_ID;
                lobjNotes.icdoNotes.form_value = busConstant.ReturnToWorkRequest.RETURN_TO_WORK_REQUEST_FORM;
                lobjNotes.icdoNotes.notes = "Re-employment form received.";
                lobjNotes.icdoNotes.reference_id = this.icdoReturnToWorkRequest.reemployment_notification_id;
                lobjNotes.icdoNotes.created_by = this.iobjPassInfo.istrUserID;
                lobjNotes.icdoNotes.created_date = DateTime.Now;
                lobjNotes.icdoNotes.modified_date = DateTime.Now;
                lobjNotes.icdoNotes.modified_by = this.iobjPassInfo.istrUserID;
                lobjNotes.icdoNotes.ienuObjectState = ObjectState.Insert;
                lobjNotes.icdoNotes.Insert();
            }
            utlPassInfo.iobjPassInfo.Commit();

            this.LoadPayeeAccount();
            this.CheckMemberIsRetiree();
            this.LoadCorTracking();
            this.LoadReturnToWorkHistory();
            this.iclbNotes = this.LoadNotesForRTW(this.ibusPayee.icdoPerson.person_id, busConstant.ReturnToWorkRequest.RETURN_TO_WORK_REQUEST_FORM, this.icdoReturnToWorkRequest.reemployment_notification_id);
            this.AddAuditingNotes();
            this.EvaluateInitialLoadRules();
            return this;
        }
    }
}
