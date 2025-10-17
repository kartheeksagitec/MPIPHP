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
    /// Class    MPIPHP.BusinessObjects.busUserActivityLogDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoUserActivityLogDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busUserActivityLogDetailGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for    MPIPHP.BusinessObjects.busUserActivityLogDetailGen
        /// </summary>
		public busUserActivityLogDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busUserActivityLogDetailGen.
        /// </summary>
		public cdoUserActivityLogDetail icdoUserActivityLogDetail { get; set; }


        /// <summary>
        /// Gets or sets the collection object of type busUserActivityLogParameters. 
        /// </summary>
		public Collection<busUserActivityLogParameters> iclbUserActivityLogParameters { get; set; }

        /// <summary>
        /// Gets or sets the collection object of type busUserActivityLogQueries. 
        /// </summary>
		public Collection<busUserActivityLogQueries> iclbUserActivityLogQueries { get; set; }



        /// <summary>
        ///    NeoSpin.busUserActivityLogDetailGen.FindUserActivityLogDetail():
        /// Finds a particular record from cdoUserActivityLogDetail with its primary key. 
        /// </summary>
        /// <param name="aintUserActivityLogDetailId">A primary key value of type int of cdoUserActivityLogDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindUserActivityLogDetail(int aintUserActivityLogDetailId)
		{
			bool lblnResult = false;
			if (icdoUserActivityLogDetail == null)
			{
				icdoUserActivityLogDetail = new cdoUserActivityLogDetail();
			}
			if (icdoUserActivityLogDetail.SelectRow(new object[1] { aintUserActivityLogDetailId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        ///    NeoSpin.busUserActivityLogDetailGen.LoadUserActivityLogParameterss():
        /// Loads Collection object iclbUserActivityLogParameters of type busUserActivityLogParameters.
        /// </summary>
		public virtual void LoadUserActivityLogParameterss()
		{
			DataTable ldtbList = Select<cdoUserActivityLogParameters>(
				new string[1] { enmUserActivityLogParameters.user_activity_log_detail_id.ToString() },
				new object[1] { icdoUserActivityLogDetail.user_activity_log_detail_id }, null, null);
			iclbUserActivityLogParameters = GetCollection<busUserActivityLogParameters>(ldtbList, "icdoUserActivityLogParameters");
		}

        /// <summary>
        ///    NeoSpin.busUserActivityLogDetailGen.LoadUserActivityLogQueriess():
        /// Loads Collection object iclbUserActivityLogQueries of type busUserActivityLogQueries.
        /// </summary>
		public virtual void LoadUserActivityLogQueriess()
		{
			DataTable ldtbList = Select<cdoUserActivityLogQueries>(
				new string[1] { enmUserActivityLogQueries.transaction_id.ToString() },
				new object[1] { icdoUserActivityLogDetail.transaction_id}, null, null);
			iclbUserActivityLogQueries = GetCollection<busUserActivityLogQueries>(ldtbList, "icdoUserActivityLogQueries");
		}

	}
}
