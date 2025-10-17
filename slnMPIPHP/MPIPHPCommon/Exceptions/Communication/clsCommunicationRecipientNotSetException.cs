using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPIPHP.Common.Exceptions.Communication
{
    public class clsCommunicationRecipientNotSetException : CommunicationException
    {
        public clsCommunicationRecipientNotSetException()
            : base(ERROR_MESSAGE_COMMUNICATION_RECIPIENT_NOT_SET)
        {

        }
    }
}
