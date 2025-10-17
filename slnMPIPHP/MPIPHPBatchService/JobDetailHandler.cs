using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.IO;
using System.Xml;
using MPIPHP.BusinessObjects;
using MPIPHP.Common;

namespace MPIPHP.MPIPHPJobService
{
    /// <summary>
    /// Base class from which all Job Handlers inherit. Implements the Worker interface.
    /// </summary>
    public class JobDetailHandler : Worker
    {
        public readonly busJobHeader lobjJobHeader;



        /// <summary>
        /// Sets the internal job.
        /// </summary>
        /// <param name="aobjJobHeader">The job the Handler will work with.</param>
        /// <param name="astrWorkerName">The name of the worker and the trace file.</param>
        protected JobDetailHandler(busJobHeader aobjJobHeader, string astrWorkerName)
            : base(astrWorkerName)
        {
            lobjJobHeader = aobjJobHeader;
        }

        // methods

        public override void Pause()
        { return; }
        public override void Resume()
        { return; }
        public override void Stop()
        { return; }

        protected override void Run()
        {
            StartTime = DateTime.Now;
            FireWorkStarted(this, EventArgs.Empty);
            Random rnd = new Random();
            Thread.Sleep(rnd.Next(20) * 1000);
            CompleteTime = DateTime.Now;
            FireWorkCompleted(this, EventArgs.Empty);
        } // Run


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Release managed resources.
            }
            // Release unmanaged resources.
            // Set large fields to null.
            // Call Dispose on your base class.
            base.Dispose(disposing);
        }

        protected virtual void StartJob()
        {
            TraceIn();
            TraceLine("Enter StartJob()", TraceLevel.Debug);

            BeginTransaction();
            // Set the status of the current job to Processing
            try
            {
                lobjJobHeader.ibusCurrentJobDetail.icdoJobDetail.Select();
                lobjJobHeader.ibusCurrentJobDetail.icdoJobDetail.status_value = BatchHelper.JOB_DETAIL_STATUS_PROCESSING;
                lobjJobHeader.ibusCurrentJobDetail.icdoJobDetail.start_time = DateTime.Now;
                lobjJobHeader.ibusCurrentJobDetail.icdoJobDetail.Update();
                CommitTransaction();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }

            iobjPassInfo.istrUserID = lobjJobHeader.BatchUserID;

            TraceLine("Exit StartJob()", TraceLevel.Debug);
            TraceOut();
        } // StartJob

        protected virtual void CompleteJobWithErrors(int aintReturnCode)
        {
            TraceIn();
            TraceLine("Enter CompleteJobWithErrors()", TraceLevel.Debug);


            BeginTransaction();
            // Set the status of the current job to Processed with Errors
            try
            {
                lobjJobHeader.ibusCurrentJobDetail.icdoJobDetail.Select();
                lobjJobHeader.ibusCurrentJobDetail.icdoJobDetail.return_code = aintReturnCode;
                lobjJobHeader.ibusCurrentJobDetail.icdoJobDetail.status_value = BatchHelper.JOB_DETAIL_STATUS_PROCESSED_WITH_ERRORS;
                lobjJobHeader.ibusCurrentJobDetail.icdoJobDetail.end_time = DateTime.Now;

                lobjJobHeader.ibusCurrentJobDetail.icdoJobDetail.Update();
                CommitTransaction();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            TraceLine("Exit CompleteJobWithErrors()", TraceLevel.Debug);
            TraceOut();
        } // CompleteJobWithErrors

        protected virtual void CompleteJobForUserCancellation(int aintReturnCode)
        {
            TraceIn();
            TraceLine("Enter CompleteJobForUserCancellation()", TraceLevel.Debug);


            BeginTransaction();
            // Set the status of the current job to Processed with Errors
            try
            {
                lobjJobHeader.ibusCurrentJobDetail.icdoJobDetail.Select();
                lobjJobHeader.ibusCurrentJobDetail.icdoJobDetail.return_code = aintReturnCode;
                lobjJobHeader.ibusCurrentJobDetail.icdoJobDetail.status_value = BatchHelper.JOB_DETAIL_STATUS_CANCELLED;
                lobjJobHeader.ibusCurrentJobDetail.icdoJobDetail.end_time = DateTime.Now;

                lobjJobHeader.ibusCurrentJobDetail.icdoJobDetail.Update();
                CommitTransaction();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            TraceLine("Exit CompleteJobWithErrors()", TraceLevel.Debug);
            TraceOut();
        } // CompleteJobForUserCancellation

        protected virtual void CompleteJobSuccessfully(int aintReturnCode)
        {
            TraceIn();
            TraceLine("Enter CompleteJobSuccessfully()", TraceLevel.Debug);

            BeginTransaction();
            // Set the status of the current job to Processed successfully
            try
            {
                lobjJobHeader.ibusCurrentJobDetail.icdoJobDetail.Select();
                lobjJobHeader.ibusCurrentJobDetail.icdoJobDetail.return_code = aintReturnCode;
                lobjJobHeader.ibusCurrentJobDetail.icdoJobDetail.status_value = BatchHelper.JOB_DETAIL_STATUS_PROCESSED_SUCCESSFULLY;
                lobjJobHeader.ibusCurrentJobDetail.icdoJobDetail.end_time = DateTime.Now;

                lobjJobHeader.ibusCurrentJobDetail.icdoJobDetail.Update();

                CommitTransaction();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }

            TraceLine("Exit CompleteJobSuccessfully()", TraceLevel.Debug);
            TraceOut();
        } // CompleteJobSuccessfully

        protected void BeginTransactionByCheckingFlag()
        {
            // We are overriding the behaviour of the base class since we want to start or not start a transaction
            // based on the Requires_Transaction_Flag in the sgs_batch_schedule for the current step.
            if (lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule.requires_transaction_flag == busConstant.Flag_Yes)
            {
                BeginTransaction();
            }
        }

        protected void CommitTransactionByCheckingFlag()
        {
            try
            {
                // We are overriding the behaviour of the base class since we want to start or not start a transaction
                // based on the Requires_Transaction_Flag in the sgs_batch_schedule for the current step.
                if (lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule.requires_transaction_flag == busConstant.Flag_Yes)
                {
                    CommitTransaction();
                }
            }
            catch
            {

            }
        }

        protected void RollbackTransactionByCheckingFlag()
        {
            try
            {
                // We are overriding the behaviour of the base class since we want to start or not start a transaction
                // based on the Requires_Transaction_Flag in the sgs_batch_schedule for the current step.
                if (lobjJobHeader.ibusCurrentJobDetail.ibusBatchSchedule.icdoBatchSchedule.requires_transaction_flag == busConstant.Flag_Yes)
                {
                    RollbackTransaction();
                }
            }
            catch
            {

            }
        }

    }
}
