using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPIPHP.BusinessObjects
{
    [Serializable()]
    public class clsSendCommunicationResult
    {
        public bool iblnSuccess { get; set; }
        public DateTime? idtmNextReminderDate { get; set; }
        public DateTime? idtmDueDate { get; set; }
        public string istrMessage { get; set; }

        public clsSendCommunicationResult()
        {
            istrMessage = string.Empty;
            idtmNextReminderDate = null;
        }

        public clsSendCommunicationResult(DateTime? adtmNextReminderDate)
            : this()
        {
            iblnSuccess = true;
            idtmNextReminderDate = adtmNextReminderDate;
        }

        public clsSendCommunicationResult(string astrFailureReason)
            : this()
        {
            iblnSuccess = false;
            istrMessage = astrFailureReason;
        }
    }
}
