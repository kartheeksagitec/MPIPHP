using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.ExceptionPub;
using System.Data;
using Sagitec.BusinessObjects;
using MPIPHP.BusinessObjects;
using System.Collections.ObjectModel;
using MPIPHP.CustomDataObjects;
using Sagitec.DBUtility;
using MPIPHP.DataObjects;
using System.Threading.Tasks;

namespace MPIPHPJobService
{
    public class busHealthAddrUpdateBatch : busBatchHandler
    {

        private object iobjLock = null;
        int iintCount = busConstant.ZERO_INT;
        int iintTotalCount = busConstant.ZERO_INT;
        private DateTime Start_Date { get; set; }
        public void HealthAddrUpdateBatch()
        {
            busBase lobjBase = new busBase();

            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
            {
                ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
            }
            iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
            utlPassInfo lobjMainPassInfo = iobjPassInfo;

            iobjLock = new object();
            //set start Date
            Start_Date = DateTime.Today.AddDays(1);
            //Start_Date = new DateTime(2013, 01, 09);
            //Initialize the parallel processing options, especially the max number of thread to be used for parallel processing
            ParallelOptions lpoParallelOptions = new ParallelOptions();
            lpoParallelOptions.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 4;
            Collection<busPersonAddress> lclbPersonAddress;

            DataTable ldtblPersonAddress = busBase.Select("cdoPersonAddress.GetPersonAddressForBatch", new object[1] { Start_Date });
            if (ldtblPersonAddress.Rows.Count > 0)
            {
                lclbPersonAddress = new Collection<busPersonAddress>();
                lclbPersonAddress = lobjBase.GetCollection<busPersonAddress>(ldtblPersonAddress, "icdoPersonAddress");

                //foreach (busPersonAddress lPersonAddress in lclbPersonAddress)
                //{
                Parallel.ForEach(lclbPersonAddress, lPersonAddress =>
                {
                    utlPassInfo lobjPassInfo = new utlPassInfo();
                    lobjPassInfo.idictParams = ldictParams;
                    lobjPassInfo.idictParams["ID"] = "HealthAddressUpdateBatch";
                    lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                    utlPassInfo.iobjPassInfo = lobjPassInfo;

                    DataRow dr = (from obj in ldtblPersonAddress.AsEnumerable()
                                  where obj.Field<int>(enmPerson.person_id.ToString()) == lPersonAddress.icdoPersonAddress.person_id
                                  select obj).FirstOrDefault();
                    if (dr != null)
                    {
                        busPerson lbusPerson = new busPerson { icdoPerson = new cdoPerson() };
                        lbusPerson.icdoPerson.LoadData(dr);

                       // UpadteAddress(lPersonAddress, lbusPerson, lobjPassInfo);
                    }

                    if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                    {
                        lobjPassInfo.iconFramework.Close();
                    }

                    lobjPassInfo.iconFramework.Dispose();
                    lobjPassInfo.iconFramework = null;
                });
                //}

                lobjMainPassInfo.iconFramework = DBFunction.GetDBConnection();
                utlPassInfo.iobjPassInfo = lobjMainPassInfo;
            }





        }

        //public void UpadteAddress(busPersonAddress lbusPersonAddress, busPerson lbusPerson, utlPassInfo autlPassInfo)
        //{
        //    IDbConnection lconHELegacy = null;

        //    if (lconHELegacy == null)
        //    {
        //        lconHELegacy = DBFunction.GetDBConnection("HELegacy");
        //    }

        //    lock (iobjLock)
        //    {
        //        iintCount++;
        //        iintTotalCount++;
        //        if (iintCount == 10)
        //        {
        //            String lstrMsg = iintTotalCount + " : " + " Records Has Been Processed";
        //            PostInfoMessage(lstrMsg);
        //            iintCount = 0;
        //        }
        //    }
        //    String lstrMsg1 = "";
        //    autlPassInfo.BeginTransaction();
        //    try
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
        //        DateTime ldtAddressStartDate = DateTime.MinValue;

