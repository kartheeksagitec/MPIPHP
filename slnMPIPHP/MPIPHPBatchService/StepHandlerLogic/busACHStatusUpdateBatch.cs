using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
using Sagitec.DataObjects;
using System.Linq;
using Sagitec.CustomDataObjects;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Sql;
using Sagitec.ExceptionPub;
using MPIPHP.BusinessObjects;
using System.Threading.Tasks;
using Sagitec.Interface;

namespace MPIPHPJobService
{
    public class busACHStatusUpdateBatch : busBatchHandler
    {
        #region Properties
        private object iobjLock = null;
        public Collection<busPaymentHistoryHeader> iclbPaymentHistoryHeader { get; set; }
        public Collection<busPaymentHistoryDistribution>iclbPaymentHistoryDistribution{get;set;}
        #endregion

        #region ACH_STATUS_UPDATE_BATCH
        public void ACHStatusUpdateBatch()
        {
            try
            {
                iobjPassInfo.BeginTransaction();
                int lintrtn = 0;
                idlgUpdateProcessLog("Inserting Payment History distribution status history Records", "INFO", istrProcessName);
                lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryDistributionStatusHistory.CreatePaymentStatusHistoryACHClear",
                          new object[0] { },
                                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                idlgUpdateProcessLog("Updating Payment History distribution Records", "INFO", istrProcessName);
                lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryDistributionStatusHistory.UpdatePaymentHistoryDistributionACHClear",
                          new object[0] { },
                                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                iobjPassInfo.Commit();
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
                iobjPassInfo.Rollback();
                idlgUpdateProcessLog("Error Occured with Message = " + e.Message, "INFO", istrProcessName);
            }
        }
        #endregion
        #region ACH_STATUS_UPDATE_BATCH_OLD
        //public void ACHStatusUpdateBatch()
        //{
        //    Dictionary<string, object> ldictParams = new Dictionary<string, object>();
        //    busBase lobjBase = new busBase();
        //    foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
        //    {
        //        ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
        //    }
        //    //Make a copy of original connection before starting the parallel loop which would be again used after completion of parallel loop
        //    iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
        //    utlPassInfo lobjMainPassInfo = iobjPassInfo;
        //    iobjLock = new object();

        //    DataTable ldtbPaymentHistoryHeaderID = busBase.Select("cdoPaymentHistoryHeader.GetPaymentHistoryHeaderIDForACHStatusUpdateBatch", new object[0] { });

        //    if (ldtbPaymentHistoryHeaderID.Rows.Count > 0)
        //    {
        //        iclbPaymentHistoryHeader = lobjBase.GetCollection<busPaymentHistoryHeader>(ldtbPaymentHistoryHeaderID, "icdoPaymentHistoryHeader");
        //        ParallelOptions po = new ParallelOptions();
        //        po.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 4;

        //        Parallel.ForEach(iclbPaymentHistoryHeader, po, (lbusPaymentHistoryHeader, loopState) =>
        //        {
        //            utlPassInfo lobjPassInfo = new utlPassInfo();
        //            lobjPassInfo.idictParams = ldictParams;
        //            lobjPassInfo.idictParams["ID"] = "ACHStatusUpdateBatch";
        //            lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
        //            utlPassInfo.iobjPassInfo = lobjPassInfo;
                    
        //            DataTable ldtbGetPaymentHistoryDistributionData = busBase.Select("cdoPaymentHistoryHeader.GetPaymentHistoryDistributionIDFromHeaderID", new object[1] {lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_history_header_id });

        //            if (ldtbGetPaymentHistoryDistributionData.Rows.Count > 0)
        //            {
        //                iclbPaymentHistoryDistribution = lobjBase.GetCollection<busPaymentHistoryDistribution>(ldtbGetPaymentHistoryDistributionData, "icdoPaymentHistoryDistribution");

        //                foreach (busPaymentHistoryDistribution lbusPaymentHistoryDistribution in iclbPaymentHistoryDistribution)
        //                {
        //                    if (lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_header_id == lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_history_header_id)
        //                    {
        //                        lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.PAYMENT_DISTRIBUTION_STATUS_CLEARED;
        //                        lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.Update();
                                
        //                        busPaymentHistoryDistributionStatusHistory lbusPaymentHistoryDistributionStatusHistory = new busPaymentHistoryDistributionStatusHistory() { icdoPaymentHistoryDistributionStatusHistory = new cdoPaymentHistoryDistributionStatusHistory() };
        //                        lbusPaymentHistoryDistributionStatusHistory.icdoPaymentHistoryDistributionStatusHistory.payment_history_distribution_id = lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_distribution_id;
        //                        lbusPaymentHistoryDistributionStatusHistory.icdoPaymentHistoryDistributionStatusHistory.payment_history_header_id = lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_header_id;
        //                        lbusPaymentHistoryDistributionStatusHistory.icdoPaymentHistoryDistributionStatusHistory.distribution_status_value = busConstant.PAYMENT_DISTRIBUTION_STATUS_CLEARED; ;
        //                        lbusPaymentHistoryDistributionStatusHistory.icdoPaymentHistoryDistributionStatusHistory.transaction_date = DateTime.Now;
        //                        lbusPaymentHistoryDistributionStatusHistory.icdoPaymentHistoryDistributionStatusHistory.status_changed_by = "OPUS BATCH";
                                
        //                        lbusPaymentHistoryDistributionStatusHistory.icdoPaymentHistoryDistributionStatusHistory.created_by = "OPUS BATCH";
        //                        lbusPaymentHistoryDistributionStatusHistory.icdoPaymentHistoryDistributionStatusHistory.created_date = DateTime.Now;
        //                        lbusPaymentHistoryDistributionStatusHistory.icdoPaymentHistoryDistributionStatusHistory.modified_by = "OPUS BATCH";
        //                        lbusPaymentHistoryDistributionStatusHistory.icdoPaymentHistoryDistributionStatusHistory.modified_date = DateTime.Now;

        //                        lbusPaymentHistoryDistributionStatusHistory.icdoPaymentHistoryDistributionStatusHistory.Insert();
        //                    }
        //                }

        //            }
        //            if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
        //            {
        //                lobjPassInfo.iconFramework.Close();
        //            }

        //            lobjPassInfo.iconFramework.Dispose();
        //            lobjPassInfo.iconFramework = null;
        //        });
        //    }
        //    utlPassInfo.iobjPassInfo = lobjMainPassInfo;
        //}
        #endregion     
    }
}
