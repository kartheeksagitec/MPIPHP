#region Using directives

using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using MPIPHP.BusinessObjects;
using MPIPHP.CustomDataObjects;
using Sagitec.Common;

#endregion

namespace MPIPHP.BusinessTier
{
    public class srvEligibilityRules : srvMPIPHP
	{
		public srvEligibilityRules()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public busBenefitProvisionEligibility FindBenefitProvisionEligibility(int aintbenefitprovisioneligibilityid)
		{
			busBenefitProvisionEligibility lobjBenefitProvisionEligibility = new busBenefitProvisionEligibility();
			if (lobjBenefitProvisionEligibility.FindBenefitProvisionEligibility(aintbenefitprovisioneligibilityid))
			{
			}

			return lobjBenefitProvisionEligibility;
		}

		public busBenefitProvisionEligibility NewBenefitProvisionEligibility()
		{
			busBenefitProvisionEligibility lobjBenefitProvisionEligibility = new busBenefitProvisionEligibility();
                        lobjBenefitProvisionEligibility.icdoBenefitProvisionEligibility = new cdoBenefitProvisionEligibility();
			return lobjBenefitProvisionEligibility;
		}

		public busBenefitProvisionEligibilityLookup LoadBenefitProvisionEligibilitys(DataTable adtbSearchResult)
		{
			busBenefitProvisionEligibilityLookup lobjBenefitProvisionEligibilityLookup = new busBenefitProvisionEligibilityLookup();
			lobjBenefitProvisionEligibilityLookup.LoadBenefitProvisionEligibilitys(adtbSearchResult);
			return lobjBenefitProvisionEligibilityLookup;
		}
	}
}
