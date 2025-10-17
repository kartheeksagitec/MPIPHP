using System;
using System.Data;
using System.IO;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using Sagitec.WebControls;
using Sagitec.WebClient;
using Sagitec.Common;

public partial class wfmExportToExcel : System.Web.UI.Page
{

    protected override void OnInit(EventArgs e)
    {
        Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
        Response.Cache.SetAllowResponseInBrowserHistory(false);
        Response.Cache.SetNoStore();
        Response.Expires = -1;
        base.OnInit(e);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        //        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        if (IsPostBack)
            return;
        cblExportToExcel.Items.Clear();
        ListItem llitColumn = null;
        sfwGridView lgrvResult = (sfwGridView)Framework.SessionForWindow["ExcelGrid"];
        foreach (DataControlField ldgcColumn in lgrvResult.Columns)
        {
			if (ldgcColumn is TemplateField)
			{
				TemplateField ltclBase = (TemplateField)ldgcColumn;
				if (ltclBase.SortExpression == "")
				{
					continue;
				}
                if(ltclBase.Visible == false)
                {
                    continue;
                }
                llitColumn = new ListItem();
                llitColumn.Text = ltclBase.HeaderText;
                llitColumn.Value = ltclBase.SortExpression;
                llitColumn.Selected = true;
                cblExportToExcel.Items.Add(llitColumn);
			}
		}
    }

	// Export the result grid view selected columns into Excel
    protected void btnExportToExcel_Click(object sender, EventArgs e)
    {
        HtmlForm lhtfBase = this.Form;
        sfwGridView lgrvResult = (sfwGridView)Framework.SessionForWindow["ExcelGrid"];
        DataControlField[] larrCols = new DataControlField[lgrvResult.Columns.Count];
        int i = 0;
        foreach (DataControlField lCol in lgrvResult.Columns)
        {
            larrCols[i++] = lCol;
        }
        TemplateField ltmfColumn;
        wfmMainDB.sfwITemplate litmBase = null;
        foreach (DataControlField ldgcColumn in lgrvResult.Columns)
        {
            if (ldgcColumn is TemplateField)
            {
                TemplateField ltclBase = (TemplateField)ldgcColumn;
                if (ltclBase.SortExpression == "")
                {
                    continue;
                }
                if (!IsExport(ltclBase.SortExpression))
                {
                    continue;
                }
                ltmfColumn = new TemplateField();
                ltmfColumn.SortExpression = ltclBase.SortExpression;
                ltmfColumn.HeaderText = ltclBase.HeaderText;
                ltmfColumn.ItemStyle.HorizontalAlign = ltclBase.ItemStyle.HorizontalAlign;
                litmBase = (wfmMainDB.sfwITemplate)ltclBase.ItemTemplate;
                foreach (XmlObject lxobBase in litmBase.ixobTemplate.icolChildObjects)
                {
                    if (lxobBase.idictAttributes.ContainsKey("sfwLinkable"))
                    {
                        lxobBase.idictAttributes.Remove("sfwLinkable");
                    }
                }
                ltmfColumn.ItemTemplate = ltclBase.ItemTemplate;
                grvExcel.Columns.Add(ltmfColumn);
            }
        }

        grvExcel.AutoGenerateColumns = false;
        grvExcel.AllowSorting = false;
        grvExcel.AllowPaging = false;
        grvExcel.GridLines = GridLines.Both;
        //Make the header text bold
        grvExcel.HeaderStyle.Font.Bold = true;
        grvExcel.HeaderStyle.Height = Unit.Pixel(20);

        grvExcel.DataSource = (ICollection)Framework.SessionForWindow["ExcelDataSource"];
        grvExcel.DataBind();


        tblExport.Rows[0].Visible = false;
        tblExport.Rows[1].Visible = false;
        //tblExport.Rows[2].Visible = false;
        tblExport.Rows[tblExport.Rows.Count - 1].Visible = false;//PIR 7132
        cblExportToExcel.Visible = false;
        lblCaption.Visible = false;
        btnExportExcel.Visible = false;

        StringWriter stringWrite = new StringWriter();
        //create an htmltextwriter which uses the stringwriter
        HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
        //tell the datagrid to render itself to our htmltextwriter
        lhtfBase.EnableViewState = false;
        lhtfBase.RenderControl(htmlWrite);
        HttpContext.Current.Response.Clear();
        HttpContext.Current.Response.ClearHeaders();
        HttpContext.Current.Response.Charset = "";
        string lstrFileName = (string)Framework.SessionForWindow["ExcelForm"] + ".xls";
        //set the response mime type for excel
        HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
        Response.AppendHeader("Content-Disposition", "attachment;filename=" + lstrFileName);
        Response.AppendHeader("Content-Length", stringWrite.ToString().Length.ToString());
        //create a string writer
        HttpContext.Current.Response.Write(stringWrite.ToString());
        HttpContext.Current.Response.End();
    }

    public bool IsExport(string astrDataField)
    {
        bool lblnResult = false;
        foreach (ListItem litmField in cblExportToExcel.Items)
        {
            if (astrDataField.IndexOf(litmField.Value) == 0)
            {
                lblnResult = litmField.Selected;
                break;
            }
        }
        return lblnResult;
    }
}
