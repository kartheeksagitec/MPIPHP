#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Sagitec.BusinessTier;
using Sagitec.BusinessObjects;
using MPIPHP.Interface;
using Sagitec.Common;
using System.Collections;
using MPIPHP.BusinessObjects;
using System.Collections.ObjectModel;
using System.Data;
using Sagitec.ExceptionPub;
using Sagitec.CorBuilder;
using System.IO;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
using Sagitec.DBUtility;
#endregion

namespace MPIPHP.BusinessTier
{
    /// <summary>
    /// Summary description for srvPension.
    /// </summary>
    public abstract class srvMPIPHP : srvMainDBAccess, IMPIPHPBusinessTier
    {
        public srvMPIPHP()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public byte[] CreateMemberAnnualStatement(int aintPersonID)
        {
            busPerson lobjPerson = new busPerson();
            return lobjPerson.GenerateAnnualStatements(aintPersonID);

        }

		//LA Sunset - Payment Directives
        public byte[] GeneratePaymentDirective(int aintPayeeAccountId,string astrSpecialInstructions,DateTime adtAdhocPaymentDate, string astrModifiedBy)
        {
            srvPayeeAccount lsrvPayeeAccount = new srvPayeeAccount();
            busPayeeAccount lbusPayeeAccount = lsrvPayeeAccount.FindPayeeAccount(aintPayeeAccountId,ablnPaymentDirective:true);
            lbusPayeeAccount.istrSpecialInstructions = astrSpecialInstructions;
            lbusPayeeAccount.idtAdhocPaymentDate = adtAdhocPaymentDate;
            lbusPayeeAccount.istrModifiedBy = astrModifiedBy;

            return lbusPayeeAccount.GeneratePaymentDirective();
        }
		//LA Sunset - Payment Directives
        public byte[] RetrievePaymentDirective(int aintPayeeAccountId, string astrSpecialInstructions, DateTime adtAdhocPaymentDate, string astrModifiedBy, DateTime adtPaymentCycleDate,int aintDeletedPaymentDirectiveId = 0)
        {
            srvPayeeAccount lsrvPayeeAccount = new srvPayeeAccount();
            busPayeeAccount lbusPayeeAccount = lsrvPayeeAccount.FindPayeeAccount(aintPayeeAccountId, ablnPaymentDirective: true);
            lbusPayeeAccount.istrSpecialInstructions = astrSpecialInstructions;
            lbusPayeeAccount.idtAdhocPaymentDate = adtAdhocPaymentDate;
            lbusPayeeAccount.istrModifiedBy = astrModifiedBy;

            return lbusPayeeAccount.GeneratePaymentDirective(adtPaymentCycleDate, aintDeletedPaymentDirectiveId);
        }


        public byte[] MonthOfSuspendibleServiceReport(int aintPersonID)
        {
            busPersonOverview lbusPersonOverview = new busPersonOverview();
            lbusPersonOverview.FindPerson(aintPersonID);

            return lbusPersonOverview.MonthOfSuspendibleServiceReport(lbusPersonOverview.icdoPerson.person_id);
        }

        public byte[] OpenMonthOfSuspendibleServiceReport(int aintPersonID, int aintTrackingId)
        {
            busReturnToWorkRequest lbusReturnToWorkRequest = new busReturnToWorkRequest() { icdoReturnToWorkRequest = new cdoReturnToWorkRequest() };
            return lbusReturnToWorkRequest.OpenMonthOfSuspendibleServiceReport(aintTrackingId);
        }

        //FM upgrade: 6.0.0.31 changes      
        //public override object GetBusinessTierDetails()
        //{
        //    if (busMPIPHPBase.iutlServerDetail == null)
        //    {
        //        busMPIPHPBase lobjN = new busMPIPHPBase();
        //    }

        //    utlServerDetail lutlServerDetail = new utlServerDetail();
        //    lutlServerDetail.istrIPAddress = busMPIPHPBase.iutlServerDetail.istrIPAddress;
        //    lutlServerDetail.istrReleaseDate = busMPIPHPBase.iutlServerDetail.istrReleaseDate;

        //    return lutlServerDetail;
        //}

        //protected override void AddWhereClause(string astrFormName, Collection<utlWhereClause> acolWhereClause)
        //{
        //    if (acolWhereClause != null && acolWhereClause.Count > 0)
        //    {
        //        if (astrFormName == busConstant.PERSON_LOOKUP || astrFormName == busConstant.BENEFICIARY_LOOKUP)
        //        {
        //            foreach (utlWhereClause lutlWhereClause in acolWhereClause)
        //            {
        //               //if (lutlWhereClause.istrFieldName == busConstant.SSN)
        //               // {
        //               //     lutlWhereClause.iobjValue1 = HelperFunction.SagitecEncryptAES(lutlWhereClause.iobjValue1.ToString());

        //               // }
        //                if (lutlWhereClause.istrFieldName == enmPerson.date_of_birth.ToString())
        //                {
        //                    lutlWhereClause.iobjValue1 = HelperFunction.SagitecEncryptAES(Convert.ToString(Convert.ToDateTime(lutlWhereClause.iobjValue1)));
        //                }
        //            }
        //        }
        //        else if (astrFormName == busConstant.BENEFIT_LOOKUP || astrFormName == busConstant.DEATH_NOTIFICATION_LOOKUP || astrFormName ==busConstant.BenefitCalculation.BENEFIT_CALCULATION_LOOKUP)
        //        {
        //            foreach (utlWhereClause lutlWhereClause in acolWhereClause)
        //            {
        //                //if (lutlWhereClause.istrFieldName == busConstant.SP_SSN)
        //                //{
        //                //    lutlWhereClause.iobjValue1 = HelperFunction.SagitecEncryptAES(lutlWhereClause.iobjValue1.ToString());
        //                //}
        //                if (lutlWhereClause.istrFieldName == busConstant.SP_PREFIX + enmPerson.date_of_birth)
        //                {
        //                    lutlWhereClause.iobjValue1 = HelperFunction.SagitecEncryptAES(Convert.ToString(Convert.ToDateTime(lutlWhereClause.iobjValue1)));
        //                    lutlWhereClause.iobjValue2 = HelperFunction.SagitecEncryptAES(Convert.ToString(Convert.ToDateTime(lutlWhereClause.iobjValue2)));
        //                }

        //            }
        //        }

