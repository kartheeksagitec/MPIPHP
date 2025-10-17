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
    /// Class MPIPHP.BusinessObjects.busRequestParameterGen:
    /// Inherited from busBase, used to create new business object for main table cdoRequestParameter and its children table. 
    /// </summary>
	[Serializable]
	public class busRequestParameterGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.BusinessObjects.busRequestParameterGen
        /// </summary>
		public busRequestParameterGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busRequestParameterGen.
        /// </summary>
		public cdoRequestParameter icdoRequestParameter { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busWorkflowRequest.
        /// </summary>
		public busWorkflowRequest ibusWorkflowRequest { get; set; }




        /// <summary>
        /// MPIPHP.busRequestParameterGen.FindRequestParameter():
        /// Finds a particular record from cdoRequestParameter with its primary key. 
        /// </summary>
        /// <param name="aintRequestParameterId">A primary key value of type int of cdoRequestParameter on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindRequestParameter(int aintRequestParameterId)
		{
			bool lblnResult = false;
			if (icdoRequestParameter == null)
			{
				icdoRequestParameter = new cdoRequestParameter();
			}
			if (icdoRequestParameter.SelectRow(new object[1] { aintRequestParameterId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// MPIPHP.busRequestParameterGen.LoadWorkflowRequest():
        /// Loads non-collection object ibusWorkflowRequest of type busWorkflowRequest.
        /// </summary>
		public virtual void LoadWorkflowRequest()
		{
			if (ibusWorkflowRequest == null)
			{
				ibusWorkflowRequest = new busWorkflowRequest();
			}
			ibusWorkflowRequest.FindWorkflowRequest(icdoRequestParameter.workflow_request_id);
		}

	}
}
