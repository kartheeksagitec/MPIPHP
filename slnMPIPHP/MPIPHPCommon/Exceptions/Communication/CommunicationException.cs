using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPIPHP.Common.Exceptions
{
    [Serializable()]
    public class CommunicationException : MPIPHPException
    {
        private int _iintTrackingID;
        private int _iintTemplateID;
        private int _iintCommSecureMessageID;
        private int _iintPersonID;
        private int _iintOrgContactID;
        private int _iintInternalUserID;

        protected enmCommunicationRecipientGroup _ienmCommunicationRecipientGroup;

        protected const string PERSON = "Person";
        protected const string ORG_CONTACT = "Org Contact";
        protected const string INTERNAL_USER_ID = "Internal User";
        protected const string ERROR_MESSAGE_UNDELIVERABLE_ADDRESS = "Communication Error: Address ID {0} is non-deliverable for {1} ID = {2}";
        protected const string ERROR_MESSAGE_ADDRESS_NOT_FOUND = "Communication Error: No address found for {0} ID = {1}";
        protected const string ERROR_MESSAGE_COMMUNICATION_RECIPIENT_NOT_SET = "Communication Error: Recipient not set";
        public int iintCommSecureMessageID
        {
            get { return _iintCommSecureMessageID; }
            set { _iintCommSecureMessageID = value; }
        }

        public int iintTemplateID
        {
            get { return _iintTemplateID; }
            set { _iintTemplateID = value; }
        }
        public int iintTrackingID
        {
            get { return _iintTrackingID; }
            set { _iintTrackingID = value; }
        }

        public int iintPersonID
        {
            get { return _iintPersonID; }
            set { _iintPersonID = value; }
        }

        public int iintOrgContactID
        {
            get { return _iintOrgContactID; }
            set { _iintOrgContactID = value; }
        }

        public int iintInternalUserID
        {
            get { return _iintInternalUserID; }
            set { _iintInternalUserID = value; }
        }

        public CommunicationException(string message)
            : base(message)
        {
            _ienmCommunicationRecipientGroup = enmCommunicationRecipientGroup.NotSet;
        }

        public CommunicationException(int aintTemplateID, int aintTrackingID, int aintCommSecureMessageID, string message)
            : this(message)
        {
            _iintCommSecureMessageID = aintCommSecureMessageID;
            _iintTemplateID = aintTemplateID;
            _iintTrackingID = aintTrackingID;
        }

    }
}
