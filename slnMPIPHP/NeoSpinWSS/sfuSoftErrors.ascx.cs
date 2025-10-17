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
using Sagitec.Common;
using Sagitec.WebControls;
using Sagitec.WebClient;

public partial class sfuSoftErrors : System.Web.UI.UserControl
{
	private string sfwobjectid;
	public string sfwObjectID
	{
		get
		{
			return sfwobjectid;
		}
		set
		{
			sfwobjectid = value;
		}
	}

	public sfwGridView sfwErrorGrid
	{
		get
		{
			return grvSoftErrors;
		}
	}

    public sfwLabel sfwLblZero
    {
        get
        {
            return lblZerogrvSoftErrors;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
	{
	}

	protected override void OnInit(EventArgs e)
	{
		wfmMainDB lwfmBase = (wfmMainDB)this.Page;
		grvSoftErrors.PageIndexChanging += new GridViewPageEventHandler(lwfmBase.GridView_PageIndexChanging);
		grvSoftErrors.Sorting += new GridViewSortEventHandler(lwfmBase.GridView_Sorting);
		//FM upgrade: 6.0.0.29 changes
		//grvSoftErrors.sfwObjectID = sfwObjectID;
		grvSoftErrors.sfwEntityField = sfwObjectID;

		TemplateField ltflMessageId = new TemplateField();
		ltflMessageId.HeaderText = "Message ID";
		ltflMessageId.SortExpression = "message_id";
		ltflMessageId.ItemTemplate = new LabelTemplate("message_id", lwfmBase);
		grvSoftErrors.Columns.Add(ltflMessageId);

		TemplateField ltflMessage = new TemplateField();
		ltflMessage.HeaderText = "Message";
		ltflMessage.SortExpression = "display_message";
		ltflMessage.ItemTemplate = new LabelTemplate("display_message", lwfmBase);
		grvSoftErrors.Columns.Add(ltflMessage);

		TemplateField ltflSeverity = new TemplateField();
		ltflSeverity.HeaderText = "Severity";
		ltflSeverity.SortExpression = "severity_description";
		ltflSeverity.ItemTemplate = new LabelTemplate("severity_description", lwfmBase);
		grvSoftErrors.Columns.Add(ltflSeverity);
    }

	public class LabelTemplate : ITemplate
	{
		string lstrObjectField;
		wfmMainDB lwfmBase;
		public LabelTemplate(string astrObjectField, wfmMainDB awfmBase)
		{
			lstrObjectField = astrObjectField;
			lwfmBase = awfmBase;
		}
		public void InstantiateIn(Control container)
		{
			sfwLabel llblBase = new sfwLabel();
			llblBase.sfwObjectField = lstrObjectField;
			llblBase.DataBinding += new EventHandler(lwfmBase.sfwItem_DataBinding);
			container.Controls.Add(llblBase);
		}
	}

	public class CheckBoxTemplate : ITemplate
	{
		string lstrObjectField;
		wfmMainDB lwfmBase;
		public CheckBoxTemplate(string astrObjectField, wfmMainDB awfmBase)
		{
			lstrObjectField = astrObjectField;
			lwfmBase = awfmBase;
		}
		public void InstantiateIn(Control container)
		{
			sfwCheckBox lcbxBase = new sfwCheckBox();
			lcbxBase.sfwObjectField = lstrObjectField;
			lcbxBase.DataBinding += new EventHandler(lwfmBase.sfwItem_DataBinding);
			container.Controls.Add(lcbxBase);
		}
	}
}
