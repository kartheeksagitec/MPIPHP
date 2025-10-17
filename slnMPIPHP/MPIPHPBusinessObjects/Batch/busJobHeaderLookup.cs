#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using MPIPHP.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;

#endregion

namespace MPIPHP.BusinessObjects
{
	[Serializable]
	public class busJobHeaderLookup : busJobHeaderLookupGen
	{
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            base.LoadOtherObjects(adtrRow, aobjBus);

            busJobHeader lobjJobHeader = (busJobHeader)aobjBus;

            lobjJobHeader.ibusLookupJobDetail = new busJobDetail();
            lobjJobHeader.ibusLookupJobDetail.icdoJobDetail = new cdoJobDetail();
            lobjJobHeader.ibusLookupJobDetail.icdoJobDetail.LoadData(adtrRow);
            /*
            // Suresh - Fix for PIR 11 in Common Issues
            // Need to populate the actual job detail status_id and status_value, instead the LoadData method updates the job header status id & value
            lobjJobHeader.ibusLookupJobDetail.icdoJobDetail.status_id = Convert.ToInt32(adtrRow["jobdetail_status_id"]);
            lobjJobHeader.ibusLookupJobDetail.icdoJobDetail.status_value = adtrRow["jobdetail_status_value"].ToString();
            lobjJobHeader.ibusLookupJobDetail.icdoJobDetail.status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(lobjJobHeader.ibusLookupJobDetail.icdoJobDetail.status_id,
                                                                                    lobjJobHeader.ibusLookupJobDetail.icdoJobDetail.status_value);
             */
        }
	}
}
