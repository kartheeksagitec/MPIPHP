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
	/// Class MPIPHP.BusinessObjects.busPaymentHistoryHeaderLookup:
	/// Inherited from busPaymentHistoryHeaderLookupGen, this class is used to customize the lookup business object busPaymentHistoryHeaderLookupGen. 
	/// </summary>
	[Serializable]
	public class busPaymentHistoryHeaderLookup : busPaymentHistoryHeaderLookupGen
	{
        protected override void LoadOtherObjects(DataRow adtrRow, busBase abusBase)
        {
            base.LoadOtherObjects(adtrRow, abusBase);
            busPaymentHistoryHeader lbusPaymentHistoryHeader = (busPaymentHistoryHeader)abusBase;
            
            lbusPaymentHistoryHeader.ibusPayee = new busPerson() { icdoPerson = new cdoPerson() };
            lbusPaymentHistoryHeader.ibusPayee.icdoPerson.mpi_person_id = adtrRow[enmPerson.mpi_person_id.ToString()].ToString();
            lbusPaymentHistoryHeader.ibusPayee.istrFullName = adtrRow["PayeeName"].ToString();

            lbusPaymentHistoryHeader.ibusOrganization = new busOrganization() { icdoOrganization = new cdoOrganization() };
            lbusPaymentHistoryHeader.ibusOrganization.icdoOrganization.mpi_org_id = adtrRow[enmOrganization.mpi_org_id.ToString()].ToString();
            lbusPaymentHistoryHeader.ibusOrganization.icdoOrganization.org_name = adtrRow[enmOrganization.org_name.ToString()].ToString();

            if ((adtrRow[enmPlan.plan_id.ToString()] != DBNull.Value) && (adtrRow[enmPlan.plan_code.ToString()] != DBNull.Value))
            {
                lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.istrPlanDescription = Convert.ToString(adtrRow[enmPlan.plan_code.ToString()]);
            }

            if ((adtrRow[enmPaymentHistoryDistribution.payment_method_id.ToString()] != DBNull.Value) && (adtrRow[enmPaymentHistoryDistribution.payment_method_value.ToString()] != DBNull.Value))
            {
                lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.istrPaymentMethod = busGlobalFunctions.GetCodeValueDescriptionByValue(Convert.ToInt32(adtrRow[enmPaymentHistoryDistribution.payment_method_id.ToString()]), Convert.ToString(adtrRow[enmPaymentHistoryDistribution.payment_method_value.ToString()])).description;
            }


            if ((adtrRow[enmPaymentHistoryDistribution.distribution_status_id.ToString()] != DBNull.Value) && (adtrRow[enmPaymentHistoryDistribution.distribution_status_value.ToString()] != DBNull.Value))
            {
                lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.istrDistributionCodeDescription = busGlobalFunctions.GetCodeValueDescriptionByValue(Convert.ToInt32(adtrRow[enmPaymentHistoryDistribution.distribution_status_id.ToString()]), Convert.ToString(adtrRow[enmPaymentHistoryDistribution.distribution_status_value.ToString()])).description;
            }

            if (adtrRow[enmPaymentHistoryDistribution.check_number.ToString()] != DBNull.Value)
            {
                lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.istrCheckNumber = Convert.ToString(adtrRow[enmPaymentHistoryDistribution.check_number.ToString()]);
            }

            if (adtrRow["PAYMENT_DATE"] != DBNull.Value)
            {
                lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.idtPaymentDate = Convert.ToDateTime(adtrRow["PAYMENT_DATE"]);
            }

            if (adtrRow["GROSS_AMOUNT"] != DBNull.Value)
            {
                lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.gross_amount = Convert.ToDecimal(adtrRow["GROSS_AMOUNT"]);
            }

            if (adtrRow["NET_AMOUNT"] != DBNull.Value)
            {
                lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.net_amount = Convert.ToDecimal(adtrRow["NET_AMOUNT"]);
            }

            if (adtrRow["DEDUCTION_AMOUNT"] != DBNull.Value)
            {
                lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.deduction_amount = Convert.ToDecimal(adtrRow["DEDUCTION_AMOUNT"]);
            }

            if (adtrRow["FEDRAL_TAX"] != DBNull.Value)
            {
                lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.idecfedraltax = Convert.ToDecimal(adtrRow["FEDRAL_TAX"]);
            }

            if (adtrRow["STATE_TAX"] != DBNull.Value)
            {
                lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.idecstatetax = Convert.ToDecimal(adtrRow["STATE_TAX"]);
            }

            if (adtrRow["PAYMENT_TYPE"] != DBNull.Value)
            {
                lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.istrPaymentType = Convert.ToString(adtrRow["PAYMENT_TYPE"]);
            }

        }
	}
}
