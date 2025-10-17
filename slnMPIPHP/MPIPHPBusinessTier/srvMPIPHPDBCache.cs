#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using MPIPHP.Interface;
using Sagitec.Interface;
using Sagitec.DBCache;
using Sagitec.Common;
using Sagitec.DBUtility;


#endregion

namespace MPIPHP.BusinessTier
{
	public class srvMPIPHPDBCache: srvDBCache, IMPIPHPDBCache
	{
		public srvMPIPHPDBCache()
		{

		}

        //FM upgrade: 6.0.0.35 changes - ValidateUser method accepts additional parameter ApplicationName
        public override utlUserInfo ValidateUser(string astrUserId, string astrPassword, string astrApplicationName)
        {
            utlUserInfo lobjUserInfo = base.ValidateUser(astrUserId, astrPassword, astrApplicationName);
            if ((lobjUserInfo.iblnAuthenticated) && (lobjUserInfo.istrUserType == "ESSR"))
            {
                IDbConnection lconFramework = DBFunction.GetDBConnection();
                string lstrUrl = MPIPHP.Common.ApplicationSettings.Instance.MetaDataCacheUrl;
                //FM upgrade changes - Remoting to WCF
                //IMetaDataCache lsrvMetaDataCache = (IMetaDataCache)Activator.GetObject(typeof(IMetaDataCache), lstrUrl);
                IMetaDataCache lsrvMetaDataCache = null;
                try
                {
                    lsrvMetaDataCache = WCFClient<IMetaDataCache>.CreateChannel(lstrUrl);
                    if (lsrvMetaDataCache == null)
                    {
                        throw new Exception("Unable to connect to MetaDataCache");
                    }
                    lobjUserInfo.idtbAdditionalInfo =
                        DBFunction.DBSelect(
                        "cdoUserEmployer.ByUser", new object[1] { lobjUserInfo.iintUserSerialId }, lconFramework, null);
                }
                finally
                {
                    HelperFunction.CloseChannel(lsrvMetaDataCache);
                }
            }
            return lobjUserInfo;
        }

        public string GetMPIPHPMessageDescription(int aintMessageID)
        {
            string lstrMessage = null;
            //FM upgrade: 6.0.0.37 changes - return type is changed from DataTable to the class utlMessageInfo
            //DataTable ldtbMessage = GetMessageInfo(aintMessageID);
            //if (ldtbMessage != null && ldtbMessage.Rows.Count == 1)
            //{
            //    lstrMessage = ldtbMessage.Rows[0]["display_message"].ToString();
            //}
            utlMessageInfo lobjutlMessageInfo = GetMessageInfo(aintMessageID);
            if (lobjutlMessageInfo != null)
                lstrMessage = lobjutlMessageInfo.display_message;
            return lstrMessage;
        }
	}
}
