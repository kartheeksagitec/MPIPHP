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
	/// Class MPIPHP.BusinessObjects.busPayeeAccountLookup:
	/// Inherited from busPayeeAccountLookupGen, this class is used to customize the lookup business object busPayeeAccountLookupGen. 
	/// </summary>
	[Serializable]
	public class busPayeeAccountLookup : busPayeeAccountLookupGen
	{
        protected override void LoadOtherObjects(DataRow adtrRow, busBase abusBase)
        {
            base.LoadOtherObjects(adtrRow, abusBase);
            busPayeeAccount lbusPayeeAccount = (busPayeeAccount)abusBase;
            lbusPayeeAccount.ibusParticipant = new busPerson() { icdoPerson = new cdoPerson() };
            lbusPayeeAccount.ibusParticipant.icdoPerson.mpi_person_id = adtrRow["ParticipantID"].ToString();
            lbusPayeeAccount.ibusParticipant.istrFullName = adtrRow["ParticipantName"].ToString();

            lbusPayeeAccount.ibusPayee = new busPerson() { icdoPerson = new cdoPerson() };
            lbusPayeeAccount.ibusPayee.icdoPerson.mpi_person_id = adtrRow[enmPerson.mpi_person_id.ToString()].ToString();
            lbusPayeeAccount.ibusPayee.istrFullName = adtrRow["PayeeName"].ToString();

            lbusPayeeAccount.ibusOrganization = new busOrganization() { icdoOrganization = new cdoOrganization() };
            lbusPayeeAccount.ibusOrganization.icdoOrganization.mpi_org_id = adtrRow[enmOrganization.mpi_org_id.ToString()].ToString();
            lbusPayeeAccount.ibusOrganization.icdoOrganization.org_name = adtrRow[enmOrganization.org_name.ToString()].ToString();

            lbusPayeeAccount.ibusBenefitApplication = new busBenefitApplication() { icdoBenefitApplication = new cdoBenefitApplication() };
            if (adtrRow[enmBenefitApplication.retirement_date.ToString()] != DBNull.Value)
            {
                lbusPayeeAccount.ibusBenefitApplication.icdoBenefitApplication.retirement_date = Convert.ToDateTime(adtrRow[enmBenefitApplication.retirement_date.ToString()]);
            }

            if (adtrRow[enmDroApplication.dro_commencement_date.ToString()] != DBNull.Value)
            {
                lbusPayeeAccount.ibusBenefitApplication.icdoBenefitApplication.retirement_date = Convert.ToDateTime(adtrRow[enmDroApplication.dro_commencement_date.ToString()]);
            }

            if (adtrRow[enmBenefitApplication.withdrawal_date.ToString()] != DBNull.Value)
            {
                lbusPayeeAccount.ibusBenefitApplication.icdoBenefitApplication.withdrawal_date = Convert.ToDateTime(adtrRow[enmBenefitApplication.withdrawal_date.ToString()]);
            }

            lbusPayeeAccount.ibusBenefitCalculationHeader = new busBenefitCalculationHeader() { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
            if (adtrRow[enmBenefitCalculationHeader.benefit_commencement_date.ToString()] != DBNull.Value)
            {
                lbusPayeeAccount.ibusBenefitCalculationHeader.icdoBenefitCalculationHeader.benefit_commencement_date = Convert.ToDateTime(adtrRow[enmBenefitCalculationHeader.benefit_commencement_date.ToString()]);
            }

            if ((adtrRow[enmPayeeAccountStatus.status_id.ToString()] != DBNull.Value) && (adtrRow[enmPayeeAccountStatus.status_value.ToString()] != DBNull.Value))
            {
                lbusPayeeAccount.istrPayeeStatus = busGlobalFunctions.GetCodeValueDescriptionByValue(Convert.ToInt32(adtrRow[enmPayeeAccountStatus.status_id.ToString()]), Convert.ToString(adtrRow[enmPayeeAccountStatus.status_value.ToString()])).description;
            }

            if ((adtrRow[enmPlanBenefitXr.benefit_option_id.ToString()] != DBNull.Value) && (adtrRow[enmPlanBenefitXr.benefit_option_value.ToString()] != DBNull.Value))
            {
                lbusPayeeAccount.icdoPayeeAccount.istrBenefitOption = busGlobalFunctions.GetCodeValueDescriptionByValue(Convert.ToInt32(adtrRow[enmPlanBenefitXr.benefit_option_id.ToString()]), Convert.ToString(adtrRow[enmPlanBenefitXr.benefit_option_value.ToString()])).description;
            }

            if ((adtrRow[enmPlan.plan_id.ToString()] != DBNull.Value) && (adtrRow[enmPlan.plan_code.ToString()] != DBNull.Value))
            {
                lbusPayeeAccount.icdoPayeeAccount.istrPlanDescription = Convert.ToString(adtrRow[enmPlan.plan_code.ToString()]);
            }
        }
	}
}
