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
	/// Class MPIPHP.BusinessObjects.busUserActivityLogDetail:
	/// Inherited from busUserActivityLogDetailGen, the class is used to customize the business object busUserActivityLogDetailGen.
	/// </summary>
	[Serializable]
	public class busUserActivityLogDetail : busUserActivityLogDetailGen
	{

        public override void LoadUserActivityLogQueriess()
        {
            DataTable ldtbUserActivityLog = busBase.Select("cdoUserActivityLogQueries.GetQueriesWithParamCount", 
                new object[1] { icdoUserActivityLogDetail.transaction_id.ToString()});
            iclbUserActivityLogQueries = GetCollection<busUserActivityLogQueries>(ldtbUserActivityLog, "icdoUserActivityLogQueries");
        }
	}
}
