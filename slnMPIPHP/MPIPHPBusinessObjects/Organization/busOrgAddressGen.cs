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
    /// Class MPIPHP.BusinessObjects.busOrgAddressGen:
    /// Inherited from busBase, used to create new business object for main table cdoOrgAddress and its children table. 
    /// </summary>
	[Serializable]
	public class busOrgAddressGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busOrgAddressGen
        /// </summary>
		public busOrgAddressGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busOrgAddressGen.
        /// </summary>
		public cdoOrgAddress icdoOrgAddress { get; set; }




        /// <summary>
        /// MPIPHP.busOrgAddressGen.FindOrgAddress():
        /// Finds a particular record from cdoOrgAddress with its primary key. 
        /// </summary>
        /// <param name="aintorgaddressid">A primary key value of type int of cdoOrgAddress on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindOrgAddress(int aintorgaddressid)
		{
			bool lblnResult = false;
			if (icdoOrgAddress == null)
			{
				icdoOrgAddress = new cdoOrgAddress();
			}
			if (icdoOrgAddress.SelectRow(new object[1] { aintorgaddressid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
