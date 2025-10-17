using System;
using System.Data;
using System.Xml;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using Sagitec.Interface;
using Sagitec.Common;
using Sagitec.WebClient;
using MPIPHP.Authentication;
using MPIPHP.Interface;
using MPIPHP.BusinessObjects;
using Sagitec.BusinessObjects;
//using EncryptionDecryptionClassLibrary;
using System.Collections.Generic;

public partial class wfmEntryPoint_aspx : Page
{
    // Page events are wired up automatically to methods 
    // with the following names:
    // Page_Load, Page_AbortTransaction, Page_CommitTransaction,
    // Page_DataBinding, Page_Disposed, Page_Error, Page_Init, 
    // Page_Init Complete, Page_Load, Page_LoadComplete, Page_PreInit
    // Page_PreLoad, Page_PreRender, Page_PreRenderComplete, 
    // Page_SaveStateComplete, Page_Unload  

    private IBusinessTier isrvBusinessTier;
    private IDBCache isrvDBCache;
    private IMetaDataCache isrvMetaDataCache;

    private void ConnectToServers()
    {

        string istrConfigMetaDataCacheURL = ConfigurationManager.AppSettings["MetaDataCacheUrl"].Split(new char[1] { ';' })[0];
        string istrConfigDBCacheURL = ConfigurationManager.AppSettings["DBCacheUrl"].Split(new char[1] { ';' })[0];
        string istrConfigBusinessTierURL = ConfigurationManager.AppSettings["BusinessTierUrl"].Split(new char[1] { ';' })[0];

        // To Do :- Make sure to add a new connection string in AppSettings that connects to the MPIPHP website database.
        // This db connection will be used to authenticate the user.

        //string istrMetaDataCacheProxy = ConfigurationManager.AppSettings["MetaDataCacheProxy"];
        //string istrDBCacheProxy = ConfigurationManager.AppSettings["DBCacheProxy"];
        //string istrBusinessTierProxy = ConfigurationManager.AppSettings["BusinessTierProxy"];

        //FM upgrade changes - Remoting to WCF - commented because no ref found for isrvMetaDataCache and isrvDBCache
        //isrvMetaDataCache = (IMetaDataCache)Activator.GetObject(typeof(IMetaDataCache), istrConfigMetaDataCacheURL);
        //isrvDBCache = (IDBCache)Activator.GetObject(typeof(IDBCache), istrConfigDBCacheURL);
        //try
        //{
        //    isrvMetaDataCache = WCFClient<IMetaDataCache>.CreateChannel(istrConfigMetaDataCacheURL);
        //    isrvDBCache = WCFClient<IDBCache>.CreateChannel(istrConfigDBCacheURL);
        //}
        //finally
        //{
        //    HelperFunction.CloseChannel(isrvMetaDataCache);
        //    HelperFunction.CloseChannel(isrvDBCache);
        //}
        // Setup MetaData Cache and DB Cache connections
        //isrvMetaDataCache = srv.GetObject(istrMetaDataCacheProxy, istrConfigMetaDataCacheURL);
        //isrvDBCache = srvDBCacheProxy.GetObject(istrDBCacheProxy, istrConfigDBCacheURL);


        string lstrRemoteObject = "srvAdmin";

        string istrBusinessTierUrl = istrConfigBusinessTierURL + lstrRemoteObject;

        // Setup Business Tier connection
        //isrvBusinessTier = srvMainDBAccessProxy.GetObject(istrBusinessTierProxy, istrBusinessTierUrl);
        //FM upgrade changes - Remoting to WCF - commented because no ref found for isrvBusinessTier
        //isrvBusinessTier = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), istrBusinessTierUrl);
        //try
        //{
        //    isrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(istrBusinessTierUrl);
        //}
        //finally
        //{
        //    HelperFunction.CloseChannel(isrvBusinessTier);
        //}
        Framework.SessionForWindow["BusinessTierUrl"] = istrBusinessTierUrl;
    }

