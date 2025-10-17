using System;
using System.ComponentModel;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MPIPHP.MPIPHPJobService
{
    /// <summary>
    /// Summary description for Parameters.
    /// </summary>
    public class Parameters
    {
        public int NumWorkers { get; set; }
        public int Ceiling { get; set; }
        public int MaxRunnables { get; set; }
        public bool TraceDebug { get; set; }
        public bool TraceInfo { get; set; }
        public bool TraceError { get; set; }
        public string TraceFilePath { get; set; }
        public string ArchiveFilePath { get; set; }
        public int ThreadSleepTime { get; set; }

        public void Load()
        {
            NumWorkers = Convert.ToInt32(ConfigurationManager.AppSettings["NumWorkers"]);
            Ceiling = Convert.ToInt32(ConfigurationManager.AppSettings["Ceiling"]);
            MaxRunnables = Convert.ToInt32(ConfigurationManager.AppSettings["MaxRunnables"]);
            ThreadSleepTime = Convert.ToInt32(ConfigurationManager.AppSettings["ThreadSleepTime"]);

            TraceDebug = Convert.ToBoolean(ConfigurationManager.AppSettings["TraceDebug"]); ;
            TraceInfo = Convert.ToBoolean(ConfigurationManager.AppSettings["TraceInfo"]);
            TraceError = Convert.ToBoolean(ConfigurationManager.AppSettings["TraceError"]);

            TraceFilePath = Convert.ToString(ConfigurationManager.AppSettings["TraceFilePath"]);
            ArchiveFilePath = Convert.ToString(ConfigurationManager.AppSettings["ArchiveFilePath"]);

        }
    }
}