        //        if (astrFormName == busConstant.PERSON_LOOKUP ||
        //              astrFormName == busConstant.BENEFICIARY_LOOKUP || astrFormName == busConstant.SSN_MERGE_LOOKUP || astrFormName == busConstant.BenefitCalculation.BENEFIT_CALCULATION_LOOKUP
        //              || astrFormName == busConstant.BENEFIT_LOOKUP || astrFormName == busConstant.DEATH_NOTIFICATION_LOOKUP || astrFormName == busConstant.SSN_MERGE_HISTORY_LOOKUP)
        //        {
        //            foreach (utlWhereClause lutlWhereClause in acolWhereClause)
        //            {
        //                if (lutlWhereClause.istrFieldName == busConstant.SSN || lutlWhereClause.istrFieldName == busConstant.SP_SSN || lutlWhereClause.istrFieldName ==busConstant.OLD_SSN 
        //                    || lutlWhereClause.istrFieldName ==busConstant.NEW_SSN )
        //                {
        //                    iobjPassInfo.iconFramework.Open();
        //                    busSsnMergeHistory lbusSSMergeHistory = new busSsnMergeHistory();
        //                    lbusSSMergeHistory =
        //                        lbusSSMergeHistory.LoadSNNMergeHistory(HelperFunction.SagitecEncryptAES(lutlWhereClause.iobjValue1.ToString()));
        //                    iobjPassInfo.iconFramework.Close();

        //                    if (lbusSSMergeHistory != null && astrFormName != busConstant.SSN_MERGE_HISTORY_LOOKUP)
        //                    {

        //                        lutlWhereClause.iobjValue1 = lbusSSMergeHistory.icdoSsnMergeHistory.new_ssn;
        //                       // lutlWhereClause.iobjValue1 = HelperFunction.SagitecEncryptAES(lutlWhereClause.iobjValue1.ToString());
        //                    }
        //                    else
        //                    {
        //                        lutlWhereClause.iobjValue1 = HelperFunction.SagitecEncryptAES(lutlWhereClause.iobjValue1.ToString());
        //                    }
        //                }


        //                //if (lutlWhereClause.istrFieldName == enmPerson.date_of_birth.ToString())
        //                //{
        //                //    lutlWhereClause.iobjValue1 = HelperFunction.SagitecEncryptAES(Convert.ToString(Convert.ToDateTime(lutlWhereClause.iobjValue1)));
        //                //}

        //            }
        //        }


        //    }
        //}

        protected override void AddWhereClause(string astrFormName, Collection<utlWhereClause> acolWhereClause)
        {
            if (astrFormName == busConstant.PERSON_LOOKUP ||
                      astrFormName == busConstant.BENEFICIARY_LOOKUP || astrFormName == busConstant.SSN_MERGE_LOOKUP || astrFormName == busConstant.BenefitCalculation.BENEFIT_CALCULATION_LOOKUP
                      || astrFormName == busConstant.BENEFIT_LOOKUP || astrFormName == busConstant.DEATH_NOTIFICATION_LOOKUP)
            {
                foreach (utlWhereClause lutlWhereClause in acolWhereClause)
                {


                    if (lutlWhereClause.istrFieldName == busConstant.SSN || lutlWhereClause.istrFieldName == busConstant.SP_SSN)
                    {
                        iobjPassInfo.iconFramework.Open();
                        busSsnMergeHistory lbusSSMergeHistory = new busSsnMergeHistory();

                        lbusSSMergeHistory =
                            lbusSSMergeHistory.LoadSNNMergeHistory(Convert.ToString(lutlWhereClause.iobjValue1));
                        iobjPassInfo.iconFramework.Close();

                        if (lbusSSMergeHistory != null && astrFormName != busConstant.SSN_MERGE_HISTORY_LOOKUP)
                        {
                            lutlWhereClause.iobjValue1 = lbusSSMergeHistory.icdoSsnMergeHistory.new_ssn;
                        }
                    }
                    else if (lutlWhereClause.istrFieldName == busConstant.mpi_per_id || lutlWhereClause.istrFieldName == busConstant.sp_mpi_per_id)
                    {
                        iobjPassInfo.iconFramework.Open();
                        busSsnMergeHistory lbusSSMergeHistory = new busSsnMergeHistory();
                        lbusSSMergeHistory =
                            lbusSSMergeHistory.LoadSNNMergeHistoryByOldMPID(Convert.ToString(lutlWhereClause.iobjValue1));
                        iobjPassInfo.iconFramework.Close();

                        if (lbusSSMergeHistory != null && astrFormName != busConstant.SSN_MERGE_HISTORY_LOOKUP)
                        {
                            lutlWhereClause.iobjValue1 = lbusSSMergeHistory.icdoSsnMergeHistory.new_mpi_person_id;
                        }
                    }

                }
            }

        }

        public string EncryptPassword(string astrPassword)
        {
            return HelperFunction.SagitecEncryptAES(astrPassword);
        }

        public string DecryptPassword(string astrPassword)
        {
            return HelperFunction.SagitecDecryptAES(astrPassword);
        }

        public utlUserInfo ValidateUser(string astrUserId, string astrPassword)
        {
            return busUser.ValidateUser(astrUserId, astrPassword);
        }

