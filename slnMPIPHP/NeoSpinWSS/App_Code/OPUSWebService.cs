using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;
using System.Collections;
using System.Configuration;
using Sagitec.Interface;
using MPIPHP.DataObjects;
using Sagitec.WebClient;
using System.ServiceModel.Activation;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using System.Web;
using System.IO;
using MPIPHP.BusinessObjects;
using MPIPHP.CustomDataObjects;
using Sagitec.DBUtility;



public class OPUSWebService : IOPUSWebService
{
     //ROHAN
    public bool IsSSNValid(string istrPrefix, string istrFirstName, string istrLastName, string istrMiddleName, string istrSuffix, string istrDateofBirth,string istrSSN)     // New Participant from EA(If SSN is Valid new Person Is inserted in OPUS DB)
    {
        IBusinessTier isrvBusinessTier = null;
        IMetaDataCache isrvMetaDataCache = null;
        IDBCache isrvDBCache = null;
        try
        {
            Configuration MPIWebConfig =System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            if (MPIWebConfig.AppSettings.Settings.Count > 0)
            {
                KeyValueConfigurationElement WebConfigAppSetting = MPIWebConfig.AppSettings.Settings["AppSettingsLocation"];
                HelperFunction.istrAppSettingsLocation = WebConfigAppSetting.Value;
            }

            string strBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson"); 
            string strMetaDataUrl = MPIPHP.Common.ApplicationSettings.Instance.MetaDataCacheUrl;
            string strDBCacheUrl = MPIPHP.Common.ApplicationSettings.Instance.DBCacheUrl;

            //FM upgrade changes - Remoting to WCF
            //IBusinessTier isrvBusinessTier = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), strBusinessTierUrl);
            //IMetaDataCache isrvMetaDataCache = (IMetaDataCache)Activator.GetObject(typeof(IMetaDataCache), strMetaDataUrl);
            //IDBCache isrvDBCache = (IDBCache)Activator.GetObject(typeof(IDBCache), strDBCacheUrl);
            isrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(strBusinessTierUrl);
            isrvMetaDataCache = WCFClient<IMetaDataCache>.CreateChannel(strMetaDataUrl);
            isrvDBCache = WCFClient<IDBCache>.CreateChannel(strDBCacheUrl);

            utlPassInfo iobjPassInfo = new utlPassInfo();
            utlPassInfo.iobjPassInfo = iobjPassInfo;
            iobjPassInfo.isrvMetaDataCache = isrvMetaDataCache;
            
            Hashtable lhstParam = new Hashtable();

            lhstParam.Add("astrPrefix", istrPrefix);
            lhstParam.Add("astrFirstName", istrFirstName);
            lhstParam.Add("astrMiddleName", istrMiddleName);
            lhstParam.Add("astrLastName", istrLastName);
            lhstParam.Add("astrSuffix", istrSuffix);
            lhstParam.Add("astrDateofBirth", istrDateofBirth);
            lhstParam.Add("astrSSN", istrSSN);

            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            ldictParams[utlConstants.istrConstUserID] = "WebService";

            int lcount = 0;
            if (!string.IsNullOrEmpty(istrSSN))
            {
                lcount = (int)isrvBusinessTier.DBExecuteScalar("cdoPerson.GetSSNCount", new object[1] { istrSSN }, ldictParams);     

            }
            if (lcount > 0)
            {
                return false;
            }
            else
            {
               // isrvBusinessTier.ExecuteMethod("InsertPersonRecordFromOPUSWebService", lhstParam, true, null);            //Replaced with Stored Procedure to Insert new Person in OPUS DB
                return true;
            }

        }
        catch
        {
            return false;
        }
        finally
        {
            HelperFunction.CloseChannel(isrvBusinessTier);
            HelperFunction.CloseChannel(isrvMetaDataCache);
            HelperFunction.CloseChannel(isrvDBCache);
        }
    }

