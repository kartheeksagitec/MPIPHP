using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Net.Mail;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Sagitec.WebClient;

public partial class wfmDefaultError : wfmMainDB
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected override void OnInit(EventArgs e)
    {
        istrFormName = "wfmDefaultError";
        base.OnInit(e);
    }

    protected override void OnLoadComplete(EventArgs e)
    {
        base.OnLoadComplete(e);
        txtDescription.Text = (string)Framework.SessionForWindow["ErrorMessage"];
        txtError.Text = (string)Framework.SessionForWindow["ErrorDetails"];
        txtError.Text += "\r\n";
        txtError.Text += "\r\n" + "---> User added information started";
        txtError.Text += "\r\n";
        txtError.Text += "\r\n" + "<--- User added information completed ";
        txtError.Text += "\r\n";
    }

    protected void btnSend_Click(object sender, EventArgs e)
    {
        string lstrSubject = "KCPSRS Exception Notification";
        string lstrBody = txtDescription.Text + "\r\n" + " Error Details : " + "\r\n" + 
            (string)Framework.SessionForWindow["ErrorDetails"];
        string lstrMailTo = ConfigurationManager.AppSettings["MailTo"];
        string lstrSmptpServer = ConfigurationManager.AppSettings["SmtpServer"];
        SmtpClient lsmc = new SmtpClient(lstrSmptpServer);
        lsmc.Send(istrUserID + "@kcpsrs.org", lstrMailTo, lstrSubject, lstrBody);
        lblResult.Text = "E-mail has been sent. Thank you.";
    }
   
}
