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
using NeoSpin.DataObjects;

#endregion

namespace  MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class  MPIPHP.BusinessObjects.busUserActivityLogGen:
    /// Inherited from busBase, used to create new business object for main table cdoUserActivityLog and its children table. 
    /// </summary>
	[Serializable]
	public class busUserActivityLogGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for  MPIPHP.BusinessObjects.busUserActivityLogGen
        /// </summary>
		public busUserActivityLogGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busUserActivityLogGen.
        /// </summary>
		public cdoUserActivityLog icdoUserActivityLog { get; set; }


        /// <summary>
        /// Gets or sets the collection object of type busUserActivityLogDetail. 
        /// </summary>
		public Collection<busUserActivityLogDetail> iclbUserActivityLogDetail { get; set; }



        /// <summary>
        ///  NeoSpin.busUserActivityLogGen.FindUserActivityLog():
        /// Finds a particular record from cdoUserActivityLog with its primary key. 
        /// </summary>
        /// <param name="aintUserActivityLogId">A primary key value of type int of cdoUserActivityLog on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindUserActivityLog(int aintUserActivityLogId)
		{
			bool lblnResult = false;
			if (icdoUserActivityLog == null)
			{
				icdoUserActivityLog = new cdoUserActivityLog();
			}
			if (icdoUserActivityLog.SelectRow(new object[1] { aintUserActivityLogId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        ///  NeoSpin.busUserActivityLogGen.LoadUserActivityLogDetails():
        /// Loads Collection object iclbUserActivityLogDetail of type busUserActivityLogDetail.
        /// </summary>
		public virtual void LoadUserActivityLogDetails()
		{
			DataTable ldtbList = Select<cdoUserActivityLogDetail>(
				new string[1] { enmUserActivityLogDetail.user_activity_log_id.ToString() },
				new object[1] { icdoUserActivityLog.user_activity_log_id }, null, null);
			iclbUserActivityLogDetail = GetCollection<busUserActivityLogDetail>(ldtbList, "icdoUserActivityLogDetail");
		}

	}
}
