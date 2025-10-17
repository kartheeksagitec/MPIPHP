#region Using directives

using System;
using Sagitec.Common;
using MPIPHP.CustomDataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public class busDocumentProcessCrossref : busDocumentProcessCrossrefGen
    {
        public string istrScreenName { get; set; } 

        private string _istrDocumentType;

        public string istrDocumentType
        {
            get { return _istrDocumentType; }
            set { _istrDocumentType = value; }
        }

        private string _istrDocumentName;

        public string istrDocumentName
        {
            get { return _istrDocumentName; }
            set { _istrDocumentName = value; }
        }
        public void LoadDocumentByCode()
        {
            if (ibusDocument == null)
            { ibusDocument = new busDocument(); }
            ibusDocument.FindDocumentByDocumentCode(istrDocumentType);
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (istrDocumentType.IsNotNullOrEmpty())
            {
                LoadDocumentByCode();
            }
            else
            {
                ibusDocument = new busDocument { icdoDocument = new cdoDocument() };
            }
            icdoDocumentProcessCrossref.document_id = ibusDocument.icdoDocument.document_id;
            base.BeforeValidate(aenmPageMode);
        }

        public override void LoadCorresProperties(string astrTemplateName)
        {
            #region RETR-0020
            istrScreenName = busGlobalFunctions.GetScreenName(this.iobjPassInfo.istrFormName);
            #endregion
        }
    }
}
