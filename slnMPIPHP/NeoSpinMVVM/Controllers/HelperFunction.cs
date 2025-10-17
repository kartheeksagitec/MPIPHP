#region [Using directives]
using NeoBase.Common;
using Sagitec.Common;
using Sagitec.Interface;
using Sagitec.MVVMClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Web;

#endregion [Using directives]

namespace Neo.Controllers
{
    /// <summary>
    /// Class Neo.Controllers.UiHelperFunction
    /// </summary>
    public class UiHelperFunction
    {

        //Modified Method For Framework Version 6.0.0.36
        /// <summary>
        /// Get Message 
        /// </summary>
        /// <param name="aintMessageID"></param>
        /// <param name="aarrParam"></param>
        /// <returns>utlResponseMessage</returns>
        public static utlResponseMessage GetMessage(int aintMessageID, object[] aarrParam = null)
        {
            IDBCache lsrvDBCache = GetDBCacheURL();
            try
            {
                utlMessageInfo lobjutlMessageInfo = lsrvDBCache.GetMessageInfo(aintMessageID);
                if (lobjutlMessageInfo == null)
                    return null;
                utlResponseMessage lobjResponseMessage = new utlResponseMessage();
                lobjResponseMessage.istrMessageID = "Msg ID : " + aintMessageID.ToString();
                if (aarrParam == null)
                {
                    lobjResponseMessage.istrMessage = " [ " + lobjutlMessageInfo.display_message + " ]";
                }
                else
                {
                    lobjResponseMessage.istrMessage = " [ " + String.Format(lobjutlMessageInfo.display_message + " ]", aarrParam);
                }

                return lobjResponseMessage;
            }
            finally
            {
                HelperFunction.CloseChannel(lsrvDBCache);
            }
        }

        /// <summary>
        /// THis method will create DB Cache Url 
        /// </summary>
        /// <param name="astrSrvName"></param>
        /// <returns></returns>
        public static IDBCache GetDBCacheURL(string astrSrvName = "srvDBCache")
        {
            return WCFClient<IDBCache>.CreateChannel(string.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], astrSrvName));
        }

        /// <summary>
        /// DS100-ACT01-BR02 : Check whether logged in user has privacy setting access or not 
        /// </summary>
        /// True if privacy setting access else False.
        public static void DoesUserHasPrivacySettingAccess(srvServers asrvServer, Dictionary<string, object> adictParams, int aintUserSerialId)
        {
            var lintPrivacySettingAccess = asrvServer.isrvBusinessTier.DBExecuteScalar("entSecurity.ByUserByResource", new object[2] {aintUserSerialId,
                UIConstants.PERSON_PRIVACY_SETTING_RESOURCE_ID }, adictParams);

            if (adictParams != null)
            {
                adictParams[UIConstants.IS_USER_HAVE_PRIVACY_SETTING_ACCESS] = (lintPrivacySettingAccess != null && Convert.ToInt32(lintPrivacySettingAccess) > 0) ? true : false;
            }
        }


        /// <summary>
        /// Method checks is user has any roles assigned
        /// </summary>
        /// <param name="asrvServer">server</param>
        /// <param name="astrUserID">user_id</param>
        /// <param name="adictParams">parameters</param>
        /// <returns>return TRUE if user has any roles or FALSE</returns>
        public static bool DoesUserHasSecurity(srvServers asrvServer, string astrUserID, Dictionary<string, object> adictParams)
        {
            Hashtable lhstParameter = new Hashtable();
            lhstParameter.Add("user_id", astrUserID);
            int lintResult = asrvServer.isrvBusinessTier.ExecuteQuery("entUser.Security", lhstParameter, adictParams).Rows.Count;
            return lintResult > 0 ? true : false;
        }
        /// <summary>
        /// Method to read Assembly title information
        /// </summary>
        /// <returns> Title of assembly</returns>
        public static string GetProductTitle()
        {
            var lassm = Assembly.GetExecutingAssembly();
            var lvarTitle = (AssemblyTitleAttribute)lassm.GetCustomAttribute(typeof(AssemblyTitleAttribute));
            return Convert.ToString(lvarTitle.Title);
        }

        /// <summary>
        /// Get Hash table 
        /// </summary>
        /// <param name="astrDelimitedString"></param>
        /// <param name="achrDelimiter1"></param>
        /// <param name="achrDelimiter2"></param>
        /// <returns></returns>
        public static Hashtable GetHashtableFromString(string astrDelimitedString, char achrDelimiter1, char achrDelimiter2)
        {
            Hashtable lhstTable = new Hashtable();
            if (!string.IsNullOrEmpty(astrDelimitedString))
            {
                string[] larrParams = astrDelimitedString.Split(achrDelimiter1);
                foreach (string lstrNavParamDetails in larrParams)
                {
                    string[] larrNavParam = lstrNavParamDetails.Split(achrDelimiter2);
                    lhstTable[larrNavParam[0]] = larrNavParam[1];
                }
            }
            return lhstTable;
        }

        /// <summary>
        /// Decrypt File Path
        /// </summary>
        /// <param name="astrFilePath"></param>
        /// <returns></returns>
        public static int DecryptFilePath(string astrFilePath)
        {
            int lintTrackingID = 0;

            if (astrFilePath.IsNotNullOrEmpty())
            {
                string lstrEncryptionKey = ControlsHelper2.GetEncryptionKey(HttpContext.Current.Session.SessionID);
                string lstrEncryptionIV = ControlsHelper2.GetEncryptionIV(HttpContext.Current.Session.SessionID);
                string lstrFilePath = HelperFunction.SagitecDecryptFIPS(astrFilePath, lstrEncryptionKey, lstrEncryptionIV);
                if (lstrFilePath != null)
                {
                    int.TryParse(lstrFilePath.Substring(lstrFilePath.LastIndexOf("-") + 1).Substring(0, lstrFilePath.LastIndexOf(".") - lstrFilePath.LastIndexOf("-") - 1), out lintTrackingID);
                }
                else
                {

                }
            }
            else
            {

            }
            return lintTrackingID;
        }
    }
}