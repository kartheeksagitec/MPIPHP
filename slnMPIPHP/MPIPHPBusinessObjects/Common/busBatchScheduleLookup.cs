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
	public class busBatchScheduleLookup : busMainBase
	{
        public Collection<busBatchSchedule> iclbLookupResult { set; get; }
		
		public void LoadBatchSchedule(DataTable adtbSearchResult)
		{
			iclbLookupResult = GetCollection<busBatchSchedule>(adtbSearchResult, "icdoBatchSchedule");
		}


	}
}
