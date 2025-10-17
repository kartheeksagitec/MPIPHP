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
    /// Class MPIPHP.BusinessObjects.busPayeeAccountStatusGen:
    /// Inherited from busBase, used to create new business object for main table cdoPayeeAccountStatus and its children table. 
    /// </summary>
	[Serializable]
	public class busPayeeAccountStatusGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPayeeAccountStatusGen
        /// </summary>
		public busPayeeAccountStatusGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPayeeAccountStatusGen.
        /// </summary>
		public cdoPayeeAccountStatus icdoPayeeAccountStatus { get; set; }




        /// <summary>
        /// MPIPHP.busPayeeAccountStatusGen.FindPayeeAccountStatus():
        /// Finds a particular record from cdoPayeeAccountStatus with its primary key. 
        /// </summary>
        /// <param name="aintPayeeAccountStatusId">A primary key value of type int of cdoPayeeAccountStatus on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPayeeAccountStatus(int aintPayeeAccountStatusId)
		{
			bool lblnResult = false;
			if (icdoPayeeAccountStatus == null)
			{
				icdoPayeeAccountStatus = new cdoPayeeAccountStatus();
			}
			if (icdoPayeeAccountStatus.SelectRow(new object[1] { aintPayeeAccountStatusId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
