using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPIPHP.MPIPHPJobService
{
    [Serializable()]
    public enum WorkerType
    {
        Master,
        Receiver,
        Response,
        ScheduleAgent,
        Enqueue
    }
}
