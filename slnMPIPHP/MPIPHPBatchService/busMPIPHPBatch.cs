using System;
using System.Collections;
using System.Data;
using System.Text;
using MPIPHP.BusinessObjects;
using MPIPHP.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.CorBuilder;
using Sagitec.DBUtility;
using MPIPHP.Common;
//using CrystalDecisions.CrystalReports.Engine;
//using CrystalDecisions.Shared;
//using CrystalDecisions.CrystalReports.Engine;
//using CrystalDecisions.Shared;

namespace MPIPHP.MPIPHPJobService
{
    /// <summary>
    /// Summary description for StepHandler.
    /// </summary>
    public class busMPIPHPBatch 
    {
        protected string _iStepName{ get; set;}
        protected int _iCount { get; set; }
        protected int _iInternalCount { get; set; }
        protected int _iLimit { get; set;}
        protected int _iMaxCount { get; set;}
        //protected ReportDocument RptBatch { get; set;}

        protected int _iTotalRecords { get; set;}
        protected int _iTotalSuccess { get; set; }
        protected int _iTotalError { get; set; }

        protected int _iCurrentCycleNo { get; set; }
        public busSystemManagement iobjSystemManagement { get; set; }
        public static CorBuilderXML iobjCorBuilder;
        public cdoBatchSchedule iobjBatchSchedule;

        public delegate void UpdateProcessLog(string astrMessage, string astrMessageType, string astrStepName);
        public UpdateProcessLog idlgUpdateProcessLog;

        public busMPIPHPBatch()
        {
            // Initialize the system management object as well since this will be needed by all handlers.
            iobjSystemManagement = new busSystemManagement();
            iobjSystemManagement.FindSystemManagement();
            _iCurrentCycleNo = iobjSystemManagement.icdoSystemManagement.current_cycle_no;
        }

        /// <summary>
        /// Generate correspondence, create the tracking record for the generated letter
        /// </summary>
        /// <param name="aintTemplateID"></param>
        /// <param name="aintPersonID"></param>
        /// <param name="astrUserId"></param>
        /// <param name="aarrResult"></param>
        /// <returns></returns>
        public string CreateCorrespondence(string astrTemplateName, string astrUserID, int aintUserSerialID, ArrayList aarrResult, Hashtable ahtbQueryBkmarks)
        {
            //utlCorresPondenceInfo lobjCorresPondenceInfo = busNeoSpinBase.SetCorrespondence(
            //    astrTemplateName, astrUserID, aintUserSerialID, aarrResult, ahtbQueryBkmarks);

            //if (lobjCorresPondenceInfo == null)
            //{
            //    throw new Exception("Unable to create correspondence, SetCorrespondence method not found in " +
            //        " business solutions base object");
            //}

            string lstrFileName = "";
            //lstrFileName = iobjCorBuilder.CreateCorrespondenceFromTemplate(astrTemplateName,
            //    lobjCorresPondenceInfo, astrUserID);
            return lstrFileName;
        }

        /*
        public string CreateReport(string astrReportName, DataTable adstResult)
        {
            ReportDocument RptBatch = new ReportDocument();
            RptBatch.InitReport += new EventHandler(this.OnReportDocInit); // Add event handler for report document init
            string labsRptDefPath = string.Empty;
            labsRptDefPath = iobjPassInfo.isrvDBCache.GetPathInfo("BatchRptDF");
            RptBatch.Load(labsRptDefPath + astrReportName);
            RptBatch.SetDataSource(adstResult);             // gets the data and bind to the report doc control
            string labsRptGenPath = string.Empty;
            labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo("BatchRptGN");
            string lstrReportFullName = string.Empty;
            lstrReportFullName = labsRptGenPath + astrReportName + "_" +
                DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
            RptBatch.ExportToDisk(ExportFormatType.PortableDocFormat, lstrReportFullName);
            return lstrReportFullName;
        }*/

        /// <summary>
        /// Create PDF Report
        /// </summary>
        /// <param name="astrReportName">Report file name</param>
        /// <param name="adstResult">Data Table that acts as source.</param>
        /// <param name="astrFileName">File Name of the Generated Report</param>        
        /// <returns>Report Name created as a PDF File</returns>
        public string CreatePDFReport(string astrReportName, DataTable adstResult, string astrFileName)
        {
            //RptBatch = new ReportDocument();
            //RptBatch.InitReport += new EventHandler(this.OnReportDocInit); // Add event handler for report document init
            string labsRptDefPath = string.Empty;
            //labsRptDefPath = iobjPassInfo.isrvDBCache.GetPathInfo("BatchRptDF");
            //RptBatch.Load(labsRptDefPath + astrReportName);
            //RptBatch.SetDataSource(adstResult);             // gets the data and bind to the report doc control
            string labsRptGenPath = string.Empty;
            //labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo("BatchRptGN");
            string lstrReportFullName = string.Empty;
            lstrReportFullName = labsRptGenPath + astrFileName;
            //RptBatch.ExportToDisk(ExportFormatType.PortableDocFormat, lstrReportFullName);
            return lstrReportFullName;
        }

