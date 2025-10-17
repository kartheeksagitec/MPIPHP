using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPIPHP.Common.Exceptions.Communication
{
    [Serializable()]
    public class GenerateCommunicationException : CommunicationException
    {
        public GenerateCommunicationException(int aintTemplateID, int aintTrackingID, string message)
            : base(aintTemplateID, aintTrackingID, 0, message)
        {

        }
    }
}
