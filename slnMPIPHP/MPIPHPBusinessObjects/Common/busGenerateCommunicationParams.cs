using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Sagitec.BusinessObjects;
using System.Runtime.Serialization;


namespace MPIPHP.BusinessObjects
{
    public enum enmFaxMode
    {
        NoFax,
        PrintAndSendFax,
        EFax
    }

    [Serializable()]
    public class busGenerateCommunicationParams
    {
        public string istrTemplateCode { get; set; }
        public busBase ibusBase { get; set; }
        public string istrCallingForm { get; set; }
        public string istrUserID { get; set; }
        public int iintUserSerialID { get; set; }
        public bool iblnSendCommunication { get; set; }
        public bool iblnInserSecureMessage { get; set; }
        public bool iblnFaxCoverSheet { get; set; }
        public Int16 iintDueDateVariation { get; set; }
        public enmFaxMode ienmFaxMode { get; set; }
        public string istrFaxNumber { get; set; }
        public string istrFaxNote { get; set; }
        public string istrPrinterName { get; set; }
        public int iintActivityInstanceId { get; set; }
        public Hashtable ihstQueryBookmarks { get { return _ihstQueryBookmarks; } }
        protected Hashtable _ihstQueryBookmarks;

        public busGenerateCommunicationParams(string astrTemplateCode, busBase abusBase, string astrCallingForm, string astrUserID, int aintUserSerialID, bool ablnSendCommunication, bool ablnInsertSecureMessage, Int16 aintDueDateVariation, Hashtable ahstQueryBookmarks = null, bool ablnFaxCoverSheet = false, enmFaxMode aenmFaxMode = enmFaxMode.NoFax, string astrFaxNumber = "", string astrFaxNote = "", string astrPrinterName = "")
        {
            istrTemplateCode = astrTemplateCode;
            ibusBase = abusBase;
            istrCallingForm = astrCallingForm;
            istrUserID = astrUserID;
            iintUserSerialID = aintUserSerialID;
            iblnSendCommunication = ablnSendCommunication;
            iblnInserSecureMessage = ablnInsertSecureMessage;            
            _ihstQueryBookmarks = ahstQueryBookmarks;
            iblnFaxCoverSheet = ablnFaxCoverSheet;
            ienmFaxMode = aenmFaxMode;
            istrFaxNumber = astrFaxNumber;
            istrFaxNote = astrFaxNote;
            iintDueDateVariation = aintDueDateVariation;
            istrPrinterName = astrPrinterName;
        }


    }
}