        /// <summary>
        /// Generate correspondence, create the tracking record for the generated letter
        /// </summary>
        /// <param name="aintTemplateID"></param>
        /// <param name="aintPersonID"></param>
        /// <param name="astrUserId"></param>
        /// <param name="aarrResult"></param>
        /// <returns></returns>
        //FM upgrade: 6.0.0.29 changes - method signature changed
        public override string CreateCorrespondence(string astrTemplateName, object aobjBase, Hashtable ahtbQueryBkmarks, Dictionary<string, object> adictParams)
        {
            iobjPassInfo.idictParams = adictParams;

            try
            {
                BeginTransaction();

                CorBuilderXML lobjCorBuilder = null;
                try
                {
                    //Request ID: 64733
                    if (astrTemplateName == "PAYEE-0011" || astrTemplateName == "PAYEE-0014" || astrTemplateName == busConstant.IAP_SECOND_PAYMENT_LETTER )
                    {

                        if(aobjBase is busBenefitCalculationRetirement && (aobjBase as busBenefitCalculationRetirement).ibusRetirementApplication.IsNull())
                        {
                            (aobjBase as busBenefitCalculationRetirement).ibusRetirementApplication = new busRetirementApplication {icdoBenefitApplication = new cdoBenefitApplication()};
                            (aobjBase as busBenefitCalculationRetirement).ibusRetirementApplication.icdoBenefitApplication.person_id =
                                (aobjBase as busBenefitCalculationRetirement).icdoBenefitCalculationHeader.person_id;
                            DateTime ldtCurrentDate = System.DateTime.Now;
                            (aobjBase as busBenefitCalculationRetirement).istrCurrentDate = ldtCurrentDate.Date.ToShortDateString();

                            
                        }
                    }

                    if (astrTemplateName == busConstant.RETIREMENT_CANCELLATION_FORM || astrTemplateName == busConstant.CANCELLATION_NOTIFICATION)
                    {
                        if (iobjPassInfo.istrFormName == busConstant.BenefitCalculation.DISABILITY_CALCULATION_MAINTENANCE)
                        {
                            (aobjBase as busDisabiltyBenefitCalculation).ibusRetirementApplication = new busRetirementApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                            (aobjBase as busDisabiltyBenefitCalculation).ibusRetirementApplication.icdoBenefitApplication.person_id =
                                (aobjBase as busDisabiltyBenefitCalculation).icdoBenefitCalculationHeader.person_id;
                            DateTime ldtCurrentDate = System.DateTime.Now;
                            (aobjBase as busDisabiltyBenefitCalculation).istrCurrentDate = ldtCurrentDate.Date.ToShortDateString();

                            (aobjBase as busDisabiltyBenefitCalculation).ibusRetirementApplication.icdoBenefitApplication.retirement_date = (aobjBase as busDisabiltyBenefitCalculation).icdoBenefitCalculationHeader.retirement_date;

                            (aobjBase as busDisabiltyBenefitCalculation).ibusRetirementApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                            (aobjBase as busDisabiltyBenefitCalculation).ibusRetirementApplication.ibusPerson.FindPerson((aobjBase as busDisabiltyBenefitCalculation).icdoBenefitCalculationHeader.person_id);


                        }
                        else if (iobjPassInfo.istrFormName == busConstant.BenefitCalculation.RETIREMENT_CALCULATION_MAINTENACE)
                        {
                           
                            (aobjBase as busBenefitCalculationRetirement).ibusRetirementApplication = new busRetirementApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                            (aobjBase as busBenefitCalculationRetirement).ibusRetirementApplication.icdoBenefitApplication.person_id =
                                (aobjBase as busBenefitCalculationRetirement).icdoBenefitCalculationHeader.person_id;
                            DateTime ldtCurrentDate = System.DateTime.Now;
                            (aobjBase as busBenefitCalculationRetirement).istrCurrentDate = ldtCurrentDate.Date.ToShortDateString();

                            (aobjBase as busBenefitCalculationRetirement).ibusRetirementApplication.icdoBenefitApplication.retirement_date = (aobjBase as busBenefitCalculationRetirement).icdoBenefitCalculationHeader.retirement_date;

                            (aobjBase as busBenefitCalculationRetirement).ibusRetirementApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                            (aobjBase as busBenefitCalculationRetirement).ibusRetirementApplication.ibusPerson.FindPerson((aobjBase as busBenefitCalculationRetirement).icdoBenefitCalculationHeader.person_id);


                        }
                    }

                    //if(iobjPassInfo.istrFormName == busConstant.BenefitCalculation.RETIREMENT_CALCULATION_MAINTENACE)
                    //{
                    //    if (astrTemplateName == busConstant.RETIREMENT_APPLICATION_CANCELLATION_NOTICE)
                    //    {

                    //        (aarrResult[0] as busBenefitCalculationRetirement).ibusBenefitCalculationRetirement = new busBenefitCalculationRetirement { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
                    //        (aarrResult[0] as busBenefitCalculationRetirement).ibusPersonOverview = new busPersonOverview { icdoPerson = new cdoPerson() };
                    //        (aarrResult[0] as busBenefitCalculationRetirement).ibusPersonOverview.FindPerson((aarrResult[0] as busBenefitCalculationHeader).icdoBenefitCalculationHeader.person_id);
                    //        (aarrResult[0] as busBenefitCalculationRetirement).ibusPersonOverview.LoadPersonAddresss();
                    //        (aarrResult[0] as busBenefitCalculationRetirement).ibusPersonOverview.LoadPersonContacts();
                    //        (aarrResult[0] as busBenefitCalculationRetirement).ibusPersonOverview.LoadCorrAddress();
                    //        (aarrResult[0] as busBenefitCalculationRetirement).ibusPersonOverview.istrRetirement_Date = (aarrResult[0] as busBenefitCalculationHeader).icdoBenefitCalculationHeader.retirement_date.ToShortDateString();



                    //    }

                    //}else if(iobjPassInfo.istrFormName == busConstant.BenefitCalculation.DISABILITY_CALCULATION_MAINTENANCE)
                    //{
                    //    if (astrTemplateName == busConstant.RETIREMENT_APPLICATION_CANCELLATION_NOTICE)
                    //    {

                    //        (aarrResult[0] as busDisabiltyBenefitCalculation).ibusDisabiltyBenefitCalculation = new busDisabiltyBenefitCalculation { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
                    //        (aarrResult[0] as busDisabiltyBenefitCalculation).ibusPersonOverview = new busPersonOverview { icdoPerson = new cdoPerson() };
                    //        (aarrResult[0] as busDisabiltyBenefitCalculation).ibusPersonOverview.FindPerson((aarrResult[0] as busBenefitCalculationHeader).icdoBenefitCalculationHeader.person_id);
                    //        (aarrResult[0] as busDisabiltyBenefitCalculation).ibusPersonOverview.LoadPersonAddresss();
                    //        (aarrResult[0] as busDisabiltyBenefitCalculation).ibusPersonOverview.LoadPersonContacts();
                    //        (aarrResult[0] as busDisabiltyBenefitCalculation).ibusPersonOverview.LoadCorrAddress();
                    //        (aarrResult[0] as busDisabiltyBenefitCalculation).ibusPersonOverview.istrRetirement_Date = (aarrResult[0] as busBenefitCalculationHeader).icdoBenefitCalculationHeader.retirement_date.ToShortDateString();


                    //    }


                    //}
                  


                    //PIR Ticket : 63735
                    if (astrTemplateName == "PER-0003")
                    {
                        if (aobjBase is busPersonOverview && (aobjBase as busPersonOverview).ibusPerson.IsNull())
                        {
                            {
                                (aobjBase as busPersonOverview).ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                                (aobjBase as busPersonOverview).ibusPerson.FindPerson((aobjBase as busPersonOverview).icdoPerson.person_id);
                            }
                        }
                    }
                    //Ticket# 72507
                    if (astrTemplateName == "PERO-0004")
                    {
                        if (aobjBase is busPersonOverview && (aobjBase as busPersonOverview).ibusPerson.IsNull())
                        {
                            {
                                (aobjBase as busPersonOverview).ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                                (aobjBase as busPersonOverview).ibusPerson.FindPerson((aobjBase as busPersonOverview).icdoPerson.person_id);


                                var lbusPersonOverView = aobjBase as busPersonOverview;
                                var accruredBenefitAmount = 0.00m;
                                var EEUVHPContribution = 0.00m;
                                var IAPBalance = 0.00m;
                                lbusPersonOverView.ldecIsvested = "N";

                                foreach (var lbPersonAccountOverview in lbusPersonOverView.iclcdoPersonAccountOverview)
                                {
                                    if(lbPersonAccountOverview.idecTotalAccruedBenefit.ToString().IsDecimal())
                                    {
                                        accruredBenefitAmount += Convert.ToDecimal(lbPersonAccountOverview.idecTotalAccruedBenefit);
                                    }
                                    if(lbPersonAccountOverview.idecSpecialAccountBalance.ToString().IsDecimal())
                                    {
                                        

                                        IAPBalance += Convert.ToDecimal(lbPersonAccountOverview.idecSpecialAccountBalance);
                                    }
                                    
                                    foreach(var item in lbusPersonOverView.lbusBenefitApplication.aclbPersonWorkHistory_MPI)
                                    {
                                        EEUVHPContribution += item.idecEEContribution;
                                        EEUVHPContribution += item.idecEEInterest;
                                        EEUVHPContribution += item.idecUVHPContribution;
                                        EEUVHPContribution += item.idecUVHPInterest;
                                    }

                                    if(lbPersonAccountOverview.istrPlanCode == "MPIPP" && lbPersonAccountOverview.istrVested == true)
                                    {
                                        lbusPersonOverView.ldecIsvested = "Y";
                                    }
                                    if (lbPersonAccountOverview.istrPlanCode.Contains("IAP") && lbPersonAccountOverview.istrAllocationEndYear.ToString().IsNumeric())
                                    {
                                        lbusPersonOverView.iintYear = Convert.ToInt32(lbPersonAccountOverview.istrAllocationEndYear);
                                    }

                                }
                               
                                lbusPersonOverView.ldtLastReportedDate = GetLastWorkingDate(lbusPersonOverView.icdoPerson.istrSSNNonEncrypted);
                                lbusPersonOverView.ldecTotalAccuruedBftAmt = Convert.ToString(accruredBenefitAmount);
                                lbusPersonOverView.ldecTotalEEUVHPAmt = Convert.ToString(EEUVHPContribution);
                                lbusPersonOverView.ldecTotalSpecialAmt = Convert.ToString(IAPBalance);
                            }
                        }
                    }

                    string lstrFileName = string.Empty;
                    if (astrTemplateName == "RETR-0045")
                    {
                        string istrBenName = string.Empty;
                        string istrBenNamePrefix = string.Empty;
                        string istrBeneLastName = string.Empty;

                        busDeathNotification lbusDeathNotification = null;
                        if (aobjBase is busDeathNotification)
                        {
                            lbusDeathNotification = aobjBase as busDeathNotification;
                        }
                        lbusDeathNotification.ibusPerson.LoadBeneficiaries();
                        foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in lbusDeathNotification.ibusPerson.iclbPersonAccountBeneficiary)
                        {
                            lstrFileName = string.Empty;
                            if (lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.iaintPlan == busConstant.MPIPP_PLAN_ID
                                && lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.beneficiary_type_value == "PRIM")
                            {
                                if (lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.idtBenDateOfDeath == DateTime.MinValue)
                                {
                                    istrBenName = lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.istrBenFullName.ToProperCase();
                                    if (lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.istrNamePrefixValue.IsNotNullOrEmpty())
                                        istrBenNamePrefix = lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.istrNamePrefixValue;
                                    istrBeneLastName = lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.istrBenLastName.ToProperCase();

                                    lbusDeathNotification.sample(istrBenName, istrBenNamePrefix, istrBeneLastName);

                                    utlCorresPondenceInfo lobjCorresPondenceInfo = busMPIPHPBase.SetCorrespondence(astrTemplateName,
                                    iobjPassInfo.istrUserID, iobjPassInfo.iintUserSerialID, new ArrayList() { aobjBase as busBase }, ahtbQueryBkmarks);

                                    lobjCorresPondenceInfo.istrAutoPrintFlag = "N";

                                    if (lobjCorresPondenceInfo == null)
                                    {
                                        throw new Exception("Unable to create correspondence, SetCorrespondence method not found in " +
                                            " business solutions base object");
                                    }

                                    lobjCorBuilder = new CorBuilderXML();
                                    lobjCorBuilder.InstantiateWord();
                                    lstrFileName = lobjCorBuilder.CreateCorrespondenceFromTemplate(astrTemplateName, lobjCorresPondenceInfo, iobjPassInfo.istrUserID);
                                    lobjCorBuilder.CloseWord();
                                }
                            }
                        }
                        Commit();
                        return lstrFileName;
                    }
                    else if (astrTemplateName == "WIDRWL-0007" || astrTemplateName == busConstant.IAP_Withdrawal_Packet)
                    {
                        string lstrPlan = string.Empty;
                        string lstrSubPlan = string.Empty;
                        string lstrIsSpecialAcnt = busConstant.FLAG_NO;
                        string lstrIsIAPSpecial = busConstant.FLAG_NO;
                        busBenefitCalculationWithdrawal lbusBenefitCalculationWithdrawal = null;

                        if (aobjBase is busBenefitCalculationWithdrawal)
                        {
                            lbusBenefitCalculationWithdrawal = new busBenefitCalculationWithdrawal { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
                            lbusBenefitCalculationWithdrawal.iclbBenefitCalculationDetail = new Collection<busBenefitCalculationDetail>();

                            lbusBenefitCalculationWithdrawal = aobjBase as busBenefitCalculationWithdrawal;
                        }

                        foreach (busBenefitCalculationDetail lbusBenefitCalculationDetail in lbusBenefitCalculationWithdrawal.iclbBenefitCalculationDetail)
                        {
                            if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.plan_id == busConstant.IAP_PLAN_ID)
                            {
                                lstrPlan = busConstant.FLAG_YES;

                                if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l52_spl_acc_flag == busConstant.FLAG_YES)
                                {
                                    //lstrSubPlan = lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.;
                                    lstrIsSpecialAcnt = busConstant.FLAG_YES;
                                }
                                else if (lbusBenefitCalculationDetail.icdoBenefitCalculationDetail.l161_spl_acc_flag == busConstant.FLAG_YES)
                                {
                                    //lstrSubPlan = lbusBenefitApplicationDetail.istrSubPlanDescription;
                                    lstrIsSpecialAcnt = busConstant.FLAG_YES;
                                }

                                if (lstrPlan == busConstant.FLAG_YES && lstrIsSpecialAcnt == busConstant.FLAG_NO)
                                    lstrIsIAPSpecial = busConstant.FLAG_YES;

                                lbusBenefitCalculationWithdrawal.SetSubPlanDescription(lstrIsIAPSpecial, lstrSubPlan, lstrIsSpecialAcnt);

                                utlCorresPondenceInfo lobjCorresPondenceInfo = busMPIPHPBase.SetCorrespondence(astrTemplateName,
                                       iobjPassInfo.istrUserID, iobjPassInfo.iintUserSerialID, new ArrayList() { aobjBase as busBase }, ahtbQueryBkmarks);

                                lobjCorresPondenceInfo.istrAutoPrintFlag = "N";

                                if (lobjCorresPondenceInfo == null)
                                {
                                    throw new Exception("Unable to create correspondence, SetCorrespondence method not found in " +
                                        " business solutions base object");
                                }



                                lobjCorBuilder = new CorBuilderXML();
                                lobjCorBuilder.InstantiateWord();
                                lstrFileName = lobjCorBuilder.CreateCorrespondenceFromTemplate(astrTemplateName, lobjCorresPondenceInfo, iobjPassInfo.istrUserID);
                                lobjCorBuilder.CloseWord();
                               if(astrTemplateName == busConstant.IAP_Withdrawal_Packet)
                                {
                                    if (lstrFileName.IsNotNullOrEmpty())
                                        InsertIntoCorPacketContentTracking(lobjCorresPondenceInfo.iintCorrespondenceTrackingId, aobjBase);

                                }
                               


                            }
                        }

                        Commit();
                        return lstrFileName;
                    }
                    else
                    {
                        bool lblnIsMss = false;
                        if (astrTemplateName == busConstant.MSS.WORK_HISTORY_REQUEST_MSS)
                        {
                            lblnIsMss = true;
                            astrTemplateName = busConstant.WORK_HISTORY_REQUEST;
                        }
                        else if (astrTemplateName == busConstant.MSS.MSS_PENSION_IAP_VERIFICATION)
                        {
                            lblnIsMss = true;
                            astrTemplateName = busConstant.PENSION_AND_IAP_VERIFICATION;
                        }
                        else if (astrTemplateName == busConstant.MSS.MSS_PENSION_INCOME_VERIFICATION)
                        {
                            lblnIsMss = true;
                            astrTemplateName = busConstant.PENSION_INCOME_VERIFICATION;
                        }

                        utlCorresPondenceInfo lobjCorresPondenceInfo = busMPIPHPBase.SetCorrespondence(astrTemplateName,
                            iobjPassInfo.istrUserID, iobjPassInfo.iintUserSerialID, new ArrayList() { aobjBase as busBase }, ahtbQueryBkmarks);
                        if (lblnIsMss)
                        {
                            utlBookmarkFieldInfo lobjField = new utlBookmarkFieldInfo();
                            lobjField.istrName = "stdLoggedInUserFullName";
                            lobjField.istrValue = "MPI";
                            bool lblnExists = false;
                            foreach (utlBookmarkFieldInfo lobj in lobjCorresPondenceInfo.icolBookmarkFieldInfo)
                            {
                                if (lobj.istrName == lobjField.istrName)
                                {
                                    lblnExists = true;
                                    lobj.istrValue = "MPI";
                                }
                            }
                            if (!lblnExists)
                            {
                                lobjCorresPondenceInfo.icolBookmarkFieldInfo.Add(lobjField);
                            }
                        }
                        //During the ondemand generation of the correspondence - suppress the auto print of the letter just in case
                        lobjCorresPondenceInfo.istrAutoPrintFlag = "N";

                        if (lobjCorresPondenceInfo == null)
                        {
                            throw new Exception("Unable to create correspondence, SetCorrespondence method not found in " +
                                " business solutions base object");
                        }

                        lobjCorBuilder = new CorBuilderXML();
                        lobjCorBuilder.InstantiateWord();
                        foreach (utlBookmarkFieldInfo obj in lobjCorresPondenceInfo.icolBookmarkFieldInfo)
                        {
                            if (obj.istrDataType == "String" && !(string.IsNullOrEmpty(obj.istrValue)))
                            {
                                if (obj.istrObjectField == "istrAddrLine1" || obj.istrObjectField == "istrAddrLine2" || obj.istrObjectField == "istrAddrLine3" ||
                                    obj.istrObjectField == "istrCountryDescription" || obj.istrObjectField == "istrState" || obj.istrObjectField == "istrCity" ||
                                    obj.istrObjectField == "istrZipCode" || obj.istrObjectField == "istrRecepientName" || obj.istrObjectField == "ibusAlternatePayee.icdoPerson.istrFullName" ||
                                    obj.istrObjectField == "ibusPayeeAccount.istrBeneficiaryFullName" || obj.istrObjectField == "icdoPayeeAccount.istrParticipantName" ||
                                    obj.istrObjectField == "ibusParticipant.ibusPersonCourtContact.icdoPersonContact.contact_name" ||
                                    obj.istrObjectField == "ibusAlternatePayee.ibusPersonAddressForCorr.icdoPersonAddress.addr_line_1" ||
                                    obj.istrObjectField == "ibusAlternatePayee.ibusPersonAddressForCorr.icdoPersonAddress.addr_line_2" ||
                                    obj.istrObjectField == "ibusAlternatePayee.ibusPersonAddressForCorr.icdoPersonAddress.addr_city" ||
                                    obj.istrObjectField == "ibusAlternatePayee.ibusPersonAddressForCorr.icdoPersonAddress.addr_state_value" ||
                                    obj.istrObjectField == "ibusAlternatePayee.ibusPersonAddressForCorr.icdoPersonAddress.istrCompleteZipCode" ||
                                    obj.istrObjectField == "ibusAlternatePayee.ibusPersonAddressForCorr.icdoPersonAddress.foreign_postal_code" ||
                                    obj.istrObjectField == "ibusAlternatePayee.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_description" ||
                                    obj.istrObjectField == "ibusParticipant.ibusPersonCourtContact.icdoPersonContact.addr_line_1" ||
                                    obj.istrObjectField == "ibusParticipant.ibusPersonCourtContact.icdoPersonContact.addr_line_2" ||
                                    obj.istrObjectField == "ibusParticipant.ibusPersonCourtContact.icdoPersonContact.addr_city" ||
                                    obj.istrObjectField == "ibusParticipant.ibusPersonCourtContact.icdoPersonContact.addr_state_value" ||
                                    obj.istrObjectField == "ibusParticipant.ibusPersonCourtContact.icdoPersonContact.istrCompleteZipCode" ||
                                    obj.istrObjectField == "ibusParticipant.ibusPersonCourtContact.icdoPersonContact.foreign_postal_code" ||
                                    obj.istrObjectField == "ibusParticipant.ibusPersonCourtContact.icdoPersonContact.addr_country_description" ||
                                    obj.istrObjectField == "istrReportedBy" || obj.istrObjectField == "istrDistributionType" ||
                                    obj.istrObjectField == "istrSurvivorFullName" || obj.istrObjectField == "ibusBeneficiary.icdoPerson.istrFullName" ||
                                    obj.istrObjectField == "ibusBeneficiary.icdoPerson.istrFullName" ||
                                    obj.istrObjectField == "ibusBeneficiary.ibusPersonAddressForCorr.icdoPersonAddress.addr_line_1" ||
                                    obj.istrObjectField == "ibusBeneficiary.ibusPersonAddressForCorr.icdoPersonAddress.addr_line_2" ||
                                    obj.istrObjectField == "ibusBeneficiary.ibusPersonAddressForCorr.icdoPersonAddress.addr_city" ||
                                    obj.istrObjectField == "ibusBeneficiary.ibusPersonAddressForCorr.icdoPersonAddress.addr_state_value" ||
                                    obj.istrObjectField == "ibusBeneficiary.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_description" ||
                                    obj.istrObjectField == "ibusBeneficiary.ibusPersonAddressForCorr.icdoPersonAddress.foreign_province" ||
                                    obj.istrObjectField == "ibusBeneficiary.ibusPersonAddressForCorr.icdoPersonAddress.addr_zip_code" ||
                                    obj.istrObjectField == "istrCurrentTime" ||
                                    obj.istrObjectField == "istrEmployerName" ||
                                    obj.istrObjectField == "istrAddress1" ||
                                    obj.istrObjectField == "istrCity1" ||
                                    obj.istrObjectField == "istrState" ||
                                    obj.istrObjectField == "istrPostalCode" ||
                                    obj.istrObjectField == "istrStreet" ||
                                    obj.istrObjectField == "istrAddress2" ||
                                    obj.istrObjectField == "istrApprovedByUserInitials" || obj.istrObjectField == "istrPlan" ||
                                    obj.istrObjectField == "ibusParticipant.ibusPersonCourtContact.icdoPersonContact.county" ||
                                    obj.istrObjectField == "icdoDroApplication.case_number" || obj.istrObjectField == "icdoPayeeAccount.istrPlanCode"||obj.istrObjectField == "istrPayeeFullName"
                                    || obj.istrObjectField == "istrRespondent"  //PIR RID 73760
                                    || obj.istrObjectField == "ibusBenefitApplication.ibusPerson.icdoPerson.istrFullName" //PIR RID 78630
                                    || obj.istrObjectField == "istrSurvivorAdrCorStreet1" //PIR RID 78592
                                    || obj.istrObjectField == "istrSurvivorAdrCorStreet2" //PIR RID 78592
                                    || obj.istrObjectField == "istrSurvivorAdrCountryDesc" //PIR RID 78592
                                    || obj.istrObjectField == "istrStdPayeeFullName" //rid 81162
                                    || obj.istrObjectField == "istrPayeeDomesticStateIntlCountry" //rid 81162
                                  )
                                {
                                    obj.istrValue = obj.istrValue.ToUpper();
                                }
                                else
                                {
                                    if (astrTemplateName == busConstant.QDRO_STATUS_PENDING && obj.istrObjectField == "istrEmailAddr")
                                    {
                                        obj.istrValue = obj.istrValue.ToLower();
                                    }
                                    else
                                    //Ticket#129961
                                        if (astrTemplateName != busConstant.IAP_ANNUITY_QUOTE_CONFIRMATION_OF_ANNUITY_PAYMENT_TO_PAYEE)
                                        obj.istrValue = obj.istrValue.ToProperCase();
                                }
                            }
                        }

                        lstrFileName = lobjCorBuilder.CreateCorrespondenceFromTemplate(astrTemplateName, lobjCorresPondenceInfo, iobjPassInfo.istrUserID);

                        if (lstrFileName.IsNotNullOrEmpty())
                            InsertIntoCorPacketContentTracking(lobjCorresPondenceInfo.iintCorrespondenceTrackingId, aobjBase);

                        lobjCorBuilder.CloseWord();
                        Commit();
                        return lstrFileName;
                    }
                }

