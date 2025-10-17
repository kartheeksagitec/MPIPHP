<%@ application language="C#" %>
<%@ import namespace="System.Diagnostics" %>
<%@ import namespace="System.ComponentModel" %>
<%@ import namespace="System.Web.UI" %>
<%@ import namespace="System.Collections" %>
<%@ import Namespace="Sagitec.Interface" %>
<%@ import Namespace="Sagitec.WebControls" %>
<%@ Import Namespace="Sagitec.WebClient" %>

<script runat="server">

    void Application_Start(Object sender, EventArgs e) 
    {
        // Code that runs on application startup
        //EventLog.WriteEntry("PensionWebClient", "Application started", EventLogEntryType.Information);

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
    }

    void Session_End(Object sender, EventArgs e) 
    {
        //HttpContext.Current.Session["RedirecOnLogoff"] = "https://10.100.104.108/mympi/";
        //HttpContext.Current.Response.Redirect("https://10.100.104.108/mympi/");
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.
        IBusinessTier lsrvBusinessTier = null;
        try
        {
            if (Session.Keys.Count > 0)
            {
                string lstrURL = ConfigurationManager.AppSettings["BusinessTierUrl"] + "srvAdmin";
                //FM upgrade changes - Remoting to WCF
                //lsrvBusinessTier = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), lstrURL);
                lsrvBusinessTier = Sagitec.Common.WCFClient<IBusinessTier>.CreateChannel(lstrURL);
                Dictionary<string,object> ldctParams = new Dictionary<string,object>();
                ldctParams[Sagitec.Common.utlConstants.istrConstUserID] = Session["UserID"].ToString();
                ldctParams[Sagitec.Common.utlConstants.istrConstFormName] = "N/A"; 
                lsrvBusinessTier.StoreProcessLog("Session timeout occured", ldctParams);
                Response.Redirect("Login.aspx");
            }
        }
        catch
        {
        }
        finally
        {
            Sagitec.Common.HelperFunction.CloseChannel(lsrvBusinessTier);
        }
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
