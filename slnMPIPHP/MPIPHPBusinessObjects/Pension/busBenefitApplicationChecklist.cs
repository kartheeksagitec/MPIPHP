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
    ///  partial class NeoSpin.BusinessObjects.busBenefitApplicationChecklist
    /// </summary>
	[Serializable]
	public  partial class busBenefitApplicationChecklist : busMPIPHPBase
	{
		/// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busBenefitApplicationChecklist
        /// </summary>
		public busBenefitApplicationChecklist()
		{
		}
		public Collection<busBenefitApplicationChecklist> iclbBenefitApplicationChecklist { get; set; }


		public virtual bool FindBenefitApplicationChecklist(int aintBenefitApplicationChecklistId)
		{
			bool lblnResult = false;
			if (icdoBenefitApplicationChecklist == null)
			{
				icdoBenefitApplicationChecklist = new doBenefitApplicationChecklist();
			}
			if (icdoBenefitApplicationChecklist.SelectRow(new object[1] { aintBenefitApplicationChecklistId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

		public void LoadBenefitApplicationChecklist(int aintbenefitapplicationid)
		{
			DataTable ldtblist = busBase.Select("entPersonAccount.LoadCheckList", new object[1] { aintbenefitapplicationid });
			iclbBenefitApplicationChecklist = GetCollection<busBenefitApplicationChecklist>(ldtblist, "icdoBenefitApplicationChecklist");
		}
	}
}
