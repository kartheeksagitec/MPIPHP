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
using Sagitec.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busPersonContact:
	/// Inherited from busPersonContactGen, the class is used to customize the business object busPersonContactGen.
	/// </summary>
	[Serializable]
	public class busPersonContact : busPersonContactGen
    {
        public cdoPersonAddress icdoPersonAddress { get; set; }
        public string istrContactType { get; set; }

        # region override

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            base.BeforeValidate(aenmPageMode);
            icdoPersonContact.PopulateDescriptions();
            if (this.icdoPersonContact.addr_same_as_person == busConstant.Flag_Yes && this.icdoPersonContact.contact_type_value != busConstant.CONTACT_ALTERNATE)
            {
                if (icdoPersonAddress != null)
                {
                    icdoPersonContact.addr_line_1 = icdoPersonAddress.addr_line_1;
                    icdoPersonContact.addr_line_2 = icdoPersonAddress.addr_line_2;
                    icdoPersonContact.addr_city = icdoPersonAddress.addr_city;
                    icdoPersonContact.addr_country_value = icdoPersonAddress.addr_country_value;
                    icdoPersonContact.addr_state_value = icdoPersonAddress.addr_state_value; //PROD PIR 180 : fixed for state value should be update as person address.
                    icdoPersonContact.addr_zip_code = icdoPersonAddress.addr_zip_code;
                    icdoPersonContact.addr_zip_4_code = icdoPersonAddress.addr_zip_4_code;
                    icdoPersonContact.foreign_province = icdoPersonAddress.foreign_province;
                    icdoPersonContact.foreign_postal_code = icdoPersonAddress.foreign_postal_code;
                }
            }
        }

        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            base.ValidateHardErrors(aenmPageMode);
            if (this.icdoPersonContact.addr_country_value == "0001" && !string.IsNullOrEmpty(Convert.ToString(this.icdoPersonContact.addr_zip_code)) && this.icdoPersonContact.addr_zip_code.Length != 5)
            {
                utlError lutlError = new utlError();
                lutlError = AddError(1171, "");
                this.iarrErrors.Add(lutlError);
            }
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            // OPUS data push to Health Eligibility for any new Address     //Commented - Rohan Code For data Push to HEDB  (Do not delete this)

           
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
            //    try
            //    {

            //        string strQuery = "select * from PersonAddress where ssn = (select ssn from Eligibility_PID_Reference where PID = '" + this.ibusPerson.icdoPerson.mpi_person_id + "')";
            //        DataTable ldtbResult = DBFunction.DBSelect(strQuery, lobjPassInfo1.iconFramework);
            //        if (ldtbResult.Rows.Count == 0)
            //        {
            //            return;
            //        }

            //    }
            //    catch
            //    {
            //    }


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
                DateTime ldtAddressStartDate = DateTime.MinValue;


                int lintPersonID = this.icdoPersonContact.person_id;
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
                        if ((lbusPersonContact.icdoPersonContact.contact_type_value == busConstant.PowOfAttr_CONTACT_VAL || lbusPersonContact.icdoPersonContact.contact_type_value == busConstant.Gaurdian_CONTACT_VAL
                            || lbusPersonContact.icdoPersonContact.contact_type_value == busConstant.Conservator_CONTACT_VAL)
                                && lbusPersonContact.icdoPersonContact.correspondence_addr_flag == busConstant.FLAG_YES)
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


                            ldtAddressStartDate = this.icdoPersonContact.effective_start_date;
                        }
                    }

                }

            // decommissioning demographics informations, since HEDB is retiring.
            //if (lbnIsContact)
            //{

            //    lstrAuditUser = iobjPassInfo.istrUserID;
            //    lstrPersonMpiID = this.ibusPerson.icdoPerson.mpi_person_id;

                    lstrAuditUser = iobjPassInfo.istrUserID;
                    lstrPersonMpiID = this.ibusPerson.icdoPerson.mpi_person_id;

            //    Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();

            //    IDbDataParameter lobjParameter1 = DBFunction.GetDBParameter();
            //    lobjParameter1.ParameterName = "@PID";
            //    lobjParameter1.DbType = DbType.String;
            //    lobjParameter1.Value = lstrPersonMpiID.ToLower();
            //    lcolParameters.Add(lobjParameter1);

            //    if (this.iobjPassInfo.ienmPageMode == utlPageMode.New)
            //    {
            //        IDbDataParameter lobjParameter2 = DBFunction.GetDBParameter();
            //        lobjParameter2.ParameterName = "@EntityTypeCode";
            //        lobjParameter2.DbType = DbType.String;
            //        lobjParameter2.Value = "P";
            //        lcolParameters.Add(lobjParameter2);
            //    }

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
            //    lobjParameter12.ParameterName = "@DoNotUpdate";
            //    lobjParameter12.DbType = DbType.String;
            //    lobjParameter12.Value = lintDoNotUpdate;
            //    lcolParameters.Add(lobjParameter12);


            //    IDbDataParameter lobjParameter13 = DBFunction.GetDBParameter();
            //    lobjParameter13.ParameterName = "@AuditUser";
            //    lobjParameter13.DbType = DbType.String;
            //    lobjParameter13.Value = lstrAuditUser;
            //    lcolParameters.Add(lobjParameter13);

            //    IDbDataParameter lobjParameter15 = DBFunction.GetDBParameter();
            //    lobjParameter15.ParameterName = "@AddressStartDate";
            //    lobjParameter15.DbType = DbType.DateTime;
            //    lobjParameter15.Value = ldtAddressStartDate;
            //    lcolParameters.Add(lobjParameter15);

            //    try
            //    {
            //        lobjPassInfo1.BeginTransaction();
            //        if (iobjPassInfo.ienmPageMode == utlPageMode.New)
            //        {
            //            DBFunction.DBExecuteProcedure("USP_PID_PersonAddress_Ins", lcolParameters, lobjPassInfo1.iconFramework, lobjPassInfo1.itrnFramework);
            //        }
            //        else if (iobjPassInfo.ienmPageMode == utlPageMode.Update)
            //        {
            //            DBFunction.DBExecuteProcedure("USP_PID_PersonAddress_UPD", lcolParameters, lobjPassInfo1.iconFramework, lobjPassInfo1.itrnFramework);
            //        }
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

            //}
        }


        public override void BeforePersistChanges()
        {
            base.BeforePersistChanges();
            if (this.icdoPersonContact.addr_same_as_person == busConstant.FLAG_YES)
            {
                this.icdoPersonContact.correspondence_addr_flag = busConstant.FLAG_NO;
            }
            if (this.icdoPersonContact.contact_type_value == busConstant.CONTACT_ALTERNATE)
            {
                this.icdoPersonContact.status_value = busConstant.STATUS_VALID;
            }
            else if ((this.icdoPersonContact.effective_end_date == DateTime.MinValue || this.icdoPersonContact.effective_end_date > DateTime.Today) && this.icdoPersonContact.status_value != busConstant.STATUS_VALID) 
            {
                this.icdoPersonContact.status_value = busConstant.STATUS_REVIEW;
            }
        }

        public bool ActiveAttorneyExists()
        {
           
            int lintCount = 0;
            if (this.icdoPersonContact.effective_end_date == DateTime.MinValue)
            {
                DataTable ldtbCount = Select("cdoPersonContact.GetActiveAttorneyContact", new object[4] { this.icdoPersonContact.person_id, this.icdoPersonContact.effective_start_date, this.icdoPersonContact.person_contact_id, this.icdoPersonContact.attorney_for_value });
                DataRow ldtRow = ldtbCount.Rows[0];
                lintCount = Convert.ToInt32(ldtRow[0]);

            }
            else
            {
                DataTable ldtbCount = Select("cdoPersonContact.GetActAttorneyContactifEndDateNotNull", new object[4] { this.icdoPersonContact.person_id, this.icdoPersonContact.attorney_for_value, this.icdoPersonContact.person_contact_id, this.icdoPersonContact.effective_end_date });
                DataRow ldtRow = ldtbCount.Rows[0];
                lintCount = Convert.ToInt32(ldtRow[0]);
            }
            if (lintCount == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
            
        }

        public bool CheckActiveContactTypeExists()
        {
            istrContactType = string.Empty;
            bool lblnActiveContact = false;
            DateTime ldtCurrentEnd = this.icdoPersonContact.effective_end_date;
            if (this.icdoPersonContact.effective_start_date != DateTime.MinValue)
            {
                this.ibusPerson.LoadPersonContacts();
                if (this.icdoPersonContact.effective_end_date == DateTime.MinValue)
                {
                    ldtCurrentEnd = DateTime.MaxValue;
                }
                //Guardian 
                if (this.icdoPersonContact.contact_type_value == "GRDN")
                {
                    istrContactType = "Gaurdian/Power of Attorney/Conservator";
                    foreach (busPersonContact lbusPersonContact in this.ibusPerson.iclbPersonContact)
                    {
                        if ((lbusPersonContact.icdoPersonContact.contact_type_value == busConstant.Gaurdian_CONTACT_VAL ||
                            lbusPersonContact.icdoPersonContact.contact_type_value == busConstant.PowOfAttr_CONTACT_VAL ||
                    lbusPersonContact.icdoPersonContact.contact_type_value == busConstant.Conservator_CONTACT_VAL) && (lbusPersonContact.icdoPersonContact.person_contact_id != this.icdoPersonContact.person_contact_id))
                        {
                            DateTime ldtExist = lbusPersonContact.icdoPersonContact.effective_end_date;
                            if (ldtExist == DateTime.MinValue)
                            {
                                ldtExist = DateTime.MaxValue;
                            }
                            if (lbusPersonContact.icdoPersonContact.effective_start_date <= this.icdoPersonContact.effective_start_date &&
                                this.icdoPersonContact.effective_start_date <= ldtExist)
                            {
                                lblnActiveContact = true;
                            }
                            else if (lbusPersonContact.icdoPersonContact.effective_start_date >= this.icdoPersonContact.effective_start_date &&
                                ldtCurrentEnd >= lbusPersonContact.icdoPersonContact.effective_start_date)
                            {
                                lblnActiveContact = true;
                            }
                        }

                    }
                }
                //Pension Contact
                if (this.icdoPersonContact.pension_contact_flag == busConstant.FLAG_YES && icdoPersonContact.contact_type_value != busConstant.CONTACT_ALTERNATE && !lblnActiveContact)
                {
                    istrContactType = busConstant.PENSION_CONTACT_DESC;
                    foreach (busPersonContact lbusPersonContact in this.ibusPerson.iclbPersonContact)
                    {
                        if ((lbusPersonContact.icdoPersonContact.contact_type_value == "GRDN" || lbusPersonContact.icdoPersonContact.pension_contact_flag == busConstant.FLAG_YES && lbusPersonContact.icdoPersonContact.contact_type_value != busConstant.CONTACT_ALTERNATE)
                            && (lbusPersonContact.icdoPersonContact.person_contact_id != this.icdoPersonContact.person_contact_id))
                        {
                            DateTime ldtExist = lbusPersonContact.icdoPersonContact.effective_end_date;
                            if (ldtExist == DateTime.MinValue)
                            {
                                ldtExist = DateTime.MaxValue;
                            }
                            if (lbusPersonContact.icdoPersonContact.effective_start_date <= this.icdoPersonContact.effective_start_date &&
                                this.icdoPersonContact.effective_start_date <= ldtExist)
                            {
                                lblnActiveContact = true;
                                
                            }
                            else if (lbusPersonContact.icdoPersonContact.effective_start_date >= this.icdoPersonContact.effective_start_date &&
                                ldtCurrentEnd >= lbusPersonContact.icdoPersonContact.effective_start_date)
                            {
                                lblnActiveContact = true;
                                
                            }
                        }
                    }
                }
                //Health Contact 
                if (this.icdoPersonContact.health_contact_flag == busConstant.FLAG_YES && this.icdoPersonContact.contact_type_value != busConstant.CONTACT_ALTERNATE && !lblnActiveContact)
                {
                    istrContactType = busConstant.HEALTH_CONTACT_DESC;
                    foreach (busPersonContact lbusPersonContact in this.ibusPerson.iclbPersonContact)
                    {
                        if ((lbusPersonContact.icdoPersonContact.contact_type_value == "GRDN" || lbusPersonContact.icdoPersonContact.health_contact_flag == busConstant.FLAG_YES && lbusPersonContact.icdoPersonContact.contact_type_value != busConstant.CONTACT_ALTERNATE)
                            && (lbusPersonContact.icdoPersonContact.person_contact_id != this.icdoPersonContact.person_contact_id))
                        {
                            DateTime ldtExist = lbusPersonContact.icdoPersonContact.effective_end_date;
                            if (ldtExist == DateTime.MinValue)
                            {
                                ldtExist = DateTime.MaxValue;
                            }
                            if (lbusPersonContact.icdoPersonContact.effective_start_date <= this.icdoPersonContact.effective_start_date &&
                                this.icdoPersonContact.effective_start_date <= ldtExist)
                            {
                                lblnActiveContact = true;
                            }
                            else if (lbusPersonContact.icdoPersonContact.effective_start_date >= this.icdoPersonContact.effective_start_date &&
                                ldtCurrentEnd >= lbusPersonContact.icdoPersonContact.effective_start_date)
                            {
                                lblnActiveContact = true;
                            }
                        }
                    }
                }
                //Court
                if (this.icdoPersonContact.contact_type_value == "CORT")
                {
                    istrContactType = this.icdoPersonContact.contact_type_description;
                    foreach (busPersonContact lbusPersonContact in this.ibusPerson.iclbPersonContact)
                    {
                        if ((lbusPersonContact.icdoPersonContact.contact_type_value == "CORT")
                            && (lbusPersonContact.icdoPersonContact.person_contact_id != this.icdoPersonContact.person_contact_id))
                        {
                            DateTime ldtExist = lbusPersonContact.icdoPersonContact.effective_end_date;
                            if (ldtExist == DateTime.MinValue)
                            {
                                ldtExist = DateTime.MaxValue;
                            }
                            if (lbusPersonContact.icdoPersonContact.effective_start_date <= this.icdoPersonContact.effective_start_date &&
                                this.icdoPersonContact.effective_start_date <= ldtExist)
                            {
                                lblnActiveContact = true;
                            }
                            else if (lbusPersonContact.icdoPersonContact.effective_start_date >= this.icdoPersonContact.effective_start_date &&
                                ldtCurrentEnd >= lbusPersonContact.icdoPersonContact.effective_start_date)
                            {
                                lblnActiveContact = true;
                            }

                            
                        }
                    }
                }
                /// As Discuused , attorney type is manadatory and could either Respondent or Petiotioner and not both : Discussed while foing Cor for Dro.
                
                //Attorney Petitioner 
                //if (this.icdoPersonContact.contact_type_value == "ATRN" && this.icdoPersonContact.attorney_for_value == "PETR")
                //{
                //    istrContactType = this.icdoPersonContact.attorney_for_description;
                //    foreach (busPersonContact lbusPersonContact in this.ibusPerson.iclbPersonContact)
                //    {
                //        if ((lbusPersonContact.icdoPersonContact.contact_type_value == "ATRN" && lbusPersonContact.icdoPersonContact.attorney_for_value == "PETR")
                //            && (lbusPersonContact.icdoPersonContact.person_contact_id != this.icdoPersonContact.person_contact_id))
                //        {
                //            if (lbusPersonContact.icdoPersonContact.effective_start_date < ldtCurrentEnd)
                //            {
                //                lblnActiveContact = true;
                //            }
                //            else
                //            {
                //                if (lbusPersonContact.icdoPersonContact.effective_end_date > this.icdoPersonContact.effective_start_date)
                //                {
                //                    lblnActiveContact = true;
                //                }
                //            }
                //        }
                //    }
                //}
                ////Attorney Respondent
                //if (this.icdoPersonContact.contact_type_value == "ATRN" && this.icdoPersonContact.attorney_for_value == "RESP")
                //{
                //    istrContactType = this.icdoPersonContact.attorney_for_description;
                //    foreach (busPersonContact lbusPersonContact in this.ibusPerson.iclbPersonContact)
                //    {
                //        if ((lbusPersonContact.icdoPersonContact.contact_type_value == "ATRN" && lbusPersonContact.icdoPersonContact.attorney_for_value == "RESP")
                //            && (lbusPersonContact.icdoPersonContact.person_contact_id != this.icdoPersonContact.person_contact_id))
                //        {
                //            if (lbusPersonContact.icdoPersonContact.effective_start_date < ldtCurrentEnd)
                //            {
                //                lblnActiveContact = true;
                //            }
                //            else
                //            {
                //                if (lbusPersonContact.icdoPersonContact.effective_end_date > this.icdoPersonContact.effective_start_date)
                //                {
                //                    lblnActiveContact = true;
                //                }
                //            }
                //        }
                //    }
                //}
                
                if (this.icdoPersonContact.contact_type_value == "ATRN"/* && string.IsNullOrEmpty(this.icdoPersonContact.attorney_for_value)*/)
                {
                    istrContactType = this.icdoPersonContact.contact_type_description;
                    foreach (busPersonContact lbusPersonContact in this.ibusPerson.iclbPersonContact)
                    {
                        if ((lbusPersonContact.icdoPersonContact.contact_type_value == "ATRN" /*&& string.IsNullOrEmpty(lbusPersonContact.icdoPersonContact.attorney_for_value)*/)
                            && (lbusPersonContact.icdoPersonContact.person_contact_id != this.icdoPersonContact.person_contact_id))
                        {
                            if (lbusPersonContact.icdoPersonContact.effective_start_date < ldtCurrentEnd)
                            {
                                lblnActiveContact = true;
                            }
                            else
                            {
                                if (lbusPersonContact.icdoPersonContact.effective_end_date > this.icdoPersonContact.effective_start_date)
                                {
                                    lblnActiveContact = true;
                                }
                            }
                        }
                    }
                }
                //Alternate

                if (this.icdoPersonContact.pension_contact_flag == busConstant.FLAG_YES && this.icdoPersonContact.contact_type_value == busConstant.CONTACT_ALTERNATE)
                {
                    istrContactType =this.icdoPersonContact.contact_type_description+"/" + busConstant.PENSION_CONTACT_DESC;
                    foreach (busPersonContact lbusPersonContact in this.ibusPerson.iclbPersonContact)
                    {
                        if (lbusPersonContact.icdoPersonContact.pension_contact_flag == busConstant.FLAG_YES && lbusPersonContact.icdoPersonContact.contact_type_value == busConstant.CONTACT_ALTERNATE
                            && (lbusPersonContact.icdoPersonContact.person_contact_id != this.icdoPersonContact.person_contact_id))
                        {
                            DateTime ldtExist = lbusPersonContact.icdoPersonContact.effective_end_date;
                            if (ldtExist == DateTime.MinValue)
                            {
                                ldtExist = DateTime.MaxValue;
                            }
                            if (lbusPersonContact.icdoPersonContact.effective_start_date <= this.icdoPersonContact.effective_start_date &&
                                this.icdoPersonContact.effective_start_date <= ldtExist)
                            {
                                lblnActiveContact = true;
                            }
                            else if (lbusPersonContact.icdoPersonContact.effective_start_date >= this.icdoPersonContact.effective_start_date &&
                                ldtCurrentEnd >= lbusPersonContact.icdoPersonContact.effective_start_date)
                            {
                                lblnActiveContact = true;
                            }
                        }
                    }
                }
                //Health Contact 
                if (this.icdoPersonContact.health_contact_flag == busConstant.FLAG_YES &&  this.icdoPersonContact.contact_type_value == busConstant.CONTACT_ALTERNATE)
                {
                    istrContactType = this.icdoPersonContact.contact_type_description + "/" + busConstant.HEALTH_CONTACT_DESC;
                    foreach (busPersonContact lbusPersonContact in this.ibusPerson.iclbPersonContact)
                    {
                        if (lbusPersonContact.icdoPersonContact.health_contact_flag == busConstant.FLAG_YES && lbusPersonContact.icdoPersonContact.contact_type_value == busConstant.CONTACT_ALTERNATE
                            && lbusPersonContact.icdoPersonContact.person_contact_id != this.icdoPersonContact.person_contact_id)
                        {
                            DateTime ldtExist = lbusPersonContact.icdoPersonContact.effective_end_date;
                            if (ldtExist == DateTime.MinValue)
                            {
                                ldtExist = DateTime.MaxValue;
                            }
                            if (lbusPersonContact.icdoPersonContact.effective_start_date <= this.icdoPersonContact.effective_start_date &&
                                this.icdoPersonContact.effective_start_date <= ldtExist)
                            {
                                lblnActiveContact = true;
                            }
                            else if (lbusPersonContact.icdoPersonContact.effective_start_date >= this.icdoPersonContact.effective_start_date &&
                                ldtCurrentEnd >= lbusPersonContact.icdoPersonContact.effective_start_date)
                            {
                                lblnActiveContact = true;
                            }
                        }
                    }
                }
            }
            return lblnActiveContact;
        }

        #endregion


        #region public

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

        public void LoadActiveAddress()
        {

            DataTable ldtbActiveAddress = Select("cdoPersonContact.GetActiveAddress", new object[1] { this.ibusPerson.icdoPerson.person_id });
            if (ldtbActiveAddress.Rows.Count > 0)
            {
                if (ldtbActiveAddress.Rows.Count == 1)
                {
                    icdoPersonAddress = new cdoPersonAddress();
                    icdoPersonAddress.LoadData(ldtbActiveAddress.Rows[0]);
                    LoadStateDescription();
                }
                else
                {
                    foreach (DataRow dtRow in ldtbActiveAddress.Rows)
                    {
                        if (dtRow[enmPersonAddressChklist.address_type_value.ToString()].ToString() == busConstant.MAILING_ADDRESS)
                        {
                            icdoPersonAddress = new cdoPersonAddress();
                            icdoPersonAddress.LoadData(dtRow);
                            LoadStateDescription();
                            break;
                        }

                    }
                }
            }

        }

        public void ApproveContact()
        {
            if (this.icdoPersonContact.status_value == busConstant.STATUS_REVIEW)
            {
                this.icdoPersonContact.status_value = busConstant.STATUS_VALID;
                this.icdoPersonContact.Update();
            }

        }


        public bool IsEmailValid()
        {
            if (!string.IsNullOrEmpty(this.icdoPersonContact.email_address))
            {
                return busGlobalFunctions.IsValidEmail(this.icdoPersonContact.email_address);
            }
            return true;
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

        #endregion
    }
}
