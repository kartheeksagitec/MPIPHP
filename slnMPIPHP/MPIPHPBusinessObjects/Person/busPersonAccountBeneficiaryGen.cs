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
    /// Class MPIPHP.BusinessObjects.busPersonAccountBeneficiaryGen:
    /// Inherited from busBase, used to create new business object for main table cdoPersonAccountBeneficiary and its children table. 
    /// </summary>
	[Serializable]
	public class busPersonAccountBeneficiaryGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPersonAccountBeneficiaryGen
        /// </summary>
		public busPersonAccountBeneficiaryGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPersonAccountBeneficiaryGen.
        /// </summary>
		public cdoPersonAccountBeneficiary icdoPersonAccountBeneficiary { get; set; }




        /// <summary>
        /// MPIPHP.busPersonAccountBeneficiaryGen.FindPersonAccountBeneficiary():
        /// Finds a particular record from cdoPersonAccountBeneficiary with its primary key. 
        /// </summary>
        /// <param name="aintpersonaccountbeneficiaryid">A primary key value of type int of cdoPersonAccountBeneficiary on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPersonAccountBeneficiary(int aintpersonaccountbeneficiaryid)
		{
			bool lblnResult = false;
			if (icdoPersonAccountBeneficiary == null)
			{
				icdoPersonAccountBeneficiary = new cdoPersonAccountBeneficiary();
			}
			if (icdoPersonAccountBeneficiary.SelectRow(new object[1] { aintpersonaccountbeneficiaryid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
