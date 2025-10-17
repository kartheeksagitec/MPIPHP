using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPIPHP.Common.Exceptions.Communication
{
    [Serializable()]
    public class InvalidCommunicationStatusException : CommunicationException
    {
        private string _istrCommunicationStatus;

        protected string istrCommunicationStatus
        {
            get { return _istrCommunicationStatus; }
        }

        public InvalidCommunicationStatusException(int aintTemplateID, int aintTrackingID, int aintCommSecureMessageID, string astrCommunicationStatus, string message)
            : base(aintTemplateID, aintTrackingID, 0, message)
        {
            _istrCommunicationStatus = astrCommunicationStatus;
        }
    }
}
