using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using Sagitec.Common;
using Sagitec.DBUtility;
using MPIPHP.BusinessObjects;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;


namespace MPIPHP.BusinessObjects
{
	[Serializable]
	public class busSystemManagement : busMPIPHPBase
    {
		public busSystemManagement()
		{
		}

        public cdoSystemManagement icdoSystemManagement { get; set; }
		
		public bool FindSystemManagement()
		{
			bool lblnResult = false;
			if (icdoSystemManagement == null)
			{
				icdoSystemManagement = new cdoSystemManagement();
			}
			if (icdoSystemManagement.SelectRow(new object[1] { 1 }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
