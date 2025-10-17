#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using MPIPHP.CustomDataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busPaymentScheduleLookup:
	/// Inherited from busPaymentScheduleLookupGen, this class is used to customize the lookup business object busPaymentScheduleLookupGen. 
	/// </summary>
	[Serializable]
	public class busPaymentScheduleLookup : busPaymentScheduleLookupGen
	{

        public override ArrayList ValidateNew(Hashtable ahstParam)
        {
            ArrayList larrErrors = new ArrayList();
            string astrScheduleTypeValue = Convert.ToString(ahstParam["astr_payment_schedule"]);

            if (astrScheduleTypeValue.IsNullOrEmpty() || astrScheduleTypeValue == "")
            {
                utlError lobjError = null;
                lobjError = AddError(6085, "");
                larrErrors.Add(lobjError);
            }
            return larrErrors;
        }
	}
}
