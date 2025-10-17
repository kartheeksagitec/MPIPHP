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
using Sagitec.CorBuilder;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace MPIPHPJobService
{
    public class busUpdateCheckStatusBatch : busBatchHandler
    {
        #region Properties
      
        public Collection<busPaymentHistoryHeader> iclbPaymentHistoryHeader { get; set; }
        public Collection<busPaymentHistoryDistribution>iclbPaymentHistoryDistribution{get;set;}
              

        #endregion

        #region ACH_STATUS_UPDATE_DAILY_BATCH
        public void UpdateCheckStatusBatch()
        {
            try
            {
                this.ibusJobHeader.icdoJobHeader.iblnGeneratePdfFlag = busConstant.BOOL_TRUE;
                int lintrtn = 0;
                //1040: This Batch will update the status to STALE when the payment date range between 180 days and lessers than 3yrs.
                // If the Payment date is greater than 3 yrs status should changed as 3YRS.
                idlgUpdateProcessLog("Inserting Payment History distribution status history Records", "INFO", istrProcessName);
                lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryDistributionStatusHistory.PaymentStatusHistoryStale&3YearPlusStatus",
                          new object[1] { iobjPassInfo.istrUserID },
                                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                //1040: Generate correspondence when  check or payment days 150 between 180 days.
                DataTable ldtbLastPaymentDateby150days = busBase.Select("cdoPaymentHistoryDistributionStatusHistory.GetLastPaymentDatebetween150&180days", new object[0] { });
                iclbPaymentHistoryDistribution = new Collection<busPaymentHistoryDistribution>();

                if (ldtbLastPaymentDateby150days.Rows.Count > 0)
                {
                    foreach (DataRow ldrPaymentHistory in ldtbLastPaymentDateby150days.Rows)
                    {
                        ArrayList arrlist = new ArrayList();
                        Hashtable ahtbQueryBkmarks = new Hashtable();
                        busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                        lbusPayeeAccount.icdoPayeeAccount.LoadData(ldrPaymentHistory);
                        lbusPayeeAccount.ibusParticipant = new busPerson { icdoPerson = new cdoPerson() };
                        lbusPayeeAccount.ibusParticipant.icdoPerson.LoadData(ldrPaymentHistory);

                        iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
                        arrlist.Add(lbusPayeeAccount);
                        this.CreateCorrespondence(busConstant.IAP_OUTSTANDING_CHECKS, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, arrlist, ahtbQueryBkmarks, ablnIsPDF: true);
                    }

                    //1040: Merged file should copied under DMST and INTR folder under Current day directory.
                    String Todaysdate = DateTime.Now.ToString("dd-MMM-yyyy");

                    if (!Directory.Exists(iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Report_Path_STALE + Todaysdate))
                    {
                        Directory.CreateDirectory(iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Report_Path_STALE + Todaysdate);

                        Directory.CreateDirectory(iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Report_Path_STALE + Todaysdate + "\\" + "DMST");

                        Directory.CreateDirectory(iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Report_Path_STALE + Todaysdate + "\\" + "INTR");
                    }


                    MergePdfsFromPath(iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Correspondence_Path,
                              iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Report_Path_STALE + Todaysdate + "\\", busConstant.BOOL_FALSE, busConstant.BOOL_FALSE);

                    //PIR 1040: On successful pdf merge, update correspondence Generated flag in Payment History distribution table.
                    idlgUpdateProcessLog("Update Payment History distribution correspondence flag", "INFO", istrProcessName);
                    lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryDistributionStatusHistory.UpdatePaymentHistoryDistributionCorrespondenceFlag",
                              new object[1] { iobjPassInfo.istrUserID },
                                            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                }

            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
                iobjPassInfo.Rollback();
                idlgUpdateProcessLog("Error Occured with Message = " + e.Message, "INFO", istrProcessName);
            }
        }
            

        #endregion
       
    }
}
