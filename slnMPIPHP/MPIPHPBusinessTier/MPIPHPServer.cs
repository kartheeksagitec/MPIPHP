using System;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Runtime.Remoting;
using Sagitec.MetaDataCache;
using Sagitec.Common;
using System.Runtime.Remoting.Channels;
using System.Diagnostics;

namespace MPIPHP.BusinessTier
{
    /// <summary>
    /// Main server application for my service.
    /// </summary>
    /// <remarks>
    /// <para>$Id$</para>
    /// <author>Authors: Sagitec</author>
    /// </remarks>
    public class MPIPHPServer
    {
        /// <summary>
        /// Definitions of the state codes for the server process.
        /// </summary>
        private enum MPIPHPServerState
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
        const int SYSRESPONSE_INTERVAL = 10000;

        /// <summary>
        /// The current process state.
        /// </summary>
        private volatile MPIPHPServerState iServerState;

        /// <summary>
        /// Lock object reserved for <see cref="Start"/> and <see cref="Stop"/>.
        /// </summary>
        private readonly object iStartStopLock = new object();

        /// <summary>
        /// Constructor.
        /// </summary>
        public MPIPHPServer()
        {
            this.iServerState = MPIPHPServerState.Stopped;

            // Create event handler to log an unhandled exception before the
            // server application is terminated by the CLR.
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(AppUnhandledExceptionEventHandler);
        }

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
                // MySystemLogger.Log(strMessage, ex, "MPIPHPServer.LogUnhandledExceptionEventHandler");
            }
            else
            {
                // MySystemLogger.Log(strMessage, "MPIPHPServer.LogUnhandledExceptionEventHandler");
            }

            // Write to the Windows event log
            EventLog.WriteEntry("MPIPHP Service", strMessage, EventLogEntryType.Error);

            // Termination has begun. Resistance is futile! Let's do an
            // orderly shutdown before the CLR terminates the application.
            Stop();

            // TODO: Exit on AppUnhandledExceptionEventHandler is a only a test.
            Environment.Exit(5); // 5 was arbitrarily chosen for this 'Test'.
        }

        /// <summary>
        /// Configures MPIPHPMetaDataCache, MPIPHPDBCache and
        /// MPIPHPBusinessTier services, and loads the cache.
        /// </summary>
        private void StartServers()
        {
            // Get the configuration file path
            string strAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            HelperFunction.istrAppSettingsLocation = Path.Combine(strAssemblyLocation, "AppSettings.xml");

            // Configure the remoting servers
            RemotingConfiguration.Configure(Path.Combine(strAssemblyLocation, "MPIPHPBusinessTier.exe.config"), false);

            // Load the MetaDataCache and DBCache
            srvMPIPHPMetaDataCache.LoadXMLCache();
            srvMPIPHPDBCache.LoadCacheInfo();
        }

        /// <summary>
        /// Clears the cache, and stops the remoting services.
        /// </summary>
        private void StopServers()
        {
            // Clear all the cache
            //FM upgrade: 6.0.0.30 changes
            //if (srvMPIPHPDBCache.idstDBCache != null)
            //    srvMPIPHPDBCache.idstDBCache.Clear();
            if (srvMPIPHPDBCache.idctDBCache != null)
                srvMPIPHPDBCache.idctDBCache.Clear();
            // Stop all remoting services
            foreach (IChannel objChannel in ChannelServices.RegisteredChannels)
                ChannelServices.UnregisterChannel(objChannel);
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
            if (this.iServerState != MPIPHPServerState.Starting)
                return;

            try
            {
                // Startup our logging.
                StartLogging();

                this.iServerState = MPIPHPServerState.Running;

                // Now start doing some work
                DoSomeWork();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("MPIPHP Service", "Error starting MPIPHP servers: " + ex.Message, EventLogEntryType.Error);

                throw;
            }

            //Debug.Print("MPIPHPServer.Startup has finished.");
        }

        /// <summary>
        /// Shutdown the server subsystems.
        /// </summary>
        private void Shutdown()
        {
            // Exit if not in "Stopping" state
            if (this.iServerState != MPIPHPServerState.Stopping)
                return;

            try
            {
                StopDoingWork();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("MPIPHP Service", "Error stopping MPIPHP servers: " + ex.Message, EventLogEntryType.Error);
            }
            finally
            {
                this.iServerState = MPIPHPServerState.Stopped;
            }

            //Debug.Print("MPIPHPServer.Shutdown has finished.");
        }

        /// <summary>
        /// Start the Server.
        /// </summary>
        /// <remarks>
        /// Start the subsystems needed to run the server.
        /// </remarks>
        public void Start()
        {
            if (this.iServerState == MPIPHPServerState.Stopped)
            {
                this.iServerState = MPIPHPServerState.Starting;

                lock (this.iStartStopLock)
                {
                    // DEVNOTE: We do not want any exceptions to be caught at
                    // this level because they are considered fatal and we want
                    // the service to shutdown so that it can be restarted.
                    Thread startup = new Thread(Startup);
                    startup.Priority = ThreadPriority.AboveNormal;
                    startup.Name = "MPIPHP Server Startup";
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
            if (this.iServerState == MPIPHPServerState.Starting || this.iServerState == MPIPHPServerState.Running)
            {
                this.iServerState = MPIPHPServerState.Stopping;

                lock (this.iStartStopLock)
                {
                    Thread shutdown = new Thread(Shutdown);
                    shutdown.Priority = ThreadPriority.Highest;
                    shutdown.Name = "MPIPHP Server Shutdown";
                    shutdown.IsBackground = true;
                    shutdown.Start();
                    shutdown.Join(SYSRESPONSE_INTERVAL);
                }
            }
        }
    }
}
