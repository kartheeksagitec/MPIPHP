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
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busPayeeAccountTaxWithholdingGen:
    /// Inherited from busBase, used to create new business object for main table cdoPayeeAccountTaxWithholding and its children table. 
    /// </summary>
	[Serializable]
	public class busPayeeAccountTaxWithholdingGen : busMPIPHPBase
	{
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPayeeAccountTaxWithholdingGen
        /// </summary>
		public busPayeeAccountTaxWithholdingGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPayeeAccountTaxWithholdingGen.
        /// </summary>
		public cdoPayeeAccountTaxWithholding icdoPayeeAccountTaxWithholding { get; set; }


        /// <summary>
        /// Gets or sets the collection object of type busPayeeAccountTaxWithholdingItemDetail. 
        /// </summary>
		public Collection<busPayeeAccountTaxWithholdingItemDetail> iclbPayeeAccountTaxWithholdingItemDetail { get; set; }



        /// <summary>
        /// MPIPHP.busPayeeAccountTaxWithholdingGen.FindPayeeAccountTaxWithholding():
        /// Finds a particular record from cdoPayeeAccountTaxWithholding with its primary key. 
        /// </summary>
        /// <param name="aintPayeeAccountTaxWithholdingId">A primary key value of type int of cdoPayeeAccountTaxWithholding on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPayeeAccountTaxWithholding(int aintPayeeAccountTaxWithholdingId)
		{
			bool lblnResult = false;
			if (icdoPayeeAccountTaxWithholding == null)
			{
				icdoPayeeAccountTaxWithholding = new cdoPayeeAccountTaxWithholding();
			}
			if (icdoPayeeAccountTaxWithholding.SelectRow(new object[1] { aintPayeeAccountTaxWithholdingId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// MPIPHP.busPayeeAccountTaxWithholdingGen.LoadPayeeAccountTaxWithholdingItemDetails():
        /// Loads Collection object iclbPayeeAccountTaxWithholdingItemDetail of type busPayeeAccountTaxWithholdingItemDetail.
        /// </summary>
		public virtual void LoadPayeeAccountTaxWithholdingItemDetails()
		{
			DataTable ldtbList = Select<cdoPayeeAccountTaxWithholdingItemDetail>(
				new string[1] { enmPayeeAccountTaxWithholdingItemDetail.payee_account_tax_withholding_id.ToString() },
				new object[1] { icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id }, null, null);
			iclbPayeeAccountTaxWithholdingItemDetail = GetCollection<busPayeeAccountTaxWithholdingItemDetail>(ldtbList, "icdoPayeeAccountTaxWithholdingItemDetail");
		}

	}
}
