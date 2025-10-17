#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
	[Serializable]
	public class busDocumentGen : busMPIPHPBase
    {
		public busDocumentGen()
		{

		}

		private cdoDocument _icdoDocument;
		public cdoDocument icdoDocument
		{
			get
			{
				return _icdoDocument;
			}
			set
			{
				_icdoDocument = value;
			}
		}

		public bool FindDocument(int Aintdocumentid)
		{
			bool lblnResult = false;
			if (_icdoDocument == null)
			{
				_icdoDocument = new cdoDocument();
			}
			if (_icdoDocument.SelectRow(new object[1] { Aintdocumentid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        public bool FindDocumentByDocumentCode(string astrDocumentCode)
        {
            if (_icdoDocument == null)
            {
                _icdoDocument = new cdoDocument();
            }
            DataTable ldtbDocument = Select<cdoDocument>(new string[1] { "doc_type" },
                  new object[1] { astrDocumentCode }, null, null);
            if (ldtbDocument.Rows.Count > 0)
            {
                _icdoDocument.LoadData(ldtbDocument.Rows[0]);
                return true;
            }
            return false;
        }
	}
}
