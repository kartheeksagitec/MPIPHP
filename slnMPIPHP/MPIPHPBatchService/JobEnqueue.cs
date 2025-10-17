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

namespace MPIPHP.MPIPHPJobService
{
    /// <summary>
    /// Summary description for JobEnqueue.
    /// </summary>
    public class JobEnqueue : Worker
    {
        private bool _stop = false;
        private bool _pause = false;

        private JobEnqueue()
            : base("[JOB_ENQUEUE]")
        { }

        internal static JobEnqueue GetInstance()
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
                        lock (Common.SCHEDULE)
                        {
                            // If there is any record in the SCHEDULE queue, we need to process it, 
                            // if there is no record in the SCHEDULE queue, we need to wait till some record is put in there
                            if (!_pause && !_stop)
                            {
                                while (Common.SCHEDULE.Count == 0)
                                    Monitor.Wait(Common.SCHEDULE);
                            }
                            else
                            {
                                continue;
                            }
                        } // lock (SCHEDULE)

                        int lintJobScheduleId;
                        // There is an item available in the SCHEDULE queue,
                        // Dequeue the schedule id from the SCHEDULE queue and process it.
                        lock (Common.SCHEDULE)
                        {
                            lintJobScheduleId = Common.SCHEDULE.Dequeue();
                            Monitor.Pulse(Common.SCHEDULE);
                        } // lock (SCHEDULE)

                        TraceLine("Picked up a job from the internal schedule queue.", TraceLevel.Info);

                        busJobSchedule lobjJobSchedule = new busJobSchedule();
                        lobjJobSchedule.FindJobSchedule(lintJobScheduleId);
                        lobjJobSchedule.LoadJobScheduleDetails(true);

					    BeginTransaction();
                        try
                        {
                            lobjJobSchedule.CloneJobFromSchedule();
                            if(lobjJobSchedule.icdoJobSchedule.status_value == "ONDE" || lobjJobSchedule.icdoJobSchedule.status_value == "ADHC")
                                lobjJobSchedule.icdoJobSchedule.active_flag = busConstant.FLAG_NO;
                            lobjJobSchedule.icdoJobSchedule.Update();
                            CommitTransaction();
                        }
                        catch(Exception ex)
                        {
                            RollbackTransaction();
                            TraceLine(ex.Message, TraceLevel.Error);
                        }

                        TraceLine("Checking for a job in the internal schedule queue.", TraceLevel.Info);
                      
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

            internal static readonly JobEnqueue instance = new JobEnqueue();

        }
    }
}
