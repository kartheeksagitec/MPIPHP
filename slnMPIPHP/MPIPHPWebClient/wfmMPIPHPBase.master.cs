using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sagitec.WebClient;

public partial class wfmMPIPHPBase_master : MasterPage
{
    //public event CommandEventHandler DayPicked;
    //public event CommandEventHandler MonthChanged;
    //public event CommandEventHandler DayRender;
    //public event CommandEventHandler CalendarLoad;

    public wfmMPIPHPBase_master()
    {
        Load += new EventHandler(Page_Load);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        //LoadMenu();
        wfmMainDB lwfmPage = (wfmMainDB)this.Page;

        lnkUserPreferencesStoreState.Attributes.Add("onmouseout", "this.src='App_Themes/" + Page.Theme + "/Image/Top_Head_Icon_Storestate_N.png'");
        lnkUserPreferencesStoreState.Attributes.Add("onmouseover", "this.src='App_Themes/" + Page.Theme + "/Image/Top_Head_Icon_Storestate_R.png'");

        btnMPPrint.Attributes.Add("onmouseout", "this.src='App_Themes/" + Page.Theme + "/Image/Top_Head_Icon_Print_N.png'");
        btnMPPrint.Attributes.Add("onmouseover", "this.src='App_Themes/" + Page.Theme + "/Image/Top_Head_Icon_Print_R.png'");

        btnReturn.Attributes.Add("onmouseout", "this.src='App_Themes/" + Page.Theme + "/Image/Top_Head_Icon_Previous_N.png'");
        btnReturn.Attributes.Add("onmouseover", "this.src='App_Themes/" + Page.Theme + "/Image/Top_Head_Icon_Previous_R.png'");

        btnMPRefresh.Attributes.Add("onmouseout", "this.src='App_Themes/" + Page.Theme + "/Image/Top_Head_Icon_Reresh_N.png'");
        btnMPRefresh.Attributes.Add("onmouseover", "this.src='App_Themes/" + Page.Theme + "/Image/Top_Head_Icon_Reresh_R.png'");

        btnSignoff.Attributes.Add("onmouseout", "this.src='App_Themes/" + Page.Theme + "/Image/Top_Head_Icon_Logoff_N.png'");
        btnSignoff.Attributes.Add("onmouseover", "this.src='App_Themes/" + Page.Theme + "/Image/Top_Head_Icon_Logoff_R.png'");
        btnPositionGrid.Click += new EventHandler(lwfmPage.btnBase_Click); //NEO_CERTIFY_CHANGES

    }

    protected void mnuMain_MenuItemDataBound(object sender, MenuEventArgs e)
    {
        // appened the SessionId to Menu Item URL to Avoid sessin loss 
        string lstrPattern = @"/\(S\(\w*\)\)/";
        string lstrReplaceString = string.Format("/(S({0}))/", Session.SessionID);
        e.Item.NavigateUrl = System.Text.RegularExpressions.Regex.Replace(e.Item.NavigateUrl, lstrPattern, lstrReplaceString, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    }
    //private void LoadMenu()
    //{
    //    mnuMain.Items.Clear();
    //    ArrayList larrMenu = (ArrayList)HttpContext.Current.Session["UserMenu"];
    //    foreach (MenuItem lmnuItem in larrMenu)
    //    {
    //        mnuMain.Items.Add(lmnuItem);
    //    }
    //}

}
