using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading;
using MPIPHP.BusinessObjects;
using Sagitec.BusinessObjects;

namespace MPIPHP.MPIPHPJobService.Core
{
    public class busLongStepForTesting: clsBatchHandlerBase<string>
    {
        public busLongStepForTesting(busJobDetail abusJobDetail
            , utlPassInfo aobjPassInfo
            , busSystemManagement abusSystemManagement
            , UpdateProcessLogDelegate adlgUpdateProcessLog
            , int aintCommitThreshold
            , int aintProgressPercentageReportThreshold
            , bool ablnContinueOnError
            , bool ablnUseTransaction
            )
            : base(abusJobDetail, aobjPassInfo, abusSystemManagement, adlgUpdateProcessLog, aintCommitThreshold, aintProgressPercentageReportThreshold, ablnContinueOnError, ablnUseTransaction)
        {
        }

        protected override Collection<string> GetDataList()
        {
            Collection<string> lclbTest = new Collection<string>();
            for (int i = 0; i < 200; i++)
            {
                lclbTest.Add(DateTime.Now.ToString());
            }
            return lclbTest;
        }

        protected override void OnProcessDataUnit(string aobjDataUnit)
        {
            Thread.Sleep(200);
        }
    }
}
