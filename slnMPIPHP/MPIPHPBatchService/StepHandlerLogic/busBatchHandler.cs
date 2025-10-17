using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Net;
using System.Xml;
using Microsoft.Reporting.WinForms;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.CorBuilder;
using Sagitec.Interface;
using MPIPHP.BusinessObjects;
using MPIPHP.Common;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Sagitec.DBUtility;
using Sagitec.DataObjects;
using System.Linq;
using Sagitec.CustomDataObjects;
using System.Data.SqlClient;
using System.Data.Sql;
using Sagitec.ExceptionPub;
using System.Threading.Tasks;


namespace MPIPHPJobService
{
    public class busBatchHandler
    {
        #region [Member Variables]
        public busJobHeader ibusJobHeader;
        public static CorBuilderXML iobjCorBuilder;
        public busSystemManagement iobjSystemManagement;
        public cdoBatchSchedule icdoBatchSchedule;

        public delegate void UpdateProcessLog(string astrMessage, string astrMessageType, string astrStepName);
        public UpdateProcessLog idlgUpdateProcessLog;
        public utlPassInfo iobjPassInfo { get; set; }
        public int iintOutboundReturnCode { get; set; }
        public string istrOutboundErrorMessage { get; set; }

        public string istrProcessName;
        public int iintCount = 0;
        public int iintInternalCount = 0;
        public int iintLimit;
        public int iintMaxCount;
        public string istrMergeFilePrefix;
        protected int iintTotalRecordCount;
        protected int iintSuccessRecordCount;
        protected int iintFailureRecordCount;

        #endregion

        #region [Public Methods]
        public busBatchHandler(busJobHeader abusJobHeader = null)
        {
            ibusJobHeader = abusJobHeader;
        }
        /// <summary>
        /// Start Counter
        /// </summary>
        public void StartCounter()
        {
            idlgUpdateProcessLog("Started processing.. ", busConstant.MPIPHPBatch.MESSAGE_TYPE_INFORMATION, istrProcessName);
            iintCount = 0;
            iintInternalCount = 0;
        }

        /// <summary>
        /// Start Counter and set Limit
        /// </summary>
        /// <param name="aintLimit"></param>
        public void StartCounter(int aintLimit)
        {
            StartCounter();
            iintLimit = aintLimit;
        }

        /// <summary>
        /// Core Processing Logic goes here.
        /// </summary>
        public virtual void Process()
        {

        }

        /// <summary>
        /// Call Counter
        /// </summary>
        public void CallCounter()
        {
            if (iintInternalCount == iintLimit)
            {
                // Display this message if max count is available
                if (iintMaxCount != 0)
                    idlgUpdateProcessLog("Currently processing record " + iintCount + " of " + iintMaxCount, busConstant.MPIPHPBatch.MESSAGE_TYPE_INFORMATION, istrProcessName);
                else
                    idlgUpdateProcessLog("Currently processing record " + iintCount, busConstant.MPIPHPBatch.MESSAGE_TYPE_INFORMATION, istrProcessName);

                iintInternalCount = 0;
            }
            iintCount++;
            iintInternalCount++;
        }

        /// <summary>
        /// End Counter.
        /// </summary>
        public void EndCounter()
        {
            idlgUpdateProcessLog("Ended processing.. Total records read " + iintCount, busConstant.MPIPHPBatch.MESSAGE_TYPE_INFORMATION, istrProcessName);
        }

        /// <summary>
        /// Post Message
        /// </summary>
        /// <param name="astrMessage">Message</param>
        /// <param name="astrMessageType">Message Type</param>
        public void PostMessage(string astrMessage, string astrMessageType)
        {
            idlgUpdateProcessLog(astrMessage, astrMessageType, istrProcessName);
        }

        /// <summary>
        /// Post Error Message
        /// </summary>
        /// <param name="astrErrorMessage">Error Message</param>
        public void PostErrorMessage(string astrErrorMessage)
        {
            PostMessage(astrErrorMessage, busConstant.MPIPHPBatch.MESSAGE_TYPE_ERROR);
        }

        /// <summary>
        /// Post Information Message
        /// </summary>
        /// <param name="astrErrorMessage">Information Message</param>
        public void PostInfoMessage(string astrErrorMessage)
        {
            PostMessage(astrErrorMessage, busConstant.MPIPHPBatch.MESSAGE_TYPE_INFORMATION);
        }

        /// <summary>
        /// Post Summary Message
        /// </summary>
        /// <param name="astrSummaryMessage">Summary Message</param>
        public void PostSummaryMessage(string astrSummaryMessage)
        {
            PostMessage(astrSummaryMessage, busConstant.MPIPHPBatch.MESSAGE_TYPE_SUMMARY);
        }

