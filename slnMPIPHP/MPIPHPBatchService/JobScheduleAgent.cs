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
using Sagitec.ExceptionPub;

namespace MPIPHP.MPIPHPJobService
{
    /// <summary>
    /// Summary description for Job Schedule Agent.
    /// </summary>
    public class JobScheduleAgent : Worker
    {
        private bool _stop = false;
        private bool _pause = false;

        private JobScheduleAgent()
            : base("[JOB_SCHEDULE_AGENT]")
        { }

        internal static JobScheduleAgent GetInstance()
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
            int lintCurrentCycleNumber = 0;
            // Call the base class method to setup the UtlpassInfo which will be used later for executing any CRUD operations by the framework.
            SetUtlPassInfo();
            TraceLine("Started at: " + StartTime, TraceLevel.Info);

            // notify work has started
            FireWorkStarted(this, EventArgs.Empty);

            // Sleeptime should be exactly 60000 for the schedule agent, since it has to wake up exactly every 1 second and process all the scheduled jobs.
            int lintSleepTime = 70000;
            try
            {
                while (!_stop)
                {
                    while (!_pause && !_stop)
                    {

                        try
                        {
                            DateTime ldtnow;
                            busSystemManagement lbusSystemManagement = new busSystemManagement();
                            DateTime now = DateTime.Now;
                            BeginTransaction();
                            if (lbusSystemManagement.FindSystemManagement())
                            {
                                bool lblnBatchDateTimeEmpty = false;
                                if (!DateTime.TryParse(lbusSystemManagement.icdoSystemManagement.batch_date.ToString(), out ldtnow))
                                {
                                    ldtnow = DateTime.Now;
                                    lblnBatchDateTimeEmpty = true;
                                }
                                else
                                {
                                    if (ldtnow == DateTime.MinValue)
                                    {
                                        ldtnow = DateTime.Now;
                                        lblnBatchDateTimeEmpty = true;
                                    }
                                    else
                                    {
                                        ldtnow = new DateTime(ldtnow.Year, ldtnow.Month, ldtnow.Day, now.Hour, now.Minute, now.Second);
                                    }
                                }
                                
                                //-----------------------------------------------------------------------------------------------------------------------
                                //overriding batch date for now, since the batch date doesnt change automatically as of now. Need to
                                //decide when the batch date changes. Possible solution to create the batch job with priority P1 to change batch date.
                                //once Job service to change batch date is created and tested that it executes correctly, comment line below
                                ldtnow = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
                                //-----------------------------------------------------------------------------------------------------------------------


                                lintCurrentCycleNumber = lbusSystemManagement.icdoSystemManagement.current_cycle_no;
                                cdoCycleHistory lobjCycleHistory = new cdoCycleHistory();
                                lobjCycleHistory = new cdoCycleHistory();
                                lobjCycleHistory.cycle_no = ++lintCurrentCycleNumber;
                                lobjCycleHistory.start_time = DateTime.Now;
                                lobjCycleHistory.end_time = DateTime.MaxValue;
                                lobjCycleHistory.Insert();

                                //Fire the query to populate the beginning of cycle data
                                if (lblnBatchDateTimeEmpty == false)
                                {
                                    lbusSystemManagement.icdoSystemManagement.batch_date = DateTime.Now.Date;
                                }
                                lbusSystemManagement.icdoSystemManagement.current_cycle_no = lintCurrentCycleNumber;
                                lbusSystemManagement.icdoSystemManagement.Update();

                                lbusSystemManagement = null;
                            }
                            else
                            {
                                //exception as system management should never be unavailable
                                ExceptionManager.Publish(new Exception("System management was not loaded, using current server date"));
                                ldtnow = DateTime.Now;
                            }
                            CommitTransaction();
                            DateTime ldtcurrent = new DateTime(ldtnow.Year, ldtnow.Month, ldtnow.Day, ldtnow.Hour, ldtnow.Minute, 0);
                            TraceLine("Current Time for schedule check: " + ldtcurrent.ToString(), TraceLevel.Debug);
                            // TODO Get the schedules from the database only once in every 5 or 10 minutes, configurable value..
                            // This call should not be done to the database every 1 minute..
                            Collection<busJobScheduleChecker> lclbJobScheduleChecker = busJobScheduleChecker.GetActiveScheduledJobs();
                            if (lclbJobScheduleChecker != null && lclbJobScheduleChecker.Count > 0)
                            {
                                TraceLine("Jobs present in schedule, checking to see if anything has to run now.", TraceLevel.Info);
                                foreach (busJobScheduleChecker lobjJobScheduleChecker in lclbJobScheduleChecker)
                                {
                                    if (lobjJobScheduleChecker.CanRun(ldtcurrent))
                                    {
                                        TraceLine("This schedule can run, enqueing the new jobs into the internal queue", TraceLevel.Info);
                                        lock (Common.SCHEDULE)
                                        {
                                            TraceLine("Adding the job to internal schedule queue: " + lobjJobScheduleChecker.ToString(), TraceLevel.Debug);
                                            Common.SCHEDULE.Enqueue(lobjJobScheduleChecker.icdoJobSchedule.job_schedule_id);
                                            Monitor.Pulse(Common.SCHEDULE);
                                        }
                                    }
                                }
                            }
                        }
                        catch (ThreadInterruptedException t)
                        {
                            RollbackTransaction();
                            LogError(t.ToString(), "NeoSpinJobService", "EXE", 0);
                            TraceLine("An exception was caught while fetching Job Schedule",
                                TraceLevel.Error);
                            continue;
                        }
                        catch (Exception ex)
                        {
                            RollbackTransaction();
                            // don't let any exceptions stop the schedule agent thread
                            LogError(ex.ToString(), "NeoSpinJobService", "EXE", 0);
                            TraceLine("An exception was caught while fetching Job Schedule", TraceLevel.Error);
                            TraceLine("Sleeping for [60] seconds.", TraceLevel.Error);
                            //Thread.Sleep(Common.THREADSLEEPTIME);
                            continue;
                        }
                        // give time back to the CPU
                        Thread.Sleep(lintSleepTime);

                    } // while !_pause

                    // give time back to the CPU
                    Thread.Sleep(lintSleepTime);

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

            internal static readonly JobScheduleAgent instance = new JobScheduleAgent();

        }
    }
}
