using System;
using System.Collections;
using System.Collections.Specialized;
using System.Threading;
using MPIPHP.BusinessObjects;
using Sagitec.BusinessObjects;
using MPIPHP.Common;

namespace MPIPHP.MPIPHPJobService
{
    /// <summary>
    /// The Master Worker class is implemented as a Singleton Pattern.
    /// </summary>
    public class JobMaster : Worker
    {
        private bool _stop = false;
        private bool _pause = false;

        private JobReceiver JobReceiver { get; set; }
        private JobResponse JobResponse { get; set; }
        private JobScheduleAgent JobScheduleAgent { get; set; }
        private JobEnqueue JobEnqueue { get; set; }

        private JobMaster()
            : base("[JOB_MASTER]")
        {
            JobReceiver = (JobReceiver)WorkerFactory.GetWorker(WorkerType.Receiver);
            JobResponse = (JobResponse)WorkerFactory.GetWorker(WorkerType.Response);
            JobScheduleAgent = (JobScheduleAgent)WorkerFactory.GetWorker(WorkerType.ScheduleAgent);
            JobEnqueue = (JobEnqueue)WorkerFactory.GetWorker(WorkerType.Enqueue);
        }

        internal static JobMaster GetInstance()
        {
            return Nested.instance;
        }

        /// <summary>
        /// Starts the JobMaster. Upon starting, JobMaster goes through an internal initialization
        /// and then starts the QueueMaster.
        /// </summary>
        public override void Start()
        {
            TraceLine("Enter Start()", TraceLevel.Debug);

            try
            {
                TraceLine("Initializing...", TraceLevel.Info);
                Initialize();
                TraceLine("Initialized...", TraceLevel.Info);


                TraceLine("Starting JobReceiver...", TraceLevel.Info);
                JobReceiver.Start();
                TraceLine("Started JobReceiver...", TraceLevel.Info);

                TraceLine("Starting JobResponse...", TraceLevel.Info);
                JobResponse.Start();
                TraceLine("Started JobResponse...", TraceLevel.Info);

                TraceLine("Starting JobScheduler...", TraceLevel.Info);
                JobScheduleAgent.Start();
                TraceLine("Started JobScheduler...", TraceLevel.Info);

                TraceLine("Starting JobEnqueue...", TraceLevel.Info);
                JobEnqueue.Start();
                TraceLine("Started JobEnqueue...", TraceLevel.Info);

                _stop = false;
                _pause = false;

                WorkerThread.Start();

                TraceLine("Internal Thread Started.", TraceLevel.Info);

            }
            catch (Exception ex)
            {
                // clean up by stopping child threads.
                try
                {
                    JobReceiver.Stop();
                    JobReceiver.Join();

                    JobResponse.Stop();
                    JobResponse.Join();

                    JobScheduleAgent.Stop();
                    JobScheduleAgent.Join();

                    JobEnqueue.Stop();
                    JobEnqueue.Join();
                }
                catch (Exception exinternal)
                {
                    TraceLine("Error in start() method" + exinternal, TraceLevel.Info);
                }

                // some other unhandled exception
                LogError(ex.ToString(), "Sagitec.Jobs.Service", "EXE", 0);
                TraceLine("Unable to Start.", TraceLevel.Info);
                TraceLine("Exit Start()", TraceLevel.Debug);
                throw;
            }
            catch
            {
                // some other unhandled exception
                LogError("Non-CLR Exception Caught.", "Sagitec.Jobs.Service", "EXE", 0);
                throw;
            }
            finally
            {
                TraceLine("Exit Start()", TraceLevel.Debug);
            }
        }

        /// <summary>
        /// Pauses the QueueMaster. This prevents any jobs with a QUEUED status from
        /// being picked up. This does not halt execution of currently RUNNING jobs.
        /// </summary>
        public override void Pause()
        {
            TraceIn();
            TraceLine("Enter Pause()", TraceLevel.Debug);
            TraceLine("Pausing Job Master.", TraceLevel.Info);

            _pause = true;
            JobReceiver.Pause();
            JobResponse.Pause();
            JobScheduleAgent.Pause();
            JobEnqueue.Pause();
            TraceLine("Exit Pause()", TraceLevel.Debug);
            TraceOut();
        }

