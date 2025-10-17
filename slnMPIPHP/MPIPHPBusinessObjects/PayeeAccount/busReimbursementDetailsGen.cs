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
    /// Class MPIPHP.BusinessObjects.busReimbursementDetailsGen:
    /// Inherited from busBase, used to create new business object for main table cdoReimbursementDetails and its children table. 
    /// </summary>
	[Serializable]
	public class busReimbursementDetailsGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busReimbursementDetailsGen
        /// </summary>
        public busReimbursementDetailsGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busReimbursementDetailsGen.
        /// </summary>
        public cdoReimbursementDetails icdoReimbursementDetails { get; set; }




        /// <summary>
        /// MPIPHP.busReimbursementDetailsGen.FindReimbursementDetails():
        /// Finds a particular record from cdoReimbursementDetails with its primary key. 
        /// </summary>
        /// <param name="aintReimbursementDetailsId">A primary key value of type int of cdoReimbursementDetails on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
        public virtual bool FindReimbursementDetails(int aintReimbursementDetailsId)
		{
			bool lblnResult = false;
            if (icdoReimbursementDetails == null)
			{
                icdoReimbursementDetails = new cdoReimbursementDetails();
			}
            if (icdoReimbursementDetails.SelectRow(new object[1] { aintReimbursementDetailsId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
