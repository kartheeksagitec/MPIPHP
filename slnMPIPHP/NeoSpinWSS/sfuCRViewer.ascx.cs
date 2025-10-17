using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class sfuCRViewer : System.Web.UI.UserControl
{
    public string sfwReportName { get; set;}

    public object ReportSource
    {
        get { return CRViewerInner.ReportSource; }
        set { CRViewerInner.ReportSource = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
    }
}