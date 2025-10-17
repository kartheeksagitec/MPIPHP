using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using Microsoft.Reporting.WebForms;
using MPIPHP.BusinessObjects;
using Sagitec.Common;
using Sagitec.WebClient;
using Sagitec.WebControls;
using Sagitec.BusinessObjects;
using System.Collections.Generic;
using System.Linq;

public partial class wfmReportClient_aspx : wfmMainDBReport
{
    LocalReport ilrptReport;
    string _istrReportID;
    string _istrReportDescription;
    string _istrReportDataDate;
    string _istrFromDate = string.Empty;
    string _istrToDate = string.Empty;
    DataSet idstResult = null;
     
    protected override void OnLoad(EventArgs aEventArgs)
    {

    }

    /// <summary>
    /// Generate Report.
    /// The Report Parameters are passed to the query, and the resulting dataset is passed to the report.
    /// Be aware that the Parameters passed from the screen are not report parameters, but parameters to the query 
    /// for builing the resultset.
    /// </summary>
    /// <param name="sender">button</param>
    /// <param name="e">click event args.</param>
    protected new void btnGenerateReport_Click(object sender, EventArgs aEventArgs)
    {
        base.btnGenerateReport_Click(sender, aEventArgs);

        if (ddlReports.SelectedItem != null && (ddlReports.SelectedItem.Text.Equals("Benefit Process Counts Report")
            || ddlReports.SelectedItem.Text.Equals("IAP Counts and Amounts by Month")
            || ddlReports.SelectedItem.Text.Equals("Disability Counts and Amounts by Month Report")
            || ddlReports.SelectedItem.Text.Equals("Continuing Survivor Benefit Counts & Amount by Month")
            || ddlReports.SelectedItem.Text.Equals("Deceased Retiree Counts & Amounts by Month")))

        {
            sfwTextBox lFromDate = GetControl(itblParent, "FROM") as sfwTextBox;
            sfwTextBox lToDate = GetControl(itblParent, "TO") as sfwTextBox;

            DateTime lFromDateYear = DateTime.MinValue;
            DateTime lToDateYear = DateTime.MinValue;

            if ((lFromDate != null || lFromDate.IsNotNull())
                && (lToDate != null || lToDate.IsNotNull()))
            {
                lFromDateYear = Convert.ToDateTime(lFromDate.Text);
                lToDateYear = Convert.ToDateTime(lToDate.Text);

                TimeSpan ts = lToDateYear - lFromDateYear;
                decimal years = 0;
                years = Convert.ToDecimal(ts.Days / 365.00);

                if (years > 3)
                {
                    DisplayError(utlMessageType.Solution, 6120, null);
                    if (rvViewer.Visible == true)
                        rvViewer.Visible = false;
                }
                else
                {
                    DisplayReport(sender);
                }
            }
        }
        else
        {
            DisplayReport(sender);
        }
    }

    // Page init event
    protected override void OnInit(EventArgs aEventArgs)
    {
        // Setting report category to 'Scout Report' to display only scout report in the drop down.
        this.istrReportCategory = "Reports";

        base.OnInit(aEventArgs);

        if (iintSecurityLevel > 0)
        {
            iscmMain.RegisterPostBackControl(rvViewer);
            UpdatePanel uppCenterMiddle = (UpdatePanel)Master.FindControl("uppCenterMiddle");
            if (rvViewer != null && uppCenterMiddle != null)
            {
                PostBackTrigger lpbt = new PostBackTrigger();
                lpbt.ControlID = rvViewer.UniqueID;
                uppCenterMiddle.Triggers.Add(lpbt);
            }

            if (Framework.SessionForWindow["ReportDataSource"] != null && istrReportName != null)          // if the generate button is clicked, execute the report; otherwise not.
            {
                ExecuteReport();                                            // Viewstate will be populated when the generate button is clicked           
            }
        }
    }

