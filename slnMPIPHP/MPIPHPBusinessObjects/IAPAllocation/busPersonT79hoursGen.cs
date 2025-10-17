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
    /// Class MPIPHP.busPersonT79hoursGen:
    /// Inherited from busMPIPHPBase, used to create new business object for main table cdoPersonT79hours and its children table. 
    /// </summary>
	[Serializable]
	public class busPersonT79hoursGen : busMPIPHPBase
	{
        /// <summary>
        /// Constructor for MPIPHP.busPersonT79hoursGen
        /// </summary>
		public busPersonT79hoursGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPersonT79hoursGen.
        /// </summary>
		public cdoPersonT79hours icdoPersonT79hours { get; set; }




        /// <summary>
        /// MPIPHP.busPersonT79hoursGen.FindPersonT79hours():
        /// Finds a particular record from cdoPersonT79hours with its primary key. 
        /// </summary>
        /// <param name="aintPersonT97Id">A primary key value of type int of cdoPersonT79hours on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPersonT79hours(int aintPersonT97Id)
		{
			bool lblnResult = false;
			if (icdoPersonT79hours == null)
			{
				icdoPersonT79hours = new cdoPersonT79hours();
			}
			if (icdoPersonT79hours.SelectRow(new object[1] { aintPersonT97Id }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
