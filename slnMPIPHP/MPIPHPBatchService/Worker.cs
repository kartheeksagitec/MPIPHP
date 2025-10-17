using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Threading;
using MPIPHP.BusinessObjects;
using Sagitec.BusinessObjects;
using Sagitec.DBUtility;

namespace MPIPHP.MPIPHPJobService
{

    /// <summary>
    /// Summary description for Worker.
    /// </summary>
    public class Worker : IWorker
    {
        // protected members
        public DateTime StartTime { get; protected set; }
        public DateTime CompleteTime { get; protected set; }
        protected Thread WorkerThread { get; set; }
        protected Tracer LogTracer { get; set; }
        protected utlPassInfo iobjPassInfo { get; set; }
        protected utlPassInfo iobjPassInfoLog { get; set; }


        // events
        public event EventHandler InitializeStarted;
        public event EventHandler InitializeCompleted;

        public event EventHandler WorkStarted;
        public event EventHandler WorkCompleted;

        public event EventHandler ShutdownStarted;
        public event EventHandler ShutdownCompleted;

        // constructors
        protected Worker()
            : this("Default")
        {
        }

        protected Worker(string astrWorkerName)
        {
            // Initialize the start time and complete time to min value, so we know when it is modified
            StartTime = DateTime.Now;
            CompleteTime = DateTime.MinValue;

            WorkerThread = new Thread(Run);

            // Set the name of the worker based on user passed in value.
            Name = astrWorkerName.ToUpper().Trim();
            // default trace level is None
            TraceLevel tl = TraceLevel.None;

            // set trace level based on configurable parameters.
            if (Common.TRACE_DEBUG)
                tl = tl | TraceLevel.Debug;
            if (Common.TRACE_INFO)
                tl = tl | TraceLevel.Info;
            if (Common.TRACE_ERROR)
                tl = tl | TraceLevel.Error;

            LogTracer = new Tracer(Name, tl, Common.TRACE_FILE_PATH, Common.ARCHIVE_FILE_PATH);
        }

        // properties
        public TimeSpan RunningTime
        {
            get
            {
                TimeSpan ts;

                if (CompleteTime != DateTime.MinValue)
                {
                    ts = CompleteTime - StartTime;
                    return ts;
                }
                else
                {
                    ts = DateTime.Now - StartTime;
                    return ts;
                }
            }
        }

        public int Id
        {
            get
            {
                if (WorkerThread != null)
                    return WorkerThread.GetHashCode();
                else
                    throw new InvalidOperationException("Internal Thread Is Not Initialized...");
            }
        }

        public string Name
        {
            get
            {
                if (WorkerThread != null)
                    return WorkerThread.Name;
                else
                    throw new InvalidOperationException("Internal Thread Is Not Initialized...");
            }
            set
            {
                if (WorkerThread != null)
                {
                    if (WorkerThread.Name == null)
                        WorkerThread.Name = value.Trim().ToUpper();
                    else
                        new InvalidOperationException("Name Already Assigned...");
                }
                else
                    throw new InvalidOperationException("Internal Thread Is Not Initialized...");
            }
        }

        public virtual bool IsAlive
        {
            get
            {
                if (WorkerThread != null)
                    return WorkerThread.IsAlive;
                else
                    throw new InvalidOperationException("Internal Thread Is Not Initialized...");
            }
        }

        // methods
        public virtual void Start()
        {
            if (WorkerThread != null)
            {
                Initialize();
                WorkerThread.Start();
            }
            else
            {
                throw new InvalidOperationException("Internal Thread Is Not Initialized...");
            }
        }

        public virtual void Pause()
        {
            TraceIn();
            TraceLine("Enter Pause", TraceLevel.Debug);

            WorkerThread.Suspend();

            TraceLine("Exit Pause", TraceLevel.Debug);
            TraceOut();
        }

        public virtual void Resume()
        {
            TraceIn();
            TraceLine("Enter Resume()", TraceLevel.Debug);

            WorkerThread.Resume();

            TraceLine("Exit Resume()", TraceLevel.Debug);
            TraceOut();
        }

