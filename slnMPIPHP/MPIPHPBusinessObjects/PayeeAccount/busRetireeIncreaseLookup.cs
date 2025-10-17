#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busRetireeIncreaseLookup:
	/// Inherited from busRetireeIncreaseLookupGen, this class is used to customize the lookup business object busRetireeIncreaseLookupGen. 
	/// </summary>
	[Serializable]
	public class busRetireeIncreaseLookup : busRetireeIncreaseLookupGen
	{
        protected override void LoadOtherObjects(DataRow adtrRow, busBase abusBase)
        {
            base.LoadOtherObjects(adtrRow, abusBase);
            busPayeeAccount lbusPayeeAccount = (busPayeeAccount)abusBase;
            lbusPayeeAccount.ibusParticipant = new busPerson() { icdoPerson = new cdoPerson() };
            lbusPayeeAccount.ibusParticipant.icdoPerson.mpi_person_id = adtrRow["PARTICIPANT_MPI_ID"].ToString();

            lbusPayeeAccount.ibusPayee = new busPerson() { icdoPerson = new cdoPerson() };
            lbusPayeeAccount.ibusPayee.icdoPerson.mpi_person_id = adtrRow[enmPerson.mpi_person_id.ToString()].ToString();

            if (adtrRow["GROSS_AMOUNT"] != DBNull.Value)
            {
                lbusPayeeAccount.icdoPayeeAccount.idecGrossAmt = Convert.ToDecimal(adtrRow["GROSS_AMOUNT"]);
            }

            if (adtrRow["DEDUCTION_AMOUNT"] != DBNull.Value)
            {
                lbusPayeeAccount.icdoPayeeAccount.idecDeductionAmt = Convert.ToDecimal(adtrRow["DEDUCTION_AMOUNT"]);
            }

            if (adtrRow["ROLLOVER_FLAG"] != DBNull.Value)
            {
                lbusPayeeAccount.icdoPayeeAccount.istrRolloverEligible = Convert.ToString(adtrRow["ROLLOVER_FLAG"]);
            }

            if (adtrRow["CREATED_DATE"] != DBNull.Value)
            {
                lbusPayeeAccount.icdoPayeeAccount.idtRolloverCraetedDate = Convert.ToDateTime(adtrRow["CREATED_DATE"]);
            }

            if (adtrRow["NET_AMOUNT"] != DBNull.Value)
            {
                lbusPayeeAccount.icdoPayeeAccount.idecNetAmount = Convert.ToDecimal(adtrRow["NET_AMOUNT"]);
            }

            if (adtrRow["FEDRAL_TAX"] != DBNull.Value)
            {
                lbusPayeeAccount.icdoPayeeAccount.idecFederalTax = Convert.ToDecimal(adtrRow["FEDRAL_TAX"]);
            }

            if (adtrRow["STATE_TAX"] != DBNull.Value)
            {
                lbusPayeeAccount.icdoPayeeAccount.idecStateTax = Convert.ToDecimal(adtrRow["STATE_TAX"]);
            }

            if ((adtrRow[enmPlan.plan_id.ToString()] != DBNull.Value) && (adtrRow[enmPlan.plan_code.ToString()] != DBNull.Value))
            {
                lbusPayeeAccount.icdoPayeeAccount.istrPlanDescription = Convert.ToString(adtrRow[enmPlan.plan_code.ToString()]);
            }

            if ((adtrRow[enmPayeeAccountStatus.status_id.ToString()] != DBNull.Value) && (adtrRow[enmPayeeAccountStatus.status_value.ToString()] != DBNull.Value))
            {
                lbusPayeeAccount.icdoPayeeAccount.istrPayeeAccountCurrentStatus = busGlobalFunctions.GetCodeValueDescriptionByValue(Convert.ToInt32(adtrRow[enmPayeeAccountStatus.status_id.ToString()]), Convert.ToString(adtrRow[enmPayeeAccountStatus.status_value.ToString()])).description;
            }

            if (adtrRow["MONTHLY_STATUS_VALUE"] != DBNull.Value)
            {
                lbusPayeeAccount.icdoPayeeAccount.istrMonthlyPayeeAccountCurrentStatus = busGlobalFunctions.GetCodeValueDescriptionByValue(Convert.ToInt32(adtrRow[enmPayeeAccountStatus.status_id.ToString()]), Convert.ToString(adtrRow["MONTHLY_STATUS_VALUE"])).description;
            }
        }
	}
}
