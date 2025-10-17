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
    /// Class MPIPHP.BusinessObjects.busPayeeAccountTaxWithholdingItemDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoPayeeAccountTaxWithholdingItemDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busPayeeAccountTaxWithholdingItemDetailGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPayeeAccountTaxWithholdingItemDetailGen
        /// </summary>
		public busPayeeAccountTaxWithholdingItemDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPayeeAccountTaxWithholdingItemDetailGen.
        /// </summary>
		public cdoPayeeAccountTaxWithholdingItemDetail icdoPayeeAccountTaxWithholdingItemDetail { get; set; }




        /// <summary>
        /// MPIPHP.busPayeeAccountTaxWithholdingItemDetailGen.FindPayeeAccountTaxWithholdingItemDetail():
        /// Finds a particular record from cdoPayeeAccountTaxWithholdingItemDetail with its primary key. 
        /// </summary>
        /// <param name="aintPayeeAccountTaxWithholdingItemDtlId">A primary key value of type int of cdoPayeeAccountTaxWithholdingItemDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPayeeAccountTaxWithholdingItemDetail(int aintPayeeAccountTaxWithholdingItemDtlId)
		{
			bool lblnResult = false;
			if (icdoPayeeAccountTaxWithholdingItemDetail == null)
			{
				icdoPayeeAccountTaxWithholdingItemDetail = new cdoPayeeAccountTaxWithholdingItemDetail();
			}
			if (icdoPayeeAccountTaxWithholdingItemDetail.SelectRow(new object[1] { aintPayeeAccountTaxWithholdingItemDtlId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
