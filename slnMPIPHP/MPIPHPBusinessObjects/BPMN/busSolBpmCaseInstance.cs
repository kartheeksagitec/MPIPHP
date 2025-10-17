using MPIPHP.BusinessObjects;
using MPIPHP.CustomDataObjects;
using NeoBase.BPM;
using NeoBase.Common;
using NeoSpin.DataObjects;
using Sagitec.Bpm;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busBpmCaseInstance:
    /// Inherited from busBpmCaseInstanceGen, the class is used to customize the business object busBpmCaseInstanceGen.
    /// </summary>
    [Serializable]
    public class busSolBpmCaseInstance : busNeobaseBpmCaseInstance //busBpmCaseInstance
    {

        /// <summary>
        /// This method is used Load BPM case Instance Grid view through collection.
        /// </summary>
        public virtual void LoadBpmCaseInstances(DataTable adtbSearchResult)
        {
            iclbBpmCaseInstance = GetCollection<busNeobaseBpmCaseInstance>(adtbSearchResult, "icdoBpmCaseInstance");
        }
        public bool IsResumeActivityInstanceVisible()
        {
            if (this.iclbBpmProcessInstance.Count > 0)
            {
                int ActivityId = this.iclbBpmProcessInstance[0].iclbBpmActivityInstance
                                 .Where(a => a.ibusBpmActivity.icdoBpmActivity.activity_type_value == "IMCE" &&
                                  a.icdoBpmActivityInstance.status_value == "SUSP").Select(a => a.ibusBpmActivity.icdoBpmActivity.activity_id).FirstOrDefault();
                if (ActivityId > 0)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// This method is used Load Other Objects which are to be used for BPM case Instance.
        /// </summary>
        /// <param name="adtrRow">object of DataRow</param>
        /// <param name="abusBusBase">object of busBase. Inherited class can be accessed.</param>
        protected override void LoadOtherObjects(DataRow adtrRow, busBase abusBusBase)
        {
            if (abusBusBase is busNeobaseBpmCaseInstance)
            {
                busNeobaseBpmCaseInstance lbusNeobaseBpmCaseInstance = (busNeobaseBpmCaseInstance)abusBusBase;
                busPerson lbusPerson = new busPerson() { icdoPerson = new cdoPerson() };
                lbusPerson.icdoPerson.LoadData(adtrRow);
                lbusNeobaseBpmCaseInstance.ibusPerson = lbusPerson;

                busOrganization lbusOrganization = new busOrganization() { icdoOrganization = new cdoOrganization() };
                lbusOrganization.icdoOrganization.LoadData(adtrRow);
                lbusNeobaseBpmCaseInstance.ibusOrganization = lbusOrganization;
                lbusNeobaseBpmCaseInstance.ibusBpmCase = new busBpmCase();
                lbusNeobaseBpmCaseInstance.ibusBpmCase.FindByPrimaryKey(lbusNeobaseBpmCaseInstance.icdoBpmCaseInstance.case_id);

                lbusNeobaseBpmCaseInstance.DisplayStatus = lbusNeobaseBpmCaseInstance.icdoBpmCaseInstance.status_description;
                if (lbusNeobaseBpmCaseInstance.DisplayStatus == busNeoBaseConstants.BPM.Status.INPROGRESS)
                {
                    string lstrActivityInstanceStatusVal = adtrRow.CheckAndGetValue<string>(busNeoBaseConstants.BPM.ACTIVITY_STATUS_VALUE);
                    if ( lstrActivityInstanceStatusVal == "FAIL")
                    {
                        lbusNeobaseBpmCaseInstance.DisplayStatus = busNeoBaseConstants.BPM.Status.STATUS_ON_ACTIVITY_FAILED;
                    }
                }
            }
            base.LoadOtherObjects(adtrRow, abusBusBase);
        }
        public void SolResumeActivityInstance()
        {

            //create request
            busSolBpmRequest lbusBpmRequest = new busSolBpmRequest() { icdoBpmRequest = new doBpmRequest() };

            if (this.ibusBpmCase.icdoBpmCase.name == "sbpReturnToWork")
            {
                if (this.iclbBpmProcessInstance.Count > 0)
                {
                    int ActivityId = this.iclbBpmProcessInstance[0].iclbBpmActivityInstance.Where(a =>
                                                                                a.ibusBpmActivity.icdoBpmActivity.activity_type_value == "IMCE" &&
                                                                                a.icdoBpmActivityInstance.status_value == "SUSP").Select(a => a.ibusBpmActivity.icdoBpmActivity.activity_id).FirstOrDefault();
                    if (ActivityId > 0)
                    {
                        lbusBpmRequest.icdoBpmRequest.process_id = this.icdoBpmCaseInstance.case_id;
                        lbusBpmRequest.icdoBpmRequest.reference_id = 0;
                        lbusBpmRequest.icdoBpmRequest.person_id = this.icdoBpmCaseInstance.person_id;
                        lbusBpmRequest.icdoBpmRequest.source_id = busConstant.ReturnToWorkRequest.DOC_TYPE_ID;
                        lbusBpmRequest.icdoBpmRequest.source_value = busConstant.ReturnToWorkRequest.SOURCE_INDEXING;
                        lbusBpmRequest.icdoBpmRequest.status_id = busConstant.ReturnToWorkRequest.BPM_REQUEST_STATUS_ID;
                        lbusBpmRequest.icdoBpmRequest.status_value = busConstant.ReturnToWorkRequest.STATUS_UNPROCESSED;
                        lbusBpmRequest.icdoBpmRequest.case_instance_id = this.icdoBpmCaseInstance.case_instance_id;
                        lbusBpmRequest.icdoBpmRequest.doc_type = busConstant.ReturnToWorkRequest.DOC_TYPE;
                        lbusBpmRequest.icdoBpmRequest.doc_class = busConstant.ReturnToWorkRequest.DOC_TYPE;
                        lbusBpmRequest.icdoBpmRequest.ecm_guid = Convert.ToString(Guid.Empty);
                        lbusBpmRequest.icdoBpmRequest.Insert();
                    }
                }

            }
           else if (this.ibusBpmCase.icdoBpmCase.name == busConstant.PersonAccountMaintenance.SERVICE_RETIREMENT_BPM)
            {
                if (this.iclbBpmProcessInstance.Count > 0)
                {
                    var ActivityName = this.iclbBpmProcessInstance[0].iclbBpmActivityInstance.Where(a =>
                                                                                a.ibusBpmActivity.icdoBpmActivity.activity_type_value == "IMCE" &&
                                                                                a.icdoBpmActivityInstance.status_value == "SUSP").Select(a => a.ibusBpmActivity.icdoBpmActivity.name).FirstOrDefault();

                    if (ActivityName == busConstant.PersonAccountMaintenance.DOCUMENT_RECEIVED_ACTIVITY)
                    {
                        lbusBpmRequest.icdoBpmRequest.process_id = this.icdoBpmCaseInstance.case_id;
                        lbusBpmRequest.icdoBpmRequest.person_id = this.icdoBpmCaseInstance.person_id;
                        lbusBpmRequest.icdoBpmRequest.reference_id = this.icdoBpmCaseInstance.reference_id;
                        lbusBpmRequest.icdoBpmRequest.source_id = busConstant.ReturnToWorkRequest.DOC_TYPE_ID;
                        lbusBpmRequest.icdoBpmRequest.source_value = busConstant.ReturnToWorkRequest.SOURCE_INDEXING;
                        lbusBpmRequest.icdoBpmRequest.status_id = busConstant.ReturnToWorkRequest.DOC_TYPE_ID;
                        lbusBpmRequest.icdoBpmRequest.status_value = busConstant.ReturnToWorkRequest.STATUS_UNPROCESSED;
                        lbusBpmRequest.icdoBpmRequest.case_instance_id = this.icdoBpmCaseInstance.case_instance_id;
                        lbusBpmRequest.icdoBpmRequest.doc_type = "SR-Wait For Application Form";
                        lbusBpmRequest.icdoBpmRequest.doc_class = "SR-Wait For Application Form";
                        lbusBpmRequest.icdoBpmRequest.ecm_guid = Convert.ToString(Guid.Empty);
                        lbusBpmRequest.icdoBpmRequest.Insert();
                    }
                    else if (ActivityName== busConstant.PersonAccountMaintenance.ELECTION_PACKET_RECEIVED_ACTIVITY)
                    {
                        lbusBpmRequest.icdoBpmRequest.process_id = this.icdoBpmCaseInstance.case_id;
                        lbusBpmRequest.icdoBpmRequest.reference_id = this.icdoBpmCaseInstance.reference_id;
                        lbusBpmRequest.icdoBpmRequest.person_id = this.icdoBpmCaseInstance.person_id;
                        lbusBpmRequest.icdoBpmRequest.source_id = busConstant.ReturnToWorkRequest.DOC_TYPE_ID;
                        lbusBpmRequest.icdoBpmRequest.source_value = busConstant.ReturnToWorkRequest.SOURCE_INDEXING;
                        lbusBpmRequest.icdoBpmRequest.status_id = busConstant.ReturnToWorkRequest.DOC_TYPE_ID;
                        lbusBpmRequest.icdoBpmRequest.status_value = busConstant.ReturnToWorkRequest.STATUS_UNPROCESSED;
                        lbusBpmRequest.icdoBpmRequest.case_instance_id = this.icdoBpmCaseInstance.case_instance_id;
                        lbusBpmRequest.icdoBpmRequest.doc_type = "SR-Wait for Election Packet";
                        lbusBpmRequest.icdoBpmRequest.doc_class = "SR-Wait for Election Packet";
                        lbusBpmRequest.icdoBpmRequest.ecm_guid = Convert.ToString(Guid.Empty);
                        lbusBpmRequest.icdoBpmRequest.Insert();
                    }
                    else if (ActivityName == busConstant.PersonAccountMaintenance.CANCELLATION_NOTICE_RECEIVED_NOTICE)
                    {
                        lbusBpmRequest.icdoBpmRequest.process_id = this.icdoBpmCaseInstance.case_id;
                        lbusBpmRequest.icdoBpmRequest.reference_id = this.icdoBpmCaseInstance.reference_id;
                        lbusBpmRequest.icdoBpmRequest.person_id = this.icdoBpmCaseInstance.person_id;
                        lbusBpmRequest.icdoBpmRequest.source_id = busConstant.ReturnToWorkRequest.DOC_TYPE_ID;
                        lbusBpmRequest.icdoBpmRequest.source_value = busConstant.ReturnToWorkRequest.SOURCE_INDEXING;
                        lbusBpmRequest.icdoBpmRequest.status_id = busConstant.ReturnToWorkRequest.DOC_TYPE_ID;
                        lbusBpmRequest.icdoBpmRequest.status_value = busConstant.ReturnToWorkRequest.STATUS_UNPROCESSED;
                        lbusBpmRequest.icdoBpmRequest.case_instance_id = this.icdoBpmCaseInstance.case_instance_id;
                        lbusBpmRequest.icdoBpmRequest.doc_type = "SR-Cancellation Notice Received";
                        lbusBpmRequest.icdoBpmRequest.doc_class = "SR-Cancellation Notice Received";
                        lbusBpmRequest.icdoBpmRequest.ecm_guid = Convert.ToString(Guid.Empty);
                        lbusBpmRequest.icdoBpmRequest.Insert();
                    }
                }
            }

            if (this.ibusBpmCase.icdoBpmCase.name == busConstant.PersonAccountMaintenance.CANCEL_SERVICE_RETIREMENT_BPM)
            {
                if (this.iclbBpmProcessInstance.Count > 0)
                {
                    var ActivityName = this.iclbBpmProcessInstance[0].iclbBpmActivityInstance.Where(a =>
                                                                                a.ibusBpmActivity.icdoBpmActivity.activity_type_value == "IMCE" &&
                                                                                a.icdoBpmActivityInstance.status_value == "SUSP").Select(a => a.ibusBpmActivity.icdoBpmActivity.name).FirstOrDefault();

                    if (ActivityName == busConstant.PersonAccountMaintenance.CANCELLATION_NOTICE_RECEIVED_NOTICE)
                    {
                        lbusBpmRequest.icdoBpmRequest.process_id = this.icdoBpmCaseInstance.case_id;
                        lbusBpmRequest.icdoBpmRequest.reference_id = this.icdoBpmCaseInstance.reference_id;
                        lbusBpmRequest.icdoBpmRequest.person_id = this.icdoBpmCaseInstance.person_id;
                        lbusBpmRequest.icdoBpmRequest.source_id = busConstant.ReturnToWorkRequest.DOC_TYPE_ID;
                        lbusBpmRequest.icdoBpmRequest.source_value = busConstant.ReturnToWorkRequest.SOURCE_INDEXING;
                        lbusBpmRequest.icdoBpmRequest.status_id = busConstant.ReturnToWorkRequest.DOC_TYPE_ID;
                        lbusBpmRequest.icdoBpmRequest.status_value = busConstant.ReturnToWorkRequest.STATUS_UNPROCESSED;
                        lbusBpmRequest.icdoBpmRequest.case_instance_id = this.icdoBpmCaseInstance.case_instance_id;
                        lbusBpmRequest.icdoBpmRequest.doc_type = "SR-Cancellation Notice Received";
                        lbusBpmRequest.icdoBpmRequest.doc_class = "SR-Cancellation Notice Received";
                        lbusBpmRequest.icdoBpmRequest.ecm_guid = Convert.ToString(Guid.Empty);
                        lbusBpmRequest.icdoBpmRequest.Insert();
                    }
                }
            }


        }
    }
}
