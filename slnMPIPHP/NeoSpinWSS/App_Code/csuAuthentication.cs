using System;
using System.Collections;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Sagitec.Interface;
using Sagitec.Common;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Sagitec.WebClient;

/// <summary>
/// Summary description for csuAuthentication
/// </summary>
namespace MPIPHP.Authentication
{
    public class Authentication
    {
        public Authentication()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public string ValidateUser(IDBCache asrvDBCache, IBusinessTier asrvBusinessTier,
            string astrUserName, string astrPassword, out string astrFormName)
        {
            string lstrResult = null;
            astrFormName = "";
            utlUserInfo lobjUserInfo = new utlUserInfo();
            if (Regex.IsMatch(astrUserName, "^[a-zA-Z0-9.\\ ]+$") == false)
            {
                lstrResult = "Invalid username or password";
                return lstrResult;
            }

            try
            {
                //FM upgrade: 6.0.2.1 changes - ValidateUser method accepts additional parameter ApplicationName
                string lstrApplicationName = ConfigurationManager.AppSettings["ApplicationName"].ToString();
                lobjUserInfo = asrvDBCache.ValidateUser(astrUserName, astrPassword, lstrApplicationName);
            }
            catch (Exception ce)
            {
                lstrResult = "Unable to connect to Database Cache Server. Please make sure that the application servers are up and running, following error occured : " + ce.Message;
                return lstrResult;
            }

            if (!lobjUserInfo.iblnAuthenticated)
            {
                lstrResult = lobjUserInfo.istrMessage;
                return lstrResult;
            }


            Framework.SessionForWindow["UserID"] = astrUserName;
            Framework.SessionForWindow["UserSerialID"] = lobjUserInfo.iintUserSerialId;
            Framework.SessionForWindow["UserType"] = lobjUserInfo.istrUserType;
            Framework.SessionForWindow["UserInfo"] = asrvDBCache.GetUserInfo(astrUserName);
            Framework.SessionForWindow["UserSecurity"] = asrvBusinessTier.GetUserSecurity(astrUserName, new Dictionary<string, object>());
            Framework.SessionForWindow["UserName"] = lobjUserInfo.istrLastName + ", " + lobjUserInfo.istrFirstName;
            Framework.SessionForWindow["ColorScheme"] = lobjUserInfo.istrColorScheme;

            //FM upgrade: 6.0.0.32 changes
            //Framework.SessionForWindow[utlConstants.istrActivityLogLevel] = lobjUserInfo.iintActivityLogLevel;
            //Framework.SessionForWindow[utlConstants.istrActivityLogSelectQuery] = lobjUserInfo.iblnLogSelectQuery;
            //Framework.SessionForWindow[utlConstants.istrActivityLogInsertQuery] = lobjUserInfo.iblnLogInsertQuery;
            //Framework.SessionForWindow[utlConstants.istrActivityLogUpdateQuery] = lobjUserInfo.iblnLogUpdateQuery;
            //Framework.SessionForWindow[utlConstants.istrActivityLogDeleteQuery] = lobjUserInfo.iblnLogDeleteQuery;

            Hashtable lhstParams = new Hashtable();
            lhstParams.Add("astrPassword", astrPassword);
            string lstrEncryptedPassword = (string)asrvBusinessTier.ExecuteMethod("EncryptPassword", lhstParams, false, new Dictionary<string, object>());
            Framework.SessionForWindow["AccessDenied"] = lstrEncryptedPassword;

            astrFormName = lobjUserInfo.istrInitialPage;
            return lstrResult;
        }
    }
}
