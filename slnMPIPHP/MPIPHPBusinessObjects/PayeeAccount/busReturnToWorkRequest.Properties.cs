#region Using directives
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.DataObjects;
using MPIPHP.DataObjects;
using MPIPHP.CustomDataObjects;
using System.Linq;
#endregion
namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// partial class NeoSpin.BusinessObjects.busReturntoWork
    /// </summary>	
	public partial class busReturnToWorkRequest : busReturnToWorkRequestGen
	{

		/// <summary>
		/// Gets or sets the main-table object contained in busReturntoWork.
		/// </summary>
		public busPerson ibusPayee { get; set; }
		public Collection<busPayeeAccount> iclbPayeeAccount { get; set; }

		public Collection<busCorTracking> iclbCorTracking { get; set; }

		public Collection<busNotes> iclbNotes { get; set; }

		public cdoNotes icdoNotes { get; set; }

		public Collection<busError> iclbReemploymentErrors { get; set; }

        /// <summary>
        /// Gets or sets the collection object of type busReturnToWorkRequest. 
        /// </summary>
		public Collection<busReturnToWorkRequest> iclbReturnToWorkHistory { get; set; }

		public busErtwSharedNotes ibusErtwSharedNotes { get; set; }
		
		public Collection<busErtwAuditingNotes> iclbErtwAuditingNotes { get; set; }
	}

}
