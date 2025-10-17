using System;
using System.Data;
using System.Xml;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Sagitec.Interface;
using Sagitec.Common;
using Sagitec.WebClient;

public partial class wfmLogin_aspx : wfmMainDB
{
	// Page events are wired up automatically to methods 
	// with the following names:
	// Page_Load, Page_AbortTransaction, Page_CommitTransaction,
	// Page_DataBinding, Page_Disposed, Page_Error, Page_Init, 
	// Page_Init Complete, Page_Load, Page_LoadComplete, Page_PreInit
	// Page_PreLoad, Page_PreRender, Page_PreRenderComplete, 
	// Page_SaveStateComplete, Page_Unload

    private string istrErrorMessage = null;

    protected override void OnInit(EventArgs e)
    {
        istrFormName = "wfmLogin";
        Session["IsNewSession"] = "false";
        try
        {
            base.OnInit(e);
        }
        catch (Exception ex)
        {
            istrErrorMessage = ex.Message;
        }
    }

	string lstrCookieName = "KitsCookie";

	protected void Page_Load(object sender, EventArgs e)
	{
        //Check if this is the first time - check if a cookie is present
		//If it is, then retrieve its value and put it in the user name
		if (!this.IsPostBack)
		{
			HttpCookie lcokKitsCookie = Request.Cookies.Get(lstrCookieName);
			if (lcokKitsCookie != null)
			{
				lgnBase.UserName = lcokKitsCookie.Value;
			}
		}


    }

