#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using MPIPHP.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using MPIPHP.Interface;

#endregion

namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public class busJobScheduleLookup : busJobScheduleLookupGen
    {
        /// <summary>
        /// Validate New from Lookup Screen
        /// </summary>
        /// <param name="ahstParam">Parameters</param>
        /// <returns>Validation Messages</returns>
        public override ArrayList ValidateNew(Hashtable ahstParam)
        {
            ArrayList larrErrors = new ArrayList();
            string lstrFrequencyTypeValue = ahstParam[busConstant.MPIPHPBatch.FORM_FIELD_FREQUENCY_TYPE_DROP_DOWN_VALUE].ToString().ToUpper();
            // Frequency Type is Required
            if (string.IsNullOrWhiteSpace(lstrFrequencyTypeValue) || lstrFrequencyTypeValue == busConstant.CodeValueAll)
            {
                utlError lobjError = new utlError();
                lobjError.istrErrorMessage = busConstant.MPIPHPBatch.ERROR_MESSAGE_SELECT_FREQUENCY_TYPE;
                larrErrors.Add(lobjError);
            }
         
            return larrErrors;
        }
    }
}
