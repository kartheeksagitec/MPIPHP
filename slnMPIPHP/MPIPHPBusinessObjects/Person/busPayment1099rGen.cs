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
    /// Class MPIPHP.BusinessObjects.busPayment1099rGen:
    /// Inherited from busBase, used to create new business object for main table cdoPayment1099r and its children table. 
    /// </summary>
	[Serializable]
	public class busPayment1099rGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPayment1099rGen
        /// </summary>
		public busPayment1099rGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPayment1099rGen.
        /// </summary>
		public cdoPayment1099r icdoPayment1099r { get; set; }


        /// <summary>
        /// Gets or sets the collection object of type busPayment1099rHistoryLink. 
        /// </summary>
		public Collection<busPayment1099rHistoryLink> iclbPayment1099rHistoryLink { get; set; }



        /// <summary>
        /// MPIPHP.busPayment1099rGen.FindPayment1099r():
        /// Finds a particular record from cdoPayment1099r with its primary key. 
        /// </summary>
        /// <param name="aintpayment1099rid">A primary key value of type int of cdoPayment1099r on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPayment1099r(int aintpayment1099rid)
		{
			bool lblnResult = false;
			if (icdoPayment1099r == null)
			{
				icdoPayment1099r = new cdoPayment1099r();
			}
			if (icdoPayment1099r.SelectRow(new object[1] { aintpayment1099rid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// MPIPHP.busPayment1099rGen.LoadPayment1099rHistoryLinks():
        /// Loads Collection object iclbPayment1099rHistoryLink of type busPayment1099rHistoryLink.
        /// </summary>
		public virtual void LoadPayment1099rHistoryLinks()
		{
			DataTable ldtbList = Select<cdoPayment1099rHistoryLink>(
				new string[1] { enmPayment1099rHistoryLink.payment_1099r_id.ToString() },
				new object[1] { icdoPayment1099r.payment_1099r_id }, null, null);
			iclbPayment1099rHistoryLink = GetCollection<busPayment1099rHistoryLink>(ldtbList, "icdoPayment1099rHistoryLink");
		}

	}
}
