using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sagitec.MVVMClient;

namespace Neo.AspxPages
{
    public partial class wfmDownload : System.Web.UI.Page
    {
        protected MVVMSession iobjSessionData;
        protected void Page_Load(object sender, EventArgs e)
        {
            ArrayList larrResult = (ArrayList)Session["downloadData"];
            String lstrFileName = String.Empty;
            byte[] larrByte = null;
            string lstrFileType = String.Empty;
            if (larrResult != null && larrResult.Count > 2)
            {
                lstrFileName = (String)larrResult[0];
                larrByte = (Byte[])larrResult[1];
                lstrFileType = (String)larrResult[2];
            }


            try
            {
                // Total bytes to read
                Response.Clear();
                Response.Buffer = true;


                if (!string.IsNullOrEmpty(lstrFileType))
                {
                    Response.ContentType = lstrFileType;
                }
                else
                {
                    Response.ContentType = "application/octet-stream";
                }

                Response.AppendHeader("Content-Disposition", "attachment;filename=" + lstrFileName);
                Response.AppendHeader("Content-Length", larrByte.Length.ToString());

                // Read the string
                if (larrByte.Length > 0)
                {
                    // Verify that the client is connected
                    if (Response.IsClientConnected)
                    {
                        // Write the data to the current output stream
                        Response.OutputStream.Write(larrByte, 0, larrByte.Length);

                        // Flush the data to the HTML output
                        Response.Flush();
                        Response.Close();
                    }
                }
            }
            catch
            {
            }

        }

        public override void Dispose()
        {
            base.Dispose();
            if (iobjSessionData["downloadData"] != null)
                iobjSessionData.Remove("downloadData");
        }

        //protected override object SaveViewState()
        //{
        //    if (iobjSessionData != null) iobjSessionData.Update(false); return base.SaveViewState();
        //}

        protected override void OnInit(EventArgs aEventArgs)
        {
            iobjSessionData = new MVVMSession(Session.SessionID);
        }
    }
}