        /// <summary>
        /// Create Excel Report
        /// </summary>
        /// <param name="astrReportName">Report file name</param>
        /// <param name="adstResult">Data Table that acts as source.</param>
        /// <param name="astrFileName">File Name of the Generated Report</param>        
        /// <returns>Report Name created as an Excel File</returns>
        public string CreateExcelReport(string astrReportName, DataTable adstResult, string astrFileName)
        {
            //RptBatch = new ReportDocument();
            //RptBatch.InitReport += new EventHandler(this.OnReportDocInit); // Add event handler for report document init
            string labsRptDefPath = iobjPassInfo.isrvDBCache.GetPathInfo("BatchRptDF");
            //RptBatch.Load(labsRptDefPath + astrReportName);
            //RptBatch.SetDataSource(adstResult);             // gets the data and bind to the report doc control
            string labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo("BatchRptGN");
            string lstrReportFullName = labsRptGenPath + astrFileName;
            //RptBatch.ExportToDisk(ExportFormatType.Excel, lstrReportFullName);
            return lstrReportFullName;
        }

        /// <summary>
        /// Create PDF Report
        /// </summary>
        /// <param name="astrReportName">Report Name</param>
        /// <param name="adstResult">DataSet</param>        
        /// <param name="astrFileName">File Name</param>
        /// <returns>Generated PDF Report File Name</returns>
        public string CreatePDFReport(string astrReportName, DataSet adstResult, string astrFileName)
        {
            //RptBatch = new ReportDocument();
            //RptBatch.InitReport += new EventHandler(this.OnReportDocInit); // Add event handler for report document init
            string labsRptDefPath = iobjPassInfo.isrvDBCache.GetPathInfo("BatchRptDF");
            //RptBatch.Load(labsRptDefPath + astrReportName);
            //RptBatch.SetDataSource(adstResult);             // gets the data and bind to the report doc control
            string labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo("BatchRptGN");
            string lstrReportFullName = labsRptGenPath + astrFileName;
            //RptBatch.ExportToDisk(ExportFormatType.PortableDocFormat, lstrReportFullName);
            return lstrReportFullName;
        }

        /// <summary>
        /// Create Excel Report
        /// </summary>
        /// <param name="astrReportName">Report Name</param>
        /// <param name="adstResult">DataSet</param>        
        /// <param name="astrFileName">File Name</param>
        /// <returns>Generated Excel Report File Name</returns>
        public string CreateExcelReport(string astrReportName, DataSet adstResult, string astrFileName)
        {
            //RptBatch = new ReportDocument();
            //RptBatch.InitReport += new EventHandler(this.OnReportDocInit); // Add event handler for report document init
            string labsRptDefPath = iobjPassInfo.isrvDBCache.GetPathInfo("BatchRptDF");
            //RptBatch.Load(labsRptDefPath + astrReportName);
            //RptBatch.SetDataSource(adstResult);             // gets the data and bind to the report doc control
            string labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo("BatchRptGN");
            string lstrReportFullName = labsRptGenPath + astrFileName;
            //RptBatch.ExportToDisk(ExportFormatType.Excel, lstrReportFullName);
            return lstrReportFullName;
        }

        // Initialize the report documnet. This event removes any databse logon information 
        // saved in the report. The call to Load the report in the above function fires this event.
        private void OnReportDocInit(object sender, System.EventArgs e)
        {
            //RptBatch.SetDatabaseLogon("", "");
        }

        public void StartCounter()
        {
            idlgUpdateProcessLog("Started processing.. ", "INFO", _iStepName);
            _iCount = 0;
            _iInternalCount = 0;
        }

        public void StartCounter(int aintLimit)
        {
            StartCounter();
            _iLimit = aintLimit;
        }

        public void CallCounter()
        {
            if (_iInternalCount == _iLimit)
            {
                if (_iMaxCount != 0)
                {
                    //display this message if max count is available
                    idlgUpdateProcessLog("Currently processing record " + _iCount + " of " + _iMaxCount, "INFO", _iStepName);
                }
                else
                {
                    idlgUpdateProcessLog("Currently processing record " + _iCount, "INFO", _iStepName);
                }
                _iInternalCount = 0;
            }
            _iCount++;
            _iInternalCount++;
        }

        public void EndCounter()
        {
            idlgUpdateProcessLog("Ended processing.. Total records read " + _iCount, "INFO", _iStepName);
        }

        public utlPassInfo iobjPassInfo
        {
            get
            {
                return utlPassInfo.iobjPassInfo;
            }
        }


        protected void PostErrorMessage(string astrMessageToBePosted)
        {
            PostMessage(astrMessageToBePosted, BatchHelper.BATCH_MESSAGE_ERROR);
        }

        protected void PostSummaryMessage(string astrMessageToBePosted)
        {
            PostMessage(astrMessageToBePosted, BatchHelper.BATCH_MESSAGE_SUMMARY);
        }

        protected void PostInfoMessage(string astrMessageToBePosted)
        {
            PostMessage(astrMessageToBePosted, BatchHelper.BATCH_MESSAGE_INFO);
        }

        protected void PostMessage(string astrMessageToBePosted, string astrMessageType)
        {
            idlgUpdateProcessLog(astrMessageToBePosted, astrMessageType, _iStepName);
        }

        protected virtual void SummarizeProcessCompletion()
        {
            PostSummaryMessage("Total number of records to be processed = " + _iTotalRecords);
            PostSummaryMessage("Total number of records successfully processed = " + _iTotalSuccess);
            PostSummaryMessage("Total number of records errored out = " + _iTotalError);
        }

        protected virtual void InitSummaryVariables()
        {
            _iTotalRecords= 0;
            _iTotalSuccess = 0;
            _iTotalError = 0;
        }
    }
}