        /// <summary>
        /// Resumes the QueueMaster. This resumes dequeueing of jobs by the QueueMaster.
        /// </summary>
        public override void Resume()
        {
            TraceIn();
            TraceLine("Enter Resume()", TraceLevel.Debug);
            TraceLine("Resuming Job Master.", TraceLevel.Info);

            _pause = false;
            JobReceiver.Resume();
            JobResponse.Resume();
            JobScheduleAgent.Resume();
            JobEnqueue.Resume();
            TraceLine("Exit Resume()", TraceLevel.Debug);
            TraceOut();
        }

        /// <summary>
        /// Calling Stop() stops execution of jobs
        /// and in turn calls Stop() and Join() on the QueueMaster. JobMaster blocks until
        /// QueueMaster's internal thread completes.
        /// </summary>
        public override void Stop()
        {
            TraceIn();
            TraceLine("Enter Stop()", TraceLevel.Debug);

            FireShutdownStarted(this, EventArgs.Empty);
            _stop = true;

            // in case the thread is in a wait, join or sleep state, send it an interrupt.
            WorkerThread.Interrupt();

            // stop it and wait
            JobReceiver.Stop();
            JobReceiver.Join();

            JobResponse.Stop();
            JobResponse.Join();

            JobScheduleAgent.Stop();
            JobScheduleAgent.Join();

            JobEnqueue.Stop();
            JobEnqueue.Join();

            FireShutdownCompleted(this, EventArgs.Empty);
            TraceLine("Stopped.", TraceLevel.Info);
            TraceOut();
        }

