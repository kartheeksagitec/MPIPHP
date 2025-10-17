using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using System.Linq;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
using System.Data.SqlClient;
using Sagitec.DataObjects;
using System.Collections;
using Sagitec.CorBuilder;



namespace MPIPHP.BusinessObjects.PayeeAccount
{
    [Serializable]
    public class busCheckReconciliationService : busFileBase
    {
        bool iblnIsValid = false;
        //property to store the data read from file
        public busPaymentHistoryDistribution ibusPaymentHistoryDistribution { get; set; }
        //property to load the data which are in outstanding status
        public Collection<busPaymentHistoryDistribution> iclbPaymentHistoryDitribution { get; set; }
        public static CorBuilderXML iobjCorBuilder;

        public busCheckReconciliationService()
        {
            iclbPaymentHistoryDitribution = new Collection<busPaymentHistoryDistribution>();
        }

        public override void InitializeFile()
        {
            //Load the data which are in outstanding status
            busBase lobjBase = new busBase();
            DataTable ldtbOutStandingChecks = busBase.Select("cdoPaymentHistoryDistribution.fleGetOutstandingRecordsForCheckFile", new object[0] { });
            iclbPaymentHistoryDitribution = lobjBase.GetCollection<busPaymentHistoryDistribution>(ldtbOutStandingChecks, "icdoPaymentHistoryDistribution");
            ArrayList larrDeleteCheckNumber = new ArrayList();

            //Issue - Batch was failing because of Old check number in Alphanumberic format. Below Code removes old alphanumeric check numbers.  
            if (iclbPaymentHistoryDitribution != null && iclbPaymentHistoryDitribution.Count() > 0)
            {
                foreach (busPaymentHistoryDistribution lbusPaymentHistoryDistribution in iclbPaymentHistoryDitribution)
                {
                    if (lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.check_number.IsNotNullOrEmpty())
                    {
                        int lintTemp = 0;
                        bool result = Int32.TryParse(lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.check_number.Trim(), out lintTemp);
                        if (!result)
                            larrDeleteCheckNumber.Add(lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.check_number);
                    }

                }

                if (larrDeleteCheckNumber.Count > 0)
                {
                    foreach (string lstrCheckNumnber in larrDeleteCheckNumber)
                    {
                        iclbPaymentHistoryDitribution.Remove(iclbPaymentHistoryDitribution.Where(t => t.icdoPaymentHistoryDistribution.check_number == lstrCheckNumnber).FirstOrDefault());
                    }
                }
            }



            base.InitializeFile();
        }

        public override busBase NewDetail()
        {
            ibusPaymentHistoryDistribution = new busPaymentHistoryDistribution { icdoPaymentHistoryDistribution = new cdoPaymentHistoryDistribution() };
            return ibusPaymentHistoryDistribution;
        }
        //if statement date exists in file ,assign to transaction date
        public override string BeforeFieldAssigned(string astrFieldName, string astrFieldValue)
        {
            string lstrReturnValue = astrFieldValue;
            return lstrReturnValue;
        }