    public void ExecuteReport()
    {
        SetDataSource();                                            // Viewstate will be populated when the generate button is clicked

        if (ddlReports.SelectedItem != null && ddlReports.SelectedItem.Text.Equals("Testers PIR Report For Severity"))
        {
            sfwDropDownList lreported_by_id = GetControl(itblParent, "aintReportedByUserId") as sfwDropDownList;
            if (lreported_by_id != null)
            {
                lreported_by_id.Items[0].Value = "";
                lreported_by_id.Items[0].Text = "All";
            }
        }
        if (ddlReports.SelectedItem != null && ddlReports.SelectedItem.Text.Equals("Developers PIR Report For Severity"))
        {
            sfwDropDownList lassigned_to_id = GetControl(itblParent, "aintAssignedToUserId") as sfwDropDownList;
            if (lassigned_to_id != null)
            {
                lassigned_to_id.Items[0].Value = "";
                lassigned_to_id.Items[0].Text = "All";
            }
        }
        if (ddlReports.SelectedItem != null && ddlReports.SelectedItem.Text.Equals("PIR Count by Block"))
        {
            sfwDropDownList lassigned_to_id = GetControl(itblParent, "astrDefectCause") as sfwDropDownList;
            if (lassigned_to_id != null)
            {
                lassigned_to_id.Items[0].Value = "";
                lassigned_to_id.Items[0].Text = "All";
            }
        }
        if ((ddlReports.SelectedItem != null && ddlReports.SelectedItem.Text.Equals("Testers PIR Report For Severity")) ||
            (ddlReports.SelectedItem != null && ddlReports.SelectedItem.Text.Equals("Developers PIR Report For Severity")) ||
            (ddlReports.SelectedItem != null && ddlReports.SelectedItem.Text.Equals("PIR Count by Block")))
        {
            sfwDropDownList lseverity_value = GetControl(itblParent, "astrSeverityValue") as sfwDropDownList;
            if (lseverity_value != null)
            {
                lseverity_value.Items[0].Value = "";
                lseverity_value.Items[0].Text = "All";
            }

            sfwDropDownList ltestPhase_value = GetControl(itblParent, "astrTestPhase") as sfwDropDownList;
            if (ltestPhase_value != null)
            {
                ltestPhase_value.Items[0].Value = "";
                ltestPhase_value.Items[0].Text = "All";
            }
           
        }

        if (rvViewer.Visible == false)
            rvViewer.Visible = true;

    }

    /// <summary>
    /// Set the DataSource.
    /// </summary>
    /// <returns></returns>
    /// 
    private void SetDataSource()
    {
        rvViewer.ProcessingMode = ProcessingMode.Local;
        ilrptReport = rvViewer.LocalReport;
        ilrptReport.ReportPath = Server.MapPath("Reports/") + istrReportName;
        idstResult = (DataSet)Framework.SessionForWindow["ReportDataSource"];
        rvViewer.LocalReport.DataSources.Clear();
        int lintCount = 0;

        foreach (DataTable dtResultTable in idstResult.Tables)
        {
            lintCount++;
            DataTable ldtbReportTable = dtResultTable;
            ReportDataSource lrdsReport = new ReportDataSource("ReportTable0" + lintCount, ldtbReportTable);            
            rvViewer.LocalReport.DataSources.Add(lrdsReport);
        }
    }


    /// <summary>
    /// Get Report ID and Description from XML
    /// </summary>
    /// <returns></returns>
    private bool GetReportHeaderParamsFromXML()
    {
        string lstrFormName = (string)Framework.SessionForWindow["ReportSelected"];

        // return on No Report Selected.
        if (lstrFormName.IsNullOrEmpty()) return false;
        XmlObject lxobMetaData = isrvMetaDataCache.GetXmlObject(lstrFormName);

        istrReportName = lxobMetaData.idictAttributes.GetValue("sfwReportName");
        // Get Report Description and Report ID
        _istrReportDescription = lxobMetaData.idictAttributes.GetValue("sfwDescription");
        _istrReportID = lxobMetaData.idictAttributes.GetValue("ID");
        _istrReportDataDate = GetReportDataDate();

        return true;
    }

