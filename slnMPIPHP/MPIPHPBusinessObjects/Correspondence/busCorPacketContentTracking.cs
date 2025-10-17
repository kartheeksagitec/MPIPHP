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
	/// Class MPIPHP.busCorPacketContentTracking:
	/// Inherited from busCorPacketContentTrackingGen, the class is used to customize the business object busCorPacketContentTrackingGen.
	/// </summary>
	[Serializable]
	public class busCorPacketContentTracking : busCorPacketContentTrackingGen
	{

        public Collection<busCorPacketContentTrackingHistory> iclbCorPacketContentTrackingHistory { get; set; }

        public override bool FindCorPacketContentTracking(int aintTrackingId)
        {
            bool lblnResult = false;
            if (icdoCorPacketContentTracking == null)
            {
                icdoCorPacketContentTracking = new cdoCorPacketContentTracking();
            }

            DataTable ldtbPacketContentTrackingDetails = busBase.Select("cdoCorPacketContentTracking.GetCorPacketContentTracking", new object[1] { aintTrackingId });
            if (ldtbPacketContentTrackingDetails != null && ldtbPacketContentTrackingDetails.Rows.Count > 0)
            {
                icdoCorPacketContentTracking.LoadData(ldtbPacketContentTrackingDetails.Rows[0]);
                LoadCorPacketContentTrackingHistory();
                lblnResult = true;
            }
          
            return lblnResult;
        }

        public virtual void LoadCorPacketContentTrackingHistory()
        {
            DataTable ldtbList = Select<cdoCorPacketContentTrackingHistory>(
                new string[1] { enmCorPacketContentTrackingHistory.cor_packet_content_tracking_id.ToString().ToUpper() },
                new object[1] { icdoCorPacketContentTracking.cor_packet_content_tracking_id }, null, null);
            iclbCorPacketContentTrackingHistory = GetCollection<busCorPacketContentTrackingHistory>(ldtbList, "icdoCorPacketContentTrackingHistory");
        }
    }
}
