#region Using directives
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.DataObjects;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
#endregion
namespace MPIPHP.BusinessObjects
{
    /// <summary>
    ///  partial class NeoSpin.BusinessObjects.busOnetimeRetireePaymentContract
    /// </summary>
	[Serializable]
	public  partial class busOnetimeRetireePaymentContract : busBase
	{
		/// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busOnetimeRetireePaymentContract
        /// </summary>
		public busOnetimeRetireePaymentContract()
		{
		}
        #region Overriden Methods

        public override void BeforePersistChanges()
        {
            if (icdoOnetimeRetireePaymentContract.ienuObjectState == ObjectState.Insert)
            {
                icdoOnetimeRetireePaymentContract.contract_status_value = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_PENDING;
            }
            //PIR 932
            icdoOnetimeRetireePaymentContract.retirement_date_from = Convert.ToDateTime("10/26/1953");
            //if (icdoOnetimeRetireePaymentContract.plan_year > 0)
            //{
            //    icdoOnetimeRetireePaymentContract.effective_start_date = new DateTime(icdoOnetimeRetireePaymentContract.plan_year, 08, 01);
            //}



            base.BeforePersistChanges();
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();

            ////if (icdoActiveRetireeIncreaseContract.effective_end_date != DateTime.MinValue)
            ////{
            //busActiveRetireeIncreaseContractHistory lbusActiveRetireeIncreaseContractHistory = new busActiveRetireeIncreaseContractHistory();
            //lbusActiveRetireeIncreaseContractHistory.InsertValuesInActiveRetireeIncreaseContractHistory(icdoActiveRetireeIncreaseContract.active_retiree_increase_contract_id,
            //    icdoActiveRetireeIncreaseContract.plan_year, icdoActiveRetireeIncreaseContract.effective_start_date, icdoActiveRetireeIncreaseContract.effective_end_date,
            //    icdoActiveRetireeIncreaseContract.percent_increase_value);

            //LoadActiveRetireeIncreaseContractHistorys();
            ////}

        }

        #endregion

        #region Public Methods

        public ArrayList btn_Approve()
        {
            ArrayList larrList = new ArrayList();
            int lintUserSerialId = utlPassInfo.iobjPassInfo.iintUserSerialID;

            busUser lbusUser = new busUser();
            lbusUser.FindUser(lintUserSerialId);

            icdoOnetimeRetireePaymentContract.contract_status_value = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_APPROVED;
            icdoOnetimeRetireePaymentContract.approved_by = lbusUser.icdoUser.last_name + "," + lbusUser.icdoUser.first_name;
            icdoOnetimeRetireePaymentContract.Update();
            larrList.Add(this);

            return larrList;
        }

        public ArrayList btn_Close()
        {
            ArrayList larrList = new ArrayList();
            int lintUserSerialId = utlPassInfo.iobjPassInfo.iintUserSerialID;

            busUser lbusUser = new busUser();
            lbusUser.FindUser(lintUserSerialId);

            //WI 14763 RID 118342  adding iblnApprovedGroupOnly parameter
            DateTime ldtEndDate = new DateTime(icdoOnetimeRetireePaymentContract.plan_year, 12, 31);

            if (icdoOnetimeRetireePaymentContract.effective_end_date != DateTime.MaxValue)
            {
                icdoOnetimeRetireePaymentContract.effective_end_date = ldtEndDate;

            }
            icdoOnetimeRetireePaymentContract.approved_by = lbusUser.icdoUser.last_name + "," + lbusUser.icdoUser.first_name;
            icdoOnetimeRetireePaymentContract.Update();
            larrList.Add(this);

            return larrList;
        }

        public busOnetimeRetireePaymentContract LoadOneTimeRetireeIncContractByPlanYear(int aintPlanYear)
        {
            DataTable ldtbContractInfo =
                Select("cdoOnetimeRetireePaymentContract.LoadContractByPlanYear", new object[1] { aintPlanYear });

            if (ldtbContractInfo.Rows.Count > 0)
            {
                busOnetimeRetireePaymentContract lbusOnetimeRetireePaymentContract =
                        new busOnetimeRetireePaymentContract { icdoOnetimeRetireePaymentContract = new doOnetimeRetireePaymentContract() };
                lbusOnetimeRetireePaymentContract.icdoOnetimeRetireePaymentContract.LoadData(ldtbContractInfo.Rows[0]);
                return lbusOnetimeRetireePaymentContract;
            }

            return null;
        }

        public Collection<busOnetimeRetireePaymentContract> LoadOnetimeRetireeIncContractByRetirementDate(DateTime adtRetirementDate)
        {
            Collection<busOnetimeRetireePaymentContract> lclbOnetimeRetireePaymentContract = new Collection<busOnetimeRetireePaymentContract>();
            DataTable ldtbContractInfo =
                Select("cdoOnetimeRetireePaymentContract.LoadContractByRetirementDate", new object[1] { adtRetirementDate });
            lclbOnetimeRetireePaymentContract = GetCollection<busOnetimeRetireePaymentContract>(ldtbContractInfo, "icdoOnetimeRetireePaymentContract");


            return lclbOnetimeRetireePaymentContract;
        }

        public Collection<busOnetimeRetireePaymentContract> LoadOnetimeRetireeIncContractByMDYear(int aintPlanYear)
        {
            Collection<busOnetimeRetireePaymentContract> lclbOnetimeRetireePaymentContract = new Collection<busOnetimeRetireePaymentContract>();
            DataTable ldtbContractInfo =
                Select("cdoOnetimeRetireePaymentContract.LoadContractByPlanYearSinceMDYear", new object[1] { aintPlanYear });
            lclbOnetimeRetireePaymentContract = GetCollection<busOnetimeRetireePaymentContract>(ldtbContractInfo, "icdoOnetimeRetireePaymentContract");


            return lclbOnetimeRetireePaymentContract;
        }

        #endregion
        #region Validations

        public bool CheckIfRetirementDateOverlapping()
        {
            if (icdoOnetimeRetireePaymentContract.retirement_date_from != DateTime.MinValue &&
                icdoOnetimeRetireePaymentContract.retirement_date_to != DateTime.MinValue)
            {
                DataTable ldtbList = busBase.Select("cdoActiveRetireeIncreaseContract.CheckForOverlappingDates",
                                     new object[] { icdoOnetimeRetireePaymentContract.plan_year, icdoOnetimeRetireePaymentContract.retirement_date_from,
                                 icdoOnetimeRetireePaymentContract.retirement_date_to});
                if (ldtbList.Rows.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
