#region Using directives
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.DataObjects;
using MPIPHP.DataObjects;
#endregion
namespace MPIPHP.BusinessObjects
{
    /// <summary>
    ///  partial class NeoSpin.BusinessObjects.busBenefitApplicationAuditingChecklist
    /// </summary>
	[Serializable]
	public  partial class busBenefitApplicationAuditingChecklist : busBase
	{
		/// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busBenefitApplicationAuditingChecklist
        /// </summary>
		public busBenefitApplicationAuditingChecklist()
		{
		}

		public virtual bool FindBenefitApplicationAuditingChecklist(int aintBenefitAuditingApplicationChecklistId)
		{
			bool lblnResult = false;
			if (icdoBenefitApplicationAuditingChecklist == null)
			{
				icdoBenefitApplicationAuditingChecklist = new doBenefitApplicationAuditingChecklist();
			}
			if (icdoBenefitApplicationAuditingChecklist.SelectRow(new object[1] { aintBenefitAuditingApplicationChecklistId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
