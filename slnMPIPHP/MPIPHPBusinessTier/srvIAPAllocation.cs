#region Using directives

using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using MPIPHP.BusinessObjects;
using MPIPHP.CustomDataObjects;
using System.Linq;
using Sagitec.Common;
using Sagitec.BusinessObjects;
using Sagitec.DBUtility;
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.BusinessTier
{
    public class srvIAPAllocation : srvMPIPHP
    {
        public srvIAPAllocation()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public busIapAllocationDetail FindIapAllocationDetail(int aintIapAllocationDetailId)
        {
            busIapAllocationDetail lobjIapAllocationDetail = new busIapAllocationDetail();
            if (lobjIapAllocationDetail.FindIapAllocationDetail(aintIapAllocationDetailId))
            {
            }

            return lobjIapAllocationDetail;
        }

        public busIapAllocationFactor FindIapAllocationFactor(int aintIapAllocationFactorId)
        {
            busIapAllocationFactor lobjIapAllocationFactor = new busIapAllocationFactor();
            if (lobjIapAllocationFactor.FindIapAllocationFactor(aintIapAllocationFactorId))
            {
            }

            return lobjIapAllocationFactor;
        }

        public busIAPAllocationDetailPersonOverview FindIAPAllocationDetailPersonOverview(int aintPersonId)
        {
            busIAPAllocationDetailPersonOverview lbusIAPAllocationDetailPersonOverview = new busIAPAllocationDetailPersonOverview();

            if (lbusIAPAllocationDetailPersonOverview.FindPerson(aintPersonId))
            {
                DataTable ldtblist = busPerson.Select("cdoPersonAccount.LoadPersonAccountbyPlanId", new object[2] { lbusIAPAllocationDetailPersonOverview.icdoPerson.person_id, busConstant.IAP_PLAN_ID });

                if (ldtblist.Rows.Count > 0)
                {
                    lbusIAPAllocationDetailPersonOverview.LoadIAPAllocationDetails(Convert.ToInt32(ldtblist.Rows[0][enmPersonAccount.person_account_id.ToString()]));
                }
            }
            return lbusIAPAllocationDetailPersonOverview;
        }

        //PIR 0989 
        public busIAPAllocationYearlyDetail FindbusIAPAllocationYearlyDetails(int aintPersonId, int aintPersonAccountId, decimal adecComputationYear, string astrSpecialAccount = "", string astrIsPayment = "N") //PIR 0989
        {
            busIAPAllocationYearlyDetail lbusIAPAllocationYearlyDetail = new busIAPAllocationYearlyDetail();
            if (lbusIAPAllocationYearlyDetail.FindPerson(aintPersonId))
            {
                lbusIAPAllocationYearlyDetail.lintPersonAccountId = aintPersonAccountId;
                lbusIAPAllocationYearlyDetail.lintComputationYear = Convert.ToInt32(adecComputationYear);
                lbusIAPAllocationYearlyDetail.LoadYearlyDetail(aintPersonAccountId, Convert.ToInt32(adecComputationYear), astrIsPayment, astrSpecialAccount);   //PIR 0989

                if (Convert.ToInt32(adecComputationYear) >= 1997 && Convert.ToInt32(adecComputationYear) <= 2001)
                {
                    busIAPAllocationHelper lobjIAPHelper = new busIAPAllocationHelper();
                    if (lobjIAPHelper.CheckParticipantIsAffiliate(Convert.ToInt32(adecComputationYear), lbusIAPAllocationYearlyDetail.icdoPerson.istrSSNNonEncrypted))
                        lbusIAPAllocationYearlyDetail.istrAffiliate = busConstant.YES;
                    else
                        lbusIAPAllocationYearlyDetail.istrAffiliate = busConstant.NO;
                }
            }
            return lbusIAPAllocationYearlyDetail;
        }

        ///Parameters adtEACutOffDate & aintComputationYear are strictly used for Late IAP Alloation Batch.
        ///These parameters has no significance in IAP Recalculation Functionality.Please do not use these parameters.
        public busIAPAllocationDetailPersonOverview LoadAndRecalculateIAPAllocationDetail(int aintPersonId)
        {
            busIAPAllocationDetailPersonOverview lbusRecalculateIAPAllocationDetail = new busIAPAllocationDetailPersonOverview();
            //PIR 628
            lbusRecalculateIAPAllocationDetail.LoadRecalculateAndPostIAPAllocationDetail(aintPersonId, DateTime.MinValue, 0);//PIR 628 extended
            //if (lbusRecalculateIAPAllocationDetail.FindPerson(aintPersonId))
            //{
            //    DataTable ldtblist = busPerson.Select("cdoPersonAccount.LoadPersonAccountbyPlanId", new object[2] { lbusRecalculateIAPAllocationDetail.icdoPerson.person_id, busConstant.IAP_PLAN_ID });

            //    if (ldtblist.Rows.Count > 0)
            //    {
            //        lbusRecalculateIAPAllocationDetail.iclbIAPAllocationDetailPersonOverview = new Collection<busIapAllocationDetailCalculation>();
            //        lbusRecalculateIAPAllocationDetail.LoadPaidIAPAllocationDetailsFromOpus(Convert.ToInt32(ldtblist.Rows[0][enmPersonAccount.person_account_id.ToString()]));
            //        lbusRecalculateIAPAllocationDetail.LoadIAPAllocation5Information(Convert.ToInt32(ldtblist.Rows[0][enmPersonAccount.person_account_id.ToString()]));
            //        lbusRecalculateIAPAllocationDetail.RecalculateIAPAllocationDetails();
            //        lbusRecalculateIAPAllocationDetail.LoadIAPAllocationDetailsFromOpus(Convert.ToInt32(ldtblist.Rows[0][enmPersonAccount.person_account_id.ToString()]));
            //        lbusRecalculateIAPAllocationDetail.GetDifferenceIAPAllocationDetailsReemployment();
            //        lbusRecalculateIAPAllocationDetail.GetDifferenceIAPAllocationDetails();
            //    }
            //}
            return lbusRecalculateIAPAllocationDetail;
        }

        //PIR 19
        public busIAPAllocationDetailPersonOverview LoadAndRecalculateL52AllocationDetail(int aintPersonId)
        {
            busIAPAllocationDetailPersonOverview lbusRecalculateIAPAllocationDetail = new busIAPAllocationDetailPersonOverview();
            lbusRecalculateIAPAllocationDetail.LoadRecalculateAndPostSpAccntAllocDetail(aintPersonId, busConstant.FundTypeLocal52SpecialAccount);
            return lbusRecalculateIAPAllocationDetail;
        }

        //PIR 19
        public busIAPAllocationDetailPersonOverview LoadAndRecalculateL161AllocationDetail(int aintPersonId)
        {
            busIAPAllocationDetailPersonOverview lbusRecalculateIAPAllocationDetail = new busIAPAllocationDetailPersonOverview();
            lbusRecalculateIAPAllocationDetail.LoadRecalculateAndPostSpAccntAllocDetail(aintPersonId,busConstant.FundTypeLocal161SpecialAccount);
            return lbusRecalculateIAPAllocationDetail;
        }

        public busIAPAllocationFactorLookup LoadIAPAllocationFactors(DataTable adtbSearchResult)
        {
            busIAPAllocationFactorLookup lobjIAPAllocationFactorLookup = new busIAPAllocationFactorLookup();
            lobjIAPAllocationFactorLookup.LoadIapAllocationFactors(adtbSearchResult);
            return lobjIAPAllocationFactorLookup;
        }

        public busIapAllocationFactor NewIAPAllocationFactor()
        {
            busIapAllocationFactor lobjIAPAllocationFactor = new busIapAllocationFactor();
            lobjIAPAllocationFactor.icdoIapAllocationFactor = new cdoIapAllocationFactor();
            return lobjIAPAllocationFactor;
        }

        public busIapAllocationSummary FindIapAllocationSummary(int aintIapAllocationSummaryId)
        {
            busIapAllocationSummary lobjIapAllocationSummary = new busIapAllocationSummary();
            if (lobjIapAllocationSummary.FindIapAllocationSummary(aintIapAllocationSummaryId))
            {
            }

            return lobjIapAllocationSummary;
        }

        public busIapAnnunityAdjustmentMultiplier LoadIAPAnnunityAdjustmentMultiplier()
        {
            busIapAnnunityAdjustmentMultiplier lobjIapAnnunityAdjustmentMultiplier = new busIapAnnunityAdjustmentMultiplier();
            lobjIapAnnunityAdjustmentMultiplier.icdoIapAnnunityAdjustmentMultiplier = new cdoIapAnnunityAdjustmentMultiplier();
            DataTable ldtblAnnunityAdjustmentMultiplier = busIapAnnunityAdjustmentMultiplier.Select("cdoIapAnnunityAdjustmentMultiplier.LookUp", new object[0] { });
            if (ldtblAnnunityAdjustmentMultiplier.Rows.Count > 0)
            {
                lobjIapAnnunityAdjustmentMultiplier.icdoIapAnnunityAdjustmentMultiplier.multiplier = Convert.ToDecimal(ldtblAnnunityAdjustmentMultiplier.Rows[0]["Multiplier"]);
                lobjIapAnnunityAdjustmentMultiplier.icdoIapAnnunityAdjustmentMultiplier.iap_annunity_adjustment_multiplier_id = Convert.ToInt32(ldtblAnnunityAdjustmentMultiplier.Rows[0]["IAP_ANNUNITY_ADJUSTMENT_MULTIPLIER_ID"]);
            }
            lobjIapAnnunityAdjustmentMultiplier.icdoIapAnnunityAdjustmentMultiplier.iintAPrimarKey = 1;
            return lobjIapAnnunityAdjustmentMultiplier;
        }

        public busIapAnnunityAdjustmentMultiplier NewIAPAnnunityAdjustmentMultiplier()
        {
            busIapAnnunityAdjustmentMultiplier lobjAnnunityAdjustmentMultiplier = new busIapAnnunityAdjustmentMultiplier();
            lobjAnnunityAdjustmentMultiplier.icdoIapAnnunityAdjustmentMultiplier = new cdoIapAnnunityAdjustmentMultiplier();
            return lobjAnnunityAdjustmentMultiplier;
        }

        public busIapAnnunityAdjustmentMultiplier FindAnnunityAdjustmentMultiplier()
        {
            busIapAnnunityAdjustmentMultiplier lobjAnnunityAdjustmentMultiplier = new busIapAnnunityAdjustmentMultiplier();
            if (lobjAnnunityAdjustmentMultiplier.FindIapAnnunityAdjustmentMultiplier())
            {
            }

            return lobjAnnunityAdjustmentMultiplier;
        }

    }
}
