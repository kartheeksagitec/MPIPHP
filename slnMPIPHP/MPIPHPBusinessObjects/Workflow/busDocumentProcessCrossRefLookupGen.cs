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
	public class busDocumentProcessCrossRefLookupGen : busMainBase
	{

		private Collection<busDocumentProcessCrossref> _iclbDocumentProcessCrossref;
		public Collection<busDocumentProcessCrossref> iclbDocumentProcessCrossref
		{
			get
			{
				return _iclbDocumentProcessCrossref;
			}
			set
			{
				_iclbDocumentProcessCrossref = value;
			}
		}

		public void LoadDocumentProcessCrossrefs(DataTable adtbSearchResult)
		{
			_iclbDocumentProcessCrossref = GetCollection<busDocumentProcessCrossref>(adtbSearchResult, "icdoDocumentProcessCrossref");
		}
	}
}
