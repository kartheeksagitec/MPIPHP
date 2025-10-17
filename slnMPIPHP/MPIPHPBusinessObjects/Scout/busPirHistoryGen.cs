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
    /// Class MPIPHP.BusinessObjects.busPirHistoryGen:
    /// Inherited from busBase, used to create new business object for main table cdoPirHistory and its children table. 
    /// </summary>
	[Serializable]
	public class busPirHistoryGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPirHistoryGen
        /// </summary>
		public busPirHistoryGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPirHistoryGen.
        /// </summary>
		public cdoPirHistory icdoPirHistory { get; set; }




        /// <summary>
        /// MPIPHP.busPirHistoryGen.FindPirHistory():
        /// Finds a particular record from cdoPirHistory with its primary key. 
        /// </summary>
        /// <param name="aintpirhistoryid">A primary key value of type int of cdoPirHistory on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPirHistory(int aintpirhistoryid)
		{
			bool lblnResult = false;
			if (icdoPirHistory == null)
			{
				icdoPirHistory = new cdoPirHistory();
			}
			if (icdoPirHistory.SelectRow(new object[1] { aintpirhistoryid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
