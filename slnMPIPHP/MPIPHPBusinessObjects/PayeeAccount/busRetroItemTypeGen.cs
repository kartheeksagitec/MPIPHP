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
    /// Class MPIPHP.BusinessObjects.busRetroItemTypeGen:
    /// Inherited from busBase, used to create new business object for main table cdoRetroItemType and its children table. 
    /// </summary>
	[Serializable]
	public class busRetroItemTypeGen : busMPIPHPBase
	{
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busRetroItemTypeGen
        /// </summary>
		public busRetroItemTypeGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busRetroItemTypeGen.
        /// </summary>
		public cdoRetroItemType icdoRetroItemType { get; set; }




        /// <summary>
        /// MPIPHP.busRetroItemTypeGen.FindRetroItemType():
        /// Finds a particular record from cdoRetroItemType with its primary key. 
        /// </summary>
        /// <param name="aintRetroItemTypeId">A primary key value of type int of cdoRetroItemType on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindRetroItemType(int aintRetroItemTypeId)
		{
			bool lblnResult = false;
			if (icdoRetroItemType == null)
			{
				icdoRetroItemType = new cdoRetroItemType();
			}
			if (icdoRetroItemType.SelectRow(new object[1] { aintRetroItemTypeId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