        //        int lintPersonID = lbusPersonAddress.icdoPersonAddress.person_id;
        //        lbusPerson.iclbPersonContact = new Collection<busPersonContact>();

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
        //                    lstrCountryValue = HelperUtil.GetData1ByCodeValue(lbusPersonContact.icdoPersonContact.addr_country_id, lbusPersonContact.icdoPersonContact.addr_country_value);

        //                    if (Convert.ToInt32(lbusPersonContact.icdoPersonContact.addr_country_value) == busConstant.USA)
        //                    {
        //                        lintForeignAddrFlag = 0;
        //                    }
        //                    else
        //                    {
        //                        lintForeignAddrFlag = 1;
        //                    }


        //                    ldtAddressStartDate = lbusPersonContact.icdoPersonContact.effective_start_date;
        //                    lstrAuditUser = iobjPassInfo.istrUserID;
        //                    if (lbusPersonContact.icdoPersonAddress.end_date.IsNotNull() && lbusPersonContact.icdoPersonAddress.end_date != DateTime.MinValue)
        //                        ldtAddressEndDate = lbusPersonContact.icdoPersonAddress.end_date;
        //                    else
        //                        ldtAddressEndDate = DateTime.MinValue;
        //                }
        //            }

        //        }


        //        if (!lbnIsContact)
        //        {
        //            lstrAddrline1 = lbusPersonAddress.icdoPersonAddress.addr_line_1;
        //            lstrAddrline2 = lbusPersonAddress.icdoPersonAddress.addr_line_2;
        //            lstrCity = lbusPersonAddress.icdoPersonAddress.addr_city;

        //            if (Convert.ToInt32(lbusPersonAddress.icdoPersonAddress.addr_country_value) == busConstant.USA || Convert.ToInt32(lbusPersonAddress.icdoPersonAddress.addr_country_value) == busConstant.AUSTRALIA
        //                || Convert.ToInt32(lbusPersonAddress.icdoPersonAddress.addr_country_value) == busConstant.CANADA || Convert.ToInt32(lbusPersonAddress.icdoPersonAddress.addr_country_value) == busConstant.MEXICO
        //                || Convert.ToInt32(lbusPersonAddress.icdoPersonAddress.addr_country_value) == busConstant.NewZealand)
        //            {
        //                lstrState = lbusPersonAddress.icdoPersonAddress.addr_state_value;
        //            }
        //            else
        //            {
        //                lstrState = lbusPersonAddress.icdoPersonAddress.foreign_province;
        //            }

        //            if (Convert.ToInt32(lbusPersonAddress.icdoPersonAddress.addr_country_value) == busConstant.USA)
        //            {

        //                lstrPostalCode = lbusPersonAddress.icdoPersonAddress.addr_zip_code + lbusPersonAddress.icdoPersonAddress.addr_zip_4_code;
        //            }
        //            else
        //            {
        //                lstrPostalCode = lbusPersonAddress.icdoPersonAddress.foreign_postal_code;
        //            }

        //            lstrCountry = lbusPersonAddress.icdoPersonAddress.addr_country_description;

        //            if (lbusPersonAddress.icdoPersonAddress.addr_country_value != null)
        //                lstrCountryValue = HelperUtil.GetData1ByCodeValue(lbusPersonAddress.icdoPersonAddress.addr_country_id, lbusPersonAddress.icdoPersonAddress.addr_country_value);

        //            if (Convert.ToInt32(lbusPersonAddress.icdoPersonAddress.addr_country_value) == busConstant.USA)
        //            {
        //                lintForeignAddrFlag = 0;
        //            }
        //            else
        //            {
        //                lintForeignAddrFlag = 1;
        //            }

