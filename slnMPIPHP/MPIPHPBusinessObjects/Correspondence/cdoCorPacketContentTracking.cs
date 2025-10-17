#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.CustomDataObjects
{
	/// <summary>
	/// Class MPIPHP.CustomDataObjects.cdoCorPacketContentTracking:
	/// Inherited from doCorPacketContentTracking, the class is used to customize the database object doCorPacketContentTracking.
	/// </summary>
    [Serializable]
	public class cdoCorPacketContentTracking : doCorPacketContentTracking
	{
        
        public int PACKET_TEMPLATE_ID { get; set; }

        public string MPID { get; set; }

        public string NAME { get; set; }

        public string EMAIL_ADDRESS_1 { get; set; }

        public string CELL_PHONE_NO { get; set; }

        public string TEMPLATE_DESC { get; set; }

        #region
        public string RETIREMENT_TYPE { get; set; }
        public string PLAN_CODE{ get; set; }
        public int APPID { get; set; }
        public DateTime APP_RETIREMEN_DATE { get; set; }
        public DateTime APP_CREATED_DATE { get; set; }
        public string APP_CREATED_BY { get; set; }
        public string APP_MODIFIED_BY { get; set; }
        public string APP_STATUS { get; set; }
        public DateTime CALC_CREATED_DATE { get; set; }
        public string CALC_CREATED_BY { get; set; }
        public DateTime CALC_MODIFIED_DATE { get; set; }
        public string CALC_MODIFIED_BY { get; set; }
        public DateTime PA_CREATED_DATE { get; set; }
        public string PA_CREATED_BY { get; set; }
        public DateTime PA_VERIFIED_DATE { get; set; }
        public string PA_VERIFIED_BY { get; set; }
        public string PA_STATUS { get; set; }
        public DateTime STATUS_DATE { get; set; }
        public string STATUS_BY { get; set; }

        public string PASAPRDBY { get; set; }

        public string ANALYSTNAME { get; set; }

        #endregion

        public cdoCorPacketContentTracking() : base()
		{
		}
    } 
} 
