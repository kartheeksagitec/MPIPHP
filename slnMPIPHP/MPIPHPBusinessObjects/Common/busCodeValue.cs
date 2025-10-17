#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using MPIPHP.BusinessObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CustomDataObjects;


#endregion

namespace MPIPHP.BusinessObjects
{
	[Serializable]
	public class busCodeValue : busMPIPHPBase
    {
		public busCodeValue()
		{
		}

        public busCode ibusCode { set; get; }

        public cdoCodeValue icdoCodeValue { set; get; }

		

		public bool FindCodeValue(int aintCodeSerialId)
		{
			bool lblnResult = false;
			if (icdoCodeValue == null)
			{
				icdoCodeValue = new cdoCodeValue();
			}
			if (icdoCodeValue.SelectRow(new object[1] { aintCodeSerialId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

		public void LoadCode()
		{
			if (ibusCode == null)
			{
				ibusCode = new busCode();
			}
			ibusCode.FindCode(icdoCodeValue.code_id);
		}
	}
}
