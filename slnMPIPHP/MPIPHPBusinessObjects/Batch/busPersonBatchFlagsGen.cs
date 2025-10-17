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
    /// Class MPIPHP.BusinessObjects.busPersonBatchFlagsGen:
    /// Inherited from busBase, used to create new business object for main table cdoPersonBatchFlags and its children table. 
    /// </summary>
	[Serializable]
	public class busPersonBatchFlagsGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busPersonBatchFlagsGen
        /// </summary>
		public busPersonBatchFlagsGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPersonBatchFlagsGen.
        /// </summary>
		public cdoPersonBatchFlags icdoPersonBatchFlags { get; set; }




        /// <summary>
        /// MPIPHP.busPersonBatchFlagsGen.FindPersonBatchFlags():
        /// Finds a particular record from cdoPersonBatchFlags with its primary key. 
        /// </summary>
        /// <param name="aintpersonbatchflagid">A primary key value of type int of cdoPersonBatchFlags on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPersonBatchFlags(int aintpersonbatchflagid)
		{
			bool lblnResult = false;
			if (icdoPersonBatchFlags == null)
			{
				icdoPersonBatchFlags = new cdoPersonBatchFlags();
			}
			if (icdoPersonBatchFlags.SelectRow(new object[1] { aintpersonbatchflagid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
