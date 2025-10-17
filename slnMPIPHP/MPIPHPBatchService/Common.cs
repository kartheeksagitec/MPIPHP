using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Collections.ObjectModel;
using MPIPHP.BusinessObjects;
//using NeoSpin.BusinessObjects;


namespace MPIPHP.MPIPHPJobService
{
    /// <summary>
    ///	Common encapulates and provides global access to configurable parameters used by the 
    ///	NeoSpinJobService component.
    /// </summary>
    public class Common
    {

        // Global/shared data
        // access to this data needs to by synchronized.
        // Synchronization is done inside the worker threads for now.

        public static Queue<string> WORKERS = new Queue<string>();
        public static Queue<busJobHeader> RUNNABLE = new Queue<busJobHeader>();
        public static Queue<busJobHeader> RESPONSE = new Queue<busJobHeader>();
        public static Queue<int> SCHEDULE = new Queue<int>();

        public static ArrayList RUNNING = new ArrayList();

        // configurable parameters used by the component.
        public static readonly int THREADSLEEPTIME;
        public static readonly int NUMWORKERS;          // Future use 
        public static readonly int CEILING;             // Future use 
        public static readonly int MAXRUNNABLES;        // Future use 

        // Tracing configuration values
        public static readonly bool TRACE_DEBUG;
        public static readonly bool TRACE_INFO;
        public static readonly bool TRACE_ERROR;

        public static readonly string TRACE_FILE_PATH;
        public static readonly string ARCHIVE_FILE_PATH;

        static Common()
        {
            Parameters p = new Parameters();
            p.Load();

            NUMWORKERS = p.NumWorkers;			
            CEILING = p.Ceiling;				
            MAXRUNNABLES = p.MaxRunnables;
            THREADSLEEPTIME = p.ThreadSleepTime;

            // tracing
            TRACE_DEBUG = p.TraceDebug;
            TRACE_INFO = p.TraceInfo;
            TRACE_ERROR = p.TraceError;
            TRACE_FILE_PATH = p.TraceFilePath;
            ARCHIVE_FILE_PATH = p.ArchiveFilePath;
        }
    }
}
