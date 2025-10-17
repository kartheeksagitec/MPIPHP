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
    /// Class MPIPHP.BusinessObjects.busCustomSecurityGen:
    /// Inherited from busBase, used to create new business object for main table cdoCustomSecurity and its children table. 
    /// </summary>
	[Serializable]
	public class busCustomSecurityGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busCustomSecurityGen
        /// </summary>
		public busCustomSecurityGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busCustomSecurityGen.
        /// </summary>
		public cdoCustomSecurity icdoCustomSecurity { get; set; }




        /// <summary>
        /// MPIPHP.busCustomSecurityGen.FindCustomSecurity():
        /// Finds a particular record from cdoCustomSecurity with its primary key. 
        /// </summary>
        /// <param name="aintcustomsecurityid">A primary key value of type int of cdoCustomSecurity on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindCustomSecurity(int aintcustomsecurityid)
		{
			bool lblnResult = false;
			if (icdoCustomSecurity == null)
			{
				icdoCustomSecurity = new cdoCustomSecurity();
			}
			if (icdoCustomSecurity.SelectRow(new object[1] { aintcustomsecurityid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