        /// <summary>
        /// Generate correspondence, create the tracking record for the generated letter
        /// </summary>
        /// <param name="aintTemplateID"></param>
        /// <param name="aintPersonID"></param>
        /// <param name="astrUserId"></param>
        /// <param name="aarrResult"></param>
        /// <returns></returns>
        public string CreateCorrespondence(string astrTemplateName, string astrUserID, int aintUserSerialID, ArrayList aarrResult, Hashtable ahtbQueryBkmarks, bool ablnIsPDF = false, string astrActiveAddr = busConstant.FLAG_YES)
        {

            iobjPassInfo.istrUserID = this.ibusJobHeader.icdoJobHeader.created_by;
            DataTable ldtbList = busBase.Select<cdoUser>(new string[1] { "USER_ID" },
                                  new object[1] { this.ibusJobHeader.icdoJobHeader.created_by }, null, null);
            busUser iBusUser = new busUser { icdoUser = new cdoUser() };
            if (ldtbList.Rows.Count > 0)
            {
                iBusUser.icdoUser.LoadData(ldtbList.Rows[0]);
                this.iobjPassInfo.istrUserID = iBusUser.icdoUser.user_id; this.iobjPassInfo.iintUserSerialID = iBusUser.icdoUser.user_serial_id;
            }

            utlCorresPondenceInfo lobjCorresPondenceInfo = busMPIPHPBase.SetCorrespondence(astrTemplateName,
                           iobjPassInfo.istrUserID, iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks);

            string lstrFileName = string.Empty;

            if (lobjCorresPondenceInfo == null)
            {
                throw new Exception("Unable to create correspondence, SetCorrespondence method not found in business solutions base object");
            }

            lobjCorresPondenceInfo.istrAutoPrintFlag = "N";

            string lstrLastName = string.Empty;
            string lstrMPID = string.Empty;

            int iintCount = 0; string istrAddressType = string.Empty;
            foreach (utlBookmarkFieldInfo obj in lobjCorresPondenceInfo.icolBookmarkFieldInfo)
            {
                if (obj.istrDataType == "String" && !(string.IsNullOrEmpty(obj.istrValue)))
                {
                    if (obj.istrObjectField == "istrAddrLine1" || obj.istrObjectField == "istrAddrLine2" || obj.istrObjectField == "istrAddrLine3" || obj.istrObjectField == "istrCountryDescription" ||
                        obj.istrObjectField == "istrState" || obj.istrObjectField == "istrCity" || obj.istrObjectField == "istrZipCode" || obj.istrObjectField == "istrRecepientName" ||
                        obj.istrObjectField == "ibusAlternatePayee.icdoPerson.istrFullName" || obj.istrObjectField == "ibusPayeeAccount.istrBeneficiaryFullName" ||
                        obj.istrObjectField == "icdoPayeeAccount.istrParticipantName" || obj.istrObjectField == "ibusParticipant.ibusPersonCourtContact.icdoPersonContact.contact_name" ||
                        obj.istrObjectField == "ibusAlternatePayee.ibusPersonAddressForCorr.icdoPersonAddress.addr_line_1" ||
                        obj.istrObjectField == "ibusAlternatePayee.ibusPersonAddressForCorr.icdoPersonAddress.addr_line_2" ||
                        obj.istrObjectField == "ibusAlternatePayee.ibusPersonAddressForCorr.icdoPersonAddress.addr_city" ||
                        obj.istrObjectField == "ibusAlternatePayee.ibusPersonAddressForCorr.icdoPersonAddress.addr_state_value" ||
                        obj.istrObjectField == "ibusAlternatePayee.ibusPersonAddressForCorr.icdoPersonAddress.istrCompleteZipCode" ||
                        obj.istrObjectField == "ibusAlternatePayee.ibusPersonAddressForCorr.icdoPersonAddress.foreign_postal_code" ||
                        obj.istrObjectField == "ibusAlternatePayee.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_description" ||
                        obj.istrObjectField == "ibusParticipant.ibusPersonCourtContact.icdoPersonContact.addr_line_1" ||
                        obj.istrObjectField == "ibusParticipant.ibusPersonCourtContact.icdoPersonContact.addr_line_2" ||
                        obj.istrObjectField == "ibusParticipant.ibusPersonCourtContact.icdoPersonContact.addr_city" ||
                        obj.istrObjectField == "ibusParticipant.ibusPersonCourtContact.icdoPersonContact.addr_state_value" ||
                        obj.istrObjectField == "ibusParticipant.ibusPersonCourtContact.icdoPersonContact.istrCompleteZipCode" ||
                        obj.istrObjectField == "ibusParticipant.ibusPersonCourtContact.icdoPersonContact.foreign_postal_code" ||
                        obj.istrObjectField == "ibusParticipant.ibusPersonCourtContact.icdoPersonContact.addr_country_description" ||
                        obj.istrObjectField == "istrReportedBy" ||
                        obj.istrObjectField == "istrEmployerName" ||
                        obj.istrObjectField == "istrAddress1" ||
                        obj.istrObjectField == "istrCity1" ||
                        obj.istrObjectField == "istrState" ||
                        obj.istrObjectField == "istrPostalCode" ||
                        obj.istrObjectField == "istrStreet" ||
                        obj.istrObjectField == "istrAddress2" || obj.istrObjectField == "istrApprovedByUserInitials" ||
                        obj.istrObjectField == "ibusParticipant.ibusPersonCourtContact.icdoPersonContact.county" ||
                        obj.istrObjectField == "icdoDroApplication.case_number" || obj.istrObjectField == "istrPayeeFullName"
                        )
                    {
                        obj.istrValue = obj.istrValue.ToUpper();
                    }
                    else
                        obj.istrValue = obj.istrValue.ToProperCase();
                }
                //PROD PIR 814
                if (this.ibusJobHeader.icdoJobHeader.job_schedule_id == 34 || astrTemplateName == busConstant.IAP_HARDSHIP_PAYMENT_CONFIRMATION
                    || this.ibusJobHeader.icdoJobHeader.iblnGeneratePdfFlag) //PROD PIR 845
                {
                    if ((obj.istrName == "stdMbrAdrCorStreet1" || obj.istrName == "stdMbrAdrCorStreet2") && obj.istrValue.IsNullOrEmpty())
                        iintCount++;
                    if (obj.istrName == "stdIsUSA" && (obj.istrValue == "1" && obj.istrValue.IsNotNullOrEmpty()))
                        istrAddressType = "DMST";
                    else if (obj.istrName == "stdIsUSA" && (obj.istrValue != "1" && obj.istrValue.IsNotNullOrEmpty()))
                        istrAddressType = "INTR";

                }

                //rid 80600
                if (obj.istrName == "stdMbrLastName")
                {
                    lstrLastName = string.IsNullOrEmpty(obj.istrValue) ? string.Empty : obj.istrValue;
                }
                if (obj.istrName == "stdMbrParticipantMPID")
                {
                    lstrMPID = string.IsNullOrEmpty(obj.istrValue) ? string.Empty : obj.istrValue;
                }
            }

            if (iintCount == 2 && (this.ibusJobHeader.icdoJobHeader.job_schedule_id == 34 || astrTemplateName == busConstant.IAP_HARDSHIP_PAYMENT_CONFIRMATION
                || this.ibusJobHeader.icdoJobHeader.iblnGeneratePdfFlag)) //PROD PIR 845
                istrAddressType = "BAD";

            try
            {
                iobjCorBuilder = new CorBuilderXML();
                iobjCorBuilder.InstantiateWord();
                lstrFileName = iobjCorBuilder.CreateCorrespondenceFromTemplate(astrTemplateName,
                    lobjCorresPondenceInfo, astrUserID);                

                if (ablnIsPDF)
                {
                    if (this.ibusJobHeader.icdoJobHeader.job_schedule_id == 34 || astrTemplateName == busConstant.IAP_HARDSHIP_PAYMENT_CONFIRMATION
                        || this.ibusJobHeader.icdoJobHeader.iblnGeneratePdfFlag) //PROD PIR 845
                    {
                        if (istrAddressType != "BAD")
                        {
                            string lstrFilepath = iobjSystemManagement.icdoSystemManagement.base_directory + "Correspondence\\Generated\\" + lstrFileName;
                            busGlobalFunctions.RenderWordAsPDF(lstrFilepath, astrActiveAddr, istrAddressType, busConstant.BOOL_TRUE, lstrLastName, lstrMPID); // PROD PIR 845
                        }
                    }
                    else
                    {
                        string lstrFilepath = iobjSystemManagement.icdoSystemManagement.base_directory + "Correspondence\\Generated\\" + lstrFileName;
                        busGlobalFunctions.RenderWordAsPDF(lstrFilepath, astrActiveAddr);
                    }
                }
                if (astrTemplateName == busConstant.IAP_RMD_PACKET)
                {
                    if (lstrFileName.IsNotNullOrEmpty())
                        InsertIntoCorPacketContentTracking(lobjCorresPondenceInfo.iintCorrespondenceTrackingId, aarrResult[0]);
                }

                iobjCorBuilder.CloseWord();
            }
            catch (Exception e)
            {
                if (iobjCorBuilder != null)
                {
                    iobjCorBuilder.CloseWord();
                }
            }

            return lstrFileName;
        }

