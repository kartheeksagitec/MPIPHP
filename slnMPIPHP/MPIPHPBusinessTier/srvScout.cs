#region Using directives

using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using MPIPHP.BusinessObjects;
using Sagitec.Common;
using MPIPHP.CustomDataObjects;
using Sagitec.DBUtility;
using System.Collections.ObjectModel;
using MPIPHP.Common;

#endregion

namespace MPIPHP.BusinessTier
{
    public class srvScout : srvMPIPHP
	{
		public srvScout()
		{
			//
			// TODO: Add constructor logic here
			//
		}
        protected override IDbConnection GetDBConnection()
        {
            return DBFunction.GetDBConnection("scout");
        }

        public busPir FindPir(int aintPirId)
        {
            busPir lobjPir = new busPir();
            if (lobjPir.FindPir(aintPirId))
            {
                lobjPir.LoadPirHistory();
                lobjPir.LoadPirAttachment();
                lobjPir.LoadAssignedTo();
                lobjPir.LoadReportedBy();
                lobjPir.LoadPirEffortHours(); //PIR Scout/Effort Hours Implementation
            }
            return lobjPir;
        }

        public void InsertAttachment(int aintPIRID, byte[] aobjAttachmentData, string astrMimeType, string astrFileName, string astrUserID)
        {
            busPirAttachment lobjPirAttachment = new busPirAttachment();
            lobjPirAttachment.InsertAttachment(aintPIRID, aobjAttachmentData, astrMimeType, astrFileName, astrUserID);

        }

        public FileDownloadContainer DownloadPirAttachment(int aintpirattachmentid)
        {
            FileDownloadContainer result = null;
            busPirAttachment lobjPirAttachment = FindPirAttachment(aintpirattachmentid);
            if (lobjPirAttachment != null)
            {
                result = new FileDownloadContainer(lobjPirAttachment.icdoPirAttachment.attachment_file_name, lobjPirAttachment.icdoPirAttachment.attachment_mime_type, lobjPirAttachment.icdoPirAttachment.attachment_content);
            }
            return result;
        }

        // F/W Upgrade : Code Conversion for btnDownloadFile_Click method.
        public ArrayList GetDownloadPirAttachment(int aintpirattachmentid)
        {
            FileDownloadContainer result = null;
            result = DownloadPirAttachment(aintpirattachmentid);
            ArrayList larrFileDetails = new ArrayList();
            larrFileDetails.Add(result.iFileName);
            larrFileDetails.Add(result.iFileContent);
            return larrFileDetails;
        }

        public busPir NewPir()
		{
			busPir lobjPir = new busPir();
			lobjPir.icdoPir = new cdoPir();
            //PIR Scout/Effort Hours Implementation
            lobjPir.ibusPirEffortsHours = new busPirEffortsHours { icdoPirEffortsHours = new cdoPirEffortsHours() };
            lobjPir.icdoPir.status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(40, lobjPir.icdoPir.status_value);
            return lobjPir;
		}

		public busPirLookup LoadPirs(DataTable adtbSearchResult)
		{
			busPirLookup lobjPirLookup = new busPirLookup();
			lobjPirLookup.LoadPirs(adtbSearchResult);
			return lobjPirLookup;
		}

		public busPirHistory FindPirHistory(int aintpirhistoryid)
		{
			busPirHistory lobjPirHistory = new busPirHistory();
			if (lobjPirHistory.FindPirHistory(aintpirhistoryid))
			{
			}

			return lobjPirHistory;
		}

		public busPirAttachment FindPirAttachment(int aintpirattachmentid)
		{
			busPirAttachment lobjPirAttachment = new busPirAttachment();
			if (lobjPirAttachment.FindPirAttachment(aintpirattachmentid))
			{
			}

			return lobjPirAttachment;
		}
	}
}