    /// <summary>
    /// Fetch the Report Data Date from the Query Parameters.
    /// </summary>
    /// <returns>Formatted Report Data Date.</returns>
    private string GetReportDataDate()
    {
        // Get Report Data Date.

        string lstrStatusValue = string.Empty;
        string lstrReportDataDate = string.Empty;
        string lstrOrgId = string.Empty;
        string lstrOrgname = string.Empty;

        // Loop through all controls and fetch the data date.
        if (this.itblQueryParameters != null && this.itblQueryParameters.Rows.Count > 0)
        {
            foreach (TableRow row in this.itblQueryParameters.Rows)
            {
                foreach (TableCell cell in row.Cells)
                {
                    if (cell.Controls.Count == 0)
                        continue;

                    if (cell.Controls[0] is sfwTextBox)
                    {
                        sfwTextBox textBox = cell.Controls[0] as sfwTextBox;
                        switch (textBox.ID)
                        {
                            case "report_data_date_from":
                                _istrFromDate = textBox.Text;
                                break;
                            case "report_data_date_to":
                                _istrToDate = textBox.Text;
                                break;
                            case "org_Id":
                                lstrOrgId = textBox.Text;
                                break;
                            case "org_name":
                                lstrOrgname = textBox.Text;
                                break;
                            default:
                                continue;
                        }
                    }
                }
            }

            // Format Report Data Date.
            if (_istrFromDate.IsNotNullOrEmpty() && _istrToDate.IsNotNullOrEmpty() && lstrOrgId.IsNotNullOrEmpty() && lstrOrgname.IsNotNullOrEmpty())
                _istrReportDataDate = string.Format("{0} - {1},{2},{3}", _istrFromDate, _istrToDate, lstrOrgId, lstrOrgname);
            if (_istrFromDate.IsNotNullOrEmpty() && _istrToDate.IsNotNullOrEmpty() && lstrOrgId.IsNotNullOrEmpty() && lstrOrgname.IsNullOrEmpty())
                _istrReportDataDate = string.Format("{0} - {1},{2}", _istrFromDate, _istrToDate, lstrOrgId);
            if (_istrFromDate.IsNotNullOrEmpty() && _istrToDate.IsNotNullOrEmpty() && lstrOrgId.IsNullOrEmpty() && lstrOrgname.IsNullOrEmpty())
                _istrReportDataDate = string.Format("{0} - {1}", _istrFromDate, _istrToDate);
            else if (_istrFromDate.IsNotNullOrEmpty() && _istrToDate.IsNullOrEmpty() && lstrOrgId.IsNullOrEmpty() && lstrOrgname.IsNullOrEmpty())
                _istrReportDataDate = string.Format("As of {0}", _istrFromDate);
            else if (_istrToDate.IsNotNullOrEmpty() && _istrFromDate.IsNullOrEmpty() && lstrOrgId.IsNullOrEmpty() && lstrOrgname.IsNullOrEmpty())
                _istrReportDataDate = string.Format("As of {0}", _istrToDate);
            else if (_istrFromDate.IsNullOrEmpty() && _istrToDate.IsNullOrEmpty() && lstrOrgId.IsNullOrEmpty() && lstrOrgname.IsNullOrEmpty())
                _istrReportDataDate = string.Format("As of {0}", DateTime.Now.ToShortDateString());
        }

        if (string.IsNullOrEmpty(_istrReportDataDate))
            _istrReportDataDate = string.Format("As of {0}", DateTime.Now.ToShortDateString());

        return _istrReportDataDate;
    }

