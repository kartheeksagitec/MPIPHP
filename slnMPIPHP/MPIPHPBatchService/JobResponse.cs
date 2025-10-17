using System;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using MPIPHP.CustomDataObjects;
using MPIPHP.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.DataObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;
using System.Collections;
using MPIPHP.Common;

namespace MPIPHP.MPIPHPJobService
{
    /// <summary>
    /// Summary description for JobResponse.
    /// </summary>
    public class JobResponse : Worker
    {
        private bool _stop = false;
        private bool _pause = false;

        private JobResponse()
            : base("[JOB_RESPONSE]")
        { }

        internal static JobResponse GetInstance()
        {
            return Nested.instance;
        }

        public override void Start()
        {
            TraceLine("Enter Start()", TraceLevel.Info);
            try
            {
                TraceLine("Initializing...", TraceLevel.Info);
                Initialize();
                TraceLine("Initialized...", TraceLevel.Info);

                if (WorkerThread == null)
                    throw new InvalidOperationException("Worker Has Not Been created...");

                _stop = false;
                _pause = false;

                WorkerThread.Start();
                TraceLine("Internal Thread Started.", TraceLevel.Info);
            }
            catch (Exception ex)
            {
                TraceLine("Non-CLR Exception Caught." + ex.Message, TraceLevel.Error);
                throw;
            }
            finally
            {
                TraceLine("Exit Start()", TraceLevel.Info);
            }
        }

        public override void Pause()
        {
            TraceIn();
            TraceLine("Enter Pause()", TraceLevel.Debug);

            _pause = true;

            TraceLine("Exit Pause()", TraceLevel.Debug);
            TraceOut();
        }

        public override void Resume()
        {
            TraceIn();
            TraceLine("Enter Resume()", TraceLevel.Debug);

            _pause = false;

            TraceLine("Exit Resume()", TraceLevel.Debug);
            TraceOut();
        }

        public override void Stop()
        {
            TraceIn();
            TraceLine("Enter Stop()", TraceLevel.Debug);

            FireShutdownStarted(this, EventArgs.Empty);
            _stop = true;
            WorkerThread.Interrupt();

            FireShutdownCompleted(this, EventArgs.Empty);
            TraceLine("Exit Stop()", TraceLevel.Debug);
            TraceOut();
        }

