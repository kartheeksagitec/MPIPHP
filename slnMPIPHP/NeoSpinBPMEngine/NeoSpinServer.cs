using Sagitec.BusinessObjects;
using Sagitec.Common;
using System;
using Sagitec.BusinessTier;
using Sagitec.MetaDataCache;
using Sagitec.Rules;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Sagitec.Interface;
using System.Configuration;
using NeoSpin.BusinessObjects;
using NeoBase.Common;
using Sagitec.DataObjects;
using System.Linq;

namespace NeoBPMN.Service
{
    /// <summary>
    /// Main server application for my service.
    /// </summary>
    /// <remarks>
    /// <para>$Id$</para>
    /// <author>Authors: Sagitec</author>
    /// </remarks>
    public class NeoSpinServer
    {
        #region Properties

        private busMQRequestHandler iobjRequestHandler = null;

        /// <summary>
        /// Definitions of the state codes for the server process.
        /// </summary>
        private enum NeoSpinServerState
        {
            /// <summary>The server is not running.</summary>
            Stopped,

            /// <summary>The server is starting.</summary>
            Starting,

            /// <summary>The server is running.</summary>
            Running,

            /// <summary>The server is stopping.</summary>
            Stopping,
        };

        /// <summary>
        /// Time in milliseconds to respond to a <see cref="Start"/> or
        /// <see cref="Stop"/>.
        /// </summary>
        private const int SYSRESPONSE_INTERVAL = 10000;

        /// <summary>
        /// The current process state.
        /// </summary>
        private volatile NeoSpinServerState iServerState;

        /// <summary>
        /// Lock object reserved for <see cref="Start"/> and <see cref="Stop"/>.
        /// </summary>
        private readonly object iStartStopLock = new object();

        private string[] arguments;

        /// <summary>
        /// Common BPM service code class
        /// </summary>
        //public BPMCommonService iBPMCommonService;

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public NeoSpinServer(string[] args)
        {
            this.arguments = args;
            this.iServerState = NeoSpinServerState.Stopped;
            //iBPMCommonService = new BPMCommonService();
            // Create event handler to log an unhandled exception before the
            // server application is terminated by the CLR.
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(AppUnhandledExceptionEventHandler);
        }

        #region private methods

        /// <summary>
        /// Event handler to log the unhandled exception event and perform an
        /// orderly shutdown before the application is terminated by the CLR.
        /// </summary>
        /// <remarks>
        /// This event handler is called by the CLR on the thread that
        /// encountered the unhandled exception. The code in this handler, as
        /// well as any single-threaded code the handler calls, will be executed
        /// without being suddenly terminated. When done executing this handler,
        /// the CLR immediately and silently terminates the application and all
        /// application threads. However, while executing this handler, the other
        /// threads in the application are still alive and the CLR keeps scheduling
        /// and running them. Plus, the CLR allows the thread executing the handler
        /// to be programmatically controlled. It does not automatically abort the
        /// thread if the handler goes into a tight loop or suspends its thread
        /// (which of course could be abused and allow an application to get into
        /// an unknown state). If code in this handler or code called by this
        /// handler encounters another unhandled exception, this handler is NOT
        /// called again. Instead, the CLR proceeds to terminate the application.
        /// </remarks>
        /// <param name="sender">Reference to object that initiated event.</param>
        /// <param name="ea">Object that holds the event data.</param>
        private void AppUnhandledExceptionEventHandler(object sender, UnhandledExceptionEventArgs ea)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            Exception ex = ea.ExceptionObject as Exception;

            // Note: In a Debug build, the system logger writes the log message
            // to all debug listeners before placing the message in the log
            // message queue for logging.
            string strMessage = "Unhandled Exception! Server state: " + iServerState;
            if (ex != null)
            {
                // MySystemLogger.Log(strMessage, ex, "NeoSpinServer.LogUnhandledExceptionEventHandler");
            }
            else
            {
                // MySystemLogger.Log(strMessage, "NeoSpinServer.LogUnhandledExceptionEventHandler");
            }

            // Write to the Windows event log
            EventLog.WriteEntry("NeoSpin Service", strMessage, EventLogEntryType.Error);

            // Termination has begun. Resistance is futile! Let's do an
            // orderly shutdown before the CLR terminates the application.
            Stop();

            // TODO: Exit on AppUnhandledExceptionEventHandler is a only a test.
            Environment.Exit(5); // 5 was arbitrarily chosen for this 'Test'.
        }

