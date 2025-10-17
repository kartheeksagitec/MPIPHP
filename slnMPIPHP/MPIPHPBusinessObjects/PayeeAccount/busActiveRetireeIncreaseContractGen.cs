#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using  MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;

#endregion

namespace  MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class  MPIPHP.BusinessObjects.busActiveRetireeIncreaseContractGen:
    /// Inherited from busBase, used to create new business object for main table cdoActiveRetireeIncreaseContract and its children table. 
    /// </summary>
	[Serializable]
	public class busActiveRetireeIncreaseContractGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for  MPIPHP.BusinessObjects.busActiveRetireeIncreaseContractGen
        /// </summary>
		public busActiveRetireeIncreaseContractGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busActiveRetireeIncreaseContractGen.
        /// </summary>
		public cdoActiveRetireeIncreaseContract icdoActiveRetireeIncreaseContract { get; set; }


        /// <summary>
        /// Gets or sets the collection object of type busActiveRetireeIncreaseContractHistory. 
        /// </summary>
		public Collection<busActiveRetireeIncreaseContractHistory> iclbActiveRetireeIncreaseContractHistory { get; set; }



        /// <summary>
        ///  MPIPHP.busActiveRetireeIncreaseContractGen.FindActiveRetireeIncreaseContract():
        /// Finds a particular record from cdoActiveRetireeIncreaseContract with its primary key. 
        /// </summary>
        /// <param name="aintActiveRetireeIncreaseContractId">A primary key value of type int of cdoActiveRetireeIncreaseContract on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindActiveRetireeIncreaseContract(int aintActiveRetireeIncreaseContractId)
		{
			bool lblnResult = false;
			if (icdoActiveRetireeIncreaseContract == null)
			{
				icdoActiveRetireeIncreaseContract = new cdoActiveRetireeIncreaseContract();
			}
			if (icdoActiveRetireeIncreaseContract.SelectRow(new object[1] { aintActiveRetireeIncreaseContractId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        ///  MPIPHP.busActiveRetireeIncreaseContractGen.LoadActiveRetireeIncreaseContractHistorys():
        /// Loads Collection object iclbActiveRetireeIncreaseContractHistory of type busActiveRetireeIncreaseContractHistory.
        /// </summary>
		public virtual void LoadActiveRetireeIncreaseContractHistorys()
		{
			DataTable ldtbList = Select<cdoActiveRetireeIncreaseContractHistory>(
				new string[1] { enmActiveRetireeIncreaseContractHistory.active_retiree_increase_contract_id.ToString() },
				new object[1] { icdoActiveRetireeIncreaseContract.active_retiree_increase_contract_id }, null, null);
			iclbActiveRetireeIncreaseContractHistory = GetCollection<busActiveRetireeIncreaseContractHistory>(ldtbList, "icdoActiveRetireeIncreaseContractHistory");
		}

	}
}
