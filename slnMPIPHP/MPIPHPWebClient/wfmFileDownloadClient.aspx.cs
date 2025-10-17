using System;
using System.Text;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MPIPHP.Common;
using Sagitec.WebClient;

public partial class wfmFileDownloadClient : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        byte[] lbyteFile = null;
        string mimeType = string.Empty;
        string fileName = string.Empty;
        bool openInBrowser = false;
        if (Session["PDFFile"] != null)
        {
            lbyteFile = (byte[])Session["PDFFile"];
            mimeType = "Application/pdf";
        }

        if (Session[FileDownloadContainer.sessionKey] != null)
        {
            FileDownloadContainer lobjFileDownloadContainer = (FileDownloadContainer)Session[FileDownloadContainer.sessionKey];
            if (lobjFileDownloadContainer.iHasErrors == false)
            {
                lbyteFile = lobjFileDownloadContainer.iFileContent;
                mimeType = lobjFileDownloadContainer.iFileMimeType;
                fileName = lobjFileDownloadContainer.iFileName;
                openInBrowser = lobjFileDownloadContainer.iblnOpenInBrowser;
            }
            //else show error?
            Session.Remove(FileDownloadContainer.sessionKey);
        }

        if (string.IsNullOrWhiteSpace(mimeType) == false)
        {
            Response.ClearContent();
            //Response.ClearHeaders();

            try
            {
                Response.ContentType = mimeType;
                if (string.IsNullOrWhiteSpace(fileName) == true)
                {
                    fileName = "file";
                }
                
                string disposition = "attachment";
                if (openInBrowser == true)
                {
                    disposition = "inline";
                }
                
                //Response.AddHeader("Content-Disposition", disposition + "; filename=" + fileName);
                Response.AppendHeader("Content-Disposition", disposition + "; filename=" + fileName);
                //Response.CacheControl = "public";
                Response.BufferOutput = false;
                Response.AppendHeader("Content-Length", lbyteFile.Length.ToString());
                Response.OutputStream.Write(lbyteFile, 0, lbyteFile.Length);
                
                Response.Flush();
            }
            catch
            {
                Response.ClearContent();
            }
        }
        Response.End();
    }

}
