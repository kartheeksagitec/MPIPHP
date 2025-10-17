#region Using directives
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.DataObjects;

#endregion
namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// partial class NeoSpin.BusinessObjects.busPayeeAccountWireDetail
    /// </summary>	
	public partial class busPayeeAccountWireDetail 
	{
		
        /// <summary>
        /// Gets or sets the main-table object contained in busPayeeAccountWireDetail.
        /// </summary>
		public doPayeeAccountWireDetail icdoPayeeAccountWireDetail { get; set; }
		
        /// <summary>
        /// Gets or sets the non-collection object of type busPayeeAccount.
        /// </summary>
		public busPayeeAccount ibusPayeeAccount { get; set; }

		public virtual bool FindPayeeAccountWireDetail(int aintPayeeAccountWireDetailId)
		{
			bool lblnResult = false;
			if (icdoPayeeAccountWireDetail == null)
			{
				icdoPayeeAccountWireDetail = new doPayeeAccountWireDetail();
			}
			if (icdoPayeeAccountWireDetail.SelectRow(new object[1] { aintPayeeAccountWireDetailId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}


	

}
