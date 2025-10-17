using System;
using MPIPHP.Common;
using MPIPHP.BusinessObjects;

namespace MPIPHP.Interface
{
    public interface IWorkflowEngine
    {
        WorkflowResult Run(int aintSystemRequestID, int aintProcessID, int aintPersonID, int aintOrgID, long aintReferenceID, string astrCreatedBy,int aintContactTicketID);       
        bool ResumeBookmark(Guid awinInstanceID, ActivityInstanceEventArgs aaieBookMarkValue);
        void Abort(Guid id, string reason);
        void Abort(Guid id);
        void Cancel(Guid id);
        void Terminate(Guid id, string reason);
    }
}
