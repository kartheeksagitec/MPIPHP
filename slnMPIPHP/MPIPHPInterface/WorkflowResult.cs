using System;
using System.Activities;
using System.Collections.Generic;

namespace MPIPHP.Interface
{
    [Serializable]
    public class WorkflowResult
    {
        #region Data Members
        private WorkflowStatus _status;
        private Dictionary<string, object> _outputs;
        private Guid _instanceId;
        private Exception _exception;
        #endregion

        #region Properties
        public WorkflowStatus Status
        {
            get { return _status; }
        }

        public Dictionary<string, object> OutputParameters
        {
            get { return _outputs; }
        }

        public Guid InstanceId
        {
            get { return _instanceId; }
        }

        public int process_instance_id { get; set; }
        public Exception Exception
        {
            get { return _exception; }
        }
        #endregion

        #region Constructors/Destructors
        private WorkflowResult()
        {
        }

        private WorkflowResult(WorkflowApplicationCompletedEventArgs args)
        {
            ArgumentIsNotNull(args, "args");
            _outputs = args.Outputs as Dictionary<string, object>;
            _instanceId = args.InstanceId;
        }

        private WorkflowResult(WorkflowApplicationUnhandledExceptionEventArgs args)
        {
            ArgumentIsNotNull(args, "args");
            _instanceId = args.InstanceId;
            _exception = args.UnhandledException;
        }

        private WorkflowResult(WorkflowApplicationAbortedEventArgs args)
        {
            ArgumentIsNotNull(args, "args");
            _instanceId = args.InstanceId;
            _exception = args.Reason;
        }

        private WorkflowResult(Guid instanceID)
        {
            _instanceId = instanceID;
        }
        #endregion

        #region Static Methods
        public static WorkflowResult CreateCompletedWorkflowResults(WorkflowApplicationCompletedEventArgs args)
        {
            WorkflowResult results = new WorkflowResult(args);
            results._status = WorkflowStatus.Completed;
            return results;
        }

        public static WorkflowResult CreateTerminatedWorkflowResults(WorkflowApplicationUnhandledExceptionEventArgs args)
        {
            WorkflowResult results = new WorkflowResult(args);
            results._status = WorkflowStatus.Terminated;
            return results;
        }

        public static WorkflowResult CreateAbortedWorkflowResults(WorkflowApplicationAbortedEventArgs args)
        {
            WorkflowResult results = new WorkflowResult(args);
            results._status = WorkflowStatus.Aborted;
            return results;
        }

        //public static WorkflowResult CreateRunningWorkflowResults(WorkflowEventArgs args)
        //{
        //    WorkflowResult results = new WorkflowResult(args);
        //    results._status = WorkflowStatus.Running;
        //    return results;
        //}

        public static WorkflowResult CreateCreatedWorkflowResults(Guid instanceID)
        {
            WorkflowResult results = new WorkflowResult(instanceID);
            results._status = WorkflowStatus.Created;
            return results;
        }

        //public static WorkflowResult CreateUnloadedWorkflowResults(WorkflowEventArgs args)
        //{
        //    WorkflowResult results = new WorkflowResult(args);
        //    results._status = WorkflowStatus.Unloaded;
        //    return results;
        //}

        private void ArgumentIsNotNull(object argument, string name)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(name);
            }
        }
        #endregion
    }

    public enum WorkflowStatus
    {
        Created,
        Completed,
        Terminated,
        Aborted,
        Running,
        Unloaded
    }
}
