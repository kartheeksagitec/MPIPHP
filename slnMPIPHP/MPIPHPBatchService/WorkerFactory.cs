using MPIPHP.BusinessObjects;

namespace MPIPHP.MPIPHPJobService
{
    /// <summary>
    /// Summary description for WorkerFactory.
    /// </summary>
    public class WorkerFactory
    {
        private WorkerFactory() { }

        public static IWorker GetJobHandler(busJobHeader aobjJobHeader, string astrWorkerName)
        {
            IWorker lobjJobHandler = null;
            if (aobjJobHeader.ibusCurrentJobDetail != null)
            {
                lobjJobHandler = new BatchHandler(aobjJobHeader,astrWorkerName);
            }
            return lobjJobHandler;
        }

        public static IWorker GetWorker(WorkerType wtype)
        {
            IWorker worker = null;
            switch (wtype)
            {
                case WorkerType.Master:
                    worker = JobMaster.GetInstance();
                    break;
                case WorkerType.Receiver:
                    worker = JobReceiver.GetInstance();
                    break;
                case WorkerType.Response:
                    worker = JobResponse.GetInstance();
                    break;
                case WorkerType.ScheduleAgent:
                    worker = JobScheduleAgent.GetInstance();
                    break;
                case WorkerType.Enqueue:
                    worker = JobEnqueue.GetInstance();
                    break;

                default:
                    return null;
            }
            return worker;

        }

    }
}
