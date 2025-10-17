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
    /// Class MPIPHP.BusinessObjects.busPersonAddressChklistGen:
    /// Inherited from busBase, used to create new business object for main table cdoPersonAddressChklist and its children table. 
    /// </summary>
	[Serializable]
	public class busPersonAddressChklistGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPersonAddressChklistGen
        /// </summary>
		public busPersonAddressChklistGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPersonAddressChklistGen.
        /// </summary>
		public cdoPersonAddressChklist icdoPersonAddressChklist { get; set; }




        /// <summary>
        /// MPIPHP.busPersonAddressChklistGen.FindPersonAddressChklist():
        /// Finds a particular record from cdoPersonAddressChklist with its primary key. 
        /// </summary>
        /// <param name="aintAddressChklistId">A primary key value of type int of cdoPersonAddressChklist on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPersonAddressChklist(int aintAddressChklistId)
		{
			bool lblnResult = false;
			if (icdoPersonAddressChklist == null)
			{
				icdoPersonAddressChklist = new cdoPersonAddressChklist();
			}
			if (icdoPersonAddressChklist.SelectRow(new object[1] { aintAddressChklistId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
