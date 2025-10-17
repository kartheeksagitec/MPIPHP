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

namespace MPIPHP
{
	/// <summary>
	/// Class MPIPHP.busPaymentDirectives:
	/// Inherited from busPaymentDirectivesGen, the class is used to customize the business object busPaymentDirectivesGen.
	/// </summary>
	[Serializable]
	public class busPaymentDirectives : busPaymentDirectivesGen
	{
        public DateTime idtPaymentCycleDate { get; set; }
        
       public DateTime idtCreatedDate { get; set; }
        public int iintPaymentDirectiveId { get; set; }
    }
}