        public override void ProcessDetail()
        {

            if (ibusPaymentHistoryDistribution.istrRecordCode == busConstant.File.RECORD_CODE_ONE)
            {
                utlError lobjError = new utlError();
                ArrayList larrError = new ArrayList();
                Hashtable ahtbQueryBkmarks = new Hashtable();
                //if the file has check number column blank ,then throw an error

                if (string.IsNullOrEmpty(ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.check_number))
                {
                    lobjError = new utlError { istrErrorID = "8101", istrErrorMessage = "Invalid Check Number" };
                    larrError.Add(lobjError);
                    ibusPaymentHistoryDistribution.iarrErrors = larrError;
                    return;
                }//  
                else
                {
                    if (iclbPaymentHistoryDitribution != null && iclbPaymentHistoryDitribution.Count > 0)
                    {
                        busPaymentHistoryDistribution lobjPaymentHistoryDistribution = null;

                        if (iclbPaymentHistoryDitribution.Where(o =>
                                Convert.ToInt32(o.icdoPaymentHistoryDistribution.check_number) == Convert.ToInt32(ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.check_number)).Count() > 0)
                        {
                            //Get Distribution record for given check number
                            lobjPaymentHistoryDistribution = iclbPaymentHistoryDitribution.Where(o =>
                               Convert.ToInt32(o.icdoPaymentHistoryDistribution.check_number) == Convert.ToInt32(ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.check_number)).FirstOrDefault();
                        }

                        //if there is no record for given check number , then throw an error
                        if (lobjPaymentHistoryDistribution != null)
                        {
                            //if the validations succeed,update the check status to cleared and distribution Status history
                            if (ibusPaymentHistoryDistribution.iarrErrors.Count == 0 && (ibusPaymentHistoryDistribution.istrStatusCode == busConstant.File.PAID_CHECK
                                || ibusPaymentHistoryDistribution.istrStatusCode == busConstant.File.RECONCILED_CHECK
                                || ibusPaymentHistoryDistribution.istrStatusCode == busConstant.File.PAID_CHECK_CODE))
                            {
                                lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.CHECK_CLEARED;
                                lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.Update();

                                InsertIntoPaymentStatusHistory(lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_distribution_id,
                                    lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_header_id, busConstant.CHECK_CLEARED, DateTime.Now);
                            }
                            else if (ibusPaymentHistoryDistribution.iarrErrors.Count == 0 &&
                                (ibusPaymentHistoryDistribution.istrStatusCode == busConstant.File.Outstanding_Issue
                                || ibusPaymentHistoryDistribution.istrStatusCode == busConstant.File.Stale_Dated_Check))
                            {
                                DateTime idtCurrentDate = DateTime.Now;
                                TimeSpan ts = idtCurrentDate - lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.check_ach_effective_date;
                                decimal years = 0;
                                years = Convert.ToDecimal(ts.Days / 365.00);

                                if (years >= 3)
                                {
                                    lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.PAYMENT_DISTRIBUTION_STATUS_STALE;
                                    lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.Update();

                                    InsertIntoPaymentStatusHistory(lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_distribution_id,
                                        lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_header_id, busConstant.PAYMENT_DISTRIBUTION_STATUS_STALE, DateTime.Now);
                                }

                                ibusPaymentHistoryDistribution.istrCurrentDate = busGlobalFunctions.ConvertDateIntoDifFormat(System.DateTime.Now);
                                lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader = new busPaymentHistoryHeader { icdoPaymentHistoryHeader = new cdoPaymentHistoryHeader() };
                                lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.FindPaymentHistoryHeader(lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_header_id);
                                lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                                lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.FindPayeeAccount(lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id);
                                lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayee = new busPerson { icdoPerson = new cdoPerson() };
                                lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayee.FindPerson(lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.person_id);

                                if (lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.plan_id == busConstant.IAP_PLAN_ID)
                                {
                                    lobjPaymentHistoryDistribution.istrPlanDescription = busConstant.IAP_PLAN;
                                }
                                else if (lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.plan_id == busConstant.MPIPP_PLAN_ID)
                                {
                                    lobjPaymentHistoryDistribution.istrPlanDescription = busConstant.PENSION_PLAN;
                                }

                                larrError.Add(lobjPaymentHistoryDistribution);

                                //PIR 831
                                //Need to uncomment and test once corr got approved
                                //this.CreateCorrespondence(busConstant.OUTSTANDING_PAYMENT_LETTER, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, larrError, ahtbQueryBkmarks);
                            }
                        }
                        else
                        {
                            lobjError = new utlError { istrErrorID = "8102", istrErrorMessage = "There is no record of this check number in the system." };
                            larrError.Add(lobjError);
                            ibusPaymentHistoryDistribution.iarrErrors = larrError;
                            return;
                        }
                    }
                }
            }
        }
        public override bool ValidateFile()
        {
            iblnIsValid = base.ValidateFile();
            return iblnIsValid;
        }
        public override void FinalizeFile()
        {
            base.FinalizeFile();
        }

        public string CreateCorrespondence(string astrTemplateName, string astrUserID, int aintUserSerialID, ArrayList aarrResult, Hashtable ahtbQueryBkmarks)
        {
            utlCorresPondenceInfo lobjCorresPondenceInfo = busMPIPHPBase.SetCorrespondence(astrTemplateName,
                           iobjPassInfo.istrUserID, iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks);

            string lstrFileName = string.Empty;

            if (lobjCorresPondenceInfo == null)
            {
                throw new Exception("Unable to create correspondence, SetCorrespondence method not found in business solutions base object");
            }

            lobjCorresPondenceInfo.istrAutoPrintFlag = "N";


            foreach (utlBookmarkFieldInfo obj in lobjCorresPondenceInfo.icolBookmarkFieldInfo)
            {
                if (obj.istrDataType == "String" && !(string.IsNullOrEmpty(obj.istrValue)))
                    obj.istrValue = obj.istrValue.ToUpper();

            }

            try
            {

                iobjCorBuilder = new CorBuilderXML();
                iobjCorBuilder.InstantiateWord();
                lstrFileName = iobjCorBuilder.CreateCorrespondenceFromTemplate(astrTemplateName,
                    lobjCorresPondenceInfo, astrUserID);
                iobjCorBuilder.CloseWord();
            }
            catch (Exception e)
            {
                if (iobjCorBuilder != null)
                {
                    iobjCorBuilder.CloseWord();
                }
            }

            return lstrFileName;
        }

        #region Insert Into Payment Status History
        public void InsertIntoPaymentStatusHistory(int aintPaymentDistributionId, int aintPaymentHistoryHeaderId, string astrDistributionStatusValue, DateTime adtTransactionDate)
        {
            cdoPaymentHistoryDistributionStatusHistory lcdoPaymentHistoryDistributionStatusHistory = new cdoPaymentHistoryDistributionStatusHistory();
            lcdoPaymentHistoryDistributionStatusHistory.payment_history_distribution_id = aintPaymentDistributionId;
            lcdoPaymentHistoryDistributionStatusHistory.payment_history_header_id = aintPaymentHistoryHeaderId;
            lcdoPaymentHistoryDistributionStatusHistory.distribution_status_id = busConstant.PAYMENT_DISTRIBUTION_STATUS_ID;
            lcdoPaymentHistoryDistributionStatusHistory.distribution_status_value = astrDistributionStatusValue;
            lcdoPaymentHistoryDistributionStatusHistory.transaction_date = adtTransactionDate;
            lcdoPaymentHistoryDistributionStatusHistory.status_changed_by = utlPassInfo.iobjPassInfo.istrUserID;
            lcdoPaymentHistoryDistributionStatusHistory.Insert();
        }
        #endregion
    }
}
