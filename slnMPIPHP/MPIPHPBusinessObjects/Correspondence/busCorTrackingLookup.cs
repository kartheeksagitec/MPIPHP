#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.CustomDataObjects;
using MPIPHP.BusinessObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using MPIPHP.CustomDataObjects;


#endregion

namespace MPIPHP.BusinessObjects
{
	[Serializable]
	public class busCorTrackingLookup : busMainBase
	{

		public Collection<busCorTracking> iclbLookupResult {get;set;}


		public void LoadCorTracking(DataTable adtbSearchResult)
		{
            iclbLookupResult = GetCollection<busCorTracking>(adtbSearchResult, "icdoCorTracking");

		}

		protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
		{
			cdoCorTemplates lobjCorTemplates = new cdoCorTemplates();
			((busCorTracking)aobjBus).ibusCorTemplates = new busCorTemplates();
            ((busCorTracking)aobjBus).ibusCorTemplates.icdoCorTemplates = new cdoCorTemplates();
			((busCorTracking)aobjBus).ibusCorTemplates.icdoCorTemplates.LoadData(adtrRow);
			lobjCorTemplates.LoadData(adtrRow);
            ((busCorTracking)aobjBus).icdoCorTracking.istrMpiPersonID = Convert.ToString(adtrRow["istrMpiPersonID"]);

            ((busCorTracking)aobjBus).ibusCorPacketContentTracking = new busCorPacketContentTracking { icdoCorPacketContentTracking = new cdoCorPacketContentTracking() };
            ((busCorTracking)aobjBus).ibusCorPacketContentTracking.FindCorPacketContentTracking(((busCorTracking)aobjBus).icdoCorTracking.tracking_id);

        }
	}
}
