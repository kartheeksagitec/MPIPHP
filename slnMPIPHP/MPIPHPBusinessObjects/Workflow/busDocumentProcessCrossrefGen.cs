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
using Sagitec.Bpm;

#endregion

namespace MPIPHP.BusinessObjects
{
	[Serializable]
	public class busDocumentProcessCrossrefGen : busMPIPHPBase
    {
		public busDocumentProcessCrossrefGen()
		{

		}

		private cdoDocumentProcessCrossref _icdoDocumentProcessCrossref;
		public cdoDocumentProcessCrossref icdoDocumentProcessCrossref
		{
			get
			{
				return _icdoDocumentProcessCrossref;
			}
			set
			{
				_icdoDocumentProcessCrossref = value;
			}
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

		public bool FindDocumentProcessCrossref(int Aintdocumentprocesscrossrefid)
		{
			bool lblnResult = false;
			if (_icdoDocumentProcessCrossref == null)
			{
				_icdoDocumentProcessCrossref = new cdoDocumentProcessCrossref();
			}
			if (_icdoDocumentProcessCrossref.SelectRow(new object[1] { Aintdocumentprocesscrossrefid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

		public void LoadDocument()
		{
			if (_ibusDocument == null)
			{
				_ibusDocument = new busDocument();
			}			
			_ibusDocument.FindDocument(_icdoDocumentProcessCrossref.document_id);
		}

        /// <summary>
        /// Gets or sets the non-collection object of type busProcess.
        /// </summary>
        public busBpmProcess ibusBpmProcess { get; set; }


        /// <summary>
        /// MPIPHP.busActivityGen.LoadProcess():
        /// Loads non-collection object ibusProcess of type busProcess.
        /// </summary>
        public virtual void LoadProcess()
        {
            if (ibusBpmProcess == null)
            {
                ibusBpmProcess = new busBpmProcess();
            }
            ibusBpmProcess.FindByPrimaryKey(icdoDocumentProcessCrossref.process_id);
        }
	}
}
