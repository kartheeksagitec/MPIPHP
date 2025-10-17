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
	public class busProcessInstanceChecklist : busProcessInstanceChecklistGen
	{
        private string _istrDocumentType;

        public string istrDocumentType
        {
            get { return _istrDocumentType; }
            set { _istrDocumentType = value; }
        }
        private busDocument _ibusDocument;
        public busDocument ibusDocument
        {
            get
            {
                return _ibusDocument;
            }
            set
            {
                _ibusDocument = value;
            }
        }

        public void LoadDocument()
        {
            if (ibusDocument == null)
            { ibusDocument = new busDocument(); }
            ibusDocument.FindDocumentByDocumentCode(istrDocumentType);
        }
	
	}
}
