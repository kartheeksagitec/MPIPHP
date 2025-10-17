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
    /// Class MPIPHP.BusinessObjects.busUserActivityLogQueryParametersGen:
    /// Inherited from busBase, used to create new business object for main table cdoUserActivityLogQueryParameters and its children table. 
    /// </summary>
	[Serializable]
	public class busUserActivityLogQueryParametersGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busUserActivityLogQueryParametersGen
        /// </summary>
		public busUserActivityLogQueryParametersGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busUserActivityLogQueryParametersGen.
        /// </summary>
		public cdoUserActivityLogQueryParameters icdoUserActivityLogQueryParameters { get; set; }




        /// <summary>
        /// NeoSpin.busUserActivityLogQueryParametersGen.FindUserActivityLogQueryParameters():
        /// Finds a particular record from cdoUserActivityLogQueryParameters with its primary key. 
        /// </summary>
        /// <param name="aintUserActivityLogQueryParameterId">A primary key value of type int of cdoUserActivityLogQueryParameters on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindUserActivityLogQueryParameters(int aintUserActivityLogQueryParameterId)
		{
			bool lblnResult = false;
			if (icdoUserActivityLogQueryParameters == null)
			{
				icdoUserActivityLogQueryParameters = new cdoUserActivityLogQueryParameters();
			}
			if (icdoUserActivityLogQueryParameters.SelectRow(new object[1] { aintUserActivityLogQueryParameterId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
