#region Using Statements
using System.Collections;
using Sagitec.WebClient;
using System;
using Sagitec.WebControls;
using System.Web.UI;
using Sagitec.Common;
using MPIPHP.Common;
using System.Web;
using System.Web.UI.WebControls;
using System.IO;
using Sagitec.Interface;
using System.Configuration;
using MPIPHP.BusinessObjects;
#endregion


/// <summary>
/// Contains methods for file exchange and popup
/// </summary>
public abstract partial class wfmCustomPageBase : wfmMainDB
{

    protected void btnUploadFile_Click(object sender, EventArgs e)
    {
        if (istrFormName == "wfmPirMaintenance")
        {
            if (Request.Files.Count > 0)
            {
                if (Request.Files[0].ContentLength <= 5 * 1024 * 1024)
                {
                    byte[] b = new byte[Request.Files[0].ContentLength];
                    Request.Files[0].InputStream.Read(b, 0, Request.Files[0].ContentLength);
                    if (Framework.SessionForWindow["CenterMiddle"] != null && Framework.SessionForWindow["CenterMiddle"] is MPIPHP.BusinessObjects.busPir)
                    {
                        string filePath = Request.Files[0].FileName;
                        FileInfo lfio = new FileInfo(filePath);
                        string lstrFileName = lfio.Name;
                        lfio = null;

                        busPir lobjPir = (busPir)Framework.SessionForWindow["CenterMiddle"];
                        Hashtable lhshParams = new Hashtable();
                        lhshParams.Add("aintPIRID", lobjPir.icdoPir.pir_id);
                        lhshParams.Add("aobjAttachmentData", b);

                        //TODO: validate mime type and size
                        lhshParams.Add("astrMimeType", Request.Files[0].ContentType);
                        lhshParams.Add("astrFileName", lstrFileName);
                        lhshParams.Add("astrUserID", istrUserID);
                        isrvBusinessTier.ExecuteMethod("InsertAttachment", lhshParams, false, idictParams);
                        btnCancel_Click(sender, e);
                    }
                }
            }
        }
    }

    protected void btnDownloadFile_Click(object sender, EventArgs e)
    {
        Hashtable lhstNavigationParam = null;
        string lstrFileName = string.Empty;
        if (istrFormName == "wfmPirMaintenance")
        {
            GetSelectedData(((IsfwButton)sender).sfwNavigationParameter, iarrDataControls);
            lhstNavigationParam = new Hashtable();
            lhstNavigationParam = (Hashtable)iarrSelectedRows[0];
        }

        if (iblnErrorOccured)
        {
            return;
        }

        // Read the content of the File from the 'srv' Method
        iobjMethodResult = isrvBusinessTier.ExecuteMethod(((IsfwButton)sender).sfwObjectMethod, lhstNavigationParam, false, idictParams);

        if (iobjMethodResult != null && iobjMethodResult is FileDownloadContainer && ((FileDownloadContainer)iobjMethodResult).iHasErrors == false)
        {
            SetFileDownloadPopup(((FileDownloadContainer)iobjMethodResult));
        }
        //else show error?
        else if (iobjMethodResult != null)
        {
            lstrFileName = lhstNavigationParam["astrFileName"].ToString();
            FileDownloadContainer fileDownloadContainer = new FileDownloadContainer(lstrFileName, busMPIPHPBase.DeriveMimeTypeFromFileName(lstrFileName), (byte[])iobjMethodResult);
            SetFileDownloadPopup(fileDownloadContainer);
        }
    }

    protected void SetFileDownloadPopup(FileDownloadContainer result)
    {
        if (Session[FileDownloadContainer.sessionKey] != null)
        {
            Session.Remove(FileDownloadContainer.sessionKey);
        }
        Session[FileDownloadContainer.sessionKey] = result;
        ContentPlaceHolder c = (ContentPlaceHolder)base.Master.FindControl("cphCenterMiddle");
        c.Controls.Add(new PopupWindowControl("wfmFileDownloadClient.aspx"));
    }
    /// <summary>
    /// Open PDF File
    /// </summary>
    protected void btn_OpenPDF(object sender, EventArgs e)
    {
        GetSelectedData(((IsfwButton)sender).sfwNavigationParameter, iarrDataControls);
        if (iblnErrorOccured)
        {
            return;
        }

        Hashtable lhstNavigationParam = new Hashtable();
        lhstNavigationParam = (Hashtable)iarrSelectedRows[0];

        iobjMethodResult = isrvBusinessTier.ExecuteMethod(((IsfwButton)sender).sfwObjectMethod,
                lhstNavigationParam, false, idictParams);
        FileInfo lobjFileInfo = new FileInfo(lhstNavigationParam["astrFileName"].ToString());
        SetFileDownloadPopup(new FileDownloadContainer(lobjFileInfo.Name, "application/pdf", (byte[])iobjMethodResult, true));
        lobjFileInfo = null;
    }

  
}