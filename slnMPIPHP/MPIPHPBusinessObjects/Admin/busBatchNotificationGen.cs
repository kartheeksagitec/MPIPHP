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
    /// Class MPIPHP.BusinessObjects.busBatchNotificationGen:
    /// Inherited from busBase, used to create new business object for main table cdoBatchNotification and its children table. 
    /// </summary>
	[Serializable]
	public class busBatchNotificationGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busBatchNotificationGen
        /// </summary>
		public busBatchNotificationGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busBatchNotificationGen.
        /// </summary>
		public cdoBatchNotification icdoBatchNotification { get; set; }




        /// <summary>
        /// MPIPHP.busBatchNotificationGen.FindBatchNotification():
        /// Finds a particular record from cdoBatchNotification with its primary key. 
        /// </summary>
        /// <param name="aintbatchnotificationid">A primary key value of type int of cdoBatchNotification on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindBatchNotification(int aintbatchnotificationid)
		{
			bool lblnResult = false;
			if (icdoBatchNotification == null)
			{
				icdoBatchNotification = new cdoBatchNotification();
			}
			if (icdoBatchNotification.SelectRow(new object[1] { aintbatchnotificationid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
