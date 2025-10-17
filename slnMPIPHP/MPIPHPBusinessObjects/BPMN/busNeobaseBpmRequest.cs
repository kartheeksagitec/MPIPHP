#region [Using directives]
using NeoBase.Common;
using NeoBase.Common.DataObjects;
using NeoSpinConstants;
using Sagitec.Bpm;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
#endregion [Using directives]

namespace NeoBase.BPM
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busBpmRequest:
    /// Inherited from busBpmRequestGen, the class is used to customize the business object busBpmRequestGen.
    /// </summary>
    [Serializable]

    public class busNeobaseBpmRequest : busBpmRequest
    {
        public busNeobaseBpmRequest() : base()
        {
   
            ibusPerson = busBase.CreateNewObject(BpmClientBusinessObjects.istrPerson);
            ibusOrganization = busBase.CreateNewObject(BpmClientBusinessObjects.istrOrganization);        
        }

        /// <summary>
        /// Load Method
        /// </summary>
        public void NewBpmRequest()
        {
            icdoBpmRequest = new doBpmRequest();
            icdoBpmRequest.status_value = WorkflowConstants.WORKFLOW_REQUEST_STATUS_NOT_PROCESSED;
            icdoBpmRequest.source_value = WorkflowConstants.WORKFLOW_REQUEST_SOURCE_ONLINE;
            icdoBpmRequest.created_by = iobjPassInfo.istrFormName;
        }

        /// <summary>
        /// Used to initialize the process request.
        /// </summary>
        /// <returns>Arraylist containing the current Object if valid, error object o.w.</returns>
        public override ArrayList InitializeProcess()
        {
            ArrayList larrResult = new ArrayList();
            utlError lutlError = new utlError();
            int lintProcessId = icdoBpmRequest.process_id;
            int lintPersonId = icdoBpmRequest.person_id;
            int lintOrgId = icdoBpmRequest.org_id;
            long llngReferenceId = icdoBpmRequest.reference_id;

            //busPerson lbusPerson = new busPerson { icdoPerson = new doPerson() };
            //busOrganization lbusOrganization = new busOrganization() { icdoOrganization = new doOrganization() };

            busBase lbusPerson = busBase.CreateNewObject(BpmClientBusinessObjects.istrPerson);
            busBase lbusOrganization = busBase.CreateNewObject(BpmClientBusinessObjects.istrOrganization);
            ibusBpmProcess = new busBpmProcess { icdoBpmProcess = new doBpmProcess() };
            ibusBpmProcess.FindByPrimaryKey(icdoBpmRequest.process_id);

            //Process Type should be defined. either NONE, PERSON, ORGANISATION
            if (this.ibusBpmProcess.icdoBpmProcess.type_value == null)
            {
                lutlError = AddError(WorkflowConstants.MESSAGE_ID_1569, String.Empty);
                larrResult.Add(lutlError);
            }

            if (this.ibusBpmProcess.icdoBpmProcess.type_value == "PERS")
            {
                if (icdoBpmRequest.person_id <= 0 || !lbusPerson.FindByPrimaryKey(icdoBpmRequest.person_id))
                {
                    lutlError = AddError(BpmMessages.Message_Id_1541, String.Empty);
                    larrResult.Add(lutlError);
                }
                if (icdoBpmRequest.org_id > 0)
                {
                    lutlError = AddError(WorkflowConstants.MESSAGE_ID_1573, String.Empty);
                    larrResult.Add(lutlError);
                }
                this.LoadPerson();
            }
            if (this.ibusBpmProcess.icdoBpmProcess.type_value == "ORGN")
            {
                if (icdoBpmRequest.org_id <= 0 || !lbusOrganization.FindByPrimaryKey(icdoBpmRequest.org_id))
                {
                    lutlError = AddError(BpmMessages.Message_Id_1542, String.Empty);
                    larrResult.Add(lutlError);
                }
                if (icdoBpmRequest.person_id > 0)
                {
                    lutlError = AddError(WorkflowConstants.MESSAGE_ID_1573, String.Empty);
                    larrResult.Add(lutlError);
                }
                this.LoadOrganization();
            }

            //if (this.istrNotes == null)
            //{
            //    lutlError = AddError(WorkflowConstants.MESSAGE_ID_20008002, String.Empty);
            //    larrResult.Add(lutlError);
            //}

            /// Date: 05/25/2021
            /// Iteration: 9
            /// Developer: Rahul Mane
            /// Comment : If Allow Multiple Instances is selected, BPM engine should allow creating of multiple process instances for the same person. 

            if (this.icdoBpmRequest.check_for_existing_inst == WorkflowConstants.FLAG_YES)
            {
                this.icdoBpmRequest.check_for_existing_inst = WorkflowConstants.FLAG_YES;
            }
            else
            {
                this.icdoBpmRequest.check_for_existing_inst = WorkflowConstants.FLAG_NO;
            }


            /// Date: 03/18/2024
            /// Iteration: 14
            /// Developer: Jayendran
            /// Comment: Fix includes showing multiple validations errors at the same time instead of showing just one at a time (changes made to fix urs)

            ArrayList larr = base.InitializeProcess();
            if (larrResult.Count > 0 && !(larr[0] is utlError))
            {
                return larrResult;
            }
            else
            {
                larrResult.AddRange(larr);
            }

            if (!(larrResult[0] is utlError))
            {
                /// Date: 09/29/2021
                /// Iteration: 10
                /// Developer: Rahul Mane 
                /// Comment: Changes for Adding notes provided during case initiating with Request Id as pimary key in sgt_notes table(done changes for cayman island)
                //busNeobaseBpmRequest lbusNeobaseBpmRequest = ClassMapper.GetObject<busNeobaseBpmRequest>();
                //lbusNeobaseBpmRequest = (busNeobaseBpmRequest)this;
                //lbusNeobaseBpmRequest.AddNotes((larrResult[0] as busBpmRequest).icdoBpmRequest.request_id);
                this.icdoBpmRequest = new doBpmRequest();
                this.icdoBpmRequest.process_id = lintProcessId;
                this.icdoBpmRequest.person_id = lintPersonId;
                this.icdoBpmRequest.org_id = lintOrgId;
                this.icdoBpmRequest.reference_id = llngReferenceId;
                larrResult.Clear();
                larrResult.Add(this);
            }
            return larrResult;
        }


        /// <summary>
        /// Add notes for the online request.
        /// </summary>
        /// <param name="aintRequestId">Request id.</param>
        public virtual void AddNotes(int aintRequestId)
        {
            if (this.istrNotes.IsNotNull() && this.istrNotes!="")
            {
                doNotes lcdoNotes = new doNotes();
                lcdoNotes.table_name = WorkflowConstants.REQUEST_TABLE;
                lcdoNotes.primary_key_id = aintRequestId;
                lcdoNotes.notes_text = this.istrNotes;
                /// Date: 09/09/2020
                /// Iteration: 7
                /// Developer: Akshay Bahirat  
                /// Comment:  Set the category value to BPNT because we want to show this notes in Activity instance notes tab.
                lcdoNotes.category_value = "BPNT";
                lcdoNotes.Insert();
            }
        }


        /// <summary>
        /// Creates a Request of source ScanAndIndex
        /// </summary>
        /// <returns></returns>
        public new ArrayList InitializeScanDocumentReceivedProcess()
        {
            ArrayList larrResult = new ArrayList();
            utlError lutlError = new utlError();
            utlError lutlError1 = new utlError();
            List<string> llstMessageFlows = new List<string>();
            //busPerson lbusPerson = new busPerson { icdoPerson = new doPerson() };
            //busOrganization lbusOrganization = new busOrganization() { icdoOrganization = new doOrganization() };

            busBase lbusPerson = busBase.CreateNewObject(BpmClientBusinessObjects.istrPerson);
            busBase lbusOrganization = busBase.CreateNewObject(BpmClientBusinessObjects.istrOrganization);
            ibusBpmProcess = new busBpmProcess { icdoBpmProcess = new doBpmProcess() };
            ibusBpmProcess.FindByPrimaryKey(icdoBpmRequest.process_id);
            //Validations added.
            //Process Type should be defined. either NONE, PERSON, ORGANISATION
            //Developer - Rahul Mane
            //Iteration - Iteration10
            //Comment -Fixed PIR - 3506 - added Saperate condition for both Error.

            //Developer - Ratnesh Kumar Singh
            //Iteration - Iteration12
            // Date   -  04/05/2022
            //Comment -Fixed PIR -4148-Added condition for showing the hard error message for the mandatory field- 'Document Type' and 'Process Name' 
            if (this.ibusBpmProcess.icdoBpmProcess.process_id <= 0 && string.IsNullOrEmpty(this.icdoBpmRequest.doc_type))
            {
                lutlError = AddError(WorkflowConstants.MESSAGE_ID_1524, String.Empty);
                larrResult.Add(lutlError);
                lutlError1 = AddError(utlMessageType.Framework, BpmMessages.Message_Id_5128, String.Empty);
                larrResult.Add(lutlError1);
                return larrResult;
            }
            if (this.ibusBpmProcess.icdoBpmProcess.process_id <= 0)
            {
                lutlError = AddError(WorkflowConstants.MESSAGE_ID_1524, String.Empty);              
                larrResult.Add(lutlError);              
                return larrResult;
            }
            if (string.IsNullOrEmpty(this.icdoBpmRequest.doc_type))
            {
                lutlError1 = AddError(utlMessageType.Framework, BpmMessages.Message_Id_5128, String.Empty);
                larrResult.Add(lutlError1);
                return larrResult;
            }

            //Developer - Ratnesh Kumar Singh
            //Iteration - Iteration12
            // Date   -  09/05/2022
            //Comment -Fixed PIR -4149-Added condition for Display Hard Error If Person ID Or Organization ID Not Entered. 

            if (this.ibusBpmProcess.icdoBpmProcess.type_value != "NONE")
            {

                if (this.icdoBpmRequest.person_id == 0 && this.icdoBpmRequest.org_id == 0)
                {
                    lutlError = AddError(WorkflowConstants.MESSAGE_ID_20007031, String.Empty);
                    larrResult.Add(lutlError);
                    return larrResult;
                }
            }

            string lstrDocClass = string.Empty;
            #region changes for cayman island

            /// Date: 09/09/2020
            /// Iteration: 7
            /// Developer: Akshay Bahirat  
            /// Comment: Changes for dynamic validation for person id and org id on recieve document screen.
            /// We are fetching PERSON_ID_REQUIRED_IND,ORG_ID_REQUIRED_IND from SGW_BPM_EVENT according to doc_type and if this flags are set then we are 
            /// checking if person id and org id is provided by user or not if this flags are not set then its not necessary to provide person id and org id
            /// (done changes for cayman island)


            string istrPersonIdInd = null;
            string istrOrgIdInd = null;
            if (this.icdoBpmRequest.doc_type.IsNotNullOrEmpty())
            {
                object lobjDocClass = DBFunction.DBExecuteScalar("entBpmEvent.GetDocClassByDocType", new object[1] { icdoBpmRequest.doc_type }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                DataTable ldtbNotes = Select("entBpmEvent.GetPersonOrgIndicator", new object[1] { icdoBpmRequest.doc_type });
                if (lobjDocClass.IsNotNull())
                {
                    lstrDocClass = lobjDocClass.ToString();
                }
                if (ldtbNotes.Rows.Count != 0)
                {
                    istrPersonIdInd = ldtbNotes.Rows[0]["PERSON_ID_REQUIRED_IND"].ToString();
                    istrOrgIdInd = ldtbNotes.Rows[0]["ORG_ID_REQUIRED_IND"].ToString();
                }
            }
            #endregion

            if (this.ibusBpmProcess.icdoBpmProcess.type_value == "PERS")
            {
                if ((icdoBpmRequest.person_id <= 0 || !lbusPerson.FindByPrimaryKey(icdoBpmRequest.person_id)) &&
                    istrPersonIdInd == "Y")
                {
                    lutlError = AddError(Common.busNeoBaseConstants.Message.MessageID_20007031, String.Empty);
                    larrResult.Add(lutlError);
                    return larrResult;
                }
                if (icdoBpmRequest.org_id > 0)
                {
                    lutlError = AddError(WorkflowConstants.MESSAGE_ID_1573, String.Empty);
                    larrResult.Add(lutlError);
                    return larrResult;
                }
                this.LoadPerson();
            }
            if (this.ibusBpmProcess.icdoBpmProcess.type_value == "ORGN")
            {
                if ((icdoBpmRequest.org_id <= 0 || !lbusOrganization.FindByPrimaryKey(icdoBpmRequest.org_id)) &&
                    istrOrgIdInd == "Y")
                {
                    lutlError = AddError(Common.busNeoBaseConstants.Message.MessageID_20007031, String.Empty);
                    larrResult.Add(lutlError);
                    return larrResult;
                }
                if (icdoBpmRequest.person_id > 0)
                {
                    lutlError = AddError(WorkflowConstants.MESSAGE_ID_1573, String.Empty);
                    larrResult.Add(lutlError);
                    return larrResult;
                }
                this.LoadOrganization();
            }
            if (this.icdoBpmRequest.process_id > 0)
            {
                this.LoadBpmProcess();
                this.ibusBpmProcess.ibusBpmCase = busBpmCase.GetBpmCase(this.ibusBpmProcess.icdoBpmProcess.case_id);
                llstMessageFlows = this.ibusBpmProcess.ibusBpmCase.GetBpmMessageFlowsByDocType(this.icdoBpmRequest.doc_type);
                this.LoadPerson();
                this.LoadOrganization();
            }
            else if (this.icdoBpmRequest.process_id <= 0)
            {
                /// Date: 09/09/2020
                /// Iteration: 7
                /// Developer: Akshay Bahirat  
                /// Comment:  not mandatory to provide the process name while receiving the document (changes for cayman) 
                //lutlError = AddError(WorkflowConstants.MESSAGE_ID_1569, String.Empty);
                //larrResult.Add(lutlError);
                //return larrResult;
            }




            if ((this.icdoBpmRequest.person_id <= 0 && istrPersonIdInd == "Y") || (this.icdoBpmRequest.org_id <= 0 && istrOrgIdInd == "Y") && this.icdoBpmRequest.reference_id <= 0)
            {
                lutlError = AddError(utlMessageType.Framework, BpmMessages.Message_Id_1558, String.Empty);
                larrResult.Add(lutlError);
                return larrResult;
            }



            if (llstMessageFlows.Count > 0)
            {
                foreach (string lstrMessageFlow in llstMessageFlows)
                {
                    //Initialize the Workflow
                    doBpmRequest lcdoBpmRequest = new doBpmRequest();
                    Guid g = Guid.NewGuid();
                    lcdoBpmRequest.person_id = icdoBpmRequest.person_id;
                    lcdoBpmRequest.org_id = icdoBpmRequest.org_id;
                    lcdoBpmRequest.process_id = icdoBpmRequest.process_id;
                    lcdoBpmRequest.source_value = BpmRequestSource.ScanAndIndex;
                    lcdoBpmRequest.status_value = BpmRequestStatus.NotProcessed;
                    lcdoBpmRequest.bpm_message_flow_id = lstrMessageFlow;
                    lcdoBpmRequest.doc_type = this.icdoBpmRequest.doc_type;
                    lcdoBpmRequest.doc_class = lstrDocClass;
                    lcdoBpmRequest.created_by = iobjPassInfo.istrUserID;
                    lcdoBpmRequest.reference_id = icdoBpmRequest.reference_id;
                    lcdoBpmRequest.priority_code_id = icdoBpmRequest.priority_code_id;
                    lcdoBpmRequest.priority_code_value = icdoBpmRequest.priority_code_value;
                    lcdoBpmRequest.tracking_id = icdoBpmRequest.tracking_id;
                    lcdoBpmRequest.reason_value = icdoBpmRequest.reason_value;
                    lcdoBpmRequest.ecm_guid = g.ToString();
                    lcdoBpmRequest.Insert();

                    foreach (busBpmRequestParameter lbusParam in iclbBpmRequestParameter)
                    {
                        lbusParam.icdoBpmRequestParameter.request_id = lcdoBpmRequest.request_id;
                        lbusParam.icdoBpmRequestParameter.Insert();
                    }
                }
            }
            else
            {
                //Initialize the Workflow
                doBpmRequest lcdoBpmRequest = new doBpmRequest();
                Guid g = Guid.NewGuid();
                lcdoBpmRequest.person_id = icdoBpmRequest.person_id;
                lcdoBpmRequest.org_id = icdoBpmRequest.org_id;
                lcdoBpmRequest.process_id = icdoBpmRequest.process_id;
                lcdoBpmRequest.source_value = BpmRequestSource.ScanAndIndex;
                lcdoBpmRequest.status_value = BpmRequestStatus.NotProcessed;
                lcdoBpmRequest.doc_type = this.icdoBpmRequest.doc_type;
                lcdoBpmRequest.doc_class = lstrDocClass;
                lcdoBpmRequest.created_by = iobjPassInfo.istrUserID;
                lcdoBpmRequest.reference_id = icdoBpmRequest.reference_id;
                lcdoBpmRequest.priority_code_id = icdoBpmRequest.priority_code_id;
                lcdoBpmRequest.priority_code_value = icdoBpmRequest.priority_code_value;
                lcdoBpmRequest.tracking_id = icdoBpmRequest.tracking_id;
                lcdoBpmRequest.reason_value = icdoBpmRequest.reason_value;
                lcdoBpmRequest.ecm_guid = g.ToString();
                lcdoBpmRequest.Insert();

                foreach (busBpmRequestParameter lbusParam in iclbBpmRequestParameter)
                {
                    lbusParam.icdoBpmRequestParameter.request_id = lcdoBpmRequest.request_id;
                    lbusParam.icdoBpmRequestParameter.Insert();
                }
            }
            larrResult.Add(this);
            return larrResult;
        }
        ///// <summary>
        ///// Framework Version 6.0.6.0
        ///// Prevent initiating a BPM based on condition.
        ///// </summary>
        ///// <param name="abusBpmCase"></param>
        ///// <param name="abusBpmProcess"></param>
        ///// <returns></returns>
        //public override bool ValidateRequestBeforeInitiation(busBpmCase abusBpmCase, busBpmProcess abusBpmProcess)
        //{
        //    return base.ValidateRequestBeforeInitiation(abusBpmCase, abusBpmProcess);
        //}
        /// Developer - Rahul Mane
        /// Date - 09-24-2021
        /// Iteration - Main-Iteration10 
        /// Comment - This method call from BPM engine there we have used Class Mapper and move this to NeobaseBPM Request this is done because we have remove the NeoSpinCommunication reference form NeoSpinBPMEngine
        /// <summary>
        /// Calls  the IncomingDocumentRecevied method to notify communication module for the incoming document.
        /// </summary>
        /// <param name="abusBpmRequest"></param>
        public virtual void IncomingDocumentReceived(busBpmRequest abusBpmRequest)
        {
            
        }

    }
}
