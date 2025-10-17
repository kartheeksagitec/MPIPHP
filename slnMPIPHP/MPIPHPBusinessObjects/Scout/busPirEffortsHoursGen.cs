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

#endregion

namespace MPIPHP
{
    /// <summary>
    /// Class MPIPHP.busPirEffortsHoursGen:
    /// Inherited from busBase, used to create new business object for main table cdoPirEffortsHours and its children table. 
    /// </summary>
	[Serializable]
	public class busPirEffortsHoursGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.busPirEffortsHoursGen
        /// </summary>
		public busPirEffortsHoursGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPirEffortsHoursGen.
        /// </summary>
		public cdoPirEffortsHours icdoPirEffortsHours { get; set; }




        /// <summary>
        /// MPIPHP.busPirEffortsHoursGen.FindPirEffortsHours():
        /// Finds a particular record from cdoPirEffortsHours with its primary key. 
        /// </summary>
        /// <param name="aintPirEffortsHoursId">A primary key value of type int of cdoPirEffortsHours on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPirEffortsHours(int aintPirEffortsHoursId)
		{
			bool lblnResult = false;
			if (icdoPirEffortsHours == null)
			{
				icdoPirEffortsHours = new cdoPirEffortsHours();
			}
			if (icdoPirEffortsHours.SelectRow(new object[1] { aintPirEffortsHoursId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
