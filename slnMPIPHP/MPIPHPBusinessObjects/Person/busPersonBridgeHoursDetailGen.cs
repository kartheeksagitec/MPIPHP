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
    /// Class MPIPHP.BusinessObjects.busPersonBridgeHoursDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoPersonBridgeHoursDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busPersonBridgeHoursDetailGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPersonBridgeHoursDetailGen
        /// </summary>
		public busPersonBridgeHoursDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPersonBridgeHoursDetailGen.
        /// </summary>
		public cdoPersonBridgeHoursDetail icdoPersonBridgeHoursDetail { get; set; }




        /// <summary>
        /// MPIPHP.busPersonBridgeHoursDetailGen.FindPersonBridgeHoursDetail():
        /// Finds a particular record from cdoPersonBridgeHoursDetail with its primary key. 
        /// </summary>
        /// <param name="aintpersonbridgehoursdetailid">A primary key value of type int of cdoPersonBridgeHoursDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPersonBridgeHoursDetail(int aintpersonbridgehoursdetailid)
		{
			bool lblnResult = false;
			if (icdoPersonBridgeHoursDetail == null)
			{
				icdoPersonBridgeHoursDetail = new cdoPersonBridgeHoursDetail();
			}
			if (icdoPersonBridgeHoursDetail.SelectRow(new object[1] { aintpersonbridgehoursdetailid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
