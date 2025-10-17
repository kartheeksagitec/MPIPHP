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
	/// Summary description for JobReceiver.
	/// </summary>
	public class JobReceiver : Worker
	{
		private bool _stop = false;
		private bool _pause = false;

		private JobReceiver() : base("[JOB_RECEIVER]")
		{ 	}
	
		internal static JobReceiver GetInstance()
		{
			return Nested.instance;
		}

		public override void Start()
		{
			TraceLine("Enter Start()",  TraceLevel.Info);
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
			TraceLine("Started at: " + StartTime , TraceLevel.Info);




			// notify work has started
			FireWorkStarted(this, EventArgs.Empty);
			
			try
			{
				while (!_stop)
				{
					while(!_pause && !_stop)
					{
						lock (Common.RUNNABLE)
						{
							// don't let the queue master thread get too far ahead of the job master.
							// this will prevent unnecessary dequeuing jobs from the db.
							// queue master will wait until the RUNNABLE queue drops below a count
							// of MAXRUNNABLES. effectively having a RUNNABLE job buffer of two for the job
							// master.

							if ( !_pause && !_stop)
							{
								while (Common.RUNNABLE.Count >= Common.MAXRUNNABLES)
									Monitor.Wait(Common.RUNNABLE);
							}
						} // lock (RUNNABLE)


					    busJobHeader lobjJobHeader = null;
						TraceLine("Checking for a job.", TraceLevel.Info);

						try
						{
							while ( !_pause && !_stop && ( (lobjJobHeader = busJobHeader.GetNextJob() ) == null ) )
							{
								TraceLine("No jobs found.", TraceLevel.Info);
							    TraceLine("Sleeping for [" + Common.THREADSLEEPTIME + "] seconds.", TraceLevel.Info);
								Thread.Sleep(Common.THREADSLEEPTIME);
								TraceLine("Checking for a job.", TraceLevel.Info);
							}  // while db queue is empty
						}
						catch (ThreadInterruptedException t)
						{
                            LogError(t.ToString(), "NeoSpinJobService", "EXE", 0);
							TraceLine("Expected: An exception was caught while fetching from busJobHeader GetNextJob...", 
								TraceLevel.Error);
							continue;
						}
						catch (Exception ex)
						{
							// don't let any exceptions stop the queue thread
							LogError(ex.ToString(), "NeoSpinJobService", "EXE", 0);
                            TraceLine("An exception was caught while fetching from GetNextJob()...", TraceLevel.Error);
							TraceLine("Sleeping for [60] seconds.", TraceLevel.Error);
                            Thread.Sleep(Common.THREADSLEEPTIME);
							continue;
						}

                        if (lobjJobHeader != null)
						{
                            // A job header record has been pulled from the database in a "queued" status once the receiver thread
                            // picks up the job we need to change the status of the job_header to "picked up".
                            // TODO : Use a constant value
						    BeginTransaction();
                            try
                            {
                                lobjJobHeader.icdoJobHeader.status_value = "PICK";
                                lobjJobHeader.icdoJobHeader.Update();
                                CommitTransaction();
                            }
                            catch (Exception ex)
                            {
                                RollbackTransaction();
                                TraceLine(ex.Message, TraceLevel.Error);
                            }

						    lock (Common.RUNNABLE)
							{
								TraceLine("Enqueing job to the internal queue: " , TraceLevel.Info);
								Common.RUNNABLE.Enqueue(lobjJobHeader);
								Monitor.Pulse(Common.RUNNABLE);
							} // lock (RUNNABLE)
						}

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
                LogError(ex.ToString(), "NeoSpinJobService", "EXE", 0);
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
			TraceLine("Enter Initialize()",TraceLevel.Debug);
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
			{	}

			internal static readonly JobReceiver instance = new JobReceiver();
		
		}
	}
}
