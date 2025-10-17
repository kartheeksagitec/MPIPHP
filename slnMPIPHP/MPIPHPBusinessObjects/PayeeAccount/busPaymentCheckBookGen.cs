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
    /// Class MPIPHP.BusinessObjects.busPaymentCheckBookGen:
    /// Inherited from busBase, used to create new business object for main table cdoPaymentCheckBook and its children table. 
    /// </summary>
	[Serializable]
	public class busPaymentCheckBookGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPaymentCheckBookGen
        /// </summary>
		public busPaymentCheckBookGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPaymentCheckBookGen.
        /// </summary>
		public cdoPaymentCheckBook icdoPaymentCheckBook { get; set; }




        /// <summary>
        /// MPIPHP.busPaymentCheckBookGen.FindPaymentCheckBook():
        /// Finds a particular record from cdoPaymentCheckBook with its primary key. 
        /// </summary>
        /// <param name="aintCheckBookId">A primary key value of type int of cdoPaymentCheckBook on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPaymentCheckBook(int aintCheckBookId)
		{
			bool lblnResult = false;
			if (icdoPaymentCheckBook == null)
			{
				icdoPaymentCheckBook = new cdoPaymentCheckBook();
			}
			if (icdoPaymentCheckBook.SelectRow(new object[1] { aintCheckBookId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
