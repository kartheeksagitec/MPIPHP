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
    /// Class MPIPHP.BusinessObjects.busDroApplicationStatusHistoryGen:
    /// Inherited from busBase, used to create new business object for main table cdoDroApplicationStatusHistory and its children table. 
    /// </summary>
	[Serializable]
	public class busDroApplicationStatusHistoryGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busDroApplicationStatusHistoryGen
        /// </summary>
		public busDroApplicationStatusHistoryGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busDroApplicationStatusHistoryGen.
        /// </summary>
		public cdoDroApplicationStatusHistory icdoDroApplicationStatusHistory { get; set; }




        /// <summary>
        /// MPIPHP.busDroApplicationStatusHistoryGen.FindDroApplicationStatusHistory():
        /// Finds a particular record from cdoDroApplicationStatusHistory with its primary key. 
        /// </summary>
        /// <param name="aintdroapplicationstatushistoryid">A primary key value of type int of cdoDroApplicationStatusHistory on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindDroApplicationStatusHistory(int aintdroapplicationstatushistoryid)
		{
			bool lblnResult = false;
			if (icdoDroApplicationStatusHistory == null)
			{
				icdoDroApplicationStatusHistory = new cdoDroApplicationStatusHistory();
			}
			if (icdoDroApplicationStatusHistory.SelectRow(new object[1] { aintdroapplicationstatushistoryid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
