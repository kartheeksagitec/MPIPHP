using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Data;
using Sagitec.BusinessObjects;
using Sagitec.DataObjects;
using Sagitec.DBUtility;



namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public class busPaymentProcess: busMPIPHPBase
    {
        int aintIncludeReissueItem = 0;
        int aintCheckStartdate = 0;
        public busPaymentProcess()
        {
        }
        public busPaymentProcess(bool IncludeReissueItem,bool CheckStartDate)
        {
            if(IncludeReissueItem)
            {
                aintIncludeReissueItem = 1;
            }
            if (!CheckStartDate)
            {
                aintCheckStartdate = 1;
            }
        }
        public Collection<busPaymentScheduleStep> iclbBatchScheduleSteps { get; set; }

        public void LoadBatchScheduleSteps(int aintPaymentScheduleId)
        {
            DataTable ldtbScheduleSteps = busBase.Select("cdoPaymentSchedule.LoadPaymentScheduleSteps", new object[1] { aintPaymentScheduleId });
            iclbBatchScheduleSteps =  GetCollection<busPaymentScheduleStep>(ldtbScheduleSteps, "icdoPaymentScheduleStep");
        }
        public int DeleteBackUpDataForCurrentScheduleId(int aintPaymentScheduleId)
        {
            int lintRtn = DBFunction.DBNonQuery("cdoPayeeAccount.DeletePrevBackUpForSamePaymentSchedule",
                                      new object[1] { aintPaymentScheduleId },
                                                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintRtn;
        }
        //Create Payment History Header for all the payee accounts considered for this payment process
        public int CreatePaymentHistoryHeader(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryHeader.createPaymentHistory", new object[3] { adtPaymentScheduleDate, 
                                        aintPaymentScheduleId,aintCheckStartdate}, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            lintrtn = lintrtn + DBFunction.DBNonQuery("cdoPaymentHistoryHeader.createPaymentHistoryForDeduction", new object[3] { adtPaymentScheduleDate, 
                                        aintPaymentScheduleId,aintCheckStartdate}, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        //Create Payment History details for all the payee accounts considered for this payment process
        public int CreatePaymentHistoryDetail(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryDetail.CreatePaymentHistoryDetail",
                                new object[3] { adtPaymentScheduleDate, aintPaymentScheduleId, aintCheckStartdate },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            lintrtn = lintrtn + DBFunction.DBNonQuery("cdoPaymentHistoryDetail.CreatePaymentHistoryDetailForDeduction",
                new object[3] { adtPaymentScheduleDate, aintPaymentScheduleId, aintCheckStartdate },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            DBFunction.DBNonQuery("cdoPaymentHistoryDetail.CreateWithHolding",
                new object[2] { adtPaymentScheduleDate, aintPaymentScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        //Create Check History details for all the payee accounts considered for this payment process
        public int CreateCheckHistoryforPayees(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, DateTime adtNextBenefitPaymentDate)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryDistribution.CreateCheckHistoryforPayees",
                new object[2] { adtPaymentScheduleDate,  aintPaymentScheduleId, },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            lintrtn = lintrtn + DBFunction.DBNonQuery("cdoPaymentHistoryDistribution.CreateCheckHistoryforPayeesForDeduction",
                new object[2] { adtPaymentScheduleDate, aintPaymentScheduleId, },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        //Create Check History details for all the payee accounts considered for this payment process
        public int CreateACHHistoryforPayees(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, DateTime adtNextBenefitPaymentDate)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryDistribution.CreateACHHistoryForPayees",
                new object[2] { aintPaymentScheduleId,  adtPaymentScheduleDate },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }
        public int CreateWIREHistoryforPayees(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, DateTime adtNextBenefitPaymentDate)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryDistribution.CreateWIREHistoryForPayees",
                new object[2] { aintPaymentScheduleId, adtPaymentScheduleDate },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }
        //Create Check History details for all the payee accounts considered for this payment process
        public int CreateRollOverACHHistoryforPayees(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryDistribution.CreateRolloverACHHistoryForPayees",
                new object[2] {adtPaymentScheduleDate, aintPaymentScheduleId  },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }
        //Get available number checks
        public int GetAvailableChecks(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate)
        {

            int lintrtn = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPaymentCheckBook.GetAvailableCheck", new object[2] { adtPaymentScheduleDate, aintPaymentScheduleId },
                                                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
            return lintrtn;
        }
        //Updating FBO_CO in payment distribution detail table, 
        public int UpdateCO(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryDistribution.UpdateCO",
                                     new object[2] { aintPaymentScheduleId,adtPaymentScheduleDate},
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }
        //Create Outstanding Distribution History Records
        public int CreateOutstandingHistoryRecords(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryDistribution.CreateOutstandingDistributionHistoryRecords",
                                     new object[1] { aintPaymentScheduleId },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }

        //Update History Distribution Status To Reissued
        public int UpdateHistoryDistributionStatusToReissued(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentReissueDetail.UpdateHistoryDistributionStatusToReissued",
                                     new object[1] { adtPaymentScheduleDate },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        //Update old History Distribution Id
        public int UpdateOldHistoryDistributionId(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentReissueDetail.UpdateOldHistoryDistributionId",
                                     new object[2] { adtPaymentScheduleDate, aintPaymentScheduleId },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        //create Repayment item every month
        public int CreateReimbursementDetail(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoRepaymentSchedule.CreateReimbursementDetail",
                                     new object[1] { aintPaymentScheduleId },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        //monthly batch - update the plan participation status to withdrawn if the benfit type is 'REFUND' and or Retired if the benefit type is 'Retirement' or
        //Pre -retirement death
        public int UpdatePersonAccountStatus(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPersonAccount.UpdatePersonAccountPlanParticipationStatus",
                new object[2] { adtPaymentScheduleDate, aintPaymentScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        //Reduce the paid amount from the member contributions for the member payee account
        //not for benefit type disability
        public int UpdateRetirementContributionForMember(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate,bool Monthly)
        {
            int lintrtn;
            if(Monthly)
                lintrtn = DBFunction.DBNonQuery("cdoPersonAccountRetirementContribution.UpdatePersonAccountRetirementContributionMONTHLY",
                    new object[1] {  aintPaymentScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            else
                lintrtn = DBFunction.DBNonQuery("cdoPersonAccountRetirementContribution.UpdatePersonAccountRetirementContributionForIAP",
                        new object[1] { aintPaymentScheduleId },
                                          iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);


            return lintrtn;
        }
        //Reduce the paid amount from the member contributions for the alternate payee's payee account
        //not for benefit type disability
        public int UpdateRetirementContributionForAltPayee(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPersonAccountRetirementContribution.UpdatePersonAccountRetirementContributionForAltPayee",
                new object[2] { adtPaymentScheduleDate, aintPaymentScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }
        //Updating Non-Taxable Amount, 
        public int UpdateNonTaxableAmount(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate)
        {
            int lintrtn = 0;
            lintrtn = DBFunction.DBNonQuery("cdoPayeeAccountPaymentItemType.UpdateNonTaxableAmount",
               new object[1] { adtPaymentScheduleDate },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }
        //update payee account status to processed for all the refund payee accounts considered for this payment process
        public int UpdatePayeeAccountStatus(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate)
        {

            int lintrtn = DBFunction.DBNonQuery("cdoPayeeAccountStatus.UpdatePayeeAccountStatus",
                new object[2] { adtPaymentScheduleDate, aintPaymentScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }
        //update SGT_REPAYMENT_SCHEDULE and payment item type associated
        public int UpdateRepaymentScedule(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate)
        {

            int lintrtn = DBFunction.DBNonQuery("cdoRepaymentSchedule.UpdateRepaymentAndPaymentItemType",
                new object[2] { adtPaymentScheduleDate, aintPaymentScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }
        //Updating payee status to Receiving / Processed for all the retirement payee account considered for monthly paymen process
        public int UpdatePayeeAccountStatustoPaymentComplete(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPayeeAccountStatus.UpdatePayeeAccountStatusComplete",
                new object[3] { adtPaymentScheduleDate, aintPaymentScheduleId,aintCheckStartdate },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }
        public int UpdatePayeeAccountStatustoPaymentCompleteIAP(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPayeeAccountStatus.UpdatePayeeAccountStatusCompleteIAP",
                new object[2] { adtPaymentScheduleDate, aintPaymentScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }
        public int UpdateAdhocFlag(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPayeeAccount.UpdateAdhocFlagWithBatch",
                new object[1] {  aintPaymentScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }
        //Method to update benefit end date for term certain payee account
        public int UpdateBenefitEndDateFromMonthlyForTermCertain(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPayeeAccount.UpdateBenefitEndDateWithTermCertainEndDate",
                                             new object[2] { adtPaymentScheduleDate, aintPaymentScheduleId },
                                                           iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }
        // Create Vendor Payment Summary for  batch
        public int CreateVendorPayments(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryDetail.CreateVendarPaymentFileRecords",
                new object[1] { aintPaymentScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }

        public int UpdateVendorPayments(int aintpLan_Id,int aintPaymentScheduleId, DateTime adtPaymentScheduleDate)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryDetail.UpdateVendorPayment",
                new object[3] { aintPaymentScheduleId,aintpLan_Id, adtPaymentScheduleDate },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }



        public int UpdatRetroPayment(DateTime adtPaymentScheduleDate, string PaymentOption, int aintPaymentScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPayeeAccountRetroPayment.UpdatRetroWithPayment",
                new object[3] { adtPaymentScheduleDate, PaymentOption, aintPaymentScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        public int UpdatRolloverPayment(DateTime adtPaymentScheduleDate,  int aintPaymentScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPayeeAccountRolloverDetail.UpdateRolloverDetailWithPayment",
               new object[2] { aintPaymentScheduleId, adtPaymentScheduleDate },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        //update payee account status to processed for all the refund payee accounts considered for this payment process
        public int UpdateBeginingBalance(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate)
        {

            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryHeader.UpdateBeginingBalance",
                new object[1] {  aintPaymentScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }
    }
}
