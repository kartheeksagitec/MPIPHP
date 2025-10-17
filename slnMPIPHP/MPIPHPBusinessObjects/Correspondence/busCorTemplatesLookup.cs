#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using MPIPHP.BusinessObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
	[Serializable]
	public class busCorTemplatesLookup : busMainBase
	{

		public Collection<busCorTemplates> iclbLookupResult {get;set;}

		public void LoadCorTemplates(DataTable adtbSearchResult)
		{
            iclbLookupResult = GetCollection<busCorTemplates>(adtbSearchResult, "icdoCorTemplates");
		}



	}
}
