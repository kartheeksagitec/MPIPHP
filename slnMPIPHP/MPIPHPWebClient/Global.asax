<%@ Application Language="C#" %>
<%@ Import Namespace="System.Diagnostics" %>
<%@ Import Namespace="System.ComponentModel" %>
<%@ Import Namespace="System.Web.UI" %>
<%@ Import Namespace="System.Web.Http" %>
<%@ Import Namespace="System.Web.Routing" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="Sagitec.WebClient" %>

<script RunAt="server">

    void Application_Start(Object sender, EventArgs e)
    {
        // Code that runs on application startup
        //EventLog.WriteEntry("PensionWebClient", "Application started", EventLogEntryType.Information);
        RouteTable.Routes.MapHttpRoute(name: "DefaultApi", routeTemplate: "api/{controller}/{action}/{id}",
                                            defaults: new { id = System.Web.Http.RouteParameter.Optional });
    }

    void Application_End(Object sender, EventArgs e)
    {
        //  Code that runs on application shutdown

    }

    // Code that runs when an unhandled error occurs
    protected void Application_Error(object sender, EventArgs e)
    {
    }

    void Session_Start(Object sender, EventArgs e)
    {
        Session["IsNewSession"] = "true";
        // Code that runs when a new session is started
        HttpContext.Current.Response.Redirect("wfmLogin.aspx");
    }

    void Session_End(Object sender, EventArgs e)
    {
        Sagitec.Interface.IBusinessTier lsrvBusinessTier = null;
        try
        {
            if (Session.Keys.Count > 0)
            {
                string lstrURL = ConfigurationManager.AppSettings["BusinessTierUrl"] + "srvAdmin";
                //FM upgrade changes - Remoting to WCF
                //lsrvBusinessTier = (Sagitec.Interface.IBusinessTier)Activator.GetObject(typeof(Sagitec.Interface.IBusinessTier), lstrURL);
                lsrvBusinessTier = Sagitec.Common.WCFClient<Sagitec.Interface.IBusinessTier>.CreateChannel(lstrURL);
                Dictionary<string, object> ldctParams = new Dictionary<string, object>();
                ldctParams[Sagitec.Common.utlConstants.istrConstUserID] = Session["IsNewSession"].ToString();
                ldctParams[Sagitec.Common.utlConstants.istrConstFormName] = "N/A";
                lsrvBusinessTier.StoreProcessLog("Session timeout occured", ldctParams);
            }
        }
        catch
        {
        }
        finally
        {
           Sagitec.Common.HelperFunction.CloseChannel(lsrvBusinessTier);
        }
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.
    }

    //FM upgrade: 6.0.0.32 changes
    protected void Application_BeginRequest(object sender, EventArgs e)
    {
        Framework.LogBeginRequest();
    }
    protected void Application_AcquireRequestState()
    {
        Framework.Initialize();
    }
    protected void Application_PostRequestHandlerExecute(object sender, EventArgs e)
    {
        Framework.LogEndRequest();
    }

</script>