    /// <summary>
    /// Clear the Data source on change event
    /// </summary>
    protected void ddlReports_SelectedIndexChanged(object sender, EventArgs aEventArgs)
    {
        rvViewer.Visible = false;
        OnIndexChanged();
        rvViewer.LocalReport.DataSources.Clear();

        if (ddlReports.SelectedItem != null && ddlReports.SelectedItem.Text.Equals("Testers PIR Report For Severity"))
        {
            sfwDropDownList lreported_by_id = GetControl(itblParent, "aintReportedByUserId") as sfwDropDownList;
            if (lreported_by_id != null)
            {
                //lreported_by_id.Items[0].Value = "";
                //lreported_by_id.Items[0].Text = "All";
                ListItem lstItem = new ListItem("All", "");
                lreported_by_id.Items.Add(lstItem);
            }
        }
        if (ddlReports.SelectedItem != null && ddlReports.SelectedItem.Text.Equals("Developers PIR Report For Severity"))
        {
            sfwDropDownList lassigned_to_id = GetControl(itblParent, "aintAssignedToUserId") as sfwDropDownList;
            if (lassigned_to_id != null)
            {
                //lassigned_to_id.Items[0].Value = "";
                //lassigned_to_id.Items[0].Text = "All";
                ListItem lstItem = new ListItem("All", "");
                lassigned_to_id.Items.Add(lstItem);
            }
        }
        if (ddlReports.SelectedItem != null && ddlReports.SelectedItem.Text.Equals("PIR Count by Block"))
        {
            sfwDropDownList lassigned_to_id = GetControl(itblParent, "astrDefectCause") as sfwDropDownList;
            if (lassigned_to_id != null)
            {
                //lassigned_to_id.Items[0].Value = "";
                //lassigned_to_id.Items[0].Text = "All";
                ListItem lstItem = new ListItem("All", "");
                lassigned_to_id.Items.Add(lstItem);
            }
        }
        if ((ddlReports.SelectedItem != null && ddlReports.SelectedItem.Text.Equals("Testers PIR Report For Severity")) ||
            (ddlReports.SelectedItem != null && ddlReports.SelectedItem.Text.Equals("Developers PIR Report For Severity")) ||
            (ddlReports.SelectedItem != null && ddlReports.SelectedItem.Text.Equals("PIR Count by Block")))
        {
            sfwDropDownList lseverity_value = GetControl(itblParent, "astrSeverityValue") as sfwDropDownList;
            if (lseverity_value != null)
            {
                //lseverity_value.Items[0].Value = "";
                // lseverity_value.Items[0].Text = "All";
                ListItem lstItem = new ListItem("All", "");
                lseverity_value.Items.Add(lstItem);
            }

            sfwDropDownList ltestPhase_value = GetControl(itblParent, "astrTestPhase") as sfwDropDownList;
            if (ltestPhase_value != null)
            {
                //ltestPhase_value.Items[0].Value = "";
                //ltestPhase_value.Items[0].Text = "All";
                ListItem lstItem = new ListItem("All", "");
                ltestPhase_value.Items.Add(lstItem);
            }
        }
        if (ddlReports.SelectedItem != null && ddlReports.SelectedItem.Text.Equals("Pension Report For Death"))
        {
            sfwDropDownList lYear = GetControl(itblParent, "YEAR") as sfwDropDownList;
            var startYear = 1974;
            lYear.DataSource = Enumerable.Range(startYear, DateTime.Now.Year - startYear + 1);
            lYear.DataBind();
            if (lYear != null)
            {
                lYear.Items[0].Value = "";
                lYear.Items[0].Text = "";
            }
        }

        if (ddlReports.SelectedItem != null && (ddlReports.SelectedItem.Text.Equals("Disability Counts and Amounts by Month Report")
            || ddlReports.SelectedItem.Text.Equals("Stale Dated Report")
            || ddlReports.SelectedItem.Text.Equals("Aging Report")
            || ddlReports.SelectedItem.Text.Equals("Distribution Status Transition Report")))
        {
            sfwDropDownList ddlPlan = GetControl(itblParent, "PLAN_ID") as sfwDropDownList;
            if (ddlPlan != null)
            {
                ddlPlan.Items[0].Value = "";
                ddlPlan.Items[0].Text = "All";
            }
        }

        if (ddlReports.SelectedItem != null && ddlReports.SelectedItem.Text.Equals("Workflow Metrics Detail Report"))
        {
            sfwDropDownList lTypeID = GetControl(itblParent, "typeId") as sfwDropDownList;
            if (lTypeID != null)
            {
                lTypeID.Items[0].Value = "";
                lTypeID.Items[0].Text = "All";
            }

            sfwDropDownList lQualifiedName = GetControl(itblParent, "qualifiedName") as sfwDropDownList;
            if (lQualifiedName != null)
            {
                lQualifiedName.Items[0].Value = "";
                lQualifiedName.Items[0].Text = "All";

            }

            sfwDropDownList lUserID = GetControl(itblParent, "userID") as sfwDropDownList;
            if (lUserID != null)
            {
                lUserID.Items[0].Value = "";
                lUserID.Items[0].Text = "All";
            }

            sfwDropDownList lStatus = GetControl(itblParent, "status") as sfwDropDownList;
            if (lStatus != null)
            {
                lStatus.Items[0].Value = "";
                lStatus.Items[0].Text = "All";
            }
            sfwTextBox lpersonID = GetControl(itblParent, "personID") as sfwTextBox;
            if (lpersonID == null)
            {
                lpersonID.Text = "";
            }
            sfwTextBox lorgID = GetControl(itblParent, "orgID") as sfwTextBox;
            if (lorgID == null)
            {
                lorgID.Text = "";
            }
        }
        if (ddlReports.SelectedItem != null && ddlReports.SelectedItem.Text.Equals("Workflow Metrics Summary Report"))
        {
            sfwDropDownList lTypeID = GetControl(itblParent, "typeId") as sfwDropDownList;
            if (lTypeID != null)
            {
                lTypeID.Items[0].Value = "";
                lTypeID.Items[0].Text = "All";
            }
        }
        if (ddlReports.SelectedItem != null && ddlReports.SelectedItem.Text.Equals("Local Death Report"))
        {
            sfwDropDownList Lcl_ID_value = GetControl(itblParent, "Lcl_ID") as sfwDropDownList;
            Lcl_ID_value.Items[0].Value = "";
            Lcl_ID_value.Items[0].Text = "All";


        }
        if (ddlReports.SelectedItem != null && ddlReports.SelectedItem.Text.Equals("Local Retiree Report"))
        {
            sfwDropDownList Lcl_ID_value = GetControl(itblParent, "Lcl_ID") as sfwDropDownList;
            Lcl_ID_value.Items[0].Value = "";
            Lcl_ID_value.Items[0].Text = "All";


        }
    }

