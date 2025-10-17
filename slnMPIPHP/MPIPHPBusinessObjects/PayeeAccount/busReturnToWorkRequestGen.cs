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
	/// Class MPIPHP.BusinessObjects.busReturnToWorkGen:
	/// Inherited from busBase, used to create new business object for main table cdoReemploymentHistory and its children table. 
	/// </summary>
	[Serializable]
	public class busReturnToWorkRequestGen : busMPIPHPBase
    {
		/// <summary>
		/// Constructor for MPIPHP.BusinessObjects.busReturnToWorkGen
		/// </summary>
		public busReturnToWorkRequestGen()
		{

		}

		/// <summary>
		/// Gets or sets the main-table object contained in busReturnToWorGen.
		/// </summary>
		public cdoReturnToWorkRequest icdoReturnToWorkRequest { get; set; }

	


		/// <summary>
		/// MPIPHP.busReturnToWorGen.FindReturnToWor():
		/// Finds a particular record from cdoReemploymentHistory with its primary key. 
		/// </summary>
		/// <param name="aintReemploymentHistoryId">A primary key value of type int of cdoReemploymentHistory on which search is performed.</param>
		/// <returns>true if found otherwise false</returns>
		public virtual bool FindReturnToWorkRequest(int aintReEmploymentNotificationId)
		{
			bool lblnResult = false;
			if (icdoReturnToWorkRequest == null)
			{
				icdoReturnToWorkRequest = new cdoReturnToWorkRequest();
			}
			if (icdoReturnToWorkRequest.SelectRow(new object[1] { aintReEmploymentNotificationId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
