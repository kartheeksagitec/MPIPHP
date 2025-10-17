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
    /// Class MPIPHP.BusinessObjects.busUserActivityLogParametersGen:
    /// Inherited from busBase, used to create new business object for main table cdoUserActivityLogParameters and its children table. 
    /// </summary>
	[Serializable]
	public class busUserActivityLogParametersGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busUserActivityLogParametersGen
        /// </summary>
		public busUserActivityLogParametersGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busUserActivityLogParametersGen.
        /// </summary>
		public cdoUserActivityLogParameters icdoUserActivityLogParameters { get; set; }




        /// <summary>
        /// NeoSpin.busUserActivityLogParametersGen.FindUserActivityLogParameters():
        /// Finds a particular record from cdoUserActivityLogParameters with its primary key. 
        /// </summary>
        /// <param name="aintUserActivityLogParameterId">A primary key value of type int of cdoUserActivityLogParameters on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindUserActivityLogParameters(int aintUserActivityLogParameterId)
		{
			bool lblnResult = false;
			if (icdoUserActivityLogParameters == null)
			{
				icdoUserActivityLogParameters = new cdoUserActivityLogParameters();
			}
			if (icdoUserActivityLogParameters.SelectRow(new object[1] { aintUserActivityLogParameterId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
