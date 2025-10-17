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
	/// Class MPIPHP.BusinessObjects.busPayeeAccountRolloverItemDetail:
	/// Inherited from busPayeeAccountRolloverItemDetailGen, the class is used to customize the business object busPayeeAccountRolloverItemDetailGen.
	/// </summary>
	[Serializable]
	public class busPayeeAccountRolloverItemDetail : busPayeeAccountRolloverItemDetailGen
	{
        public busPayeeAccountPaymentItemType ibusPayeeAccountPaymentItemType { get; set; }
	}
}