        public void DeleteFile(int aintFileID, string astrFileNameToBeDeleted = "")
        {
            if (iobjSystemManagement == null)
            {
                iobjSystemManagement = new busSystemManagement();
                iobjSystemManagement.FindSystemManagement();
            }
            if (astrFileNameToBeDeleted.IsNullOrEmpty())
                astrFileNameToBeDeleted = busGlobalFunctions.GetFileName(aintFileID);
            if (astrFileNameToBeDeleted.IsNotNullOrEmpty())
            {
                string lstrFileName = iobjSystemManagement.icdoSystemManagement.base_directory;
                busFile lbusFile = new busFile();
                lbusFile.FindFile(aintFileID);

                if (lbusFile.icdoFile.mailbox_path_code.IsNotNullOrEmpty())
                {
                    DataTable ldt = busBase.Select("cdoSystemPaths.GetPathByCode", new object[1] { lbusFile.icdoFile.mailbox_path_code });
                    if (ldt.IsNotNull() && ldt.Rows.Count > 0)
                    {
                        string lstrpathcode = Convert.ToString(ldt.Rows[0]["path_value"]);
                        if (Directory.Exists(lstrFileName + lstrpathcode))
                        {
                            lstrFileName += lstrpathcode;
                        }
                    }
                    string[] lstrList = System.IO.Directory.GetFiles(lstrFileName);
                    foreach (string lstrFileToBeDeleted in lstrList)
                    {
                        if (!Path.GetExtension(lstrFileToBeDeleted).Contains("xml"))
                        {
                            if (lstrFileToBeDeleted.Contains(astrFileNameToBeDeleted))
                            {
                                System.IO.File.Delete(lstrFileToBeDeleted);
                            }
                        }
                    }
                }
            }
        }

        public void DeleteIAPRecalculatePDFFile(DataRow acdoPerson)
        {
            string astrImagedFileName = string.Empty;
            utlPassInfo lobjPassInfo = utlPassInfo.iobjPassInfo;
           
            busSystemManagement iobjSystemManagement = null;
            iobjSystemManagement = new busSystemManagement();
            iobjSystemManagement.FindSystemManagement();
           
            string istrfilepath = "";
            if (!string.IsNullOrEmpty(iobjSystemManagement.icdoSystemManagement.base_directory))
            {
               
                istrfilepath = iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Report_IAP_Recalculation_Report_File_Path;
                astrImagedFileName = istrfilepath + acdoPerson["FILE_NAME"].ToString() + ".pdf";
                if (Directory.Exists(istrfilepath))
                {
                    if (File.Exists(astrImagedFileName))
                    {
                        System.IO.File.Delete(astrImagedFileName);
                    }
                }

            }
        }

        public string CreatePDFReport(DataTable ldtbResultTable, string astrReportName, string astrPrefix = "", string astrSubfolder = "")
        {
            ////PIR-868
            string outPutFilePathMD = string.Empty;
            string outPutReportPathMD = string.Empty;
            string outPutFilePathBIS = string.Empty;
            string outPutReportPathBIS = string.Empty;
            if (astrPrefix== busConstant.MD_BATCH_NAME)
            {
               
                 outPutFilePathMD = iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Report_Path_MD + DateTime.Today.Year;
                if (!Directory.Exists(outPutFilePathMD))
                    Directory.CreateDirectory(outPutFilePathMD);
                if (Directory.Exists(outPutFilePathMD))
                {
                   
                        outPutReportPathMD = outPutFilePathMD + '\\' + "Report";
                    if (!Directory.Exists(outPutReportPathMD))
                        Directory.CreateDirectory(outPutReportPathMD);
                    if (astrSubfolder.IsNotNullOrEmpty())
                    {
                        outPutReportPathMD = outPutFilePathMD + '\\' + "Report" + '\\' + astrSubfolder;
                        if (!Directory.Exists(outPutReportPathMD))
                            Directory.CreateDirectory(outPutReportPathMD);
                    }
                }

            }
            if (astrPrefix ==busConstant.NOTIFICATIONBIS_BATCH_NAME)
            {

                outPutFilePathBIS = iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Report_Path_BIS + DateTime.Today.Year;
                if (!Directory.Exists(outPutFilePathBIS))
                    Directory.CreateDirectory(outPutFilePathBIS);
                if (Directory.Exists(outPutFilePathBIS))
                {
                    outPutReportPathBIS = outPutFilePathBIS + '\\' + "Report";
                    if (!Directory.Exists(outPutReportPathBIS))
                        Directory.CreateDirectory(outPutReportPathBIS);
                }

            }
            ReportViewer rvViewer = new ReportViewer();
            Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;
            string labsRptDefPath = string.Empty;

            DataTable ldtbReportTable = ldtbResultTable;

            rvViewer.ProcessingMode = ProcessingMode.Local;
            labsRptDefPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION);

            rvViewer.LocalReport.ReportPath = labsRptDefPath + astrReportName + ".rdlc";
            ReportDataSource lrdsReport = new ReportDataSource(ldtbReportTable.TableName, ldtbReportTable);

            rvViewer.LocalReport.DataSources.Add(lrdsReport);
            byte[] bytes = rvViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

            string labsRptGenPath = string.Empty;

