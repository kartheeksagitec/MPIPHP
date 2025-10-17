#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using MPIPHP.BusinessObjects;
using Sagitec.DBUtility;

#endregion

namespace MPIPHP.BusinessObjects
{
	[Serializable]
	public class busUserLookup : busMainBase
	{
        public Collection<busUser> iclbLookupResult { set; get; }
		
		public void LoadUsers(DataTable adtbSearchResult)
		{
			iclbLookupResult = GetCollection<busUser>(adtbSearchResult, "icdoUser");
		}
	}
}