    public string ReteiveMPIDFromOPUS(string astrSSN)
    {
        string lstrMPID = string.Empty;
        IBusinessTier isrvBusinessTier = null;
        IMetaDataCache isrvMetaDataCache = null;
        try
        {
            Configuration MPIWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            if (MPIWebConfig.AppSettings.Settings.Count > 0)
            {
                KeyValueConfigurationElement WebConfigAppSetting = MPIWebConfig.AppSettings.Settings["AppSettingsLocation"];
                HelperFunction.istrAppSettingsLocation = WebConfigAppSetting.Value;
            }

            KeyValueConfigurationElement WebConfigBusinessTierUrl = MPIWebConfig.AppSettings.Settings["BusinessTierUrl"];
            string strBusinessTierUrl = WebConfigBusinessTierUrl.Value + "srvPerson";
            string strMetaDataUrl = MPIPHP.Common.ApplicationSettings.Instance.MetaDataCacheUrl;
            string strDBCacheUrl = MPIPHP.Common.ApplicationSettings.Instance.DBCacheUrl;

            //FM upgrade changes - Remoting to WCF
            //IBusinessTier isrvBusinessTier = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), strBusinessTierUrl);
            //IMetaDataCache isrvMetaDataCache = (IMetaDataCache)Activator.GetObject(typeof(IMetaDataCache), strMetaDataUrl);
            isrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(strBusinessTierUrl);
            isrvMetaDataCache = WCFClient<IMetaDataCache>.CreateChannel(strMetaDataUrl);

            utlPassInfo iobjPassInfo = new utlPassInfo();
            utlPassInfo.iobjPassInfo = iobjPassInfo;
            iobjPassInfo.isrvMetaDataCache = isrvMetaDataCache;

            Hashtable lhstParam = new Hashtable();
            lhstParam.Add("astrSSN", astrSSN);
            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            ldictParams[utlConstants.istrConstUserID] = "WebService";

            if (!string.IsNullOrEmpty(astrSSN))
            {
                DataTable ldtPersonID = new DataTable();

                lstrMPID = Convert.ToString(isrvBusinessTier.ExecuteMethod("GetMPIDFromSSN", lhstParam, true, ldictParams)); 
     
            }
            else
            {
                lstrMPID = "Please Enter SSN";
            }

        }
        catch
        {
            return "Exception";
        }
        finally
        {
            HelperFunction.CloseChannel(isrvBusinessTier);
            HelperFunction.CloseChannel(isrvMetaDataCache);
        }
        return lstrMPID;
    }

    public bool AddUpdatePersonAddress(string astrSSN, string astrAddressLine1, string astrAddressLine2, string astrCity, string astrState, string astrZipCode, string astrZipCode4, string astrCountryCode,
        string astrAddressType, string astrAddressEndDate)          //Add/Update Address in OPUS from EA / NCO / ACS
    {
        IBusinessTier isrvBusinessTier = null;
        IMetaDataCache isrvMetaDataCache = null;
        IDBCache isrvDBCache = null;
        try
        {

            if ((!string.IsNullOrEmpty(astrCountryCode) && astrCountryCode.Length > 4) 
                || (!string.IsNullOrEmpty(astrAddressType) && astrAddressType.Length > 4) || (!string.IsNullOrEmpty(astrZipCode) &&  astrZipCode.Length > 5)
                || (!string.IsNullOrEmpty(astrZipCode4) && astrZipCode4.Length > 4))
            {
                return false;
            }

            int aintPersonID = 0;
            Configuration MPIWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            if (MPIWebConfig.AppSettings.Settings.Count > 0)
            {
                KeyValueConfigurationElement WebConfigAppSetting = MPIWebConfig.AppSettings.Settings["AppSettingsLocation"];
                HelperFunction.istrAppSettingsLocation = WebConfigAppSetting.Value;
            }

            string strBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
            string strMetaDataUrl = MPIPHP.Common.ApplicationSettings.Instance.MetaDataCacheUrl;
            string strDBCacheUrl = MPIPHP.Common.ApplicationSettings.Instance.DBCacheUrl;

            //FM upgrade changes - Remoting to WCF
            //IBusinessTier isrvBusinessTier = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), strBusinessTierUrl);
            //IMetaDataCache isrvMetaDataCache = (IMetaDataCache)Activator.GetObject(typeof(IMetaDataCache), strMetaDataUrl);
            //IDBCache isrvDBCache = (IDBCache)Activator.GetObject(typeof(IDBCache), strDBCacheUrl);
            isrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(strBusinessTierUrl);
            isrvMetaDataCache = WCFClient<IMetaDataCache>.CreateChannel(strMetaDataUrl);
            isrvDBCache = WCFClient<IDBCache>.CreateChannel(strDBCacheUrl);

            utlPassInfo iobjPassInfo = new utlPassInfo();
            utlPassInfo.iobjPassInfo = iobjPassInfo;

            iobjPassInfo.isrvMetaDataCache = isrvMetaDataCache;

            Hashtable lhstParam = new Hashtable();
            lhstParam.Add(enmPerson.ssn.ToString().ToUpper(), astrSSN);

            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            ldictParams[utlConstants.istrConstUserID] = "WebService";

            //if (!string.IsNullOrEmpty(astrSSN))
            //{
            //    DataTable ldtPersonID = isrvBusinessTier.ExecuteQuery("cdoPerson.GetPersonIDFromSSN", lhstParam, ldictParams);
            //    aintPersonID = Convert.ToInt32(ldtPersonID.Rows[0][0]);
            //}

            lhstParam.Clear();

            lhstParam.Add("astrSSN", astrSSN);
            lhstParam.Add("astrAddressLine1", astrAddressLine1);
            lhstParam.Add("astrAddressLine2", astrAddressLine2);
            lhstParam.Add("astrCity", astrCity);
            lhstParam.Add("astrState", astrState);
            lhstParam.Add("astrZipCode", astrZipCode);
            lhstParam.Add("astrZipCode4", astrZipCode4);
            lhstParam.Add("astrCountryCode",astrCountryCode);
            lhstParam.Add("astrAddressType",astrAddressType);
            lhstParam.Add("astrAddressEndDate", astrAddressEndDate);

            if ( astrSSN != string.Empty)
            {
                isrvBusinessTier.ExecuteMethod("AddUpdatePersonAddressFromOPUSService", lhstParam, true, ldictParams);
                return true;
            }
            else
            {
                return false;
            }

        }
        catch
        {
            return false;
        }
        finally
        {
            HelperFunction.CloseChannel(isrvBusinessTier);
            HelperFunction.CloseChannel(isrvMetaDataCache);
            HelperFunction.CloseChannel(isrvDBCache);
        }
    }

