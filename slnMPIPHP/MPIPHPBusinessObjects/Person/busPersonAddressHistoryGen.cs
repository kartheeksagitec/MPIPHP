#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using    MPIPHP.CustomDataObjects;

#endregion

namespace    MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class    MPIPHP.BusinessObjects.busPersonAddressHistoryGen:
    /// Inherited from busBase, used to create new business object for main table cdoPersonAddressHistory and its children table. 
    /// </summary>
	[Serializable]
	public class busPersonAddressHistoryGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for    MPIPHP.BusinessObjects.busPersonAddressHistoryGen
        /// </summary>
		public busPersonAddressHistoryGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPersonAddressHistoryGen.
        /// </summary>
		public cdoPersonAddressHistory icdoPersonAddressHistory { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busPersonAddress.
        /// </summary>
		public busPersonAddress ibusPersonAddress { get; set; }


        /// <summary>
        ///    MPIPHP.busPersonAddressHistoryGen.FindPersonAddressHistory():
        /// Finds a particular record from cdoPersonAddressHistory with its primary key. 
        /// </summary>
        /// <param name="aintpersonaddresshistoryid">A primary key value of type int of cdoPersonAddressHistory on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPersonAddressHistory(int aintpersonaddresshistoryid)
		{
			bool lblnResult = false;
			if (icdoPersonAddressHistory == null)
			{
				icdoPersonAddressHistory = new cdoPersonAddressHistory();
			}
			if (icdoPersonAddressHistory.SelectRow(new object[1] { aintpersonaddresshistoryid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        ///    MPIPHP.busPersonAddressHistoryGen.LoadPersonAddress():
        /// Loads non-collection object ibusPersonAddress of type busPersonAddress.
        /// </summary>
		public virtual void LoadPersonAddress()
		{
			if (ibusPersonAddress == null)
			{
				ibusPersonAddress = new busPersonAddress();
			}
			ibusPersonAddress.FindPersonAddress(icdoPersonAddressHistory.address_id);
		}      

	}
}