        //            if (lbusPersonAddress.icdoPersonAddress.secured_flag == "Y")
        //            {
        //                lintDoNotUpdate = 1;
        //            }
        //            else
        //            {
        //                lintDoNotUpdate = 0;
        //            }

        //            ldtAddressStartDate = lbusPersonAddress.icdoPersonAddress.start_date;

        //            lstrAuditUser = iobjPassInfo.istrUserID;
        //            if (lbusPersonAddress.icdoPersonAddress.end_date.IsNotNull() && lbusPersonAddress.icdoPersonAddress.end_date != DateTime.MinValue)
        //                ldtAddressEndDate = lbusPersonAddress.icdoPersonAddress.end_date;
        //            else
        //                ldtAddressEndDate = DateTime.MinValue;
        //        }

        //        //On EADB
        //        DataTable ldtbSysMgmt = iobjPassInfo.isrvDBCache.GetSystemManagement();
        //        //if (ldtbSysMgmt.Rows.Count > 0 && ldtbSysMgmt.Rows[0]["REGION_VALUE"].ToString() == "DEVL")
        //        //{
        //        //    return;
        //        //}



        //        //try
        //        //{

        //        //    string strQuery = "select * from PersonAddress where ssn = (select ssn from Eligibility_PID_Reference where PID = '" + lbusPerson.icdoPerson.mpi_person_id + "')";
        //        //    DataTable ldtbResult = DBFunction.DBSelect(strQuery, lconHELegacy);
        //        //    if (ldtbResult.Rows.Count == 0)
        //        //    {
        //        //        return;
        //        //    }
        //        //}
        //        //catch
        //        //{
        //        //}

        //        lstrPersonMpiID = lbusPerson.icdoPerson.mpi_person_id;


        //        Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();

        //        IDbDataParameter lobjParameter1 = DBFunction.GetDBParameter();
        //        lobjParameter1.ParameterName = "@PID";
        //        lobjParameter1.DbType = DbType.String;
        //        lobjParameter1.Value = lstrPersonMpiID.ToLower();
        //        lcolParameters.Add(lobjParameter1);

        //        //IDbDataParameter lobjParameter2 = DBFunction.GetDBParameter();
        //        //lobjParameter2.ParameterName = "@EntityTypeCode";
        //        //lobjParameter2.DbType = DbType.String;
        //        //lobjParameter2.Value = "P";
        //        //lcolParameters.Add(lobjParameter2);

        //        if (lbnIsContact)
        //        {
        //            IDbDataParameter lobjParameter3 = DBFunction.GetDBParameter();
        //            lobjParameter3.ParameterName = "@Attention";
        //            lobjParameter3.DbType = DbType.String;
        //            lobjParameter3.Value = lstrContactName;
        //            lcolParameters.Add(lobjParameter3);

        //        }

        //        IDbDataParameter lobjParameter4 = DBFunction.GetDBParameter();
        //        lobjParameter4.ParameterName = "@Address1";
        //        lobjParameter4.DbType = DbType.String;
        //        lobjParameter4.Value = lstrAddrline1;
        //        lcolParameters.Add(lobjParameter4);

        //        IDbDataParameter lobjParameter5 = DBFunction.GetDBParameter();
        //        lobjParameter5.ParameterName = "@Address2";
        //        lobjParameter5.DbType = DbType.String;
        //        lobjParameter5.Value = lstrAddrline2;
        //        lcolParameters.Add(lobjParameter5);

        //        IDbDataParameter lobjParameter6 = DBFunction.GetDBParameter();
        //        lobjParameter6.ParameterName = "@City";
        //        lobjParameter6.DbType = DbType.String;
        //        lobjParameter6.Value = lstrCity;
        //        lcolParameters.Add(lobjParameter6);

        //        IDbDataParameter lobjParameter7 = DBFunction.GetDBParameter();
        //        lobjParameter7.ParameterName = "@State";
        //        lobjParameter7.DbType = DbType.String;
        //        lobjParameter7.Value = lstrState;
        //        lcolParameters.Add(lobjParameter7);

