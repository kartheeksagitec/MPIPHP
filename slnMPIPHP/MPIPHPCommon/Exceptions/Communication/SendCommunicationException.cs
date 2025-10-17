using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPIPHP.Common.Exceptions.Communication
{
    [Serializable()]
    public class SendCommunicationException : CommunicationException
    {
        public SendCommunicationException(int aintCorTrackingID)
            : base(0, aintCorTrackingID, 0, string.Empty)
        {
        }
    }
}