    public string GetPersonInformation(string astrSSN)      //Health Eligibility data pull for New Participant
    {
        IBusinessTier isrvBusinessTier = null;
        IMetaDataCache isrvMetaDataCache = null;
        IDBCache isrvDBCache = null;
        try
        {
            int aintPersonID = 0;
            Configuration MPIWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            if (MPIWebConfig.AppSettings.Settings.Count > 0)
            {
                KeyValueConfigurationElement WebConfigAppSetting = MPIWebConfig.AppSettings.Settings["AppSettingsLocation"];
                HelperFunction.istrAppSettingsLocation = WebConfigAppSetting.Value;
            }

            string strBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
            string strMetaDataUrl = MPIPHP.Common.ApplicationSettings.Instance.MetaDataCacheUrl;
            string strDBCacheUrl = MPIPHP.Common.ApplicationSettings.Instance.DBCacheUrl;

            //FM upgrade changes - Remoting to WCF
            //IBusinessTier isrvBusinessTier = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), strBusinessTierUrl);
            //IMetaDataCache isrvMetaDataCache = (IMetaDataCache)Activator.GetObject(typeof(IMetaDataCache), strMetaDataUrl);
            //IDBCache isrvDBCache = (IDBCache)Activator.GetObject(typeof(IDBCache), strDBCacheUrl);
            isrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(strBusinessTierUrl);
            isrvMetaDataCache = WCFClient<IMetaDataCache>.CreateChannel(strMetaDataUrl);
            isrvDBCache = WCFClient<IDBCache>.CreateChannel(strDBCacheUrl);

            utlPassInfo iobjPassInfo = new utlPassInfo();
            utlPassInfo.iobjPassInfo = iobjPassInfo;
            iobjPassInfo.isrvMetaDataCache = isrvMetaDataCache;

            Hashtable lhstParam = new Hashtable();
            lhstParam.Add(enmPerson.ssn.ToString().ToUpper(), astrSSN);


            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            ldictParams[utlConstants.istrConstUserID] = "WebService";

            if (!string.IsNullOrEmpty(astrSSN))
            {
                DataTable ldtPersonID = isrvBusinessTier.ExecuteQuery("cdoPerson.GetPersonIDFromSSN", lhstParam, ldictParams);
                aintPersonID = Convert.ToInt32(ldtPersonID.Rows[0][0]);

            }

            lhstParam.Clear();

            lhstParam.Add("aintPersonID", aintPersonID);
            
            if (aintPersonID > 0)
            {

                object strTemp = isrvBusinessTier.ExecuteMethod("GetPersonInformationFromOPUSService", lhstParam, true, ldictParams);
                busPerson lbusPerson = strTemp as busPerson;
                return enmPerson.last_name + ":" + lbusPerson.icdoPerson.last_name + ";" + enmPerson.first_name + ":" + lbusPerson.icdoPerson.first_name; //Need To Change The Return Type
                    
            }
            else
            {
                return "Person does not exist";
            }

        }
        catch
        {
            return "exception";
        }
        finally
        {
            HelperFunction.CloseChannel(isrvBusinessTier);
            HelperFunction.CloseChannel(isrvMetaDataCache);
            HelperFunction.CloseChannel(isrvDBCache);
        }

    }

}
