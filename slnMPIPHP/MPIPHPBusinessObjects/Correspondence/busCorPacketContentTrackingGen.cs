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
using MPIPHP.BusinessObjects;
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.busCorPacketContentTrackingGen:
    /// Inherited from busBase, used to create new business object for main table cdoCorPacketContentTracking and its children table. 
    /// </summary>
	[Serializable]
	public class busCorPacketContentTrackingGen : busMPIPHPBase
    {
        /// <summary>
        /// Constructor for MPIPHP.busCorPacketContentTrackingGen
        /// </summary>
		public busCorPacketContentTrackingGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busCorPacketContentTrackingGen.
        /// </summary>
		public cdoCorPacketContentTracking icdoCorPacketContentTracking { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busCorPacketContent.
        /// </summary>
		public busCorPacketContent ibusCorPacketContent { get; set; }




        /// <summary>
        /// MPIPHP.busCorPacketContentTrackingGen.FindCorPacketContentTracking():
        /// Finds a particular record from cdoCorPacketContentTracking with its primary key. 
        /// </summary>
        /// <param name="a">A primary key value of type  of cdoCorPacketContentTracking on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindCorPacketContentTracking(int aintCorPacketContentTrackinId)
		{
			bool lblnResult = false;
			if (icdoCorPacketContentTracking == null)
			{
				icdoCorPacketContentTracking = new cdoCorPacketContentTracking();
			}
			if (icdoCorPacketContentTracking.SelectRow(new object[1] { aintCorPacketContentTrackinId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// MPIPHP.busCorPacketContentTrackingGen.LoadCorPacketContent():
        /// Loads non-collection object ibusCorPacketContent of type busCorPacketContent.
        /// </summary>
		public virtual void LoadCorPacketContent()
		{
			if (ibusCorPacketContent == null)
			{
				ibusCorPacketContent = new busCorPacketContent();
			}
			ibusCorPacketContent.FindCorPacketContent(icdoCorPacketContentTracking.cor_packet_content_id);
		}

	}
}
