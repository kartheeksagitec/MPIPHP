#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using    MPIPHP.CustomDataObjects;
using NeoSpin.DataObjects;

#endregion

namespace    MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class    MPIPHP.BusinessObjects.busUserActivityLogQueriesGen:
    /// Inherited from busBase, used to create new business object for main table cdoUserActivityLogQueries and its children table. 
    /// </summary>
	[Serializable]
	public class busUserActivityLogQueriesGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for    MPIPHP.BusinessObjects.busUserActivityLogQueriesGen
        /// </summary>
		public busUserActivityLogQueriesGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busUserActivityLogQueriesGen.
        /// </summary>
		public cdoUserActivityLogQueries icdoUserActivityLogQueries { get; set; }


        /// <summary>
        /// Gets or sets the collection object of type busUserActivityLogQueryParameters. 
        /// </summary>
		public Collection<busUserActivityLogQueryParameters> iclbUserActivityLogQueryParameters { get; set; }



        /// <summary>
        ///    NeoSpin.busUserActivityLogQueriesGen.FindUserActivityLogQueries():
        /// Finds a particular record from cdoUserActivityLogQueries with its primary key. 
        /// </summary>
        /// <param name="aintUserActivityLogQueryId">A primary key value of type int of cdoUserActivityLogQueries on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindUserActivityLogQueries(int aintUserActivityLogQueryId)
		{
			bool lblnResult = false;
			if (icdoUserActivityLogQueries == null)
			{
				icdoUserActivityLogQueries = new cdoUserActivityLogQueries();
			}
			if (icdoUserActivityLogQueries.SelectRow(new object[1] { aintUserActivityLogQueryId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        ///    NeoSpin.busUserActivityLogQueriesGen.LoadUserActivityLogQueryParameterss():
        /// Loads Collection object iclbUserActivityLogQueryParameters of type busUserActivityLogQueryParameters.
        /// </summary>
		public virtual void LoadUserActivityLogQueryParameterss()
		{
			DataTable ldtbList = Select<cdoUserActivityLogQueryParameters>(
				new string[1] { enmUserActivityLogQueryParameters.user_activity_log_query_id.ToString() },
				new object[1] { icdoUserActivityLogQueries.user_activity_log_query_id }, null, null);
			iclbUserActivityLogQueryParameters = GetCollection<busUserActivityLogQueryParameters>(ldtbList, "icdoUserActivityLogQueryParameters");
		}

	}
}
