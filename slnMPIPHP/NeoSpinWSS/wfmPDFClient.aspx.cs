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
using Sagitec.WebClient;

public partial class wfmPDFClient : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["PDFFile"] != null)
        {
            byte[] lbyteFile = null;
            string mimeType = string.Empty;
            string fileName = string.Empty;
            bool openInBrowser = false;
            if (Session["PDFFile"] != null)
            {
                lbyteFile = (byte[])Session["PDFFile"];
                Session["PDFFile"] = null;
                mimeType = "Application/pdf";
            }
            try
            {
                Response.ContentType = mimeType;
                if (string.IsNullOrWhiteSpace(fileName) == true)
                {
                    fileName = "file.pdf";
                }
                string disposition = "inline";
                if (openInBrowser == true)
                {
                    disposition = "inline";
                }
                Response.AddHeader("Content-Disposition", disposition + "; filename=" + fileName);
                Response.BufferOutput = false;
                Response.AppendHeader("Content-Length", lbyteFile.Length.ToString());
                Response.BinaryWrite(lbyteFile);

                Response.Flush();
                Response.Close();
                Response.End();
            }
            catch
            {
                Response.ClearContent();
            }
        }

     }
}
