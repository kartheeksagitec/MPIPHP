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
    /// Class MPIPHP.BusinessObjects.busFedStateFlatTaxRateGen:
    /// Inherited from busBase, used to create new business object for main table cdoFedStateFlatTaxRate and its children table. 
    /// </summary>
	[Serializable]
	public class busFedStateFlatTaxRateGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busFedStateFlatTaxRateGen
        /// </summary>
		public busFedStateFlatTaxRateGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busFedStateFlatTaxRateGen.
        /// </summary>
		public cdoFedStateFlatTaxRate icdoFedStateFlatTaxRate { get; set; }




        /// <summary>
        /// MPIPHP.busFedStateFlatTaxRateGen.FindFedStateFlatTaxRate():
        /// Finds a particular record from cdoFedStateFlatTaxRate with its primary key. 
        /// </summary>
        /// <param name="aintFedStateFlatTaxId">A primary key value of type int of cdoFedStateFlatTaxRate on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindFedStateFlatTaxRate(int aintFedStateFlatTaxId)
		{
			bool lblnResult = false;
			if (icdoFedStateFlatTaxRate == null)
			{
				icdoFedStateFlatTaxRate = new cdoFedStateFlatTaxRate();
			}
			if (icdoFedStateFlatTaxRate.SelectRow(new object[1] { aintFedStateFlatTaxId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
