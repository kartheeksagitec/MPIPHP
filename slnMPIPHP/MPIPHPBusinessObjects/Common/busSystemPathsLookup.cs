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
	public class busSystemPathsLookup : busMainBase
	{
        public Collection<busSystemPaths> iclbLookupResult { get; set; }
		
		public void LoadSystemPaths(DataTable adtbSearchResult)
		{
			iclbLookupResult = GetCollection<busSystemPaths>(adtbSearchResult, "icdoSystemPaths");
		}
	}
}
