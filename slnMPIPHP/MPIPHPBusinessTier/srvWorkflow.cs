#region Using directives

using System;
using System.Data;
using System.Collections;
using MPIPHP.BusinessObjects;
using MPIPHP.CustomDataObjects;
using Sagitec.Common;


#endregion

namespace MPIPHP.BusinessTier
{
    public class srvWorkflow : srvMPIPHP
    {
        public srvWorkflow()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public busDocument FindDocument(int Aintdocumentid)
        {
            busDocument lobjDocument = new busDocument();
            if (lobjDocument.FindDocument(Aintdocumentid))
            {
            }
            return lobjDocument;
        }

        public busDocumentLookup LoadDocuments(DataTable adtbSearchResult)
        {
            busDocumentLookup lobjDocumentLookup = new busDocumentLookup();
            lobjDocumentLookup.LoadDocuments(adtbSearchResult);
            return lobjDocumentLookup;
        }

        public busDocument NewDocument()
        {
            busDocument lobjDocument = new busDocument();
            lobjDocument.icdoDocument = new cdoDocument();
            return lobjDocument;
        }
        public busDocumentProcessCrossref FindDocumentProcessCrossref(int Aintdocumentprocesscrossrefid)
        {
            busDocumentProcessCrossref lobjDocumentProcessCrossref = new busDocumentProcessCrossref();
            if (lobjDocumentProcessCrossref.FindDocumentProcessCrossref(Aintdocumentprocesscrossrefid))
            {
                lobjDocumentProcessCrossref.LoadDocument();
                lobjDocumentProcessCrossref.istrDocumentType = lobjDocumentProcessCrossref.ibusDocument.icdoDocument.doc_type;
            }
            return lobjDocumentProcessCrossref;
        }

        public busDocumentProcessCrossRefLookup LoadDocumentProcessCrossRefs(DataTable adtbSearchResult)
        {
            busDocumentProcessCrossRefLookup lobjDocumentProcessCrossRefLookup = new busDocumentProcessCrossRefLookup();
            lobjDocumentProcessCrossRefLookup.LoadDocumentProcessCrossrefs(adtbSearchResult);
            return lobjDocumentProcessCrossRefLookup;
        }

        public busDocumentProcessCrossref NewDocumentProcessCrossref(int aintProcessID)
        {
            busDocumentProcessCrossref lobjDocumentProcessCrossref = new busDocumentProcessCrossref();
            lobjDocumentProcessCrossref.icdoDocumentProcessCrossref = new cdoDocumentProcessCrossref();
            if (aintProcessID != 0)
            {
                lobjDocumentProcessCrossref.icdoDocumentProcessCrossref.process_id = aintProcessID;
            }
            return lobjDocumentProcessCrossref;
        }

        //public busWFMimicIndexing FindWFMimicIndexing(int aintWFImageDataFilenetID)
        //{
        //    busWFMimicIndexing lobjWFMimicIndexing = new busWFMimicIndexing();
        //    if (lobjWFMimicIndexing.FindWFMimicIndexing(aintWFImageDataFilenetID))
        //    {

        //    }
        //    return lobjWFMimicIndexing;
        //}

        public busProcess FindProcess(int aintprocessid)
        {
            busProcess lobjProcess = new busProcess();
            if (lobjProcess.FindProcess(aintprocessid))
            {
                lobjProcess.LoadActivityList();
                lobjProcess.LoadDocumentProcessCrossrefs();
            }

            return lobjProcess;
        }

        public busProcessLookup LoadProcess(DataTable adtbSearchResult)
        {
            busProcessLookup lobjProcessLookup = new busProcessLookup();
            lobjProcessLookup.LoadProcess(adtbSearchResult);
            return lobjProcessLookup;
        }

        public busActivity FindActivity(int aintactivityid)
        {
            busActivity lobjActivity = new busActivity();
            if (lobjActivity.FindActivity(aintactivityid))
            {
                lobjActivity.LoadProcess();
                lobjActivity.LoadRoles();
            }

            return lobjActivity;
        }

        public busProcessInstance FindProcessInstance(int aintprocessinstanceid)
        {
            busProcessInstance lobjProcessInstance = new busProcessInstance();
            if (lobjProcessInstance.FindProcessInstance(aintprocessinstanceid))
            {
                //lobjProcessInstance.LoadContactTicket();
                //lobjProcessInstance.LoadOrganization();
                lobjProcessInstance.LoadPerson();
                lobjProcessInstance.LoadProcess();
                lobjProcessInstance.LoadWorkflowRequest();
            }

            return lobjProcessInstance;
        }

