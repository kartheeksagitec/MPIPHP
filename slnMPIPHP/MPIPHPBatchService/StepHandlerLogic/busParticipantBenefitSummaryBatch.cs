using System;
using System.Data;
using System.Collections.Generic;
using MPIPHP.CustomDataObjects;
using MPIPHP.BusinessObjects;
using MPIPHP.DataObjects;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using System.Linq;
using System.Threading.Tasks;
using Sagitec.ExceptionPub;

namespace MPIPHPJobService
{
    public class busParticipantBenefitSummaryBatch : busBatchHandler
    {

        #region Properties
        private object iobjLock = null;
        int lintCount = 0;
        int lintTotalCount = 0;
        int iintTempTable { get; set; }


        #endregion

        #region Participant Benefit Summary Batch
        public busParticipantBenefitSummaryBatch()
        {

        }
        #endregion


        public void ParticipantBenefitSummaryBatch()
        {
            iobjLock = new object();

            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
            {
                ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
            }

            iobjPassInfo.idictParams["ID"] = "ParticipantBenefitSummaryBatch";
            utlPassInfo lobjMainPassInfo = iobjPassInfo;

            RetrieveBatchParameters();

            DateTime ldtLastBatchProcessedDate = new DateTime();
            DataTable ldtblLastBatchProcessedDate = busBase.Select("cdoParticipantBenefitSummary.LastBatchProcessedDetails", new object[0]);
            DataTable ldtParticipants = new DataTable();
            cdoParticipantBenefitSummaryBatchRunDetail lcdoParticipantBenefitSummaryBatchRunDetail = new cdoParticipantBenefitSummaryBatchRunDetail();

            if (ldtblLastBatchProcessedDate != null && ldtblLastBatchProcessedDate.Rows.Count > 0
                && Convert.ToString(ldtblLastBatchProcessedDate.Rows[0]["LAST_RUN_DATE"]).IsNotNullOrEmpty()                
                )
            {
                ldtLastBatchProcessedDate = Convert.ToDateTime(ldtblLastBatchProcessedDate.Rows[0]["LAST_RUN_DATE"]).AddDays(1).Date;
                ldtParticipants = busBase.Select("cdoParticipantBenefitSummary.GetParticipantsForBenefitSummaryBatch", new object[2] { ldtLastBatchProcessedDate.Date, iintTempTable });
            }       
            
            if (ldtParticipants != null && ldtParticipants.Rows.Count > 0)
            {
                PostInfoMessage("Processing Participants with changes in Hours");

                ParallelOptions po = new ParallelOptions();
                po.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 4;

                Parallel.ForEach(ldtParticipants.AsEnumerable(), po, (acdoPerson, loopState) =>
                {
                    utlPassInfo lobjPassInfo = new utlPassInfo();
                    lobjPassInfo.idictParams = ldictParams;
                    lobjPassInfo.idictParams["ID"] = "ParticipantBenefitSummaryBatch";
                    lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                    utlPassInfo.iobjPassInfo = lobjPassInfo;

                    try
                    {
                        busPersonOverview lbusPersonOverview = new busPersonOverview { icdoPerson = new cdoPerson() };

                        if (lbusPersonOverview.FindPerson(Convert.ToInt32(acdoPerson[enmPerson.person_id.ToString().ToUpper()])))
                        {

                            lbusPersonOverview.LoadPlanDetails();

                            if (lbusPersonOverview != null && lbusPersonOverview.iclcdoPersonAccountOverview != null
                                && lbusPersonOverview.iclcdoPersonAccountOverview.Count > 0)
                            {
                                foreach (cdoPersonAccount lcdoPersonAccount in lbusPersonOverview.iclcdoPersonAccountOverview)
                                {

                                    cdoParticipantBenefitSummary lcdoParticipantBenefitSummary = new cdoParticipantBenefitSummary();

                                    lcdoParticipantBenefitSummary.plan_id = lcdoPersonAccount.plan_id;
                                    lcdoParticipantBenefitSummary.person_id = lbusPersonOverview.icdoPerson.person_id;
                                    lcdoParticipantBenefitSummary.mpi_person_id = lbusPersonOverview.icdoPerson.mpi_person_id;
                                    lcdoParticipantBenefitSummary.plan_name = lcdoPersonAccount.istrPlan;
                                    lcdoParticipantBenefitSummary.plan_status = lcdoPersonAccount.status_description;
                                    lcdoParticipantBenefitSummary.pension_hours = lcdoPersonAccount.istrTotalHours;
                                    lcdoParticipantBenefitSummary.qualified_years = lcdoPersonAccount.istrTotalQualifiedYears;
                                    lcdoParticipantBenefitSummary.pension_credit = lcdoPersonAccount.istrPensionCredit;
                                    lcdoParticipantBenefitSummary.vested_date = lcdoPersonAccount.dtVestedDate.Date;
                                    lcdoParticipantBenefitSummary.health_hours = lcdoPersonAccount.istrHealthHoursPO;
                                    lcdoParticipantBenefitSummary.health_years = lcdoPersonAccount.istrTotalHealthYearsPO;
                                    lcdoParticipantBenefitSummary.monthly_benefit = lcdoPersonAccount.idecTotalAccruedBenefit;
                                    lcdoParticipantBenefitSummary.iap_balance = lcdoPersonAccount.idecSpecialAccountBalance;
                                    lcdoParticipantBenefitSummary.allocation_as_of_yr_end = lcdoPersonAccount.istrAllocationEndYear;
                                    lcdoParticipantBenefitSummary.created_by = iobjPassInfo.istrUserID;
                                    lcdoParticipantBenefitSummary.update_benefit = 0;
                                    lcdoParticipantBenefitSummary.created_date = DateTime.Now;
                                    lcdoParticipantBenefitSummary.modified_by = iobjPassInfo.istrUserID;
                                    lcdoParticipantBenefitSummary.modified_date = DateTime.Now;
                                    lcdoParticipantBenefitSummary.Insert();

                                }
                            }

                            lock (iobjLock)
                            {
                                lintCount++;
                                lintTotalCount++;
                                if (lintCount == 100)
                                {
                                    String lstrMsg = lintTotalCount + " : " + " Records Has Been Processed";
                                    PostInfoMessage(lstrMsg);
                                    lintCount = 0;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        lock (iobjLock)
                        {
                            ExceptionManager.Publish(e);
                            String lstrMsg = "Error while Executing Batch,Error Message For MPID " + Convert.ToString(acdoPerson[enmPerson.mpi_person_id.ToString().ToUpper()]) + ":" + e.ToString();
                            PostErrorMessage(lstrMsg);
                        }
                    }

                    if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                    {
                        lobjPassInfo.iconFramework.Close();
                    }

                    lobjPassInfo.iconFramework.Dispose();
                    lobjPassInfo.iconFramework = null;

                });

                lobjMainPassInfo.iconFramework = DBFunction.GetDBConnection();
                utlPassInfo.iobjPassInfo = lobjMainPassInfo;
            }


            PostInfoMessage("Processing Participants with changes of Benefits in OPUS");

            int lintCnt = (int)DBFunction.DBExecuteScalar("cdoParticipantBenefitSummary.GetOPUSChangesForSummaryBatch", new object[2] { iobjPassInfo.istrUserID, ldtLastBatchProcessedDate.Date }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            PostInfoMessage("Successfully processed " + lintCnt + " Participant /Participants with changes of Benefits in OPUS");

            lcdoParticipantBenefitSummaryBatchRunDetail.last_run_date = ibusJobHeader.icdoJobHeader.start_time.AddDays(-1).Date;
            lcdoParticipantBenefitSummaryBatchRunDetail.batch_status_flag = busConstant.FLAG_YES;
            lcdoParticipantBenefitSummaryBatchRunDetail.Insert();
            
        }

        private void RetrieveBatchParameters()
        {
            if (ibusJobHeader != null)
            {
                if (ibusJobHeader.iclbJobDetail == null)
                    ibusJobHeader.LoadJobDetail(true);

                foreach (busJobDetail lobjDetail in ibusJobHeader.iclbJobDetail)
                {
                    foreach (busJobParameters lobjParam in lobjDetail.iclbJobParameters)
                    {
                        switch (lobjParam.icdoJobParameters.param_name)
                        {
                            case "RunForSpecificParticipants":
                                if (Convert.ToString(lobjParam.icdoJobParameters.param_value).IsNotNullOrEmpty()
                                    && Convert.ToString(lobjParam.icdoJobParameters.param_value).Trim() == busConstant.FLAG_YES)
                                {
                                    iintTempTable = 1;
                                }
                                else
                                {
                                    iintTempTable = 0;
                                }
                                break;
                        }
                    }
                }
            }
        }

    }
}







