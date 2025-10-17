using Sagitec.Common;
using Sagitec.WebControls;
using MPIPHP.Interface;
using System.Text.RegularExpressions;
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Sagitec.WebClient;
using System.Collections;
using MPIPHP.BusinessObjects;
using Sagitec.BusinessObjects;
using System.Collections.Generic;


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

    public bool iblnMSSEnabled { get; set; }
    public bool iblnESSEnabled { get; set; }
    private bool iblnBenefitEstimateCalculationEnabled = false;
    private Dictionary<string, object> idctParams;
    //MSS Layout Completely modified

    void Page_Load(object sender, EventArgs e)
    {
        LoadMenu();
        iblnESSEnabled = false;
        iblnMSSEnabled = false;

        //hlnkHome.Attributes.Add("onmouseout", "src='images/Button_Head_Home.gif'");
        //hlnkHome.Attributes.Add("onmouseover", "this.src='images/Button_Head_HomeR.gif'");

        //hlnkContactUs.Attributes.Add("onmouseout", "src='images/Button_Head_Contact.gif'");
        //hlnkContactUs.Attributes.Add("onmouseover", "this.src='images/Button_Head_ContactR.gif'");

        //// PIR 9884
        //hlnkForm.Attributes.Add("onmouseout", "src='images/Button_Head_Form.gif'");
        //hlnkForm.Attributes.Add("onmouseover", "this.src='images/Button_Head_FormR.gif'");

        //hlnkSignoff.Attributes.Add("onmouseout", "src='images/Button_Head_Logoff.gif'");
        //hlnkSignoff.Attributes.Add("onmouseover", "this.src='images/Button_Head_LogoffR.gif'");
        //btnMPPrint.Attributes.Add("onmouseout", "this.src='App_Themes/" + Page.Theme + "/Image/Top_Button_Print_N.png'");
        //btnMPPrint.Attributes.Add("onmouseover", "this.src='App_Themes/" + Page.Theme + "/Image/Top_Button_Print_R.png'");

        //btnReturn.Attributes.Add("onmouseout", "this.src='App_Themes/" + Page.Theme + "/image/Top_Button_Back_N.png'");
        //btnReturn.Attributes.Add("onmouseover", "this.src='App_Themes/" + Page.Theme + "/image/Top_Button_Back_R.png'");

        //btnHelp.Attributes.Add("onmouseout", "this.src='App_Themes/" + Page.Theme + "/image/Top_Button_Help_N.png'");
        //btnHelp.Attributes.Add("onmouseover", "this.src='App_Themes/" + Page.Theme + "/image/Top_Button_Help_R.png'");

        //btnSignoff.Attributes.Add("onmouseout", "this.src='App_Themes/" + Page.Theme + "/image/Top_Button_Logout_N.png'");
        //btnSignoff.Attributes.Add("onmouseover", "this.src='App_Themes/" + Page.Theme + "/image/Top_Button_Logout_R.png'");
        //hlnkSwitchMember.Attributes.Add("onmouseout", "src='images/Button_Head_Switch.gif'");
        //hlnkSwitchMember.Attributes.Add("onmouseover", "src='images/Button_Head_SwitchR.gif'");
        //hlnkSwitchAccounts.Attributes.Add("onmouseout", "src='images/Button_Head_SwitchAccount.gif'");
        //hlnkSwitchAccounts.Attributes.Add("onmouseover", "src='images/Button_Head_SwitchAccountR.gif'");
        //btnHome.Attributes.Add("onmouseout", "this.src='App_Themes/" + Page.Theme + "/image/Top_Button_home.png'");
        //btnHome.Attributes.Add("onmouseover", "this.src='App_Themes/" + Page.Theme + "/image/Top_Button_home_R.png'");
        lblUserIdMSS.Text = Convert.ToString(Framework.SessionForWindow["MPID"]);
        if (Framework.SessionForWindow["PersonID"] != null && Convert.ToInt32(Framework.SessionForWindow["PersonID"]) != 0)
        {
            lblUserName.Text = Framework.SessionForWindow["MSSDisplayName"].ToString();
            lblUserIdMSS.Text = Convert.ToString(Framework.SessionForWindow["MPID"]);
            iblnMSSEnabled = true;
            lblMessageID.Visible = false;

            SetNewMessages();
        }
        //if (Session["ContactID"] != null && Convert.ToInt32(Session["ContactID"]) != 0)
        //{
        //    iblnESSEnabled = true;
        //    headerMSS.Visible = false;
        //    mnuMain.DataSourceID = "smpMenu";
        //    WestPanel.Visible = false;
        //    //EastPanel.Visible = false;
        //    tblCenter.Attributes.Add("Class", "Message");
        //    BannerTopLeft.CssClass = "HeaderTopLeftESS";
        //    uppFooter.Visible = false;
        //}
        //For MSS Layout Change
        //if (Session["UserSerialID"].ToString() == "0")
        //    hlnkSwitchMember.Visible = false;

        //if (Session["CurrentForm"] != null && Session["CurrentForm"].ToString() == "wfmMSSDeathNoticeMaintenance")
        //{
        //    hlnkReportDeath.CssClass = "selected";
        //    hlnkReportDeath.Enabled = false;
        //}
        //else if (Session["CurrentForm"] != null && Session["CurrentForm"].ToString() == "wfmMSSAppointmentScheduleMaintenance")
        //{
        //    hlnkAppointment.CssClass = "selected";
        //    hlnkAppointment.Enabled = false;
        //}
        //else if (Session["CurrentForm"] != null && Session["CurrentForm"].ToString() == "wfmMSSContactNDPERSMaintenance")
        //{
        //    hlnkContactNDPERS.CssClass = "selected";
        //    hlnkContactNDPERS.Enabled = false;
        //}
        //else
        //{
        //    hlnkAppointment.CssClass = "";
        //    hlnkReportDeath.CssClass = "";
        //    hlnkContactNDPERS.CssClass = "";
        //    hlnkAppointment.Enabled = true;
        //    hlnkReportDeath.Enabled = true;
        //    hlnkContactNDPERS.Enabled = true;
        //}
    }

    protected override void OnLoad(EventArgs e)
    {
        //if (!Convert.ToBoolean(Session["IsMemberBothRetireeAndActive"]))
        //{
        //    InitializeUserParams();
        //    Session["IsRetiree"] = false;
        //    Hashtable lhstParam = new Hashtable();
        //    lhstParam.Add("aintPersonId", Convert.ToInt32(Session["PersonID"]));
        //    string lstrUrl = ConfigurationManager.AppSettings["BusinessTierUrl"] + "srvMPIPHPMSS";
        //    IMPIPHPBusinessTier isrvNeoSpinBusinessTier = (IMPIPHPBusinessTier)Activator.GetObject(typeof(IMPIPHPBusinessTier), lstrUrl);
        ////    if ((bool)isrvNeoSpinBusinessTier.ExecuteMethod("IsPersonRetiredOrWithdrawnPlan", lhstParam, false, idctParams) ||
        //        (bool)isrvNeoSpinBusinessTier.ExecuteMethod("IsRetiree", lhstParam, false, idctParams) ||
        //        (bool)isrvNeoSpinBusinessTier.ExecuteMethod("IsInsurancePlanRetirees", lhstParam, false, idctParams))
        //    {
        //        Session["IsRetiree"] = true;
        //        if ((bool)isrvNeoSpinBusinessTier.ExecuteMethod("IsActiveMember", lhstParam, false, idctParams))
        //        {
        //            Session["IsMemberBothRetireeAndActive"] = true;
        //            Response.Redirect("wfmDefault.aspx?FormID=wfmSwitchMemberAccountMaintenance");
        //        }
        //    }
        //}
        //if (Convert.ToBoolean(Session["IsMemberBothRetireeAndActive"]))
        //{
        //    hlnkSwitchAccounts.Visible = true;
        //}
        //if (!Convert.ToBoolean(Session["IsAnnualEnrollment"]))
        //{
        //    trAnneMenu.Visible = false;
        //    trANNESpace.Visible = false;
        //}
        base.OnLoad(e);
    }

    protected void btnSwitchMemberAccount_Click(object sender, EventArgs e)
    {
        Framework.Redirect("wfmDefault.aspx?FormID=wfmSwitchMemberAccountMaintenance");
    }

    protected void btnSwitchMember_Click(object sender, EventArgs e)
    {
        if (Framework.SessionForWindow["PersonId"] != null) Framework.SessionForWindow["PersonId"] = "";
        if (Framework.SessionForWindow["request_id"] != null) Framework.SessionForWindow["request_id"] = "";
        if (Framework.SessionForWindow["person_contact_id"] != null) Framework.SessionForWindow["person_contact_id"] = "";
        if (Framework.SessionForWindow["person_account_id"] != null) Framework.SessionForWindow["person_account_id"] = "";
        if (Framework.SessionForWindow["payee_account_id"] != null) Framework.SessionForWindow["payee_account_id"] = "";
        if (Framework.SessionForWindow["person_employment_detail_id"] != null) Framework.SessionForWindow["person_employment_detail_id"] = "";
        if (Framework.SessionForWindow["IsMemberBothRetireeAndActive"] != null) Framework.SessionForWindow["IsMemberBothRetireeAndActive"] = false;
        if (Framework.SessionForWindow["MSSAccessValue"] != null) Framework.SessionForWindow["MSSAccessValue"] = "";
        Framework.Redirect("wfmMSSSwitchMember.aspx");
    }

    protected void btnHome_Click(object sender, EventArgs e)
    {
        if (Convert.ToString(Framework.SessionForWindow["RETIREE"]) == busConstant.FLAG_YES)
        {
            string lstrUrl = "wfmDefault.aspx?FormID=wfmMSSRetireeMemberHomeMaintenance";
            Framework.Redirect(lstrUrl);
        }
        else
        {
            string lstrUrl = "wfmDefault.aspx?FormID=wfmMSSActiveMemberHomeMaintenance";
            Framework.Redirect(lstrUrl);
        }
    }
    
  

    protected void btnFrameworkBase_Click(object sender, EventArgs e)
    {
        ((wfmMainDB)Page).btnBase_Click(sender, e);
    }

    private void SetNewMessages()
    {
        //InitializeUserParams();
        //string lstrUrl = ConfigurationManager.AppSettings["BusinessTierUrl"] + "srvMPIPHPMSS";
        //IMPIPHPBusinessTier isrvNeoSpinMSSBusinessTier = (IMPIPHPBusinessTier)Activator.GetObject(typeof(IMPIPHPBusinessTier), lstrUrl);

        //Hashtable lhstParameter = new Hashtable();
        //lhstParameter.Add("aintPersonID", Convert.ToInt32(Session["PersonID"]));
        //string lstrMessageCount = Convert.ToString((int)isrvNeoSpinMSSBusinessTier.ExecuteMethod("GetMSSUnreadMessagesCount", lhstParameter, false, idctParams));
        //hlnkMessages.Text = "You have " + lstrMessageCount + " messages";
        //if (Convert.ToInt32(lstrMessageCount) > 0)
        //    imgNewMsg.Visible = true;
    }

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        if ((Framework.SessionForWindow["PersonID"] != null && Convert.ToInt32(Framework.SessionForWindow["PersonID"]) != 0))
            PopulateMenu();
    }

    protected void mnuMain_MenuItemDataBound(object sender, MenuEventArgs e)
    {
        // appened the SessionId to Menu Item URL to Avoid sessin loss     
        if (String.IsNullOrEmpty(e.Item.NavigateUrl))
        {
            e.Item.NavigateUrl = "#";
        }
        else
        {
            string lstrPattern = @"/\(S\(\w*\)\)/";
            string lstrReplaceString = string.Format("/(S({0}))/", Session.SessionID);
            e.Item.NavigateUrl = System.Text.RegularExpressions.Regex.Replace(e.Item.NavigateUrl, lstrPattern, lstrReplaceString, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        Menu lmnuMain = (Menu)sender;
        SiteMapNode mapNode = (SiteMapNode)e.Item.DataItem;
        MenuItem itemToRemove = lmnuMain.FindItem(mapNode.Title);
        MenuItem parent = e.Item.Parent;

        if (itemToRemove != null)
        {
            switch (mapNode.Title)
            {
                case "Member Data":
                case "Benefit and Service Purchase Calculator":
                case "Statements":
                case "Form Listing":
                case "Overview":
                    if ((!iblnMSSEnabled) && lmnuMain.Items.Count > 0) //PROD PIR ID 6989
                        lmnuMain.Items.Remove(itemToRemove);
                    break;
                case "Organization Information":
                case "Payroll Reports":
                case "ESS Form Listing":
                    if ((!iblnESSEnabled) && lmnuMain.Items.Count > 0)
                        lmnuMain.Items.Remove(itemToRemove);
                    break;
            }
        }
        if (!string.IsNullOrEmpty(e.Item.NavigateUrl) && e.Item.NavigateUrl.Contains("FormID="))
        {
            wfmMainDB lwfmPage = (wfmMainDB)this.Page;

            string lstrFormName = e.Item.NavigateUrl.Substring(e.Item.NavigateUrl.IndexOf("FormID=")).Replace("FormID=", "").Replace("&FromMenu=True", "");

            Hashtable lhshFormAttributes = lwfmPage.isrvMetaDataCache.GetRootNode(lstrFormName);
            if ((lhshFormAttributes != null) && lhshFormAttributes.Contains("sfwResource"))
            {
                string lstrResource = lhshFormAttributes["sfwResource"].ToString().Trim();
                if (!string.IsNullOrEmpty(lstrResource))
                {
                    //object[] larrSecurity = lwfmPage.GetSecurity(Convert.ToInt16(lstrResource));

                    //if ((int)larrSecurity[0] == 0)
                    //{
                    //    if (parent != null)
                    //    {
                    //        parent.ChildItems.Remove(e.Item);

                    //        if (parent.ChildItems.Count == 0)
                    //            lmnuMain.Items.Remove(parent);
                    //    }
                    //}
                    //else
                    //{

                    InitializeUserParams();

                    //UCS 24 rule - Benefit Calculation menu when the person is having the DB/DC plan account in enrolled or suspended status
                    //if (lstrFormName == "wfmBenefitCalculationWebWizard")
                    //{
                    //    int lintPersonID = 0;
                    //    if (HttpContext.Current.Session["PersonID"] != null)
                    //        lintPersonID = Convert.ToInt32(HttpContext.Current.Session["PersonID"]);
                    //    Hashtable lhstParameter = new Hashtable();
                    //    lhstParameter.Add("aintPersonId", lintPersonID);
                    //    iblnBenefitEstimateCalculationEnabled = (bool)lwfmPage.isrvBusinessTier.ExecuteMethod("IsPersonEnrolledOrSuspendedInRetPlan", lhstParameter, false);

                    //    if (!iblnBenefitEstimateCalculationEnabled)
                    //        lmnuMain.Items.Remove(parent);
                    //}
                    ////pir 6887
                    //else 

                    //RA revisit
                    if (lstrFormName == "wfmMSSPensionPaymentDetailsMaintenance")
                    {
                        int lintPersonID = 0;
                        if (Framework.SessionForWindow["PersonID"] != null)
                            lintPersonID = Convert.ToInt32(Framework.SessionForWindow["PersonID"]);
                        Hashtable lhstParameter = new Hashtable();
                        lhstParameter.Add("aintPersonId", lintPersonID);
                        bool lblnRetrdWithAccountExists = (bool)lwfmPage.isrvBusinessTier.ExecuteMethod("IsPersonRetiredOrWithdrawnPlan", lhstParameter, false, idctParams);

                        if (!lblnRetrdWithAccountExists)
                            parent.ChildItems.Remove(e.Item);
                    }
                    //pir 6887
                    else if (lstrFormName == "wfmMSSServicePurchaseMaintenance")
                    {
                        int lintPersonID = 0;
                        if (Framework.SessionForWindow["PersonID"] != null)
                            lintPersonID = Convert.ToInt32(Framework.SessionForWindow["PersonID"]);
                        Hashtable lhstParameter = new Hashtable();
                        lhstParameter.Add("aintPersonId", lintPersonID);
                        bool lblnPurchaseExists = (bool)lwfmPage.isrvBusinessTier.ExecuteMethod("IsPersonHavePurchase", lhstParameter, false, idctParams);

                        if (!lblnPurchaseExists)
                            parent.ChildItems.Remove(e.Item);
                    }
                    //}
                }
            }
        }
    }

    private void InitializeUserParams()
    {
        idctParams = new Dictionary<string, object>();
         
        if (Framework.SessionForWindow["UserSerialID"] != null)
            idctParams[utlConstants.istrConstUserSerialID] = (int)Framework.SessionForWindow["UserSerialID"];
        if (Framework.SessionForWindow["UserId"] != null)
            idctParams[utlConstants.istrConstUserID] = Convert.ToString(Framework.SessionForWindow["UserId"]);
    }

    private void PopulateMenu()
    {
        pnlCenterLeft.Visible = true;
        if (Convert.ToString(Framework.SessionForWindow["RETIREE"]) == busConstant.FLAG_YES)
        {
            trRetireeMenu.Visible = true;
            if (Convert.ToString(Framework.SessionForWindow["CURRENT_YEAR_RETIREE"]) == busConstant.FLAG_YES)
            {
                trRetAnnStat.Visible = true;
                trRetAnnStatSep1.Visible = true;
                trRetAnnStatSep2.Visible = true;
            }
            else
            {
                trRetAnnStat.Visible = false;
                trRetAnnStatSep1.Visible = false;
                trRetAnnStatSep2.Visible = false;
            }
        }
        else
        {
            trActiveMemberMenu.Visible = true;
        }

        lblMessage.CssClass = "errMessageStyle";
        FormID.Value = Framework.SessionForWindow["CurrentForm"].ToString();
        lblMessage.Visible = false;

    }

    public static bool isEmail(string inputEmail)
    {
        if (inputEmail == null || inputEmail.Length == 0)
        {
            return true;
        }

        const string expression = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

        Regex regex = new Regex(expression);
        return regex.IsMatch(inputEmail);
    }

    /// <summary>
    /// Load the menu dynamically from session variable.
    /// </summary>
    public void LoadMenu()
    {

        //mnuMain.Items.Clear();
        //ArrayList larrMenu = (ArrayList)HttpContext.Current.Session["UserMenu"];
        //MenuItem lmnuSubItem, lmnuChildItem;
        //foreach (utlMenuItem lmnuItem in larrMenu)
        //{
        //    lmnuSubItem = new MenuItem();
        //    lmnuSubItem.NavigateUrl = lmnuItem.istrNavigateUrl;
        //    lmnuSubItem.Text = lmnuItem.istrText;

        //    foreach (utlMenuItem lmnuChild in lmnuItem.icolChildItems)
        //    {
        //        lmnuChildItem = new MenuItem();
        //        lmnuChildItem.NavigateUrl = lmnuChild.istrNavigateUrl;
        //        lmnuChildItem.Text = lmnuChild.istrText;
        //        lmnuSubItem.ChildItems.Add(lmnuChildItem);
        //    }
        //   // mnuMain.Items.Add(lmnuSubItem);
        //}
    }
}