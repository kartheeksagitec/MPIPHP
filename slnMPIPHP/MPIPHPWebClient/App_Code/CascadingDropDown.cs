using System;
using System.Data;
using System.Collections;
using System.Web.Services;
using System.Configuration;
using Sagitec.Common;
using Sagitec.Interface;
using Sagitec.DataObjects;
using Sagitec.BusinessObjects;
using System.Collections.Generic;

/// <summary>
/// Summary description for CascadingDropDown
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService()]
public class CascadingDropDown : WebService
{
    public CascadingDropDown()
    {
        // Uncomment the following line if using designed components 
        // InitializeComponent(); 
    }

    [WebMethod(EnableSession = true)]
    [System.Web.Script.Services.ScriptMethod]
    public string GetDropDownValuesFromCode(int aintCodeID, string astrParameters)
    {
        string lstrResult = ":";

        string lstrDBCacheUrl = ConfigurationManager.AppSettings["DBCacheUrl"].Split(new char[1] { ';' })[0];
        //FM upgrade changes - Remoting to WCF
        //IDBCache lsrvDBCache = (IDBCache)Activator.GetObject(typeof(IDBCache), lstrDBCacheUrl);
        IDBCache lsrvDBCache = null;
        try
        {
            lsrvDBCache = WCFClient<IDBCache>.CreateChannel(lstrDBCacheUrl);

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
        }
        finally
        {
            HelperFunction.CloseChannel(lsrvDBCache);
        }
        return lstrResult;
    }

    [WebMethod(EnableSession = true)]
    [System.Web.Script.Services.ScriptMethod]
    public string GetDropDownValuesFromQuery(string astrQuery, string astrParameters, string astrDataValueField, string astrDataTextField)
    {
        string lstrResult = ":";

        string lstrBusinessTierUrl = (string)Session["BusinessTierUrl"];
        //FM upgrade changes - Remoting to WCF
        //IBusinessTier lsrvBusinessTier = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), lstrBusinessTierUrl);
        IBusinessTier lsrvBusinessTier = null;
        try
        {
            lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrBusinessTierUrl);

            DataTable ldtbResult = lsrvBusinessTier.GetDropDownValuesUsingQuery(astrQuery, astrParameters, new Dictionary<string, object>());
            if (ldtbResult != null)
            {
                foreach (DataRow ldtrResult in ldtbResult.Rows)
                {
                    lstrResult += ";" + ldtrResult[astrDataValueField].ToString() + ":" + ldtrResult[astrDataTextField].ToString();
                }
            }
        }
        finally
        {
            HelperFunction.CloseChannel(lsrvBusinessTier);
        }
        return lstrResult;
    }

    [WebMethod(EnableSession = true)]
    [System.Web.Script.Services.ScriptMethod]
    public string GetDropDownValuesFromMethod(string astrMethodName, string astrParameters, string astrDataValueField, string astrDataTextField)
    {
        string lstrResult = ":";

        object lobjMain = Session["CenterMiddle"];
        string lstrBusinessTierUrl = (string)Session["BusinessTierUrl"];
        //FM upgrade changes - Remoting to WCF
        //IBusinessTier lsrvBusinessTier = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), lstrBusinessTierUrl);
        IBusinessTier lsrvBusinessTier = null;
        try
        {
            lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrBusinessTierUrl);

            ICollection lcolDataValues = lsrvBusinessTier.GetDropDownValuesUsingMethod(lobjMain, astrMethodName, astrParameters, new Dictionary<string, object>());
            if (lcolDataValues != null)
            {
                foreach (doBase lobjBase in lcolDataValues)
                {
                    lstrResult += ";" + busMainBase.GetFieldValue(lobjBase, astrDataValueField) +
                                  ":" + busMainBase.GetFieldValue(lobjBase, astrDataTextField);
                }
            }
        }
        finally
        {
            HelperFunction.CloseChannel(lsrvBusinessTier);
        }
        return lstrResult;
    }
}
