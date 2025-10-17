using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.ExceptionPub;

namespace MPIPHP.Common.Exceptions
{
    [Serializable()]
    public class MPIPHPException : BaseApplicationException
    {
        public MPIPHPException()
            : base()
        {
        }
        public MPIPHPException(string message)
            : base(message)
        {

        }
    }
}

