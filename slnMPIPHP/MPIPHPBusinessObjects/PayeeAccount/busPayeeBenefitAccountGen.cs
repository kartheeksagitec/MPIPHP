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
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busPayeeBenefitAccountGen:
    /// Inherited from busBase, used to create new business object for main table cdoPayeeBenefitAccount and its children table. 
    /// </summary>
	[Serializable]
	public class busPayeeBenefitAccountGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPayeeBenefitAccountGen
        /// </summary>
		public busPayeeBenefitAccountGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPayeeBenefitAccountGen.
        /// </summary>
		public cdoPayeeBenefitAccount icdoPayeeBenefitAccount { get; set; }




        /// <summary>
        /// MPIPHP.busPayeeBenefitAccountGen.FindPayeeBenefitAccount():
        /// Finds a particular record from cdoPayeeBenefitAccount with its primary key. 
        /// </summary>
        /// <param name="aintPayeeBenefitAccountId">A primary key value of type int of cdoPayeeBenefitAccount on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPayeeBenefitAccount(int aintPayeeBenefitAccountId)
		{
			bool lblnResult = false;
			if (icdoPayeeBenefitAccount == null)
			{
				icdoPayeeBenefitAccount = new cdoPayeeBenefitAccount();
			}
			if (icdoPayeeBenefitAccount.SelectRow(new object[1] { aintPayeeBenefitAccountId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
