#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using MPIPHP.BusinessObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
	[Serializable]
	public class busResourcesScreen : busMPIPHPBase
    {
		public busResourcesScreen()
		{
		}
        public string istrResourceFileName { get; set; }

        public string istrResourceElement { get; set; }

        public string istrResourceID { get; set; }
        
    }
}