    protected override void OnInit(EventArgs aEventArgs)
    {
        IBusinessTier isrvNeoSpinMSSBusinessTier = null;
        try
        {
            ConnectToServers();
            string lstrInitialPage = "";
            int lstrUserSerialId;
            string lstrCustomAccessTime = string.Empty;
            //HttpCookie MyCookie = FormsAuthentication.GetAuthCookie(User.Identity.Name.ToString(), false);
            //MyCookie.Domain = "mpiphp.org";
            //Response.AppendCookie(MyCookie);
            //FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(MyCookie.Value);
            //string lstrUserName = ticket.Name;


            string lstrUserName = null;
            HttpCookieCollection MyCookieCollection = Request.Cookies;
            HttpCookie ChkMyCookie = MyCookieCollection.Get("MPID");

            if (MyCookieCollection != null && MyCookieCollection.Count > 0 && ChkMyCookie != null)
            {
                HttpCookie MyCookie = ChkMyCookie;
                if (MyCookie.Value.IsNotNullOrEmpty())
                {
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(MyCookie.Value);
                    lstrUserName = ticket.UserData;
                }
                else
                {
                    lstrUserName = "NULL";//ticket.UserData;
                    // to do - add an exception handler if the MPID is not found.
                }
            }
            else
            {
                HttpCookie MPID_MSS_LOGIN_FROM_APPLICATION = MyCookieCollection.Get("MPID_MSS_LOGIN_FROM_APPLICATION");
                if (MPID_MSS_LOGIN_FROM_APPLICATION.Value.IsNotNullOrEmpty())
                {
                    lstrUserName = MPID_MSS_LOGIN_FROM_APPLICATION.Value;
                }
                else
                {
                    HttpCookie MyCookie = FormsAuthentication.GetAuthCookie(User.Identity.Name.ToString(), true);
                    MyCookie.Domain = "mpiphp.org";
                    Response.AppendCookie(MyCookie);
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(MyCookie.Value);
                    lstrUserName = ticket.Name;
                }

            }

            string lstrUrl = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvMPIPHPMSS");
            //FM upgrade changes - Remoting to WCF
            //IBusinessTier isrvNeoSpinMSSBusinessTier = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), lstrUrl);
            isrvNeoSpinMSSBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);
            Dictionary<string, object> ldctParams = new Dictionary<string, object>();
            ldctParams[utlConstants.istrConstUserID] = "ppunjabi";
            //ldctParams[utlConstants.istrConstUserSerialID] = "119";
            ldctParams[utlConstants.istrConstFormName] = "wfmLogin.aspx";

            DataTable ldtbUserSecurity = new DataTable();
            try
            {
                ldtbUserSecurity.Columns.Add("user_id", typeof(string));
                ldtbUserSecurity.Columns.Add("user_type_vaue", typeof(string));
                ldtbUserSecurity.Columns.Add("resource_id", typeof(int));
                ldtbUserSecurity.Columns.Add("resource_description", typeof(string));
                ldtbUserSecurity.Columns.Add("security_level", typeof(int));

                //ldtbUserSecurity = isrvNeoSpinMSSBusinessTier.GetUserSecurity("sjain", ldctParams);
            }
            catch (Exception)
            {
                //lblError.Text = "Unable to connect to internal Servers.";
                //iblnErrorOccured = true;
                return;
            }

            string lstrWindowName = Guid.NewGuid().ToString();// Request["FWN"].ToString();
            //FM upgrade: 6.0.0.32 changes
            //wfmMainDB.SetAuthenticatedWindow(lstrWindowName);
            wfmMainDB.SetAuthenticatedWindow();

            Framework.SessionForWindow["UserID"] = "MSS";

            Framework.SessionForWindow["UserMPID"] = lstrUserName;

            string lstrUserNameCookie = "MSSExternalUserName";//For MSS Layout Change
            HttpCookie lcokNeoSpinCookie = new HttpCookie(lstrUserNameCookie);
            lcokNeoSpinCookie.Expires = Convert.ToDateTime("1/1/2050");
            lcokNeoSpinCookie.Value = lstrUserName;

            //Check if a cookie with the same name exists
            if (Response.Cookies.Get(lstrUserNameCookie) != null)
            {
                Response.Cookies.Remove(lstrUserNameCookie);
            }
            Response.Cookies.Add(lcokNeoSpinCookie);

            Hashtable lhstParam = new Hashtable();
            lhstParam.Add("astrMpiPersonId", lstrUserName);
            busPerson lobjPerson = (busPerson)isrvNeoSpinMSSBusinessTier.ExecuteMethod("LoadPersonWithMPID", lhstParam, false, ldctParams);
            DataRow ldtRow = ldtbUserSecurity.NewRow();
            if (lobjPerson.icdoPerson.person_id > 0)
            {
                Framework.SessionForWindow["UserSerialID"] = lobjPerson.icdoPerson.person_id;
                Framework.SessionForWindow["UserName"] = lobjPerson.icdoPerson.istrFullName;

                ldtRow["user_id"] = lobjPerson.icdoPerson.mpi_person_id;
                ldtRow["user_type_vaue"] = Convert.ToString("A");
                ldtRow["resource_id"] = 790;
                ldtRow["resource_description"] = Convert.ToString("MSS SCREENS");
                ldtRow["security_level"] = 5;
                ldtbUserSecurity.Rows.Add(ldtRow);
                Framework.SessionForWindow["UserSecurity"] = ldtbUserSecurity;

                Framework.SessionForWindow["PersonID"] = lobjPerson.icdoPerson.person_id;
                Framework.SessionForWindow["MPID"] = lobjPerson.icdoPerson.mpi_person_id;
                Framework.SessionForWindow["MSSDisplayName"] = lobjPerson.icdoPerson.istrFullName;
                Framework.SessionForWindow["M_SSN"] = lobjPerson.icdoPerson.ssn;
                Framework.SessionForWindow["RETIREE"] = lobjPerson.istrRetiree;
                Framework.SessionForWindow["CURRENT_YEAR_RETIREE"] = lobjPerson.istrRetiredInCurrentYear;
            }

            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('" + lstrUserName + "');", true);
                return;
            }

