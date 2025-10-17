#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using  MPIPHP.CustomDataObjects;

#endregion

namespace  MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class  MPIPHP.BusinessObjects.busActivityGen:
    /// Inherited from busBase, used to create new business object for main table cdoActivity and its children table. 
    /// </summary>
	[Serializable]
	public class busActivityGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for  MPIPHP.BusinessObjects.busActivityGen
        /// </summary>
		public busActivityGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busActivityGen.
        /// </summary>
		public cdoActivity icdoActivity { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busProcess.
        /// </summary>
		public busProcess ibusProcess { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busRoles.
        /// </summary>
		public busRoles ibusRoles { get; set; }




        /// <summary>
        ///  MPIPHP.busActivityGen.FindActivity():
        /// Finds a particular record from cdoActivity with its primary key. 
        /// </summary>
        /// <param name="aintactivityid">A primary key value of type int of cdoActivity on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindActivity(int aintactivityid)
		{
			bool lblnResult = false;
			if (icdoActivity == null)
			{
				icdoActivity = new cdoActivity();
			}
			if (icdoActivity.SelectRow(new object[1] { aintactivityid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        ///  MPIPHP.busActivityGen.LoadProcess():
        /// Loads non-collection object ibusProcess of type busProcess.
        /// </summary>
		public virtual void LoadProcess()
		{
			if (ibusProcess == null)
			{
				ibusProcess = new busProcess();
			}
			ibusProcess.FindProcess(icdoActivity.process_id);
		}

        /// <summary>
        ///  MPIPHP.busActivityGen.LoadRoles():
        /// Loads non-collection object ibusRoles of type busRoles.
        /// </summary>
		public virtual void LoadRoles()
		{
			if (ibusRoles == null)
			{
				ibusRoles = new busRoles();
			}
			ibusRoles.FindRole(icdoActivity.role_id);
		}

	}
}