        public busActivityInstance FindActivityInstance(int aintactivityinstanceid)
        {
            busActivityInstance lobjActivityInstance = new busActivityInstance();
            if (lobjActivityInstance.FindActivityInstance(aintactivityinstanceid))
            {
                lobjActivityInstance.LoadActivity();
                lobjActivityInstance.ibusActivity.LoadRoles();
                lobjActivityInstance.LoadProcessInstance();
                lobjActivityInstance.ibusProcessInstance.ibusProcess = lobjActivityInstance.ibusActivity.ibusProcess;
                //lobjActivityInstance.ibusProcessInstance.LoadOrganization();
                lobjActivityInstance.ibusProcessInstance.LoadPerson();
                //lobjActivityInstance.ibusProcessInstance.LoadContactTicket();
                //lobjActivityInstance.LoadProcessInstanceImageData();
                lobjActivityInstance.LoadProcessInstanceHistory();
                lobjActivityInstance.LoadProcessInstanceChecklist();
                lobjActivityInstance.LoadProcessInstanceNotes();
                //prod pir 4118
                //default suspension date to today's date + 30 days
                //if (lobjActivityInstance.icdoActivityInstance.suspension_end_date == DateTime.MinValue)
                //    lobjActivityInstance.icdoActivityInstance.suspension_end_date = DateTime.Now.AddDays(30);
                lobjActivityInstance.EvaluateInitialLoadRules();
            }

            return lobjActivityInstance;
        }

        public busWorkflowRequest FindWorkflowRequest(int aintworkflowrequestid)
        {
            busWorkflowRequest lobjWorkflowRequest = new busWorkflowRequest();
            if (lobjWorkflowRequest.FindWorkflowRequest(aintworkflowrequestid))
            {
            }

            return lobjWorkflowRequest;
        }

        public busProcessInstanceImageData FindProcessInstanceImageData(int aintprocessinstanceimagedataid)
        {
            busProcessInstanceImageData lobjProcessInstanceImageData = new busProcessInstanceImageData();
            if (lobjProcessInstanceImageData.FindProcessInstanceImageData(aintprocessinstanceimagedataid))
            {
            }

            return lobjProcessInstanceImageData;
        }

        public busReassignWork SearchAndLoadReassignmentBasket()
        {
            busReassignWork lobjReassignWork = new busReassignWork();
            lobjReassignWork.SearchAndLoadReassignmentBasket();
            return lobjReassignWork;
        }

        public busReassignWorkDetail LoadReassignmentWorkDetail(int aintActivityInstanceID)
        {
            busReassignWorkDetail lobjReassignWorkDetail = new busReassignWorkDetail();
            lobjReassignWorkDetail.LoadReassigmentDetail(aintActivityInstanceID);
            return lobjReassignWorkDetail;
        }

        public busProcessInitiation InitiateOnlineProcessInitiation()
        {
            busProcessInitiation lobjProcessInitiation = new busProcessInitiation();
            lobjProcessInitiation.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            return lobjProcessInitiation;
        }

        public busMyBasket SearchAndLoadMyBasket()
        {
            busMyBasket lobjMyBasket = new busMyBasket();
            //Set the Default Filter As Work Pool
            lobjMyBasket.istrMyBasketFilter = busConstant.MyBasketFilter_WorkPool;
            lobjMyBasket.SearchAndLoadMyBasket();
            return lobjMyBasket;
        }


        
        public busActivityInstance GetUserActivities(int aintActivityInstanceID)
        {
            busActivityInstance lobjActivityInstance = new busActivityInstance();
            lobjActivityInstance.LoadCenterleftObjects(aintActivityInstanceID);
            return lobjActivityInstance;
        }
       
              

        public busActivityInstanceHistory FindActivityInstanceHistory(int aintactivityinstancehistoryid)
        {
            busActivityInstanceHistory lobjActivityInstanceHistory = new busActivityInstanceHistory();
            if (lobjActivityInstanceHistory.FindActivityInstanceHistory(aintactivityinstancehistoryid))
            {
                lobjActivityInstanceHistory.LoadActivityInstance();
            }

            return lobjActivityInstanceHistory;
        }

