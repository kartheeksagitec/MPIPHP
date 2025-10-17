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
    /// Class MPIPHP.BusinessObjects.busPersonDependent:
    /// Inherited from busPersonDependentGen, the class is used to customize the business object busPersonDependentGen.
    /// </summary>
    [Serializable]
    public class busPersonDependent : busRelationship
    {
        #region Properties

        public string istrMpiPersonID { get; set; }

        public busPerson ibusPersonDependent { get; set; }

        public Collection<busPersonDependent> iclbPersonDependent { get; set; }

        #endregion

        # region override
        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            if (this.icdoRelationship.dependent_person_id != 0)
            {
                base.ValidateHardErrors(aenmPageMode);
            }
            else
            {
                this.iarrErrors.Add(AddError(1150, ""));
            }
        }
        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();

            ibusPersonDependent = new busPerson();
            if (this.icdoRelationship.dependent_person_id != 0)
            {
                ibusPersonDependent.FindPerson(icdoRelationship.dependent_person_id);

                //While adding a dependent - If the participant is a VIP then the dependent also becomes a VIP
                if (this.ibusPerson.icdoPerson.vip_flag == busConstant.FLAG_YES)
                {
                    this.ibusPersonDependent.icdoPerson.vip_flag = busConstant.FLAG_YES;
                    this.ibusPersonDependent.icdoPerson.Update();
                    AuditLogHistoryForPerson();
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
                // decommissioning demographics informations, since HEDB is retiring.
                //OPUS data push to Health Eligibility for New Dependent                //Commented - Rohan Code For data Push to HEDB  (Do not delete this)
                //if (this.iobjPassInfo.ienmPageMode == utlPageMode.New)
                //{

                //    utlPassInfo lobjPassInfo1 = new utlPassInfo();
                //    lobjPassInfo1.idictParams["ID"] = "OPUS_INTEGRATION";
                //    lobjPassInfo1.iconFramework = DBFunction.GetDBConnection("HELegacy");

                //    if (lobjPassInfo1.iconFramework != null)
                //    {


                //        string lstrDependentPersonId = this.istrMpiPersonID;
                //        string lstrDependentPersonSSN = this.ibusPersonDependent.icdoPerson.istrSSNNonEncrypted;
                //        string lstrParticipantMPIId = this.ibusPerson.icdoPerson.mpi_person_id;
                //        string lstrRelationshipType = "D";
                //        string lstrFirstName = this.ibusPersonDependent.icdoPerson.first_name;
                //        string lstrMiddleName = this.ibusPersonDependent.icdoPerson.middle_name;
                //        string lstrlastName = this.ibusPersonDependent.icdoPerson.last_name;
                //        string lstrGender = this.ibusPersonDependent.icdoPerson.gender_value;

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

                //        DateTime lstrDOB = DateTime.MinValue;

                //        lstrDOB = this.ibusPersonDependent.icdoPerson.idtDateofBirth;

                //        DateTime lstrDOD = DateTime.MinValue;

                //        lstrDOD = this.ibusPersonDependent.icdoPerson.date_of_death;

                //        if (this.ibusPersonDependent.icdoPerson.date_of_death == DateTime.MinValue)
                //        {
                //            DataTable ldtblGetDateOfDeath = Select("cdoDeathNotification.GetDateOfDeathInProgress", new object[1] { this.ibusPersonDependent.icdoPerson.person_id });
                //            if (ldtblGetDateOfDeath != null && ldtblGetDateOfDeath.Rows.Count > 0
                //                && Convert.ToString(ldtblGetDateOfDeath.Rows[0][enmDeathNotification.date_of_death.ToString().ToUpper()]).IsNotNullOrEmpty())
                //            {
                //                lstrDOD = Convert.ToDateTime(ldtblGetDateOfDeath.Rows[0][enmDeathNotification.date_of_death.ToString().ToUpper()]);
                //            }
                //        }
                //        else
                //        {
                //            lstrDOD = this.ibusPersonDependent.icdoPerson.date_of_death;
                //        }



                //        string lstrHomePhone = this.ibusPersonDependent.icdoPerson.home_phone_no;
                //        string lstrCellPhone = this.ibusPersonDependent.icdoPerson.cell_phone_no;
                //        string lstrFax = this.ibusPersonDependent.icdoPerson.fax_no;
                //        string lstrEmail = this.ibusPersonDependent.icdoPerson.email_address_1;
                //        string lstrCreatedBy = this.icdoRelationship.modified_by;


                //        //Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();

                //        //IDbDataParameter lobjParameter = DBFunction.GetDBParameter();
                //        //lobjParameter.ParameterName = "@PID";
                //        //lobjParameter.DbType = DbType.String;
                //        //lobjParameter.Value = lstrDependentPersonId;
                //        //lcolParameters.Add(lobjParameter);

                //        //if (lstrDependentPersonSSN.IsNotNullOrEmpty())
                //        //{
                //        //    IDbDataParameter lobjParameter1 = DBFunction.GetDBParameter();
                //        //    lobjParameter1.ParameterName = "@SSN";
                //        //    lobjParameter1.DbType = DbType.String;
                //        //    lobjParameter1.Value = lstrDependentPersonSSN.ToLower();
                //        //    lcolParameters.Add(lobjParameter1);
                //        //}

                //        //IDbDataParameter lobjParameter2 = DBFunction.GetDBParameter();
                //        //lobjParameter2.ParameterName = "@ParticipantPID";
                //        //lobjParameter2.DbType = DbType.String;
                //        //lobjParameter2.Value = lstrParticipantMPIId.ToLower();
                //        //lcolParameters.Add(lobjParameter2);

                //        //IDbDataParameter lobjParameter3 = DBFunction.GetDBParameter();
                //        //lobjParameter3.ParameterName = "@EntityTypeCode";
                //        //lobjParameter3.DbType = DbType.String;
                //        //lobjParameter3.Value = "P";                  //we will always use Person
                //        //lcolParameters.Add(lobjParameter3);

                //        //IDbDataParameter lobjParameter4 = DBFunction.GetDBParameter();
                //        //lobjParameter4.ParameterName = "@RelationType";
                //        //lobjParameter4.DbType = DbType.String;
                //        //lobjParameter4.Value = lstrRelationshipType;
                //        //lcolParameters.Add(lobjParameter4);

                //        //IDbDataParameter lobjParameter5 = DBFunction.GetDBParameter();
                //        //lobjParameter5.ParameterName = "@FirstName";
                //        //lobjParameter5.DbType = DbType.String;
                //        //lobjParameter5.Value = lstrFirstName;
                //        //lcolParameters.Add(lobjParameter5);

                //        //IDbDataParameter lobjParameter6 = DBFunction.GetDBParameter();
                //        //lobjParameter6.ParameterName = "@MiddleName";
                //        //lobjParameter6.DbType = DbType.String;
                //        //lobjParameter6.Value = lstrMiddleName;
                //        //lcolParameters.Add(lobjParameter6);

                //        //IDbDataParameter lobjParameter7 = DBFunction.GetDBParameter();
                //        //lobjParameter7.ParameterName = "@LastName";
                //        //lobjParameter7.DbType = DbType.String;
                //        //lobjParameter7.Value = lstrlastName;
                //        //lcolParameters.Add(lobjParameter7);

                //        //IDbDataParameter lobjParameter9 = DBFunction.GetDBParameter();
                //        //lobjParameter9.ParameterName = "@Gender";
                //        //lobjParameter9.DbType = DbType.String;
                //        //lobjParameter9.Value = lstrGender;
                //        //lcolParameters.Add(lobjParameter9);


                //        //IDbDataParameter lobjParameter10 = DBFunction.GetDBParameter();
                //        //lobjParameter10.ParameterName = "@DateOfBirth";
                //        //lobjParameter10.DbType = DbType.DateTime;
                //        //if (lstrDOB != DateTime.MinValue)
                //        //{
                //        //    lobjParameter10.Value = lstrDOB;
                //        //}
                //        //lcolParameters.Add(lobjParameter10);


                //        //IDbDataParameter lobjParameter11 = DBFunction.GetDBParameter();
                //        //lobjParameter11.ParameterName = "@DateOfDeath";
                //        //lobjParameter11.DbType = DbType.DateTime;

                //        //if (lstrDOD != DateTime.MinValue)
                //        //{
                //        //    lobjParameter11.Value = lstrDOD;
                //        //}
                //        //lcolParameters.Add(lobjParameter11);


                //        //IDbDataParameter lobjParameter12 = DBFunction.GetDBParameter();
                //        //lobjParameter12.ParameterName = "@HomePhone";
                //        //lobjParameter12.DbType = DbType.String;
                //        //lobjParameter12.Value = lstrHomePhone;
                //        //lcolParameters.Add(lobjParameter12);

                //        //IDbDataParameter lobjParameter13 = DBFunction.GetDBParameter();
                //        //lobjParameter13.ParameterName = "@CellPhone";
                //        //lobjParameter13.DbType = DbType.String;
                //        //lobjParameter13.Value = lstrCellPhone;
                //        //lcolParameters.Add(lobjParameter13);

                //        //IDbDataParameter lobjParameter14 = DBFunction.GetDBParameter();
                //        //lobjParameter14.ParameterName = "@Fax";
                //        //lobjParameter14.DbType = DbType.String;
                //        //lobjParameter14.Value = lstrFax;
                //        //lcolParameters.Add(lobjParameter14);

                //        //IDbDataParameter lobjParameter15 = DBFunction.GetDBParameter();
                //        //lobjParameter15.ParameterName = "@Email";
                //        //lobjParameter15.DbType = DbType.String;
                //        //lobjParameter15.Value = lstrEmail;
                //        //lcolParameters.Add(lobjParameter15);

                //        //IDbDataParameter lobjParameter16 = DBFunction.GetDBParameter();
                //        //lobjParameter16.ParameterName = "@AuditUser";
                //        //lobjParameter16.DbType = DbType.String;
                //        //lobjParameter16.Value = lstrCreatedBy;
                //        //lcolParameters.Add(lobjParameter16);

                //        //try
                //        //{
                //        //    lobjPassInfo1.BeginTransaction();
                //        //    DBFunction.DBExecuteProcedure("USP_PID_PERSON_INS", lcolParameters, lobjPassInfo1.iconFramework, lobjPassInfo1.itrnFramework);
                //        //    lobjPassInfo1.Commit();
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

                //        //Commented - Rohan Code For data Push to HEDB
                //    }



                //    // OPUS data push to Health Eligibility for any new Address     //Commented - Rohan Code For data Push to HEDB  (Do not delete this)


                //    busPersonAddress lbusDependentAddress = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() };
                //    ibusPersonDependent.LoadPersonAddresss();

                //    if (ibusPersonDependent.iclbPersonAddress != null
                //        && ibusPersonDependent.iclbPersonAddress.Count > 0)
                //    {
                //        if (ibusPersonDependent.iclbPersonAddress.Where(item => item.icdoPersonAddress.start_date <= DateTime.Now
                //                    && (item.icdoPersonAddress.end_date == DateTime.MinValue || item.icdoPersonAddress.end_date > DateTime.Now)).Count() > 0)
                //        {
                //            lbusDependentAddress = ibusPersonDependent.iclbPersonAddress.Where(item => item.icdoPersonAddress.start_date <= DateTime.Now
                //                && (item.icdoPersonAddress.end_date == DateTime.MinValue || item.icdoPersonAddress.end_date > DateTime.Now)).FirstOrDefault();
                //        }
                //    }

                //    if (lbusDependentAddress.icdoPersonAddress.start_date != DateTime.MinValue &&
                //        lbusDependentAddress.icdoPersonAddress.start_date <= DateTime.Now)
                //    {
                //        string lstrContactName = string.Empty;
                //        string lstrAddrline1 = string.Empty;
                //        string lstrAddrline2 = string.Empty;
                //        string lstrCity = string.Empty;
                //        string lstrState = string.Empty;
                //        string lstrPostalCode = string.Empty;
                //        string lstrCountry = string.Empty;
                //        string lstrCountryValue = string.Empty;
                //        int lintForeignAddrFlag = 0;
                //        int lintDoNotUpdate = 0;
                //        bool lbnIsContact = false;
                //        string lstrAuditUser = string.Empty;
                //        string lstrPersonMpiID = string.Empty;
                //        DateTime ldtAddressEndDate = DateTime.MinValue;
                //        string lstrAddressSource = string.Empty;
                //        DateTime ldtAddressStartDate = DateTime.MinValue;

                //        int lintPersonID = lbusDependentAddress.icdoPersonAddress.person_id;
                //        ibusPerson.iclbPersonContact = new Collection<busPersonContact>();

                //        DataTable ldtbPersonContactList = busBase.Select<cdoPersonContact>(
                //            new string[1] { enmPersonContact.person_id.ToString() },
                //            new object[1] { lintPersonID }, null, enmPersonContact.effective_start_date.ToString());

                //        foreach (DataRow ldtRow in ldtbPersonContactList.Rows)
                //        {
                //            busPersonContact lbusPersonContact = new busPersonContact { icdoPersonContact = new cdoPersonContact() };
                //            lbusPersonContact.icdoPersonContact.LoadData(ldtRow);


                //            if (lbusPersonContact.icdoPersonContact.effective_end_date == DateTime.MinValue || lbusPersonContact.icdoPersonContact.effective_end_date > DateTime.Now)
                //            {
                //                if ((lbusPersonContact.icdoPersonContact.contact_type_value == "POAP" || lbusPersonContact.icdoPersonContact.contact_type_value == "GRDN"
                //                    || lbusPersonContact.icdoPersonContact.contact_type_value == "COAP")
                //                        && lbusPersonContact.icdoPersonContact.correspondence_addr_flag == "Y")
                //                {
                //                    lbnIsContact = true;
                //                    lstrContactName = lbusPersonContact.icdoPersonContact.contact_name;
                //                    lstrAddrline1 = lbusPersonContact.icdoPersonContact.addr_line_1;
                //                    lstrAddrline2 = lbusPersonContact.icdoPersonContact.addr_line_2;
                //                    lstrCity = lbusPersonContact.icdoPersonContact.addr_city;

                //                    if (Convert.ToInt32(lbusPersonContact.icdoPersonContact.addr_country_value) == busConstant.USA || Convert.ToInt32(lbusPersonContact.icdoPersonContact.addr_country_value) == busConstant.AUSTRALIA
                //                           || Convert.ToInt32(lbusPersonContact.icdoPersonContact.addr_country_value) == busConstant.CANADA || Convert.ToInt32(lbusPersonContact.icdoPersonContact.addr_country_value) == busConstant.MEXICO
                //                           || Convert.ToInt32(lbusPersonContact.icdoPersonContact.addr_country_value) == busConstant.NewZealand)
                //                    {
                //                        lstrState = lbusPersonContact.icdoPersonContact.addr_state_value;
                //                    }
                //                    else
                //                    {
                //                        lstrState = lbusPersonContact.icdoPersonContact.foreign_province;
                //                    }

                //                    if (Convert.ToInt32(lbusPersonContact.icdoPersonContact.addr_country_value) == busConstant.USA)
                //                    {

                //                        lstrPostalCode = lbusPersonContact.icdoPersonContact.addr_zip_code + lbusPersonContact.icdoPersonContact.addr_zip_4_code;
                //                    }
                //                    else
                //                    {
                //                        lstrPostalCode = lbusPersonContact.icdoPersonContact.foreign_postal_code;
                //                    }

                //                    lstrCountry = lbusPersonContact.icdoPersonContact.addr_country_description;
                //                    if (!string.IsNullOrEmpty(lbusPersonContact.icdoPersonContact.addr_country_value))
                //                        lstrCountryValue = HelperUtil.GetData1ByCodeValue(lbusPersonContact.icdoPersonContact.addr_country_id, lbusPersonContact.icdoPersonContact.addr_country_value);

                //                    if (Convert.ToInt32(lbusPersonContact.icdoPersonContact.addr_country_value) == busConstant.USA)
                //                    {
                //                        lintForeignAddrFlag = 0;
                //                    }
                //                    else
                //                    {
                //                        lintForeignAddrFlag = 1;
                //                    }

                //                    lstrAuditUser = iobjPassInfo.istrUserID;

                //                    ldtAddressStartDate = lbusPersonContact.icdoPersonContact.effective_start_date;

                //                    if (lbusPersonContact.icdoPersonAddress.end_date.IsNotNull() && lbusPersonContact.icdoPersonAddress.end_date != DateTime.MinValue)
                //                        ldtAddressEndDate = lbusPersonContact.icdoPersonAddress.end_date;
                //                    else
                //                        ldtAddressEndDate = DateTime.MinValue;

                //                }
                //            }

                //        }


                //        if (!lbnIsContact)
                //        {
                //            lstrAddrline1 = lbusDependentAddress.icdoPersonAddress.addr_line_1;
                //            lstrAddrline2 = lbusDependentAddress.icdoPersonAddress.addr_line_2;
                //            lstrCity = lbusDependentAddress.icdoPersonAddress.addr_city;

                //            if (Convert.ToInt32(lbusDependentAddress.icdoPersonAddress.addr_country_value) == busConstant.USA || Convert.ToInt32(lbusDependentAddress.icdoPersonAddress.addr_country_value) == busConstant.AUSTRALIA
                //                || Convert.ToInt32(lbusDependentAddress.icdoPersonAddress.addr_country_value) == busConstant.CANADA || Convert.ToInt32(lbusDependentAddress.icdoPersonAddress.addr_country_value) == busConstant.MEXICO
                //                || Convert.ToInt32(lbusDependentAddress.icdoPersonAddress.addr_country_value) == busConstant.NewZealand)
                //            {
                //                lstrState = lbusDependentAddress.icdoPersonAddress.addr_state_value;
                //            }
                //            else
                //            {
                //                lstrState = lbusDependentAddress.icdoPersonAddress.foreign_province;
                //            }

                //            if (Convert.ToInt32(lbusDependentAddress.icdoPersonAddress.addr_country_value) == busConstant.USA)
                //            {

                //                lstrPostalCode = lbusDependentAddress.icdoPersonAddress.addr_zip_code + lbusDependentAddress.icdoPersonAddress.addr_zip_4_code;
                //            }
                //            else
                //            {
                //                lstrPostalCode = lbusDependentAddress.icdoPersonAddress.foreign_postal_code;
                //            }

                //            lstrCountry = lbusDependentAddress.icdoPersonAddress.addr_country_description;
                //            if (!string.IsNullOrEmpty(lbusDependentAddress.icdoPersonAddress.addr_country_value))
                //            {
                //                lstrCountryValue = HelperUtil.GetData1ByCodeValue(lbusDependentAddress.icdoPersonAddress.addr_country_id, lbusDependentAddress.icdoPersonAddress.addr_country_value);
                //            }
                //            if (Convert.ToInt32(lbusDependentAddress.icdoPersonAddress.addr_country_value) == busConstant.USA)
                //            {
                //                lintForeignAddrFlag = 0;
                //            }
                //            else
                //            {
                //                lintForeignAddrFlag = 1;
                //            }

                //            if (lbusDependentAddress.icdoPersonAddress.secured_flag == "Y")
                //            {
                //                lintDoNotUpdate = 1;
                //            }
                //            else
                //            {
                //                lintDoNotUpdate = 0;
                //            }

                //            lstrAuditUser = iobjPassInfo.istrUserID;

                //            ldtAddressStartDate = lbusDependentAddress.icdoPersonAddress.start_date;

                //            if (lbusDependentAddress.icdoPersonAddress.end_date.IsNotNull() && lbusDependentAddress.icdoPersonAddress.end_date != DateTime.MinValue)
                //                ldtAddressEndDate = lbusDependentAddress.icdoPersonAddress.end_date;
                //            else
                //                ldtAddressEndDate = DateTime.MinValue;

                //            lstrAddressSource = lbusDependentAddress.icdoPersonAddress.addr_source_description;
                //        }

                //        ////On EADB

                //        if ((iobjPassInfo.iconFramework).Database == "MPI" || (iobjPassInfo.iconFramework).Database == "MPIPHP")
                //        {
                //            return;
                //        }
                //        //DataTable ldtbSysMgmt = iobjPassInfo.isrvDBCache.GetSystemManagement();
                //        //if (ldtbSysMgmt.Rows.Count > 0 && ldtbSysMgmt.Rows[0]["REGION_VALUE"].ToString() == "DEVL")
                //        //{
                //        //    return;
                //        //}


                //        lobjPassInfo1 = new utlPassInfo();
                //        lobjPassInfo1.idictParams["ID"] = "OPUS_INTEGRATION";
                //        lobjPassInfo1.iconFramework = DBFunction.GetDBConnection("HELegacy");

                //        if (lobjPassInfo1.iconFramework != null)
                //        {

                //            lstrPersonMpiID = ibusPersonDependent.icdoPerson.mpi_person_id;


                //            Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();

                //            IDbDataParameter lobjParameter1 = DBFunction.GetDBParameter();
                //            lobjParameter1.ParameterName = "@PID";
                //            lobjParameter1.DbType = DbType.String;
                //            lobjParameter1.Value = lstrPersonMpiID.ToLower();
                //            lcolParameters.Add(lobjParameter1);



                //            if (lbnIsContact)
                //            {
                //                IDbDataParameter lobjParameter3 = DBFunction.GetDBParameter();
                //                lobjParameter3.ParameterName = "@Attention";
                //                lobjParameter3.DbType = DbType.String;
                //                lobjParameter3.Value = lstrContactName;
                //                lcolParameters.Add(lobjParameter3);

                //            }

                //            IDbDataParameter lobjParameter4 = DBFunction.GetDBParameter();
                //            lobjParameter4.ParameterName = "@Address1";
                //            lobjParameter4.DbType = DbType.String;
                //            lobjParameter4.Value = lstrAddrline1;
                //            lcolParameters.Add(lobjParameter4);

                //            IDbDataParameter lobjParameter5 = DBFunction.GetDBParameter();
                //            lobjParameter5.ParameterName = "@Address2";
                //            lobjParameter5.DbType = DbType.String;
                //            lobjParameter5.Value = lstrAddrline2;
                //            lcolParameters.Add(lobjParameter5);

                //            IDbDataParameter lobjParameter6 = DBFunction.GetDBParameter();
                //            lobjParameter6.ParameterName = "@City";
                //            lobjParameter6.DbType = DbType.String;
                //            lobjParameter6.Value = lstrCity;
                //            lcolParameters.Add(lobjParameter6);

                //            IDbDataParameter lobjParameter7 = DBFunction.GetDBParameter();
                //            lobjParameter7.ParameterName = "@State";
                //            lobjParameter7.DbType = DbType.String;
                //            lobjParameter7.Value = lstrState;
                //            lcolParameters.Add(lobjParameter7);

                //            IDbDataParameter lobjParameter8 = DBFunction.GetDBParameter();
                //            lobjParameter8.ParameterName = "@PostalCode";
                //            lobjParameter8.DbType = DbType.String;
                //            lobjParameter8.Value = lstrPostalCode;
                //            lcolParameters.Add(lobjParameter8);

                //            IDbDataParameter lobjParameter9 = DBFunction.GetDBParameter();
                //            lobjParameter9.ParameterName = "@Country";
                //            lobjParameter9.DbType = DbType.String;
                //            lobjParameter9.Value = lstrCountry;
                //            lcolParameters.Add(lobjParameter9);

                //            IDbDataParameter lobjParameter10 = DBFunction.GetDBParameter();
                //            lobjParameter10.ParameterName = "@CountryCode";
                //            lobjParameter10.DbType = DbType.String;
                //            lobjParameter10.Value = lstrCountryValue;
                //            lcolParameters.Add(lobjParameter10);

                //            IDbDataParameter lobjParameter11 = DBFunction.GetDBParameter();
                //            lobjParameter11.ParameterName = "@ForeignAddr";
                //            lobjParameter11.DbType = DbType.String;
                //            lobjParameter11.Value = lintForeignAddrFlag;
                //            lcolParameters.Add(lobjParameter11);


                //            IDbDataParameter lobjParameter12 = DBFunction.GetDBParameter();
                //            lobjParameter12.ParameterName = "@ReturnedMail";
                //            lobjParameter12.DbType = DbType.DateTime;
                //            if (ldtAddressEndDate == DateTime.MinValue)
                //                lobjParameter12.Value = DBNull.Value;
                //            else
                //                lobjParameter12.Value = ldtAddressEndDate;
                //            lcolParameters.Add(lobjParameter12);

                //            if (!lbnIsContact)
                //            {
                //                IDbDataParameter lobjParameter13 = DBFunction.GetDBParameter();
                //                lobjParameter13.ParameterName = "@DoNotUpdate";
                //                lobjParameter13.DbType = DbType.String;
                //                lobjParameter13.Value = lintDoNotUpdate;
                //                lcolParameters.Add(lobjParameter13);
                //            }

                //            IDbDataParameter lobjParameter14 = DBFunction.GetDBParameter();
                //            lobjParameter14.ParameterName = "@AuditUser";
                //            lobjParameter14.DbType = DbType.String;
                //            lobjParameter14.Value = lstrAuditUser;
                //            lcolParameters.Add(lobjParameter14);

                //            IDbDataParameter lobjParameter15 = DBFunction.GetDBParameter();
                //            lobjParameter15.ParameterName = "@ReceivedFrom";
                //            lobjParameter15.DbType = DbType.String;
                //            lobjParameter15.Value = lstrAddressSource;
                //            lcolParameters.Add(lobjParameter15);


                //            IDbDataParameter lobjParameter16 = DBFunction.GetDBParameter();
                //            lobjParameter16.ParameterName = "@AddressStartDate";
                //            lobjParameter16.DbType = DbType.DateTime;
                //            lobjParameter16.Value = ldtAddressStartDate;
                //            lcolParameters.Add(lobjParameter16);


                //            try
                //            {
                //                lobjPassInfo1.BeginTransaction();
                //                DBFunction.DBExecuteProcedure("USP_PID_PersonAddress_UPD", lcolParameters, lobjPassInfo1.iconFramework, lobjPassInfo1.itrnFramework);
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

                //PIR 525
                if (this.icdoRelationship != null && this.icdoRelationship.addr_same_as_participant_flag == busConstant.FLAG_YES)
                {
                    this.icdoRelationship.addr_same_as_participant_flag = busConstant.FLAG_NO;
                    this.icdoRelationship.Update();
                }
            }
        }


        public override void BeforePersistChanges()
        {
            base.BeforePersistChanges();

            if (iobjPassInfo.idictParams.ContainsKey("Dependent_Maintenance_AddressFlag") && !string.IsNullOrEmpty(Convert.ToString(iobjPassInfo.GetParamValue("Dependent_Maintenance_AddressFlag"))))
            {
                this.icdoRelationship.addr_same_as_participant_flag = Convert.ToString(iobjPassInfo.GetParamValue("Dependent_Maintenance_AddressFlag"));
            }
            if (string.IsNullOrEmpty(this.icdoRelationship.addr_same_as_participant_flag))
            {
                this.icdoRelationship.addr_same_as_participant_flag = busConstant.FLAG_NO;
            }
        }

        public override int PersistChanges()
        {
            int lint = 0;
            if (icdoRelationship.ienuObjectState == ObjectState.Insert)
            {
                DataTable ldtbList = Select("cdoRelationship.CheckBeneficiaryExists", new object[2] { 
                   this.ibusPerson.icdoPerson.person_id, this.icdoRelationship.dependent_person_id });

                if (ldtbList.Rows.Count > 0)
                {
                    DataRow ldtRow = ldtbList.Rows[0];
                    busRelationship lbusRelationship = new busRelationship { icdoRelationship = new cdoRelationship() };
                    lbusRelationship.icdoRelationship.LoadData(ldtRow);
                    lbusRelationship.icdoRelationship.dependent_person_id = this.icdoRelationship.dependent_person_id;
                    lbusRelationship.icdoRelationship.relationship_value = this.icdoRelationship.relationship_value;
                    lbusRelationship.icdoRelationship.effective_end_date = this.icdoRelationship.effective_end_date;
                    lbusRelationship.icdoRelationship.effective_start_date = this.icdoRelationship.effective_start_date;
                    lbusRelationship.icdoRelationship.Update();
                }
                else
                {
                    lint = base.PersistChanges();
                }
            }
            else
            {
                lint = base.PersistChanges();
            }
            return lint;
        }

        public override int Delete()
        {
            int lint = this.icdoRelationship.dependent_person_id;
            if (this.icdoRelationship.beneficiary_person_id != 0)
            {
                this.icdoRelationship.dependent_person_id = 0;
                this.icdoRelationship.Update();
            }
            else
            {
                lint = base.Delete();
            }
            return lint;
        }
        #endregion


        #region public
        /// <summary>
        ///To check if Dependent age is greater than Participant age.
        /// </summary>  
        public bool CheckDependentDOB()
        {
            DataTable ldtbDOB = Select("cdoRelationship.GetDependentDOB", new object[1] { this.icdoRelationship.dependent_person_id });

            if (ldtbDOB.Rows[0][enmPerson.date_of_birth.ToString()].ToString().IsNotNullOrEmpty())
            {
                string lstrDependentDOB = Convert.ToString(ldtbDOB.Rows[0][enmPerson.date_of_birth.ToString()]);
                DateTime ldtDependentDOB = Convert.ToDateTime(lstrDependentDOB);
                DateTime ldtPersonDOB = this.ibusPerson.icdoPerson.idtDateofBirth;

                if ((ldtPersonDOB.ToString().IsNotNullOrEmpty()) && ldtPersonDOB != DateTime.MinValue)
                {
                    if ((ldtDependentDOB != DateTime.MinValue) && (ldtDependentDOB <= ldtPersonDOB))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        public bool CheckDependentMaritalStatus()
        {
            DataTable ldtbMaritalStatus = Select("cdoRelationship.GetDependentMaritalStatus", new object[1] { this.icdoRelationship.dependent_person_id });
            DataRow ldr = ldtbMaritalStatus.Rows[0];
            int lintresult = 0;

            lintresult = string.Compare(Convert.ToString(ldr[enmPerson.marital_status_value.ToString()]), busConstant.MARITAL_STATUS_MARRIED);
            if (!(lintresult == 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        public void AuditLogHistoryForPerson()
        {
            cdoFullAuditLog lcdoFullAuditLog = new cdoFullAuditLog();
            lcdoFullAuditLog.person_id = this.ibusPersonDependent.icdoPerson.person_id;
            lcdoFullAuditLog.primary_key = this.ibusPersonDependent.icdoPerson.person_id;
            lcdoFullAuditLog.form_name = "wfmPersonMaintenance";
            lcdoFullAuditLog.table_name = "sgt_person";
            //lcdoFullAuditLog.Insert();

            //cdoFullAuditLogDetail lcdoFullAuditLogDetail = new cdoFullAuditLogDetail();
            //lcdoFullAuditLogDetail.audit_log_id = lcdoFullAuditLog.audit_log_id;
            //lcdoFullAuditLogDetail.old_value = this.ibusPerson.icdoPerson.vip_flag;
            //lcdoFullAuditLogDetail.new_value = this.ibusPersonDependent.icdoPerson.vip_flag;
            //lcdoFullAuditLogDetail.column_name = "vip_flag";
            //lcdoFullAuditLogDetail.Insert();

            //Fw upgrade: PIR ID : 28660: New implementation of Audit History using audit_details
            var lcdoFullAuditLogDetail = new
            {
                column_name = "vip_flag",
                old_value = this.ibusPerson.icdoPerson.vip_flag,
                new_value = this.ibusPersonDependent.icdoPerson.vip_flag,
            };
            string lsrtJSONAuditDetails = Newtonsoft.Json.JsonConvert.SerializeObject(lcdoFullAuditLogDetail);
            lcdoFullAuditLog.audit_details = lsrtJSONAuditDetails;
            lcdoFullAuditLog.Insert();
        }

    }
}


