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
	/// Class MPIPHP.BusinessObjects.busRecipientRolloverOrganisationLookup:
	/// Inherited from busRecipientRolloverOrganisationLookupGen, this class is used to customize the lookup business object busRecipientRolloverOrganisationLookupGen. 
	/// </summary>
	[Serializable]
	public class busRecipientRolloverOrganisationLookup : busRecipientRolloverOrganisationLookupGen
	{
        public override ArrayList ValidateNew(Hashtable ahstParam)
        {
            ArrayList larrErrors = new ArrayList();
            utlError lobjError = null;
            int lintPayeeAccountID = 0;
            int.TryParse(ahstParam["aint_payee_account_id"].ToString(), out lintPayeeAccountID); 

            if (lintPayeeAccountID <= 0)
            {
                lobjError = AddError(0, "Payee Account ID required.");
                larrErrors.Add(lobjError);
                return larrErrors;
            }

            return larrErrors;
        }


        protected override void LoadOtherObjects(DataRow adtrRow, busBase abusBase)
        {
            base.LoadOtherObjects(adtrRow, abusBase);
            busPayeeAccountRolloverDetail lbusPayeeAccountRolloverDetail = (busPayeeAccountRolloverDetail)abusBase;

            lbusPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.istrOrgMPID = adtrRow["MPI_ORG_ID"].ToString();
            lbusPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.istrOrgName = adtrRow["ORG_NAME"].ToString();

        }
	}
}
