#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using MPIPHP.BusinessObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
	[Serializable]
	public class busResourcesLookup : busMainBase
	{
        public Collection<busResources> iclbLookupResult { get; set; }
		
		public void LoadResources(DataTable adtbSearchResult)
		{
			iclbLookupResult = GetCollection<busResources>(adtbSearchResult, "icdoResources");
		}
	}
}