        //        IDbDataParameter lobjParameter8 = DBFunction.GetDBParameter();
        //        lobjParameter8.ParameterName = "@PostalCode";
        //        lobjParameter8.DbType = DbType.String;
        //        lobjParameter8.Value = lstrPostalCode;
        //        lcolParameters.Add(lobjParameter8);

        //        IDbDataParameter lobjParameter9 = DBFunction.GetDBParameter();
        //        lobjParameter9.ParameterName = "@Country";
        //        lobjParameter9.DbType = DbType.String;
        //        lobjParameter9.Value = lstrCountry;
        //        lcolParameters.Add(lobjParameter9);

        //        IDbDataParameter lobjParameter10 = DBFunction.GetDBParameter();
        //        lobjParameter10.ParameterName = "@CountryCode";
        //        lobjParameter10.DbType = DbType.String;
        //        lobjParameter10.Value = lstrCountryValue;
        //        lcolParameters.Add(lobjParameter10);

        //        IDbDataParameter lobjParameter11 = DBFunction.GetDBParameter();
        //        lobjParameter11.ParameterName = "@ForeignAddr";
        //        lobjParameter11.DbType = DbType.String;
        //        lobjParameter11.Value = lintForeignAddrFlag;
        //        lcolParameters.Add(lobjParameter11);


        //        IDbDataParameter lobjParameter12 = DBFunction.GetDBParameter();
        //        lobjParameter12.ParameterName = "@ReturnedMail";
        //        lobjParameter12.DbType = DbType.DateTime;
        //        if (ldtAddressEndDate == DateTime.MinValue)
        //            lobjParameter12.Value = DBNull.Value;
        //        else
        //            lobjParameter12.Value = ldtAddressEndDate;
        //        lcolParameters.Add(lobjParameter12);

        //        if (!lbnIsContact)
        //        {
        //            IDbDataParameter lobjParameter13 = DBFunction.GetDBParameter();
        //            lobjParameter13.ParameterName = "@DoNotUpdate";
        //            lobjParameter13.DbType = DbType.String;
        //            lobjParameter13.Value = lintDoNotUpdate;
        //            lcolParameters.Add(lobjParameter13);
        //        }

        //        IDbDataParameter lobjParameter14 = DBFunction.GetDBParameter();
        //        lobjParameter14.ParameterName = "@AuditUser";
        //        lobjParameter14.DbType = DbType.String;
        //        lobjParameter14.Value = lstrAuditUser;
        //        lcolParameters.Add(lobjParameter14);


        //        IDbDataParameter lobjParameter15 = DBFunction.GetDBParameter();
        //        lobjParameter15.ParameterName = "@AddressStartDate";
        //        lobjParameter15.DbType = DbType.DateTime;
        //        lobjParameter15.Value = ldtAddressStartDate;
        //        lcolParameters.Add(lobjParameter15);


        //        DBFunction.DBExecuteProcedure("USP_PID_PersonAddress_UPD", lcolParameters, lconHELegacy, null);
        //        lconHELegacy.Close();
        //        lstrMsg1 = lbusPerson.icdoPerson.mpi_person_id + " : " + " is  processed";
        //        PostInfoMessage(lstrMsg1);


        //        autlPassInfo.Commit();
        //    }
        //    catch (Exception e)
        //    {
        //        lock (iobjLock)
        //        {
        //            ExceptionManager.Publish(e);
        //            String lstrMsg = "Error while Executing Batch,Error Message For MPID " + lbusPerson.icdoPerson.mpi_person_id + e.ToString();
        //            PostErrorMessage(lstrMsg);


        //            if (lconHELegacy.State == ConnectionState.Open)
        //            {
        //                lconHELegacy.Close();
        //            }
        //        }
        //        autlPassInfo.Rollback();

        //    }

        //}






    }
}
