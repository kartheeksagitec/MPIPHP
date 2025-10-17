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
    /// Class MPIPHP.BusinessObjects.busAttorneyGen:
    /// Inherited from busBase, used to create new business object for main table cdoAttorney and its children table. 
    /// </summary>
	[Serializable]
	public class busAttorneyGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busAttorneyGen
        /// </summary>
		public busAttorneyGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busAttorneyGen.
        /// </summary>
		public cdoAttorney icdoAttorney { get; set; }




        /// <summary>
        /// MPIPHP.busAttorneyGen.FindAttorney():
        /// Finds a particular record from cdoAttorney with its primary key. 
        /// </summary>
        /// <param name="aintAttorneyId">A primary key value of type int of cdoAttorney on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindAttorney(int aintAttorneyId)
		{
			bool lblnResult = false;
			if (icdoAttorney == null)
			{
				icdoAttorney = new cdoAttorney();
			}
			if (icdoAttorney.SelectRow(new object[1] { aintAttorneyId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
