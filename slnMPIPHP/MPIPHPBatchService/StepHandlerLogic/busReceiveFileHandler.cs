#region [Using Directives]
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.CorBuilder;
using Sagitec.Interface;
using MPIPHP.BusinessObjects;
using MPIPHP.Common;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
using MPIPHPJobService;
#endregion

namespace MPIPHPJobService
{
    /// <summary>
    /// Class to handle receiving of files
    /// </summary>
    public class busReceiveFileHandler : busBatchHandler
    {
        /// <summary>
        /// Process Files Instance.
        /// </summary>
        busProcessFiles ibusProcessFiles;
        
        /// <summary>
        /// File Id.
        /// </summary>
        private int iintFileId;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="aintFileId">File to be Received</param>
        /// <param name="abusSystemManagement">System Management</param>
        /// <param name="adlgUpdateProcessLog">Process Log Delegate Instance</param>
        /// <param name="aintCurrentCycleNo">Current Cycle No</param>
        public busReceiveFileHandler(int aintFileId, busSystemManagement abusSystemManagement, UpdateProcessLog adlgUpdateProcessLog, int aintCurrentCycleNo, utlPassInfo aobjPassInfo)
        {
            ibusProcessFiles = new busProcessFiles();
            ibusProcessFiles.iobjSystemManagement = abusSystemManagement;
            ibusProcessFiles.idlgUpdateProcessLog = new busProcessFiles.UpdateProcessLog(adlgUpdateProcessLog);
            ibusProcessFiles.iintCycleNo = aintCurrentCycleNo;
            iobjPassInfo = aobjPassInfo;
            iintFileId = aintFileId;
        }

        /// <summary>
        /// Receive Files
        /// </summary>
        public override void Process()
        {
            base.Process();

            if (iintFileId > 0)
               
               ibusProcessFiles.ReceiveAndValidate(iintFileId, true);
        }
    }
}
