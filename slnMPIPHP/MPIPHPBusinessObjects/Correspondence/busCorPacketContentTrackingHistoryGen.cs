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
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.busCorPacketContentTrackingHistoryGen:
    /// Inherited from busBase, used to create new business object for main table cdoCorPacketContentTrackingHistory and its children table. 
    /// </summary>
	[Serializable]
	public class busCorPacketContentTrackingHistoryGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.busCorPacketContentTrackingHistoryGen
        /// </summary>
		public busCorPacketContentTrackingHistoryGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busCorPacketContentTrackingHistoryGen.
        /// </summary>
		public cdoCorPacketContentTrackingHistory icdoCorPacketContentTrackingHistory { get; set; }




        /// <summary>
        /// MPIPHP.busCorPacketContentTrackingHistoryGen.FindCorPacketContentTrackingHistory():
        /// Finds a particular record from cdoCorPacketContentTrackingHistory with its primary key. 
        /// </summary>
        /// <param name="a">A primary key value of type  of cdoCorPacketContentTrackingHistory on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindCorPacketContentTrackingHistory(int aintCorPacketContentTrackingHistoryId)
		{
			bool lblnResult = false;
			if (icdoCorPacketContentTrackingHistory == null)
			{
				icdoCorPacketContentTrackingHistory = new cdoCorPacketContentTrackingHistory();
			}
			if (icdoCorPacketContentTrackingHistory.SelectRow(new object[1] { aintCorPacketContentTrackingHistoryId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