        //public override void ModifyActivityRedirectInformation(utlWorkflowActivityInfo aobjWorkflowActivityInfo, utlProcessMaintainance.utlActivity aobjActivity, string astrButtonID, ArrayList aarrResult, string astrUserID, int aintUserSerialID)
        //{
        //    busActivityInstance lobAct = (busActivityInstance)aobjWorkflowActivityInfo.ibusBaseActivityInstance;
        //    if (lobAct.ibusActivity.icdoActivity.process_id == busConstant.Map_Recalculate_Pension_and_RHIC_Benefit && lobAct.icdoActivityInstance.activity_id == 171)
        //    {
        //        busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
        //        if (lobjPayeeAccount.FindPayeeAccount(lobAct.icdoActivityInstance.reference_id))
        //        {
        //            if (lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
        //            {
        //                aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
        //                aobjWorkflowActivityInfo.istrURL = "wfmPreRetirementDeathFinalCalculationMaintenance";
        //                aobjWorkflowActivityInfo.ihstLaunchParameters.Add("reference_id", lobAct.icdoActivityInstance.reference_id);
        //            }
        //            else if (lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath)
        //            {
        //                aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
        //                aobjWorkflowActivityInfo.istrURL = "wfmPostRetirementDeathFinalCalculationMaintenance";
        //                aobjWorkflowActivityInfo.ihstLaunchParameters.Add("reference_id", lobAct.icdoActivityInstance.reference_id);
        //            }
        //        }
        //    }
        //    else if (lobAct.ibusActivity.icdoActivity.process_id == busConstant.Map_Initialize_Process_Death_Notification_Workflow
        //               && lobAct.icdoActivityInstance.activity_id == 57 && lobAct.icdoActivityInstance.reference_id == 0)
        //    {
        //        if (lobAct.ibusProcessInstance.ibusWorkflowRequest == null)
        //            lobAct.ibusProcessInstance.LoadWorkflowRequest();

        //        if (lobAct.ibusProcessInstance.ibusContactTicket == null)
        //            lobAct.ibusProcessInstance.LoadContactTicket();

        //        if (lobAct.ibusProcessInstance.ibusContactTicket.icdoContactTicket.contact_ticket_id > 0)
        //        {
        //            if (lobAct.ibusProcessInstance.ibusContactTicket.ibusDeathNotice == null)
        //            {
        //                lobAct.ibusProcessInstance.ibusContactTicket.ibusDeathNotice = new busDeathNotice();
        //            }
        //            lobAct.ibusProcessInstance.ibusContactTicket.ibusDeathNotice.FindDeathNoticeByContactTicket(lobAct.ibusProcessInstance.icdoProcessInstance.contact_ticket_id);
        //            aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
        //            aobjWorkflowActivityInfo.ihstLaunchParameters.Add("person_id", lobAct.ibusProcessInstance.icdoProcessInstance.person_id);
        //            aobjWorkflowActivityInfo.ihstLaunchParameters.Add("dateof_death", lobAct.ibusProcessInstance.ibusContactTicket.ibusDeathNotice.icdoDeathNotice.death_date);
        //        }
        //    }
        //    else if (lobAct.ibusActivity.icdoActivity.process_id == busConstant.Map_Update_Dues_Rate_Table
        //                && (lobAct.icdoActivityInstance.activity_id == 189 || lobAct.icdoActivityInstance.activity_id == 190) && lobAct.icdoActivityInstance.reference_id == 0)
        //    {
        //        if (lobAct.ibusProcessInstance.ibusOrganization == null)
        //            lobAct.ibusProcessInstance.LoadOrganization();

