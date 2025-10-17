using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using MPIPHP.CustomDataObjects;
using MPIPHP.BusinessObjects;
using MPIPHP.DataObjects;
using MPIPHP.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using System.Linq;
using MPIPHPJobService;
using Sagitec.CustomDataObjects;
using System.Data.SqlClient;
using System.Data.Sql;
using Sagitec.Common;
using System.Threading.Tasks;
using Sagitec.ExceptionPub;


namespace MPIPHPJobService
{
    public class busIAPRecalcSnapshotCleanupBatch : busBatchHandler
    {
        private object iobjLock = null;

        public void ProcessIAPFileCleanUp()
        {

            int lintCount = 0;
            int lintTotalCount = 0;
          
            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
            {
                ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
            }

            //Make a copy of original connection before starting the parallel loop which would be again used after completion of parallel loop
            iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
            utlPassInfo lobjMainPassInfo = iobjPassInfo;
            iobjLock = new object();

            DataTable ldtbReportData = busBase.Select("cdoIapRecalculationCopy.IAPRecalculateFileCleanUp", new object[0] {});


            //Initialize the parallel processing options, especially the max number of thread to be used for parallel processing
            ParallelOptions po = new ParallelOptions();
            po.MaxDegreeOfParallelism = 1;//System.Environment.ProcessorCount * 4;

            Parallel.ForEach(ldtbReportData.AsEnumerable(), po, (acdoPerson, loopState) =>
            {
                utlPassInfo lobjPassInfo = new utlPassInfo();
                lobjPassInfo.idictParams = ldictParams;
                lobjPassInfo.idictParams["ID"] = "IAP RECALCULATE FILE CLEANUP BATCH";
                lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                utlPassInfo.iobjPassInfo = lobjPassInfo;
                               
                DeleteIAPRecalculatePDFFile(acdoPerson);

                if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                {
                    lobjPassInfo.iconFramework.Close();
                }

                lobjPassInfo.iconFramework.Dispose();
                lobjPassInfo.iconFramework = null;


            });

            

            lobjMainPassInfo.iconFramework = DBFunction.GetDBConnection();
            utlPassInfo.iobjPassInfo = lobjMainPassInfo;
            DeletIAPRecalculationIAPOldData();
        }

        private void DeletIAPRecalculationIAPOldData()
        {
            iobjPassInfo.BeginTransaction();

            DBFunction.DBNonQuery("cdoIapRecalculationCopy.DeleteIAPRecalculationFile", new object[0] { }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            iobjPassInfo.Commit();
        }


    }
}