    /// <summary>
    /// Assign Report Parameters
    /// </summary>
    /// <param name="acolReportParams">Report Params</param>
    /// <param name="astrReportID">Report ID</param>
    private void AssignReportParameters(Collection<ReportParameter> acolReportParams, string astrReportID)
    {
        if (astrReportID == "rptPirCountByBlock")
        {
            sfwDropDownList lddlDefectClassification = GetControl(itblParent, "astrDefectCause") as sfwDropDownList;
            sfwDropDownList lddlTestPhase = GetControl(itblParent, "astrTestPhase") as sfwDropDownList;

            acolReportParams.Add(new ReportParameter("DefectClassification", lddlDefectClassification.SelectedItem.Text, true));
            acolReportParams.Add(new ReportParameter("TestPhase", lddlTestPhase.SelectedItem.Text, true));
        }
    }

    public static IEnumerable<int> Range(int start, int count)
    {
        int end = start + count;
        for (int i = start; i < end; i++)
            yield return i;
    }

    public void DisplayReport(object sender)
    {
        // Report was not selected; Return.
        if (!GetReportHeaderParamsFromXML()) return;

        // Report Data source is null.
        if (Framework.SessionForWindow["ReportDataSource"] == null) return;

        // Set the DataSet for the Report.
        SetDataSource();

        // Set the Header Parameters.
        Collection<ReportParameter> lcolReportParams = new Collection<ReportParameter>();

        sfwButton lbtnSender = (sfwButton)sender;

        rvViewer.LocalReport.Refresh();
        rvViewer.AsyncRendering = false;
        if (rvViewer.Visible == false)
            rvViewer.Visible = true;
        if (!rvViewer.Enabled) rvViewer.CurrentPage = 1;

        //if (lbtnSender.Text == "View")
        //    lcolReportParams.Add(new ReportParameter("VisibilityLogo", "true", false)); // Set to 'true' as requirement changed.
        //else
        //    lcolReportParams.Add(new ReportParameter("VisibilityLogo", "true", false));

        //lcolReportParams.Add(new ReportParameter("ReportID", _istrReportID, true));
        //lcolReportParams.Add(new ReportParameter("ReportName", _istrReportDescription, true));

        //if (ConfigurationManager.AppSettings["ProjectAndRegion"].IsNotNull())
        //    lcolReportParams.Add(new ReportParameter("Environment", ConfigurationManager.AppSettings["ProjectAndRegion"].ToString(), true));
        //else
        //    lcolReportParams.Add(new ReportParameter("Environment", "Test", true));

        //lcolReportParams.Add(new ReportParameter("RequestedBy", Session["UserId"].ToString()));
        //lcolReportParams.Add(new ReportParameter("ReportDataDate", _istrReportDataDate, true));

        //AssignReportParameters(lcolReportParams, _istrReportID);

        //ilrptReport.SetParameters(lcolReportParams);

    }

    /// <summary>
    /// Invalid Session while navigatting to Report page
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPreInit(EventArgs e)
    {
        if ((string)Session["IsNewSession"] == "true")
        {
            Response.Redirect("wfmLogin.aspx");
        }
        base.OnPreInit(e);
    }   
}
