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
using MPIPHP.BusinessObjects;

#endregion

namespace MPIPHP
{
    /// <summary>
    /// Class MPIPHP.busIapHardshipPaybackGen:
    /// Inherited from busBase, used to create new business object for main table cdoIapHardshipPayback and its children table. 
    /// </summary>
	[Serializable]
	public class busIapHardshipPaybackGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.busIapHardshipPaybackGen
        /// </summary>
		public busIapHardshipPaybackGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busIapHardshipPaybackGen.
        /// </summary>
		public cdoIapHardshipPayback icdoIapHardshipPayback { get; set; }




        /// <summary>
        /// MPIPHP.busIapHardshipPaybackGen.FindIapHardshipPayback():
        /// Finds a particular record from cdoIapHardshipPayback with its primary key. 
        /// </summary>
        /// <param name="aintIapHardshipPaybackId">A primary key value of type int of cdoIapHardshipPayback on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindIapHardshipPayback(int aintIapHardshipPaybackId)
		{
			bool lblnResult = false;
			if (icdoIapHardshipPayback == null)
			{
				icdoIapHardshipPayback = new cdoIapHardshipPayback();
			}
			if (icdoIapHardshipPayback.SelectRow(new object[1] { aintIapHardshipPaybackId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
