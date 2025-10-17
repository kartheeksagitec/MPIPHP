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
	public class busRolesLookup : busMainBase
	{
        public Collection<busRoles> iclbLookupResult { get; set; }
		
		public void LoadRoles(DataTable adtbSearchResult)
		{
			iclbLookupResult = GetCollection<busRoles>(adtbSearchResult, "icdoRoles");
		}
	}
}
