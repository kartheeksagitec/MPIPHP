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
    /// Class MPIPHP.BusinessObjects.busWorkflowRequestGen:
    /// Inherited from busBase, used to create new business object for main table cdoWorkflowRequest and its children table. 
    /// </summary>
	[Serializable]
	public class busWorkflowRequestGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busWorkflowRequestGen
        /// </summary>
		public busWorkflowRequestGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busWorkflowRequestGen.
        /// </summary>
		public cdoWorkflowRequest icdoWorkflowRequest { get; set; }




        /// <summary>
        /// MPIPHP.busWorkflowRequestGen.FindWorkflowRequest():
        /// Finds a particular record from cdoWorkflowRequest with its primary key. 
        /// </summary>
        /// <param name="aintworkflowrequestid">A primary key value of type int of cdoWorkflowRequest on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindWorkflowRequest(int aintworkflowrequestid)
		{
			bool lblnResult = false;
			if (icdoWorkflowRequest == null)
			{
				icdoWorkflowRequest = new cdoWorkflowRequest();
			}
			if (icdoWorkflowRequest.SelectRow(new object[1] { aintworkflowrequestid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
