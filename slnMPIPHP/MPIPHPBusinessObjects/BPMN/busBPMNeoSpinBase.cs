#region [Using directives]

using NeoBase.BPM;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using System;
using System.Collections;
#endregion [Using directives]

namespace NeoBase.BPM
{
    public partial class busNeoSpinBaseBPM : busBase
    {
        #region [Public Methods]

        /// <summary>
        /// This method is virtual which can be overriden by child class. It return empty string.
        /// </summary>
        /// <returns>Empty string</returns>
        public virtual string GetRecipientEmail()
        {
            return string.Empty;
        }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// This function is used to validate the activity instance checklist
        /// </summary>
        /// <returns></returns>
        public override ArrayList OnBpmSubmit()
        {
            ArrayList larrResult = new ArrayList();
            utlError lutlError = new utlError();
            busNeobaseBpmActivityInstance lbusSolBpmActivityInstance = ibusBaseActivityInstance as busNeobaseBpmActivityInstance;
            if (lbusSolBpmActivityInstance != null)
            {
                if (!lbusSolBpmActivityInstance.HasAllRequiredChecklistsCompleted())
                {
                    lutlError = AddError(1565, String.Empty);
                    larrResult.Add(lutlError);
                    return larrResult;
                }
                if (lbusSolBpmActivityInstance.IsCompletedDateFutureDate())
                {
                    lutlError = AddError(1568, String.Empty);
                    larrResult.Add(lutlError);
                    return larrResult;
                }

                larrResult.AddRange(base.OnBpmSubmit());
            }
            else
            {
                larrResult.AddRange(base.OnBpmSubmit());
            }
            return larrResult;
        }
        #endregion
    }
}
