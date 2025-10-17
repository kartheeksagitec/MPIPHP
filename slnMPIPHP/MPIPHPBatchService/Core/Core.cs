using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.Common;
using MPIPHP.BusinessObjects;
using MPIPHPJobService;
using Sagitec.BusinessObjects;
using MPIPHP.Common;
using System.Data;
using System.Collections.ObjectModel;
using Sagitec.DBUtility;
using MPIPHP.CustomDataObjects;
//using MPIPHP.MPIPHPJobService.StepHandlerLogic;
using System.Collections;
using Sagitec.CorBuilder;
using System.Net;
using System.IO;
using Microsoft.Reporting.WinForms;
using System.Xml;
using Sagitec.Interface;
using MPIPHP.DataObjects;

namespace MPIPHP.MPIPHPJobService.Core
{
    public delegate void VoidIntDelegate(int aintPercentage);
    public delegate bool BoolVoidDelegate();

    //public interface iiDataUnit<T>
    //{
    //    T GetDataUnit();
    //    int iintIdentifierValue { get; }
    //}
    public interface iiDataProcessor
    {
        /// <summary>
        /// Main function
        /// </summary>
        bool Process();

    }

    //public abstract class clsDataUnitBase<T> : iiDataUnit<T>
    //{
    //    protected T _iobjDataUnit;
    //    private int _iintIdentifierValue;

    //    public int iintIdentifierValue
    //    {
    //        get { return _iintIdentifierValue; }
    //    }


    //    public clsDataUnitBase(T aobjDataUnit, int aintIdentifierValue)
    //    {
    //        if (aobjDataUnit == null) throw new ArgumentNullException("Data unit is null");
    //        if (aintIdentifierValue <= 0) throw new ArgumentException("Identifier value should be positive integer");
    //        _iobjDataUnit = aobjDataUnit;
    //        _iintIdentifierValue = aintIdentifierValue;
    //    }
    //    public T GetDataUnit()
    //    {
    //        return _iobjDataUnit;
    //    }
    //}

    public abstract class clsBatchHandlerBase<T> : iiDataProcessor
    {
        #region busBatchHandler Code
        #region [Member Variables]

        public static CorBuilderXML iobjCorBuilder;
        public busSystemManagement ibusSystemManagement;

        public delegate void UpdateProcessLogDelegate(string astrMessage, string astrMessageType, string astrStepName);
        public UpdateProcessLogDelegate idlgUpdateProcessLog;
        public utlPassInfo iutlPassInfo { get; set; }

        public string istrProcessName;
        public int iintCount = 0;
        public int iintInternalCount = 0;
        public int iintLimit;
        public int iintMaxCount;

        protected int iintTotalRecordCount;
        private int iintSuccessRecordCount;
        private int iintFailureRecordCount;

        #endregion

        #region [Public Methods]

