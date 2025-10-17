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
	/// Class MPIPHP.BusinessObjects.busPersonAddressChklist:
	/// Inherited from busPersonAddressChklistGen, the class is used to customize the business object busPersonAddressChklistGen.
	/// </summary>
	[Serializable]
	public class busPersonAddressChklist : busPersonAddressChklistGen
	{
        public void InsertDataInPersonAddressChecklist(int aintAddressId,string astrAddressTypeValue)
        {
            icdoPersonAddressChklist = new cdoPersonAddressChklist();
            icdoPersonAddressChklist.address_id = aintAddressId;
            icdoPersonAddressChklist.address_type_id = busConstant.ADDRESS_TYPE_ID;
            icdoPersonAddressChklist.address_type_value = astrAddressTypeValue;
            icdoPersonAddressChklist.Insert();
        }

        public Collection<busPersonAddressChklist> LoadChecklistByAddressId(int aintAddressId)
        {
            DataTable ldtbList = Select<cdoPersonAddressChklist>(
                new string[1] { enmPersonAddressChklist.address_id.ToString() },
                new object[1] { aintAddressId }, null, null);
            Collection<busPersonAddressChklist> lclbusPersonAddressChklist = GetCollection<busPersonAddressChklist>(ldtbList, "icdoPersonAddressChklist");
            return lclbusPersonAddressChklist;
        }
	}
}
