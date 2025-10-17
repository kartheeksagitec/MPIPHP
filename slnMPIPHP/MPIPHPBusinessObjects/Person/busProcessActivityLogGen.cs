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
    /// Class MPIPHP.busProcessActivityLogGen:
    /// Inherited from busBase, used to create new business object for main table cdoProcessActivityLog and its children table. 
    /// </summary>
	[Serializable]
	public class busProcessActivityLogGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.busProcessActivityLogGen
        /// </summary>
		public busProcessActivityLogGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busProcessActivityLogGen.
        /// </summary>
		public cdoProcessActivityLog icdoProcessActivityLog { get; set; }




        /// <summary>
        /// MPIPHP.busProcessActivityLogGen.FindProcessActivityLog():
        /// Finds a particular record from cdoProcessActivityLog with its primary key. 
        /// </summary>
        /// <param name="aintProcessActivityLogId">A primary key value of type int of cdoProcessActivityLog on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindProcessActivityLog(int aintProcessActivityLogId)
		{
			bool lblnResult = false;
			if (icdoProcessActivityLog == null)
			{
				icdoProcessActivityLog = new cdoProcessActivityLog();
			}
			if (icdoProcessActivityLog.SelectRow(new object[1] { aintProcessActivityLogId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
