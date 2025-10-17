using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using Sagitec.WebClient;
using Sagitec.WebControls;
using Sagitec.Common;
using System.Reflection;
using Sagitec.Interface;
//using CrystalDecisions.CrystalReports.Engine;
using MPIPHP.BusinessObjects;
using MPIPHP.Common;
using Sagitec.BusinessObjects;
using MPIPHP.DataObjects;
using System.Globalization;
using System.Text.RegularExpressions;
using MPIPHP.CustomDataObjects;
using System.Collections.Generic;
using Sagitec.ExceptionPub;

public partial class wfmDefault_aspx : wfmCustomPageBase
{
	// Page events are wired up automatically to methods 
	// with the following names:
	// Page_Load, Page_AbortTransaction, Page_CommitTransaction,
	// Page_DataBinding, Page_Disposed, Page_Error, Page_Init, 
	// Page_Init Complete, Page_Load, Page_LoadComplete, Page_PreInit
	// Page_PreLoad, Page_PreRender, Page_PreRenderComplete, 
	// Page_SaveStateComplete, Page_Unload
    public bool lblnQualifiedSpouse { get; set; }
    //private ReportDocument irptReportDocument = null;

    protected void Page_Load(object sender, EventArgs e)
	{
        string lstrOnlineHelpFolderPath = ConfigurationManager.AppSettings["OnlineHelpFolderPath"] ?? String.Empty;
        string lstrURL = lstrOnlineHelpFolderPath + this.istrFormName + ".htm'";
        string lstrFeatures = "'left=600,top=100,width=500,height=500,resizable=yes,toolbar=yes,scrollbars=yes,alwaysRaised=yes'";
        string lstrScript = "<script language='javascript'> function ShowOnlineHelp(){" +
            "window.open(" + lstrURL + ", null , " + lstrFeatures + ");return false;}; </script>";
        //ClientScript.RegisterClientScriptBlock(this.GetType(), "Help Client", lstrScript);
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Help Client", lstrScript, false);

	}


    private void DownloadFile(string astrFilePath, string astrFileName)
    {
        try
        {
            string lstrFilePath = astrFilePath + "\\" + astrFileName;
            OpenPdfDoc(lstrFilePath);
        }
        catch (Exception _exc)
        {
            ExceptionManager.Publish(_exc);
            DisplayError(_exc.Message, _exc);
        }
    }

    //Employment History
    public void CreateCorrespondence(object sender, EventArgs e)
    {
        try
        {
            sfwButton lsfwlnkbtn = (sfwButton)sender;
            string lstrFilePath = string.Empty;
            string lstrFileName = string.Empty;
            if (lsfwlnkbtn.IsNotNull())
            {
                int lintPlanId = 0;
                if (istrFormName == "wfmMSSPlanInformationMaintenance")
                {
                    busMSSPlan lbusMSSHome = (busMSSPlan)Framework.SessionForWindow["CenterMiddle"];
                    if (lbusMSSHome.icdoPersonAccount.IsNotNull())
                    {
                        lintPlanId = lbusMSSHome.icdoPersonAccount.plan_id;
                    }
                }
                lstrFileName = lsfwlnkbtn.sfwNavigationParameter.ToString();
                int lintpersonid = Convert.ToInt32(Framework.SessionForWindow["PersonId"]);
                Hashtable lhstParams = new Hashtable();
                lhstParams.Add("adictParams", idictParams);
                lhstParams.Add("astrTemplateName", lstrFileName);
                lhstParams.Add("aintpersonid", lintpersonid);
                lhstParams.Add("aintplanid", lintPlanId);
                lstrFilePath = (string)isrvBusinessTier.ExecuteMethod("CreateCorrespondenceForMss", lhstParams, false, idictParams);
                OpenPdfDoc(lstrFilePath);
            }
        }
        catch (Exception ex)
        {
            ExceptionManager.Publish(ex);
            DisplayError(ex.Message, ex);
        }
    }

    //Employment History
    public void OpenPdfDoc(string astrFilePath)
    {
       
        Hashtable lhstParams = new Hashtable();
        lhstParams.Add("astrFileName", astrFilePath);

        byte[] lbArrFileContent = (byte[])isrvBusinessTier.ExecuteMethod("RenderWordAsPDF", lhstParams, false, idictParams);

        if ((lbArrFileContent != null) && (lbArrFileContent.Length > 0))
        {
            Framework.SessionForWindow["PDFFile"] = lbArrFileContent;
            string lstrFeatures = "'width=800,height=800,resizable=yes,toolbar=yes,scrollbars=yes,alwaysRaised=yes'";
            string lstrScript = "fwkOpenPopupWindow('wfmPDFClient.aspx'," + lstrFeatures + ");";
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Open PDF", lstrScript, true);
        }
    }

