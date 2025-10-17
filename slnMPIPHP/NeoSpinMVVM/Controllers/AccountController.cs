#region [Using directives]
using NeoBase.Common;
using Sagitec.Common;
using Sagitec.MVVMClient;
using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.WebPages;
using System.Collections.Generic;
using System.Net;
using System.Security.Principal;
#endregion [Using directives]

namespace Neo.Controllers
{
    /// <summary>
    /// Controller Neo.Controllers.AccountController
    /// </summary>
    public class AccountController : AccountControllerBase
    {

        #region [Protected]
        /// <summary>
        /// Overridden BeforeLoginRedirect to bind footer with the application details
        /// </summary>
        /// <param name="astrReturnUrl">astrReturnUrl</param>
        /// <returns>string</returns>
        protected override string BeforeLoginRedirect(string astrReturnUrl)
        {
            UiHelperFunction.DoesUserHasPrivacySettingAccess(isrvServers, idictParams, Convert.ToInt32(iobjSessionData["UserSerialID"]));
            // Enhancement: To Log Messages in Process Log when User Login to the application 
            iobjSessionData["Landing_Page"] = iobjSessionData["InitialPage"];
            Hashtable lhstParams = new Hashtable();
            lhstParams.Add("astrInput", "Login");
            //isrvServers.isrvBusinessTier.ExecuteMethod("UpdateProcessLog", lhstParams, false, idictParams);
            return base.BeforeLoginRedirect(astrReturnUrl);
        }
        #endregion


        #region [Override]
        public override bool ValidateInternalUser()
        {
            iblnLoginValid = false;
            utlUserInfo lobjUserInfo = null;

            imdlLogin.UserName = imdlLogin.UserName.Trim();
            imdlLogin.Password = imdlLogin.Password.Trim();

            string lstrUserName = imdlLogin.UserName.Trim().Replace("'", "''");
            string lstrPassword = imdlLogin.Password.Trim().Replace("'", "''");
            string lstrInitialPage = string.Empty;

            if ((Regex.IsMatch(lstrUserName, "^[@_!a-zA-Z0-9.\\ -]+$") == false) || lstrUserName.Contains("--"))
            {
                imdlLogin.Message = "Invalid username or password";
                return false;
            }

            try
            {
                string lstrApplicationName = ConfigurationManager.AppSettings.AllKeys.Contains("ApplicationName") ? ConfigurationManager.AppSettings["ApplicationName"] : string.Empty;
                lobjUserInfo = isrvServers.isrvDbCache.ValidateUser(imdlLogin.UserName, imdlLogin.Password, lstrApplicationName);
            }
            catch (Exception ce)
            {
                imdlLogin.Message = "Unable to connect to Database Cache Server. Please make sure that the application servers are up and running, following error occured : " + ce.Message;
                return false;
            }
            SetTraceInfo(iobjTraceInfo);
            if (iobjTraceInfo.iobjLogInfo != null && ((iobjTraceInfo.iobjLogInfo.iblnIsMobile && iobjTraceInfo.iobjLogInfo.iblnMobileAccessDenied) || iobjTraceInfo.iobjLogInfo.iblnDesktopAccessDenied))
            {
                imdlLogin.Message = "Application access is turned off for this device.";
                return false;
            }
            // If the system is available to Administrator only
            if ((istrSystemAvailabilityValue == "ADMN") && !(lobjUserInfo.istrUserType == "A" || lobjUserInfo.istrUserType == "SPUR"))
            {
                imdlLogin.Message = "Only administrators are allowed to access the system";
                return false;
            }

            SetParams(lobjUserInfo);

            if (!lobjUserInfo.iblnAuthenticated)
            {
                string lstrRequestFrom = "";
                utlTraceInfo lobjTraceInfo = HttpContext.Items["TraceInfo"] as utlTraceInfo;
                if (lobjTraceInfo != null)
                {
                    if (!idictParams.ContainsKey(utlConstants.istrWebServerName))
                        idictParams.Add(utlConstants.istrWebServerName, lobjTraceInfo.istrWebServer);

                    if (!string.IsNullOrEmpty(lobjTraceInfo.istrRequestUrl) && lobjTraceInfo.istrRequestUrl.Length > 500)
                        lstrRequestFrom = lobjTraceInfo.istrRequestUrl.Substring(0, 499);
                    else
                        lstrRequestFrom = lobjTraceInfo.istrRequestUrl;
                    if (!idictParams.ContainsKey(utlConstants.istrRequestFrom))
                        idictParams.Add(utlConstants.istrRequestFrom, lstrRequestFrom);
                }

                imdlLogin.Message = lobjUserInfo.istrMessage;
                idictParams[utlConstants.istrRequestInvalidLoginFlag] = "Y";
                idictParams[utlConstants.istrClientDetails] = HelperFunction.GetClientDetailsByRequestObject(Request);
                isrvServers.isrvBusinessTier.LogInstance(Session.SessionID, idictParams);
                idictParams.Remove(utlConstants.istrClientDetails);
                return false;
            }
            idictParams[utlConstants.istrRequestInvalidLoginFlag] = "N";

            //Added the below lines as per framework - Security fixes
            //Session.Clear();//7783
            string lstrInitialPageMode = Convert.ToString(iobjSessionData["InitialPageMode"]);
            lstrInitialPage = Convert.ToString(iobjSessionData["InitialPage"]);

            iobjSessionData.Clear(isrvServers.isrvBusinessTier);
            if (!string.IsNullOrEmpty(lstrInitialPageMode))
            {
                iobjSessionData["InitialPageMode"] = lstrInitialPageMode;
            }
            if (!string.IsNullOrEmpty(lstrInitialPage))
            {
                iobjSessionData["InitialPage"] = lstrInitialPage;
                idictParams["InitialPage"] = lstrInitialPage;
            }
            idictParams["SystemRegion"] = istrRegionValue;

            //Language was getting cleared
            if (string.IsNullOrEmpty(imdlLogin.Language))
            {
                idictParams[utlConstants.istrLanguage] = utlConstants.istrDefaultCultureLanguage;
            }
            else
            {
                idictParams[utlConstants.istrLanguage] = imdlLogin.Language;
            }
            string lstrUserFullName = string.Empty;
            if (!String.IsNullOrEmpty(lobjUserInfo.istrMiddleInitial))
            {
                lstrUserFullName = lobjUserInfo.istrFirstName + " " + lobjUserInfo.istrMiddleInitial + " " + lobjUserInfo.istrLastName;
            }
            else
            {
                lstrUserFullName = lobjUserInfo.istrFirstName + " " + lobjUserInfo.istrLastName;
            }

            idictParams[utlConstants.istrUserType] = lobjUserInfo.istrUserType;
            iobjSessionData["UserName"] = (string.IsNullOrEmpty(lobjUserInfo.istrFirstName) ? "" : lobjUserInfo.istrFirstName + ", ") + (string.IsNullOrEmpty(lobjUserInfo.istrLastName) ? "" : lobjUserInfo.istrLastName);
            iobjSessionData["UserFullName"] = lstrUserFullName;
            iobjSessionData["UserFirstName"] = lobjUserInfo.istrFirstName;
            iobjSessionData["UserLastName"] = lobjUserInfo.istrLastName;
            iobjSessionData["UserEmailId"] = lobjUserInfo.istrEmailId;
            iobjSessionData["UserID"] = lstrUserName;
            iobjSessionData["UserSerialID"] = lobjUserInfo.iintUserSerialId;
            iobjSessionData["UserInfo"] = isrvServers.isrvDbCache.GetUserInfo(lstrUserName);
            iobjSessionData["UserSecurity"] = isrvServers.isrvBusinessTier.GetUserSecurity(lstrUserName, idictParams);
            iobjSessionData["ColorScheme"] = lobjUserInfo.istrColorScheme;

            int lcount = 0;
            if (!string.IsNullOrEmpty(lstrUserName))
            {
                lcount = (int)isrvServers.isrvBusinessTier.DBExecuteScalar("cdoPerson.PersonVIPCheck", new object[1] { lstrUserName }, idictParams);
            }
            if (lcount == 1)
            {
                iobjSessionData["Logged_In_User_is_VIP"] = "VIPAccessUser";
                idictParams["Logged_In_User_is_VIP"] = "VIPAccessUser";
                iobjSessionData["Logged_In_User_Vip"] = "Y";
                idictParams["Logged_In_User_Vip"] = "Y";
            }

            idictParams[utlConstants.istrConstUserSerialID] = (int)iobjSessionData["UserSerialID"];
            idictParams[utlConstants.istrConstUserID] = (string)iobjSessionData["UserID"];
            idictParams["istrAppServer"] = iobjTraceInfo.istrAppServer;


            if (!idictParams.ContainsKey("InitialPage") || string.IsNullOrEmpty(idictParams["InitialPage"]?.ToString()))
            {
                idictParams["InitialPage"] = string.IsNullOrEmpty(lobjUserInfo.istrInitialPage) ? "wfmMyBasketMaintenance" : lobjUserInfo.istrInitialPage;
            }
            iblnLoginValid = true;

            Hashtable lhstParams = new Hashtable();
            lhstParams.Add("astrPassword", lstrPassword);
            string lstrEncryptedPassword = (string)isrvServers.isrvBusinessTier.ExecuteMethod("EncryptPassword", lhstParams, false, new Dictionary<string, object>());
            iobjSessionData["AccessDenied"] = lstrEncryptedPassword;
            idictParams["AccessDenied"] = lstrEncryptedPassword;

            return true;
        }

