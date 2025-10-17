#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;
using System.Linq;
using Sagitec.CustomDataObjects;
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busPaymentReissueDetail:
	/// Inherited from busPaymentReissueDetailGen, the class is used to customize the business object busPaymentReissueDetailGen.
	/// </summary>
    [Serializable]
    public class busPaymentReissueDetail : busPaymentReissueDetailGen
    {
        #region Properties

        public busPaymentHistoryDistribution ibusPaymentHistoryDistribution { get; set; }
        public busPaymentHistoryHeader ibusPaymentHistoryHeader { get; set; }
        public string istrBenefitOptionValue { get; set; }
        public Collection<cdoPerson> iclbSurvivorNames { get; set; }
        public Collection<busPaymentReissueItemDetail> iclbPaymentReissueItemDetail { get; set; }
        #endregion

        #region Public Methods

        public void LoadInitialData()
        {
            //DataTable ltblPaymentDistribution = Select("cdoPaymentReissueDetail.GetPaymentHistoryDistribution", new object[1] { this.icdoPaymentReissueDetail.payment_history_header_id });
            //if (ltblPaymentDistribution.Rows.Count > 0)
            //{
            //    this.ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.LoadData(ltblPaymentDistribution.Rows[0]);
            //}
          
        }

        public void LoadPaymentReissueItemDetail()
        {
            if (icdoPaymentReissueDetail != null)
            {
                if (iclbPaymentReissueItemDetail == null)
                    iclbPaymentReissueItemDetail = new Collection<busPaymentReissueItemDetail>();

                DataTable ldtbList = Select<cdoPaymentReissueItemDetail>(
                    new string[1] { enmPaymentReissueItemDetail.payment_reissue_detail_id.ToString() },
                    new object[1] { icdoPaymentReissueDetail.payment_reissue_detail_id }, null, enmPaymentReissueItemDetail.payment_reissue_detail_id.ToString());
                iclbPaymentReissueItemDetail = GetCollection<busPaymentReissueItemDetail>(ldtbList, "icdoPaymentReissueItemDetail");
            }
        }


        public void InsertIntoPaymentReissueItemDetail(int aintPaymentReissueDetailId,int aintPayeeAccountPaymentItemTypeId)
        {
            if (iclbPaymentReissueItemDetail == null)
            {
                iclbPaymentReissueItemDetail = new Collection<busPaymentReissueItemDetail>();
                LoadPaymentReissueItemDetail();
            }

            if (iclbPaymentReissueItemDetail != null && 
                iclbPaymentReissueItemDetail.Where(item => item.icdoPaymentReissueItemDetail.payment_reissue_detail_id == aintPaymentReissueDetailId
                    && item.icdoPaymentReissueItemDetail.payee_account_payment_item_type_id == aintPayeeAccountPaymentItemTypeId).Count() <= 0)
            {
                busPaymentReissueItemDetail lbusPaymentReissueItemDetail = new busPaymentReissueItemDetail { icdoPaymentReissueItemDetail = new cdoPaymentReissueItemDetail() };

                if (aintPaymentReissueDetailId > 0)
                {
                    lbusPaymentReissueItemDetail.icdoPaymentReissueItemDetail.payment_reissue_detail_id = aintPaymentReissueDetailId;
                }
                lbusPaymentReissueItemDetail.icdoPaymentReissueItemDetail.payee_account_payment_item_type_id = aintPayeeAccountPaymentItemTypeId;
                lbusPaymentReissueItemDetail.icdoPaymentReissueItemDetail.created_by = iobjPassInfo.istrUserID;
                lbusPaymentReissueItemDetail.icdoPaymentReissueItemDetail.created_date = DateTime.Now;
                lbusPaymentReissueItemDetail.icdoPaymentReissueItemDetail.modified_by = iobjPassInfo.istrUserID;
                lbusPaymentReissueItemDetail.icdoPaymentReissueItemDetail.modified_date = DateTime.Now;
                lbusPaymentReissueItemDetail.icdoPaymentReissueItemDetail.Insert();

                iclbPaymentReissueItemDetail.Add(lbusPaymentReissueItemDetail);
            }
        }
        #endregion


        public ArrayList CheckErrorOnAddButton(object aobj, Hashtable ahstParams, ref ArrayList aarrErrors, bool ablnHardErrorOnSave = false)
        {
            utlError lobjError = null;
            
            busPaymentHistoryDistribution lbusPaymentHistoryDistribution = aobj as busPaymentHistoryDistribution;

            if (Convert.ToString(ahstParams["icdoPaymentReissueDetail.reissue_payment_type_value"]).IsNullOrEmpty())
            {
                lobjError = AddError(6099, " ");
                aarrErrors.Add(lobjError);
                return aarrErrors;
            }

            if (Convert.ToString(ahstParams["icdoPaymentReissueDetail.reissue_reason_value"]).IsNullOrEmpty())
            {

                lobjError = AddError(6114, " ");
                aarrErrors.Add(lobjError);
                return aarrErrors;
            }

            if (lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_ROLLOVER_ACH || lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_ROLLOVER_CHECK)
            {

                    if (Convert.ToString(ahstParams["icdoPaymentReissueDetail.reissue_payment_type_value"]) != busConstant.REISSUE_PAYMENT_TYPE_ROLLOVER_ORGANIZATION
                        && Convert.ToString(ahstParams["icdoPaymentReissueDetail.reissue_payment_type_value"]) != busConstant.REISSUE_PAYMENT_TYPE_ROLLOVER_ORGANIZATION_TO_PAYEE)
                    {
                        lobjError = AddError(6094, "  ");
                        aarrErrors.Add(lobjError);
                        return aarrErrors;

                    }
            }
            else if ((lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_ACH || lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PAYMENT_METHOD_CHECK)
                && lbusPaymentHistoryDistribution.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.person_id == lbusPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.person_id)
            {

                if ( Convert.ToString(ahstParams["icdoPaymentReissueDetail.reissue_payment_type_value"]) != busConstant.REISSUE_PAYMENT_TYPE_PAYEE
                       && Convert.ToString(ahstParams["icdoPaymentReissueDetail.reissue_payment_type_value"]) != busConstant.REISSUE_PAYMENT_TYPE_PAYEE_TO_SURVIVOR &&
                  Convert.ToString(ahstParams["icdoPaymentReissueDetail.reissue_payment_type_value"]) != busConstant.REISSUE_PAYMENT_TYPE_PAYEE_TO_ROLLOVER_ORGANIZATION)
           
                {
                    lobjError = AddError(6094, "  ");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }

            }
            else
            {
                //in case of deduction
                if (Convert.ToString(ahstParams["icdoPaymentReissueDetail.reissue_payment_type_value"]) != busConstant.REISSUE_PAYMENT_TYPE_PAYEE && Convert.ToString(ahstParams["icdoPaymentReissueDetail.reissue_payment_type_value"]) != busConstant.REISSUE_PAYMENT_TYPE_TRANSFER_ORGANIZATION)
                {
                    lobjError = AddError(6094, "  ");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }
            }


            if (ablnHardErrorOnSave && lbusPaymentHistoryDistribution.iclbPaymentReissueDetail.IsNotNull() && Convert.ToString(ahstParams["icdoPaymentReissueDetail.reissue_payment_type_value"]) != busConstant.REISSUE_PAYMENT_TYPE_PAYEE_TO_SURVIVOR)
                {
                    if (lbusPaymentHistoryDistribution.iclbPaymentReissueDetail.Count > 1)
                    {
                        bool lblnflag = true;

                        foreach (utlError lobjutlError in aarrErrors)
                        {
                            if (lobjutlError.istrErrorID == "6095")
                            {
                                lblnflag = false;
                            }
                        }

                        if (lblnflag)
                        {
                            lobjError = AddError(6095, "  ");
                            aarrErrors.Add(lobjError);
                        }
                        return aarrErrors;

                    }

                }


            if (Convert.ToString(ahstParams["icdoPaymentReissueDetail.reissue_payment_type_value"]) == busConstant.REISSUE_PAYMENT_TYPE_PAYEE_TO_ROLLOVER_ORGANIZATION || Convert.ToString(ahstParams["icdoPaymentReissueDetail.reissue_payment_type_value"]) == busConstant.REISSUE_PAYMENT_TYPE_ROLLOVER_ORGANIZATION)
            {
                if (Convert.ToString(ahstParams["icdoPaymentReissueDetail.recipient_rollover_org_id"]).IsNullOrEmpty())
                {
                    lobjError = AddError(6091, " ");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }
            }

            int lint1099RBatchRanYear = 0;
            DataTable ldtbl1099RBatchRanYear = Select("cdoRepaymentSchedule.GetYear1099RBatchRan", new object[0] { });
            if (ldtbl1099RBatchRanYear != null && ldtbl1099RBatchRanYear.Rows.Count > 0 && Convert.ToString(ldtbl1099RBatchRanYear.Rows[0][0]).IsNotNullOrEmpty())
            {
                lint1099RBatchRanYear = Convert.ToInt32(ldtbl1099RBatchRanYear.Rows[0][0]);
            }

            if (lbusPaymentHistoryDistribution.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_date != DateTime.MinValue
                && lbusPaymentHistoryDistribution.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_date.Year <= lint1099RBatchRanYear)
            {
                if (Convert.ToString(ahstParams["icdoPaymentReissueDetail.reissue_payment_type_value"]) == busConstant.REISSUE_PAYMENT_TYPE_PAYEE_TO_ROLLOVER_ORGANIZATION
                    || Convert.ToString(ahstParams["icdoPaymentReissueDetail.reissue_payment_type_value"]) == busConstant.REISSUE_PAYMENT_TYPE_ROLLOVER_ORGANIZATION_TO_PAYEE)
                {
                    lobjError = AddError(6097, "  ");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }
                
            }


            if (Convert.ToString(ahstParams["icdoPaymentReissueDetail.reissue_payment_type_value"]) == busConstant.REISSUE_PAYMENT_TYPE_PAYEE_TO_SURVIVOR)
            {
                int lintParticipantDateOfDeathCnt = (int)DBFunction.DBExecuteScalar("cdoPaymentReissueDetail.CheckIfParticipantDODExists", new object[1] { lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_distribution_id },
                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (lintParticipantDateOfDeathCnt == 0)
                {
                    lobjError = AddError(6089, " ");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }

            }
            
            
          
            if (lbusPaymentHistoryDistribution.iclbPaymentReissueDetail.IsNotNull() && Convert.ToString(ahstParams["icdoPaymentReissueDetail.reissue_payment_type_value"]) == busConstant.REISSUE_PAYMENT_TYPE_PAYEE_TO_SURVIVOR)
            {
                decimal ldecTotalPercentage = 0M;
               
                int lintSurvivorId = 0;

                if (Convert.ToString(ahstParams["icdoPaymentReissueDetail.iintSurvivorId"]).IsNotNullOrEmpty())
                {
                    lintSurvivorId = Convert.ToInt32(ahstParams["icdoPaymentReissueDetail.iintSurvivorId"]);
                }
                else
                {
                    lobjError = AddError(6098, "  ");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }


                foreach (busPaymentReissueDetail lbusPaymentReissueDetail in lbusPaymentHistoryDistribution.iclbPaymentReissueDetail)
                {
                    if (lbusPaymentHistoryDistribution.iclbSurvivorNames.Where(item => item.iintBenId == lbusPaymentReissueDetail.icdoPaymentReissueDetail.iintSurvivorId).Count() > 0)
                        ldecTotalPercentage += lbusPaymentHistoryDistribution.iclbSurvivorNames.Where(item => item.iintBenId == lbusPaymentReissueDetail.icdoPaymentReissueDetail.iintSurvivorId).First().idecBenPercentage;
                }
                //ldecTotalPercentage += lbusPaymentHistoryDistribution.iclbSurvivorNames.Where(item => item.iintBenId == lintSurvivorId).First().idecBenPercentage;

                if (ldecTotalPercentage > 100)
                {
                    lobjError = AddError(6096, "  ");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }
            }
            //1099r test case check
            if (lbusPaymentHistoryDistribution.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_date.Year != DateTime.Today.Year)
            {
                DateTime idtYearDate = new DateTime(2012, 04, 15);
                if (Convert.ToString(ahstParams["icdoPaymentReissueDetail.reissue_payment_type_value"]) == busConstant.REISSUE_PAYMENT_TYPE_ROLLOVER_ORGANIZATION_TO_PAYEE && idtYearDate < DateTime.Today)
                {
                    lobjError = AddError(6157, "  ");
                    aarrErrors.Add(lobjError);
                    return aarrErrors;
                }
            }

            return aarrErrors;
        }

        public Collection<cdoCodeValue> GetReissuePaymentTypes()
        {
            Collection<cdoCodeValue> lclbCodeValue = new Collection<cdoCodeValue>();
            busCodeValue lbusCodeValue = new busCodeValue();
            lclbCodeValue = lbusCodeValue.GetCodeValue(busConstant.REISSUE_PAYMENT_CODE_ID);
            
            return lclbCodeValue;
        }

        public Collection<cdoPerson> LoadSurvivorsForPayee()
        {
            return iclbSurvivorNames;
        }
    }
}
