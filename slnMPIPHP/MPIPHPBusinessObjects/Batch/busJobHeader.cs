#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;
using System.Linq;
using System.Linq.Expressions;
using Sagitec.CustomDataObjects;
using MPIPHP.Common;
using MPIPHP.DataObjects;
using Sagitec.DataObjects;
#endregion

namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public class busJobHeader : busJobHeaderGen
    {
        #region Public Variables
        public Collection<busJobDetail> iclbJobDetail { get; set; }
        public Queue<int> iqueJobDetailSteps { get; set; }
        // This object stores the current job that is being processed by the Job Service
        public busJobDetail ibusCurrentJobDetail { get; set; }

        // This object is used only for the purpose of lookup screen
        public busJobDetail ibusLookupJobDetail { get; set; }
        #endregion

        #region Public Methods

        public string BatchUserID
        {
            get
            {
                return string.Format(busConstant.MPIPHPBatch.BATCH_USER_ID_FORMAT, icdoJobHeader.job_schedule_id.ToString(), icdoJobHeader.job_header_id.ToString());
            }
        }
        public void LoadJobDetail()
        {
            LoadJobDetail(true);
        }
        public void LoadProcessLogs()
        {
            DataTable ldtbProcessLogs = Select("cdoJobHeader.GetProcessLogs",
                                        new object[1] { this.BatchUserID });
            
            if (ldtbProcessLogs.Rows.Count > 0)
            {
                this.iclcProcessLog = doBase.GetCollection<cdoProcessLog>(ldtbProcessLogs);
            }
        }
        /// <summary>
        /// This method will load the all the Job Detail records for the appropriate Header Id
        /// and will also load the Parameters for the appropriate detail record if the parameter is passed as true.
        /// </summary>
        /// <param name="ablnLoadWithParameters"></param>
        public void LoadJobDetail(bool ablnLoadWithParameters)
        {
            LoadJobDetail(ablnLoadWithParameters, false);
        }

        /// <summary>
        /// This method will load the all the Job Detail records for the appropriate Header Id
        /// and will also load the Parameters for the appropriate detail record if the parameter is passed as true.
        /// </summary>
        /// <param name="ablnLoadWithParameters"></param>
        /// <param name="ablnLoadJobSteps"></param>
        public void LoadJobDetail(bool ablnLoadWithParameters, bool ablnLoadJobSteps)
        {
            // Get all the Job detail records from the SGS_Job_Detail table

            DataTable ldtbJobDetails = Select<cdoJobDetail>(
                new string[1] { enmJobDetail.job_header_id.ToString() },
                new object[1] { icdoJobHeader.job_header_id }, null, null);

            if (ldtbJobDetails.Rows.Count > 0)
            {
                iclbJobDetail = GetCollection<busJobDetail>(ldtbJobDetails, "icdoJobDetail");

                // If the request is to load the parameters as well, go ahead and load the same.
                if (ablnLoadWithParameters)
                {
                    // Load all the parameters corresponding to the particular Job Detail as well.
                    foreach (busJobDetail lobjJobDetail in iclbJobDetail)
                    {
                        lobjJobDetail.LoadJobParameters();
                    }
                }

                // If the request is to load the JobDetailSteps as well, go ahead and load the same.
                if (ablnLoadJobSteps)
                {
                    PopulateQueueBasedOnOrderNumber();
                }
            }
        }

        /// <summary>
        /// This method will load the step information for all the detail records
        /// as mentioned in the SGS_Batch_Schedule table.
        /// </summary>
        public void LoadJobDetailStepInfo()
        {
            if (iclbJobDetail != null && iclbJobDetail.Count > 0)
            {
                // Load all the parameters corresponding to the particular Job Detail as well.
                foreach (busJobDetail lobjJobDetail in iclbJobDetail)
                {
                    lobjJobDetail.LoadStepInfo();
                }
            }
        }

        /// <summary>
        /// we have to get the next step to be performed based on the order_number of the step
        /// as mentioned in the SGS_Batch_Schedule table.
        /// </summary>
        public void GetNextStep()
        {
            // default the currentjob object to null and populate it only when there is a particular
            // step to be performed.
            ibusCurrentJobDetail = null;
            // we have steps to perform, now go ahead and get the first step to be performed from the
            // queue and set the appropriate jobdetail in the ibusCurrentJobDetail object.
            if (iqueJobDetailSteps != null && iqueJobDetailSteps.Count > 0)
            {
                int lintCurrentStepNumber = iqueJobDetailSteps.Dequeue();

                // Now try to find the corresponding job detail object from the iclbJobDetail collection.
                // Prem Trying the LINQ query .. Hope it works correctly in this situation... 
                ibusCurrentJobDetail = (from jobdetail in iclbJobDetail
                                        where
                                            jobdetail.icdoJobDetail.step_no == lintCurrentStepNumber
                                        select jobdetail).FirstOrDefault();

                // Load the step information as well, since the factory will use this information to create the correct handler for the step
                ibusCurrentJobDetail.LoadStepInfo();

                // Business rule check to see whether the dependent job has completed successfully before this step
                // and also has the return value that we are expecting
                if (ibusCurrentJobDetail.icdoJobDetail.dependent_step_no > 0)
                {
                    // If a dependent step has been mentioned then we have to see if that step has completed successfully
                    int lintDependentStepNumber = ibusCurrentJobDetail.icdoJobDetail.dependent_step_no;
                    // Get a reference to the dependent job and then inspect to see whether that job ran successfully and returned the appropriate return code.
                    busJobDetail lbusDependentJobDetail = (from jobdetail in iclbJobDetail
                                                           where
                                                               jobdetail.icdoJobDetail.step_no ==
                                                               lintDependentStepNumber
                                                           select jobdetail).FirstOrDefault();

                    bool lblnDependentJobProcessedSuccessfully = true;

                    if (lbusDependentJobDetail == null)
                    {
                        lblnDependentJobProcessedSuccessfully = false;
                    }
                    else if (lbusDependentJobDetail.icdoJobDetail.status_value != BatchHelper.JOB_DETAIL_STATUS_PROCESSED_SUCCESSFULLY)
                    {
                        lblnDependentJobProcessedSuccessfully = false;
                    }
                    else
                    {

                        cdoCodeValue lobjcdoCodeValue =
                            HelperUtil.GetCodeValueDetails(ibusCurrentJobDetail.icdoJobDetail.operator_id,
                                                           ibusCurrentJobDetail.icdoJobDetail.operator_value);

                        utlOperator lenmOperator = (utlOperator)Enum.Parse(typeof(utlOperator), lobjcdoCodeValue.data1, true);
                        if (!HelperFunction.EvaluateValuesBasedOnOperator(lbusDependentJobDetail.icdoJobDetail.return_code,
                                                                     Convert.ToInt32(ibusCurrentJobDetail.icdoJobDetail.dependent_step_return_value), lenmOperator))
                        {
                            lblnDependentJobProcessedSuccessfully = false;
                        }
                    }

                    // If the dependent job did not process successfully then we have to skip processing all the other jobs.
                    if (!lblnDependentJobProcessedSuccessfully)
                    {
                        SkipProcessingAllOtherJobDetails(ibusCurrentJobDetail.icdoJobDetail.order_number);
                        ibusCurrentJobDetail = null;
                    }
                }
            }
        }

        private void SkipProcessingAllOtherJobDetails(int aintOrderNumber)
        {
            // Get a reference to all other job detail that are yet to be processed which are greater than or equal to the order number passed
            // in as argument
            IEnumerable<busJobDetail> lclbJobDetailsToSkip = from jobdetail in iclbJobDetail
                                                             where jobdetail.icdoJobDetail.order_number >= aintOrderNumber
                                                             select jobdetail;
            foreach (busJobDetail lobjJobDetail in lclbJobDetailsToSkip)
            {
                lobjJobDetail.icdoJobDetail.status_value = BatchHelper.JOB_DETAIL_STATUS_SKIPPED;
                lobjJobDetail.icdoJobDetail.start_time = DateTime.Now;
                lobjJobDetail.icdoJobDetail.end_time = DateTime.Now;
                lobjJobDetail.icdoJobDetail.Update();
            }
        }

        private void PopulateQueueBasedOnOrderNumber()
        {
            // Get all the step information from the iclbJobDetail collection and add it to the queue to enforce
            // the order in which the steps will be performed.
            if (iqueJobDetailSteps == null)
            {
                var lclbJobDetail =
                    from lobjJobDetail in iclbJobDetail
                    orderby lobjJobDetail.icdoJobDetail.order_number
                    select lobjJobDetail;

                if (lclbJobDetail.Count() > 0)
                {
                    iqueJobDetailSteps = new Queue<int>();
                    foreach (busJobDetail lobjJobDetail in lclbJobDetail)
                    {
                        iqueJobDetailSteps.Enqueue(Convert.ToInt32(lobjJobDetail.icdoJobDetail.step_no));
                    }
                }
            }
        }

        // Validation Methods
        public bool IsDuplicateStepsPresent()
        {
            DataTable ldtbJobDetails = Select("cdoJobDetail.CountOfStepsThatAreDuplicate",
                                                  new object[1] { icdoJobHeader.job_header_id });

            if (ldtbJobDetails.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }

        public void btnCancelJob_Click()
        {
            busJobHeader lbusJobHeader = new busJobHeader();
            if (lbusJobHeader.FindJobHeader(icdoJobHeader.job_header_id))
            {
                lbusJobHeader.icdoJobHeader.status_value = BatchHelper.JOB_DETAIL_STATUS_CANCEL_REQUESTED;
                lbusJobHeader.icdoJobHeader.Update();
            }
            this.icdoJobHeader.Reset();
        }

        // button methods
        public void btnApprove_Click()
        {
            icdoJobHeader.status_value = BatchHelper.JOB_HEADER_STATUS_QUEUED;
            icdoJobHeader.Update();
        }

        // button methods
        public void btnSubmitForApproval_Click()
        {
            icdoJobHeader.status_value = BatchHelper.JOB_HEADER_STATUS_SUBMIT_FOR_APPROVAL;
            icdoJobHeader.Update();
        }

        // Helper methods
        public static busJobHeader GetNextJob()
        {
            // Execute the query to find out if there is any job in the table that has been queued
            // For the first phase of Job Service , we are going to run only 1 job simultaneously, so enfore the logic
            // using sql query, we don't get a queued job if there already exists a job that is currently 
            // being processed.
            DataTable ldtbJobs = Select("cdoJobHeader.GetNextJob",
                                        new object[0] { });
            busJobHeader lobjJobHeader = null;
            if (ldtbJobs.Rows.Count > 0)
            {
                lobjJobHeader = new busJobHeader();
                lobjJobHeader.icdoJobHeader = new cdoJobHeader();
                lobjJobHeader.icdoJobHeader.LoadData(ldtbJobs.Rows[0]);
                lobjJobHeader.LoadJobDetail(true, true);
            }

            return lobjJobHeader;
        }
        #endregion
    }
}
