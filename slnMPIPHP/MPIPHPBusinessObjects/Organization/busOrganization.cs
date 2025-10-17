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
using Sagitec.CustomDataObjects;
using System.Linq;
#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busOrganization:
	/// Inherited from busOrganizationGen, the class is used to customize the business object busOrganizationGen.
	/// </summary>
	[Serializable]
	public class busOrganization : busOrganizationGen
    {
        public string iblnParticipant { get; set; }
        public string iblnBeneficiary { get; set; }
        public string iblnDependent { get; set; }
        public Collection<busNotes> iclbNotes { get; set; }
        public busOrgAddress ibusOrgAddress { get; set; }
        public busOrgBank ibusOrgBank { get; set; }
        #region override
        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            //To Generate MPI Person ID Temp:
            if (string.IsNullOrEmpty(this.icdoOrganization.mpi_org_id) )
            {
                //lintCapitalGain.ToString().PadLeft(12, '0');
                cdoCodeValue lobjcdoCodeValue = HelperUtil.GetCodeValueDetails(52, busConstant.MPID);
                int lintNewOrgID = Convert.ToInt32(lobjcdoCodeValue.data1);
                this.icdoOrganization.mpi_org_id = "M" + lintNewOrgID.ToString("D8");
                this.icdoOrganization.Update();

                lintNewOrgID += 1;
                lobjcdoCodeValue.data1 = lintNewOrgID.ToString();
                lobjcdoCodeValue.Update();

            }
            else if (this.icdoOrganization.mpi_org_id.Length < 9)
            {
                this.icdoOrganization.mpi_org_id = "M" + this.icdoOrganization.org_id.ToString().PadLeft(8, '0');
                this.icdoOrganization.Update();
            }

            //OPUS data push to Health Eligibility for any person Update  //Commented - Rohan Code For data Push to HEDB (Do not delete this)
            string lstrEntityCodeType = string.Empty;

            //DataTable ldtbSysMgmt = iobjPassInfo.isrvDBCache.GetSystemManagement();
            //if (ldtbSysMgmt.Rows.Count > 0 && ldtbSysMgmt.Rows[0]["REGION_VALUE"].ToString() == "DEVL")
            //{
            //    return;
            //}
            if ((iobjPassInfo.iconFramework).Database == "MPI" || (iobjPassInfo.iconFramework).Database == "MPIPHP")
            {
                return;
            }
            if (this.iarrChangeLog.Count == 0)
            {
                iobjPassInfo.idictParams["SaveMesssageOnOrganization"] = "false";
            }
            // decommissioning demographics informations, since HEDB is retiring.
            //if (iobjPassInfo.ienmPageMode == utlPageMode.Update)
            //{
            //    utlPassInfo lobjPassInfo1 = new utlPassInfo();
            //    lobjPassInfo1.idictParams["ID"] = "OPUS_INTEGRATION";
            //    lobjPassInfo1.iconFramework = DBFunction.GetDBConnection("HELegacy");

            //    if (lobjPassInfo1.iconFramework != null)
            //    {
            //        string strQuery = "select * from Organization where TaxID = (select ssn from Eligibility_PID_Reference where PID = '" + this.icdoOrganization.mpi_org_id + "')";
            //        DataTable ldtbResult = DBFunction.DBSelect(strQuery, lobjPassInfo1.iconFramework);
            //        if (ldtbResult.Rows.Count == 0)
            //        {
            //            return;
            //        }
            //        string lstrMPIOrgId = this.icdoOrganization.mpi_org_id;
            //        string lstrOrganizationfederalId = this.icdoOrganization.federal_id;
            //        string lstrRelationType = string.Empty;
            //        int CountBeneficiary = (int)DBFunction.DBExecuteScalar("cdoOrganization.CheckOrgIsBeneficiary", new object[1] { this.icdoOrganization.org_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            //        if (CountBeneficiary > 0)
            //        {
            //            lstrRelationType = "B";
            //        }
            //        else
            //        {
            //            lstrRelationType = null;
            //        }

            //        if (this.icdoOrganization.org_type_value == "TRST" || this.icdoOrganization.org_type_value == "MTFA")
            //        {
            //            lstrEntityCodeType = "T";
            //        }
            //        else
            //        {
            //            lstrEntityCodeType = "O";
            //        }
            //        string lstrFirstName = this.icdoOrganization.org_name;
            //        string lstrMiddleName = string.Empty;
            //        string lstrlastName = string.Empty;
            //        string lstrGender = string.Empty;
            //        DateTime lstrDOB = Convert.ToDateTime("01/01/1900");
            //        DateTime lstrDOD = Convert.ToDateTime("01/01/1900");
            //        string lstrHomePhone = string.Empty;
            //        string lstrCellPhone = string.Empty;
            //        string lstrFax = string.Empty;
            //        string lstrEmail = string.Empty;
            //        string lstrCreatedBy = iobjPassInfo.istrUserID;
            //        //Ticket : 55015
            //        bool lboolVipFlag = false;
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

            //        if (iobjPassInfo.ienmPageMode == utlPageMode.Update)
            //        {

            //            //if (!String.IsNullOrEmpty(lstrOrganizationfederalId))
            //            //{
            //            Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();


            //            IDbDataParameter lobjParameter1 = DBFunction.GetDBParameter();
            //            lobjParameter1.ParameterName = "@PID";
            //            lobjParameter1.DbType = DbType.String;
            //            lobjParameter1.Value = lstrMPIOrgId.ToLower();
            //            lcolParameters.Add(lobjParameter1);

            //            //PROD PIR 69
            //            if (!String.IsNullOrEmpty(lstrOrganizationfederalId))
            //            {
            //                IDbDataParameter lobjParameter2 = DBFunction.GetDBParameter();
            //                lobjParameter2.ParameterName = "@SSN";
            //                lobjParameter2.DbType = DbType.String;
            //                lobjParameter2.Value = lstrOrganizationfederalId.ToLower();
            //                lcolParameters.Add(lobjParameter2);
            //            }

            //            IDbDataParameter lobjParameter3 = DBFunction.GetDBParameter();
            //            lobjParameter3.ParameterName = "@ParticipantPID";
            //            lobjParameter3.DbType = DbType.String;
            //            lobjParameter3.Value = DBNull.Value;
            //            lcolParameters.Add(lobjParameter3);

            //            IDbDataParameter lobjParameter4 = DBFunction.GetDBParameter();
            //            lobjParameter4.ParameterName = "@EntityTypeCode";
            //            lobjParameter4.DbType = DbType.String;
            //            lobjParameter4.Value = lstrEntityCodeType;
            //            lcolParameters.Add(lobjParameter4);

            //            IDbDataParameter lobjParameter5 = DBFunction.GetDBParameter();
            //            lobjParameter5.ParameterName = "@RelationType";
            //            lobjParameter5.DbType = DbType.String;
            //            lobjParameter5.Value = lstrRelationType;
            //            lcolParameters.Add(lobjParameter5);

            //            IDbDataParameter lobjParameter6 = DBFunction.GetDBParameter();
            //            lobjParameter6.ParameterName = "@FirstName";
            //            lobjParameter6.DbType = DbType.String;
            //            lobjParameter6.Value = lstrFirstName;
            //            lcolParameters.Add(lobjParameter6);

            //            IDbDataParameter lobjParameter7 = DBFunction.GetDBParameter();
            //            lobjParameter7.ParameterName = "@MiddleName";
            //            lobjParameter7.DbType = DbType.String;
            //            lobjParameter7.Value = lstrMiddleName;
            //            lcolParameters.Add(lobjParameter7);

            //            IDbDataParameter lobjParameter8 = DBFunction.GetDBParameter();
            //            lobjParameter8.ParameterName = "@LastName";
            //            lobjParameter8.DbType = DbType.String;
            //            lobjParameter8.Value = lstrlastName;
            //            lcolParameters.Add(lobjParameter8);

            //            IDbDataParameter lobjParameter9 = DBFunction.GetDBParameter();
            //            lobjParameter9.ParameterName = "@Gender";
            //            lobjParameter9.DbType = DbType.String;
            //            lobjParameter9.Value = lstrGender;
            //            lcolParameters.Add(lobjParameter9);

            //            IDbDataParameter lobjParameter10 = DBFunction.GetDBParameter();
            //            lobjParameter10.ParameterName = "@DateOfBirth";
            //            lobjParameter10.DbType = DbType.DateTime;
            //            lobjParameter10.Value = lstrDOB;
            //            lcolParameters.Add(lobjParameter10);

            //            IDbDataParameter lobjParameter11 = DBFunction.GetDBParameter();
            //            lobjParameter11.ParameterName = "@DateOfDeath";
            //            lobjParameter11.DbType = DbType.DateTime;
            //            lobjParameter11.Value = DBNull.Value;
            //            if (lstrDOD != DateTime.MinValue)
            //            {
            //                lobjParameter11.Value = lstrDOD;
            //            }
            //            lcolParameters.Add(lobjParameter11);

            //            IDbDataParameter lobjParameter12 = DBFunction.GetDBParameter();
            //            lobjParameter12.ParameterName = "@HomePhone";
            //            lobjParameter12.DbType = DbType.String;
            //            lobjParameter12.Value = lstrHomePhone;
            //            lcolParameters.Add(lobjParameter12);

            //            IDbDataParameter lobjParameter13 = DBFunction.GetDBParameter();
            //            lobjParameter13.ParameterName = "@CellPhone";
            //            lobjParameter13.DbType = DbType.String;
            //            lobjParameter13.Value = lstrCellPhone;
            //            lcolParameters.Add(lobjParameter13);

            //            IDbDataParameter lobjParameter14 = DBFunction.GetDBParameter();
            //            lobjParameter14.ParameterName = "@Fax";
            //            lobjParameter14.DbType = DbType.String;
            //            lobjParameter14.Value = lstrFax;
            //            lcolParameters.Add(lobjParameter14);

            //            IDbDataParameter lobjParameter15 = DBFunction.GetDBParameter();
            //            lobjParameter15.ParameterName = "@Email";
            //            lobjParameter15.DbType = DbType.String;
            //            lobjParameter15.Value = lstrEmail;
            //            lcolParameters.Add(lobjParameter15);

            //            IDbDataParameter lobjParameter16 = DBFunction.GetDBParameter();
            //            lobjParameter16.ParameterName = "@AuditUser";
            //            lobjParameter16.DbType = DbType.String;
            //            lobjParameter16.Value = lstrCreatedBy;
            //            lcolParameters.Add(lobjParameter16);

            //            //Ticket : 55015
            //            IDbDataParameter lobjParameter17 = DBFunction.GetDBParameter();
            //            lobjParameter17.ParameterName = "@VipFlag";
            //            lobjParameter17.DbType = DbType.Boolean;
            //            lobjParameter17.Value = lboolVipFlag;
            //            lcolParameters.Add(lobjParameter17);
            //            try
            //            {
            //                lobjPassInfo1.BeginTransaction();
            //                DBFunction.DBExecuteProcedure("USP_PID_Person_UPD", lcolParameters, lobjPassInfo1.iconFramework, lobjPassInfo1.itrnFramework);
            //                lobjPassInfo1.Commit();

            //            }
            //            catch (Exception e)
            //            {
            //                lobjPassInfo1.Rollback();
            //                throw e;
            //            }
            //            finally
            //            {
            //                lobjPassInfo1.iconFramework.Close();
            //            }

            //        }

            //    }
            //}
        }

        public override void BeforePersistChanges()
        {
            if (string.IsNullOrEmpty(this.icdoOrganization.status_value))
            {
                this.icdoOrganization.status_value = busConstant.STATUS_ACTIVE;
            }

            //Shankar Bug_46 Empty Notes
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

            if (this.iclbNotes != null)
            {
                this.iclbNotes.ForEach(item =>
                {
                    if (item.icdoNotes.org_id == 0)
                        item.icdoNotes.person_id = this.icdoOrganization.org_id;
                    item.icdoNotes.form_id = busConstant.Form_ID;
                    item.icdoNotes.form_value = busConstant.ORG_MAINTAINENCE_FORM;
                });
            }
            base.BeforePersistChanges();
        }
        #endregion

        #region public
        public override void LoadOrgBanks()
        {
            DataTable ldtbOrgBanks = busBase.Select("cdoOrgBank.GetOrgDetailsByOrgId", new object[1] { icdoOrganization.org_id });
            this.iclbOrgBank = GetCollection<busOrgBank>(ldtbOrgBanks, "icdoOrgBank");
        }

        public busOrganization LoadOrgBanks4Organization(int aintOrgId)
        {
            DataTable ldtbOrgBanks = busBase.Select("cdoOrgBank.GetOrgDetailsByOrgId", new object[1] { aintOrgId });
            DataTable ldtbPaymentSchedule = busBase.Select<cdoPaymentSchedule>(new string[2] { "schedule_type_value", "status_value" },
                                                  new object[2] {busConstant.PaymentScheduleTypeMonthly,busConstant.PaymentScheduleActionStatusReadyforFinal
                                                   }, null, null);
            this.iclbOrgBank = GetCollection<busOrgBank>(ldtbOrgBanks, "icdoOrgBank");
            return this;
        }
        public busOrganization LoadOrganization(int aintOrgId)
        {
            busOrganization lbusOrganization = new busOrganization{icdoOrganization = new cdoOrganization()};
            lbusOrganization.FindOrganization(aintOrgId);
            return lbusOrganization;

        }
        #endregion
        public Collection<busReturnedMail> iclbReturnedMail { get; set; }
        public void LoadReturnedMail()
        {
            DataTable ldtblist = busPerson.Select("cdoReturnedMail.LoadReturnMailForOrg", new object[1] { this.icdoOrganization.org_id });
            if (ldtblist != null && ldtblist.Rows.Count > 0)
            {
                iclbReturnedMail = new Collection<busReturnedMail>();
                foreach (DataRow dr in ldtblist.Rows)
                {
                    busReturnedMail lbusReturnedMail = new busReturnedMail { icdoReturnedMail = new cdoReturnedMail() };
                    lbusReturnedMail.icdoReturnedMail.LoadData(dr);
                    
                    
                    lbusReturnedMail.ibusOrgAddress = new busOrgAddress { icdoOrgAddress = new cdoOrgAddress() };
                    lbusReturnedMail.ibusOrgAddress.icdoOrgAddress.LoadData(dr);
                    lbusReturnedMail.ibusDocument = new busDocument { icdoDocument = new cdoDocument() };
                    lbusReturnedMail.ibusDocument.icdoDocument.LoadData(dr);
                    iclbReturnedMail.Add(lbusReturnedMail);
                }
            }
            // iclbReturnedMail = GetCollection<busReturnedMail>(ldtblist, "icdoReturnedMail");
        }
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            base.LoadOtherObjects(adtrRow, aobjBus);
            if (aobjBus is busOrganization)
            {
                //busPerson lbusPerson = (aobjBus as busPerson);
                
                busOrganization lbusOrganization;
                lbusOrganization = (aobjBus as busOrganization);
                lbusOrganization.ibusOrgAddress = new busOrgAddress() { icdoOrgAddress = new cdoOrgAddress() };
                lbusOrganization.ibusOrgAddress.icdoOrgAddress.LoadData(adtrRow);
            }
        }

        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            base.ValidateHardErrors(aenmPageMode);

            utlError lobjError = null;
            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();


            if (icdoOrganization.payment_type_value.IsNullOrEmpty())
            {
                lobjError = AddError(6278, " ");
                this.iarrErrors.Add(lobjError);
            }

            if (icdoOrganization.org_type_value == busConstant.Organization.OrgPaymentTypeRolloverOrg
                && (icdoOrganization.payment_type_value != busConstant.PAYMENT_METHOD_ROLLOVER_CHECK && 
                    icdoOrganization.payment_type_value != busConstant.PAYMENT_METHOD_ROLLOVER_ACH))
            {
                lobjError = AddError(6279, " ");
                this.iarrErrors.Add(lobjError);
            }

            if (icdoOrganization.org_type_value != busConstant.Organization.OrgPaymentTypeRolloverOrg
                && (icdoOrganization.payment_type_value == busConstant.PAYMENT_METHOD_ROLLOVER_CHECK ||
                    icdoOrganization.payment_type_value == busConstant.PAYMENT_METHOD_ROLLOVER_ACH))
            {
                lobjError = AddError(6280, " ");
                this.iarrErrors.Add(lobjError);
            }
            if (icdoOrganization.aba_swift_bank_code.IsNotNullOrEmpty())
            {
                if (icdoOrganization.aba_swift_bank_code.Length < 8)
                {
                    lobjError = AddError(6312, " ");
                    this.iarrErrors.Add(lobjError);
                }

            }
            
        }

    }

  
}
