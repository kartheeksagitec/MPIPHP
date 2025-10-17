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
	/// Class MPIPHP.BusinessObjects.busCustomSecurity:
	/// Inherited from busCustomSecurityGen, the class is used to customize the business object busCustomSecurityGen.
	/// </summary>
	[Serializable]
	public class busCustomSecurity : busCustomSecurityGen
	{
        private busUser _ibusUser;
        public busUser ibusUser {get;set;}
	}
}
