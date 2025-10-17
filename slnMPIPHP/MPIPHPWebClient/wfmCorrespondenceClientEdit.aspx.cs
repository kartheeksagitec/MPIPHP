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

public partial class wfmCorrespondenceClientEdit_aspx : wfmMainDBCorrespondence
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
        Framework.SessionForWindow["CorrCallingPage"] = "wfmCorTrackingLookup";
        base.OnInit(e);
    }

    protected void btnImageCorrespondence_Click(object sender, EventArgs e)
    {
        //Get the Generated File Path and Push into Web Extender
        if (Framework.SessionForWindow["CorrFileName"] != null && !String.IsNullOrEmpty(Framework.SessionForWindow["CorrFileName"].ToString()))
        {
            string lstrFileName = Framework.SessionForWindow["CorrFileName"].ToString();

            Hashtable lhstParams = new Hashtable();
            lhstParams.Add("astrFileName", lstrFileName);
            lhstParams.Add("astrUserID", istrUserID);
            lhstParams.Add("aintUserSerialID", iintUserSerialID);
            ArrayList larrResult = (ArrayList)isrvBusinessTier.ExecuteMethod("InitializeWebExtenderUploadService", lhstParams, false, idictParams);
            if (larrResult.Count == 0)
            {
                DisplayMessage("Correspondence is successfully imaged!");
            }
            else
            {
                utlError lobjError = (utlError)larrResult[0];
                if (!String.IsNullOrEmpty(lobjError.istrErrorID))
                {
                    //FM upgrade: 6.0.0.16 changes
                    //DisplayMessage(Convert.ToInt32(lobjError.istrErrorID));
                    DisplayMessage(utlMessageType.Solution, Convert.ToInt32(lobjError.istrErrorID));
                }
                else
                {
                    DisplayMessage(lobjError.istrErrorMessage);
                }
            }
        }
        else
        {
            DisplayMessage("Correspondence must be generated before image!");
        }
    }

}
