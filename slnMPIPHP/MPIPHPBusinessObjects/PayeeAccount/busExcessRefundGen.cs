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
    /// Class MPIPHP.BusinessObjects.busExcessRefundGen:
    /// Inherited from busBase, used to create new business object for main table cdoExcessRefund and its children table. 
    /// </summary>
	[Serializable]
	public class busExcessRefundGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busExcessRefundGen
        /// </summary>
		public busExcessRefundGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busExcessRefundGen.
        /// </summary>
		public cdoExcessRefund icdoExcessRefund { get; set; }




        /// <summary>
        /// MPIPHP.busExcessRefundGen.FindExcessRefund():
        /// Finds a particular record from cdoExcessRefund with its primary key. 
        /// </summary>
        /// <param name="aintExcessRefunId">A primary key value of type int of cdoExcessRefund on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindExcessRefund(int aintExcessRefunId)
		{
			bool lblnResult = false;
			if (icdoExcessRefund == null)
			{
				icdoExcessRefund = new cdoExcessRefund();
			}
			if (icdoExcessRefund.SelectRow(new object[1] { aintExcessRefunId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