        [HttpGet]
        //public override ActionResult Login(string astrReturnUrl)
        //{
        //    //if (this.HttpContext.User.IsNotNull() && this.HttpContext.User.Identity.Name.IsNotNullOrEmpty())
        //    {
        //        HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
        //    }
        //    return base.Login(astrReturnUrl);
        //}

        public override ActionResult Login(string astrReturnUrl)
        {
            //if (this.HttpContext.User.IsNotNull() && this.HttpContext.User.Identity.Name.IsNotNullOrEmpty())
            if (!BaseDelegatingHandler.iblnWindowNameCheck && ValidateMVVMSessionValidateCookie(iobjSessionData.idictParams))
            {
                if (astrReturnUrl == null)
                {
                    if (System.Configuration.ConfigurationManager.AppSettings["IsRootPath"] != null &&
                       Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsRootPath"]))
                    {
                        return Redirect("/");
                    }
                    else
                    {
                        return Redirect("/" + Request.Url.AbsolutePath.Split('/')[1]);
                    }
                }
                else
                {
                    if (astrReturnUrl.EndsWith("/"))
                    {
                        astrReturnUrl = astrReturnUrl.Remove(astrReturnUrl.Length - 1);
                    }
                    return Redirect(astrReturnUrl);
                }
            }
            else
            {
                HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
                return base.Login(astrReturnUrl);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult Login(LoginModel model, string astrReturnUrl)
        {
            return base.Login(model, astrReturnUrl);
        }
        #endregion

    }
}
