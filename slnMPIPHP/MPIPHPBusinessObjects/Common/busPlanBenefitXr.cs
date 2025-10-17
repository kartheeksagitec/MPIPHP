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
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busPlanBenefitXr:
	/// Inherited from busPlanBenefitXrGen, the class is used to customize the business object busPlanBenefitXrGen.
	/// </summary>
	[Serializable]
	public class busPlanBenefitXr : busPlanBenefitXrGen
	{
        public int GetPlanBenefitId(int aintPlanId, string astrBenefitOption)
        {
            DataTable ldtbList = Select<cdoPlanBenefitXr>(
                new string[2] { enmPlanBenefitXr.plan_id.ToString(), enmPlanBenefitXr.benefit_option_value.ToString() },
                new object[2] { aintPlanId, astrBenefitOption }, null, null);
            if (ldtbList.Rows.Count > 0)
            {
                return Convert.ToInt32(ldtbList.Rows[0][enmPlanBenefitXr.plan_benefit_id.ToString()]);
            }
            else
            {
                return 0;
            }
        }
	}
}
