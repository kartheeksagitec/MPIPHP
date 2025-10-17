
using NeoBase.Common;
using NeoSpin.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace Neo.Models
{
    /// <summary>
    /// Shibboleth Principal
    /// </summary>
    public class ShibbolethPrincipal : GenericPrincipal
    {
        public ShibbolethPrincipal()
            : base(new GenericIdentity(GetUserIdentityFromHeaders()), null)
        {
        }

        public int UserSerialID { get; set; } // TODO: Need to check usage of this field

        //const string HTTPEMPLOYEEID = busNeoBaseConstants.SAML.SAML_ASSERSION_PERSON_ID;

        /// <summary>
        /// Get User Identity from Headers
        /// </summary>
        /// <returns>Identity Info</returns>
        public static string GetUserIdentityFromHeaders()
        {
            string lstrIdentifyInfo = string.Empty;

            if (HttpContext.Current.Request.ServerVariables.GetValues("HTTPEMPLOYEEID") != null
                && HttpContext.Current.Request.ServerVariables.GetValues("HTTPEMPLOYEEID").Length > 0)
            {
                lstrIdentifyInfo = HttpContext.Current.Request.ServerVariables.GetValues("HTTPEMPLOYEEID")[0];
            }

            return lstrIdentifyInfo;
        }
    }
}