        /// <summary>
        /// Main execution loop for the internal Thread. This method will execute indefinitely
        /// until the internal stop flag is set by calling Stop(). Inside this loop, the 
        /// JobMaster will execute any jobs in the in-memory queue 
        /// </summary>
        protected override void Run()
        {
            TraceIn();
            TraceLine("Enter Run()", TraceLevel.Debug);

            // notify that we have started working
            FireWorkStarted(this, EventArgs.Empty);

            // Call the base class method to setup the UtlpassInfo which will be used later for executing any CRUD operations by the framework.
            SetUtlPassInfo();


            // initialize our start time 
            StartTime = DateTime.Now;

            TraceLine("Started at: " + StartTime + ".", TraceLevel.Info);

            try
            {
                while (!_stop)
                {
                    while (!_pause && !_stop)
                    {
                        TraceLine("Inside Loop ", TraceLevel.Info);
                        Thread.Sleep(Common.THREADSLEEPTIME);

                        // if the RUNNABLE queue is empty, wait until it is
                        // pulsed and grab a job to run.
                        lock (Common.RUNNABLE)
                        {
                            TraceLine("Inspecting the runnable queue", TraceLevel.Info);
                            if (!_pause && !_stop)
                            {
                                while (Common.RUNNABLE.Count == 0)
                                    Monitor.Wait(Common.RUNNABLE);
                            }
                            else
                            {
                                continue;
                            }
                        } // lock (RUNNABLE)
                        TraceLine("Inspecting the running queue", TraceLevel.Info);
                        // if the number of jobs currently running is greater than or
                        // equal the number requested then wait until
                        // the running list changes and check again.
                        lock (Common.RUNNING)
                        {
                            if (!_pause && !_stop)
                            {
                                while (Common.RUNNING.Count >= Common.NUMWORKERS)
                                    Monitor.Wait(Common.RUNNING);
                            }
                        } // lock (RUNNING)

                        // if the WORKERS queue is empty, wait until
                        // another worker completes and gets added to the
                        // wait queue.
                        TraceLine("Looking for Available Thread", TraceLevel.Info);

                        string lstrAvailableThread;
                        lock (Common.WORKERS)
                        {
                            if (!_pause && !_stop)
                            {
                                TraceLine("Debug Master .6 ", TraceLevel.Info);
                                while (Common.WORKERS.Count == 0)
                                    Monitor.Wait(Common.WORKERS);
                            }
                            else
                            {
                                continue;
                            }
                            lstrAvailableThread = Common.WORKERS.Dequeue();

                        } // lock (WORKERS)

                        busJobHeader lobjJobHeader = null;

                        // It's ok to run a job now, so pop it off the queue.
                        // also	let the receiver thread know that we have popped a job
                        // so it can potentially get more jobs.
                        lock (Common.RUNNABLE)
                        {
                            lobjJobHeader = Common.RUNNABLE.Dequeue();
                            Monitor.Pulse(Common.RUNNABLE);
                        } // lock (RUNNABLE)
                        // We could get two types of jobs from the runnable queue, a job coming in first time
                        // through the receiver, which will be in a picked up status. and a job coming in again 
                        // through the workcompleted event which would be in a Processing status.
                        // update the jobs coming in for the first time to processing status and also set the start time.
                        if (lobjJobHeader.icdoJobHeader.status_value == "PICK")
                        {
                            TraceLine("About to set job header status to picked up", TraceLevel.Info);
                            BeginTransaction();
                            try
                            {
                                lobjJobHeader.icdoJobHeader.status_value = "PRCS";
                                lobjJobHeader.icdoJobHeader.start_time = DateTime.Now;
                                lobjJobHeader.icdoJobHeader.Update();
                                CommitTransaction();
                            }
                            catch (Exception ex)
                            {
                                TraceLine("Exception while setting job status to Processing " + ex, TraceLevel.Error);
                                RollbackTransaction();
                                TraceLine(ex.Message, TraceLevel.Error);
                            }
                            // Get the next step to be executed within the current job,
                            // we need to do it only the first time the job is picked up
                            // in the subsequent times the OnWorkCompleted event would call this and make
                            // the next step readily available for us.
                            lobjJobHeader.GetNextStep();
                        }

                        TraceLine("Getting Worker.", TraceLevel.Info);
                        // initialize worker
                        IWorker lobjWorker = WorkerFactory.GetJobHandler(lobjJobHeader, lstrAvailableThread);

                        // wire events
                        lobjWorker.WorkStarted += new EventHandler(OnWorkStarted);
                        lobjWorker.WorkCompleted += new EventHandler(OnWorkCompleted);

                        // start worker
                        TraceLine("Starting worker: " + lobjWorker.ToString(), TraceLevel.Info);

                        // start the worker thread
                        lobjWorker.Start();
                        TraceLine("Started Worked Thread ", TraceLevel.Info);
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
                throw;
            }
            finally
            {
                CompleteTime = System.DateTime.Now;

                TraceLine("Stopped at: " + this.CompleteTime.ToString() + ".", TraceLevel.Info);

                // notify of work completed.
                FireWorkCompleted(this, EventArgs.Empty);

                // do not fire shutdown complete until all workers have finished.
                lock (Common.RUNNING)
                {
                    while (Common.RUNNING.Count > 0)
                        Monitor.Wait(Common.RUNNING);
                }

                FireShutdownCompleted(this, EventArgs.Empty);
                TraceLine("Shutdown Completed.", TraceLevel.Info);

                TraceLine("Exit Run()", TraceLevel.Debug);
                TraceOut();
            }
        } // Run

        /// <summary>
        /// Gets all jobs in the database that are in a running status. If there are any,
        /// it is assumed they have not completed because of an abnormal shutdown of the 
        /// JobMaster. 
        /// </summary>
        protected override void Initialize()
        {
            TraceIn();
            TraceLine("Enter Initialize()", TraceLevel.Debug);
            // notify of initialization started.
            FireInitializeStarted(this, EventArgs.Empty);
            TraceLine("Creating Worker Threads...", TraceLevel.Info);
            // Create worker threads needed
            lock (Common.WORKERS)
            {
                for (int lintCounter = 1; lintCounter < Common.CEILING + 1; lintCounter++)
                {
                    Common.WORKERS.Enqueue("[Worker_" + lintCounter + "]");
                }
            }

            // notify of initialization completed.
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

        /// <summary>
        /// Handles the WorkStartedEvent of the workers and bubbles the event up.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnWorkStarted(Object sender, EventArgs eventArgs)
        {
            // TraceIn() does not yield the expected results in a multi-threaded env
            // TraceIn();
            TraceLine("Enter OnWorkStarted()", TraceLevel.Debug);

            IWorker lobjWorker = sender as IWorker;

            if (lobjWorker == null)
                return;


            // add this worker to the running queue.
            lock (Common.RUNNING)
            {
                Common.RUNNING.Add(lobjWorker);
            } // lock (RUNNING)

            TraceLine("Worker started: " + lobjWorker.ToString(), TraceLevel.Info);

            FireWorkStarted(lobjWorker, EventArgs.Empty);

            TraceLine("Exit OnWorkStarted()", TraceLevel.Debug);
            // TraceOut() does not yield the expected results in a multi-threaded env
            // TraceOut();
        } // OnWorkStarted

        /// <summary>
        /// Handles the WorkCompletedEvent of the workers and bubbles the event up.
        /// Recycles the worker name, removes and Pulses the worker from the 
        /// Common.RUNNING list, updates the Common.BATCHJOBS list, sends notifications
        /// for SPROC type jobs by calling SprocHandler.Notify(), and updates statistics.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnWorkCompleted(Object sender, EventArgs eventArgs)
        {
            // TraceIn() does not yield the expected results in a multi-threaded env
            //TraceIn();
            TraceLine("Enter OnWorkCompleted()", TraceLevel.Debug);

            IWorker lobjWorker = sender as IWorker;
            if (lobjWorker == null)
                return;

            lock (Common.WORKERS)
            {
                Common.WORKERS.Enqueue(lobjWorker.Name.Clone().ToString());
                Monitor.Pulse(Common.WORKERS);
            }

            // remove worker from the running list.
            lock (Common.RUNNING)
            {
                Common.RUNNING.Remove(lobjWorker);
                Monitor.Pulse(Common.RUNNING);
            }

            // Inspect the jobheader that has been completed now and if there 
            // are further steps to be performed add it to the Runnable queue, if not add it to the Response queue

            JobDetailHandler lobjJobHandler = lobjWorker as JobDetailHandler;

            if (lobjJobHandler != null)
            {
                busJobHeader lobjJobHeader = lobjJobHandler.lobjJobHeader;
                //Check whether previous step was cancelled by user, if yes, then cancel job
                if (lobjJobHeader.ibusCurrentJobDetail.icdoJobDetail.status_value == BatchHelper.JOB_DETAIL_STATUS_CANCELLED)
                {
                    //skip all jobs
                    lobjJobHeader.GetNextStep();
                    while (lobjJobHeader.ibusCurrentJobDetail != null)
                    {
                        lobjJobHeader.ibusCurrentJobDetail.icdoJobDetail.status_value = BatchHelper.JOB_DETAIL_STATUS_SKIPPED;
                        lobjJobHeader.ibusCurrentJobDetail.icdoJobDetail.Update();
                        lobjJobHeader.GetNextStep();
                    }
                    

                }
                else
                {
                    // Get the next step to be executed within the current job
                    lobjJobHeader.GetNextStep();
                }
                if (lobjJobHeader.ibusCurrentJobDetail == null)
                {
                    TraceLine("Next job is Null, so enqueinq it back in Response queue", TraceLevel.Info);
                    // Add the jobheader to the Response queue, This would make the job to completed from the Job Service perspective.
                    lock (Common.RESPONSE)
                    {
                        Common.RESPONSE.Enqueue(lobjJobHeader);
                        Monitor.Pulse(Common.RESPONSE);
                    }
                }
                else
                {
                    TraceLine("Some more steps to be completed, adding job the the runnable queue", TraceLevel.Info);
                    // Add it to the runnable queue, if there are still steps to be completed
                    lock (Common.RUNNABLE)
                    {
                        Common.RUNNABLE.Enqueue(lobjJobHandler.lobjJobHeader);
                        Monitor.Pulse(Common.RUNNABLE);
                    }
                }
            }

            TraceLine("Worker completed: " + lobjWorker.ToString(), TraceLevel.Info);

            FireWorkCompleted(lobjWorker, EventArgs.Empty);

            TraceLine("Exit OnWorkCompleted()", TraceLevel.Debug);

            // TraceOut() does not yield the expected results in a multi-threaded env
            //TraceOut();
        } // OnWorkCompleted

        private class Nested
        {

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            { }

            internal static readonly JobMaster instance = new JobMaster();

        }
    }
}
