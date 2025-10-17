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
    /// Class MPIPHP.BusinessObjects.busPayeeAccountAchDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoPayeeAccountAchDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busPayeeAccountAchDetailGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPayeeAccountAchDetailGen
        /// </summary>
		public busPayeeAccountAchDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPayeeAccountAchDetailGen.
        /// </summary>
		public cdoPayeeAccountAchDetail icdoPayeeAccountAchDetail { get; set; }




        /// <summary>
        /// MPIPHP.busPayeeAccountAchDetailGen.FindPayeeAccountAchDetail():
        /// Finds a particular record from cdoPayeeAccountAchDetail with its primary key. 
        /// </summary>
        /// <param name="aintPayeeAccountAchDetailId">A primary key value of type int of cdoPayeeAccountAchDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPayeeAccountAchDetail(int aintPayeeAccountAchDetailId)
		{
			bool lblnResult = false;
			if (icdoPayeeAccountAchDetail == null)
			{
				icdoPayeeAccountAchDetail = new cdoPayeeAccountAchDetail();
			}
			if (icdoPayeeAccountAchDetail.SelectRow(new object[1] { aintPayeeAccountAchDetailId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
