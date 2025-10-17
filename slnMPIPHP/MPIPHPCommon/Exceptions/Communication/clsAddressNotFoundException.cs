using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPIPHP.Common.Exceptions.Communication
{
    public class clsAddressNotFoundException : CommunicationException
    {
        public clsAddressNotFoundException(int aintTemplateID, int aintRecipientID, enmCommunicationRecipientGroup aenmCommunicationRecipientGroup)
            : base(aintTemplateID, 0, 0, string.Format
            (CommunicationException.ERROR_MESSAGE_ADDRESS_NOT_FOUND, aenmCommunicationRecipientGroup.ToString(), aintRecipientID)
            )
        {
            _ienmCommunicationRecipientGroup = aenmCommunicationRecipientGroup;
            switch (aenmCommunicationRecipientGroup)
            {
                case enmCommunicationRecipientGroup.InternalUser:
                    this.iintInternalUserID = aintRecipientID;
                    break;
                case enmCommunicationRecipientGroup.OrganizationContact:
                    this.iintOrgContactID = aintRecipientID;
                    break;
                case enmCommunicationRecipientGroup.Person:
                    this.iintPersonID = aintRecipientID;
                    break;
            }
        }
    }
}
