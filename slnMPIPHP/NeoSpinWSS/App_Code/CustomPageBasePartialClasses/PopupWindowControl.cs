using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

/// <summary>
/// Summary description for PopupWindowControl
/// </summary>
[ToolboxData("<{0}:PopupWindowControl runat=server></{0}:PopupWindowControl>")]
public class PopupWindowControl : Control
{
    private string _istrURL;
    private string _istrTitle;
    private string _istrFeatures;

    public PopupWindowControl(string astrURL) : this(astrURL, string.Empty, string.Empty) { }

    public PopupWindowControl(string astrURL, string astrTitle, string astrFeatures)
    {
        _istrURL = astrURL;
        _istrTitle = astrTitle;
        _istrFeatures = astrFeatures;
    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        string script = "<script type=\"text/javascript\" language=\"javascript\">function dopopup(){window.open('" + _istrURL + "','" + _istrTitle + "','" + _istrFeatures + "',false);} dopopup();</script>";
        ScriptManager.RegisterStartupScript(this, GetType(),
                      "ServerControlScript", script, false);
    }
}