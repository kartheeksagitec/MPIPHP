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

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busPaymentItemType:
	/// Inherited from busPaymentItemTypeGen, the class is used to customize the business object busPaymentItemTypeGen.
	/// </summary>
	[Serializable]
	public class busPaymentItemType : busPaymentItemTypeGen
	{
        public bool FindPaymentItemTypeByItemCode(string astrPaymentItemTypeCode)
        {
            bool lblnResult = false;
            DataTable ldtbList = Select<cdoPaymentItemType>(
                new string[1] { "item_type_code" },
                new object[1] { astrPaymentItemTypeCode }, null, null);
            if (icdoPaymentItemType == null)
                icdoPaymentItemType = new cdoPaymentItemType();
            if (ldtbList.Rows.Count == 1)
            {
                icdoPaymentItemType.LoadData(ldtbList.Rows[0]);
                lblnResult = true;
            }
            return lblnResult;
        }
	}
}
