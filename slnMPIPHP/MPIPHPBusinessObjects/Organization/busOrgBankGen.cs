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
    /// Class MPIPHP.BusinessObjects.busOrgBankGen:
    /// Inherited from busBase, used to create new business object for main table cdoOrgBank and its children table. 
    /// </summary>
	[Serializable]
	public class busOrgBankGen : busMPIPHPBase
	{
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busOrgBankGen
        /// </summary>
		public busOrgBankGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busOrgBankGen.
        /// </summary>
		public cdoOrgBank icdoOrgBank { get; set; }




        /// <summary>
        /// MPIPHP.busOrgBankGen.FindOrgBank():
        /// Finds a particular record from cdoOrgBank with its primary key. 
        /// </summary>
        /// <param name="aintorgbankid">A primary key value of type int of cdoOrgBank on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindOrgBank(int aintorgbankid)
		{
			bool lblnResult = false;
			if (icdoOrgBank == null)
			{
				icdoOrgBank = new cdoOrgBank();
			}
			if (icdoOrgBank.SelectRow(new object[1] { aintorgbankid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