        /// <summary>
        /// Configures NeoSpinMetaDataCache, NeoSpinDBCache and
        /// NeoSpinBusinessTier services, and loads the cache.
        /// </summary>
        private void StartServers()
        {
            // Get the configuration file path
            string strAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            HelperFunction.istrAppSettingsLocation = Path.Combine(strAssemblyLocation, "AppSettings.xml");

            srvMetaDataCache.LoadXMLCache();
            bool lblnSuccess = Sagitec.DBCache.srvDBCache.LoadCacheInfo();

            CompileRules();
            CorBookmarkHelper.CacheTemplateStdBookmarks();
            InitializeClassMapper();
            //StartThreadToRefreshDBSettings();
            // Dev name - Rahul mane
            // Date - 02-13-2025
            // Comment - Refresh server functionality Migrated From HEB
            utlCacheRefreshHelper.StartThreadToRefreshDBSettings();
            iobjRequestHandler = new busMQRequestHandler(utlConstants.SystemQueue);
            iobjRequestHandler.StartProcessing();
            //iBPMCommonService.InitializeTimers();
            ExtendedBPMService.Initialize(arguments);
            ExtendedBPMService.Instance?.Start();
        }

        /// <summary>
        /// Clears the cache, and stops the remoting services.
        /// </summary>
        private void StopServers()
        {
            if (iobjRequestHandler != null)
                iobjRequestHandler.StopProcessing();

            // Clear all the cache
            if (Sagitec.DBCache.srvDBCache.idctDBCache != null)
                Sagitec.DBCache.srvDBCache.idctDBCache.Clear();

            // Stop all remoting services
            foreach (IChannel objChannel in ChannelServices.RegisteredChannels)
                ChannelServices.UnregisterChannel(objChannel);
            //iBPMCommonService.StopTimers();
            ExtendedBPMService.Instance?.Stop();
        }

        /// <summary>
        /// A sample routine where you'll start your server work
        /// </summary>
        private void DoSomeWork()
        {
            // Do some work here. For example start listening to some event on
            // a serial port or start a tcp listener or start watching your
            // directories and files on your machine etc
            // Or wire up event listeners

            StartServers();
        }

        // *********************************************************************
        /// <summary>
        /// A sample routine where you'll start your server work
        /// </summary>
        private void StopDoingWork()
        {
            // Stop doing your sample work here. For example stop listening to the
            // event on your serial port or stop the tcp listener or stop watching your
            // directories and files on your machine etc. that you started in
            // DoSomeWork or un-wire your event listeners

            StopServers();
        }

        /// <summary>
        /// Starts the logging subsystem for the server, the server's system
        /// logger, and the audit logger.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Startup of the logging subsystem creates a static work queue that
        /// is used by all loggers.
        /// </para>
        /// <para>
        /// Important server process information is written to the system log
        /// at startup.
        /// </para>
        /// </remarks>
        private void StartLogging()
        {
            // Starup your custom logging here in some text file etc.
        }

        /// <summary>
        /// Startup the server subsystems.
        /// </summary>
        /// <exception cref="Exception">Exceptions are captured in <c>Startup</c>
        /// only to update state info or log additional details.
        /// The <see cref="Exception"/> must be rethrown so that the OS service
        /// control routines can handle it.</exception>
        private void Startup()
        {
            // Exit if not in "Starting" state
            if (this.iServerState != NeoSpinServerState.Starting)
                return;

            try
            {
                // Startup our logging.
                StartLogging();

                this.iServerState = NeoSpinServerState.Running;

                // Now start doing some work
                DoSomeWork();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("NeoSpin Service", "Error starting NeoSpin servers: " + ex.Message, EventLogEntryType.Error);

                throw;
            }
        }

        /// <summary>
        /// Shutdown the server subsystems.
        /// </summary>
        private void Shutdown()
        {
            // Exit if not in "Stopping" state
            if (this.iServerState != NeoSpinServerState.Stopping)
                return;

            try
            {
                StopDoingWork();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("NeoSpin Service", "Error stopping NeoSpin servers: " + ex.Message, EventLogEntryType.Error);
            }
            finally
            {
                this.iServerState = NeoSpinServerState.Stopped;
            }
        }

