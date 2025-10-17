#region Using directives

using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using NeoSpin.BusinessObjects;
using Sagitec.Common;

#endregion

namespace NeoSpin.BusinessTier
{
	public class srvOrganizationPOSBase : srvDefault
	{
		public srvOrganizationPOSBase()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public busOrganizationPOSBase FindByPrimaryKey(int aintPOSBaseId)
		{
			busOrganizationPOSBase lobjOrganizationPOSBase = new busOrganizationPOSBase();
			lobjOrganizationPOSBase.ienuLoadDepth = LoadDepth.High;
			lobjOrganizationPOSBase.iobjPassInfo = iobjPassInfo;
			lobjOrganizationPOSBase.FindByPrimaryKey(aintPOSBaseId);
			return lobjOrganizationPOSBase;
		}

		public busOrganizationPOSBase LoadRelatedObjects(int aintOrgID)
		{
			busOrganizationPOSBase lobjOrganizationPOSBase = new busOrganizationPOSBase();
			lobjOrganizationPOSBase.ienuLoadDepth = LoadDepth.High;
			lobjOrganizationPOSBase.iobjPassInfo = iobjPassInfo;
			//lobjOrganizationPOSBase.LoadRelatedObjects(aintOrgID);
			return lobjOrganizationPOSBase;
		}
		public busOrganizationPOSBaseLookup GetSearchResult(DataTable adtbSearchResult)
		{
			busOrganizationPOSBaseLookup lobjOrganizationPOSBaseLookup = new busOrganizationPOSBaseLookup();
			lobjOrganizationPOSBaseLookup.LoadSearchResult(adtbSearchResult, iobjPassInfo);
			return lobjOrganizationPOSBaseLookup;
		}
		public busOrganizationPOSLookup GetSearchResultPOS(DataTable adtbSearchResult)
		{
			busOrganizationPOSLookup lobjOrganizationPOSLookup = new busOrganizationPOSLookup();
			lobjOrganizationPOSLookup.LoadSearchResult(adtbSearchResult, iobjPassInfo);
			return lobjOrganizationPOSLookup;
		}

		public busOrganizationPOS FindByPrimaryKeyPOS(int aintPOSId)
		{
			busOrganizationPOS lobjOrganizationPOS = new busOrganizationPOS();
			lobjOrganizationPOS.ienuLoadDepth = LoadDepth.High;
			lobjOrganizationPOS.iobjPassInfo = iobjPassInfo;
			lobjOrganizationPOS.FindByPrimaryKey(aintPOSId);
			return lobjOrganizationPOS;
		}
	}
}