            if (astrPrefix.IsNotNullOrEmpty() && astrPrefix.Contains("TRIAL"))
            {
                labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_TRIAL_REPORT_PATH);
            }
            else if (astrPrefix.IsNotNullOrEmpty() && astrPrefix.Contains("FINAL"))
            {
                labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_FINAL_REPORT_PATH);
            }
            else if (astrPrefix.IsNotNullOrEmpty() && astrPrefix.Contains("Annual"))
            {
                labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_ANNUAL_REPORT_PATH);
                astrPrefix = string.Empty;
            }
            else
            {
                //PIR : 868
                if (astrPrefix.IsNotNullOrEmpty() && astrPrefix == busConstant.MD_BATCH_NAME)
                {
                    if (astrSubfolder.IsNotNullOrEmpty())
                        labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_MINIMUMDISTRIBUTION_REPORT_PATH) + '\\' + DateTime.Today.Year + '\\' + "Report" + '\\'+ astrSubfolder+'\\';
                    else
                        labsRptGenPath =  iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_MINIMUMDISTRIBUTION_REPORT_PATH )+'\\'+ DateTime.Today.Year+ '\\' + "Report" + '\\';
                }
                else if (astrPrefix.IsNotNullOrEmpty() && astrPrefix == busConstant.NOTIFICATIONBIS_BATCH_NAME)
                {
                    labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_BIS_REPORT_PATH) + '\\' + DateTime.Today.Year + '\\' + "Report"+'\\';
                }
                else if (astrPrefix.IsNotNullOrEmpty() && astrPrefix == busConstant.Report_Path)  //PIR 978
                {
                    labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_PREBIS);
                    astrPrefix = string.Empty;
                }

                else
                {
                    labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_GENERATED);
                }
            }

            string lstrReportFullName = string.Empty;

            if (astrPrefix.IsNotNullOrEmpty())
                lstrReportFullName = labsRptGenPath + astrPrefix + "_" + astrReportName + "_" +
                    DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
            else
                lstrReportFullName = labsRptGenPath + astrReportName + "_" +
                    DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";

            FileStream fs = new FileStream(@lstrReportFullName,
               FileMode.Create);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
            return lstrReportFullName;
        }
        public string CreatePDFReport(DataSet ldtbResultTable, string astrReportName, string astrPrefix = "")
        {

            ReportViewer rvViewer = new ReportViewer();
            Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;
            string labsRptDefPath = string.Empty;

            //DataTable ldtbReportTable = ldtbResultTable;

            rvViewer.ProcessingMode = ProcessingMode.Local;
            labsRptDefPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION);

            rvViewer.LocalReport.ReportPath = labsRptDefPath + astrReportName + ".rdlc";
            //ReportDataSource lrdsReport = new ReportDataSource(ldtbResultTable.DataSetName, ldtbResultTable);

            //rvViewer.LocalReport.DataSources.Add(lrdsReport);
            foreach (DataTable dtResultTable in ldtbResultTable.Tables)
            {
                DataTable ldtbReportTable = dtResultTable;
                ReportDataSource lrdsReport = new ReportDataSource(ldtbReportTable.TableName, ldtbReportTable);
                rvViewer.LocalReport.DataSources.Add(lrdsReport);
            }


            byte[] bytes = rvViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

            string labsRptGenPath = string.Empty;

            if (astrPrefix.Contains("TRIAL"))
            {
                labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_TRIAL_REPORT_PATH);
            }
            else if (astrPrefix.Contains("FINAL"))
            {
                labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_FINAL_REPORT_PATH);
            }
            else if (astrPrefix.Contains("Annual"))
            {
                labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_ANNUAL_REPORT_PATH);
                astrPrefix = string.Empty;
            }
            else if (astrPrefix.Contains(busConstant.MPIPHPBatch.GENERATED_SSADISABILITY_REPORT_PATH))
            {
                labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_SSADISABILITY_REPORT_PATH);
                astrPrefix = string.Empty;
            }
            else if (astrPrefix.Contains(busConstant.MPIPHPBatch.GENERATED_PENSION_ELIGIBILITY_REPORT_PATH))
            {
                labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_PENSION_ELIGIBILITY_REPORT_PATH);
                astrPrefix = string.Empty;
            }
            else if (astrPrefix.Contains(busConstant.MPIPHPBatch.GENERATED_RITIREE_HEALTH_ELIGIBILITY_REPORT_PATH))
            {
                labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_RITIREE_HEALTH_ELIGIBILITY_REPORT_PATH);
                astrPrefix = string.Empty;
            }
            else if (astrPrefix.Contains(busConstant.MPIPHPBatch.GENERATE_STATE_TAX_UPDATE_BATCH_PATH))
            {
                labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATE_STATE_TAX_UPDATE_BATCH_PATH);
                astrPrefix = string.Empty;
            }
            else
            {
                labsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_GENERATED);
            }

            string lstrReportFullName = string.Empty;

            if (astrPrefix.IsNotNullOrEmpty())
                lstrReportFullName = labsRptGenPath + astrPrefix + "_" + astrReportName + "_" +
                    DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
            else
                lstrReportFullName = labsRptGenPath + astrReportName + "_" +
                    DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";

            FileStream fs = new FileStream(@lstrReportFullName,
               FileMode.Create);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
            return lstrReportFullName;
        }

        #endregion

        #region Merge PDF into single PDF file //PROD PIR 814
        public void MergePdfsFromPath(string astrGeneratedFilePath, string astrOutFilePath = "", bool ablnIsBISBatch = false, bool ablnIsMDBatch = false, bool ablnDoNotDelete = false)
        {
            MergePDFs(astrGeneratedFilePath, "DMST", astrOutFilePath, ablnIsBISBatch, ablnIsMDBatch, ablnDoNotDelete); //PROD PIR 845-(second last parameter added For BIS batch)
            MergePDFs(astrGeneratedFilePath, "INTR", astrOutFilePath, ablnIsBISBatch, ablnIsMDBatch, ablnDoNotDelete); //PROD PIR 845-(last parameter added For MD batch)
        }
        void MergePDFs(string generatedpath, string astrpostfix, string astrOutFilePath = "", bool ablnIsBISBatch = false, bool ablnIsMDBatch = false, bool ablnDoNotDelete = false)//PROD PIR 845 -- RASHMI(second last parameter added For BIS batch and last parameter added for MD batch)
        {
            DirectoryInfo dir = new DirectoryInfo(generatedpath);
            List<string> filesPath = new List<string>();
            foreach (FileInfo fi in dir.GetFiles("*.*").OrderBy(item => item.CreationTime))
            {
                if (fi.CreationTime > ibusJobHeader.icdoJobHeader.start_time)
                    filesPath.Add(fi.FullName);
            }
            //Get All Files
            List<string> AllPostFixs = (from obj in filesPath
                                        where obj.Contains(astrpostfix)
                                        select obj).ToList();            

            if (AllPostFixs != null && AllPostFixs.Count() > 0)
            {
                AllPostFixs = AllPostFixs.Where(obj => obj.Contains(".pdf")).ToList();
                AllPostFixs = AllPostFixs.Select(o => o.Replace(".pdf", "")).Distinct().ToList();

                List<PdfReader> readerList = new List<PdfReader>();
                string outPutFilePath = string.Empty;
                string outPutFilePathVested = string.Empty;
                string outPutFilePathNonVested = string.Empty;
                string lstrVested = string.Empty;
                //PROD PIR 845 - MD_BATCH

                #region PIR 861 Code Commented
                //string outPutFilePathMDCoverLttr = string.Empty;
                //string outPutFilePathFormMD = string.Empty;
                //string outPutFilePathFORML52 = string.Empty;
                //string outPutFilePathFORML600 = string.Empty;
                //string outPutFilePathFORML666 = string.Empty;
                //string outPutFilePathFORML700 = string.Empty;
                //string outPutFilePathFORML161 = string.Empty; //MD_BATCH By Suresh
                #endregion PIR 861 Code Commented
                //PIR 861
                string outPutFilePathMD = string.Empty;

                if (!ablnIsBISBatch)
                {
                    //PROD PIR 845 - MD_BATCH                    
                    if (ablnIsMDBatch)
                    {
                        #region PIR 861 Code Commented
                        ////New Name and Path of FIle.                   
                        //outPutFilePathMDCoverLttr = astrOutFilePath + "MD_COVER_LTTR" + "\\" + astrpostfix + "\\" + astrpostfix + "-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
                        //outPutFilePathFormMD = astrOutFilePath + "FORM_MPI" + "\\" + astrpostfix + "\\" + astrpostfix + "-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
                        //outPutFilePathFORML52 = astrOutFilePath + "FORM_LOCAL_52" + "\\" + astrpostfix + "\\" + astrpostfix + "-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
                        //outPutFilePathFORML600 = astrOutFilePath + "FORM_LOCAL_600" + "\\" + astrpostfix + "\\" + astrpostfix + "-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
                        //outPutFilePathFORML666 = astrOutFilePath + "FORM_LOCAL_666" + "\\" + astrpostfix + "\\" + astrpostfix + "-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
                        //outPutFilePathFORML700 = astrOutFilePath + "FORM_LOCAL_700" + "\\" + astrpostfix + "\\" + astrpostfix + "-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
                        ////MD_BATCH By Suresh
                        ////outPutFilePathFORML161 = astrOutFilePath + "FORM_LOCAL_161" + "\\" + astrpostfix + "\\" + astrpostfix + "-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
                        //outPutFilePathFORML161 = astrOutFilePath + "FORM_LOCAL_161" + "\\" + astrpostfix + "\\";
                        //if (!Directory.Exists(outPutFilePathFORML161))
                        //    Directory.CreateDirectory(outPutFilePathFORML161);
                        //outPutFilePathFORML161 = outPutFilePathFORML161 + astrpostfix + "-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
                        #endregion PIR 861 Code Commented

                        //PIR 861
                        outPutFilePathMD = astrOutFilePath + DateTime.Today.Year + "\\" + astrpostfix + "\\";
                        if (!Directory.Exists(outPutFilePathMD))
                            Directory.CreateDirectory(outPutFilePathMD);
                        outPutFilePathMD = outPutFilePathMD + "\\" + "MD-" + astrpostfix + "-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
                    }
                    else
                    {
                        //New Name and Path of FIle.
                        if (istrMergeFilePrefix.IsNotNullOrEmpty())
                            outPutFilePath = astrOutFilePath + "\\" + istrMergeFilePrefix + "-" + astrpostfix + "-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
                        else
                            outPutFilePath = astrOutFilePath + astrpostfix + "\\" + astrpostfix + "-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
                    }
                }
                else //PROD PIR 845 -- RASHMI(Output path added For BIS batch)
                {
                    //New Name and Path of FIle.
                    outPutFilePathVested = astrOutFilePath + "VESTED" + "\\" + astrpostfix + "\\" + astrpostfix + "-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
                    outPutFilePathNonVested = astrOutFilePath + "NON-VESTED" + "\\" + astrpostfix + "\\" + astrpostfix + "-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
                }
                List<string> SelectedfilesPath = new List<string>();
                SelectedfilesPath = filesPath.Where(obj => obj.EndsWith(astrpostfix + ".pdf")).ToList();

                if (!ablnIsBISBatch)
                {
                    if (!ablnIsMDBatch)
                    {
                        foreach (string filePath in SelectedfilesPath)
                        {
                            PdfReader pdfReader = new PdfReader(filePath);
                            readerList.Add(pdfReader);
                        }
                        //Define a new output document and its size, type
                        Document document = new Document(PageSize.LETTER, 0, 0, 0, 0);
                        //Create blank output pdf file and get the stream to write on it.
                        PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outPutFilePath, FileMode.Create));
                        document.Open();
                        Console.WriteLine("Merging Files");
                        foreach (PdfReader reader in readerList)
                        {
                            for (int i = 1; i <= reader.NumberOfPages; i++)
                            {
                                PdfImportedPage page = writer.GetImportedPage(reader, i);
                                document.Add(iTextSharp.text.Image.GetInstance(page));
                            }
                        }
                        Console.WriteLine("Closing Files");
                        document.Close();
                    }
                    else
                    {
                        #region PIR 861 Code Commented
                        //List<string> SelectedfilesPath_MD_COVER_LTTR = new List<string>();
                        //List<string> SelectedfilesPath_FORM_MD = new List<string>();
                        //List<string> SelectedfilesPath_FORM_L52 = new List<string>();
                        //List<string> SelectedfilesPath_FORM_L600 = new List<string>();
                        //List<string> SelectedfilesPath_FORM_L666 = new List<string>();
                        //List<string> SelectedfilesPath_FORM_L700 = new List<string>();
                        //List<string> SelectedfilesPath_FORM_L161 = new List<string>(); //MD_BATCH By Suresh
                        #endregion PIR 861 Code Commented

                        List<string> SelectedfilesPath_MD = new List<string>();

                        #region PIR 861 Code Commented
                        //SelectedfilesPath_MD_COVER_LTTR = filesPath.Where(obj => obj.Contains("PER-0006")).ToList();
                        //SelectedfilesPath_MD_COVER_LTTR = SelectedfilesPath_MD_COVER_LTTR.Where(obj => obj.Contains(astrpostfix + ".pdf")).ToList();

                        //SelectedfilesPath_FORM_MD = filesPath.Where(obj => obj.Contains("RETR-0011")).ToList();
                        //SelectedfilesPath_FORM_MD = SelectedfilesPath_FORM_MD.Where(obj => obj.Contains(astrpostfix + ".pdf")).ToList();

                        //SelectedfilesPath_FORM_L52 = filesPath.Where(obj => obj.Contains("RETR-0027")).ToList();
                        //SelectedfilesPath_FORM_L52 = SelectedfilesPath_FORM_L52.Where(obj => obj.Contains(astrpostfix + ".pdf")).ToList();

                        //SelectedfilesPath_FORM_L600 = filesPath.Where(obj => obj.Contains("RETR-0026")).ToList();
                        //SelectedfilesPath_FORM_L600 = SelectedfilesPath_FORM_L600.Where(obj => obj.Contains(astrpostfix + ".pdf")).ToList();

                        //SelectedfilesPath_FORM_L666 = filesPath.Where(obj => obj.Contains("RETR-0025")).ToList();
                        //SelectedfilesPath_FORM_L666 = SelectedfilesPath_FORM_L666.Where(obj => obj.Contains(astrpostfix + ".pdf")).ToList();

                        //SelectedfilesPath_FORM_L700 = filesPath.Where(obj => obj.Contains("RETR-0024")).ToList();
                        //SelectedfilesPath_FORM_L700 = SelectedfilesPath_FORM_L700.Where(obj => obj.Contains(astrpostfix + ".pdf")).ToList();

                        ////MD_BATCH By Suresh
                        //SelectedfilesPath_FORM_L161 = filesPath.Where(obj => obj.Contains("RETR-0034")).ToList();//Rohan 2/17/2015
                        //SelectedfilesPath_FORM_L161 = SelectedfilesPath_FORM_L161.Where(obj => obj.Contains(astrpostfix + ".pdf")).ToList();
                        #endregion PIR 861 Code Commented

                        SelectedfilesPath_MD = filesPath.Where(obj => obj.Contains("PER-0016") || obj.Contains("PER-0006") || obj.Contains("RETR-0011") || obj.Contains("RETR-0024")
                           || obj.Contains("RETR-0025") || obj.Contains("RETR-0026") || obj.Contains("RETR-0027") || obj.Contains("RETR-0034")).ToList();
                        SelectedfilesPath_MD = SelectedfilesPath_MD.Where(obj => obj.Contains(astrpostfix + ".pdf")).ToList();

                        if (SelectedfilesPath_MD.Count > 0)
                        {
                            readerList.Clear();
                            foreach (string filePath in SelectedfilesPath_MD)
                            {
                                PdfReader pdfReader = new PdfReader(filePath);
                                readerList.Add(pdfReader);
                            }
                            //Define a new output document and its size, type
                            Document document = new Document(PageSize.LETTER, 0, 0, 0, 0);
                            //Create blank output pdf file and get the stream to write on it.
                            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outPutFilePathMD, FileMode.Create));
                            document.Open();
                            Console.WriteLine("Merging Files");
                            foreach (PdfReader reader in readerList)
                            {
                                for (int i = 1; i <= reader.NumberOfPages; i++)
                                {
                                    PdfImportedPage page = writer.GetImportedPage(reader, i);
                                    var pdfImage = iTextSharp.text.Image.GetInstance(page);
                                    pdfImage.Alignment = Element.ALIGN_CENTER;
                                    pdfImage.ScaleToFit(document.PageSize.Width, document.PageSize.Height);

                                    document.Add(pdfImage);
                                    // document.Add(iTextSharp.text.Image.GetInstance(page));
                                }
                            }
                            Console.WriteLine("Closing Files");
                            document.Close();
                        }


                        #region PIR 861 Code Commented
                        //if (SelectedfilesPath_MD_COVER_LTTR.Count > 0)
                        //{
                        //    foreach (string filePath in SelectedfilesPath_MD_COVER_LTTR)
                        //    {
                        //        PdfReader pdfReader = new PdfReader(filePath);
                        //        readerList.Add(pdfReader);
                        //    }
                        //    //Define a new output document and its size, type
                        //    Document document = new Document(PageSize.LETTER, 0, 0, 0, 0);
                        //    //Create blank output pdf file and get the stream to write on it.
                        //    PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outPutFilePathMDCoverLttr, FileMode.Create));
                        //    document.Open();
                        //    Console.WriteLine("Merging Files");
                        //    foreach (PdfReader reader in readerList)
                        //    {
                        //        for (int i = 1; i <= reader.NumberOfPages; i++)
                        //        {
                        //            PdfImportedPage page = writer.GetImportedPage(reader, i);
                        //            document.Add(iTextSharp.text.Image.GetInstance(page));
                        //        }
                        //    }
                        //    Console.WriteLine("Closing Files");
                        //    document.Close();
                        //}
                        //if (SelectedfilesPath_FORM_MD.Count > 0)
                        //{
                        //    readerList.Clear();
                        //    foreach (string filePath in SelectedfilesPath_FORM_MD)
                        //    {
                        //        PdfReader pdfReader = new PdfReader(filePath);
                        //        readerList.Add(pdfReader);
                        //    }
                        //    //Define a new output document and its size, type
                        //    Document document = new Document(PageSize.LETTER, 0, 0, 0, 0);
                        //    //Create blank output pdf file and get the stream to write on it.
                        //    PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outPutFilePathFormMD, FileMode.Create));
                        //    document.Open();
                        //    Console.WriteLine("Merging Files");
                        //    foreach (PdfReader reader in readerList)
                        //    {
                        //        for (int i = 1; i <= reader.NumberOfPages; i++)
                        //        {
                        //            PdfImportedPage page = writer.GetImportedPage(reader, i);
                        //            var pdfImage = iTextSharp.text.Image.GetInstance(page);
                        //            pdfImage.Alignment = Element.ALIGN_CENTER;
                        //            pdfImage.ScaleToFit(document.PageSize.Width, document.PageSize.Height);

                        //            document.Add(pdfImage);
                        //           // document.Add(iTextSharp.text.Image.GetInstance(page));
                        //        }
                        //    }
                        //    Console.WriteLine("Closing Files");
                        //    document.Close();
                        //}
                        //if (SelectedfilesPath_FORM_L52.Count > 0)
                        //{
                        //    readerList.Clear();
                        //    foreach (string filePath in SelectedfilesPath_FORM_L52)
                        //    {
                        //        PdfReader pdfReader = new PdfReader(filePath);
                        //        readerList.Add(pdfReader);
                        //    }
                        //    //Define a new output document and its size, type
                        //    Document document = new Document(PageSize.LETTER, 0, 0, 0, 0);
                        //    //Create blank output pdf file and get the stream to write on it.
                        //    PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outPutFilePathFORML52, FileMode.Create));
                        //    document.Open();
                        //    Console.WriteLine("Merging Files");
                        //    foreach (PdfReader reader in readerList)
                        //    {
                        //        for (int i = 1; i <= reader.NumberOfPages; i++)
                        //        {
                        //            PdfImportedPage page = writer.GetImportedPage(reader, i);
                        //            document.Add(iTextSharp.text.Image.GetInstance(page));
                        //        }
                        //    }
                        //    Console.WriteLine("Closing Files");
                        //    document.Close();
                        //}
                        //if (SelectedfilesPath_FORM_L600.Count > 0)
                        //{
                        //    readerList.Clear();
                        //    foreach (string filePath in SelectedfilesPath_FORM_L600)
                        //    {
                        //        PdfReader pdfReader = new PdfReader(filePath);
                        //        readerList.Add(pdfReader);
                        //    }
                        //    //Define a new output document and its size, type
                        //    Document document = new Document(PageSize.LETTER, 0, 0, 0, 0);
                        //    //Create blank output pdf file and get the stream to write on it.
                        //    PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outPutFilePathFORML600, FileMode.Create));
                        //    document.Open();
                        //    Console.WriteLine("Merging Files");
                        //    foreach (PdfReader reader in readerList)
                        //    {
                        //        for (int i = 1; i <= reader.NumberOfPages; i++)
                        //        {
                        //            PdfImportedPage page = writer.GetImportedPage(reader, i);
                        //            document.Add(iTextSharp.text.Image.GetInstance(page));
                        //        }
                        //    }
                        //    Console.WriteLine("Closing Files");
                        //    document.Close();
                        //}
                        //if (SelectedfilesPath_FORM_L666.Count > 0)
                        //{
                        //    readerList.Clear();
                        //    foreach (string filePath in SelectedfilesPath_FORM_L666)
                        //    {
                        //        PdfReader pdfReader = new PdfReader(filePath);
                        //        readerList.Add(pdfReader);
                        //    }
                        //    //Define a new output document and its size, type
                        //    Document document = new Document(PageSize.LETTER, 0, 0, 0, 0);
                        //    //Create blank output pdf file and get the stream to write on it.
                        //    PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outPutFilePathFORML666, FileMode.Create));
                        //    document.Open();
                        //    Console.WriteLine("Merging Files");
                        //    foreach (PdfReader reader in readerList)
                        //    {
                        //        for (int i = 1; i <= reader.NumberOfPages; i++)
                        //        {
                        //            PdfImportedPage page = writer.GetImportedPage(reader, i);
                        //            document.Add(iTextSharp.text.Image.GetInstance(page));
                        //        }
                        //    }
                        //    Console.WriteLine("Closing Files");
                        //    document.Close();
                        //}
                        //if (SelectedfilesPath_FORM_L700.Count > 0)
                        //{
                        //    readerList.Clear();
                        //    foreach (string filePath in SelectedfilesPath_FORM_L700)
                        //    {
                        //        PdfReader pdfReader = new PdfReader(filePath);
                        //        readerList.Add(pdfReader);
                        //    }
                        //    //Define a new output document and its size, type
                        //    Document document = new Document(PageSize.LETTER, 0, 0, 0, 0);
                        //    //Create blank output pdf file and get the stream to write on it.
                        //    PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outPutFilePathFORML700, FileMode.Create));
                        //    document.Open();
                        //    Console.WriteLine("Merging Files");
                        //    foreach (PdfReader reader in readerList)
                        //    {
                        //        for (int i = 1; i <= reader.NumberOfPages; i++)
                        //        {
                        //            PdfImportedPage page = writer.GetImportedPage(reader, i);
                        //            document.Add(iTextSharp.text.Image.GetInstance(page));
                        //        }
                        //    }
                        //    Console.WriteLine("Closing Files");
                        //    document.Close();
                        //}
                        ////MD_BATCH By Suresh
                        //if (SelectedfilesPath_FORM_L161.Count > 0)
                        //{
                        //    readerList.Clear();
                        //    foreach (string filePath in SelectedfilesPath_FORM_L161)
                        //    {
                        //        PdfReader pdfReader = new PdfReader(filePath);
                        //        readerList.Add(pdfReader);
                        //    }
                        //    //Define a new output document and its size, type
                        //    Document document = new Document(PageSize.LETTER, 0, 0, 0, 0);
                        //    //Create blank output pdf file and get the stream to write on it.
                        //    PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outPutFilePathFORML161, FileMode.Create));
                        //    document.Open();
                        //    Console.WriteLine("Merging Files");
                        //    foreach (PdfReader reader in readerList)
                        //    {
                        //        for (int i = 1; i <= reader.NumberOfPages; i++)
                        //        {
                        //            PdfImportedPage page = writer.GetImportedPage(reader, i);
                        //            document.Add(iTextSharp.text.Image.GetInstance(page));
                        //        }
                        //    }
                        //    Console.WriteLine("Closing Files");
                        //    document.Close();
                        //}
                        #endregion PIR 861 Code Commented
                    }
                }
                else //PROD PIR 845 -- RASHMI(added For BIS batch)
                {
                    List<string> SelectedfilesPath_Vested = new List<string>();
                    List<string> SelectedfilesPath_NonVested = new List<string>();

                    SelectedfilesPath_Vested = filesPath.Where(obj => obj.Contains("PERO-0006")).ToList();
                    SelectedfilesPath_Vested = SelectedfilesPath_Vested.Where(obj => obj.Contains(astrpostfix + ".pdf")).ToList();

                    SelectedfilesPath_NonVested = filesPath.Where(obj => obj.Contains("PERO-0002")).ToList();
                    SelectedfilesPath_NonVested = SelectedfilesPath_NonVested.Where(obj => obj.Contains(astrpostfix + ".pdf")).ToList();

                    if (SelectedfilesPath_Vested.Count > 0)
                    {
                        foreach (string filePath in SelectedfilesPath_Vested)
                        {
                            PdfReader pdfReader = new PdfReader(filePath);
                            //readerList.Clear();
                            readerList.Add(pdfReader);
                        }
                        //Define a new output document and its size, type
                        Document document = new Document(PageSize.LETTER, 0, 0, 0, 0);
                        //Create blank output pdf file and get the stream to write on it.
                        PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outPutFilePathVested, FileMode.Create));
                        document.Open();
                        Console.WriteLine("Merging Files");
                        foreach (PdfReader reader in readerList)
                        {
                            for (int i = 1; i <= reader.NumberOfPages; i++)
                            {
                                PdfImportedPage page = writer.GetImportedPage(reader, i);
                                document.Add(iTextSharp.text.Image.GetInstance(page));
                            }
                        }
                        Console.WriteLine("Closing Files");
                        document.Close();
                    }
                    if (SelectedfilesPath_NonVested.Count > 0)
                    {
                        readerList.Clear();
                        foreach (string filePath in SelectedfilesPath_NonVested)
                        {
                            PdfReader pdfReader = new PdfReader(filePath);
                            readerList.Add(pdfReader);
                        }
                        //Define a new output document and its size, type
                        Document document = new Document(PageSize.LETTER, 0, 0, 0, 0);
                        //Create blank output pdf file and get the stream to write on it.
                        PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outPutFilePathNonVested, FileMode.Create));
                        document.Open();
                        Console.WriteLine("Merging Files");
                        foreach (PdfReader reader in readerList)
                        {
                            for (int i = 1; i <= reader.NumberOfPages; i++)
                            {
                                PdfImportedPage page = writer.GetImportedPage(reader, i);
                                document.Add(iTextSharp.text.Image.GetInstance(page));
                            }
                        }
                        Console.WriteLine("Closing Files");
                        document.Close();
                    }
                }
                // Delete file if not required
                if (!ablnDoNotDelete)
                {
                    foreach (string filePath in SelectedfilesPath)
                    {
                        File.Delete(filePath);
                    }
                }
            }
        }
        #endregion
        public void InsertIntoCorPacketContentTracking(int aintTrackingId, object aobj)
        {
            busCorPacketContentTracking lbusCorPacketContentTracking = new busCorPacketContentTracking { icdoCorPacketContentTracking = new cdoCorPacketContentTracking() };

            DataTable ldtbPacketContentDetails = busBase.Select("cdoCorPacketContent.GetPacketContentDetails", new object[1] { aintTrackingId });
            if (ldtbPacketContentDetails != null && ldtbPacketContentDetails.Rows.Count > 0)
            {
                lbusCorPacketContentTracking.icdoCorPacketContentTracking.LoadData(ldtbPacketContentDetails.Rows[0]);
            }

            if (lbusCorPacketContentTracking.icdoCorPacketContentTracking.tracking_id > 0)
            {
                if (aobj is busBenefitApplication)
                {
                    lbusCorPacketContentTracking.icdoCorPacketContentTracking.benefit_appicaltion_id = (aobj as busBenefitApplication).icdoBenefitApplication.benefit_application_id;
                    lbusCorPacketContentTracking.icdoCorPacketContentTracking.retirement_date = (aobj as busBenefitApplication).icdoBenefitApplication.retirement_date;
                }
                else if (aobj is busBenefitCalculationHeader)
                {
                    lbusCorPacketContentTracking.icdoCorPacketContentTracking.benefit_calculation_header_id = (aobj as busBenefitCalculationHeader).icdoBenefitCalculationHeader.benefit_calculation_header_id;
                    lbusCorPacketContentTracking.icdoCorPacketContentTracking.retirement_date = (aobj as busBenefitCalculationHeader).icdoBenefitCalculationHeader.retirement_date;
                }
                if (aobj is busQdroApplication)
                {
                    lbusCorPacketContentTracking.icdoCorPacketContentTracking.qdro_appicaltion_id = (aobj as busQdroApplication).icdoDroApplication.dro_application_id;
                    lbusCorPacketContentTracking.icdoCorPacketContentTracking.retirement_date = (aobj as busQdroApplication).icdoDroApplication.dro_commencement_date;
                }
                else if (aobj is busQdroCalculationHeader)
                {
                    lbusCorPacketContentTracking.icdoCorPacketContentTracking.qdro_calculation_header_id = (aobj as busQdroCalculationHeader).icdoQdroCalculationHeader.qdro_calculation_header_id;
                    lbusCorPacketContentTracking.icdoCorPacketContentTracking.retirement_date = (aobj as busQdroCalculationHeader).icdoQdroCalculationHeader.qdro_commencement_date;
                }
                else if (aobj is busPerson)
                {
                    lbusCorPacketContentTracking.icdoCorPacketContentTracking.retirement_date = (aobj as busPerson).icdoPerson.retirement_health_date;
                }


                lbusCorPacketContentTracking.icdoCorPacketContentTracking.packet_status_value = busConstant.BENEFIT_APPLICATION_STATUS_PENDING;
                lbusCorPacketContentTracking.icdoCorPacketContentTracking.Insert();

                busCorPacketContentTrackingHistory lbusCorPacketContentTrackingHistory = new busCorPacketContentTrackingHistory { icdoCorPacketContentTrackingHistory = new cdoCorPacketContentTrackingHistory() };

                lbusCorPacketContentTrackingHistory.icdoCorPacketContentTrackingHistory.cor_packet_content_tracking_id = lbusCorPacketContentTracking.icdoCorPacketContentTracking.cor_packet_content_tracking_id;


                lbusCorPacketContentTrackingHistory.icdoCorPacketContentTrackingHistory.packet_status_value = busConstant.BENEFIT_APPLICATION_STATUS_PENDING;
                lbusCorPacketContentTrackingHistory.icdoCorPacketContentTrackingHistory.Insert();
            }

        }
    }
}
