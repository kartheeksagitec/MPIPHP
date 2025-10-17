using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Sagitec.WebClient;
using Sagitec.Common;
using Sagitec.WebControls;
using MPIPHP.CustomDataObjects;
using System.Collections.ObjectModel;
using MPIPHP.BusinessObjects;
using System.IO;

public partial class wfmCorrespondenceClient_aspx : wfmMainDBCorrespondence
{
    // Page events are wired up automatically to methods 
    // with the following names:
    // Page_Load, Page_AbortTransaction, Page_CommitTransaction,
    // Page_DataBinding, Page_Disposed, Page_Error, Page_Init, 
    // Page_Init Complete, Page_Load, Page_LoadComplete, Page_PreInit
    // Page_PreLoad, Page_PreRender, Page_PreRenderComplete, 
    // Page_SaveStateComplete, Page_Unload

    protected override void OnInit(EventArgs e)
    {
        Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
        Response.Cache.SetAllowResponseInBrowserHistory(false);
        Response.Cache.SetNoStore();
        base.OnInit(e);
    }

    protected void Page_Load()
    {

    }

    protected override void OnLoadComplete(EventArgs e)
    {
        base.OnLoadComplete(e);
        FillPlanValues();
		
		//LA Sunset - Payment Directives
        FillCorrpondenceNames();
    }

    protected override void OnPreRender(EventArgs e)
    {
        //iarrTriggerControls.Add(lbtnView);
        base.OnPreRender(e);
    }

    protected override void btnEditCorrespondence_Click(object sender, EventArgs e)
    {
        base.btnEditCorrespondence_Click(sender, e);

    }

    protected void btnView_Click(object sender, EventArgs e)
    {
        string lstrFileName = string.Empty;
        lstrFileName = Convert.ToString(Framework.SessionForWindow["CorrFileName"]);
        if (!string.IsNullOrEmpty(lstrFileName))
        {
            Hashtable lhstTables = new Hashtable();
            lhstTables.Add("astrFileName", lstrFileName);
            byte[] larrCorr = (byte[])isrvBusinessTier.ExecuteMethod("RenderWordAsPDF", lhstTables, false, idictParams);

            try
            {
                Response.Clear();
                Response.Buffer = true;

                lstrFileName = Path.ChangeExtension(Path.GetFileName(lstrFileName), ".pdf");

                Response.ContentType = "application/vnd.pdf";
                Response.AppendHeader("Content-Disposition", "attachment;filename=" + lstrFileName);
                Response.AppendHeader("Content-Length", larrCorr.Length.ToString());

                // Read the string
                if (larrCorr.Length > 0)
                {
                    // Verify that the client is connected
                    if (Response.IsClientConnected)
                    {
                        // Write the data to the current output stream
                        Response.OutputStream.Write(larrCorr, 0, larrCorr.Length);

                        // Flush the data to the HTML output
                        Response.Flush();
                        Response.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                // Trap the error, if any
                Response.Write("Error : " + ex.Message);
            }
        }
    }

    //override 
    
    

    protected void mdl_Correspondence_SelectedIndexChanged(object sender, EventArgs e)
    {
        OnIndexChanged();
    }

	//LA Sunset - Payment Directives
    void FillCorrpondenceNames()
    {
        if (this.mdl_Correspondence != null && this.mdl_Correspondence.Items.Count > 0 && idictParams.ContainsKey("FormName") && idictParams["FormName"].ToString() == "wfmPayeeAccountMaintenance")
        {
            ListItem lListItem = null;
            //FM upgrade: 6.0.0.29 changes
            //busPayeeAccount lbusPayeeAccount = (busPayeeAccount)this.iarrData[0];
            busPayeeAccount lbusPayeeAccount = (busPayeeAccount)this.ibusData;
            if (lbusPayeeAccount.istrTemplateName.IsNotNullOrEmpty())
            {
                lListItem = this.mdl_Correspondence.Items.FindByValue(lbusPayeeAccount.istrTemplateName);
                if (lListItem != null)
                {
                    this.mdl_Correspondence.Items.Clear();
                    this.mdl_Correspondence.Items.Add(lListItem);
                }
            }         
        }
    }

    private void FillPlanValues()
     {
         if (this.mdl_Correspondence.SelectedItem != null 
            && (this.mdl_Correspondence.SelectedItem.Value == busConstant.RETIREMENT_APPLICATION_LOCALS_52_600_666_700 || this.mdl_Correspondence.SelectedItem.Value == busConstant.Retirement_Application_Packet_Local_52_600_666_700))
        {
            sfwDropDownList lsfwDropDownList = (sfwDropDownList)GetControl(this, "PLAN");
            if (lsfwDropDownList != null)
            {
                lsfwDropDownList.Items.Clear();
                lsfwDropDownList.Items.Add(string.Empty);
                if (this.ibusData != null)
                {
                    //FM upgrade: 6.0.0.29 changes
                    //busBenefitCalculationRetirement lbusBenefitCalculationRetirement = (busBenefitCalculationRetirement)this.iarrData[0];
                    busBenefitCalculationRetirement lbusBenefitCalculationRetirement = (busBenefitCalculationRetirement)this.ibusData;
                    foreach (busPersonAccount lbusPersonAccount in lbusBenefitCalculationRetirement.ibusPerson.iclbPersonAccount)
                    {
                        if (lbusPersonAccount.icdoPersonAccount.istrPlanCode != busConstant.MPIPP && lbusPersonAccount.icdoPersonAccount.istrPlanCode != busConstant.IAP)
                        {
                            //lsfwDropDownList.Items.Add(lbusPersonAccount.icdoPersonAccount.istrPlanCode);

                            if (lbusPersonAccount.icdoPersonAccount.istrPlanCode == "Local600")
                            {
                                string lstrPlanName = "Local 600";
                                lsfwDropDownList.Items.Add(lstrPlanName);
                            }
                            if (lbusPersonAccount.icdoPersonAccount.istrPlanCode == "Local666")
                            {
                                string lstrPlanName = "Local 666";
                                lsfwDropDownList.Items.Add(lstrPlanName);
                            }
                            if (lbusPersonAccount.icdoPersonAccount.istrPlanCode == "Local700")
                            {
                                string lstrPlanName = "Local 700";
                                lsfwDropDownList.Items.Add(lstrPlanName);
                            }
                            if (lbusPersonAccount.icdoPersonAccount.istrPlanCode == "Local52")
                            {
                                string lstrPlanName = "Local 52";
                                lsfwDropDownList.Items.Add(lstrPlanName);
                            }
                            if (lbusPersonAccount.icdoPersonAccount.istrPlanCode == "Local161")
                            {
                                string lstrPlanName = "Local 161";
                                lsfwDropDownList.Items.Add(lstrPlanName);
                            }
                        }
                    }                  
                }
            }
        }        
    }

}
