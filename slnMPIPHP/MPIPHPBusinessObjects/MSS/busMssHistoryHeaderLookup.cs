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

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busHistoryHeaderLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busMssHistoryHeaderLookup : busPaymentHistoryHeaderLookup
	{
        public string istrMPID { get; set; }
        /*
		/// <summary>
		/// Gets or sets the collection object of type busPaymentHistoryHeader. 
		/// </summary>
		public Collection<busPaymentHistoryHeader> iclbPaymentHistoryHeader { get; set; }


		/// <summary>
		/// MPIPHP.BusinessObjects.busHistoryHeaderLookupGen.LoadPaymentHistoryHeaders(DataTable):
		/// Loads Collection object iclbPaymentHistoryHeader of type busPaymentHistoryHeader.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busHistoryHeaderLookupGen.iclbPaymentHistoryHeader</param>
		public virtual void LoadPaymentHistoryHeaders(DataTable adtbSearchResult)
		{
			iclbPaymentHistoryHeader = GetCollection<busPaymentHistoryHeader>(adtbSearchResult, "icdoPaymentHistoryHeader");
		}*/

       

        protected override void LoadOtherObjects(DataRow adtrRow, busBase abusBase)
        {
            base.LoadOtherObjects(adtrRow, abusBase);
            busPaymentHistoryHeader lbusPaymentHistoryHeader = (busPaymentHistoryHeader)abusBase;
            if (lbusPaymentHistoryHeader.ibusPayee.icdoPerson.mpi_person_id == Convert.ToString(adtrRow["Part_MPID"]))
            {
                lbusPaymentHistoryHeader.ibusPayee.istrFullName = "Self";
            }
            if (lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.istrPlanDescription == busConstant.MPIPP)
            {
                lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.istrPlanDescription = "Pension Plan";
            }
            if (lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.istrPlanDescription == busConstant.IAP)
            {
                lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.istrPlanDescription = busConstant.IAP_PLAN;
            }
            if (adtrRow["RETRO_AMOUNT"] != DBNull.Value)
            {
                lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.retro_amount = Convert.ToDecimal(adtrRow["RETRO_AMOUNT"]);
            }
            if (adtrRow["FUNDS_TYPE_VALUE"] != DBNull.Value)
            {
                lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.istrFundsType = Convert.ToString(adtrRow["FUNDS_TYPE_VALUE"]);
            }
            if (adtrRow["BANK_NAME"] != DBNull.Value)
            {
                lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.istrBankName = Convert.ToString(adtrRow["BANK_NAME"]);
            }
            if (adtrRow["BENEFIT_ACCOUNT_TYPE_VALUE"] != DBNull.Value)
            {
                lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.istrBenefitType = busGlobalFunctions.GetCodeValueDescriptionByValue(Convert.ToInt32(adtrRow["BENEFIT_ACCOUNT_TYPE_ID"]), Convert.ToString(adtrRow["BENEFIT_ACCOUNT_TYPE_VALUE"])).description;
            }
            if (adtrRow["PAYMENT_METHOD_VALUE"] != DBNull.Value && lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.istrPaymentMethod.Equals(busConstant.PAYMENT_METHOD_ACH))
            {
                lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.istrPaymentMethod = busConstant.PAYMENT_METHOD_DD;
            }
        }
	}
}
