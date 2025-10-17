#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using MPIPHP.CustomDataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busUserActivityLogQueryLookup:
	/// Inherited from busUserActivityLogQueryLookupGen, this class is used to customize the lookup business object busUserActivityLogQueryLookupGen. 
	/// </summary>
	[Serializable]
	public class busUserActivityLogQueryLookup : busUserActivityLogQueryLookupGen
	{
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busUserActivityLogQueries)
            {
                busUserActivityLogQueries lbusUALQ = (busUserActivityLogQueries)aobjBus;
                lbusUALQ.ibusUserActivityLogDetail = new busUserActivityLogDetail();
                lbusUALQ.ibusUserActivityLogDetail.icdoUserActivityLogDetail = new cdoUserActivityLogDetail();
                lbusUALQ.ibusUserActivityLogDetail.icdoUserActivityLogDetail.LoadData(adtrRow);

            }
        }
	}
}