        /// <summary>
        /// Start Counter
        /// </summary>
        public void StartCounter()
        {
            UpdateProcessLog("Started processing.. ", busConstant.MPIPHPBatch.MESSAGE_TYPE_INFORMATION);
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
        /// Call Counter
        /// </summary>
        public void CallCounter()
        {
            if (iintInternalCount == iintLimit)
            {
                // Display this message if max count is available
                if (iintMaxCount != 0)
                    UpdateProcessLog("Currently processing record " + iintCount + " of " + iintMaxCount, busConstant.MPIPHPBatch.MESSAGE_TYPE_INFORMATION);
                else
                    UpdateProcessLog("Currently processing record " + iintCount, busConstant.MPIPHPBatch.MESSAGE_TYPE_INFORMATION);

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
            UpdateProcessLog("Ended processing.. Total records read " + iintCount, busConstant.MPIPHPBatch.MESSAGE_TYPE_INFORMATION);
        }

        /// <summary>
        /// Post Message
        /// </summary>
        /// <param name="astrMessage">Message</param>
        /// <param name="astrMessageType">Message Type</param>
        public void PostMessage(string astrMessage, string astrMessageType)
        {
            UpdateProcessLog(astrMessage, astrMessageType);
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
        public string CreateCorrespondence(string astrTemplateName, string astrUserID, int aintUserSerialID, ArrayList aarrResult, Hashtable ahtbQueryBkmarks)
        {
            utlCorresPondenceInfo lobjCorresPondenceInfo = null; // Need to call busMPIPHPBase Set correspondence method here.

            if (lobjCorresPondenceInfo == null)
            {
                throw new Exception("Unable to create correspondence, SetCorrespondence method not found in business solutions base object");
            }

            string lstrFileName = string.Empty;
            lstrFileName = iobjCorBuilder.CreateCorrespondenceFromTemplate(astrTemplateName,
                lobjCorresPondenceInfo, astrUserID);
            return lstrFileName;
        }

        /// <summary>
        /// Generate Report in the Requested Format
        /// </summary>
        /// <param name="aintReportId">Report Id</param>
        /// <param name="aenmExportFormat">Export Format</param>
        /// <param name="astrReferenceTableName">Name of the reference table.</param>
        /// <param name="aintReferenceKeyId">The reference key id.</param>
      

        /// <summary>
        /// Initialize Summary Variables
        /// </summary>
        protected virtual void InitializeSummaryVariables()
        {
            iintSuccessRecordCount = 0;
            iintFailureRecordCount = 0;
        }

        #endregion

        #region [Private Methods]

 

        /// <summary>
        /// Get DataSet for SQL Server Reports from the XML file
        /// </summary>
        /// <param name="astrFormName">Report Form Name</param>
        /// <param name="ahstReportParams">Report Params</param>
        /// <returns>DataSet</returns>
        private DataSet GetDataSetForSQLReportFromXML(string astrFormName, Hashtable ahstReportParams)
        {
            string lstrQuery = string.Empty;
            string lstrMethodName = string.Empty;


            XmlObject lxobMetaData = iutlPassInfo.isrvMetaDataCache.GetXmlObject(astrFormName);
            if (lxobMetaData == null)
            {
                throw new Exception("XML document for Report not found.");
            }

            XmlObject lxobInitialLoad = lxobMetaData.icolChildObjects.Where(i => i.istrElementName == "initialload").FirstOrDefault();
            if ((lxobInitialLoad != null) && (lxobInitialLoad.icolChildObjects.Count > 0))
            {
                XmlObject lxobMethodInfo = lxobInitialLoad.icolChildObjects[0];
                if (lxobMethodInfo.istrElementName == "query")
                {
                    lstrQuery = lxobMethodInfo.idictAttributes.GetValue("sfwQueryRef");
                }
                else if (lxobMethodInfo.istrElementName == "custommethod")
                {
                    lstrMethodName = lxobMethodInfo.idictAttributes.GetValue("sfwMethodName");
                }
            }

            // Return : Report XML document is not well-formed.
            if (string.IsNullOrEmpty(lstrQuery) && string.IsNullOrEmpty(lstrMethodName))
            {
                throw new Exception("XML document for Report is not well-formed with Query or Method.");
            }

            DataSet ldsReportData = null;

            string lstrRemoteServerName = "srvReports";
            //FM upgrade: 6.0.7.0 changes - Removed support for reading system settings through HelperFunctions
            //string lstrBusinessTierUrl = String.Format(HelperFunction.GetAppSettings("BusinessTierUrl"), lstrRemoteServerName);
            string lstrBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, lstrRemoteServerName);
            //FM upgrade changes - Remoting to WCF
            //IBusinessTier lsrvBusinessTier = (IBusinessTier)Activator.GetObject(typeof(IBusinessTier), lstrBusinessTierUrl);
            IBusinessTier lsrvBusinessTier = null;
            try
            {
                lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrBusinessTierUrl);

                // Query is specified
                if (!string.IsNullOrEmpty(lstrQuery))
                {
                    DataTable ldtReportData = lsrvBusinessTier.ExecuteQuery(lstrQuery, ahstReportParams, new Dictionary<string, object>());
                    ldsReportData = new DataSet();
                    ldsReportData.Tables.Add(ldtReportData);
                }
                else // Method is specified
                {
                    ldsReportData = (DataSet)lsrvBusinessTier.ExecuteMethod(lstrMethodName, ahstReportParams, false, new Dictionary<string, object>());
                }
            }
            finally
            {
                HelperFunction.CloseChannel(lsrvBusinessTier);
            }
            return ldsReportData;
        }


        #endregion
        #endregion

        protected void UpdateProcessLog(string astrMessage, string astrMessageType)
        {
            /*
            iutlPassInfo.BeginTransaction();
            try
            {
                // Assigns the current job schedule step name when the passed parameter step name is empty

                DBFunction.StoreProcessLog(ibusSystemManagement.icdoSystemManagement.current_cycle_no, ibusJobDetail.icdoJobDetail.step_no.ToString(), astrMessageType, astrMessage, iutlPassInfo.istrUserID,
                    iutlPassInfo.iconFramework, iutlPassInfo.itrnFramework);
                iutlPassInfo.Commit();
            }
            catch (Exception ex)
            {
                iutlPassInfo.Rollback();
            }*/
            try
            {
                idlgUpdateProcessLog.BeginInvoke(astrMessage, astrMessageType, ibusJobDetail.ibusBatchSchedule.icdoBatchSchedule.step_name, null, null);
            }
            catch
            {

            }
        }


        protected int iintCommitThreshold;
        protected int iintProgressPercentageReportThreshold;
        protected bool iblnContinueOnError;
        protected abstract Collection<T> GetDataList();
        protected abstract void OnProcessDataUnit(T aobjDataUnit);
        protected bool iblnUseTransaction;
        protected bool iblnCancelRequested;
        private busJobDetail ibusJobDetail;
        public clsBatchHandlerBase
        (
            busJobDetail abusJobDetail
            , utlPassInfo aobjPassInfo
            , busSystemManagement abusSystemManagement
            , UpdateProcessLogDelegate adlgUpdateProcessLog
            , int aintCommitThreshold
            , int aintProgressPercentageReportThreshold
            , bool ablnContinueOnError
            , bool ablnUseTransaction
        )
        {
            ibusJobDetail = abusJobDetail;

            if (ibusJobDetail.ibusJobHeader == null)
            {
                ibusJobDetail.LoadJobHeader();
            }

            if (ibusJobDetail.ibusBatchSchedule == null)
            {
                ibusJobDetail.LoadStepInfo();
            }

            idlgUpdateProcessLog = adlgUpdateProcessLog;
            iutlPassInfo = aobjPassInfo;
            iintCommitThreshold = aintCommitThreshold;
            iintProgressPercentageReportThreshold = aintProgressPercentageReportThreshold;
            iDataList = new Collection<T>();
            iblnContinueOnError = ablnContinueOnError;
            iblnUseTransaction = ablnUseTransaction;
        }
        protected Collection<T> iDataList;

        private void ProcessDataUnit(T aobjDataUnit)
        {
            OnProcessDataUnit(aobjDataUnit);
        }


        protected virtual bool OnBeforeProcessing()
        {
            return true;
        }

        protected bool HandleAfterProcessing(bool ablnExceptionCaught)
        {
            try
            {
                return (!ablnExceptionCaught && OnAfterProcessing());
            }
            catch
            {
                return false;
            }
        }
        protected virtual bool OnAfterProcessing()
        {
            return true;
        }


        private bool AfterProcessing()
        {
            iutlPassInfo.BeginTransaction();
            try
            {
                OnAfterProcessing();
                iutlPassInfo.Commit();
                return true;
            }
            catch (Exception ex)
            {
                try
                {
                    iutlPassInfo.Rollback();
                }
                catch
                {
                }
                const string _afterProcessing = "AfterProcessing: {0}";
                UpdateProcessLog(string.Format(_afterProcessing, ex.Message), busConstant.MPIPHPBatch.MESSAGE_TYPE_ERROR);
            }
            return false;
        }

        private bool BeforeProcessing()
        {
            iutlPassInfo.BeginTransaction();
            try
            {
                OnBeforeProcessing();
                iutlPassInfo.Commit();
                return true;
            }
            catch (Exception ex)
            {
                try
                {
                    iutlPassInfo.Rollback();
                }
                catch
                {
                }
                const string _beforeProcessing = "BeforeProcessing: {0}";
                UpdateProcessLog(string.Format(_beforeProcessing, ex.Message), busConstant.MPIPHPBatch.MESSAGE_TYPE_ERROR);
            }
            return false;
        }


        protected bool _iblnExceptionCaught = false;
        public bool Process()
        {
            if (OnBeforeProcessing())
            {
                try
                {
                    iDataList = GetDataList();
                }
                catch (Exception ex)
                {
                    UpdateProcessLog(ex.Message, busConstant.MPIPHPBatch.MESSAGE_TYPE_ERROR);
                    return false;
                }

                if (iDataList != null && iDataList.Count > 0)
                {
                    bool lblnCommitAfterLoop = false;
                    if (iDataList.Count <= iintCommitThreshold)
                    {
                        lblnCommitAfterLoop = true;
                    }
                    int lintCurrentDataSequence = 0;

                    foreach (T lobjDataUnit in iDataList)
                    {
                        if (PerformThresholdTaskResult(lintCurrentDataSequence) == false)
                        {
                            break;
                        }

                        DoProgressNotification(lintCurrentDataSequence);

                        try
                        {
                            ProcessDataUnit(lobjDataUnit);
                            iintSuccessRecordCount++;
                        }
                        catch (Exception ex)
                        {
                            iintFailureRecordCount++;
                            _iblnExceptionCaught = true;
                            //rollback tx
                            if (iblnUseTransaction)
                            {
                                iutlPassInfo.Rollback();
                                iutlPassInfo.BeginTransaction();
                            }
                            //log in restart table with identifier value + lintCurrentDataSequence

                            //log exception in process log table with identifier 
                            UpdateProcessLog(ex.Message, busConstant.MPIPHPBatch.MESSAGE_TYPE_ERROR);
                            HandleDataUnitException(lobjDataUnit);
                            if (iblnContinueOnError == false) break;
                        }
                        lintCurrentDataSequence++;
                    }
                    UpdateProcessLog(string.Format("Total Records: {2}, Successfully processed records: {0}, Failed Records: {1}", iintSuccessRecordCount, iintFailureRecordCount, iDataList.Count), busConstant.MPIPHPBatch.MESSAGE_TYPE_INFORMATION);
                    if (iblnUseTransaction && lblnCommitAfterLoop)
                        try
                        {
                            iutlPassInfo.Commit();
                        }
                        catch
                        {
                            //deliberately eating exception 
                        }
                    if (!iblnCancelRequested)
                    {
                        DoProgressNotification(iDataList.Count);
                    }
                }
                return HandleAfterProcessing(_iblnExceptionCaught);
            }
            else
                return iblnCancelRequested;
        }


        protected virtual void OnDataUnitProcessingException(T aobjDataUnit)
        {
        }


        private void HandleDataUnitException(T aobjDataUnit)
        {
            try
            {
                OnDataUnitProcessingException(aobjDataUnit);
            }
            catch (Exception ex)
            {
                UpdateProcessLog(ex.Message, busConstant.MPIPHPBatch.MESSAGE_TYPE_ERROR);
            }
        }


        private bool PerformThresholdTaskResult(int lintCurrentDataSequence)
        {
            bool lblPerformThresholdTaskResult = true;
            if (lintCurrentDataSequence % iintCommitThreshold == 0)
            {
                //time to commit
                //commit tx
                if (iblnUseTransaction)
                {
                    iutlPassInfo.Commit();
                }

                if (ShouldContinueDataProcessing() == false)
                {
                    ibusJobDetail.icdoJobDetail.Select();
                    ibusJobDetail.icdoJobDetail.status_value = BatchHelper.JOB_DETAIL_STATUS_CANCELLED;
                    ibusJobDetail.icdoJobDetail.end_time = DateTime.Now;
                    ibusJobDetail.icdoJobDetail.Update();
                    lblPerformThresholdTaskResult = false;
                    iblnCancelRequested = true;
                }
                //start tx
                if (iblnUseTransaction)
                {
                    iutlPassInfo.BeginTransaction();
                }
            }
            return lblPerformThresholdTaskResult;
        }


        private void DoProgressNotification(int aintCurrentDataSequence)
        {

            try
            {
                int lintPercentageProgress = (int)((float)((float)aintCurrentDataSequence / ((float)(iDataList.Count))) * 100);
                if (lintPercentageProgress > iintProgressPercentageReportThreshold)
                {
                    //begin tx
                    ibusJobDetail.icdoJobDetail.Select();
                    ibusJobDetail.icdoJobDetail.progress_percentage = lintPercentageProgress;
                    ibusJobDetail.icdoJobDetail.Update();
                    //commit tx

                }
            }
            catch
            {
            }
        }
        private bool ShouldContinueDataProcessing()
        {
            bool lblnResult = false;
            busJobHeader lbusJobHeader = new busJobHeader() { icdoJobHeader = new cdoJobHeader() };
            if (lbusJobHeader.FindJobHeader(ibusJobDetail.ibusJobHeader.icdoJobHeader.job_header_id))
            {
                lblnResult = (BatchHelper.JOB_HEADER_STATUS_CANCEL_REQUESTED.ToLower() != lbusJobHeader.icdoJobHeader.status_value.ToLower());
            }
            else
            {
                //return false if job header is not found!!
                lblnResult = false;
            }
            return lblnResult;
        }
    }

    //public abstract class StepHandlerFactory
    //{
    //    public static iiDataProcessor GetDataProcessorForStepNo(int aintStepNumber, busJobHeader o)
    //    {
    //        if (o == null) throw new ArgumentNullException("busJobHeader");

    //        iiDataProcessor lobjDataProcessor = null;
    //        switch (aintStepNumber)
    //        {
    //            case 1:
    //                lobjDataProcessor = new SampleBatchHandler(2, 2, o, null);
    //                break;
    //            default:
    //                throw new NotImplementedException("No batch handler supported for step number: " + aintStepNumber.ToString());

    //        }

    //        return lobjDataProcessor;
    //    }
    //}

    //public class SampleDataUnit : clsDataUnitBase<int>
    //{
    //    public SampleDataUnit(int i, int k)
    //        : base(i, k)
    //    {

    //    }
    //}


}
