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
	/// Class MPIPHP.BusinessObjects.busPersonAddressHistory:
	/// Inherited from busPersonAddressHistoryGen, the class is used to customize the business object busPersonAddressHistoryGen.
	/// </summary>
	[Serializable]
	public class busPersonAddressHistory : busPersonAddressHistoryGen
	{
        public utlCollection<cdoPersonAddressChklistHistory> iclcPersonAddressChklistHistory { get; set; }

        public virtual void LoadPersonAddressChklists()
        {
            iclcPersonAddressChklistHistory = GetCollection<cdoPersonAddressChklistHistory>(
                new string[1] { enmPersonAddressChklistHistory.person_address_history_id.ToString() },
                new object[1] { icdoPersonAddressHistory.person_address_history_id }, null, null);
        }

	}
}
