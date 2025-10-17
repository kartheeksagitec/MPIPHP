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
	public class busMessagesLookup : busMainBase
	{
        public Collection<busMessages> iclbLookupResult { set; get; }
		
		public void LoadMessages(DataTable adtbSearchResult)
		{
			iclbLookupResult = GetCollection<busMessages>(adtbSearchResult, "icdoMessages");
		}
	}
}