        //        aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
        //        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("org_code", lobAct.ibusProcessInstance.ibusOrganization.icdoOrganization.org_code);
        //    }
        //    else if (lobAct.ibusActivity.icdoActivity.process_id == busConstant.MapWSSEnrollNewHireInPensionAndInsurancePlans)
        //    {
        //        busWssPersonAccountEnrollmentRequest lobjEnrollmentRequest = new busWssPersonAccountEnrollmentRequest();
        //        if (lobjEnrollmentRequest.FindWssPersonAccountEnrollmentRequest(lobAct.icdoActivityInstance.reference_id))
        //        {
        //            if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDBRetirement)
        //            {
        //                aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
        //                aobjWorkflowActivityInfo.istrURL = "wfmViewRequestPensionPlanRetirmentEnrollmentMaintenance";
        //                aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aint_request_id", lobAct.icdoActivityInstance.reference_id);
        //            }
        //            else if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDBOptional)
        //            {
        //                aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
        //                aobjWorkflowActivityInfo.istrURL = "wfmViewRequestPensionPlanMainRetirementOptionalEnrollmentMaintenance";
        //                aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aint_request_id", lobAct.icdoActivityInstance.reference_id);
        //            }
        //            else if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDBElectedOfficial)
        //            {
        //                aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
        //                aobjWorkflowActivityInfo.istrURL = "wfmViewRequestDBElectedOfficialMaintenance";
        //                aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aint_request_id", lobAct.icdoActivityInstance.reference_id);
        //            }
        //            else if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDCOptional)
        //            {
        //                aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
        //                aobjWorkflowActivityInfo.istrURL = "wfmViewRequestPensionPlanDCRetirementEnrollmentMaintenance";
        //                aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aint_request_id", lobAct.icdoActivityInstance.reference_id);
        //            }
        //            else if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDCRetirement)
        //            {
        //                aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
        //                aobjWorkflowActivityInfo.istrURL = "wfmViewRquestPensionPlanDCRetirementEnrollmentMaintenance";
        //                aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aint_request_id", lobAct.icdoActivityInstance.reference_id);
        //            }
        //            else if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeGHDV)
        //            {
        //                aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
        //                aobjWorkflowActivityInfo.istrURL = "wfmViewRequestGHDVEnrollmentMaintenance";
        //                aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aint_request_id", lobAct.icdoActivityInstance.reference_id);
        //            }
        //            else if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeLife)
        //            {
        //                aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
        //                aobjWorkflowActivityInfo.istrURL = "wfmViewRequestLifeEnrollmentMaintenance";
        //                aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aint_request_id", lobAct.icdoActivityInstance.reference_id);
        //            }
        //            else if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeFlexComp)
        //            {
        //                aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
        //                aobjWorkflowActivityInfo.istrURL = "wfmViewRequestFlexCompEnrollmentMaintenance";
        //                aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aint_request_id", lobAct.icdoActivityInstance.reference_id);
        //            }
        //        }
        //    }
        //    else if (lobAct.ibusActivity.icdoActivity.process_id == busConstant.Map_ACH_Pull_For_IBS_Insurance ||
        //        lobAct.ibusActivity.icdoActivity.process_id == busConstant.Map_ACH_Pull_For_Insurance ||
        //        lobAct.ibusActivity.icdoActivity.process_id == busConstant.Map_ACH_Pull_For_Retirement ||
        //        lobAct.ibusActivity.icdoActivity.process_id == busConstant.Map_ACH_Pull_For_DeferredCompensation)
        //    {
        //        if (aobjWorkflowActivityInfo.istrURL == "wfmDepositTapeMaintenance")
        //        {
        //            aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aint_activity_instance_id", lobAct.icdoActivityInstance.activity_instance_id);
        //        }
        //    }
        //    else if (lobAct.ibusActivity.icdoActivity.process_id == busConstant.Map_Initialize_Process_Beneficiary_Auto_Refund_Workflow)
        //    {
        //        //process instance is not loaded thats y loaded here again. for PIR - 1724
        //        lobAct.LoadProcessInstance();
        //        lobAct.ibusProcessInstance.LoadWorkflowRequest();

        //        int lintBeneficiaryPersonId = 0;
        //        if (!String.IsNullOrEmpty(lobAct.ibusProcessInstance.ibusWorkflowRequest.icdoWorkflowRequest.additional_parameter1))
        //            lintBeneficiaryPersonId = Convert.ToInt32(lobAct.ibusProcessInstance.ibusWorkflowRequest.icdoWorkflowRequest.additional_parameter1);

        //        aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
        //        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("member_person_id", lobAct.ibusProcessInstance.icdoProcessInstance.person_id);
        //        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("recipient_person_id", lintBeneficiaryPersonId);
        //        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("benefit_application_id", lobAct.icdoActivityInstance.reference_id);
        //    }
        //}

        public busProcessInstanceImageData OpenFileNetImage(string astrObjectStore,
                                                              string astrVersionSeriesId,
                                                              string astrDocumentId,
                                                              string astrDocumentTitle)
        {
            busProcessInstanceImageData lobjbusWfImageData = new busProcessInstanceImageData();
            lobjbusWfImageData.object_store = astrObjectStore;
            lobjbusWfImageData.version_series_id = astrVersionSeriesId;
            lobjbusWfImageData.document_id = astrDocumentId;
            lobjbusWfImageData.document_title = astrDocumentTitle;
            return lobjbusWfImageData;
        }

		public busRequestParameter FindRequestParameter(int aintRequestParameterId)
		{
			busRequestParameter lobjRequestParameter = new busRequestParameter();
			if (lobjRequestParameter.FindRequestParameter(aintRequestParameterId))
			{
				lobjRequestParameter.LoadWorkflowRequest();
			}

			return lobjRequestParameter;
		}
    }
}
