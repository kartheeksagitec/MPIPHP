using Microsoft.Reporting.WebForms;
using NeoBase.Common;
using NeoBase.Reports;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.MVVMClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Web;

namespace Neo.AspxPages
{
    public partial class wfmReportClient : System.Web.UI.Page
    {
        private DataSet idstResult;
        protected MVVMSession iobjSessionData;
        protected override void OnInit(EventArgs aEventArgs)
        {
            iobjSessionData = new MVVMSession(Session.SessionID);
            //Pushawart: Restricted response header from passing unwanted information to browser.
            Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
            Response.Cache.SetNoStore();
            Response.AppendHeader("Cache-Control", "no-cache, no-store, private, must-revalidate"); // HTTP 1.1.
            Response.AppendHeader("Pragma", "no-cache"); // HTTP 1.0.
            Response.AppendHeader("Expires", "0"); // Proxies.

            base.OnInit(aEventArgs);
            if (iobjSessionData["MVVMReportDataSource"] != null)          // if the generate button is clicked, execute the report; otherwise not.
            {
                DataSet ds = iobjSessionData["MVVMReportDataSource"] as DataSet;
                iobjSessionData["ReportDataSource"] = ds;
                ExecuteReport();
            }
        }

        public void ExecuteReport()
        {
            SetDataSource();   // Viewstate will be populated when the generate button is clicked

            rvViewer.AsyncRendering = false;
            if (rvViewer.Visible == false)
            { 
                rvViewer.Visible = true;
            }
            if (!rvViewer.Enabled) rvViewer.CurrentPage = 1;
        }

        /// <summary>
        /// Set the DataSource.
        /// </summary>
        public void SetDataSource()
        {
            rvViewer.LocalReport.Refresh();
            rvViewer.ProcessingMode = ProcessingMode.Local;
            srvServers lsrvServers = new srvServers();
            Hashtable lhstReportAttrib = lsrvServers.isrvMetaDataCache.GetRootNode(Request.QueryString["ddlReports"]);

            rvViewer.LocalReport.ReportPath = Server.MapPath("~/Reports\\") + Convert.ToString(lhstReportAttrib["sfwReportName"]);
            idstResult = (DataSet)iobjSessionData["ReportDataSource"];
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
    }
}