        protected override void Run()
        {
            TraceIn();
            TraceLine("Enter Run()", TraceLevel.Debug);

            StartTime = DateTime.Now;

            // Call the base class method to setup the UtlpassInfo which will be used later for executing any CRUD operations by the framework.
            SetUtlPassInfo();

            TraceLine("Started at: " + StartTime, TraceLevel.Info);

            // notify work has started
            FireWorkStarted(this, EventArgs.Empty);

            try
            {
                while (!_stop)
                {
                    while (!_pause && !_stop)
                    {
                        TraceLine("Checking for a job in the response queue.", TraceLevel.Info);
                        lock (Common.RESPONSE)
                        {
                            // If there is any record in the Response queue, we need to process it, 
                            // if there is no record in the response queue, we need to wait till some record is put in there
                            if (!_pause && !_stop)
                            {
                                while (Common.RESPONSE.Count == 0)
                                    Monitor.Wait(Common.RESPONSE);
                            }
                            else
                            {
                                continue;
                            }
                        } // lock (RESPONSE)

                        // There is an item available in the response queue,
                        // Dequeue the job from the response queue and process it.
                        busJobHeader lobjJobHeader = null;
                        lock (Common.RESPONSE)
                        {
                            lobjJobHeader = Common.RESPONSE.Dequeue();
                            Monitor.Pulse(Common.RESPONSE);
                        } // lock (RESPONSE)

                        TraceLine("Dequeued Job in the response queue.", TraceLevel.Info);

					    BeginTransaction();
                        try
                        {
                            // Check the status of individual detail steps inside the job header
                            // if all of them have not completed successfully, then we have to set the header statues
                            // to completed with errors
                            bool lblnCompletedSuccessfully = true;
                            
                            bool lblnUserCancelledJob = false;
                            foreach(busJobDetail lobjJobDetail in lobjJobHeader.iclbJobDetail)
                            {
                                if (lobjJobDetail.icdoJobDetail.status_value != BatchHelper.JOB_DETAIL_STATUS_PROCESSED_SUCCESSFULLY)
                                {
                                    if (lobjJobDetail.icdoJobDetail.status_value == BatchHelper.JOB_DETAIL_STATUS_CANCELLED)
                                    {
                                        lblnUserCancelledJob = true;
                                    }
                                    lblnCompletedSuccessfully = false;
                                    break;
                                }
                            }
                            lobjJobHeader.FindJobHeader(lobjJobHeader.icdoJobHeader.job_header_id);
                            if (lblnCompletedSuccessfully)
                            {
                                lobjJobHeader.icdoJobHeader.status_value = BatchHelper.JOB_HEADER_STATUS_PROCESSED_SUCCESSFULLY;
                               // busCommunication.SendBatchCompletionEMails(lobjJobHeader.icdoJobHeader.job_schedule_id);
                            }
                            else
                            {
                                if (lblnUserCancelledJob == false)
                                {
                                    lobjJobHeader.icdoJobHeader.status_value = BatchHelper.JOB_HEADER_STATUS_PROCESSED_WITH_ERRORS;
                                    //busCommunication.SendBatchErrorEMails(lobjJobHeader.icdoJobHeader.job_schedule_id);
                                }
                                else
                                {
                                    lobjJobHeader.icdoJobHeader.status_value = BatchHelper.JOB_HEADER_STATUS_CANCELLED;
                                 //   busCommunication.SendBatchUserCancellationEMails(lobjJobHeader.icdoJobHeader.job_schedule_id);
                                }
                            }
                            lobjJobHeader.icdoJobHeader.end_time = DateTime.Now;
                            lobjJobHeader.icdoJobHeader.Update();
                            CommitTransaction();
                        }
                        catch(Exception ex)
                        {
                            RollbackTransaction();
                            TraceLine(ex.Message, TraceLevel.Error);
                        }
                        TraceLine("Processed Job in the response queue.", TraceLevel.Info);
                      
                        // give time back to the CPU
                        Thread.Sleep(Common.THREADSLEEPTIME);

                    } // while !_pause

                    // give time back to the CPU
                    Thread.Sleep(Common.THREADSLEEPTIME);

                } // while !_stop

            } // try
            catch (ThreadInterruptedException ex)
            {
                // someone called Stop() and we were interrupted.
                TraceLine("Exception Caught (expected): \n" + ex.ToString(), TraceLevel.Error);
            }
            catch (Exception ex)
            {
                TraceLine("Exception Caught: \n" + ex.ToString(), TraceLevel.Error);
                LogError(ex.ToString(), "Sagitec.Jobs.Service", "EXE", 0);
                throw;
            }
            finally
            {
                CompleteTime = DateTime.Now;
                TraceLine("Stopped at: " + CompleteTime + ".", TraceLevel.Info);

                // notify work has completed
                FireWorkCompleted(this, EventArgs.Empty);

                TraceLine("Exit Run()", TraceLevel.Debug);
                TraceOut();
            }
        } // Run

        protected override void Initialize()
        {
            TraceIn();
            TraceLine("Enter Initialize()", TraceLevel.Debug);
            FireInitializeStarted(this, EventArgs.Empty);

            // TODO Whenever the job service comes back up, we need to add all those entries in the databse
            // that are in Processing/Picked up status, so they are not lost due to improper shutdowns.

            FireInitializeCompleted(this, EventArgs.Empty);

            TraceLine("Exit Initialize()", TraceLevel.Debug);
            TraceOut();
        }

        protected override void Shutdown()
        {
            TraceIn();
            TraceLine("Enter Shutdown()", TraceLevel.Debug);

            FireShutdownStarted(this, EventArgs.Empty);
            FireShutdownCompleted(this, EventArgs.Empty);

            TraceLine("Exit Shutdown()", TraceLevel.Debug);
            TraceOut();
        }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            { }

            internal static readonly JobResponse instance = new JobResponse();

        }
    }
}
