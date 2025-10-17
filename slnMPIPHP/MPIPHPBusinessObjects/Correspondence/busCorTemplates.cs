#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using Sagitec.CustomDataObjects;
using MPIPHP.BusinessObjects;
using MPIPHP.CustomDataObjects;


#endregion

namespace MPIPHP.BusinessObjects
{
	[Serializable]
	public class busCorTemplates : busMPIPHPBase
    {
		public busCorTemplates()
		{

		} 

		public cdoCorTemplates icdoCorTemplates {get;set;}

		public bool FindCorTemplates(int ainttemplateid)
		{

			bool lblnResult = false;
            if (icdoCorTemplates == null)
			{
                icdoCorTemplates = new cdoCorTemplates();
			}
            if (icdoCorTemplates.SelectRow(new object[1] { ainttemplateid }))
			{
				lblnResult = true;
			}
			return lblnResult;
			
		}

	}
}
