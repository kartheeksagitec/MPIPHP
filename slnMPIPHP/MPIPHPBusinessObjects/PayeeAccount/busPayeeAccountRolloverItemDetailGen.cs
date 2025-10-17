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
    /// Class MPIPHP.BusinessObjects.busPayeeAccountRolloverItemDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoPayeeAccountRolloverItemDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busPayeeAccountRolloverItemDetailGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPayeeAccountRolloverItemDetailGen
        /// </summary>
		public busPayeeAccountRolloverItemDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPayeeAccountRolloverItemDetailGen.
        /// </summary>
		public cdoPayeeAccountRolloverItemDetail icdoPayeeAccountRolloverItemDetail { get; set; }




        /// <summary>
        /// MPIPHP.busPayeeAccountRolloverItemDetailGen.FindPayeeAccountRolloverItemDetail():
        /// Finds a particular record from cdoPayeeAccountRolloverItemDetail with its primary key. 
        /// </summary>
        /// <param name="aintPayeeAccountRolloverItemDetailId">A primary key value of type int of cdoPayeeAccountRolloverItemDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPayeeAccountRolloverItemDetail(int aintPayeeAccountRolloverItemDetailId)
		{
			bool lblnResult = false;
			if (icdoPayeeAccountRolloverItemDetail == null)
			{
				icdoPayeeAccountRolloverItemDetail = new cdoPayeeAccountRolloverItemDetail();
			}
			if (icdoPayeeAccountRolloverItemDetail.SelectRow(new object[1] { aintPayeeAccountRolloverItemDetailId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
