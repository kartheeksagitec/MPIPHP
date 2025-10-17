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
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busPersonAccountEligibility:
	/// Inherited from busPersonAccountEligibilityGen, the class is used to customize the business object busPersonAccountEligibilityGen.
	/// </summary>
	[Serializable]
	public class busPersonAccountEligibility : busPersonAccountEligibilityGen
	{
        public busPersonAccountEligibility LoadPersonAccEligibilityByPersonAccountId(int aintPersonAccountId)
        {
            Collection<busPersonAccountEligibility> lclbPersonAccountEligibility = new Collection<busPersonAccountEligibility>();

            DataTable ldtbList = Select<cdoPersonAccountEligibility>(
               new string[1] { enmPersonAccountEligibility.person_account_id.ToString() },
               new object[1] { aintPersonAccountId }, null, null);

            lclbPersonAccountEligibility = GetCollection<busPersonAccountEligibility>(ldtbList, "icdoPersonAccountEligibility");

            if (lclbPersonAccountEligibility != null && lclbPersonAccountEligibility.Count > 0)
            {
              return  lclbPersonAccountEligibility[0];
            }

            return null;
        }
	}
}
