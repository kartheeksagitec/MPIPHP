using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MPIPHP.MPIPHPJobService
{
    public class BatchStepTimings
    {
        /// <summary>
        /// Total number of retrievals events
        /// </summary>
        private int _iDBSelects = 0;

        /// <summary>
        /// Stop Watch for data retrieval events
        /// </summary>
        Stopwatch iobjSelectWatch = new Stopwatch();

        /// <summary>
        /// Total number of data insert events
        /// </summary>
        private int _iDBInserts = 0;

        /// <summary>
        /// Stop Watch for data insert events
        /// </summary>
        Stopwatch iobjInsertWatch = new Stopwatch();

        /// <summary>
        /// Total number of data update events
        /// </summary>
        private int _iDBUpdates = 0;

        /// <summary>
        /// Stop Watch for data update events
        /// </summary>
        Stopwatch iobjUpdateWatch = new Stopwatch();

        /// <summary>
        /// Total number of data update events
        /// </summary>
        private int _iLoads = 0;

        /// <summary>
        /// Stop Watch for data load events
        /// </summary>
        Stopwatch iobjLoadWatch = new Stopwatch();

        /// <summary>
        /// Constructor - initializes timing variables
        /// </summary>
        public BatchStepTimings()
        {
        }

        /// <summary>
        /// Data Retrieval Start Event Handler - Stores the event occurence time
        /// </summary>
        [Conditional("DEBUG")]
        public void OnDBSelectStart()
        {
            iobjSelectWatch.Start();
        }

        /// <summary>
        /// Data Retrieval End Event - Adds event time, increments event count
        /// </summary>
        [Conditional("DEBUG")]
        public void OnDBSelectEnd()
        {
            iobjSelectWatch.Stop();
            ++_iDBSelects;
        }

        /// <summary>
        /// DB Insert Start Event - Stores the event occurence time
        /// </summary>
        [Conditional("DEBUG")]
        public void OnDBInsertStart()
        {
            iobjInsertWatch.Start();
        }

        /// <summary>
        /// DB Insert End Event - Adds event time, increments event count
        /// </summary>
        [Conditional("DEBUG")]
        public void OnDBInsertEnd()
        {
            iobjInsertWatch.Stop();
            ++_iDBInserts;
        }

        /// <summary>
        /// Data Update Start Event - Stores the event occurence time
        /// </summary>
        [Conditional("DEBUG")]
        public void OnDBUpdateStart()
        {
            iobjUpdateWatch.Start();
        }

        /// <summary>
        /// Data Update End Event - Adds event time, increments event count
        /// </summary>
        [Conditional("DEBUG")]
        public void OnDBUpdateEnd()
        {
            iobjUpdateWatch.Stop();
            ++_iDBUpdates;
        }

        /// <summary>
        /// Load Start Event - Stores the event occurence time
        /// </summary>
        [Conditional("DEBUG")]
        public void OnLoadStart()
        {
            iobjLoadWatch.Start();
        }

        /// <summary>
        /// Load End Event - Adds event time, increments event count
        /// </summary>
        [Conditional("DEBUG")]
        public void OnLoadEnd()
        {
            iobjLoadWatch.Stop();
            ++_iLoads;
        }

        /// <summary>
        /// Load End Event - Adds event time, increments event count
        /// </summary>
        /// <param name="aintCount">Collection.Count when a collection is loaded.</param>
        [Conditional("DEBUG")]
        public void OnLoadEnd(int aintCount)
        {
            iobjLoadWatch.Stop();
            _iLoads += aintCount;
        }

        /// <summary>
        /// Total time taken for all the data retrieval events
        /// </summary>
        public TimeSpan DBSelectTimings
        {
            get { return iobjSelectWatch.Elapsed; }
        }

        /// <summary>
        /// Number of data retrieval events
        /// </summary>
        public int DBSelectCount
        {
            get { return _iDBSelects; }
        }

        /// <summary>
        /// Total time taken for all the db insert events
        /// </summary>
        public TimeSpan DBInsertTimings
        {
            get { return iobjInsertWatch.Elapsed; }
        }

        /// <summary>
        /// Number of db insert events
        /// </summary>
        public int DBInsertCount
        {
            get { return _iDBInserts; }
        }

        /// <summary>
        /// Total time taken for all the data update events
        /// </summary>
        public TimeSpan DBUpdateTimings
        {
            get { return iobjUpdateWatch.Elapsed; }
        }

        /// <summary>
        /// Number of data update events
        /// </summary>
        public int DBUpdateCount
        {
            get { return _iDBUpdates; }
        }

        /// <summary>
        /// Total time taken for all the data load events
        /// </summary>
        public TimeSpan LoadTimings
        {
            get { return iobjLoadWatch.Elapsed; }
        }

        /// <summary>
        /// Number of data load events
        /// </summary>
        public int LoadCount
        {
            get { return _iLoads; }
        }
    }
}
