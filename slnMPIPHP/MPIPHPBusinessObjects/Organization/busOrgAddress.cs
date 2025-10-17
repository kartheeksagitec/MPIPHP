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
using Sagitec.CustomDataObjects;
using Sagitec.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busOrgAddress:
    /// Inherited from busOrgAddressGen, the class is used to customize the business object busOrgAddressGen.
    /// </summary>
    [Serializable]
    public class busOrgAddress : busOrgAddressGen
    {
        public busOrganization ibusOrganization { get; set; }
        public string IsReturnFlag { get; set; }

        public bool iblnIsUpdateEndDate = false;
        public cdoOrgAddress iobjcdoOrgAddress { get; set; }
        public override long iintPrimaryKey
        {
            get
            {
                long LintPrimaryKey = 0;
                if (iobjPassInfo?.idictParams != null && iobjPassInfo.istrSenderID == "btnSave" && this.icdoOrgAddress.ihstOldValues.ContainsKey("org_address_id") && Convert.ToInt32(this.icdoOrgAddress.ihstOldValues["org_address_id"]) != 0)
                {
                    return LintPrimaryKey = Convert.ToInt32(this.icdoOrgAddress.ihstOldValues["org_address_id"]);
                }
                else { return icdoOrgAddress.iintPrimaryKey; }
            }
        }

        //Ticket#71828
        public override void BeforePersistChanges()
        {
            base.BeforePersistChanges();


            iblnIsUpdateEndDate = busConstant.BOOL_FALSE;
            if (iobjPassInfo.ienmPageMode == utlPageMode.Update && icdoOrgAddress.end_date == DateTime.MinValue)
            {
                if (Convert.ToDateTime(icdoOrgAddress.start_date.Date) < Convert.ToDateTime(DateTime.Now.Date))
                {
                    LoadCurrentAddressValues();
                    UpdateOldAddressWithExisting();
                    icdoOrgAddress.end_date = DateTime.Now.AddDays(-1);
                    iblnIsUpdateEndDate = busConstant.BOOL_TRUE;
                }
            }


           

        }
        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();


            //DataTable ldtbSysMgmt = iobjPassInfo.isrvDBCache.GetSystemManagement();
            //if (ldtbSysMgmt.Rows.Count > 0 && ldtbSysMgmt.Rows[0]["REGION_VALUE"].ToString() == "DEVL")
            //{
            //    return;
            //}


            //Ticket#71828
            if (iobjPassInfo.ienmPageMode == utlPageMode.Update && iblnIsUpdateEndDate)
            {
                //Insert new address when update existing
                UpdateNewAddressWithExisting();
                icdoOrgAddress.Insert();
            }
            //Ticket#71828
            if (!iblnIsUpdateEndDate)
                UpdateEndDateInExistAdd();

            if ((iobjPassInfo.iconFramework).Database == "MPI" || (iobjPassInfo.iconFramework).Database == "MPIPHP")
            {
                return;
            }

            // OPUS data push to Health Eligibility for any new Address     //Commented - Rohan Code For data Push to HEDB  (Do not delete this)


            //utlPassInfo lobjPassInfo1 = new utlPassInfo();
            //lobjPassInfo1.idictParams["ID"] = "OPUS_INTEGRATION";
            //lobjPassInfo1.iconFramework = DBFunction.GetDBConnection("HELegacy");

            //if (lobjPassInfo1.iconFramework != null)
            //{

            //    string lstrContactName = string.Empty;
            //    string lstrAddrline1 = string.Empty;
            //    string lstrAddrline2 = string.Empty;
            //    string lstrCity = string.Empty;
            //    string lstrState = string.Empty;
            //    string lstrPostalCode = string.Empty;
            //    string lstrCountry = string.Empty;
            //    string lstrCountryValue = string.Empty;
            //    int lintForeignAddrFlag = 0;
            //    int lintDoNotUpdate = 0;
            //    string lstrAuditUser = string.Empty;
            //    string lstrOrgMpiID = string.Empty;
            //    string lstrEntityTypeCode = string.Empty;
            //    DateTime ldtAddressStartDate = new DateTime();


            //    int lintOrgID = this.icdoOrgAddress.org_id;
            //    string strQuery = "select * from Organization where TaxID = (select ssn from Eligibility_PID_Reference where PID = '" + this.ibusOrganization.icdoOrganization.mpi_org_id + "')";
            //    try
            //    {
            //        DataTable ldtbResult = DBFunction.DBSelect(strQuery, lobjPassInfo1.iconFramework);
            //        if (ldtbResult.Rows.Count == 0)
            //        {
            //            return;
            //        }
            //    }
            //    catch
            //    {
            //    }

            //    if (ibusOrganization.icdoOrganization.org_type_value == "TRST" || ibusOrganization.icdoOrganization.org_type_value == "MTFA")
            //    {
            //        lstrEntityTypeCode = "T";
            //    }
            //    else
            //    {
            //        lstrEntityTypeCode = "O";
            //    }


            //    lstrAddrline1 = this.icdoOrgAddress.addr_line_1;
            //    lstrAddrline2 = this.icdoOrgAddress.addr_line_2;
            //    lstrCity = this.icdoOrgAddress.city;

            //    if (Convert.ToInt32(this.icdoOrgAddress.country_value) == busConstant.USA)
            //    {
            //        lstrState = this.icdoOrgAddress.state_value;
            //    }
            //    else
            //    {
            //        lstrState = this.icdoOrgAddress.foreign_province;
            //    }

            //    if (Convert.ToInt32(this.icdoOrgAddress.country_value) == busConstant.USA)
            //    {

            //        lstrPostalCode = this.icdoOrgAddress.zip_code + this.icdoOrgAddress.zip_4_code;
            //    }
            //    else
            //    {
            //        lstrPostalCode = this.icdoOrgAddress.foreign_postal_code;
            //    }

            //    lstrCountry = this.icdoOrgAddress.country_description;
            //    if (!string.IsNullOrEmpty(this.icdoOrgAddress.country_value))
            //    {
            //        lstrCountryValue = HelperUtil.GetData1ByCodeValue(this.icdoOrgAddress.org_address_id, this.icdoOrgAddress.country_value);
            //    }

            //    if (Convert.ToInt32(this.icdoOrgAddress.country_value) == busConstant.USA)
            //    {
            //        lintForeignAddrFlag = 0;
            //    }
            //    else
            //    {
            //        lintForeignAddrFlag = 1;
            //    }


            //    lintDoNotUpdate = 0;

            //    lstrAuditUser = iobjPassInfo.istrUserID;
            //    lstrOrgMpiID = this.ibusOrganization.icdoOrganization.mpi_org_id;
            //    ldtAddressStartDate = icdoOrgAddress.start_date;


            //    Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();

            //    IDbDataParameter lobjParameter1 = DBFunction.GetDBParameter();
            //    lobjParameter1.ParameterName = "@PID";
            //    lobjParameter1.DbType = DbType.String;
            //    lobjParameter1.Value = lstrOrgMpiID.ToLower();
            //    lcolParameters.Add(lobjParameter1);

            //    if (this.iobjPassInfo.ienmPageMode == utlPageMode.New)
            //    {
            //        IDbDataParameter lobjParameter2 = DBFunction.GetDBParameter();
            //        lobjParameter2.ParameterName = "@EntityTypeCode";
            //        lobjParameter2.DbType = DbType.String;
            //        lobjParameter2.Value = lstrEntityTypeCode;
            //        lcolParameters.Add(lobjParameter2);
            //    }

            //    IDbDataParameter lobjParameter3 = DBFunction.GetDBParameter();
            //    lobjParameter3.ParameterName = "@Attention";
            //    lobjParameter3.DbType = DbType.String;
            //    lobjParameter3.Value = "0";
            //    lcolParameters.Add(lobjParameter3);

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


            //    IDbDataParameter lobjParameter14 = DBFunction.GetDBParameter();
            //    lobjParameter14.ParameterName = "@AddressStartDate";
            //    lobjParameter14.DbType = DbType.DateTime;
            //    lobjParameter14.Value = ldtAddressStartDate;
            //    lcolParameters.Add(lobjParameter14);

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
            //Commented - Rohan Code For data Push to HEDB
        }
        //Ticket#71828
        private void UpdateOldAddressWithExisting()
        {
            if (!this.icdoOrgAddress.ihstOldValues.IsNullOrEmpty())
            {

                this.icdoOrgAddress.addr_line_1 = Convert.ToString(this.icdoOrgAddress.ihstOldValues["addr_line_1"]);
                this.icdoOrgAddress.addr_line_2 = Convert.ToString(this.icdoOrgAddress.ihstOldValues["addr_line_2"]);
                this.icdoOrgAddress.city = Convert.ToString(this.icdoOrgAddress.ihstOldValues["city"]);
                this.icdoOrgAddress.state_id = Convert.ToInt32(this.icdoOrgAddress.ihstOldValues["state_id"]);
                this.icdoOrgAddress.state_value = Convert.ToString(this.icdoOrgAddress.ihstOldValues["state_value"]);
                this.icdoOrgAddress.country_value = Convert.ToString(this.icdoOrgAddress.ihstOldValues["country_value"]);
                this.icdoOrgAddress.zip_code = Convert.ToString(this.icdoOrgAddress.ihstOldValues["zip_code"]);
                this.icdoOrgAddress.zip_4_code = Convert.ToString(this.icdoOrgAddress.ihstOldValues["zip_4_code"]);
                this.icdoOrgAddress.foreign_province = Convert.ToString(this.icdoOrgAddress.ihstOldValues["foreign_province"]);
                this.icdoOrgAddress.foreign_postal_code = Convert.ToString(this.icdoOrgAddress.ihstOldValues["foreign_postal_code"]);
                this.icdoOrgAddress.start_date = Convert.ToDateTime(this.icdoOrgAddress.ihstOldValues["start_date"]);
                this.icdoOrgAddress.end_date = Convert.ToDateTime(this.icdoOrgAddress.ihstOldValues["end_date"]);
                this.icdoOrgAddress.primary_addr_flag = Convert.ToString(this.icdoOrgAddress.ihstOldValues["primary_addr_flag"]);
                this.icdoOrgAddress.address_type_value = Convert.ToString(this.icdoOrgAddress.ihstOldValues["address_type_value"]);
                this.icdoOrgAddress.org_id = Convert.ToInt32(this.icdoOrgAddress.ihstOldValues["org_id"]);
                this.icdoOrgAddress.state_value = Convert.ToString(this.icdoOrgAddress.ihstOldValues["state_value"]);

                              
            }
        }
        //Ticket#71828
        private void LoadCurrentAddressValues()
        {
            iobjcdoOrgAddress = new cdoOrgAddress();
            iobjcdoOrgAddress.addr_line_1 = this.icdoOrgAddress.addr_line_1;
            iobjcdoOrgAddress.addr_line_2 = this.icdoOrgAddress.addr_line_2;
            iobjcdoOrgAddress.city = this.icdoOrgAddress.city;
            iobjcdoOrgAddress.state_id = this.icdoOrgAddress.state_id;
            iobjcdoOrgAddress.state_value = this.icdoOrgAddress.state_value;
            iobjcdoOrgAddress.country_value = this.icdoOrgAddress.country_value;
            iobjcdoOrgAddress.zip_code = this.icdoOrgAddress.zip_code;
            iobjcdoOrgAddress.zip_4_code = this.icdoOrgAddress.zip_4_code;
            iobjcdoOrgAddress.foreign_province = this.icdoOrgAddress.foreign_province;
            iobjcdoOrgAddress.foreign_postal_code = this.icdoOrgAddress.foreign_postal_code;
            iobjcdoOrgAddress.start_date = DateTime.Now;
            iobjcdoOrgAddress.end_date = DateTime.MinValue;
            iobjcdoOrgAddress.primary_addr_flag = this.icdoOrgAddress.primary_addr_flag;
            iobjcdoOrgAddress.address_type_value = this.icdoOrgAddress.address_type_value;
            iobjcdoOrgAddress.org_id = this.icdoOrgAddress.org_id;
            iobjcdoOrgAddress.state_value = this.icdoOrgAddress.state_value;
        }

        //Ticket#71828
        private void UpdateNewAddressWithExisting()
        {

            this.icdoOrgAddress.addr_line_1 = iobjcdoOrgAddress.addr_line_1;
            this.icdoOrgAddress.addr_line_2 = iobjcdoOrgAddress.addr_line_2;
            this.icdoOrgAddress.city = iobjcdoOrgAddress.city;
            this.icdoOrgAddress.state_id = iobjcdoOrgAddress.state_id;
            this.icdoOrgAddress.state_value = iobjcdoOrgAddress.state_value;
            this.icdoOrgAddress.country_value = iobjcdoOrgAddress.country_value;
            this.icdoOrgAddress.zip_code = iobjcdoOrgAddress.zip_code;
            this.icdoOrgAddress.zip_4_code = iobjcdoOrgAddress.zip_4_code;
            this.icdoOrgAddress.foreign_province = iobjcdoOrgAddress.foreign_province;
            this.icdoOrgAddress.foreign_postal_code = iobjcdoOrgAddress.foreign_postal_code;
            this.icdoOrgAddress.start_date = DateTime.Now.Date; // iobjcdoOrgAddress.start_date;
            this.icdoOrgAddress.end_date = iobjcdoOrgAddress.end_date;
            this.icdoOrgAddress.primary_addr_flag = iobjcdoOrgAddress.primary_addr_flag;
            this.icdoOrgAddress.address_type_id = 6030;
            this.icdoOrgAddress.address_type_value = busConstant.MAIL;
            this.icdoOrgAddress.org_id = iobjcdoOrgAddress.org_id;
            this.icdoOrgAddress.state_value = iobjcdoOrgAddress.state_value;

            if (!this.iobjPassInfo.istrUserID.IsNullOrEmpty())
            {
                this.icdoOrgAddress.created_by = this.iobjPassInfo.istrUserID;
                this.icdoOrgAddress.modified_by = this.iobjPassInfo.istrUserID;
            }
            this.icdoOrgAddress.created_date = DateTime.Now;
            this.icdoOrgAddress.modified_date = DateTime.Now;

          }

        public void UpdateEndDateInExistAdd()
        {

            if (this.ibusOrganization != null && this.ibusOrganization.iclbOrgAddress != null)
            {
                foreach (busOrgAddress lbusOrgAddr in this.ibusOrganization.iclbOrgAddress)
                {
                    
                if (lbusOrgAddr.icdoOrgAddress.org_address_id != this.icdoOrgAddress.org_address_id)
                {
                    if (lbusOrgAddr.icdoOrgAddress.end_date == DateTime.MinValue || lbusOrgAddr.icdoOrgAddress.end_date > this.icdoOrgAddress.start_date)
                    {
                            lbusOrgAddr.icdoOrgAddress.end_date = DateTime.Now.AddDays(-1);
                            lbusOrgAddr.icdoOrgAddress.Update();
                         
                      }

                    }

                }
            }
        }

        public bool CheckDuplicateAddressType()
        {
            DateTime ldtCurrentEnd = this.icdoOrgAddress.end_date;
            if (ldtCurrentEnd == DateTime.MinValue)
            {
                ldtCurrentEnd = DateTime.MaxValue;
            }
            //Safe check 
            if (this.ibusOrganization != null && this.ibusOrganization.iclbOrgAddress != null)
            {
                foreach (busOrgAddress lbusOrgAddr in this.ibusOrganization.iclbOrgAddress)
                {
                    DateTime ldtExist = lbusOrgAddr.icdoOrgAddress.end_date;
                    if (ldtExist == DateTime.MinValue)
                    {
                        ldtExist = DateTime.MaxValue;
                    }
                    if (lbusOrgAddr.icdoOrgAddress.org_address_id != this.icdoOrgAddress.org_address_id)
                    {

                      
                        if (lbusOrgAddr.icdoOrgAddress.end_date != DateTime.MinValue && lbusOrgAddr.icdoOrgAddress.end_date.Date != DateTime.Now.AddDays(-1).Date)
                        {
                            return true;
                        }
                       
                    }
                }
                //Ticket#71828
                if (iobjPassInfo.ienmPageMode == utlPageMode.New)
                {
                    if (this.icdoOrgAddress.start_date.Date < DateTime.Today.Date)
                    {
                        return true;
                    }
                }

            }
            return false;
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
    }
}