            //ArrayList larrMenu = wfmMainDB.LoadMenu(Server.MapPath("Web.sitemap"), isrvMetaDataCache);
            //Session["UserMenu"] = larrMenu;
            string lstrCurrentForm = string.Empty;

            if (lobjPerson.istrRetiree == busConstant.FLAG_YES)
            {
                lstrCurrentForm = "wfmMSSRetireeMemberHomeMaintenance";
                lstrUrl = "wfmDefault.aspx?FormID=wfmMSSRetireeMemberHomeMaintenance";
            }
            else
            {
                lstrCurrentForm = busConstant.MSS.ACTIVE_MEMBER_HOME;
                lstrUrl = "wfmDefault.aspx?FormID=wfmMSSActiveMemberHomeMaintenance";
            }
            Framework.SessionForWindow["IsNewSession"] = "true";
            Framework.SessionForWindow["RedirecOnLogoff"] = "/mympi/";
            Framework.SessionForWindow["CurrentForm"] = lstrCurrentForm;
            Framework.Redirect(lstrUrl);
        }
        catch (Exception ex)
        {
            if (ex.Message.StartsWith("Unable to connect to MetaDataCache server at address"))
            {
            }
            else
            {
                throw;
            }
        }
        finally
        {
            HelperFunction.CloseChannel(isrvNeoSpinMSSBusinessTier);
        }
    }
    //string lstrUserNameCookie = "NeoSpinCookie";


    const string _istrSiteMapXPath = "/def:siteMap/def:siteMapNode/def:siteMapNode/def:siteMapNode[@title='{0}']";


}

