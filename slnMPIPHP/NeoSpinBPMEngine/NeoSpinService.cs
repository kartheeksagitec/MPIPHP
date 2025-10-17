
using System.Diagnostics;

using System.ServiceProcess;

namespace NeoBPMN.Service
{
    public partial class NeoSpinService : ServiceBase
    {

        #region properties
        /// <summary>

        /// The main functional class for this service.
        /// </summary>
        private NeoSpinServer iNeoSpinServer;

       

        #endregion

        /// <summary>
        /// Performs basic initialization including creating the instance of
        /// <tt>NeoSpinServer</tt> which does the real work for this service.
        /// </summary>
        /// <seealso cref="NeoSpinServer"/>
        public NeoSpinService(string[] args)
        {
            // This call is required by the Component Designer.
            InitializeComponent();

            // Create the main server instance.
            iNeoSpinServer = new NeoSpinServer(args);
            

            this.EventLog.Source = "NeoSpin Service";
            this.EventLog.Log = "Application";

            if (!EventLog.SourceExists(this.EventLog.Source))
                EventLog.CreateEventSource(this.EventLog.Source, this.EventLog.Log);
        }

        /// <summary>
        /// OnStart: Put startup code here
        ///  - Start threads, get inital data, etc.
        /// </summary>
        /// <param name="args">Data passed by the start command.</param>
        protected override void OnStart(string[] args)
        {
            this.RequestAdditionalTime(10000);
            base.OnStart(args);

            // DEVNOTE: We want any exceptions thrown by Start to go back to our
            // caller so it will terminate properly or else the Windows SCM will
            // get very confused.
            iNeoSpinServer.Start();
        }

        /// <summary>
        /// OnStop: Put your stop code here
        /// - Stop threads, set final data, etc.
        /// </summary>
        protected override void OnStop()
        {
            iNeoSpinServer.Stop();

            base.OnStop();
        }

        /// <summary>
        /// OnPause: Put your pause code here
        /// - Pause working threads, etc.
        /// </summary>
        protected override void OnPause()
        {
            iNeoSpinServer.Stop();

            base.OnPause();
        }

        /// <summary>
        /// OnContinue: Put your continue code here
        /// - Un-pause working threads, etc.
        /// </summary>
        protected override void OnContinue()
        {
            iNeoSpinServer.Start();

            base.OnContinue();
        }

        /// <summary>
        /// OnShutdown(): Called when the System is shutting down
        /// - Put code here when you need special handling
        ///   of code that deals with a system shutdown, such
        ///   as saving special data before shutdown.
        /// </summary>
        protected override void OnShutdown()
        {
            iNeoSpinServer.Stop();

            base.OnShutdown();
        }

        /// <summary>
        /// OnCustomCommand(): If you need to send a command to your
        ///   service without the need for Remoting or Sockets, use
        ///   this method to do custom methods.
        /// </summary>
        /// <param name="command">Arbitrary Integer between 128 & 256</param>
        protected override void OnCustomCommand(int command)
        {
            //#  A custom command can be sent to a service by using this method:
            //#  int command = 128; //Some Arbitrary number between 128 & 256
            //#  ServiceController sc = new ServiceController("NameOfService");
            //#  sc.ExecuteCommand(command);
            base.OnCustomCommand(command);
        }

        /// <summary>
        /// Handles changes in the computer's power status for this service.
        /// </summary>
        /// <remarks>
        /// This method does the following actions based on <tt>powerStatus</tt>.
        /// <list type="bullet">
        /// <item>QuerySuspend => returns <tt>true</tt> to grant permission</item>
        /// <item>Suspend => calls <tt>iNeoSpinServer.Stop</tt></item>
        /// <item>Resume* => calls <tt>iNeoSpinServer.Start</tt></item>
        /// <item>else, does nothing.</item>
        /// </list>
        /// Except for QuerySuspend, the base class <tt>OnPowerEvent</tt> is
        /// also called.
        /// </remarks>
        /// <param name="powerStatus">A <tt>PowerBroadcastStatus</tt> that
        /// indicates a notification from the system about its power status.</param>
        /// <returns>Always returns <tt>true</tt>.</returns>
        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            switch (powerStatus)
            {
                case PowerBroadcastStatus.QuerySuspend:
                    // The system has requested permission to suspend the
                    // computer. We return true to grant our permission.
                    return true;

                case PowerBroadcastStatus.Suspend:
                    // The computer is about to enter a suspended state.
                    iNeoSpinServer.Stop();
                    break;

                case PowerBroadcastStatus.ResumeAutomatic:
                case PowerBroadcastStatus.ResumeCritical:
                case PowerBroadcastStatus.ResumeSuspend:
                    // The system has resumed operation after being suspended.
                    iNeoSpinServer.Start();
                    break;

                default:
                    break;
            }

            // Let the base class method know about this.
            base.OnPowerEvent(powerStatus);

            return true;
        }

        /// <summary>
        /// OnSessionChange(): To handle a change event from a Terminal Server session.
        ///   Useful if you need to determine when a user logs in remotely or logs off,
        ///   or when someone logs into the console.
        /// </summary>
        /// <param name="changeDescription"></param>
        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            base.OnSessionChange(changeDescription);
        }

    }
}