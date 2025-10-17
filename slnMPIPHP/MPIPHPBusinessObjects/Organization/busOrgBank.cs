#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;
using System.Linq;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busOrgBank:
	/// Inherited from busOrgBankGen, the class is used to customize the business object busOrgBankGen.
	/// </summary>
	[Serializable]
	public class busOrgBank : busOrgBankGen
	{
        public ArrayList CheckErrorOnAddButton(object aobj, Hashtable ahstParams, ref ArrayList aarrErrors, bool ablnHardError = false)
        {
            utlError lobjError = null;
            busOrganization lbusOrganization = aobj as busOrganization;

            lbusOrganization.icdoOrganization.istrStatusValue = Convert.ToString(ahstParams["status_value"]);

            ahstParams["icdoOrgBank.status_value"] = ahstParams["status_value"];
            ahstParams["icdoOrgBank.account_no"] = ahstParams["account_no"];
            ahstParams["icdoOrgBank.account_type_value"] = ahstParams["account_type_value"];

            if (lbusOrganization.iclbOrgBank.IsNotNull() && Convert.ToString(ahstParams["icdoOrgBank.status_value"]) == busConstant.OrgBankStatusActive)
            {
                int CountOrgBankWithActiveStatus = (from obj in lbusOrganization.iclbOrgBank where obj.icdoOrgBank.status_value == busConstant.OrgBankStatusActive select obj).Count();
                if (CountOrgBankWithActiveStatus > 0)
                {
                    lobjError = AddError(6105, "");
                    aarrErrors.Add(lobjError);
                }
            }

            if (Convert.ToString(ahstParams["icdoOrgBank.account_no"]).IsNullOrEmpty())
            {
                lobjError = AddError(6026, "");
                aarrErrors.Add(lobjError);

            }

            if (Convert.ToString(ahstParams["icdoOrgBank.account_type_value"]).IsNullOrEmpty())
            {
                lobjError = AddError(6020, "");
                aarrErrors.Add(lobjError);

            }

            if (Convert.ToString(ahstParams["icdoOrgBank.status_value"]).IsNullOrEmpty())
            {
                lobjError = AddError(6104,"");
                aarrErrors.Add(lobjError);

            }


            return aarrErrors;
        }
	}
}
