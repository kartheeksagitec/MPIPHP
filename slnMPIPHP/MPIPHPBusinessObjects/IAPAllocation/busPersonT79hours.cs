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
	/// Class MPIPHP.busPersonT79hours:
	/// Inherited from busPersonT79hoursGen, the class is used to customize the business object busPersonT79hoursGen.
	/// </summary>
	[Serializable]
	public class busPersonT79hours : busPersonT79hoursGen
	{
        public override bool FindPersonT79hours(int aintPersonAccountId)
        {
            bool lblnResult = false;
            if (icdoPersonT79hours == null)
            {
                icdoPersonT79hours = new cdoPersonT79hours();
            }

            DataTable ldtResult = Select<cdoPersonT79hours>(new string[1] { "person_account_id" }, new object[1] { aintPersonAccountId }, null, null);
            
            if (ldtResult != null && ldtResult.Rows.Count > 0)
            {
                icdoPersonT79hours.LoadData(ldtResult.Rows[0]);
            }

            if (icdoPersonT79hours.person_t97_id > 0)
            {
                lblnResult = true;
            }
            return lblnResult;
        }
    }
}
