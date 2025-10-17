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
    /// Class MPIPHP.BusinessObjects.busPersonBridgeHoursGen:
    /// Inherited from busBase, used to create new business object for main table cdoPersonBridgeHours and its children table. 
    /// </summary>
	[Serializable]
	public class busPersonBridgeHoursGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPersonBridgeHoursGen
        /// </summary>
		public busPersonBridgeHoursGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPersonBridgeHoursGen.
        /// </summary>
		public cdoPersonBridgeHours icdoPersonBridgeHours { get; set; }




        /// <summary>
        /// MPIPHP.busPersonBridgeHoursGen.FindPersonBridgeHours():
        /// Finds a particular record from cdoPersonBridgeHours with its primary key. 
        /// </summary>
        /// <param name="aintpersonbridgeid">A primary key value of type int of cdoPersonBridgeHours on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPersonBridgeHours(int aintpersonbridgeid)
		{
			bool lblnResult = false;
			if (icdoPersonBridgeHours == null)
			{
				icdoPersonBridgeHours = new cdoPersonBridgeHours();
			}
			if (icdoPersonBridgeHours.SelectRow(new object[1] { aintpersonbridgeid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
