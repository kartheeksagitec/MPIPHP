#region Using directives

using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using MPIPHP.BusinessObjects;
using Sagitec.Common;


#endregion

namespace MPIPHP.BusinessTier
{
    public class srvCorrespondence : srvMPIPHP
	{
		public srvCorrespondence()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public busCorTemplates FindCorrTemplate(int ainttemplateid)
		{
			busCorTemplates lobjCorTemplates = new busCorTemplates();
			if (lobjCorTemplates.FindCorTemplates(ainttemplateid))
			{
			}

			return lobjCorTemplates;
		}

        public busCorTracking FindCorrTracking(int aintTrackingId)
        {
            busCorTracking lobjCorTracking = new busCorTracking();
            if (lobjCorTracking.FindCorTracking(aintTrackingId))
            {
                lobjCorTracking.ibusCorTemplates = new busCorTemplates { icdoCorTemplates = new CustomDataObjects.cdoCorTemplates() };
                lobjCorTracking.ibusCorTemplates.FindCorTemplates(lobjCorTracking.icdoCorTracking.template_id);

                lobjCorTracking.ibusCorPacketContentTracking = new busCorPacketContentTracking { icdoCorPacketContentTracking = new CustomDataObjects.cdoCorPacketContentTracking() };
                lobjCorTracking.ibusCorPacketContentTracking.FindCorPacketContentTracking(aintTrackingId);

                lobjCorTracking.LoadOtherCorrespondences();
                lobjCorTracking.LoadPerson();
            }            
            return lobjCorTracking;
        }


        public busCorTemplatesLookup LoadCorrTemplates(DataTable adtbSearchResult)
		{
			busCorTemplatesLookup lobjCorTemplatesLookup = new busCorTemplatesLookup();
			lobjCorTemplatesLookup.LoadCorTemplates(adtbSearchResult);
			return lobjCorTemplatesLookup;
		}

		public busCorTrackingLookup LoadCorTracking(DataTable adtbSearchResult)
		{
			busCorTrackingLookup lobjCorTrackingLookup = new busCorTrackingLookup();
			lobjCorTrackingLookup.LoadCorTracking(adtbSearchResult);
			return lobjCorTrackingLookup;
		}

        public busPendingRetirementPacketStatusLookup LoadPendingRetirementPacketStatus(DataTable adtbSearchResult)
        {
            busPendingRetirementPacketStatusLookup lbusPendingRetirementPacketStatusLookup = new busPendingRetirementPacketStatusLookup();
            lbusPendingRetirementPacketStatusLookup.LoadCorPacketContentTrackings(adtbSearchResult);
            return lbusPendingRetirementPacketStatusLookup;
        }

        public busPendingPaymentCycleLookup LoadPendingPaymentCycleStatus(DataTable adtbSearchResult)
        {
            busPendingPaymentCycleLookup lbusPendingPaymentCycleLookup = new busPendingPaymentCycleLookup();
            lbusPendingPaymentCycleLookup.LoadCorPacketContentTrackings(adtbSearchResult);
            return lbusPendingPaymentCycleLookup;
        }

    }
}
