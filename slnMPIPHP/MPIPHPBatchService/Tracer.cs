using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Threading;
using System.Collections.Generic;

namespace MPIPHP.MPIPHPJobService
{
    [Flags]
    public enum TraceLevel { None = 0, Debug = 1, Info = 2, Error = 4 }

    /// <summary>
    /// Summary description for TraceLogger.
    /// </summary>
    public class Tracer : IDisposable
    {
        private const string DEFAULT_PATH = @"c:\";
        public TraceLevel Level{ get; private set; }
        public string LogFileName { get; private set; }
        public string LogFileNameWithPath { get; private set; }
        public string LogPath { get; private set; }
        public string ArchivePath { get; private set; }
        public Stack<DateTime> TimeStack { get; private set; }

        public static readonly int MAX_LINE_WIDTH = 110;
        private bool bDisposed = false;

        // If we don't pass in any path for error log we are going to use the default path
        public Tracer(string astrTraceFileName, TraceLevel tl)
            : this(astrTraceFileName, tl, DEFAULT_PATH, DEFAULT_PATH)
        {

        }


        public Tracer(string astrTraceFileName, TraceLevel tl, string astrLogPath, string astrArchivePath)
        {
            // Set Logpath as well as Archive path
            LogPath = astrLogPath;
            ArchivePath = astrArchivePath;

            // Set the tracefileName and the complete path
            LogFileName = astrTraceFileName;
            LogFileNameWithPath = LogPath + "\\" + LogFileName + ".log";

            // set the trace level
            Level = tl;

            // Initialize the stack variable
            TimeStack = new Stack<DateTime>();
        }

        ~Tracer()
        {
            //the destructor makes sure that your tracking logs out correctly.
            if (!bDisposed)
                Dispose();
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
            bDisposed = true;
        }

        public void TraceIn()
        {
            // If no tracelevel is set, we don't have to do anything further.
            if (Level == TraceLevel.None)
                return;

            CheckForLogName();
            // keep track of when we traced in using a Stack object, so we can find out the time elapsed when
            // we call traceout().
            DateTime ldtTraceIn = DateTime.Now;
            TimeStack.Push(ldtTraceIn);

            TraceLine("Trace In Started " + ldtTraceIn, Level);
        }

        private void CheckForLogName()
        {
            if (string.IsNullOrEmpty(LogFileNameWithPath))
            {
                throw new Exception("Please set the LogName property before tracing is to be done.");
            }
        }

        public void TraceOut()
        {
            // If no tracelevel is set, we don't have to do anything further.
            if (Level == TraceLevel.None)
                return;

            CheckForLogName();

            DateTime ltsTraceOutTime = DateTime.Now;
            if (TimeStack.Count > 0)
            {
                // Popout the last datetime value available from the Stack object, so we can find out the time elapsed when
                // we call traceout().
                TimeSpan ltsElapsed = ltsTraceOutTime - TimeStack.Pop();
                TraceLine("Time Elapsed" + ltsElapsed , Level);
            }
            TraceLine("Trace Out" + ltsTraceOutTime, Level);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="astrTraceMessage"></param>
        /// <param name="tl"></param>
        public void TraceLine(string astrTraceMessage, TraceLevel tl)
        {
            // If no tracelevel is set, we don't have to do anything further.
            if (Level == TraceLevel.None)
                return;

            string lstrTraceMessage = "[" + DateTime.Now.ToString("hh.mm.ss.ffff") + "] " + astrTraceMessage;
            // if the tracelevel passed in is less than the configured level bail out
            bool lblntrace = (tl == (tl & Level));

            // If this trace level doesnot 
            if (!lblntrace)
                return;

            FileInfo lobjTraceFileInfo;
            StreamWriter lobjLogStreamWriter = null;
            FileStream lobjTraceFileStream = null;

            try
            {

                // If the file exists, Create it and open it, else open the existing file in append mode.
                if (File.Exists(LogFileNameWithPath))
                {
                    lobjTraceFileInfo = new FileInfo(LogFileNameWithPath);
				    if (lobjTraceFileInfo.Length > 2048000)
				    {
					    ArchiveLog();
				        lobjTraceFileStream = File.Open(LogFileNameWithPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
				    }
				    else
				    {
					    lobjTraceFileStream = File.Open(LogFileNameWithPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
				    }
                }
                else
                {
                    lobjTraceFileStream = File.Open(LogFileNameWithPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                }

                lobjLogStreamWriter = new StreamWriter(lobjTraceFileStream);

                while (lstrTraceMessage.Length > MAX_LINE_WIDTH)
                {
                    string lstrTrimMesg = lstrTraceMessage.Substring(0, MAX_LINE_WIDTH);
                    lobjLogStreamWriter.WriteLine(lstrTrimMesg);
                    lstrTraceMessage = lstrTraceMessage.Substring(MAX_LINE_WIDTH);
                }
                lobjLogStreamWriter.WriteLine(lstrTraceMessage);
                lobjLogStreamWriter.Flush();
            }
            catch
            {
                throw;
                // TODO 
                // Need to think of what needs to be done if calls to write to file fails.
            }
            finally
            {
                try
                {
                    // clean-up and close
                    if (null != lobjLogStreamWriter)
                    {
                        lobjLogStreamWriter.Flush();
                        lobjLogStreamWriter.Close();
                    }
                    if (null != lobjTraceFileStream)
                    {
                        lobjTraceFileStream.Close();
                    }
                }
                catch
                {
                    throw;
                    // TODO 
                    // Need to think of what needs to be done if calls to flush() and close() fails.
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void ArchiveLog()
        {
            string astrArchiveFile = "Archived_" + LogFileName +
                "_" + DateTime.Today.ToString("MMddyyyy") + "_" +
                DateTime.Now.ToString("hhmmss.ffff") + ".log";

            FileInfo lobjTraceFileInfo = new FileInfo(LogFileNameWithPath);
            try
            {
                string lstrFullarchiveFilePath = ArchivePath + "\\" + astrArchiveFile;
                lobjTraceFileInfo.CopyTo(lstrFullarchiveFilePath, true);
            }
            catch (Exception ex)
            {
                throw;
                // This is most likely cannot create file if it exists error..
                // changed the file format so that it should not occur.
                // TODO
                // To add this error to the Database
            }
            finally
            {
                lobjTraceFileInfo = null;
            }
        }
    }
}
