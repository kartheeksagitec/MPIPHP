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
using System.Collections.ObjectModel;
using Newtonsoft.Json;

public class OPUSWebService : IOPUSWebService
{
    #region Properties/Constructor
    private static string istrConfigBusinessTierURL;
    private IDBCache _isrvDBCache;

    /// <summary>
    /// Interface for database cache server
    /// </summary>
    public IDBCache isrvDBCache
    {
        get
        {
            ServiceHelper.iobjTraceInfo.istrTraceHost = "IDBCache.";
            if (_isrvDBCache == null)
            {
                _isrvDBCache = WCFClient<IDBCache>.CreateChannel(istrConfigDBCacheURL);
            }
            else if (((IClientChannel)_isrvDBCache).State == CommunicationState.Closed)
            {
                _isrvDBCache = WCFClient<IDBCache>.CreateChannel(istrConfigDBCacheURL);
            }
            return _isrvDBCache;
        }
    }

    private IMetaDataCache _isrvMetaDataCache;

    /// <summary>
    /// Interface for XML MetaData cache server
    /// </summary>
    public IMetaDataCache isrvMetaDataCache
    {
        get
        {
            ServiceHelper.iobjTraceInfo.istrTraceHost = "IMetaDataCache.";
            if (_isrvMetaDataCache == null)
            {
                _isrvMetaDataCache = WCFClient<IMetaDataCache>.CreateChannel(istrConfigMetaDataCacheURL);
            }
            else if (((IClientChannel)_isrvMetaDataCache).State == CommunicationState.Closed)
            {
                _isrvMetaDataCache = WCFClient<IMetaDataCache>.CreateChannel(istrConfigMetaDataCacheURL);
            }
            return _isrvMetaDataCache;
        }
    }

    private static string istrConfigMetaDataCacheURL;

