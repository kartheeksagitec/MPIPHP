using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPIPHP.Common.Exceptions.Communication
{
    [Serializable()]
    public class InvalidCorTrackingIDException : CommunicationException
    {
        public InvalidCorTrackingIDException(int aintCorTracking)
            : base(0, aintCorTracking, 0, string.Empty)
        {
            
        }

    }
}
