using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPIPHP.MPIPHPJobService
{
    public interface IWorker : IDisposable
    {
        /// <summary>
        /// The event fired by a class derived from worker when it has begun performing work.  Typically
        /// at the start of its Run() Method
        /// </summary>
        event EventHandler WorkStarted;

        /// <summary>
        /// The event fired by a class derived from worker when it has completed performing work.  Typically
        /// at the end of its Run() Method
        /// </summary>
        event EventHandler WorkCompleted;

        /// <summary>
        /// The event fired by a class derived from worker when it has begun initialization.  Typically
        /// at the start of its Initialize() Method
        /// </summary>
        event EventHandler InitializeStarted;

        /// <summary>
        /// The event fired by a class derived from worker when it has completed performing initialization.  Typically
        /// at the end of its Initialize() Method
        /// </summary>
        event EventHandler InitializeCompleted;

        /// <summary>
        /// The event fired by a class derived from worker when it has begun shutting down.
        /// </summary>
        event EventHandler ShutdownStarted;

        /// <summary>
        /// The event fired by a class derived from worker when it has completed shutting down.
        /// </summary>
        event EventHandler ShutdownCompleted;

        // properties
        /// <summary>
        /// Time in which the worker was started.
        /// </summary>
        DateTime StartTime { get; }

        /// <summary>
        /// Time in which the worker completed it's work.
        /// </summary>
        DateTime CompleteTime { get; }

        /// <summary>
        /// If work has completed, the amount of time between StartTime and CompleteTime,
        /// otherwise, the amount of time between StartTime and System.DateTime.Now.
        /// </summary>
        TimeSpan RunningTime { get; }

        /// <summary>
        /// The HashCode of the internal Thread object. 
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The name assigned to his worker by the Job Master.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Wraps the internal Thread objects IsAlive(). 
        /// </summary>
        bool IsAlive { get; }

        // methods		
        /// <summary>
        /// Wraps the internal Thread objects Start(). Should be overridden.
        /// </summary>
        void Start();

        /// <summary>
        /// Wraps the internal Thread objects Suspend(). Should be overridden.
        /// </summary>
        void Pause();

        /// <summary>
        /// Wraps the internal Thread objects Resume(). Should be overridden. 
        /// </summary>
        void Resume();

        /// <summary>
        /// Wraps the internal Thread objects Abort(). Should be overridden. 
        /// </summary>
        void Stop();

        /// <summary>
        /// Wraps the internal Thread objects Join(). 
        /// </summary>
        void Join();

        /// <summary>
        /// Wraps the internal Thread objects Abort(). Should be overridden by classes
        /// requiring compound operation shut down procedures.
        /// </summary>
        void ShutDown();

    }
}
