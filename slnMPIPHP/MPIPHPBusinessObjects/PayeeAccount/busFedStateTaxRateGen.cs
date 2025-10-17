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
    /// Class MPIPHP.BusinessObjects.busFedStateTaxRateGen:
    /// Inherited from busBase, used to create new business object for main table cdoFedStateTaxRate and its children table. 
    /// </summary>
	[Serializable]
	public class busFedStateTaxRateGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busFedStateTaxRateGen
        /// </summary>
		public busFedStateTaxRateGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busFedStateTaxRateGen.
        /// </summary>
		public cdoFedStateTaxRate icdoFedStateTaxRate { get; set; }




        /// <summary>
        /// MPIPHP.busFedStateTaxRateGen.FindFedStateTaxRate():
        /// Finds a particular record from cdoFedStateTaxRate with its primary key. 
        /// </summary>
        /// <param name="aintFedStateTaxId">A primary key value of type int of cdoFedStateTaxRate on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindFedStateTaxRate(int aintFedStateTaxId)
		{
			bool lblnResult = false;
			if (icdoFedStateTaxRate == null)
			{
				icdoFedStateTaxRate = new cdoFedStateTaxRate();
			}
			if (icdoFedStateTaxRate.SelectRow(new object[1] { aintFedStateTaxId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
