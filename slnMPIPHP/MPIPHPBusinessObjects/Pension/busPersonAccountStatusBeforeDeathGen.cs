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

namespace MPIPHP
{
    /// <summary>
    /// Class MPIPHP.busPersonAccountStatusBeforeDeathGen:
    /// Inherited from busBase, used to create new business object for main table cdoPersonAccountStatusBeforeDeath and its children table. 
    /// </summary>
	[Serializable]
	public class busPersonAccountStatusBeforeDeathGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.busPersonAccountStatusBeforeDeathGen
        /// </summary>
		public busPersonAccountStatusBeforeDeathGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPersonAccountStatusBeforeDeathGen.
        /// </summary>
		public cdoPersonAccountStatusBeforeDeath icdoPersonAccountStatusBeforeDeath { get; set; }




        /// <summary>
        /// MPIPHP.busPersonAccountStatusBeforeDeathGen.FindPersonAccountStatusBeforeDeath():
        /// Finds a particular record from cdoPersonAccountStatusBeforeDeath with its primary key. 
        /// </summary>
        /// <param name="aintPersonAccountStatusBeforeDeathId">A primary key value of type int of cdoPersonAccountStatusBeforeDeath on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPersonAccountStatusBeforeDeath(int aintPersonAccountStatusBeforeDeathId)
		{
			bool lblnResult = false;
			if (icdoPersonAccountStatusBeforeDeath == null)
			{
				icdoPersonAccountStatusBeforeDeath = new cdoPersonAccountStatusBeforeDeath();
			}
			if (icdoPersonAccountStatusBeforeDeath.SelectRow(new object[1] { aintPersonAccountStatusBeforeDeathId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
