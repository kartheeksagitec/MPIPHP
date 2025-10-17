using Sagitec.Bpm;
using System;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busSolBpmProcessInstanceRestrictNotifyXr : busBpmProcessInstanceRestrictNotifyXr
    {
        public busSolBpmProcessInstanceRestrictNotifyXr() : base()
        {
            icdoBpmProcessInstanceRestrictNotifyXr = new doBpmPrcsInstRsrtNotyXr();
        }
        /// <summary>
        /// Property to hold Notification Message of process Instance
        /// </summary>
        public string istrNotificationMessages { get; set; }
    }
}
