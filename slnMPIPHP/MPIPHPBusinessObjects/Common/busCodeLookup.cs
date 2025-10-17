#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.BusinessObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
	[Serializable]
	public class busCodeLookup : busMainBase
	{
        public Collection<busCode> iclbLookupResult { set; get; }
		
		public void LoadCodes(DataTable adtbSearchResult)
		{
			iclbLookupResult = GetCollection<busCode>(adtbSearchResult, "icdoCode");
		}
	}
}
