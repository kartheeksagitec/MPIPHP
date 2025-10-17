#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using MPIPHP.BusinessObjects;
using MPIPHP.CustomDataObjects;


#endregion

namespace MPIPHP.BusinessObjects
{
	[Serializable]
	public class busSystemPaths : busMPIPHPBase
    {
		public busSystemPaths()
		{
		}

        public cdoSystemPaths icdoSystemPaths { get; set; }
		
        public bool FindPath(int aintPathId)
		{
			bool lblnResult = false;
			if (icdoSystemPaths == null)
			{
				icdoSystemPaths = new cdoSystemPaths();
			}
            if (icdoSystemPaths.SelectRow(new object[1] { aintPathId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
