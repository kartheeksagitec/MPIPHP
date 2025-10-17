using System;
using MPIPHP.BusinessObjects;
using MPIPHP.CustomDataObjects;

namespace MPIPHP.Common
{
    [Serializable]
    public class ActivityInstanceEventArgs
    {
        public ActivityInstanceEventArgs()
        {
        }
        public cdoActivityInstance icdoActivityInstance { get; set; }
        public enmNextAction ienmNextAction { get; set; }
    }

    [Serializable]
    public enum enmNextAction
    {
        Next,
        Previous,
        First,
        Return,
        Correspondance,
        Cancel,
        ReturnBack
    }
}