    /// <summary>
    /// Configuration setting for connecting to DBCache
    /// </summary>
    private static string istrConfigDBCacheURL;
    static OPUSWebService()
    {
        istrConfigBusinessTierURL = ConfigurationManager.AppSettings["BusinessTierUrl"].Split(new char[1] { ';' })[0];
        istrConfigMetaDataCacheURL = String.Format(istrConfigBusinessTierURL, utlConstants.istrMetaDataCache);
        istrConfigDBCacheURL = String.Format(istrConfigBusinessTierURL, utlConstants.istrDBCache);
    }
    #endregion
    //ROHAN
    public bool IsSSNValid(string istrPrefix, string istrFirstName, string istrLastName, string istrMiddleName, string istrSuffix, string istrDateofBirth, string istrSSN)     // New Participant from EA(If SSN is Valid new Person Is inserted in OPUS DB)
    {
        IBusinessTier isrvBusinessTier = null;
        IMetaDataCache isrvMetaDataCache = null;
        IDBCache isrvDBCache = null;
        try
        {
            //Configuration MPIWebConfig =System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            //if (MPIWebConfig.AppSettings.Settings.Count > 0)
            //{
            //    KeyValueConfigurationElement WebConfigAppSetting = MPIWebConfig.AppSettings.Settings["AppSettingsLocation"];
            //    HelperFunction.istrAppSettingsLocation = WebConfigAppSetting.Value;
            //}

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

            //utlPassInfo iobjPassInfo = new utlPassInfo();
            //utlPassInfo.iobjPassInfo = iobjPassInfo;
            //iobjPassInfo.isrvMetaDataCache = isrvMetaDataCache;

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
            //Configuration MPIWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            //if (MPIWebConfig.AppSettings.Settings.Count > 0)
            //{
            //    KeyValueConfigurationElement WebConfigAppSetting = MPIWebConfig.AppSettings.Settings["AppSettingsLocation"];
            //    HelperFunction.istrAppSettingsLocation = WebConfigAppSetting.Value;
            //}

            //KeyValueConfigurationElement WebConfigBusinessTierUrl = MPIWebConfig.AppSettings.Settings["BusinessTierUrl"];
            string strBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
            string strMetaDataUrl = MPIPHP.Common.ApplicationSettings.Instance.MetaDataCacheUrl;
            string strDBCacheUrl = MPIPHP.Common.ApplicationSettings.Instance.DBCacheUrl;

            //FM upgrade changes - Remoting to WCF
            //IBusinessTier isrvBusinessTier = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), strBusinessTierUrl);
            //IMetaDataCache isrvMetaDataCache = (IMetaDataCache)Activator.GetObject(typeof(IMetaDataCache), strMetaDataUrl);
            isrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(strBusinessTierUrl);
            isrvMetaDataCache = WCFClient<IMetaDataCache>.CreateChannel(strMetaDataUrl);

            //utlPassInfo iobjPassInfo = new utlPassInfo();
            //utlPassInfo.iobjPassInfo = iobjPassInfo;
            //iobjPassInfo.isrvMetaDataCache = isrvMetaDataCache;

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
                || (!string.IsNullOrEmpty(astrAddressType) && astrAddressType.Length > 4) || (!string.IsNullOrEmpty(astrZipCode) && astrZipCode.Length > 5)
                || (!string.IsNullOrEmpty(astrZipCode4) && astrZipCode4.Length > 4))
            {
                return false;
            }

            int aintPersonID = 0;
            //Configuration MPIWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            //if (MPIWebConfig.AppSettings.Settings.Count > 0)
            //{
            //    KeyValueConfigurationElement WebConfigAppSetting = MPIWebConfig.AppSettings.Settings["AppSettingsLocation"];
            //    HelperFunction.istrAppSettingsLocation = WebConfigAppSetting.Value;
            //}

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

            //utlPassInfo iobjPassInfo = new utlPassInfo();
            //utlPassInfo.iobjPassInfo = iobjPassInfo;

            //iobjPassInfo.isrvMetaDataCache = isrvMetaDataCache;

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
            lhstParam.Add("astrCountryCode", astrCountryCode);
            lhstParam.Add("astrAddressType", astrAddressType);
            lhstParam.Add("astrAddressEndDate", astrAddressEndDate);

            if (astrSSN != string.Empty)
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
            //Configuration MPIWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            //if (MPIWebConfig.AppSettings.Settings.Count > 0)
            //{
            //    KeyValueConfigurationElement WebConfigAppSetting = MPIWebConfig.AppSettings.Settings["AppSettingsLocation"];
            //    HelperFunction.istrAppSettingsLocation = WebConfigAppSetting.Value;
            //}

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

            //utlPassInfo iobjPassInfo = new utlPassInfo();
            //utlPassInfo.iobjPassInfo = iobjPassInfo;
            //iobjPassInfo.isrvMetaDataCache = isrvMetaDataCache;

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


    #region WCF - Website and Mobile App

    public List<Person> GetPensionIAPSummary(string MPID)
    {
        List<Person> PlanBenefitSummary = new List<Person>();

        IBusinessTier isrvBusinessTierForPerson = null;
        IBusinessTier isrvBusinessTier = null;
        try
        {
            string lstrUrlForPersonId = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvPerson");
            isrvBusinessTierForPerson = WCFClient<IBusinessTier>.CreateChannel(lstrUrlForPersonId);
            string lstrUrl = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvMPIPHPMSS");
            isrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);
            Hashtable lhstParam = new Hashtable();
            lhstParam.Add(enmPerson.mpi_person_id.ToString().ToUpper(), MPID);

            Dictionary<string, object> ldictParamsForPersonId = new Dictionary<string, object>();
            ldictParamsForPersonId[utlConstants.istrConstUserID] = "WebService";

            busPersonOverview lbusPersonOverview = new busPersonOverview();

            if (!string.IsNullOrEmpty(MPID))
            {

                Dictionary<string, object> ldictParams = new Dictionary<string, object>();
                ldictParams[utlConstants.istrConstUserID] = "WebService";
                Hashtable lhstParam1 = new Hashtable();
                lhstParam1.Add("astrMpiPersonId", MPID);

                lbusPersonOverview = (busPersonOverview)isrvBusinessTier.ExecuteMethod("GetPensionIAPSummary", lhstParam1, true, ldictParams);
                if (lbusPersonOverview != null && lbusPersonOverview.iclcdoPersonAccountOverview != null && lbusPersonOverview.iclcdoPersonAccountOverview.Count() > 0)
                {
                    PlanBenefitSummary = lbusPersonOverview.iclcdoPersonAccountOverview
                        .Select(x => new Person { Plan = x.istrPlan, PlanStatus = x.status_description, PensionHours = x.istrTotalHours
                        , QualifiedYears = x.istrTotalQualifiedYears, PensionCredit = x.istrPensionCredit, VestedDate = x.dtVestedDate.Date
                        , HealthHours = x.istrHealthHoursPO, HealthYears = x.istrTotalHealthYearsPO, MonthlyBenefit = x.idecTotalAccruedBenefit
                        , IAPBalance = x.idecSpecialAccountBalance, AllocationAsOfYrEnd = x.istrAllocationEndYear
                        , MPIPersonId = lbusPersonOverview.icdoPerson.mpi_person_id, IsSuccess = true }).ToList<Person>();

                }
                else
                {
                    Person lPerson = new Person();
                    lPerson.IsSuccess = false;
                    lPerson.Comments = "Plan Information does not exists.";
                    PlanBenefitSummary.Add(lPerson);
                }
                return PlanBenefitSummary;

            }
            else
            {
                Person lPerson = new Person();
                lPerson.IsSuccess = false;
                lPerson.Comments = "Please Enter Valid MPID.";
                PlanBenefitSummary.Add(lPerson);
                return PlanBenefitSummary;
            }

        }
        catch (Exception ex)
        {
            Person lPerson = new Person();
            lPerson.IsSuccess = false;
            lPerson.Comments = HelperFunction.istrAppSettingsLocation;
            lPerson.Comments += "Error Processing Record With MPID : " + MPID + ex.Message + "\n" + ex.StackTrace;
            if (ex.InnerException.IsNotNull())
            {
                lPerson.Comments += ex.InnerException.Message + "\n" + ex.InnerException.StackTrace;
            }
            PlanBenefitSummary.Add(lPerson);
            return PlanBenefitSummary;

        }
        finally
        {
            HelperFunction.CloseChannel(isrvBusinessTierForPerson);
            HelperFunction.CloseChannel(isrvBusinessTier);
        }
    }

