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
    /// Class MPIPHP.BusinessObjects.busPayeeAccountDeductionGen:
    /// Inherited from busBase, used to create new business object for main table cdoPayeeAccountDeduction and its children table. 
    /// </summary>
	[Serializable]
	public class busPayeeAccountDeductionGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPayeeAccountDeductionGen
        /// </summary>
		public busPayeeAccountDeductionGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPayeeAccountDeductionGen.
        /// </summary>
		public cdoPayeeAccountDeduction icdoPayeeAccountDeduction { get; set; }        


        /// <summary>
        /// MPIPHP.busPayeeAccountDeductionGen.FindPayeeAccountDeduction():
        /// Finds a particular record from cdoPayeeAccountDeduction with its primary key. 
        /// </summary>
        /// <param name="aintpayeeaccountdeductionid">A primary key value of type int of cdoPayeeAccountDeduction on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPayeeAccountDeduction(int aintpayeeaccountdeductionid)
		{
			bool lblnResult = false;
			if (icdoPayeeAccountDeduction == null)
			{
				icdoPayeeAccountDeduction = new cdoPayeeAccountDeduction();
			}
			if (icdoPayeeAccountDeduction.SelectRow(new object[1] { aintpayeeaccountdeductionid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