    protected void btn_OpenMssForms(object sender, EventArgs e)
    {
        string lstrFileName = string.Empty;
        //string lstrFilePath = utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("MSS");
        string lstrFilePath = isrvDBCache.GetConstantValue("MSER");
        if (istrFormName == busConstant.MSS.PLAN_SUMMARY_FORM)
        {
            sfwImageButton lsfwlnkbtn = (sfwImageButton)sender;
            {
                if (lsfwlnkbtn.IsNotNull())
                {
                    
                    string lstrPlanCode = string.Empty;
                    if (this.iarrSelectedRows.Count > 0)
                    {
                        lstrPlanCode = Convert.ToString((this.iarrSelectedRows[0] as Hashtable)[lsfwlnkbtn.sfwNavigationParameter.ToString()]);
                    }
                    if(lstrPlanCode == busConstant.MPIPP)
                    {
                        lstrFileName = busConstant.MSS.PENSION_SPD;
                    }
                    else if(lstrPlanCode == busConstant.IAP)
                    {
                        lstrFileName = busConstant.MSS.PENSION_SPD;
                    }
                    else if(lstrPlanCode == busConstant.Local_52)
                    {
                        lstrFileName = busConstant.MSS.L52_SPD;
                    }
                    else if(lstrPlanCode == busConstant.Local_600)
                    {
                        lstrFileName = busConstant.MSS.L600_SPD;
                    }
                    else if(lstrPlanCode == busConstant.Local_666)
                    {
                        lstrFileName = busConstant.MSS.L666_SPD;
                    }
                    else if(lstrPlanCode == busConstant.LOCAL_700)
                    {
                        lstrFileName = busConstant.MSS.L700_SPD;
                    }
                    else if (lstrPlanCode == busConstant.Local_161)
                    {
                        lstrFileName = busConstant.MSS.L161_SPD;
                    }
                    OpenPdfDocFromFolder(lstrFilePath + lstrFileName);
                    //DownloadPDFForms(lstrFilePath + "\\" + lstrFileName);
                }
            }
        }
        else if (istrFormName == busConstant.MSS.PERSON_PROFILE_FORM || istrFormName == "wfmMSSPlanInformationMaintenance" || istrFormName == busConstant.MSS.BEN_ESTI_MAINT_FORM ||
            istrFormName == "wfmMssViewRetirementEstimateMaintenance" || istrFormName == busConstant.MSS.APP_ACT_FORM)
        {
            sfwButton lsfwlnkbtn = (sfwButton)sender;
            if (lsfwlnkbtn.IsNotNull())
            {
                lstrFileName = lsfwlnkbtn.sfwNavigationParameter;
                OpenPdfDocFromFolder(lstrFilePath + lstrFileName);
               // DownloadPDFForms(lstrFilePath + "\\" + lstrFileName);
            }
        }
        else if (istrFormName == busConstant.MSS.ANNUAL_STAT_FORM)
        {
            lstrFilePath = utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_ANNUAL_REPORT_PATH);
            sfwButton lsfwButton = (sfwButton)sender;
            if (lsfwButton.IsNotNull())
            {
                busMSSHome lbusMSSHome = (busMSSHome)Framework.SessionForWindow["CenterMiddle"];
                if (lbusMSSHome.istrFileName.IsNotNullOrEmpty())
                {
                    if (lbusMSSHome.ibusPerson.icdoPerson.mpi_person_id.IsNotNullOrEmpty())
                    {
                        DownloadPDFForms(lbusMSSHome.istrFileName);
                    }
                }

            }
        }
    }

    protected void DownloadPDFForms(string astrFilePath)
    {
        try
        {
            Hashtable lhstParams = new Hashtable();
            lhstParams.Add("astrFilePath", astrFilePath);

            byte[] lbArrFileContent = (byte[])isrvBusinessTier.ExecuteMethod("DownloadAttachment", lhstParams, false, idictParams);
            ConvertPDFByteArrayToDoc(lbArrFileContent);
            
        }
        catch (Exception _exc)
        {
            ExceptionManager.Publish(_exc);
            DisplayError(_exc.Message, _exc);
        }
    }

    private void ConvertPDFByteArrayToDoc(byte[] abArrFileContent)
    {
        if ((abArrFileContent != null) && (abArrFileContent.Length > 0))
        {
            //string lstrFilePath = utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("MSS") + "\\" + busConstant.MSS.L161_SPD;
            //FileDownloadContainer fileDownloadContainer = new FileDownloadContainer(lstrFilePath, busMPIPHPBase.DeriveMimeTypeFromFileName(lstrFilePath), abArrFileContent);
            //SetFileDownloadPopup(fileDownloadContainer);

            Framework.SessionForWindow["PDFFile"] = abArrFileContent;
            string lstrFeatures = "'width=800,height=800,resizable=yes,toolbar=yes,scrollbars=yes,alwaysRaised=yes,top=25,menubar=yes,dependent=yes'";
            string lstrScript = "fwkOpenPopupWindow('wfmPDFClient.aspx'," + lstrFeatures + ");";
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Open PDF", lstrScript, true);
        }
    }

    public void CreatePDFReport(object sender, EventArgs e)
    {
        Hashtable lhstNavigationParam = new Hashtable();
        string lstrNavigationParameter = ((IsfwButton)sender).sfwNavigationParameter;
        int lintPersonID = Convert.ToInt32(Framework.SessionForWindow["PersonId"]);

        lhstNavigationParam.Add("aintpersonid", lintPersonID);
        lhstNavigationParam.Add("astrReportName", lstrNavigationParameter);
        
        iobjMethodResult = isrvBusinessTier.ExecuteMethod(((IsfwButton)sender).sfwObjectMethod,
                lhstNavigationParam, true, idictParams);
        byte[] lbyteFile = (byte[])iobjMethodResult;
        Framework.SessionForWindow["PDFFile"] = iobjMethodResult;

        if ((lbyteFile != null) && (lbyteFile.Length > 0))
        {
            Framework.SessionForWindow["PDFFile"] = lbyteFile;
            string lstrFeatures = "'left=10,width=900,height=800,resizable=yes,toolbar=yes,scrollbars=yes,alwaysRaised=yes'";
            string lstrScript = "fwkOpenPopupWindow('wfmPDFClient.aspx'," + lstrFeatures + ");";
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Open PDF", lstrScript, true);
        }
        //Response.Write("<SCRIPT language=javascript>var w=window.open('wfmFileDownloadClient.aspx" + "','Report','height=800,width=800');</SCRIPT>");
        return;

        //Response.ContentType = "application/vnd.pdf";
        //Response.AppendHeader("Content-Disposition", "attachment;filename=" + lstrFileName);
        //Response.AppendHeader("Content-Length", larrCorr.Length.ToString());

        //// Read the string
        //if (larrCorr.Length > 0)
        //{
        //    // Verify that the client is connected
        //    if (Response.IsClientConnected)
        //    {
        //        // Write the data to the current output stream
        //        Response.OutputStream.Write(larrCorr, 0, larrCorr.Length);

        //        // Flush the data to the HTML output
        //        Response.Flush();
        //        Response.Close();
        //    }
        //}
    }

    public void OpenPdfDocFromFolder(string lstrFileName)
    {
        string lstrScript = "window.open('" + lstrFileName + "','1321854201472','width=620,height=600,toolbar=0,menubar=0,location=0,status=1,scrollbar=1,resizable=1,left=0,top=0');";
          ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Open PDF", lstrScript, true);
    }

    private void Dispose(string lstrPath)
    {
        try
        {
            File.Delete(lstrPath);
        }
        catch
        {

        }
        
    }

    protected override void LaunchImagePopup(busMainBase abusActivityInstance)
    {
        busActivityInstance lobjActivityInstance = (busActivityInstance)abusActivityInstance;

        string lstrFinalDisplayURL = String.Empty;

        string lstrDisplayURL = "http://" + ConfigurationManager.AppSettings["AppXtender_ServerName"] + "/ISubmitQuery.aspx?Appname=" + ConfigurationManager.AppSettings["AppXtender_AppName"] + "&DataSource=" + ConfigurationManager.AppSettings["AppXtender_DataSource"] + "&QueryType=0&SSN=" + lobjActivityInstance.ibusProcessInstance.ibusPerson.icdoPerson.ssn;
        if (lstrDisplayURL.IsNotNullOrEmpty())
        {
            string lstrFeatures = "width=1000px,height=800px,center=yes,help=no, resizable=yes, top=25, scrollbars=yes, toolbar=no , location=yes , directories=no ,status=yes,menubar=yes";
            string lstrScript = "fwkOpenPopupWindow('" + lstrDisplayURL + "','" + lstrFeatures + "','" + "');";
            lstrFinalDisplayURL += lstrScript;
        }
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "LaunchImage", lstrFinalDisplayURL, true);
    }

    protected override void InitializeGridSelection()
    {
        base.InitializeGridSelection();
        if (this.istrFormName == "wfmBenefitCalculationPostRetirementDeathMaintenance")
        {
            sfwGridView lsfwGridView = (sfwGridView)GetControl(this, "grvBenefitCalculationDetailIAP");
            //if ((((busBenefitCalculationPostRetirementDeath)this.ibusMain)).iclbBenefitCalculationDetail.Count > 1)
            //{
            //    lsfwGridView.SelectedIndex = 1;
            //}
            //else
            //{
                lsfwGridView.SelectedIndex = -1;
            //}
        }
        
    }

    protected override void AfterGetInitialData(string astrFormName)
    {
        if (astrFormName == "wfmWorkflowCenterLeftMaintenance")
        {
            Framework.SessionForWindow["SelectedActivityInstanceID"] = HelperFunction.GetObjectValue(ibusMain, "icdoActivityInstance.activity_instance_id", ReturnType.Object);

           // Important to set this to get the busActivityInstance value for setting the focus to the control, when workflow activity related lob screen 
           // is opended from the Left-Nav screen.
            Framework.SessionForWindow["CenterLeftActivityInstance"] = ibusMain;
        }

        if (astrFormName == busConstant.PARTICIPANT_BENEFICIARY_MAINTENANCE || astrFormName == busConstant.PERSON_DEPENDENT_MAINTENANCE )
        {
            Framework.SessionForWindow["ParticipantId"] = HelperFunction.GetObjectValue(ibusMain, "ibusPerson.icdoPerson.person_id", ReturnType.Object);
        }
    }

    protected override void BeforeGetInitialData(string astrFormName, Hashtable ahstParams)
    {
        if (astrFormName == "wfmWorkflowCenterLeftMaintenance")
        {
            int lintInstanceID = 0;
            if (Framework.SessionForWindow["SelectedActivityInstanceID"] != null)
            {
                lintInstanceID = (int)Framework.SessionForWindow["SelectedActivityInstanceID"];
            }
            ahstParams.Clear();
            ahstParams.Add("aintActivityInstanceID", lintInstanceID);
        }
        if (astrFormName == busConstant.BENEFICIARY_MAINTENANCE)
        {
            idictParams.Add("SelectedParticipantId", Framework.SessionForWindow["ParticipantId"]);
        }
        else if (astrFormName == busConstant.MSS.PERSON_PROFILE_FORM || astrFormName == busConstant.MSS.ABS_FORM ||
           astrFormName == busConstant.MSS.BEN_ESTI_MAINT_FORM || astrFormName == busConstant.MSS.ANNUAL_STAT_FORM ||
            astrFormName == busConstant.MSS.APPLICATION_FORM || astrFormName == busConstant.MSS.APP_ACT_FORM || astrFormName == busConstant.MSS.ACTIVE_MEMBER_HOME ||
            astrFormName == busConstant.MSS.RETIREE_MEMBER_HOME )
        {
            ahstParams["aintpersonid"] = Convert.ToInt32(Framework.SessionForWindow["PersonId"]);
        }
        else if (astrFormName == busConstant.MSS.PLAN_SUMMARY_FORM)
        {
            ahstParams["aintpersonid"] = Convert.ToInt32(Framework.SessionForWindow["PersonId"]);
        }
        else if (astrFormName == "wfmMssRetirementEstimateWizard")
        {
            ahstParams["aintpersonid"] = Convert.ToInt32(Framework.SessionForWindow["PersonId"]);
        }
        else if (astrFormName == "wfmMssDroEstimateWizard")
        {
            ahstParams["aintpersonid"] = Convert.ToInt32(Framework.SessionForWindow["PersonId"]);
        }
        else if (astrFormName == "wfmMSSPlanInformationMaintenance")
        {
            ahstParams["istrretiree"] = Convert.ToString(Framework.SessionForWindow["RETIREE"]);
        }

        base.BeforeGetInitialData(astrFormName, ahstParams);
    }

    protected override void InitializeLookup()
    {
        if (istrFormName == "wfmMssRetirementEstimateLookup" || istrFormName == "wfmMssDroEstimateLookup")
        {
            Framework.SessionForWindow[istrFormName + "ExecuteSearch"] = true;
            Framework.SessionForWindow[istrFormName + "RefreshData"] = true;

            sfwTextBox lsfwPersonID = (sfwTextBox)GetControl(this, "txtSpPersonId");
            if (lsfwPersonID.IsNotNull())
            {
                lsfwPersonID.Text =Convert.ToString(Framework.SessionForWindow["PersonId"]);
            }

            Hashtable lhshTable = new Hashtable();
            lhshTable.Add("aintpersonid", Convert.ToInt32(Framework.SessionForWindow["PersonId"]));
            string lstrUrl = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvMPIPHPMSS");
            //FM upgrade changes - Remoting to WCF
            //IBusinessTier isrvNeoSpinMSSBusinessTier = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), lstrUrl);
            IBusinessTier isrvNeoSpinMSSBusinessTier = null;
            try
            {
                isrvNeoSpinMSSBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);

                DataTable ldtbList = (DataTable)isrvNeoSpinMSSBusinessTier.ExecuteMethod("GetPlanValues", lhshTable, true, idictParams);

                if (itblParent != null)
                {
                    sfwDropDownList ddlPlan = (sfwDropDownList)GetControl(itblParent, "ddlPlan");
                    if (ddlPlan != null)
                    {
                        if (ddlPlan.SelectedValue.IsNullOrEmpty())
                            ddlPlan.SelectedValue = null;
                        ddlPlan.DataSource = ldtbList;
                        ddlPlan.DataTextField = "PLAN_NAME";
                        ddlPlan.DataValueField = "PLAN_ID";
                        ddlPlan.DataBind();
                        //ddlPlan.DataTextField = "P
                        //ddlPlan.DataBind();
                        ddlPlan.Items.Insert(0, new ListItem("All", null));
                    }
                }
            }
            finally
            {
                HelperFunction.CloseChannel(isrvNeoSpinMSSBusinessTier);
            }
        }
        else if (istrFormName == "wfmMssHistoryHeaderLookup")
        {
            sfwTextBox lsfwPersonID = (sfwTextBox)GetControl(this, "txtPerMpiPersonId");
            if (lsfwPersonID.IsNotNull())
            {
                lsfwPersonID.Text = Convert.ToString(Framework.SessionForWindow["MPID"]);
            }

            Hashtable lhshTable = new Hashtable();
            lhshTable.Add("aintpersonid", Convert.ToInt32(Framework.SessionForWindow["PersonId"]));
            string lstrUrl = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvMPIPHPMSS");
            //FM upgrade changes - Remoting to WCF
            //IBusinessTier isrvNeoSpinMSSBusinessTier = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), lstrUrl);
            IBusinessTier isrvNeoSpinMSSBusinessTier = null;
            try
            {
                isrvNeoSpinMSSBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);

                DataTable ldtbList = (DataTable)isrvNeoSpinMSSBusinessTier.ExecuteMethod("GetPlanValues", lhshTable, true, idictParams);
                if (itblParent != null)
                {
                    sfwDropDownList ddlPlan = (sfwDropDownList)GetControl(itblParent, "ddlPhhPlanId");
                    if (ddlPlan != null)
                    {
                        if (ddlPlan.SelectedValue.IsNullOrEmpty())
                            ddlPlan.SelectedValue = null;
                        ddlPlan.DataSource = ldtbList;
                        ddlPlan.DataTextField = "PLAN_NAME";
                        ddlPlan.DataValueField = "PLAN_ID";
                        ddlPlan.DataBind();
                        ddlPlan.Items.Insert(0, new ListItem("All", null));
                    }
                }


                DataTable ldtbBenType = (DataTable)isrvNeoSpinMSSBusinessTier.ExecuteMethod("GetBenefitTypes", lhshTable, true, idictParams);
                if (itblParent != null)
                {
                    sfwDropDownList ddlBen = (sfwDropDownList)GetControl(itblParent, "ddlDropDownList");
                    if (ddlBen != null)
                    {
                        if (ddlBen.SelectedValue.IsNullOrEmpty())
                            ddlBen.SelectedValue = null;
                        ddlBen.DataSource = ldtbBenType;
                        ddlBen.DataTextField = "DESCRIPTION";
                        ddlBen.DataValueField = "CODE_VALUE";
                        ddlBen.DataBind();
                        ddlBen.Items.Insert(0, new ListItem("All", null));
                    }
                }
            }
            finally
            {
                HelperFunction.CloseChannel(isrvNeoSpinMSSBusinessTier);
            }
        }
        base.InitializeLookup();
        if (istrFormName == "wfmMssRetirementEstimateLookup" || istrFormName == "wfmMssDroEstimateLookup")
        {
            sfwButton lbtnSearch = (sfwButton)GetControl(itblParent, "btnSearch");
            if (lbtnSearch != null)
            {
                btnSearch_Click(lbtnSearch, null);
            }
        }
    }

    public bool IsNegative(string astrNumber)
    {
        bool lblnValidPercentage = false;
        Regex lrexGex = new Regex("^[0-9,.]*$");
        if (!lrexGex.IsMatch(astrNumber))
        {
            lblnValidPercentage = true;
        }
        return lblnValidPercentage;
    }

  
    public bool IsNonNegative(string astrNumber)
    {
        bool lblnValidPercentage = false;
        Regex lrexGex = new Regex("^[0-9,.]*$");
        if (!lrexGex.IsMatch(astrNumber))
        {
            lblnValidPercentage = true;
        }
        return lblnValidPercentage;
    }

    protected override void FrameworkInit(string astrRemoteServer = null)
    {
        base.FrameworkInit(astrRemoteServer);
    }

    protected override void OnLoadComplete(EventArgs e)
    {
        base.OnLoadComplete(e);
       if (istrFormName == busConstant.MSS.ANNUAL_STAT_FORM)
        {
            if (this.ibusMain is busMSSHome)
            {
                sfwLabel lsfwLabel = (sfwLabel)GetControl(this, "lblShowNoStat");
                sfwButton lsfwDownload = (sfwButton)GetControl(this, "btnDownloadAnnualStatements");
                sfwLabel lsfwLabForDown = (sfwLabel)GetControl(this, "lblLabel");
  
                if ((this.ibusMain as busMSSHome).istrFileName.IsNotNullOrEmpty())
                {
                    lsfwLabel.Visible = false;
                    lsfwDownload.Visible = true;
                    lsfwLabForDown.Visible = true;
                }
                else
                {
                    lsfwLabel.Visible = true;
                    lsfwDownload.Visible = false;
                    lsfwLabForDown.Visible = false;
                }
            }
        }
       else if (istrFormName == busConstant.MSS.APP_ACT_FORM)
       {
           if (this.ibusMain is busMSSHome)
           {
               sfwPanel lsfwpanel = (sfwPanel)GetControl(this, "pnlCancelForm");
               if ((this.ibusMain as busMSSHome).iblnShowCancellationForm)
               {
                   lsfwpanel.Visible = true;
               }
               else
               {
                   lsfwpanel.Visible = false;
               }
           }
       }
    }
    protected override void FrameworkPreLoad()
    {
        base.FrameworkPreLoad();
    }

    protected void btnShowURLCLick(object sender, EventArgs e)
    {
       
    }
    /// <summary>
    /// WorkFlow
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnMyBasketSearch_Click(object sender, EventArgs e)
    {
        Hashtable lhshList = new Hashtable();
        //Search Criteria control
        sfwPanel lpnlMain = (sfwPanel)GetControl(itblParent, "pnlMain");
        GetFormValue(lpnlMain, lhshList);
        Framework.SessionForWindow[istrFormName + "wfmAIM"] = lhshList;

        btnValidateExecuteBusinessMethod_Click(sender, e);
    }

    protected override void AddToValidationSummary(string astrMessageID, string astrMessage, string astrFocusControl = null, string astrTooltip = null)
    {
        if (istrFormName == "wfmMssBenefitEstimateMaintenance")
        {
            if (astrMessageID == Convert.ToString(10000))
            {
                sfwLabel lsfwLabel = (sfwLabel)GetControl(this, "lblLabel3");
                lsfwLabel.Visible = true;
            }
            else
            {
                base.AddToValidationSummary(astrMessageID, astrMessage, astrFocusControl, astrTooltip);
            }
        }
        else
        {
            base.AddToValidationSummary(astrMessageID, astrMessage, astrFocusControl, astrTooltip);
        }

    }

    protected void btn_ExportTable(object sender, EventArgs e)
    {
        Hashtable lhst = new Hashtable();
        //Need to add a query textbox on lookup with this ID 
        sfwTextBox lsfwTextBox = (sfwTextBox)GetControl(this, "txbSql");
        if (lsfwTextBox.IsNotNull())
        {
            string lstrFinalQuery = lsfwTextBox.Text;
            lstrFinalQuery = lstrFinalQuery.Replace("TOP 100", "");
            lhst.Add("astrFinalQuery", lstrFinalQuery);
            DataTable ldtTable = (DataTable)isrvBusinessTier.ExecuteMethod("ExportInExcel", lhst, false, idictParams);
            if (ldtTable.IsNotNull() && ldtTable.Rows.Count > 0)
            {
                Framework.SessionForWindow["ExportForm"] = istrFormName;
                sfwGridView lsfwGridView = (sfwGridView)GetControl(this, "grvExport");
                lsfwGridView.GridLines = GridLines.Both;
                lsfwGridView.HeaderStyle.Height = Unit.Pixel(20);
                lsfwGridView.PageSize = ldtTable.Rows.Count;
                lsfwGridView.DataSource = ldtTable;
                lsfwGridView.DataBind();

                //FM upgrade: 6.0.0.21 changes
                //ClearExportGridControls(lsfwGridView);
                //FM upgrade: 6.0.0.24 changes
                //ExportGridViewToExcel(lsfwGridView);
                ICollection lcolData = (ICollection)Framework.SessionForWindow["ExportDataSource"];
                ExportGridViewToExcel(lsfwGridView, lcolData);
            }
        }


    }

    public void btnGo_Click(object sender, EventArgs e)
    {
        sfwDropDownList lddlValue = (sfwDropDownList)GetControl(this, "ddlDropDownList");
        sfwCascadingDropDownList lddlBenefitType = (sfwCascadingDropDownList)GetControl(this, "ddlCascadingDropDownList");
        if (lddlValue.IsNotNull() && lddlBenefitType.IsNotNull())
        {

            if (lddlValue.SelectedValue == "VIEW")
            {
                if (lddlBenefitType.sfwSelectedValue == "RTMT")
                {
                    Framework.Redirect("wfmDefault.aspx?FormID=wfmMssRetirementEstimateLookup");
                }
                else if (lddlBenefitType.sfwSelectedValue == "DRO")
                {
                    Framework.Redirect("wfmDefault.aspx?FormID=wfmMssDroEstimateLookup");
                }
            }
        }
        
    }


    public void btnFAQ_Click(object sender, EventArgs e)
    {
        string lstrFeatures = "'width=800,height=800,resizable=yes,toolbar=yes,scrollbars=yes,alwaysRaised=yes,top=25,menubar=yes,dependent=yes'";
        string lstrScript = "fwkOpenPopupWindow('wfmMSSFAQ.aspx'," + lstrFeatures + ");";        //string lstrScript = "window.open('wfmMSSDisclaimer.aspx','1321854201472','width=620,height=600,toolbar=0,menubar=0,location=0,status=1,scrollbars=1,resizable=1,left=0,top=0";
           ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "FAQ", lstrScript, true);
    }

    protected override void btnSave_Click(object sender, EventArgs e)
    {
        base.btnSave_Click(sender, e);
        if (istrFormName == "wfmMssViewRetirementEstimateMaintenance")
        {
            sfwGridView lgrvBase = (sfwGridView)GetControl(itblParent, "grvMssBenefitCalculationDetail");
            if (lgrvBase.IsNotNull())
            {
                lgrvBase.SelectedIndex = 0;
                BindDataToForm();

            }
        }
    }


    protected override utlColumnInfo GetDataValue(IsfwDataControl adctBase, object aobjBase)
    {
        return base.GetDataValue(adctBase, aobjBase);
    }
    # region NeoCerify
    protected void btnPositionGrid_Click(object sender, EventArgs e)
    {
        string lstrParam = Request.Form[((WebControl)sender).UniqueID];
        string[] larrParam = lstrParam.Split(",");
        sfwGridView lgrvTemp = (sfwGridView)GetControl(this, larrParam[0]);
        string strExpression = larrParam[1];
        bool blnIsRetrieve = false;
        if (larrParam.Length > 2)
        {
            if (larrParam[2] == "Y")
            {
                blnIsRetrieve = true;
            }
        }
        string strCenterLeftForm = string.Empty;
        if (larrParam.Length > 3)
        {
            strCenterLeftForm = larrParam[3];
        }
        if (!string.IsNullOrEmpty(strCenterLeftForm))
        {
            GetCurrentObject(strCenterLeftForm);
        }
        else
        {
            GetCurrentObject();
        }
        if (ienmPageType != sfwPageType.Lookup)
        {
            BindFormToData(iarrDataControls);
        }
        SetGridPage(lgrvTemp, strExpression, blnIsRetrieve, strCenterLeftForm);
        BindDataToForm();
        if (!string.IsNullOrEmpty(strCenterLeftForm))
        {
            SetCurrentObject(strCenterLeftForm);
            RefreshUpdatePanel("uppAccordian");
        }
        else
        {
            SetCurrentObject();
            RefreshUpdatePanel("uppCenterMiddle");
        }
    }

    protected string RemoveWhiteSpace(string astrInputString)
    {
        if (!string.IsNullOrEmpty(astrInputString))
        {
            astrInputString = astrInputString.Trim();
            if (astrInputString.Contains(" "))
            {
                string[] strInput = astrInputString.Split(' ');
                astrInputString = string.Empty;
                foreach (string s in strInput)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        astrInputString += s.Trim() + " ";
                    }
                }
                astrInputString = astrInputString.Trim();
            }
            if (astrInputString.IndexOf("<") > -1 && astrInputString.IndexOf(">") > 0)
            {
                string[] strSplitSpan = astrInputString.Split('>');
                int countSpan = 0;
                astrInputString = string.Empty;
                foreach (string strSpan in strSplitSpan)
                {
                    countSpan++;
                    string strInp = strSpan;
                    if (strInp.IndexOf("<") > -1 && countSpan < strSplitSpan.Length)
                    {
                        strInp = strInp.Substring(0, strInp.IndexOf("<"));
                    }
                    if (!string.IsNullOrEmpty(strInp))
                    {
                        astrInputString += strInp + " ";
                    }
                }
                astrInputString = astrInputString.Trim();
            }
        }
        return astrInputString;
    }

    protected void SetGridPage(sfwGridView aGridView, string astrExpression, bool blnRetrieve, string astrCenterLeftForm)
    {
        ArrayList arrBus = new ArrayList();
        if (!string.IsNullOrEmpty(astrCenterLeftForm))
        {
            arrBus.Add(Framework.SessionForWindow[astrCenterLeftForm]);
        }
        else if (blnRetrieve)
        {
            arrBus.Add(Framework.SessionForWindow["ChildCenterMiddle"]);
        }
        else
        {
            arrBus.Add(Framework.SessionForWindow["CenterMiddle"]);
        }
        //FM upgrade: 6.0.0.29 changes
        //ICollection lcolBase = (ICollection)HelperFunction.GetObjectFromResult(arrBus, aGridView.sfwObjectID);
        ICollection lcolBase = (ICollection)HelperFunction.GetObjectFromResult(arrBus, aGridView.sfwObjectField);
        int iPageSize = aGridView.PageSize;
        int iPageIndex = aGridView.PageIndex;
        if (lcolBase != null)
        {
            int iRowIndex = -1;
            if (int.TryParse(astrExpression, out iRowIndex))
            {
                if (iRowIndex >= iPageSize)
                {
                    //Handle last item in grid on a apage(removed + 1)
                    iPageIndex = ((iRowIndex) / iPageSize);
                    iRowIndex = iRowIndex - (iPageIndex * iPageSize);
                }
                else
                {
                    iPageIndex = 0;
                }
            }
            else
            {
                string[] strFields = astrExpression.Split(';');
                int iColIdx = 0;
                bool blnColIndx = false;
                //Chnage done for Soft Errors
                foreach (object objBase in lcolBase)
                {
                    bool blnMatch = true;
                    foreach (string strField in strFields)
                    {
                        string strObjectField = strField.Substring(0, strField.IndexOf('='));
                        string strFieldValue = strField.Substring(strField.IndexOf('=') + 1);
                        object objActualValue = HelperFunction.GetObjectValue(objBase, strObjectField, ReturnType.Object);
                        if (objActualValue is DateTime)
                        {
                            if (Convert.ToDateTime(strFieldValue) != Convert.ToDateTime(objActualValue))
                            {
                                blnMatch = false;
                                break;
                            }
                        }
                        else if (objActualValue is decimal)
                        {
                            if (Convert.ToDecimal(strFieldValue) != Convert.ToDecimal(objActualValue))
                            {
                                blnMatch = false;
                                break;
                            }
                        }
                        else if (objActualValue is int)
                        {
                            if (Convert.ToInt32(strFieldValue) != Convert.ToInt32(objActualValue))
                            {
                                blnMatch = false;
                                break;
                            }
                        }
                        else
                        {
                            if (RemoveWhiteSpace(Convert.ToString(strFieldValue)) != RemoveWhiteSpace(Convert.ToString(objActualValue)))
                            {
                                blnMatch = false;
                                break;
                            }
                        }
                    }
                    if (blnMatch)
                    {
                        iRowIndex = iColIdx;
                        blnColIndx = true;
                        if (iRowIndex >= iPageSize)
                        {
                            iPageIndex = ((iRowIndex + 1) / iPageSize);
                            iRowIndex = iRowIndex - (iPageIndex * iPageSize);
                        }
                        else
                        {
                            iPageIndex = 0;
                        }
                        break;
                    }
                    iColIdx++;
                }
                if (!blnColIndx)
                {
                    iRowIndex = -1;
                }
            }
            aGridView.PageIndex = iPageIndex;
            HiddenField hfldRowIndex = (HiddenField)GetControl(this, "hfldRowIndex");
            hfldRowIndex.Value = Convert.ToString(iRowIndex);
        }
    }



    #endregion

    protected override void btnLogoff_Click(object sender, EventArgs e)
    {
        // clear authentication cookie
        HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
        cookie1.Expires = DateTime.Now.AddYears(-1);
        Response.Cookies.Add(cookie1);

        base.btnLogoff_Click(sender, e);

    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
        Response.Cache.SetNoStore();
        Response.AppendHeader("Cache-Control", "no-cache, no-store, private, must-revalidate"); // HTTP 1.1. 
        Response.AppendHeader("Pragma", "no-cache"); // HTTP 1.0. 
        Response.AppendHeader("Expires", "0"); // Proxies.
    }

    protected override void btnReset_Click(object sender, EventArgs e)
    {
        base.btnReset_Click(sender, e);
    }
    protected override void btnSearch_Click(object sender, EventArgs e)
    {
        base.btnSearch_Click(sender, e);
        if (istrFormName == "wfmMssRetirementEstimateLookup")
        {
            sfwTable ltblCriteria = GetParentTable((Control)sender);
            if (ltblCriteria.IsNotNull())
            {
                ArrayList arrControls = GetCriteriaControls(ltblCriteria);
                if (!arrControls.IsNullOrEmpty())
                {
                    bool lblnSetDefault = true;
                    foreach (Control lclcontrol in arrControls)
                    {
                        lblnSetDefault = true;
                        if (lclcontrol is sfwTextBox)
                        {
                            string lstrName = (lclcontrol as sfwTextBox).ID;
                            if (lstrName == "txtSpPersonId")
                            {
                                lblnSetDefault = false;
                            }
                        }
                        if (lblnSetDefault)
                        {
                           SetDefaultValueForControl((IsfwCriteriaControl)lclcontrol);
                        }

                    }
                }
            }

        }
        
    }

    protected void btn_OpenMSSPDF(object sender, EventArgs e)
    {
        Hashtable lhstNavigationParam = new Hashtable();
        lhstNavigationParam = (Hashtable)iarrSelectedRows[0];
        iobjMethodResult = isrvBusinessTier.ExecuteMethod(((IsfwButton)sender).sfwObjectMethod,
                lhstNavigationParam, true, idictParams);
        byte[] lbyteFile = (byte[])iobjMethodResult;
        Session["PDFFile"] = iobjMethodResult;

        string lstrFeatures = "'width=800,height=800,resizable=yes,toolbar=yes,scrollbars=yes,alwaysRaised=yes'";
        //'WND_Webhelp'
        string lstrScript = "fwkOpenPopupWindow('wfmPDFClient.aspx'," + lstrFeatures + ");";
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Open PDF", lstrScript, true);
    }

    //protected override void OnPreInit(EventArgs e)
    //{
    //    if ((string)Session["IsNewSession"] == "true")
    //    {
    //        Response.Redirect("wfmLoginMI.aspx");
    //    }
    //    base.OnPreInit(e);
    //} 
}
