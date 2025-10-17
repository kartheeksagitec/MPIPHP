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
    /// Class MPIPHP.BusinessObjects.busActivityInstanceHistoryGen:
    /// Inherited from busBase, used to create new business object for main table cdoActivityInstanceHistory and its children table. 
    /// </summary>
	[Serializable]
	public class busActivityInstanceHistoryGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busActivityInstanceHistoryGen
        /// </summary>
		public busActivityInstanceHistoryGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busActivityInstanceHistoryGen.
        /// </summary>
		public cdoActivityInstanceHistory icdoActivityInstanceHistory { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busActivityInstance.
        /// </summary>
		public busActivityInstance ibusSolutionActivityInstance { get; set; }




        /// <summary>
        /// MPIPHP.busActivityInstanceHistoryGen.FindActivityInstanceHistory():
        /// Finds a particular record from cdoActivityInstanceHistory with its primary key. 
        /// </summary>
        /// <param name="aintactivityinstancehistoryid">A primary key value of type int of cdoActivityInstanceHistory on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindActivityInstanceHistory(int aintactivityinstancehistoryid)
		{
			bool lblnResult = false;
			if (icdoActivityInstanceHistory == null)
			{
				icdoActivityInstanceHistory = new cdoActivityInstanceHistory();
			}
			if (icdoActivityInstanceHistory.SelectRow(new object[1] { aintactivityinstancehistoryid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// MPIPHP.busActivityInstanceHistoryGen.LoadActivityInstance():
        /// Loads non-collection object ibusActivityInstance of type busActivityInstance.
        /// </summary>
		public virtual void LoadActivityInstance()
		{
			if (ibusSolutionActivityInstance == null)
			{
				ibusSolutionActivityInstance = new busActivityInstance();
			}
			ibusSolutionActivityInstance.FindActivityInstance(icdoActivityInstanceHistory.activity_instance_id);
		}

	}
}