        public virtual void Stop()
        {
            TraceIn();
            TraceLine("Enter Stop()", TraceLevel.Debug);

            try
            {
                WorkerThread.Abort();
            }
            catch (ThreadAbortException ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
            finally
            {
                TraceLine("Exit Stop()", TraceLevel.Debug);
                TraceOut();
            }
        }

        public virtual void Join()
        {
            WorkerThread.Join();
        }

        public virtual void ShutDown()
        {
            TraceIn();
            TraceLine("Enter Shutdown()", TraceLevel.Debug);

            try
            {
                WorkerThread.Abort();
            }
            catch (ThreadAbortException ex)
            {
                TraceLine("Exception Caught: \n" + ex.ToString(), TraceLevel.Error);
                throw;
            }
            finally
            {
                TraceLine("Exit Shutdown()", TraceLevel.Debug);
                TraceOut();
            }

        }

        protected virtual void Run()
        {
            TraceIn();
            TraceLine("Enter Run()", TraceLevel.Debug);

            FireWorkStarted(this, EventArgs.Empty);
            FireWorkCompleted(this, EventArgs.Empty);

            TraceLine("Exit Run()", TraceLevel.Debug);
            TraceOut();
        }

        protected virtual void Initialize()
        {
            TraceIn();
            TraceLine("Enter Initialize()", TraceLevel.Debug);

            FireInitializeStarted(this, EventArgs.Empty);
            FireInitializeCompleted(this, EventArgs.Empty);

            TraceLine("Exit Initialize()", TraceLevel.Debug);
            TraceOut();
        }

        protected virtual void Shutdown()
        {
            TraceIn();
            TraceLine("Enter Shutdown()", TraceLevel.Debug);

            FireShutdownStarted(this, EventArgs.Empty);
            FireShutdownCompleted(this, EventArgs.Empty);

            TraceLine("Exit Shutdown()", TraceLevel.Debug);
            TraceOut();
        }

        protected void FireInitializeStarted(Object sender, EventArgs eventArgs)
        {
            if (InitializeStarted != null)
                InitializeStarted(sender, EventArgs.Empty);
            return;
        }

        protected void FireInitializeCompleted(Object sender, EventArgs eventArgs)
        {
            if (InitializeCompleted != null)
                InitializeCompleted(sender, EventArgs.Empty);
            return;
        }

        protected void FireWorkStarted(Object sender, EventArgs eventArgs)
        {
            if (WorkStarted != null)
                WorkStarted(sender, EventArgs.Empty);
            return;
        }

        protected void FireWorkCompleted(Object sender, EventArgs eventArgs)
        {
            if (WorkCompleted != null)
                WorkCompleted(sender, EventArgs.Empty);
            return;
        }

        protected void FireShutdownStarted(Object sender, EventArgs eventArgs)
        {
            if (ShutdownStarted != null)
                ShutdownStarted(sender, EventArgs.Empty);
            return;
        }

        protected void FireShutdownCompleted(Object sender, EventArgs eventArgs)
        {
            if (ShutdownCompleted != null)
                ShutdownCompleted(sender, EventArgs.Empty);
            return;
        }


        protected void LogError(string message, string source, string appType,
            int debugLevel)
        {
            TraceLine(message, TraceLevel.Error);

        }

        protected void TraceIn()
        {
            if (LogTracer != null)
            {
                try
                {
                    LogTracer.TraceIn();
                }
                catch { }
            }
            else
            {
                throw new InvalidOperationException("Internal Tracer Is Not Initialized...");
            }
        }

        protected void TraceOut()
        {
            if (LogTracer != null)
            {
                try
                {
                    LogTracer.TraceOut();
                }
                catch { }
            }
            else
            {
                throw new InvalidOperationException("Internal Tracer Is Not Initialized...");
            }
        }

        protected virtual void TraceLine(string message, TraceLevel tl)
        {
            if (LogTracer != null)
            {
                try
                {
                    LogTracer.TraceLine(message, tl);
                }
                catch (Exception ex)
                {
                    // Cant do much when we can't write to a file..
                    // TODO need to have logic to write the error to DB
                }
            }
            else
            {
                throw new InvalidOperationException("Internal Tracer Is Not Initialized...");

            }
        }

        //Implement IDisposable.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Free other state (managed objects).
                LogTracer.Dispose();

            }
            // Free your own state (unmanaged objects).
            // Set large fields to null.
        }

        // Use C# destructor syntax for finalization code.
        ~Worker()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }

        public override string ToString()
        {
            StringBuilder lsbWorker = new StringBuilder();
            lsbWorker.Append("\n[Id:" + Id + "]  ");
            lsbWorker.Append("[Name:" + Name + "]  ");
            lsbWorker.Append("[StartTime:" + StartTime + "]  ");

            if (CompleteTime == DateTime.MinValue)
                lsbWorker.Append("[CompleteTime: NA]  ");
            else
                lsbWorker.Append("[CompleteTime:" + CompleteTime + "]  ");

            lsbWorker.Append("[RunningTime:" + RunningTime + "]  ");
            lsbWorker.Append("[IsAlive:" + IsAlive + "]");

            return lsbWorker.ToString();
        }

        protected void SetUtlPassInfo()
        {
            SetUtlPassInfo(Name);
        }

        /// <summary>
        /// Helper method to setup utlpassinfo, be cautious, this has to be called 
        /// only within the Run() method of the particular thread, since the values in utlPassInfo
        /// are marked with "ThreadStatic".
        /// </summary>
        /// <param name="astrUserId"></param>
        protected void SetUtlPassInfo(string astrUserId)
        {
            iobjPassInfo = new utlPassInfo();
            utlPassInfo.iobjPassInfo = iobjPassInfo;
            iobjPassInfo.iconFramework = DBFunction.GetDBConnection();
            iobjPassInfo.istrUserID = astrUserId;

            iobjPassInfoLog = new utlPassInfo();
            utlPassInfo.iobjPassInfoProcessLog = iobjPassInfoLog;
            iobjPassInfoLog.iconFramework = DBFunction.GetDBConnection();
            iobjPassInfoLog.istrUserID = astrUserId;
        }

        protected virtual void BeginTransaction()
        {
            try
            {
                if (iobjPassInfo != null)
                {
                    iobjPassInfo.BeginTransaction();
                }
            }
            catch
            {
                
            }
        }

        protected virtual void CommitTransaction()
        {
            try
            {
                if (iobjPassInfo != null)
                {
                    iobjPassInfo.Commit();
                }
            }
            catch
            {
                
            }
        }

        protected virtual void RollbackTransaction()
        {
            try
            {
                if (iobjPassInfo != null)
                {
                    iobjPassInfo.Rollback();
                }
            }
            catch
            {
                
            }
        }

    }
}
