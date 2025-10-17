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
using Sagitec.CustomDataObjects;
using MPIPHP.DataObjects;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busActiveRetireeIncreaseContract:
    /// Inherited from busActiveRetireeIncreaseContractGen, the class is used to customize the business object busActiveRetireeIncreaseContractGen.
    /// </summary>
    [Serializable]
    public class busActiveRetireeIncreaseContract : busActiveRetireeIncreaseContractGen
    {
        #region Overriden Methods

        public override void BeforePersistChanges()
        {
            if (icdoActiveRetireeIncreaseContract.ienuObjectState == ObjectState.Insert)
            {
                icdoActiveRetireeIncreaseContract.contract_status_value = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_PENDING;
            }
            //PIR 932
            icdoActiveRetireeIncreaseContract.retirement_date_from = Convert.ToDateTime("10/26/1953");
            if (icdoActiveRetireeIncreaseContract.plan_year > 0)
            {
                icdoActiveRetireeIncreaseContract.effective_start_date = new DateTime(icdoActiveRetireeIncreaseContract.plan_year, 08, 01);
            }

            base.BeforePersistChanges();
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();

            //if (icdoActiveRetireeIncreaseContract.effective_end_date != DateTime.MinValue)
            //{
            busActiveRetireeIncreaseContractHistory lbusActiveRetireeIncreaseContractHistory = new busActiveRetireeIncreaseContractHistory();
            lbusActiveRetireeIncreaseContractHistory.InsertValuesInActiveRetireeIncreaseContractHistory(icdoActiveRetireeIncreaseContract.active_retiree_increase_contract_id,
                icdoActiveRetireeIncreaseContract.plan_year, icdoActiveRetireeIncreaseContract.effective_start_date, icdoActiveRetireeIncreaseContract.effective_end_date,
                icdoActiveRetireeIncreaseContract.percent_increase_value);

            LoadActiveRetireeIncreaseContractHistorys();
            //}

        }

        #endregion

        #region Public Methods

        public ArrayList btn_Approve()
        {
            ArrayList larrList = new ArrayList();
            int lintUserSerialId = utlPassInfo.iobjPassInfo.iintUserSerialID;

            busUser lbusUser = new busUser();
            lbusUser.FindUser(lintUserSerialId);

            icdoActiveRetireeIncreaseContract.contract_status_value = busConstant.BenefitCalculation.CALCULATION_STATUS_TYPE_APPROVED;
            icdoActiveRetireeIncreaseContract.approved_by = lbusUser.icdoUser.last_name + "," + lbusUser.icdoUser.first_name;
            icdoActiveRetireeIncreaseContract.Update();
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
            DateTime ldtEndDate = new DateTime(icdoActiveRetireeIncreaseContract.plan_year, 12, 31);
            icdoActiveRetireeIncreaseContract.effective_end_date = ldtEndDate;
            icdoActiveRetireeIncreaseContract.approved_by = lbusUser.icdoUser.last_name + "," + lbusUser.icdoUser.first_name;
            icdoActiveRetireeIncreaseContract.Update();
            larrList.Add(this);

            return larrList;
        }

        public busActiveRetireeIncreaseContract LoadActiveRetireeIncContractByPlanYear(int aintPlanYear)
        {
            DataTable ldtbContractInfo =
                Select("cdoActiveRetireeIncreaseContract.LoadContractByPlanYear", new object[1] { aintPlanYear });

            if (ldtbContractInfo.Rows.Count > 0)
            {
                busActiveRetireeIncreaseContract lbusActiveRetireeIncreaseContract =
                        new busActiveRetireeIncreaseContract { icdoActiveRetireeIncreaseContract = new cdoActiveRetireeIncreaseContract() };
                lbusActiveRetireeIncreaseContract.icdoActiveRetireeIncreaseContract.LoadData(ldtbContractInfo.Rows[0]);
                return lbusActiveRetireeIncreaseContract;
            }

            return null;
        }

        public Collection<busActiveRetireeIncreaseContract> LoadActiveRetireeIncContractByRetirementDate(DateTime adtRetirementDate)
        {
            Collection<busActiveRetireeIncreaseContract> lclbActiveRetireeIncreaseContract = new Collection<busActiveRetireeIncreaseContract>();
            DataTable ldtbContractInfo =
                Select("cdoActiveRetireeIncreaseContract.LoadContractByRetirementDate", new object[1] { adtRetirementDate });
            lclbActiveRetireeIncreaseContract = GetCollection<busActiveRetireeIncreaseContract>(ldtbContractInfo, "icdoActiveRetireeIncreaseContract");


            return lclbActiveRetireeIncreaseContract;
        }

        public Collection<busActiveRetireeIncreaseContract> LoadActiveRetireeIncContractByMDYear(int aintPlanYear)
        {
            Collection<busActiveRetireeIncreaseContract> lclbActiveRetireeIncreaseContract = new Collection<busActiveRetireeIncreaseContract>();
            DataTable ldtbContractInfo =
                Select("cdoActiveRetireeIncreaseContract.LoadContractByPlanYearSinceMDYear", new object[1] { aintPlanYear });
            lclbActiveRetireeIncreaseContract = GetCollection<busActiveRetireeIncreaseContract>(ldtbContractInfo, "icdoActiveRetireeIncreaseContract");


            return lclbActiveRetireeIncreaseContract;
        }

        #endregion

        #region Validations

        public bool CheckIfRetirementDateOverlapping()
        {
            if (icdoActiveRetireeIncreaseContract.retirement_date_from != DateTime.MinValue &&
                icdoActiveRetireeIncreaseContract.retirement_date_to != DateTime.MinValue)
            {
                DataTable ldtbList = busBase.Select("cdoActiveRetireeIncreaseContract.CheckForOverlappingDates",
                                     new object[] { icdoActiveRetireeIncreaseContract.plan_year, icdoActiveRetireeIncreaseContract.retirement_date_from,
                                 icdoActiveRetireeIncreaseContract.retirement_date_to});
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