    public List<Person> GetPlanBenefitSummary(string SSN)
    {
        List<Person> PlanBenefitSummary = new List<Person>();

        IBusinessTier isrvBusinessTierForPerson = null;
        IBusinessTier isrvBusinessTier = null;
        try
        {
            //            Configuration MPIWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            //            if (MPIWebConfig.AppSettings.Settings.Count > 0)
            //            {
            //                KeyValueConfigurationElement WebConfigAppSetting = MPIWebConfig.AppSettings.Settings["AppSettingsLocation"];
            //                HelperFunction.istrAppSettingsLocation = WebConfigAppSetting.Value;
            //            }

            //            KeyValueConfigurationElement WebConfigBusinessTierUrl = MPIWebConfig.AppSettings.Settings["BusinessTierUrl"];
            //FM upgrade changes - Remoting to WCF
            string lstrUrlForPersonId = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvPerson");
            //IBusinessTier isrvBusinessTierForPerson = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), lstrUrlForPersonId);
            isrvBusinessTierForPerson = WCFClient<IBusinessTier>.CreateChannel(lstrUrlForPersonId);
            string lstrUrl = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvMPIPHPMSS");
            //IBusinessTier isrvBusinessTier = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), lstrUrl);
            isrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);

            //            utlPassInfo iobjPassInfo = new utlPassInfo();
            //            utlPassInfo.iobjPassInfo = iobjPassInfo;
            Hashtable lhstParam = new Hashtable();
            lhstParam.Add(enmPerson.ssn.ToString().ToUpper(), SSN);

            Dictionary<string, object> ldictParamsForPersonId = new Dictionary<string, object>();
            ldictParamsForPersonId[utlConstants.istrConstUserID] = "WebService";

            busPersonOverview lbusPersonOverview = new busPersonOverview();

            if (!string.IsNullOrEmpty(SSN))
            {

                Dictionary<string, object> ldictParams = new Dictionary<string, object>();
                ldictParams[utlConstants.istrConstUserID] = "WebService";
                Hashtable lhstParam1 = new Hashtable();
                lhstParam1.Add("astrSSN", SSN);

                lbusPersonOverview = (busPersonOverview)isrvBusinessTier.ExecuteMethod("GetPlanBenefitSummary", lhstParam1, true, ldictParams);
                if (lbusPersonOverview != null && lbusPersonOverview.iclcdoPersonAccountOverview != null && lbusPersonOverview.iclcdoPersonAccountOverview.Count() > 0)
                {
                    PlanBenefitSummary = lbusPersonOverview.iclcdoPersonAccountOverview.Select(x => new Person { Plan = x.istrPlan, PlanStatus = x.status_description, PensionHours = x.istrTotalHours, QualifiedYears = x.istrTotalQualifiedYears, PensionCredit = x.istrPensionCredit, VestedDate = x.dtVestedDate.Date, HealthHours = x.istrHealthHoursPO, HealthYears = x.istrTotalHealthYearsPO, MonthlyBenefit = x.idecTotalAccruedBenefit, IAPBalance = x.idecSpecialAccountBalance, AllocationAsOfYrEnd = x.istrAllocationEndYear, MPIPersonId = lbusPersonOverview.icdoPerson.mpi_person_id, IsSuccess = true }).ToList<Person>();

                }
                else if (lbusPersonOverview.icdoPerson.ssn.IsNullOrEmpty())
                {
                    Person lPerson = new Person();
                    lPerson.IsSuccess = false;
                    lPerson.Comments = "Please Enter Valid SSN";
                    PlanBenefitSummary.Add(lPerson);
                }
                else
                {
                    Person lPerson = new Person();
                    lPerson.IsSuccess = false;
                    lPerson.Comments = "Plan Information does not exists.";
                    PlanBenefitSummary.Add(lPerson);
                }
                return PlanBenefitSummary;

            }
            else
            {
                Person lPerson = new Person();
                lPerson.IsSuccess = false;
                lPerson.Comments = "Please Enter Valid SSN";
                PlanBenefitSummary.Add(lPerson);
                return PlanBenefitSummary;
            }

        }
        catch (Exception ex)
        {
            Person lPerson = new Person();
            lPerson.IsSuccess = false;
            lPerson.Comments = HelperFunction.istrAppSettingsLocation;
            lPerson.Comments += "Error Processing Record With SSN : " + SSN + ex.Message + "\n" + ex.StackTrace;
            if (ex.InnerException.IsNotNull())
            {
                lPerson.Comments += ex.InnerException.Message + "\n" + ex.InnerException.StackTrace;
            }
            PlanBenefitSummary.Add(lPerson);
            return PlanBenefitSummary;

        }
        finally
        {
            HelperFunction.CloseChannel(isrvBusinessTierForPerson);
            HelperFunction.CloseChannel(isrvBusinessTier);
        }
        return PlanBenefitSummary;
    }

    public List<BenefitCalculationDetail> GetBenefitCalculationEstimate(string MPID, int BenefitPlanId, DateTime RetirementDate, DateTime SpouseDateOfBirth)
    {

        List<BenefitCalculationDetail> BenefitCalculation = new List<BenefitCalculationDetail>();
        Collection<BenefitCalculationDetail> lclbBenefitCalculationDetail = new Collection<BenefitCalculationDetail>();

        IBusinessTier isrvBusinessTierForPerson = null;
        IBusinessTier isrvBusinessTier = null;
        try
        {
            //Configuration MPIWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            //if (MPIWebConfig.AppSettings.Settings.Count > 0)
            //{
            //    KeyValueConfigurationElement WebConfigAppSetting = MPIWebConfig.AppSettings.Settings["AppSettingsLocation"];
            //    HelperFunction.istrAppSettingsLocation = WebConfigAppSetting.Value;
            //}

            //KeyValueConfigurationElement WebConfigBusinessTierUrl = MPIWebConfig.AppSettings.Settings["BusinessTierUrl"];
            //FM upgrade changes - Remoting to WCF
            string lstrUrlForPersonId = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvPerson");
            //IBusinessTier isrvBusinessTierForPerson = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), lstrUrlForPersonId);
            isrvBusinessTierForPerson = WCFClient<IBusinessTier>.CreateChannel(lstrUrlForPersonId);
            string lstrUrl = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvMPIPHPMSS");
            //IBusinessTier isrvBusinessTier = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), lstrUrl);
            isrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);
            utlPassInfo iobjPassInfo = new utlPassInfo(true);
            iobjPassInfo.isrvDBCache = isrvDBCache;
            //iobjPassInfo.isrvMetaDataCache = isrvMetaDataCache;
            utlPassInfo.iobjPassInfo = iobjPassInfo;


            if (!string.IsNullOrEmpty(MPID))
            {
                Dictionary<string, object> ldictParams = new Dictionary<string, object>();
                ldictParams[utlConstants.istrConstUserID] = "WebService";
                Hashtable lhstParam = new Hashtable();
                lhstParam.Add("astrMPID", MPID);
                lhstParam.Add("adtRetirementDate", RetirementDate);
                lhstParam.Add("adtSpouseDOB", SpouseDateOfBirth);
                lhstParam.Add("aintPlanId", BenefitPlanId);

                busMssBenefitCalculationRetirement lbusMssBenefitCalculationRetirement = new busMssBenefitCalculationRetirement();
                lbusMssBenefitCalculationRetirement = (busMssBenefitCalculationRetirement)isrvBusinessTier.ExecuteMethod("NewRetirementCalculation", lhstParam, true, ldictParams);

                if (lbusMssBenefitCalculationRetirement != null && lbusMssBenefitCalculationRetirement.iclbBenefitCalculationDetail != null && lbusMssBenefitCalculationRetirement.iclbBenefitCalculationDetail.Count > 0
                    && lbusMssBenefitCalculationRetirement.iclbBenefitCalculationDetail[0] != null && lbusMssBenefitCalculationRetirement.iclbBenefitCalculationDetail[0].iclbBenefitCalculationOptions.Count > 0)
                {

                    foreach (busBenefitCalculationDetail lbusBenefitCalculationDetail in lbusMssBenefitCalculationRetirement.iclbBenefitCalculationDetail)
                    {
                        BenefitCalculationDetail lBenefitCalculationDetail = new BenefitCalculationDetail();
                        lBenefitCalculationDetail.BenefitPlanId = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id;
                        lBenefitCalculationDetail.PlanName = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.istrPlanCode;
                        lBenefitCalculationDetail.VestedDate = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.vested_date;
                        lBenefitCalculationDetail.AccruedBenefitAmount = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.unreduced_benefit_amount;
                        lBenefitCalculationDetail.LifeAnnuityBenefitAmount = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.early_reduced_benefit_amount;
                        lBenefitCalculationDetail.QDROOffset = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.qdro_offset;
                        lBenefitCalculationDetail.EarlyRetirementFactor = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.early_reduction_factor;
                        lBenefitCalculationDetail.IAPBalanceAmount = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iap_balance_amount;
                        lBenefitCalculationDetail.IAPAsOfDate = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.iap_as_of_date;
                        lBenefitCalculationDetail.Local52SpecialAccountBalanceAmount = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.local52_special_acct_bal_amount;
                        lBenefitCalculationDetail.Local161SpecialAccountBalanceAmount = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.local161_special_acct_bal_amount;
                        lBenefitCalculationDetail.MPIPersonId = MPID;
                        lBenefitCalculationDetail.RetirementType = lbusMssBenefitCalculationRetirement.icdoBenefitCalculationHeader.istrRetirementTypeDescription;
                        lBenefitCalculationDetail.IsSuccess = true;

                        lBenefitCalculationDetail.clBenefitCalculationOption = new Collection<BenefitCalculationOption>();

                        if (lbusBenefitCalculationDetail.iclbBenefitCalculationOptions != null && lbusBenefitCalculationDetail.iclbBenefitCalculationOptions.Count > 0)
                        {
                            foreach (busBenefitCalculationOptions lbusBenefitCalculationOptions in lbusBenefitCalculationDetail.iclbBenefitCalculationOptions)
                            {
                                BenefitCalculationOption lBenefitCalculationOption = new BenefitCalculationOption();
                                lBenefitCalculationOption.BenefitPlanId = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id;
                                lBenefitCalculationOption.BenefitOption = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.istrBenefitOptionDescription;
                                lBenefitCalculationOption.BenefitOptionFactor = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_option_factor;
                                lBenefitCalculationOption.BenefitAmount = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_amount;
                                lBenefitCalculationOption.SurvivorAmount = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.survivor_amount;
                                lBenefitCalculationOption.RelativeValue = lbusBenefitCalculationOptions.icdoBenefitCalculationOptions.relative_value;
                                lBenefitCalculationOption.MPIPersonId = MPID;
                                lBenefitCalculationDetail.clBenefitCalculationOption.Add(lBenefitCalculationOption);
                            }
                        }
                        lclbBenefitCalculationDetail.Add(lBenefitCalculationDetail);
                    }

                    BenefitCalculation = lclbBenefitCalculationDetail.ToList<BenefitCalculationDetail>();
                }
                else
                {
                    BenefitCalculationDetail lBenefitCalculationDetail = new BenefitCalculationDetail();
                    lBenefitCalculationDetail.IsSuccess = false;
                    lBenefitCalculationDetail.Comments = "Participant is not eligible for the retirement";
                    lclbBenefitCalculationDetail.Add(lBenefitCalculationDetail);
                    BenefitCalculation = lclbBenefitCalculationDetail.ToList<BenefitCalculationDetail>();
                }

                return BenefitCalculation;
            }
            else
            {
                BenefitCalculationDetail lBenefitCalculationDetail = new BenefitCalculationDetail();
                lBenefitCalculationDetail.IsSuccess = false;
                lBenefitCalculationDetail.Comments = "Invalid parameter : MPI Person ID";
                lclbBenefitCalculationDetail.Add(lBenefitCalculationDetail);
                BenefitCalculation = lclbBenefitCalculationDetail.ToList<BenefitCalculationDetail>();
                return BenefitCalculation;
            }

        }
        catch (Exception ex)
        {
            BenefitCalculationDetail lBenefitCalculationDetail = new BenefitCalculationDetail();
            lBenefitCalculationDetail.IsSuccess = false;
            lBenefitCalculationDetail.Comments = "Error Processing Record With MPID : " + MPID + ex.Message + "\n" + ex.StackTrace;
            if (ex.InnerException.IsNotNull())
            {
                lBenefitCalculationDetail.Comments += ex.InnerException.Message + "\n" + ex.InnerException.StackTrace;
            }
            lclbBenefitCalculationDetail.Add(lBenefitCalculationDetail);
            BenefitCalculation = lclbBenefitCalculationDetail.ToList<BenefitCalculationDetail>();
            return BenefitCalculation;
        }
        finally
        {
            HelperFunction.CloseChannel(isrvBusinessTierForPerson);
            HelperFunction.CloseChannel(isrvBusinessTier);
            HelperFunction.CloseChannel(_isrvMetaDataCache);
            HelperFunction.CloseChannel(_isrvDBCache);
        }
        return BenefitCalculation;
    }

    //ABS Data
    public List<AnnualBenefitSummaryData> GetAnnualBenefitSummary(string astrMpiPersonId)
    {

        List<AnnualBenefitSummaryData> AnnualBenefitSummary = new List<AnnualBenefitSummaryData>();
        Collection<AnnualBenefitSummaryData> lclbAnnualBenefitSummary = new Collection<AnnualBenefitSummaryData>();

        IBusinessTier isrvBusinessTierForPerson = null;
        IBusinessTier isrvBusinessTier = null;
        try
        {
            //Configuration MPIWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            //if (MPIWebConfig.AppSettings.Settings.Count > 0)
            //{
            //    KeyValueConfigurationElement WebConfigAppSetting = MPIWebConfig.AppSettings.Settings["AppSettingsLocation"];
            //    HelperFunction.istrAppSettingsLocation = WebConfigAppSetting.Value;
            //}

            //KeyValueConfigurationElement WebConfigBusinessTierUrl = MPIWebConfig.AppSettings.Settings["BusinessTierUrl"];
            //FM upgrade changes - Remoting to WCF
            string lstrUrlForPersonId = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvPerson");
            //IBusinessTier isrvBusinessTierForPerson = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), lstrUrlForPersonId);
            isrvBusinessTierForPerson = WCFClient<IBusinessTier>.CreateChannel(lstrUrlForPersonId);
            string lstrUrl = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvMPIPHPMSS");
            //IBusinessTier isrvBusinessTier = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), lstrUrl);
            isrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);
            //utlPassInfo iobjPassInfo = new utlPassInfo();
            //utlPassInfo.iobjPassInfo = iobjPassInfo;


            if (!string.IsNullOrEmpty(astrMpiPersonId))
            {
                Dictionary<string, object> ldictParams = new Dictionary<string, object>();
                ldictParams[utlConstants.istrConstUserID] = "WebService";
                Hashtable lhstParam = new Hashtable();
                lhstParam.Add("astrMpiPersonId", astrMpiPersonId);

                busAnnualBenefitSummaryOverview lbusAnnualBenefitSummary = new busAnnualBenefitSummaryOverview();
                lbusAnnualBenefitSummary = (busAnnualBenefitSummaryOverview)isrvBusinessTier.ExecuteMethod("GetAnnualBenefitSummaryOverview", lhstParam, true, ldictParams);

                if (lbusAnnualBenefitSummary != null && lbusAnnualBenefitSummary.lbusBenefitApplication != null
                    && lbusAnnualBenefitSummary.lbusBenefitApplication.aclbPersonWorkHistory_MPI != null
                    && lbusAnnualBenefitSummary.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Count > 0)
                {

                    foreach (cdoDummyWorkData lcdoDummyWorkData in lbusAnnualBenefitSummary.lbusBenefitApplication.aclbPersonWorkHistory_MPI)
                    {
                        AnnualBenefitSummaryData lAnnualBenefitSummaryData = new AnnualBenefitSummaryData();
                        lAnnualBenefitSummaryData.Year = lcdoDummyWorkData.year;
                        lAnnualBenefitSummaryData.CreditedHours = lcdoDummyWorkData.qualified_hours;
                        lAnnualBenefitSummaryData.WithdrawnHours = lcdoDummyWorkData.idecWithdrawalHours;
                        lAnnualBenefitSummaryData.QualifiedYears = lcdoDummyWorkData.qualified_years_count;
                        lAnnualBenefitSummaryData.VestedYears = lcdoDummyWorkData.vested_years_count;
                        lAnnualBenefitSummaryData.NonQualifiedYearCount = lcdoDummyWorkData.iintNonQualifiedYears;
                        lAnnualBenefitSummaryData.RetireeHealthHours = lcdoDummyWorkData.idecTotalHealthHours;
                        lAnnualBenefitSummaryData.RetireeHealthYears = lcdoDummyWorkData.iintHealthCount;
                        lAnnualBenefitSummaryData.EEContribution = lcdoDummyWorkData.idecEEContribution;
                        lAnnualBenefitSummaryData.EEInterest = lcdoDummyWorkData.idecEEInterest;
                        lAnnualBenefitSummaryData.UVHPContribution = lcdoDummyWorkData.idecUVHPContribution;
                        lAnnualBenefitSummaryData.UVHPInterest = lcdoDummyWorkData.idecUVHPInterest;
                        lAnnualBenefitSummaryData.AccruedBenefit = lcdoDummyWorkData.idecBenefitAmount;
                        lAnnualBenefitSummaryData.AccruedBenefitLocal = lcdoDummyWorkData.idecBenefitAmountLocal;
                        lAnnualBenefitSummaryData.AccumulatedAccruedBenefit = lcdoDummyWorkData.idecCummBalance;
                        lAnnualBenefitSummaryData.Comments = lcdoDummyWorkData.comments;
                        lAnnualBenefitSummaryData.IsSuccess = true;
                        lclbAnnualBenefitSummary.Add(lAnnualBenefitSummaryData);
                    }

                    AnnualBenefitSummary = lclbAnnualBenefitSummary.ToList<AnnualBenefitSummaryData>();
                }
                else
                {
                    AnnualBenefitSummaryData lAnnualBenefitSummaryData = new AnnualBenefitSummaryData();
                    lAnnualBenefitSummaryData.IsSuccess = false;
                    lAnnualBenefitSummaryData.Comments = "Annual benefit summary is not available for this MPID.";
                    lclbAnnualBenefitSummary.Add(lAnnualBenefitSummaryData);
                    AnnualBenefitSummary = lclbAnnualBenefitSummary.ToList<AnnualBenefitSummaryData>();
                }

                return AnnualBenefitSummary;
            }
            else
            {
                AnnualBenefitSummaryData lAnnualBenefitSummaryData = new AnnualBenefitSummaryData();
                lAnnualBenefitSummaryData.IsSuccess = false;
                lAnnualBenefitSummaryData.Comments = "Invalid parameter : MPI Person ID";
                lclbAnnualBenefitSummary.Add(lAnnualBenefitSummaryData);
                AnnualBenefitSummary = lclbAnnualBenefitSummary.ToList<AnnualBenefitSummaryData>();
                return AnnualBenefitSummary;
            }

        }
        catch (Exception ex)
        {
            AnnualBenefitSummaryData lAnnualBenefitSummaryData = new AnnualBenefitSummaryData();
            lAnnualBenefitSummaryData.IsSuccess = false;
            lAnnualBenefitSummaryData.Comments = "Error Processing Record With MPID : " + astrMpiPersonId + ex.Message + "\n" + ex.StackTrace;
            if (ex.InnerException.IsNotNull())
            {
                lAnnualBenefitSummaryData.Comments += ex.InnerException.Message + "\n" + ex.InnerException.StackTrace;
            }
            lclbAnnualBenefitSummary.Add(lAnnualBenefitSummaryData);
            AnnualBenefitSummary = lclbAnnualBenefitSummary.ToList<AnnualBenefitSummaryData>();
            return AnnualBenefitSummary;
        }
        finally
        {
            HelperFunction.CloseChannel(isrvBusinessTierForPerson);
            HelperFunction.CloseChannel(isrvBusinessTier);
        }
        return AnnualBenefitSummary;
    }

    public List<RetirementProcessTrackerData> GetRetirementProcessTracker(string astrMpiPersonId)
    {

        List<RetirementProcessTrackerData> lstRetirementProcessTrackerData = new List<RetirementProcessTrackerData>();
        Collection<RetirementProcessTrackerData> lclRetirementProcessTrackerData = new Collection<RetirementProcessTrackerData>();

        IBusinessTier isrvBusinessTierForPerson = null;
        IBusinessTier isrvBusinessTier = null;
        try
        {
            //Configuration MPIWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            //if (MPIWebConfig.AppSettings.Settings.Count > 0)
            //{
            //    KeyValueConfigurationElement WebConfigAppSetting = MPIWebConfig.AppSettings.Settings["AppSettingsLocation"];
            //    HelperFunction.istrAppSettingsLocation = WebConfigAppSetting.Value;
            //}

            //KeyValueConfigurationElement WebConfigBusinessTierUrl = MPIWebConfig.AppSettings.Settings["BusinessTierUrl"];
            //FM upgrade changes - Remoting to WCF
            string lstrUrlForPersonId = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvPerson");
            //IBusinessTier isrvBusinessTierForPerson = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), lstrUrlForPersonId);
            isrvBusinessTierForPerson = WCFClient<IBusinessTier>.CreateChannel(lstrUrlForPersonId);
            string lstrUrl = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvMPIPHPMSS");
            //IBusinessTier isrvBusinessTier = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), lstrUrl);
            isrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);
            //utlPassInfo iobjPassInfo = new utlPassInfo();
            //utlPassInfo.iobjPassInfo = iobjPassInfo;


            if (!string.IsNullOrEmpty(astrMpiPersonId))
            {
                Dictionary<string, object> ldictParams = new Dictionary<string, object>();
                ldictParams[utlConstants.istrConstUserID] = "WebService";
                Hashtable lhstParam = new Hashtable();
                lhstParam.Add("astrMpiPersonId", astrMpiPersonId);

                DataTable ldtRetirementProcessTracker = (DataTable)isrvBusinessTier.ExecuteMethod("GetRetirementProcessTracker", lhstParam, true, ldictParams);

                if (ldtRetirementProcessTracker != null && ldtRetirementProcessTracker.Rows.Count > 0)
                {

                    foreach (DataRow ldrRetirementProcessTracker in ldtRetirementProcessTracker.Rows)
                    {
                        RetirementProcessTrackerData lRetirementProcessTrackerData = new RetirementProcessTrackerData();
                        lRetirementProcessTrackerData.MPIPersonId = Convert.ToString(ldrRetirementProcessTracker["MPI_PERSON_ID"]);
                        lRetirementProcessTrackerData.PlanName = Convert.ToString(ldrRetirementProcessTracker["PLAN_NAME"]);
                        lRetirementProcessTrackerData.ApplicationMailed = Convert.ToDateTime(ldrRetirementProcessTracker["APPLICATION_MAILED"]);
                        lRetirementProcessTrackerData.ApplicationReceived = Convert.ToDateTime(ldrRetirementProcessTracker["APPLICATION_RECEIVED"]);
                        lRetirementProcessTrackerData.BenefitElectionPacketMailed = Convert.ToDateTime(ldrRetirementProcessTracker["BENEFIT_ELECTION_PACKET_MAILED"]);
                        lRetirementProcessTrackerData.BenefitElectionPacketReceived = Convert.ToDateTime(ldrRetirementProcessTracker["BENEFIT_ELECTION_PACKET_RECEIVED"]);
                        lRetirementProcessTrackerData.PayStatusDate = Convert.ToDateTime(ldrRetirementProcessTracker["PAY_STATUS_DATE"]);
                        lRetirementProcessTrackerData.PayStatus = Convert.ToString(ldrRetirementProcessTracker["PAY_STATUS"]);
                        lRetirementProcessTrackerData.Comments = "";
                        lRetirementProcessTrackerData.IsSuccess = true;
                        lclRetirementProcessTrackerData.Add(lRetirementProcessTrackerData);
                    }

                    lstRetirementProcessTrackerData = lclRetirementProcessTrackerData.ToList<RetirementProcessTrackerData>();
                }
                else
                {
                    RetirementProcessTrackerData lRetirementProcessTrackerData = new RetirementProcessTrackerData();
                    lRetirementProcessTrackerData.IsSuccess = false;
                    lRetirementProcessTrackerData.Comments = "Retirement Process Tracker data is not available for this MPID.";
                    lclRetirementProcessTrackerData.Add(lRetirementProcessTrackerData);
                    lstRetirementProcessTrackerData = lclRetirementProcessTrackerData.ToList<RetirementProcessTrackerData>();
                }

                return lstRetirementProcessTrackerData;
            }
            else
            {
                RetirementProcessTrackerData lRetirementProcessTrackerData = new RetirementProcessTrackerData();
                lRetirementProcessTrackerData.IsSuccess = false;
                lRetirementProcessTrackerData.Comments = "Invalid parameter : MPI Person ID";
                lclRetirementProcessTrackerData.Add(lRetirementProcessTrackerData);
                lstRetirementProcessTrackerData = lclRetirementProcessTrackerData.ToList<RetirementProcessTrackerData>();
                return lstRetirementProcessTrackerData;
            }

        }
        catch (Exception ex)
        {
            RetirementProcessTrackerData lRetirementProcessTrackerData = new RetirementProcessTrackerData();
            lRetirementProcessTrackerData.IsSuccess = false;
            lRetirementProcessTrackerData.Comments = "Error Processing Record With MPID : " + astrMpiPersonId + ex.Message + "\n" + ex.StackTrace;
            if (ex.InnerException.IsNotNull())
            {
                lRetirementProcessTrackerData.Comments += ex.InnerException.Message + "\n" + ex.InnerException.StackTrace;
            }
            lclRetirementProcessTrackerData.Add(lRetirementProcessTrackerData);
            lstRetirementProcessTrackerData = lclRetirementProcessTrackerData.ToList<RetirementProcessTrackerData>();
            return lstRetirementProcessTrackerData;
        }
        finally
        {
            HelperFunction.CloseChannel(isrvBusinessTierForPerson);
            HelperFunction.CloseChannel(isrvBusinessTier);
        }
        return lstRetirementProcessTrackerData;
    }

    #endregion WCF - Website and Mobile App

    public List<PayeeAccountBreakdownData> GetPayeeAccountBreakdown(int aintPayeeAccountId)
    {

        List<PayeeAccountBreakdownData> lstPayeeAccountBreakdownData = new List<PayeeAccountBreakdownData>();
        Collection<PayeeAccountBreakdownData> lclbPayeeAccountBreakdownData = new Collection<PayeeAccountBreakdownData>();

        IBusinessTier isrvBusinessTierForPerson = null;
        IBusinessTier isrvBusinessTier = null;
        try
        {
            //Configuration MPIWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            //if (MPIWebConfig.AppSettings.Settings.Count > 0)
            //{
            //    KeyValueConfigurationElement WebConfigAppSetting = MPIWebConfig.AppSettings.Settings["AppSettingsLocation"];
            //    HelperFunction.istrAppSettingsLocation = WebConfigAppSetting.Value;
            //}

            //KeyValueConfigurationElement WebConfigBusinessTierUrl = MPIWebConfig.AppSettings.Settings["BusinessTierUrl"];
            //FM upgrade changes - Remoting to WCF
            string lstrUrlForPersonId = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvPerson");
            //IBusinessTier isrvBusinessTierForPerson = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), lstrUrlForPersonId);
            isrvBusinessTierForPerson = WCFClient<IBusinessTier>.CreateChannel(lstrUrlForPersonId);
            string lstrUrl = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvMPIPHPMSS");
            //IBusinessTier isrvBusinessTier = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), lstrUrl);
            isrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);
            //utlPassInfo iobjPassInfo = new utlPassInfo();
            //utlPassInfo.iobjPassInfo = iobjPassInfo;


            if (aintPayeeAccountId > 0)
            {
                Dictionary<string, object> ldictParams = new Dictionary<string, object>();
                ldictParams[utlConstants.istrConstUserID] = "WebService";
                Hashtable lhstParam = new Hashtable();
                lhstParam.Add("aintPayeeAccountId", aintPayeeAccountId);

                busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
                lobjPayeeAccount = (busPayeeAccount)isrvBusinessTier.ExecuteMethod("GetPayeeAccountBreakDownDetails", lhstParam, true, ldictParams);

                if (lobjPayeeAccount != null)
                {
                    PayeeAccountBreakdownData lPayeeAccountBreakdownData = new PayeeAccountBreakdownData();
                    lPayeeAccountBreakdownData.PayeeAccountId = aintPayeeAccountId; // lobjPayeeAccount.PAYEE_ACCOUNT_ID;
                    lPayeeAccountBreakdownData.LastPaymentDate = lobjPayeeAccount.idtPayeeLastBenefitPaymentDate;
                    lPayeeAccountBreakdownData.GrossAmount = lobjPayeeAccount.idecGrossBenefitAmount;
                    lPayeeAccountBreakdownData.NextMonthTaxableAmount = lobjPayeeAccount.idecNextMonthTaxable;
                    lPayeeAccountBreakdownData.NextMonthNonTaxableAmount = lobjPayeeAccount.idecNextMonthNonTaxable;
                    lPayeeAccountBreakdownData.NextPaymentDate = lobjPayeeAccount.idtNextBenefitPaymentDate;
                    lPayeeAccountBreakdownData.NextGrossRolloverAmount = lobjPayeeAccount.idecNextGrossPaymentRollOver;
                    lPayeeAccountBreakdownData.NextNetRolloverAmount = lobjPayeeAccount.idecNextNetPaymentRollOver;

                    lPayeeAccountBreakdownData.NextGrossPayment = lobjPayeeAccount.idecNextGrossPaymentACH;
                    lPayeeAccountBreakdownData.RetroAdjustmentAmount = lobjPayeeAccount.idecRetroAdjustmentAmount;
                    lPayeeAccountBreakdownData.FederalTaxWithholding = lobjPayeeAccount.idecFederalTaxWithHolding;
                    lPayeeAccountBreakdownData.StateTaxWithholding = lobjPayeeAccount.idecStateTaxWithHolding;
                    lPayeeAccountBreakdownData.Deductions = lobjPayeeAccount.idecDeduction;
                    lPayeeAccountBreakdownData.PensionReceivable = lobjPayeeAccount.idecPensionReceivable;
                    lPayeeAccountBreakdownData.NextNetPayment = lobjPayeeAccount.idecNextNetPaymentACH;
                    lPayeeAccountBreakdownData.IsSuccess = true;
                    lPayeeAccountBreakdownData.Comments = "";

                    lclbPayeeAccountBreakdownData.Add(lPayeeAccountBreakdownData);
                    lstPayeeAccountBreakdownData = lclbPayeeAccountBreakdownData.ToList<PayeeAccountBreakdownData>();
                }
                else
                {
                    PayeeAccountBreakdownData lPayeeAccountBreakdownData = new PayeeAccountBreakdownData();
                    lPayeeAccountBreakdownData.IsSuccess = false;
                    lPayeeAccountBreakdownData.Comments = "Payee Account breakdown is not available for this Payee Account ID.";
                    lclbPayeeAccountBreakdownData.Add(lPayeeAccountBreakdownData);
                    lstPayeeAccountBreakdownData = lclbPayeeAccountBreakdownData.ToList<PayeeAccountBreakdownData>();
                }

                return lstPayeeAccountBreakdownData;
            }
            else
            {
                PayeeAccountBreakdownData lPayeeAccountBreakdownData = new PayeeAccountBreakdownData();
                lPayeeAccountBreakdownData.IsSuccess = false;
                lPayeeAccountBreakdownData.Comments = "Invalid parameter : Payee Account ID.";
                lclbPayeeAccountBreakdownData.Add(lPayeeAccountBreakdownData);
                lstPayeeAccountBreakdownData = lclbPayeeAccountBreakdownData.ToList<PayeeAccountBreakdownData>();
                return lstPayeeAccountBreakdownData;
            }

        }
        catch (Exception ex)
        {
            PayeeAccountBreakdownData lPayeeAccountBreakdownData = new PayeeAccountBreakdownData();
            lPayeeAccountBreakdownData.IsSuccess = false;
            lPayeeAccountBreakdownData.Comments = "Error Processing record with Payee Account ID: " + aintPayeeAccountId.ToString() + ex.Message + "\n" + ex.StackTrace;
            if (ex.InnerException.IsNotNull())
            {
                lPayeeAccountBreakdownData.Comments += ex.InnerException.Message + "\n" + ex.InnerException.StackTrace;
            }
            lclbPayeeAccountBreakdownData.Add(lPayeeAccountBreakdownData);
            lstPayeeAccountBreakdownData = lclbPayeeAccountBreakdownData.ToList<PayeeAccountBreakdownData>();
            return lstPayeeAccountBreakdownData;
        }
        finally
        {
            HelperFunction.CloseChannel(isrvBusinessTierForPerson);
            HelperFunction.CloseChannel(isrvBusinessTier);
        }
        //return lstPayeeAccountBreakdownData;
    }

    public string GetRetireeHealthEligibilityFlag(string astrMpiPersonId)
    {

        string lstrRetireeHealthEligibilityFlag = "";

        IBusinessTier isrvBusinessTierForPerson = null;
        IBusinessTier isrvBusinessTier = null;
        try
        {
            string lstrUrlForPersonId = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvPerson");
            isrvBusinessTierForPerson = WCFClient<IBusinessTier>.CreateChannel(lstrUrlForPersonId);
            string lstrUrl = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvMPIPHPMSS");
            isrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);

            if (!string.IsNullOrEmpty(astrMpiPersonId))
            {
                Dictionary<string, object> ldictParams = new Dictionary<string, object>();
                ldictParams[utlConstants.istrConstUserID] = "WebService";
                Hashtable lhstParam = new Hashtable();
                lhstParam.Add("astrMpiPersonId", astrMpiPersonId);

                lstrRetireeHealthEligibilityFlag = (string)isrvBusinessTier.ExecuteMethod("GetRetireeHealthEligibilityFlag", lhstParam, true, ldictParams);

                if (lstrRetireeHealthEligibilityFlag.IsNullOrEmpty())
                {
                    lstrRetireeHealthEligibilityFlag = "";
                }
                else
                {
                    if (lstrRetireeHealthEligibilityFlag == "Y")
                        lstrRetireeHealthEligibilityFlag = "Yes";
                    else if (lstrRetireeHealthEligibilityFlag == "N")
                        lstrRetireeHealthEligibilityFlag = "No";
                    else
                        lstrRetireeHealthEligibilityFlag = "";
                }

            }
            else
            {
                lstrRetireeHealthEligibilityFlag = "";
            }

        }
        catch (Exception ex)
        {
            lstrRetireeHealthEligibilityFlag = "";
        }
        finally
        {
            HelperFunction.CloseChannel(isrvBusinessTierForPerson);
            HelperFunction.CloseChannel(isrvBusinessTier);
        }

        return lstrRetireeHealthEligibilityFlag;
    }

}
