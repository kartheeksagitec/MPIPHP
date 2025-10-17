#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.BusinessObjects;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busWorkflowRequest:
    /// Inherited from busWorkflowRequestGen, the class is used to customize the business object busWorkflowRequestGen.
    /// </summary>
    [Serializable]
    public class busWorkflowRequest : busWorkflowRequestGen
    {
        public Collection<busRequestParameter> iclbRequestParameter { get; set; }

        //public busOrganization ibusOrganization { get; set; }

        //public void LoadOrganization()
        //{
        //    if (ibusOrganization == null)
        //    {
        //        ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
        //    }
        //    if (icdoWorkflowRequest.org_code.IsNotNullOrEmpty())
        //        ibusOrganization.FindOrganizationByOrgCode(icdoWorkflowRequest.org_code);
        //}

        //public busContactTicket ibusContactTicket { get; set; }

        //public void LoadContactTicket()
        //{
        //    if (ibusContactTicket == null)
        //    {
        //        ibusContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
        //    }
        //    if (icdoWorkflowRequest.contact_ticket_id > 0)
        //        ibusContactTicket.FindContactTicket(icdoWorkflowRequest.contact_ticket_id);
        //}

        public busDocument ibusDocument { get; set; }

        public void LoadDocument()
        {
            if (ibusDocument == null)
                ibusDocument = new busDocument();

            ibusDocument.FindDocumentByDocumentCode(icdoWorkflowRequest.doc_type);
        }

        public virtual void LoadWorkflowRequestParameters()
        {
            DataTable ldtbList = Select<cdoRequestParameter>(
                new string[1] { enmRequestParameter.workflow_request_id.ToString() },
                new object[1] { icdoWorkflowRequest.workflow_request_id }, null, null);
            iclbRequestParameter = GetCollection<busRequestParameter>(ldtbList, "icdoRequestParameter");
        }

        /// <summary>
        /// Used to add the new workflow instance request for the process file passed as an argument.
        /// </summary>
        /// <param name="astrProcessName">Process name.</param>
        /// <param name="ahstRequestParameters">Hashtable of request parameters.</param>
        /// <returns>Arraylist containing the current object if valid, error object o.w.</returns>
        public ArrayList AddNewWorkFlowRequest(string astrProcessName, Hashtable ahstRequestParameters)
        {
            ArrayList larrResult = new ArrayList();
            utlError lutlError = new utlError();

            //Get the process id for the workflow process name passed as an argument.
            int lintProcessId = GetProcessIDForWorkflow(astrProcessName);

            //Process is required.
            if (lintProcessId == 0)
            {
                lutlError = AddError(5428, String.Empty);
                larrResult.Add(lutlError);
                return larrResult;
            }

            this.icdoWorkflowRequest.process_id = lintProcessId;
            this.icdoWorkflowRequest.source_value = busConstant.WorkflowProcessSource_Online;
            this.icdoWorkflowRequest.status_value = busConstant.WorkflowProcessStatus_UnProcessed;
            this.icdoWorkflowRequest.created_by = iobjPassInfo.istrUserID;

            this.icdoWorkflowRequest.Insert();
            larrResult.Add(this);

            if (ahstRequestParameters.IsNotNull())
            {
                foreach (DictionaryEntry ldeParam in ahstRequestParameters)
                {
                    if (ldeParam.Value.IsNotNull())
                    {
                        AddRequestParameters(ldeParam.Key.ToString(), ldeParam.Value.ToString());
                    }
                    else
                    {
                        AddRequestParameters(ldeParam.Key.ToString(), string.Empty);
                    }
                }
            }

            return larrResult;
        }

        /// <summary>
        /// Used to add the new workflow instance request for the process file passed as an argument.
        /// </summary>
        /// <param name="astrProcessName">Process name.</param>
        /// <returns>Arraylist containing the current object if valid, error object o.w.</returns>
        public ArrayList AddNewWorkFlowRequest(string astrProcessName)
        {
            ArrayList larrResult = new ArrayList();
            utlError lutlError = new utlError();

            //Get the process id for the workflow process name passed as an argument.
            int lintProcessId = GetProcessIDForWorkflow(astrProcessName);

            //Process is required.
            if (lintProcessId == 0)
            {
                lutlError = AddError(5428, String.Empty);
                larrResult.Add(lutlError);
                return larrResult;
            }

            this.icdoWorkflowRequest.process_id = lintProcessId;
            this.icdoWorkflowRequest.source_value = busConstant.WorkflowProcessSource_Online;
            this.icdoWorkflowRequest.status_value = busConstant.WorkflowProcessStatus_UnProcessed;
            this.icdoWorkflowRequest.created_by = iobjPassInfo.istrUserID;

            this.icdoWorkflowRequest.Insert();
            larrResult.Add(this);

            return larrResult;
        }

        /// <summary>
        /// Used to get the process id for the workflow file name passed as an argument.
        /// </summary>
        /// <param name="astrWorkflowFileName">Workflow file name.</param>
        /// <returns>Process id value > 0 if the workflow file exists in database, false o.w.</returns>
        public int GetProcessIDForWorkflow(string astrWorkflowFileName)
        {

            int lintProcessId = 0;
            DataTable ldtProcess = busBase.Select("cdoProcess.GetProcessIdByName", new object[1] { astrWorkflowFileName });

            if (ldtProcess.Rows.Count > 0)
            {
                lintProcessId = Convert.ToInt32(ldtProcess.Rows[0][0]);
            }

            return lintProcessId;
        }

        /// <summary>
        /// Adds the additional parameters related to the request.
        /// </summary>
        /// <param name="astrParamName">Parameter Name.</param>
        /// <param name="astrParamValue">Parameter Value.</param>
        public void AddRequestParameters(string astrParamName, string astrParamValue)
        {
            cdoRequestParameter lcdoRequestParameter = new cdoRequestParameter();
            lcdoRequestParameter.workflow_request_id = this.icdoWorkflowRequest.workflow_request_id;
            lcdoRequestParameter.parameter_name = astrParamName;
            lcdoRequestParameter.parameter_value = astrParamValue;
            lcdoRequestParameter.Insert();
        }


        /// <summary>
        /// Returns true if there exists active workflow for the person id.
        /// </summary>
        /// <returns>True if the active workflow exists, false o.w.</returns>
        public bool IsActiveWorkflowExistsForPerson(int aintPersonId, int aintProcessId,bool ablnCheckInstanceWithRefId = false,int aintReferenceId = 0)
        {
           
            if(ablnCheckInstanceWithRefId && aintReferenceId > 0)
            {
                DataTable ldtblistPerson = Select("cdoProcessInstance.CountActiveProcessForPersonWithRefId", new object[3] { aintPersonId, aintProcessId,aintReferenceId });
                if (ldtblistPerson.Rows.Count > 0)
                {
                    if (Convert.ToInt32(ldtblistPerson.Rows[0][0]) > 0)
                    {
                        return true;
                    }
                }

                ldtblistPerson = Select("cdoWorkflowRequest.GetUnprocessRequestForPersonWithRefId", new object[3] { aintPersonId, aintProcessId, aintReferenceId });

                if (ldtblistPerson.Rows.Count > 0)
                {
                    if (Convert.ToInt32(ldtblistPerson.Rows[0][0]) > 0)
                    {
                        return true;
                    }
                }
            }
            else
            {
                DataTable ldtblistPerson = Select("cdoProcessInstance.CountActiveProcessForPerson", new object[2] { aintPersonId, aintProcessId });
                if (ldtblistPerson.Rows.Count > 0)
                {
                    if (Convert.ToInt32(ldtblistPerson.Rows[0][0]) > 0)
                    {
                        return true;
                    }
                }

                ldtblistPerson = Select("cdoWorkflowRequest.GetUnprocessRequestForPerson", new object[2] { aintPersonId, aintProcessId });

                if (ldtblistPerson.Rows.Count > 0)
                {
                    if (Convert.ToInt32(ldtblistPerson.Rows[0][0]) > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

    }
}
