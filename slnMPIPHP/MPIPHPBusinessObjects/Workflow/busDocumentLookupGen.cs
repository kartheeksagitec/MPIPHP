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
	public class busDocumentLookupGen : busMainBase
	{

		private Collection<busDocument> _iclbDocument;
		public Collection<busDocument> iclbDocument
		{
			get
			{
				return _iclbDocument;
			}
			set
			{
				_iclbDocument = value;
			}
		}

		public void LoadDocuments(DataTable adtbSearchResult)
		{
			_iclbDocument = GetCollection<busDocument>(adtbSearchResult, "icdoDocument");
		}
	}
}
