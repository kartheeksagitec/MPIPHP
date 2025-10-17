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
using MPIPHP.BusinessObjects;
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP
{
    /// <summary>
    /// Class MPIPHP.busPaymentDirectivesGen:
    /// Inherited from busBase, used to create new business object for main table cdoPaymentDirectives and its children table. 
    /// </summary>
	[Serializable]
	public class busPaymentDirectivesGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.busPaymentDirectivesGen
        /// </summary>
		public busPaymentDirectivesGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPaymentDirectivesGen.
        /// </summary>
		public cdoPaymentDirectives icdoPaymentDirectives { get; set; }




        /// <summary>
        /// MPIPHP.busPaymentDirectivesGen.FindPaymentDirectives():
        /// Finds a particular record from cdoPaymentDirectives with its primary key. 
        /// </summary>
        /// <param name="aintPaymentDirectivesId">A primary key value of type int of cdoPaymentDirectives on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPaymentDirectives(int aintPaymentDirectivesId)
		{
			bool lblnResult = false;
			if (icdoPaymentDirectives == null)
			{
				icdoPaymentDirectives = new cdoPaymentDirectives();
			}
			if (icdoPaymentDirectives.SelectRow(new object[1] { aintPaymentDirectivesId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