	protected void lgnBase_Authenticate(object sender, AuthenticateEventArgs e)
	{
		Login llgn = (Login)sender;
        
        if (istrErrorMessage != null)
        {
            llgn.FailureText = istrErrorMessage;
            return;
        }
	    string lstrUserName = llgn.UserName.Trim().Replace("'", "''");
        string lstrPassword = llgn.Password.Trim().Replace("'", "''");
        string lstrInitialPage = string.Empty;

		utlUserInfo lobjUserInfo;
		try
		{
            //Hashtable lhstParameter = new Hashtable();
            //lhstParameter.Add("astrUserId", lstrUserName);
            //lhstParameter.Add("astrPassword", lstrPassword);

            //lobjUserInfo = (utlUserInfo)isrvBusinessTier.ExecuteMethod("ValidateUser", lhstParameter, false, idictParams);
            //FM upgrade: 6.0.0.35 changes - ValidateUser method accepts additional parameter ApplicationName
            string lstrApplicationName = ConfigurationManager.AppSettings["ApplicationName"].ToString();
            lobjUserInfo = isrvDBCache.ValidateUser(lstrUserName, lstrPassword, lstrApplicationName);
            lstrInitialPage = lobjUserInfo.istrInitialPage;
            if (!lobjUserInfo.iblnAuthenticated)
            {
                lgnBase.FailureText = lobjUserInfo.istrMessageInternal;
                return;
            }
            //lhstParameter.Clear();
		}
		catch (Exception ce)
		{
            llgn.FailureText = "Unable to connect to Database Cache Server. Please make sure that the application servers are up and running, following error occured : " + ce.Message;
			return;
		}

        try
        {
            //isrvBusinessTier.StoreProcessLog(lstrUserName, istrFormName, "Successflly signed on");
        }
        catch (Exception E)
        {
            llgn.FailureText = "Unable to connect to Business Tier : Exception was raised by the server, following error occured " + E.Message;
            return;
        }

        //FM upgrade: 6.0.0.32 changes
        //wfmMainDB.SetAuthenticatedWindow(hfldLoginWindowName.Value);
        //wfmMainDB.SetAuthenticatedWindow();
        UpdateLogInstance(lstrUserName, lobjUserInfo.iintUserSerialId, lobjUserInfo.istrUserType, lobjUserInfo.istrColorScheme, wfmMainDB.istrApplicationName, lobjUserInfo.iblnAuthenticated);

        Framework.SessionForWindow["UserID"] = lstrUserName;
		Framework.SessionForWindow["UserSerialID"] = lobjUserInfo.iintUserSerialId;
        Framework.SessionForWindow["UserInfo"] = isrvDBCache.GetUserInfo(lstrUserName);
        Framework.SessionForWindow["UserSecurity"] = isrvBusinessTier.GetUserSecurity(lstrUserName, idictParams);
        Framework.SessionForWindow["UserName"] = lobjUserInfo.istrFirstName + " " + lobjUserInfo.istrLastName;
        Framework.SessionForWindow["ColorScheme"] = lobjUserInfo.istrColorScheme;

        //FM upgrade: 6.0.0.32 changes
        //Framework.SessionForWindow[utlConstants.istrActivityLogLevel] = lobjUserInfo.iintActivityLogLevel;
        //Framework.SessionForWindow[utlConstants.istrActivityLogSelectQuery] = lobjUserInfo.iblnLogSelectQuery;
        //Framework.SessionForWindow[utlConstants.istrActivityLogInsertQuery] = lobjUserInfo.iblnLogInsertQuery;
        //Framework.SessionForWindow[utlConstants.istrActivityLogUpdateQuery] = lobjUserInfo.iblnLogUpdateQuery;
        //Framework.SessionForWindow[utlConstants.istrActivityLogDeleteQuery] = lobjUserInfo.iblnLogDeleteQuery;
        
        //Test code to store if person has Vip Access Role is Session
        int lcount = 0;
        if (!string.IsNullOrEmpty(lstrUserName))
        {
            lcount = (int)isrvBusinessTier.DBExecuteScalar("cdoPerson.PersonVIPCheck", new object[1] { lstrUserName }, idictParams);
        }
        if (lcount == 1)
        {
            Framework.SessionForWindow["Logged_In_User_is_VIP"] = "VIPAccessUser";
            idictParams["Logged_In_User_Vip"] = "Y";
        }
       
        idictParams[utlConstants.istrRequestMACAddress] = "00:00:00:00";  // Replace with actual MAC Address
        idictParams[utlConstants.istrRequestIPAddress] = GetIPAddress();
        idictParams[utlConstants.istrRequestMachineName] = Request.UserHostName;
        idictParams[utlConstants.istrConstUserSerialID] = (int)Framework.SessionForWindow["UserSerialID"];
        idictParams[utlConstants.istrConstUserID] = (string)Framework.SessionForWindow["UserID"];
        //FM upgrade: 6.0.0.32 changes
        //idictParams[utlConstants.istrActivityLogLevel] = lobjUserInfo.iintActivityLogLevel;

        if (Request.UserHostName != null)
            idictParams[utlConstants.istrRequestMachineName] = Request.UserHostName;
        else
            idictParams[utlConstants.istrRequestMachineName] = Environment.MachineName;

        idictParams[utlConstants.istrRequestApplicationName] = "InternalPortal";
        idictParams[utlConstants.istrWindowName] = hfldLoginWindowName.Value;
        idictParams[utlConstants.istrRequestInvalidLoginFlag] = "N";

        //FM upgrade: 5.4.22.0, 6.0.0.23, 6.0.0.29 changes
        idictParams[utlConstants.istrClientDetails] = HelperFunction.GetClientDetailsByRequestObject(HttpContext.Current.Request);
        //FM upgrade: 6.0.0.32 changes
        //int lintKeyValue = isrvBusinessTier.LogUserLogin(Context.Session.SessionID, DateTime.Now, idictParams);
        //isrvBusinessTier.LogInstance(Context.Session.SessionID, idictParams);
        int lintKeyValue = 0;
        Framework.SessionForWindow["UserActivityLogId"] = lintKeyValue;
        Framework.SessionForWindow["ActionSource"] = "Login Page:Redirected";

        string lstrURL = "";

		//Store the username in the cookie for future use
		HttpCookie lcokKitsCookie = new HttpCookie(lstrCookieName);
		lcokKitsCookie.Expires = Convert.ToDateTime("1/1/2050");
		lcokKitsCookie.Value = lstrUserName;
		//Check if a cookie with the same name exists
		if (Response.Cookies.Get(lstrCookieName) != null)
		{
			Response.Cookies.Remove(lstrCookieName);
		}
		Response.Cookies.Add(lcokKitsCookie);

        //Set the session timeout based on TIMO constants in cache
        string lstrTimeout = isrvDBCache.GetConstantValue("TIMO");
        if (lstrTimeout != "")
        {
            int lintTimeout = Convert.ToInt32(lstrTimeout);
            Session.Timeout = lintTimeout;
        }

        lstrURL = "wfmDefault.aspx?FormID=wfmMyBasketMaintenance";
        if (lstrInitialPage != "")
        {
            XmlDocument lxmdMenu = new XmlDocument();
            lxmdMenu.Load(Server.MapPath("Web.sitemap"));
            XmlNamespaceManager lxmlNameSpaceManager = new XmlNamespaceManager(lxmdMenu.NameTable);
            lxmlNameSpaceManager.AddNamespace("def", "http://schemas.microsoft.com/AspNet/SiteMap-File-1.0");

            XmlNode lxmnMenu = lxmdMenu.SelectSingleNode("/def:siteMap/def:siteMapNode/def:siteMapNode/def:siteMapNode[@title='" + lstrInitialPage + "']",
                lxmlNameSpaceManager);
            if (lxmnMenu != null)
            {
                lstrURL = HelperFunction.GetAttribute(lxmnMenu, "url");
            }
            lstrURL = "wfmDefault.aspx?FormID=" + lstrInitialPage;
        }


        Framework.Redirect(lstrURL);
      
	}
}
