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
    /// Inherited from busBase, used to create new business object for main table cdoJobHeader and its children table. 
    /// </summary>
	[Serializable]
	public class busJobHeaderGen : busMPIPHPBase
    {
        /// <summary>
        /// </summary>
		public busJobHeaderGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busJobHeaderGen.
        /// </summary>
		public cdoJobHeader icdoJobHeader { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busJobSchedule.
        /// </summary>
		public busJobSchedule ibusJobSchedule { get; set; }



        /// <summary>
        /// Gets or sets the Sagitec.Common.utlCollection object of type cdoProcessLog. 
        /// </summary>
		public utlCollection<cdoProcessLog> iclcProcessLog { get; set; }

        /// <summary>
        /// Finds a particular record from cdoJobHeader with its primary key. 
        /// </summary>
        /// <param name="aintjobheaderid">A primary key value of type int of cdoJobHeader on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindJobHeader(int aintjobheaderid)
		{
			bool lblnResult = false;
			if (icdoJobHeader == null)
			{
				icdoJobHeader = new cdoJobHeader();
			}
			if (icdoJobHeader.SelectRow(new object[1] { aintjobheaderid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// Loads non-collection object ibusJobSchedule of type busJobSchedule.
        /// </summary>
		public virtual void LoadJobSchedule()
		{
			if (ibusJobSchedule == null)
			{
				ibusJobSchedule = new busJobSchedule();
			}
			ibusJobSchedule.FindJobSchedule(icdoJobHeader.job_schedule_id);
		}

        /// <summary>
        ///    MPIPHP.busJobHeaderGen.LoadProcessLogs():
        /// Loads Sagitec.Common.utlCollection object iclcProcessLog of type cdoProcessLog.
        /// </summary>
		public virtual void LoadProcessLogs()
		{
			iclcProcessLog = GetCollection<cdoProcessLog>(
				new string[1] { MPIPHP.DataObjects.doProcessLog.enmProcessLog.job_header_id.ToString() }, 
				new object[1] { icdoJobHeader.job_header_id }, null, null);
		}

	}
}
