#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using Sagitec.CustomDataObjects;
using System.IO;
using MPIPHP.BusinessObjects;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
using System.Linq;


#endregion

namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public class busCorTracking : busMPIPHPBase
    {
        public busCorTracking()
        {

        }

        public string short_description
        {
            get
            {
                string lstrDescription = "";
                if (ibusCorTemplates != null)
                {
                    lstrDescription = ibusCorTemplates.icdoCorTemplates.template_desc;
                }
                else
                {
                    lstrDescription = "Invalid Correspondence";
                }
                return lstrDescription;
            }
        }

        public string long_description
        {
            get
            {
                string lstrDescription = "";
                if (ibusCorTemplates != null)
                {
                    lstrDescription = ibusCorTemplates.icdoCorTemplates.template_desc +
                        " is in " + icdoCorTracking.cor_status_description + " status and generated " +
                        " on " + HelperFunction.FormatData(icdoCorTracking.generated_date.ToString(), "{0:d}");
                }
                else
                {
                    lstrDescription = "Invalid Correspondence";
                }
                return lstrDescription;
            }
        }

        public cdoCorTracking icdoCorTracking { get; set; }

        public busCorTemplates ibusCorTemplates { get; set; }

        public busCorPacketContentTracking ibusCorPacketContentTracking { get; set; }

        public busPerson ibusPerson { get; set; }

        public Collection<busCorTracking> iclbCorTracking { get; set; }

        /*private busStakeholder _ibusStakeholder;
        public busStakeholder ibusStakeholder
		{
			get
			{
				return _ibusStakeholder;
			}

			set
			{
				_ibusStakeholder = value;
			}
		}*/

        public bool FindCorTracking(int ainttrackingid)
        {

            bool lblnResult = false;
            if (icdoCorTracking == null)
            {
                icdoCorTracking = new cdoCorTracking();
            }
            if (icdoCorTracking.SelectRow(new object[1] { ainttrackingid }))
            {
                lblnResult = true;
            }
            return lblnResult;

        }

        public void LoadCorTemplates()
        {
            if (ibusCorTemplates == null)
            {
                ibusCorTemplates = new busCorTemplates();
            }
            ibusCorTemplates.FindCorTemplates(icdoCorTracking.template_id);
        }

        public void LoadCorPacketContentTracking()
        {
            if (ibusCorPacketContentTracking == null)
            {
                ibusCorPacketContentTracking = new busCorPacketContentTracking();
            }
            ibusCorPacketContentTracking.FindCorPacketContentTracking(icdoCorTracking.template_id);
        }


        public string istrFileName
        {
            get
            {
                return icdoCorTracking.tracking_id.ToString().PadLeft(10, '0');
            }
        }

        public string istrWordFileName
        {
            get
            {
                return ibusCorTemplates.icdoCorTemplates.template_name + "-" + istrFileName + ".docx";
            }
        }

        public string istrPDFFileName
        {
            get
            {
                return icdoCorTracking.tracking_id.ToString() + ".pdf";
            }
        }

        public void RemoveFile()
        {
            string lstrFileName = iobjPassInfo.isrvDBCache.GetPathInfo("CorrGenr") + istrWordFileName;
            File.Delete(lstrFileName);
        }

        public Collection<busCorTracking> LoadOtherCorrespondences()
        {
            DataTable ldtbList = Select("cdoCorPacketContent.GetCorTrackingDetails", new object[1] { icdoCorTracking.person_id });
            iclbCorTracking = GetCollection<busCorTracking>(ldtbList, "icdoCorTracking");

            if (iclbCorTracking != null && iclbCorTracking.Count > 0)
            {
                if (iclbCorTracking != null && iclbCorTracking.Count > 0 && iclbCorTracking.Where(t => t.icdoCorTracking.tracking_id == icdoCorTracking.tracking_id).Count() > 0)
                {
                    iclbCorTracking.Remove(iclbCorTracking.Where(t => t.icdoCorTracking.tracking_id == icdoCorTracking.tracking_id).FirstOrDefault());
                }

                foreach (busCorTracking lbusCorTracking in iclbCorTracking)
                {
                    lbusCorTracking.ibusCorTemplates = new busCorTemplates { icdoCorTemplates = new cdoCorTemplates() };
                    lbusCorTracking.ibusCorTemplates.FindCorTemplates(lbusCorTracking.icdoCorTracking.template_id);

                    lbusCorTracking.ibusCorPacketContentTracking = new busCorPacketContentTracking { icdoCorPacketContentTracking = new cdoCorPacketContentTracking() };
                    lbusCorTracking.ibusCorPacketContentTracking.FindCorPacketContentTracking(lbusCorTracking.icdoCorTracking.tracking_id);

                    //if (lbusCorTracking.icdoCorTracking.packet_status_value.IsNotNullOrEmpty())
                    //    lbusCorTracking.icdoCorTracking.packet_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(7094, lbusCorTracking.icdoCorTracking.packet_status_value);
                }
            }

            return iclbCorTracking;
        }

        public void LoadPerson()
        {
            ibusPerson = new busPerson { icdoPerson = new CustomDataObjects.cdoPerson() };
            if (ibusPerson.FindPerson(icdoCorTracking.person_id))
            { }
        }

        public override void BeforePersistChanges()
        {
            base.BeforePersistChanges();

            if (ibusCorPacketContentTracking.icdoCorPacketContentTracking.received_date != DateTime.MinValue
                    && Convert.ToString(ibusCorPacketContentTracking.icdoCorPacketContentTracking.ihstOldValues[enmCorPacketContentTracking.received_date.ToString().ToUpper()]).IsNullOrEmpty() &&
                    ibusCorPacketContentTracking.icdoCorPacketContentTracking.received_date != Convert.ToDateTime(ibusCorPacketContentTracking.icdoCorPacketContentTracking.ihstOldValues[enmCorPacketContentTracking.received_date.ToString().ToUpper()]))
            {
                ibusCorPacketContentTracking.icdoCorPacketContentTracking.received_by = iobjPassInfo.istrUserID;
            }

            if (ibusCorPacketContentTracking.icdoCorPacketContentTracking.received_date == DateTime.MinValue)
            {
                ibusCorPacketContentTracking.icdoCorPacketContentTracking.received_by = string.Empty;
            }

            if (ibusCorPacketContentTracking.icdoCorPacketContentTracking.packet_status_value.IsNotNullOrEmpty()
                   && Convert.ToString(ibusCorPacketContentTracking.icdoCorPacketContentTracking.ihstOldValues[enmCorPacketContentTracking.packet_status_value.ToString()]).IsNotNullOrEmpty() &&
                   ibusCorPacketContentTracking.icdoCorPacketContentTracking.packet_status_value != Convert.ToString(ibusCorPacketContentTracking.icdoCorPacketContentTracking.ihstOldValues[enmCorPacketContentTracking.packet_status_value.ToString()]))
            {
                if (ibusCorPacketContentTracking.icdoCorPacketContentTracking.packet_status_value == busConstant.MAILED_OUT_STATUS)
                {
                    ibusCorPacketContentTracking.icdoCorPacketContentTracking.mailed_date = DateTime.Now;
                    ibusCorPacketContentTracking.icdoCorPacketContentTracking.mailed_by = iobjPassInfo.istrUserID;
                    //WI 23234 Secure Document submission
                    SetDocumentUploadFlag();
                }
                else if (ibusCorPacketContentTracking.icdoCorPacketContentTracking.packet_status_value == "CNCL")
                {
                    //WI 23234 Secure Document submission
                    DataTable ldtbList = Select("cdoCorPacketContent.GetPacketTrackingList", new object[2] { icdoCorTracking.person_id, icdoCorTracking.tracking_id });
                    if (ldtbList != null && ldtbList.Rows.Count == 0)
                    {
                        RemoveDocumentUploadFlag();
                    }
                }
                else if (ibusCorPacketContentTracking.icdoCorPacketContentTracking.packet_status_value == "PEND")
                {
                    ibusCorPacketContentTracking.icdoCorPacketContentTracking.mailed_date = DateTime.MinValue;
                    ibusCorPacketContentTracking.icdoCorPacketContentTracking.mailed_by = string.Empty;
                }
            }
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();

            busCorPacketContentTrackingHistory lbusCorPacketContentTrackingHistory = new busCorPacketContentTrackingHistory { icdoCorPacketContentTrackingHistory = new cdoCorPacketContentTrackingHistory() };
            lbusCorPacketContentTrackingHistory.icdoCorPacketContentTrackingHistory.cor_packet_content_tracking_id = ibusCorPacketContentTracking.icdoCorPacketContentTracking.cor_packet_content_tracking_id;
            lbusCorPacketContentTrackingHistory.icdoCorPacketContentTrackingHistory.mailed_by = ibusCorPacketContentTracking.icdoCorPacketContentTracking.mailed_by;
            lbusCorPacketContentTrackingHistory.icdoCorPacketContentTrackingHistory.mailed_date = ibusCorPacketContentTracking.icdoCorPacketContentTracking.mailed_date;
            lbusCorPacketContentTrackingHistory.icdoCorPacketContentTrackingHistory.received_by = ibusCorPacketContentTracking.icdoCorPacketContentTracking.received_by;
            lbusCorPacketContentTrackingHistory.icdoCorPacketContentTrackingHistory.received_date = ibusCorPacketContentTracking.icdoCorPacketContentTracking.received_date;
            lbusCorPacketContentTrackingHistory.icdoCorPacketContentTrackingHistory.packet_status_value = ibusCorPacketContentTracking.icdoCorPacketContentTracking.packet_status_value;
            lbusCorPacketContentTrackingHistory.icdoCorPacketContentTrackingHistory.notes = ibusCorPacketContentTracking.icdoCorPacketContentTracking.notes;
            lbusCorPacketContentTrackingHistory.icdoCorPacketContentTrackingHistory.Insert();

            ibusCorPacketContentTracking.LoadCorPacketContentTrackingHistory();
        }

        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            utlError lobjError = null;
            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();

            if(ibusCorPacketContentTracking.icdoCorPacketContentTracking.packet_status_value == busConstant.PAYEE_ACCOUNT_STATUS_RECEIVING 
                && (ibusCorPacketContentTracking.icdoCorPacketContentTracking.received_date == DateTime.MinValue ||
                ibusCorPacketContentTracking.icdoCorPacketContentTracking.received_date > DateTime.Today))
            {
                lobjError = AddError(0, "Please enter a received date of today or earlier.");
                this.iarrErrors.Add(lobjError);
            }

            if (ibusCorPacketContentTracking.icdoCorPacketContentTracking.received_date != DateTime.MinValue && ibusCorPacketContentTracking.icdoCorPacketContentTracking.received_date.Date < ibusCorPacketContentTracking.icdoCorPacketContentTracking.mailed_date.Date)
            {
                lobjError = AddError(0, "Received Date cannot be before Mailed Date.");
                this.iarrErrors.Add(lobjError);
            }
            
            if (ibusCorPacketContentTracking.icdoCorPacketContentTracking.packet_status_value == busConstant.PAYEE_ACCOUNT_STATUS_COMPLETED
            && ibusCorPacketContentTracking.icdoCorPacketContentTracking.received_date == DateTime.MinValue)
            {
                lobjError = AddError(0, "Please enter received date.");
                this.iarrErrors.Add(lobjError);
            }

            if (ibusCorPacketContentTracking.icdoCorPacketContentTracking.mailed_date == DateTime.MinValue && ibusCorPacketContentTracking.icdoCorPacketContentTracking.packet_status_value != busConstant.PACKET_STATUS_MAILED_OUT
                    && ibusCorPacketContentTracking.icdoCorPacketContentTracking.received_date != DateTime.MinValue)
            {
                lobjError = AddError(0, "Please enter mailed date.");
                this.iarrErrors.Add(lobjError);
            }

            base.ValidateHardErrors(aenmPageMode);
        }

        //WI 23234 Secure Document submission
        private void SetDocumentUploadFlag()
        {
            int lintPersonId = icdoCorTracking.person_id;
            busPerson lbusPerson = new busPerson { icdoPerson = new cdoPerson() };
            DataTable ldtPerson;

            //ldtPerson = Select("cdoPerson.SetDocumentUploadFlag", new object[1] { lintPersonId });
            //if (ldtPerson != null && ldtPerson.Rows.Count > 0)
            //{
            //    int intRecordUpdated = Convert.ToInt32(ldtPerson.Rows[0]["RECORDS_UPDATED"]);
            //    if(intRecordUpdated == 1) { 
            //        //Call webapi to trigger email.
                                            
            //    }
            //}
            //Other option
            ldtPerson = Select("cdoPerson.GetPersonDocumentUploadFlag", new object[1] { lintPersonId });
            if (ldtPerson != null && ldtPerson.Rows.Count > 0)
            {
                if(Convert.ToString(ldtPerson.Rows[0][0]) == "N")
                {
                    if (lbusPerson.FindPerson(lintPersonId))
                    {
                        lbusPerson.icdoPerson.document_upload_flag = busConstant.FLAG_YES;
                        lbusPerson.icdoPerson.Update();
                    }
                    //Call webapi to trigger email.

                }
            }

        }
        private void RemoveDocumentUploadFlag()
        {
            int lintPersonId = icdoCorTracking.person_id;
            busPerson lbusPerson = new busPerson { icdoPerson = new cdoPerson() };
            DataTable ldtPerson;

            ldtPerson = Select("cdoPerson.GetPersonDocumentUploadFlag", new object[1] { lintPersonId });
            if (ldtPerson != null && ldtPerson.Rows.Count > 0)
            {
                if (Convert.ToString(ldtPerson.Rows[0][0]) == "Y")
                {
                    if (lbusPerson.FindPerson(lintPersonId))
                    {
                        lbusPerson.icdoPerson.document_upload_flag = busConstant.FLAG_NO;
                        lbusPerson.icdoPerson.Update();
                    }
                }
            }

        }


    }
}
