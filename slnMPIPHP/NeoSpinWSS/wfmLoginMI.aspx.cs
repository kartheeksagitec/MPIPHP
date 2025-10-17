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
using Sagitec.Interface;
using Sagitec.Common;
using Sagitec.WebClient;
using System.Collections.Generic;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using Sagitec.DataObjects;
using MPIPHP.Interface;
using MPIPHP.BusinessObjects;

public partial class wfmLoginMI : wfmMainDB
{
    // Page events are wired up automatically to methods 
    // with the following names:
    // Page_Load, Page_AbortTransaction, Page_CommitTransaction,
    // Page_DataBinding, Page_Disposed, Page_Error, Page_Init, 
    // Page_Init Complete, Page_Load, Page_LoadComplete, Page_PreInit
    // Page_PreLoad, Page_PreRender, Page_PreRenderComplete, 
    // Page_SaveStateComplete, Page_Unload
    
    string lstrUserNameCookie = "MWPInternalUserName";//For MSS Layout Change
    string lstrUserTypeCookie = "NeoSpinUserType";
    protected override void OnInit(EventArgs e)
    {
        istrFormName = "wfmLoginMI";
        Session["IsNewSession"] = "false";
        IBusinessTier lsrvBusinessTier = null;
        try
        {
            string lstrapplicationpath = Request.ApplicationPath;
            string lstrProjectAndRegion = ConfigurationManager.AppSettings["ProjectAndRegion"];
            utlUserInfo lobjUserInfo = (utlUserInfo)Framework.SessionForWindow["UserInfoObject"];
            string lstrBusinessTierUrl = (string)String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvAdmin");
            //FM upgrade changes - Remoting to WCF
            //IBusinessTier lsrvBusinessTier = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), lstrBusinessTierUrl);
            lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrBusinessTierUrl);

            HttpCookie MyCookie = FormsAuthentication.GetAuthCookie(User.Identity.Name.ToString(), false);

            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(MyCookie.Value);

            base.OnInit(e);
        }
        catch (Exception)//For MSS Layout Change
        {
            lblError.Text = "Unable to connect to internal Servers.";
            iblnErrorOccured = true;
            return;
        }
        finally
        {
            HelperFunction.CloseChannel(lsrvBusinessTier);
        }
    }
    protected override void OnLoadComplete(EventArgs e)
    {
        if (Framework.SessionForWindow["ApplicationError"] != null  && Framework.SessionForWindow["UserIDInvalid"] !=null)
        {
            Framework.SessionForWindow["ApplicationError"] = null;
        }
        base.OnLoadComplete(e);
    }

    protected void Page_Load(object sender, EventArgs e)//For MSS Layout Change
    {
        //Check if this is the first time - check if a cookie is present
        //If it is, then retrieve its value and put it in the user name
        if (!this.IsPostBack)
        {
            HttpCookie lcokWSSCookie = Request.Cookies.Get(lstrUserNameCookie);
            if (lcokWSSCookie != null)
            {
                txtUserId.Text = lcokWSSCookie.Value;
                txtPassword.Focus();
            }
            else
            {
                txtUserId.Focus();
            }

        }
        //Framework.SessionRemove("UserInfoObject");
        Session.Remove("UserInfoObject");
    }

    //For MSS Layout Change
    protected void btnLogin_Click(object sender, EventArgs e)
    {
        if (iblnErrorOccured)
        {
            return;
        }
        if (txtUserId.Text.Trim().Length == 0 || txtPassword.Text.Trim().Length == 0 || txtPersonID.Text.Trim().Length == 0)
            return;

        //FM upgrade: 6.0.0.35 changes - ValidateUser method accepts additional parameter ApplicationName
        string lstrApplicationName = ConfigurationManager.AppSettings["ApplicationName"].ToString();
        utlUserInfo lobjUserInfo = isrvDBCache.ValidateUser(txtUserId.Text, txtPassword.Text, lstrApplicationName);

        if (!lobjUserInfo.iblnAuthenticated)
        {
            Framework.SessionForWindow["UserIDInvalid"] = true;
            lblError.Text = lobjUserInfo.istrMessage;
            return;
        }


        Dictionary<string, object> ldctParams = new Dictionary<string, object>();
        ldctParams[utlConstants.istrConstUserID] = txtUserId.Text;
        ldctParams[utlConstants.istrConstUserSerialID] = lobjUserInfo.iintUserSerialId;
        ldctParams[utlConstants.istrConstFormName] = "wfmLoginMI.aspx";

        DataTable ldtbUserSecurity = new DataTable();
        try
        {
            ldtbUserSecurity.Columns.Add("user_id", typeof(string));
            ldtbUserSecurity.Columns.Add("user_type_vaue", typeof(string));
            ldtbUserSecurity.Columns.Add("resource_id", typeof(int));
            ldtbUserSecurity.Columns.Add("resource_description", typeof(string));
            ldtbUserSecurity.Columns.Add("security_level", typeof(int));
            //ldtbUserSecurity = isrvBusinessTier.GetUserSecurity(txtUserId.Text, ldctParams);
        }
        catch (Exception)
        {
            lblError.Text = "Unable to connect to internal Servers.";
            iblnErrorOccured = true;
            return;
        }

        //FM upgrade: 6.0.0.32 changes
        //wfmMainDB.SetAuthenticatedWindow(hfldLoginWindowName.Value);
        //wfmMainDB.SetAuthenticatedWindow();
        UpdateLogInstance(txtUserId.Text, lobjUserInfo.iintUserSerialId, lobjUserInfo.istrUserType, lobjUserInfo.istrColorScheme, wfmMainDB.istrApplicationName, lobjUserInfo.iblnAuthenticated);

        lobjUserInfo.istrUserId = txtUserId.Text;
        Framework.SessionForWindow["UserID"] = lobjUserInfo.istrUserId;
        Framework.SessionForWindow["UserSerialID"] = lobjUserInfo.iintUserSerialId;
        Framework.SessionForWindow["ColorScheme"] = "ControlsTheme"; // MSS should always bind to ControlsTheme
        //HttpContext.Current.Session["UserType"] = busConstant.UserTypeInternal;
        //Setting the User Security Here to lanuch the Lookup in Impersonate Screen.        
        //HttpContext.Current.Session["UserSecurity"] = ldtbUserSecurity;
        Framework.SessionForWindow["UserName"] = lobjUserInfo.istrLastName + ", " + lobjUserInfo.istrFirstName;

        //Hashtable lhstParams = new Hashtable();
        //lhstParams.Add("astrKey", null);
        //lhstParams.Add("astrValue", txtPassword.Text);
        //string lstrEncryptedPassword = (string)isrvBusinessTier.ExecuteMethod("SagitecEncrypt", lhstParams, false, ldctParams);
        //HttpContext.Current.Session["AccessDenied"] = lstrEncryptedPassword;

        //Store the username in the cookie for future use
        HttpCookie lcokWSSCookie = new HttpCookie(lstrUserNameCookie);
        lcokWSSCookie.Expires = Convert.ToDateTime("1/1/2050");
        lcokWSSCookie.Value = txtUserId.Text;
        //Check if a cookie with the same name exists
        if (Response.Cookies.Get(lstrUserNameCookie) != null)
        {
            Response.Cookies.Remove(lstrUserNameCookie);
        }
        Response.Cookies.Add(lcokWSSCookie);
        string lstrUrl = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvMPIPHPMSS");
        //FM upgrade changes - Remoting to WCF
        //IBusinessTier isrvNeoSpinMSSBusinessTier = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), lstrUrl);
        IBusinessTier isrvNeoSpinMSSBusinessTier = null;
        try
        {
            isrvNeoSpinMSSBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);
            Hashtable lhstParam = new Hashtable();
            Int32 lint32PersonId;
            if (!txtPersonID.Text.IsNumeric() || !int.TryParse(txtPersonID.Text, out lint32PersonId))
            {
                lblError.Text = "Invalid Person ID";
                return;
            }
            lhstParam.Add("aintPersonID", txtPersonID.Text);
            DataRow ldtRow = ldtbUserSecurity.NewRow();
            busPerson lobjPerson = (busPerson)isrvNeoSpinMSSBusinessTier.ExecuteMethod("LoadPerson", lhstParam, false, ldctParams);
            if (lobjPerson.icdoPerson.person_id > 0)
            {

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
                lblError.Text = "Invalid Person ID";
                return;
            }

            int lintUserSerialID = 0;
            if (Framework.SessionForWindow["UserSerialID"] != null)
                lintUserSerialID = (int)Framework.SessionForWindow["UserSerialID"];

            string lstrUserID = lobjPerson.icdoPerson.person_id.ToString() ?? string.Empty;
            if (Framework.SessionForWindow["UserId"] != null)
                lstrUserID = Convert.ToString(Framework.SessionForWindow["UserId"]);

            //csLoginWSSHelper.SetSessionVariables(lobjPerson.icdoPerson.ndpers_login_id,
            //                                    lobjPerson.icdoPerson.last_name,
            //                                    lobjPerson.icdoPerson.first_name,
            //                                    (string)HttpContext.Current.Session["UserType"] ?? string.Empty,
            //                                    lstrUserID,
            //                                    lintUserSerialID,
            //                                    (string)HttpContext.Current.Session["ColorScheme"] ?? string.Empty,
            //                                    lobjPerson.icdoPerson.email_address);

            //Setting the User Security
            //csLoginWSSHelper.SetUserSecurityForMember(lobjPerson.icdoPerson.person_id, isrvNeoSpinMSSBusinessTier);


            //FM upgrade: 6.0.0.32 changes
            //ArrayList larrMenu = wfmMainDB.LoadMenu(Server.MapPath("Web.sitemap"), isrvMetaDataCache);
            ArrayList larrMenu = wfmMainDB.LoadMenu(Server.MapPath("Web.sitemap"), this);
            Framework.SessionForWindow["UserMenu"] = larrMenu;

            //Launching the Portal

            // Set the session timeout based on TIMS constants in cache - PIR 7723
            string lstrTimeout = isrvDBCache.GetConstantValue("TIMS");
            if (lstrTimeout != "")
            {
                int lintTimeout = Convert.ToInt32(lstrTimeout);
                Session.Timeout = lintTimeout;
            }


            FormsAuthentication.SetAuthCookie(txtUserId.Text, false);
            HttpCookie MyCookie = FormsAuthentication.GetAuthCookie(User.Identity.Name.ToString(), false);

            Framework.SessionForWindow["RedirecOnLogoff"] = "https://www.mpiphp.org/mympi/";
            if (lobjPerson.istrRetiree == busConstant.FLAG_YES)
            {
                lstrUrl = "wfmDefault.aspx?FormID=wfmMSSRetireeMemberHomeMaintenance";
            }
            else
            {
                lstrUrl = "wfmDefault.aspx?FormID=wfmMSSActiveMemberHomeMaintenance";
            }
            Framework.Redirect(lstrUrl);
            //string lstrURL = csLoginWSSHelper.GetLaunchURLforMemberPortal(lobjPerson.icdoPerson.person_id, isrvBusinessTier);
            ////Setting the Audit Trail

            //isrvBusinessTier.StoreProcessLog(HttpContext.Current.Session["UserID"].ToString() + " -Internal User successfully logged in to WSS Member Portal", ldctParams);
            //HttpContext.Current.Response.Redirect(lstrURL);
        }
        finally
        {
            HelperFunction.CloseChannel(isrvNeoSpinMSSBusinessTier);
        }
       
    }

    
}
