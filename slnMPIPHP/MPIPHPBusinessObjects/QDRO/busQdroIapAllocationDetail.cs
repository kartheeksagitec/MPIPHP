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
	/// Class MPIPHP.BusinessObjects.busQdroIapAllocationDetail:
	/// Inherited from busQdroIapAllocationDetailGen, the class is used to customize the business object busQdroIapAllocationDetailGen.
	/// </summary>
	[Serializable]
	public class busQdroIapAllocationDetail : busQdroIapAllocationDetailGen
	{
        public void LoadData(int aintPersonAccountId, int aintComputationYear,
                            decimal adecQuarter1Factor, decimal adecQuarter2Factor,decimal adecQuarter3Factor,decimal adecQuarter4Factor,
                            decimal adecGainLossAmt, decimal adecBalanceAmt)
        {
            if (this.icdoQdroIapAllocationDetail == null)
            {
                this.icdoQdroIapAllocationDetail = new cdoQdroIapAllocationDetail();
            }

            this.icdoQdroIapAllocationDetail.person_account_id = aintPersonAccountId;
            this.icdoQdroIapAllocationDetail.computation_year = aintComputationYear;
            this.icdoQdroIapAllocationDetail.quarter_1_factor = adecQuarter1Factor;
            this.icdoQdroIapAllocationDetail.quarter_2_factor = adecQuarter2Factor;
            this.icdoQdroIapAllocationDetail.quarter_3_factor = adecQuarter3Factor;
            this.icdoQdroIapAllocationDetail.quarter_4_factor = adecQuarter4Factor;
            this.icdoQdroIapAllocationDetail.gain_loss_amount = adecGainLossAmt;
            this.icdoQdroIapAllocationDetail.balance_amount = adecBalanceAmt;
        }
	}
}
