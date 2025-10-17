#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using MPIPHP.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;

#endregion

namespace MPIPHP.BusinessObjects
{
	[Serializable]
	public class busJobHeaderLookupGen : busMainBase
	{

		private Collection<busJobHeader> _iclbJobHeader;
		public Collection<busJobHeader> iclbJobHeader
		{
			get
			{
				return _iclbJobHeader;
			}
			set
			{
				_iclbJobHeader = value;
			}
		}

		public void LoadJobHeaders(DataTable adtbSearchResult)
		{
			_iclbJobHeader = GetCollection<busJobHeader>(adtbSearchResult, "icdoJobHeader");
		}
	}
}