        /// <summary>
        /// Start the Server.
        /// </summary>
        /// <remarks>
        /// Start the subsystems needed to run the server.
        /// </remarks>
        public void Start()
        {
            if (this.iServerState == NeoSpinServerState.Stopped)
            {
                this.iServerState = NeoSpinServerState.Starting;

                lock (this.iStartStopLock)
                {
                    // DEVNOTE: We do not want any exceptions to be caught at
                    // this level because they are considered fatal and we want
                    // the service to shutdown so that it can be restarted.
                    Thread startup = new Thread(Startup)
                    {
                        Priority = ThreadPriority.AboveNormal,
                        Name = "NeoSpin Server Startup"
                    };
                    startup.Start();
                    startup.IsBackground = true;
                    startup.Join(SYSRESPONSE_INTERVAL);
                }
            }
        }

        /// <summary>
        /// Stops the Server.
        /// </summary>
        /// <remarks>
        /// Stops the server and its subsystems.
        /// </remarks>
        public void Stop()
        {
            if (this.iServerState == NeoSpinServerState.Starting || this.iServerState == NeoSpinServerState.Running)
            {
                this.iServerState = NeoSpinServerState.Stopping;

                lock (this.iStartStopLock)
                {
                    Thread shutdown = new Thread(Shutdown)
                    {
                        Priority = ThreadPriority.Highest,
                        Name = "NeoSpin Server Shutdown",
                        IsBackground = true
                    };
                    shutdown.Start();
                    shutdown.Join(SYSRESPONSE_INTERVAL);
                }
            }
        }

        private static void CompileRules()
        {
            try
            {
                StringBuilder lstrRulesError = new StringBuilder();
                ParsingResult lobjLoadResult = srvMainDB.LoadRulesAndExpressions();
                RulesEngine.CompileRules(false);
                RulesEngine.CompileRules(true);
            }
            catch
            {
            }
        }

        public static void InitializeClassMapper()
        {
            System.Reflection.Assembly lasmSrcAssembly = null;
            string lstrBusinessObjectAssembly = NeoBaseApplicationSettings.Instance.BusinessObjectAssembly;
            System.Reflection.Assembly lasmTargetAssembly = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name == lstrBusinessObjectAssembly).FirstOrDefault();
            if (lasmTargetAssembly.IsNotNull())
            {
                Collection<Type> lcolType = new Collection<Type>
                {
                    typeof(busBase),
                    typeof(doBase)
                };

                /// Developer : Tanaji Biradar
                /// Release : Iteration-9
                /// Date : 07th July 2021
                /// PIR : 3649,3652,3657 : System is unable to 'search' user records and 'navigate' to maintenance screen when on 'user lookup screen'

                List<utlCustomAssemblyInfo> llstCustomAssemblyInfo = (List<utlCustomAssemblyInfo>)HelperFunction.JsonDeserialize(SystemSettings.Instance.CustomAssemblyInfo, typeof(List<utlCustomAssemblyInfo>));

                foreach (utlCustomAssemblyInfo item in llstCustomAssemblyInfo)
                {
                    if (item != null)
                    {
                        lasmSrcAssembly = Assembly.LoadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, item.Assembly + ".dll"));
                        ClassMapper.LoadSolutionSideTypes(lasmSrcAssembly, lasmTargetAssembly, lcolType);
                    }
                }

                // Developer : Rahul Mane
                // Iteration : 8.2
                // Date : 06_14_2021
                // Comment - Change Related to Override busBPMService method at Solution side /Application side
                lasmSrcAssembly = typeof(ExtendedBPMService).Assembly;
                lcolType.Clear();
                lcolType.Add(typeof(ExtendedBPMService));
                ClassMapper.LoadSolutionSideTypes(lasmSrcAssembly, lasmTargetAssembly, lcolType);
            }
        }

        /// <summary>
        /// Method to start Refresh database cached settings thread. 
        /// once date got updated in sgs_act_log table REFRESH_DB_CACHE column to current date then cached data will be updated.
        /// Optional change if project team want to adopt this change then they need to set value of  "RefreshDBCache = true" in sgs_system_settings table.
        /// </summary>
        /// Developer Name: Tanaji Biradar.
        /// Date: 25-07-2023
        /// Changes made for: CCCERA Team.
        public static void StartThreadToRefreshDBSettings()
        {
            string lstrRefreshDBSettings = ConfigurationManager.AppSettings["RefreshDBCache"];
            bool lblnRefreshDBSettings = lstrRefreshDBSettings == null ? NeoBaseApplicationSettings.Instance.RefreshDBCache : Convert.ToBoolean(lstrRefreshDBSettings);

            if (lblnRefreshDBSettings)
            {
                utlCacheRefreshHelper.StartThreadToRefreshDBSettings();
            }
        }

        #endregion
    }
}