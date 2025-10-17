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
    /// Class MPIPHP.BusinessObjects.busHealthEligibiltyActuaryDataGen:
    /// Inherited from busBase, used to create new business object for main table cdoHealthEligibiltyActuaryData and its children table. 
    /// </summary>
	[Serializable]
	public class busHealthEligibiltyActuaryDataGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busHealthEligibiltyActuaryDataGen
        /// </summary>
		public busHealthEligibiltyActuaryDataGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busHealthEligibiltyActuaryDataGen.
        /// </summary>
		public cdoHealthEligibiltyActuaryData icdoHealthEligibiltyActuaryData { get; set; }




        /// <summary>
        /// MPIPHP.busHealthEligibiltyActuaryDataGen.FindHealthEligibiltyActuaryData():
        /// Finds a particular record from cdoHealthEligibiltyActuaryData with its primary key. 
        /// </summary>
        /// <param name="ainthealthactuarydataid">A primary key value of type int of cdoHealthEligibiltyActuaryData on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindHealthEligibiltyActuaryData(int ainthealthactuarydataid)
		{
			bool lblnResult = false;
			if (icdoHealthEligibiltyActuaryData == null)
			{
				icdoHealthEligibiltyActuaryData = new cdoHealthEligibiltyActuaryData();
			}
			if (icdoHealthEligibiltyActuaryData.SelectRow(new object[1] { ainthealthactuarydataid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
