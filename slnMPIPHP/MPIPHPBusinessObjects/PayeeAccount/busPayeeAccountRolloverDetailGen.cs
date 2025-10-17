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
    /// Class MPIPHP.BusinessObjects.busPayeeAccountRolloverDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoPayeeAccountRolloverDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busPayeeAccountRolloverDetailGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPayeeAccountRolloverDetailGen
        /// </summary>
		public busPayeeAccountRolloverDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPayeeAccountRolloverDetailGen.
        /// </summary>
		public cdoPayeeAccountRolloverDetail icdoPayeeAccountRolloverDetail { get; set; }


        /// <summary>
        /// Gets or sets the collection object of type busPayeeAccountRolloverItemDetail. 
        /// </summary>
		public Collection<busPayeeAccountRolloverItemDetail> iclbPayeeAccountRolloverItemDetail { get; set; }



        /// <summary>
        /// MPIPHP.busPayeeAccountRolloverDetailGen.FindPayeeAccountRolloverDetail():
        /// Finds a particular record from cdoPayeeAccountRolloverDetail with its primary key. 
        /// </summary>
        /// <param name="aintPayeeAccountRolloverDetailId">A primary key value of type int of cdoPayeeAccountRolloverDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPayeeAccountRolloverDetail(int aintPayeeAccountRolloverDetailId)
		{
			bool lblnResult = false;
			if (icdoPayeeAccountRolloverDetail == null)
			{
				icdoPayeeAccountRolloverDetail = new cdoPayeeAccountRolloverDetail();
			}
			if (icdoPayeeAccountRolloverDetail.SelectRow(new object[1] { aintPayeeAccountRolloverDetailId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// MPIPHP.busPayeeAccountRolloverDetailGen.LoadPayeeAccountRolloverItemDetails():
        /// Loads Collection object iclbPayeeAccountRolloverItemDetail of type busPayeeAccountRolloverItemDetail.
        /// </summary>
		public virtual void LoadPayeeAccountRolloverItemDetails()
		{
			DataTable ldtbList = Select<cdoPayeeAccountRolloverItemDetail>(
				new string[1] { enmPayeeAccountRolloverItemDetail.payee_account_rollover_detail_id.ToString() },
				new object[1] { icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id }, null, null);
			iclbPayeeAccountRolloverItemDetail = GetCollection<busPayeeAccountRolloverItemDetail>(ldtbList, "icdoPayeeAccountRolloverItemDetail");
		}

	}
}
