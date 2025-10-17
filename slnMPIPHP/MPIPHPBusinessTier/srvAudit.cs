#region Using directives

using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using MPIPHP.BusinessObjects;

#endregion

namespace MPIPHP.BusinessTier
{
	public class srvAudit : srvMPIPHP
	{
		public srvAudit()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		//FM upgrade: 6.0.1.2 changes
		//public busAudit FindAuditLog(int aintAuditLogID)
		//{
		//	busAudit lobjAudit = new busAudit();
		//	if (lobjAudit.FindAuditLog(aintAuditLogID))
		//	{
		//		lobjAudit.LoadAuditLogDetail();
		//	}
		//	return lobjAudit;
		//}


		//public busAuditLookup GetAuditDetails(int aintPrimaryKey, string astrFormName)
		//{
		//	busAuditLookup lobjAuditLookup = new busAuditLookup();
		//	lobjAuditLookup.GetAuditDetails(aintPrimaryKey, astrFormName);
		//	return lobjAuditLookup;
		//}

		//      public busAuditLookup LoadSearchResult(DataTable adtbSearchResult)
		//      {
		//          busAuditLookup lobjAuditLookup = new busAuditLookup();
		//          lobjAuditLookup.LoadSearchResult(adtbSearchResult);
		//          return lobjAuditLookup;
		//      }

        //public busMPIAudit FindMPIAudit(int aintauditlogid)
        //{
        //    busMPIAudit lobjMPIAudit = new busMPIAudit();
        //    if (lobjMPIAudit.FindAuditLog(aintauditlogid))
        //    {
        //        lobjMPIAudit.LoadAuditLogDetail();
        //    }

        //    return lobjMPIAudit;
        //}

		//public busAuditLogDetail FindAuditLogDetail(int aintauditlogdetailid)
		//{
		//    busAuditLogDetail lobjAuditLogDetail = new busAuditLogDetail();
		//    if (lobjAuditLogDetail.FindAuditLogDetail(aintauditlogdetailid))
		//    {
		//    }

		//    return lobjAuditLogDetail;
		//}

		public busMPIAuditLookup LoadbusMPIAudits(DataTable adtbSearchResult)
		{
			busMPIAuditLookup lobjbusMPIAuditLookup = new busMPIAuditLookup();
            lobjbusMPIAuditLookup.LoadSearchResult(adtbSearchResult);
			return lobjbusMPIAuditLookup;
		}

		public BusinessObjects.busFullAuditLog FindFullAuditLog(int aintAuditLogId)
		{
            BusinessObjects.busFullAuditLog lobjFullAuditLog = new BusinessObjects.busFullAuditLog();
			if (lobjFullAuditLog.FindFullAuditLog(aintAuditLogId))
			{
				lobjFullAuditLog.LoadFullAuditLogDetails();
			}

			return lobjFullAuditLog;
		}

		public busFullAuditLogDetail FindFullAuditLogDetail(int aintAuditLogDetailId)
		{
			busFullAuditLogDetail lobjFullAuditLogDetail = new busFullAuditLogDetail();
			if (lobjFullAuditLogDetail.FindFullAuditLogDetail(aintAuditLogDetailId))
			{
			}

			return lobjFullAuditLogDetail;
		}

		//public BusinessObjects.busFullAuditLog GetFullAuditLog(int aintPrimarKey, string astrFormName)
		//{
		//	BusinessObjects.busFullAuditLog lobjFullAuditLog = new BusinessObjects.busFullAuditLog();
		//	lobjFullAuditLog.GetFullAuditLog(aintPrimarKey, astrFormName);
		//	return lobjFullAuditLog;
		//}

		//Fw upgrade: PIR ID : 28660: New implementation of Audit History using audit_details
		public busMPIFullAudit GetFullAuditLog(int aintPrimarKey, string astrFormName)
        {
			busMPIFullAudit lobjMpiAudit = new busMPIFullAudit();
            lobjMpiAudit.LoadFullAuditLogs(aintPrimarKey, astrFormName);
            lobjMpiAudit.LoadPerson(aintPrimarKey);
            return lobjMpiAudit;
        }
	}
}
