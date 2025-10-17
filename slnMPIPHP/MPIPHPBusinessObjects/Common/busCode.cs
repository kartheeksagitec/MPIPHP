#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using MPIPHP.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using Sagitec.CustomDataObjects;
using MPIPHP.CustomDataObjects;


#endregion

namespace MPIPHP.BusinessObjects
{
	[Serializable]
	public class busCode : busMPIPHPBase
    {
		public busCode()
		{
		}

        public cdoCode icdoCode { set; get; }
        public Collection<busCodeValue> iclbCodeValue { set; get; }

		
		public bool FindCode(int aintCodeId)
		{
			bool lblnResult = false;
			if (icdoCode == null)
			{
				icdoCode = new cdoCode();
			}
			if (icdoCode.SelectRow(new object[1] { aintCodeId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

		public void LoadCodeValues()
		{
			DataTable ldtbList = Select<cdoCodeValue>( new string[1] { "code_id" },
				new object[1] { icdoCode.code_id }, null, "code_value_order, description");
			iclbCodeValue = GetCollection<busCodeValue>(ldtbList, "icdoCodeValue");
		}
	}
}
