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

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busIapAllocationFactor:
	/// Inherited from busIapAllocationFactorGen, the class is used to customize the business object busIapAllocationFactorGen.
	/// </summary>
	[Serializable]
	public class busIapAllocationFactor : busIapAllocationFactorGen
	{
        /// <summary>
        /// Method to load iap allocation factor business object based on plan year
        /// </summary>
        /// <param name="aintPlanYear">Plan Year</param>
        public void LoadIAPAllocationFactorByPlanYear(int aintPlanYear)
        {
            DataTable ldtResult = Select<cdoIapAllocationFactor>(new string[1] { "plan_year" }, new object[1] { aintPlanYear }, null, null);
            icdoIapAllocationFactor = new cdoIapAllocationFactor();
            if (ldtResult != null && ldtResult.Rows.Count > 0)
                icdoIapAllocationFactor.LoadData(ldtResult.Rows[0]);
        }

        public bool IsIAPAllocationFactorAlreadyExistsForTheYear()
        {
            bool lblnResult = false;

            DataTable ldtIAPFactor = SelectWithOperator<cdoIapAllocationFactor>(new string[2] { "plan_year", "iap_allocation_factor_id" }, new string[2] { "=", "<>" }, 
                new object[2] { icdoIapAllocationFactor.plan_year, icdoIapAllocationFactor.iap_allocation_factor_id }, null);
            if (ldtIAPFactor != null && ldtIAPFactor.Rows.Count > 0)
                lblnResult = true;

            return lblnResult;
        }

        public bool IsHistoricFactorsUpdated()
        {
            bool lblnResult = false;

            if (icdoIapAllocationFactor.plan_year < DateTime.Now.Year)
                lblnResult = true;
            else if (DateTime.Now.Month >= 7 && DateTime.Now.Month <= 9 && idecOldQ1Alloc1Factor != icdoIapAllocationFactor.alloc1_qf1_factor)
                lblnResult = true;
            else if (DateTime.Now.Month >= 10 && DateTime.Now.Month <= 12 && idecOldQ2Alloc1Factor != icdoIapAllocationFactor.alloc1_qf2_factor)
                lblnResult = true;

            return lblnResult;
        }

        public decimal idecOldQ1Alloc1Factor { get; set; }
        public decimal idecOldQ2Alloc1Factor { get; set; }
        public decimal idecOldQ3Alloc1Factor { get; set; }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (icdoIapAllocationFactor.ihstOldValues.Count > 0)
            {
                if (icdoIapAllocationFactor.ihstOldValues["alloc1_qf1_factor"] != null)
                    idecOldQ1Alloc1Factor = Convert.ToDecimal(icdoIapAllocationFactor.ihstOldValues["alloc1_qf1_factor"]);
                if (icdoIapAllocationFactor.ihstOldValues["alloc1_qf2_factor"] != null)
                    idecOldQ2Alloc1Factor = Convert.ToDecimal(icdoIapAllocationFactor.ihstOldValues["alloc1_qf2_factor"]);
                if (icdoIapAllocationFactor.ihstOldValues["alloc1_qf3_factor"] != null)
                    idecOldQ3Alloc1Factor = Convert.ToDecimal(icdoIapAllocationFactor.ihstOldValues["alloc1_qf3_factor"]);
            }
            base.BeforeValidate(aenmPageMode);
        }
	}
}
