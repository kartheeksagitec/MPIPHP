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
	/// Class MPIPHP.BusinessObjects.busPayeeBenefitAccount:
	/// Inherited from busPayeeBenefitAccountGen, the class is used to customize the business object busPayeeBenefitAccountGen.
	/// </summary>
	[Serializable]
	public class busPayeeBenefitAccount : busPayeeBenefitAccountGen
	{
        public busPerson ibusParticipant { get; set; }


        public int ManagePayeeBenefitAccount(int aintPayeeBenefitAccountId, int aintPersonId, int aintPersonAccountId, decimal adecStartingTaxableAmount, decimal adecStartingNonTaxableAmount, decimal adecGrossAmount, string astrFundsTypeValue)
        {
            if (aintPayeeBenefitAccountId > 0)
            {
                this.FindPayeeBenefitAccount(aintPayeeBenefitAccountId);
            }

            this.icdoPayeeBenefitAccount.person_id = aintPersonId;
            this.icdoPayeeBenefitAccount.person_account_id = aintPersonAccountId;
            this.icdoPayeeBenefitAccount.starting_taxable_amount = adecStartingTaxableAmount;
            this.icdoPayeeBenefitAccount.starting_nontaxable_amount = adecStartingNonTaxableAmount;
            this.icdoPayeeBenefitAccount.gross_amount = adecGrossAmount;
            this.icdoPayeeBenefitAccount.funds_type_value = astrFundsTypeValue;

            if (aintPayeeBenefitAccountId > 0)
            {
                if (this.icdoPayeeBenefitAccount.Update() == 1)
                    return this.icdoPayeeBenefitAccount.payee_benefit_account_id;
            }
            else
            {
                if (this.icdoPayeeBenefitAccount.Insert() == 1)
                    return this.icdoPayeeBenefitAccount.payee_benefit_account_id;
            }

            return 0;
        }
	}
}
