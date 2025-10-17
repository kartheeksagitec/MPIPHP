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
    /// Class MPIPHP.BusinessObjects.busDroBenefitDetailsGen:
    /// Inherited from busBase, used to create new business object for main table cdoDroBenefitDetails and its children table. 
    /// </summary>
	[Serializable]
	public class busDroBenefitDetailsGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busDroBenefitDetailsGen
        /// </summary>
		public busDroBenefitDetailsGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busDroBenefitDetailsGen.
        /// </summary>
		public cdoDroBenefitDetails icdoDroBenefitDetails { get; set; }




        /// <summary>
        /// MPIPHP.busDroBenefitDetailsGen.FindDroBenefitDetails():
        /// Finds a particular record from cdoDroBenefitDetails with its primary key. 
        /// </summary>
        /// <param name="aintDroBenefitId">A primary key value of type int of cdoDroBenefitDetails on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindDroBenefitDetails(int aintDroBenefitId)
		{
			bool lblnResult = false;
			if (icdoDroBenefitDetails == null)
			{
				icdoDroBenefitDetails = new cdoDroBenefitDetails();
			}
			if (icdoDroBenefitDetails.SelectRow(new object[1] { aintDroBenefitId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
