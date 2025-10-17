using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.Script.Services;
using System.Configuration;
using Sagitec.Interface;
using Sagitec.DataObjects;
using Sagitec.BusinessObjects;
using Sagitec.WebClient;

/// <summary>
/// Summary description for SagitecWebServices
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class SagitecWebServices : WebService
{
    public SagitecWebServices()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [ScriptMethod]
    [WebMethod(EnableSession = true)]
    public string GetRetrievalValuesFromCode(string astrParameters, string astrFieldNames)
    {
        string lstrResult = string.Empty;

        string lstrDBCacheUrl = ConfigurationManager.AppSettings["DBCacheUrl"].Split(new char[1] { ';' })[0];
        IDBCache lsrvDBCache = (IDBCache)Activator.GetObject(typeof(IDBCache), lstrDBCacheUrl);

        string lstrWhere = string.Empty;
        if (!string.IsNullOrEmpty(astrParameters))
        {
            string[] larrParameters = astrParameters.Split(';');
            foreach (string lstrParameter in larrParameters)
            {
                string[] larrParameter = lstrParameter.Split('=');
                if (lstrWhere == string.Empty)
                    lstrWhere = larrParameter[0] + " = '" + larrParameter[1] + "'";
                else
                    lstrWhere += " and " + larrParameter[0] + " = '" + larrParameter[1] + "'";
            }
        }

        DataTable ldtbResult = lsrvDBCache.GetCacheData("sgs_code_value", lstrWhere);
        if ((ldtbResult != null) && (ldtbResult.Rows.Count > 0))
        {
            DataRow ldtrResult = ldtbResult.Rows[0];
            string[] lstrArrFields = astrFieldNames.Split(new char[1] { ';' });

            foreach (string lstrFieldName in lstrArrFields)
            {
                if (lstrResult == string.Empty)
                    lstrResult = ldtrResult[lstrFieldName].ToString();
                else
                    lstrResult += ";" + ldtrResult[lstrFieldName].ToString();
            }
        }

        return lstrResult;
    }

    [ScriptMethod]
    [WebMethod(EnableSession = true)]
    public string GetRetrievalValuesFromQuery(string astrQuery, string astrParameters, string astrFieldNames)
    {
        string lstrResult = string.Empty;

        string lstrBusinessTierUrl = (string)Framework.SessionForWindow["BusinessTierUrl"];
        IBusinessTier lsrvBusinessTier = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), lstrBusinessTierUrl);

        Dictionary<string, object> ldictParams = new Dictionary<string, object>();
        DataTable ldtbResult = lsrvBusinessTier.GetValuesUsingQuery(astrQuery, astrParameters, ldictParams);
        if ((ldtbResult != null) && (ldtbResult.Rows.Count > 0))
        {
            DataRow ldtrResult = ldtbResult.Rows[0];
            string[] lstrArrFields = astrFieldNames.Split(new char[1] { ';' });

            foreach (string lstrFieldName in lstrArrFields)
            {
                if (lstrResult == string.Empty)
                    lstrResult = ldtrResult[lstrFieldName].ToString();
                else
                    lstrResult += ";" + ldtrResult[lstrFieldName].ToString();
            }
        }

        return lstrResult;
    }

    [ScriptMethod]
    [WebMethod(EnableSession = true)]
    public string GetRetrievalValuesFromMethod(string astrMethodName, string astrParameters, string astrFieldNames)
    {
        string lstrResult = string.Empty;

        object lobjMain = Framework.SessionForWindow["CenterMiddle"];
        string lstrBusinessTierUrl = (string)Framework.SessionForWindow["BusinessTierUrl"];
        IBusinessTier lsrvBusinessTier = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), lstrBusinessTierUrl);

        Dictionary<string, object> ldictParams = new Dictionary<string, object>();
        object lobjResult = lsrvBusinessTier.GetValuesUsingMethod(lobjMain, astrMethodName, astrParameters, ldictParams);
        if (lobjResult != null)
        {
            string[] lstrArrProperties = astrFieldNames.Split(new char[1] { ';' });
            foreach (string lstrPropertyName in lstrArrProperties)
            {
                if (lstrResult == string.Empty)
                    lstrResult = busMainBase.GetFieldValue(lobjResult, lstrPropertyName);
                else
                    lstrResult += ";" + busMainBase.GetFieldValue(lobjResult, lstrPropertyName);
            }
        }

        return lstrResult;
    }

    [ScriptMethod]
    [WebMethod(EnableSession = true)]
    public string GetDropDownValuesFromCode(int aintCodeID, string astrParameters)
    {
        string lstrResult = ":";

        string lstrDBCacheUrl = ConfigurationManager.AppSettings["DBCacheUrl"].Split(new char[1] { ';' })[0];
        IDBCache lsrvDBCache = (IDBCache)Activator.GetObject(typeof(IDBCache), lstrDBCacheUrl);

        DataTable ldbtList = null;
        if (string.IsNullOrEmpty(astrParameters))
        {
            ldbtList = lsrvDBCache.GetCodeValues(aintCodeID);
        }
        else
        {
            string[] larrData = astrParameters.Split(';');
            string lstrData1 = null, lstrData2 = null, lstrData3 = null;

            if (larrData.Length > 0)
            {
                lstrData1 = larrData[0].Substring(larrData[0].IndexOf("=") + 1);
            }
            if (larrData.Length > 1)
            {
                lstrData2 = larrData[1].Substring(larrData[1].IndexOf("=") + 1);
            }
            if (larrData.Length > 2)
            {
                lstrData3 = larrData[2].Substring(larrData[2].IndexOf("=") + 1);
            }

            ldbtList = lsrvDBCache.GetCodeValues(aintCodeID, lstrData1, lstrData2, lstrData3);
        }

        foreach (DataRow ldtrRow in ldbtList.Rows)
        {
            lstrResult += ";" + ldtrRow["code_value"].ToString() + ":" + ldtrRow["description"].ToString();
        }

        return lstrResult;
    }

    [ScriptMethod]
    [WebMethod(EnableSession = true)]
    public string GetDropDownValuesFromQuery(string astrQuery, string astrParameters, string astrDataValueField, string astrDataTextField)
    {
        string lstrResult = ":";

        //When there is no value for parameterpassed this was throwing exception. - START
        if (astrQuery.Equals("cdoOrganization.GetChildOrganizationForDropdown"))
        {
            if (astrParameters != null)
            {
                string[] larrParameters = astrParameters.Split(';');
                string lstrParameters = string.Empty;
                string lstrTempParameter = string.Empty;
                foreach (string lstrParameter in larrParameters)
                {
                    if (lstrParameter == "CENTRAL_ORG_ID=")
                    {
                        lstrTempParameter = "CENTRAL_ORG_ID=0";
                    }
                    else if (lstrParameter == "ORG_ID=")
                    {
                        lstrTempParameter = "ORG_ID=0";
                    }
                    else
                    {
                        lstrTempParameter = lstrParameter;
                    }
                    lstrParameters = (string.IsNullOrEmpty(lstrParameters)) ? lstrTempParameter : lstrParameters + ";" + lstrTempParameter;
                    lstrTempParameter = string.Empty;
                }

                astrParameters = lstrParameters;
            }
        }
        //When there is no value for parameterpassed this was throwing exception. - END

        string lstrBusinessTierUrl = (string)Framework.SessionForWindow["BusinessTierUrl"];
        IBusinessTier lsrvBusinessTier = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), lstrBusinessTierUrl);

        Dictionary<string, object> ldictParams = new Dictionary<string, object>();
        DataTable ldtbResult = lsrvBusinessTier.GetDropDownValuesUsingQuery(astrQuery, astrParameters, ldictParams);
        if (ldtbResult != null)
        {
            foreach (DataRow ldtrResult in ldtbResult.Rows)
            {
                lstrResult += ";" + ldtrResult[astrDataValueField].ToString() + ":" + ldtrResult[astrDataTextField].ToString();
            }
        }

        return lstrResult;
    }

    [ScriptMethod]
    [WebMethod(EnableSession = true)]
    public string GetDropDownValuesFromMethod(string astrMethodName, string astrParameters, string astrDataValueField, string astrDataTextField)
    {
        string lstrResult = ":";

        object lobjMain = Framework.SessionForWindow["CenterMiddle"];
        string lstrBusinessTierUrl = (string)Framework.SessionForWindow["BusinessTierUrl"];
        IBusinessTier lsrvBusinessTier = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), lstrBusinessTierUrl);

        Dictionary<string, object> ldictParams = new Dictionary<string, object>();
        ICollection lcolDataValues = lsrvBusinessTier.GetDropDownValuesUsingMethod(lobjMain, astrMethodName, astrParameters, ldictParams);
        if (lcolDataValues != null)
        {
            foreach (doBase lobjBase in lcolDataValues)
            {
                lstrResult += ";" + busMainBase.GetFieldValue(lobjBase, astrDataValueField) +
                              ":" + busMainBase.GetFieldValue(lobjBase, astrDataTextField);
            }
        }

        return lstrResult;
    }

    [ScriptMethod]
    [WebMethod(EnableSession = true)]
    public bool UpdateNeoCertifyTable(int aintScenarioOtherID, string astrObjectType)
    {
        object lobjMain = Framework.SessionForWindow["CenterMiddle"];

        string lstrBusinessTierUrl = (string)Framework.SessionForWindow["BusinessTierUrl"];
        lstrBusinessTierUrl = lstrBusinessTierUrl.Substring(0, lstrBusinessTierUrl.LastIndexOf('/'));
        lstrBusinessTierUrl += "/srvStudio";

        IBusinessTier lsrvBusinessTier = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), lstrBusinessTierUrl);

        Hashtable lhstParams = new Hashtable();
        lhstParams.Add("aobjBase", lobjMain);
        lhstParams.Add("aintScenarioOtherID", aintScenarioOtherID);
        lhstParams.Add("astrObjectType", astrObjectType);

        return (bool)lsrvBusinessTier.ExecuteMethod("UpdateNeoCertifyOtherTable", lhstParams, false, null);
    }
}
