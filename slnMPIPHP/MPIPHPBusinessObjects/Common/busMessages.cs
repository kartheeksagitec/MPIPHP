#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using MPIPHP.BusinessObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using MPIPHP.CustomDataObjects;


#endregion

namespace MPIPHP.BusinessObjects
{
	[Serializable]
	public class busMessages : busMPIPHPBase
    {
		public busMessages()
		{
		}

        public cdoMessages icdoMessages { set; get; }
		

		public bool FindMessage(int aintMessageId)
		{
			bool lblnResult = false;
			if (icdoMessages == null)
			{
				icdoMessages = new cdoMessages();
			}
			if (icdoMessages.SelectRow(new object[1] { aintMessageId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
