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
    /// Class MPIPHP.BusinessObjects.busReimbursementDetails:
    /// Inherited from busReimbursementDetailsGen, the class is used to customize the business object busReimbursementDetailsGen.
	/// </summary>
	[Serializable]
    public class busReimbursementDetails : busReimbursementDetailsGen
	{


        public ArrayList CheckErrorOnAddButton(object aobj, Hashtable ahstParams, ref ArrayList aarrErrors, bool ablnHardErrorOnSave = false)
        {
            utlError lobjError = null;
            decimal ldecCheckAmount = 0M;
            busRepaymentSchedule lbusRepaymentSchedule = null;

            if (aobj is busRepaymentSchedule)
            {
                lbusRepaymentSchedule = aobj as busRepaymentSchedule;
            }
            ahstParams["icdoReimbursementDetails.amount_paid"] = ahstParams["amount_paid"];

            if (Convert.ToString(ahstParams["icdoReimbursementDetails.amount_paid"]).IsNotNullOrEmpty())
            {
                ldecCheckAmount = Convert.ToDecimal(ahstParams["icdoReimbursementDetails.amount_paid"]);
            }
            else
            {
                lobjError = AddError(0, "Please enter check Amount");
                aarrErrors.Add(lobjError);
                return aarrErrors;
            }


            if (lbusRepaymentSchedule.icdoRepaymentSchedule.repayment_schedule_id == 0)
            {
                lobjError = AddError(0, "Please save the Repayment Schedule before adding check details");
                aarrErrors.Add(lobjError);
                return aarrErrors;
            }

            return aarrErrors;
        }
	}
}
