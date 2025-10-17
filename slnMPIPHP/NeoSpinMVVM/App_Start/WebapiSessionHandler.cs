using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Sagitec.MVVMClient;

namespace Neo
{
    public class WebapiSessionHandler : DelegatingHandler
    {

        protected MVVMSession iobjSessionData;
        public WebapiSessionHandler(HttpConfiguration httpConfiguration)
        {
            InnerHandler = new HttpControllerDispatcher(httpConfiguration);
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!ValidateSession(request))
            {
                var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                var tsc = new TaskCompletionSource<HttpResponseMessage>();
                tsc.SetResult(response);
                return tsc.Task;
            }

            NameValueCollection lcolQueryString = request.RequestUri.ParseQueryString();
            if (lcolQueryString != null && lcolQueryString.Count > 0)
            {
                string lstrSenderID = lcolQueryString.Get("SenderID");
                if (!string.IsNullOrEmpty(lstrSenderID))
                {
                    iobjSessionData["SenderID"] = lstrSenderID;
                }
            }

            return base.SendAsync(request, cancellationToken);
        }

        private bool ValidateSession(HttpRequestMessage message)
        {
            if (iobjSessionData != null)
            {                
                if (HttpContext.Current.Session.IsNewSession)
                {                    
                    string szCookieHeader = HttpContext.Current.Request.Headers["Cookie"];
                    if ((null != szCookieHeader) && (szCookieHeader.IndexOf("ASP.NET_SessionId") >= 0))
                    {
                       return false;
                    }
                }
            }

            return true;
        }
    }  
}