                catch (Exception e)
                {
                    //modified by deepak and rachit on 04-22-2009 with FPPA Muralidharan 
                    //FPPA pir 101162, if the network drive where the correspondence is getting generated goes down then the 
                    //word instance does not get closed. below code will ensure that word instance gets closed.
                    if (lobjCorBuilder != null)
                    {
                        try
                        {
                            lobjCorBuilder.CloseWord();
                        }
                        catch
                        {
                        }
                    }
                    Rollback();
                    string lstrMessage = "Failed to CreateCorrespondence, following error occured : " + e.Message;
                    if (e.InnerException != null)
                    {
                        lstrMessage += " Inner Exception : " + e.InnerException.Message;
                    }
                    throw new Exception(lstrMessage);
                }
            }
            finally
            {
                if ((iobjPassInfo.iconFramework != null) &&
                    (iobjPassInfo.iconFramework.State == ConnectionState.Open))
                {
                    iobjPassInfo.iconFramework.Close();
                    iobjPassInfo.iconFramework.Dispose();
                }

            }

        }

        public override string CreateMultiCorrespondence(string astrTemplateName, ArrayList aarrSelectedRows, Dictionary<string, object> adictParams)
        {
            return base.CreateMultiCorrespondence(astrTemplateName, aarrSelectedRows, adictParams);
        }

        //Rohan
        public ArrayList InitializeWebExtenderUploadService(string astrFileName, string astrUserID, int aintUserSerialID)
        {
            ArrayList larrList = new ArrayList();
            utlError lobjError;
            try
            {

                int lintTrackingID = Convert.ToInt32(astrFileName.Substring(astrFileName.LastIndexOf("-") + 1, 10));

                //Validation
                cdoCorTracking lcdoCorTracking = new cdoCorTracking();
                if (lcdoCorTracking.SelectRow(new object[1] { lintTrackingID }))
                {
                    if (lcdoCorTracking.cor_status_value == busConstant.CORRSTATUS_IMAGED)
                    {
                        lobjError = new utlError();
                        lobjError.istrErrorMessage = "Document is already Imaged!";
                        larrList.Add(lobjError);
                        return larrList;
                    }

                    busCorTemplates lobjCorTemplates = new busCorTemplates();
                    if (lobjCorTemplates.FindCorTemplates(lcdoCorTracking.template_id))
                    {
                        if (String.IsNullOrEmpty(lobjCorTemplates.icdoCorTemplates.doc_type))
                        {
                            lobjError = new utlError();
                            lobjError.istrErrorID = "939";
                            larrList.Add(lobjError);
                            return larrList;
                        }
                    }
                    else
                    {
                        lobjError = new utlError();
                        lobjError.istrErrorMessage = "Invalid Template";
                        larrList.Add(lobjError);
                        return larrList;
                    }
                }
                else
                {
                    lobjError = new utlError();
                    lobjError.istrErrorMessage = "Invalid Tracking ID";
                    larrList.Add(lobjError);
                    return larrList;
                }

                Dictionary<string, object> ldicitUserInfo = new Dictionary<string, object>();
                ldicitUserInfo[utlConstants.istrConstUserID] = astrUserID;
                ldicitUserInfo[utlConstants.istrConstUserSerialID] = aintUserSerialID;

                UpdateCorrespondenceTrackingStatus(lintTrackingID, busConstant.CORRSTATUS_READY_FOR_IMAGING, ldicitUserInfo);
            }
            catch (Exception _exc)
            {
                ExceptionManager.Publish(_exc);
                lobjError = new utlError();
                lobjError.istrErrorMessage = "Invalid Tracking ID";
                larrList.Add(lobjError);
            }
            return larrList;
        }

        public override bool UpdateCorrespondenceTrackingStatus(int aintTrackingID, string astrStatus, Dictionary<string, object> adictParams)
        {

            try
            {
                iobjPassInfo.idictParams = adictParams;
                BeginTransaction();
                cdoCorTracking lcdoCorTracking = new cdoCorTracking();
                if (lcdoCorTracking.SelectRow(new object[1] { aintTrackingID }))
                {
                    if (astrStatus != "")
                    {
                        lcdoCorTracking.cor_status_value = astrStatus;
                        if (astrStatus == busConstant.CORRSTATUS_PRINTED)
                            lcdoCorTracking.printed_date = DateTime.Now;
                        //Dont Set the Imaged Date here.. Because Imaging is happening through Service 
                        //and the service will update the imaged_date.
                    }
                    lcdoCorTracking.Update();
                }
                Commit();
            }
            finally
            {
                if ((iobjPassInfo.iconFramework != null) &&
                    (iobjPassInfo.iconFramework.State == ConnectionState.Open))
                {
                    iobjPassInfo.iconFramework.Close();
                    iobjPassInfo.iconFramework.Dispose();
                }
            }
            return true;
        }

        public byte[] RenderWordAsPDF(string astrFileName)
        {
            byte[] lcorWordFile = null;
            lcorWordFile = busGlobalFunctions.RenderWordAsPDF(astrFileName);
            return lcorWordFile;

        }

        public byte[] RenderPDF(string astr_filename)
        {
            byte[] lPdfFile = null;
            lPdfFile = busGlobalFunctions.RenderPDF(astr_filename);
            return lPdfFile;

        }


        public byte[] DownloadAttachment(string astrFilePath)
        {
            return File.ReadAllBytes(astrFilePath); ;
        }

        public DataTable ExportInExcel(string astrFinalQuery)
        {
            string lstrFinalQuery = astrFinalQuery; // "select * from SGT_DATA_EXTRACTION_BATCH_INFO";

            DataTable ldtPensionActuary = DBFunction.DBSelect(lstrFinalQuery, 
                                                            iobjPassInfo.iconFramework,  iobjPassInfo.itrnFramework);

            return ldtPensionActuary;
        }

        //Ticket#72507        
        public DateTime GetLastWorkingDate(string astrSSN)
        {
            DateTime ldtLastWorkingDate = new DateTime();
            IDbConnection lconLegacy = DBFunction.GetDBConnection("Legacy");
            if (lconLegacy != null)
            {
                Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();
                IDbDataParameter lobjParameter = DBFunction.GetDBParameter();
                lobjParameter.ParameterName = "@SSN";
                lobjParameter.DbType = DbType.String;
                lobjParameter.Value = astrSSN;
                lcolParameters.Add(lobjParameter); ;
                DataTable ldataTable = new DataTable();

                IDataReader lDataReader = DBFunction.DBExecuteProcedureResult("usp_GetLastWorkingDate", lcolParameters, lconLegacy, null);
                if (lDataReader != null)
                {
                    ldataTable.Load(lDataReader);
                    if (ldataTable.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(ldataTable.Rows[0][0])))
                        {
                            ldtLastWorkingDate = Convert.ToDateTime(ldataTable.Rows[0][0]);
                        }
                    }
                }
            }
            return ldtLastWorkingDate;
        }

        public void InsertIntoCorPacketContentTracking(int aintTrackingId,object aobj)
        {
            busCorPacketContentTracking lbusCorPacketContentTracking = new busCorPacketContentTracking { icdoCorPacketContentTracking = new cdoCorPacketContentTracking() };

            DataTable ldtbPacketContentDetails = busBase.Select("cdoCorPacketContent.GetPacketContentDetails", new object[1] { aintTrackingId });
            if(ldtbPacketContentDetails != null && ldtbPacketContentDetails.Rows.Count > 0)
            {
                lbusCorPacketContentTracking.icdoCorPacketContentTracking.LoadData(ldtbPacketContentDetails.Rows[0]);
            }

            if (lbusCorPacketContentTracking.icdoCorPacketContentTracking.tracking_id > 0)
            {
                if (aobj is busBenefitApplication)
                {
                    lbusCorPacketContentTracking.icdoCorPacketContentTracking.benefit_appicaltion_id = (aobj as busBenefitApplication).icdoBenefitApplication.benefit_application_id;
                    lbusCorPacketContentTracking.icdoCorPacketContentTracking.retirement_date = (aobj as busBenefitApplication).icdoBenefitApplication.retirement_date;
                }
                else if (aobj is busBenefitCalculationHeader)
                {
                    lbusCorPacketContentTracking.icdoCorPacketContentTracking.benefit_calculation_header_id = (aobj as busBenefitCalculationHeader).icdoBenefitCalculationHeader.benefit_calculation_header_id;
                    lbusCorPacketContentTracking.icdoCorPacketContentTracking.retirement_date = (aobj as busBenefitCalculationHeader).icdoBenefitCalculationHeader.retirement_date;
                }
                if (aobj is busQdroApplication)
                {
                    lbusCorPacketContentTracking.icdoCorPacketContentTracking.qdro_appicaltion_id = (aobj as busQdroApplication).icdoDroApplication.dro_application_id;
                    lbusCorPacketContentTracking.icdoCorPacketContentTracking.retirement_date = (aobj as busQdroApplication).icdoDroApplication.dro_commencement_date;
                }
                else if (aobj is busQdroCalculationHeader)
                {
                    lbusCorPacketContentTracking.icdoCorPacketContentTracking.qdro_calculation_header_id = (aobj as busQdroCalculationHeader).icdoQdroCalculationHeader.qdro_calculation_header_id;
                    lbusCorPacketContentTracking.icdoCorPacketContentTracking.retirement_date = (aobj as busQdroCalculationHeader).icdoQdroCalculationHeader.qdro_commencement_date;
                }
                else if (aobj is busPerson) 
                {
                    lbusCorPacketContentTracking.icdoCorPacketContentTracking.retirement_date = (aobj as busPerson).icdoPerson.retirement_health_date;
                }


                    lbusCorPacketContentTracking.icdoCorPacketContentTracking.packet_status_value = busConstant.BENEFIT_APPLICATION_STATUS_PENDING;
                lbusCorPacketContentTracking.icdoCorPacketContentTracking.Insert();

                busCorPacketContentTrackingHistory lbusCorPacketContentTrackingHistory = new busCorPacketContentTrackingHistory { icdoCorPacketContentTrackingHistory = new cdoCorPacketContentTrackingHistory() };

                lbusCorPacketContentTrackingHistory.icdoCorPacketContentTrackingHistory.cor_packet_content_tracking_id = lbusCorPacketContentTracking.icdoCorPacketContentTracking.cor_packet_content_tracking_id;
                

                lbusCorPacketContentTrackingHistory.icdoCorPacketContentTrackingHistory.packet_status_value = busConstant.BENEFIT_APPLICATION_STATUS_PENDING;
                lbusCorPacketContentTrackingHistory.icdoCorPacketContentTrackingHistory.Insert();
            }

        }

        public ArrayList UpdateBenefitOptionValue(int intPlan_Id, string istrBenefitOptionValue,int benefit_application_detail_id, string spousal_consent_flag, int iintJointAnnuaintID)
        {
            ArrayList larrList = new ArrayList();
            utlError lobjError;
            DataTable ldtbPayeeAccount = busBase.Select("cdoBenefitApplication.CheckIfPayeeAccountExist", new object[1] { benefit_application_detail_id });

            if (ldtbPayeeAccount.Rows.Count > 0)
            {
                lobjError = new utlError();
                lobjError.istrErrorMessage = "Payee Account exist cannot change Benefit Option.";
                larrList.Add(lobjError);
                return larrList;

            }
            if(intPlan_Id == busConstant.IAP_PLAN_ID)
            {
                busPlanBenefitXr lbusPlanBenefitXr = new busPlanBenefitXr();
                var lplanbenefitId = lbusPlanBenefitXr.GetPlanBenefitId(intPlan_Id, istrBenefitOptionValue);

                busBenefitApplicationDetail lbusBenefitApplicationDetail = new busBenefitApplicationDetail();
                lbusBenefitApplicationDetail.FindBenefitApplicationDetail(benefit_application_detail_id);

                if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id > 0)
                {
                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.plan_benefit_id = lplanbenefitId;

                    if (spousal_consent_flag != string.Empty)
                    {
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.spousal_consent_flag = spousal_consent_flag;
                    }


                    if (iintJointAnnuaintID > 0)
                    {
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.joint_annuitant_id = iintJointAnnuaintID;
                    }
                    else
                    {
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.joint_annuitant_id = Convert.ToInt32(null);
                    }

                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.Update();

                    busBenefitApplication lbusBenefitApplication = new busBenefitApplication();
                    lbusBenefitApplication.FindBenefitApplication(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_id);
                    if (lbusBenefitApplication.icdoBenefitApplication.benefit_application_id > 0)
                    {
                        lbusBenefitApplication.icdoBenefitApplication.final_calc_flag = null;
                        lbusBenefitApplication.icdoBenefitApplication.change_benefit_option_flag = busConstant.FLAG_YES;
                        lbusBenefitApplication.icdoBenefitApplication.Update();

                        lbusBenefitApplication.CancelIAPFinalPendingCalculations(lbusBenefitApplication.icdoBenefitApplication.benefit_application_id);

                    }

                }

            }
            
            return larrList;
       }

        protected override ArrayList GetGeneratedCorrespondence(string astrGeneratedDocumentName = null, string astrTemplateName = null, string astrTrackingId = null)
        {
            ArrayList larrResult = null;

            try
            {
                if (iobjPassInfo.istrFormName == "wfmCorTemplatesLookup")
                {

                    busSystemManagement iobjSystemManagement = null;
                    iobjSystemManagement = new busSystemManagement();
                    iobjSystemManagement.FindSystemManagement();

                    if (!string.IsNullOrEmpty(iobjSystemManagement.icdoSystemManagement.base_directory))
                    {
                        string FilePath = iobjSystemManagement.icdoSystemManagement.base_directory;
                        astrGeneratedDocumentName = FilePath + "Correspondence\\Templates\\" + astrGeneratedDocumentName + ".docx";
                    }
                }
                if (astrGeneratedDocumentName != null)
                {
                    larrResult = EditCorrOnLocalTool(astrGeneratedDocumentName, iobjPassInfo.idictParams);
                }
                else
                {
                    string lstrFileName = null;
                    astrTrackingId = astrTrackingId.PadLeft(10, '0');
                    lstrFileName = astrTemplateName + "-" + astrTrackingId + ".docx";
                    string lstrCorrespondencePath = iobjPassInfo.isrvDBCache.GetPathInfo("CorrGenr");                  
                    if (!System.IO.File.Exists(lstrCorrespondencePath + lstrFileName))
                    {
                        lstrFileName = lstrFileName.Replace(".docx", ".doc");
                    }
                    larrResult = EditCorrOnLocalTool(lstrCorrespondencePath + lstrFileName, iobjPassInfo.idictParams);
                }
                return larrResult;
            }
            catch (Exception exc)
            {
                throw HandleGlobalError(exc);
            }
            finally
            {
                DisposeFrameworkConnection();
            }
        }

        protected override busMainBase GetWorkflowActivityInstanceById(int aintActivityInstanceId)
        {
            busActivityInstance lbusActivityInstance = new busActivityInstance();
            if (lbusActivityInstance.FindActivityInstance(aintActivityInstanceId))
            {
                lbusActivityInstance.LoadActivity();
                lbusActivityInstance.LoadProcessInstance();
                lbusActivityInstance.ibusProcessInstance.ibusProcess = lbusActivityInstance.ibusActivity.ibusProcess;
                lbusActivityInstance.ibusProcessInstance.LoadPerson();
            }
            return lbusActivityInstance;
        }
    }
}
