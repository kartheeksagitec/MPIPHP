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
    /// Class MPIPHP.BusinessObjects.busCustomAccessPolicyGen:
    /// Inherited from busBase, used to create new business object for main table cdoCustomAccessPolicy and its children table. 
    /// </summary>
	[Serializable]
	public class busCustomAccessPolicyGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busCustomAccessPolicyGen
        /// </summary>
		public busCustomAccessPolicyGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busCustomAccessPolicyGen.
        /// </summary>
		public cdoCustomAccessPolicy icdoCustomAccessPolicy { get; set; }




        /// <summary>
        /// MPIPHP.busCustomAccessPolicyGen.FindCustomAccessPolicy():
        /// Finds a particular record from cdoCustomAccessPolicy with its primary key. 
        /// </summary>
        /// <param name="aintcustomaccesspolicyid">A primary key value of type int of cdoCustomAccessPolicy on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindCustomAccessPolicy(int aintcustomaccesspolicyid)
		{
			bool lblnResult = false;
			if (icdoCustomAccessPolicy == null)
			{
				icdoCustomAccessPolicy = new cdoCustomAccessPolicy();
			}
			if (icdoCustomAccessPolicy.SelectRow(new object[1] { aintcustomaccesspolicyid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
