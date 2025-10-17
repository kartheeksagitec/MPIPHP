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
	/// Class MPIPHP.busPensionVerificationHistory:
	/// Inherited from busPensionVerificationHistoryGen, the class is used to customize the business object busPensionVerificationHistoryGen.
	/// </summary>
	[Serializable]
	public class busPensionVerificationHistory : busPensionVerificationHistoryGen
	{
        #region Public Property
        public busPerson ibusPayee { get; set; }
        #endregion
    }
}
