#region Using directives

using System;
using System.Collections;
using System.Data;
using Sagitec.Common;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
using Sagitec.DBUtility;

#endregion

namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public class busProcessInitiation : busMPIPHPBase
    {
        private int _iintPersonId;
        public int iintPersonId
        {
            get { return _iintPersonId; }
            set { _iintPersonId = value; }
        }
        private string _istrOrgCodeId;
        public string istrOrgCodeId
        {
            get { return _istrOrgCodeId; }
            set { _istrOrgCodeId = value; }
        }

        private int _iintProcessID;
        public int iintProcessID
        {
            get { return _iintProcessID; }
            set { _iintProcessID = value; }
        }

        //property is used to load Organization Details
        //private busOrganization _ibusOrganization;
        //public busOrganization ibusOrganization
        //{
        //    get
        //    {
        //        return _ibusOrganization;
        //    }

        //    set
        //    {
        //        _ibusOrganization = value;
        //    }
        //}

        //property is used to load Person Details
        private busPerson _ibusPerson;
        public busPerson ibusPerson
        {
            get
            {
                return _ibusPerson;
            }

            set
            {
                _ibusPerson = value;
            }
        }
        public ArrayList InitializeProcess()
        {
            ArrayList larrResult = new ArrayList();
            utlError lobjError = new utlError();

            //Validation           
            if (iintProcessID == 0)
            {
                lobjError = AddError(4129, String.Empty);
                larrResult.Add(lobjError);
                return larrResult;
            }
            //PIR 2207
            //if ((_iintPersonId == 0) && (String.IsNullOrEmpty(_istrOrgCodeId)))
            //{
            //    lobjError = AddError(4130, String.Empty);
            //    larrResult.Add(lobjError);
            //    return larrResult;
            //}

            DataTable ldtUserRoles = busBase.Select("cdoUserRoles.LoadRoles", new object[1] { iobjPassInfo.iintUserSerialID });

            if (ldtUserRoles.IsNotNull() && ldtUserRoles.Rows.Count > 0)
            {
                DataRow[] ldrRoles = ldtUserRoles.FilterTable(utlDataType.Numeric, "ROLE_ID", 101);
                if (ldrRoles.IsNotNull() && ldrRoles.Length > 0 && iintProcessID != 5 && iintProcessID != 9 && iintProcessID != 10)
                {
                    lobjError = AddError(940, String.Empty);
                    larrResult.Add(lobjError);
                    return larrResult;
                }
            }

            if (ibusPerson.icdoPerson.mpi_person_id.IsNotNullOrEmpty())
            {
                _iintPersonId = 0;
                DataTable ldtblPersonId = Select("cdoPerson.GetPersonDetails", new object[1] { ibusPerson.icdoPerson.mpi_person_id });

                if (ldtblPersonId != null && ldtblPersonId.Rows.Count > 0 && Convert.ToString(ldtblPersonId.Rows[0][enmPerson.person_id.ToString().ToUpper()]).IsNotNullOrEmpty())
                {
                    _iintPersonId = Convert.ToInt32(ldtblPersonId.Rows[0][enmPerson.person_id.ToString().ToUpper()]);
                }


                if (_iintPersonId == 0)
                {
                    lobjError = AddError(0, "Participant with this MPID does not exists");
                    larrResult.Add(lobjError);
                    return larrResult;
                }
            }
            else
            {
                lobjError = AddError(0, "Please insert Participant MPID");
                larrResult.Add(lobjError);
                return larrResult;
            }

          
            if ((_iintPersonId != 0) && (!String.IsNullOrEmpty(_istrOrgCodeId)))
            {
                lobjError = AddError(4130, String.Empty);
                larrResult.Add(lobjError);
                return larrResult;
            }

            if (_iintPersonId != 0)
            {
                LoadPerson();
                if (ibusPerson.icdoPerson.person_id == 0)
                {
                    lobjError = AddError(4131, String.Empty);
                    larrResult.Add(lobjError);
                    return larrResult;
                }
            }
          

            if (!String.IsNullOrEmpty(_istrOrgCodeId))
            {
                //LoadOrganization();
                //if (ibusOrganization.icdoOrganization.org_id == 0)
                //{
                //    lobjError = AddError(4132, String.Empty);
                //    larrResult.Add(lobjError);
                //    return larrResult;
                //}
            }

            busProcess lbusProcess = new busProcess();
            lbusProcess.FindProcess(iintProcessID);

            if (lbusProcess.icdoProcess.process_id > 0)
            {
                if (!String.IsNullOrEmpty(_istrOrgCodeId))
                {
                    if (lbusProcess.icdoProcess.type_value == busConstant.ProcessType_Person)
                    {
                        lobjError = AddError(4134, String.Empty);
                        larrResult.Add(lobjError);
                        return larrResult;

                    }
                }
                else if ((_iintPersonId != 0))
                {
                    if (lbusProcess.icdoProcess.type_value == busConstant.ProcessType_Org)
                    {
                        lobjError = AddError(4135, String.Empty);
                        larrResult.Add(lobjError);
                        return larrResult;
                    }
                }

                if (lbusProcess.icdoProcess.type_value == busConstant.ProcessType_Person)
                {
                    if (_iintPersonId == 0)
                    {
                        lobjError = AddError(4075, String.Empty);
                        larrResult.Add(lobjError);
                        return larrResult;
                    }
                }
                else if (lbusProcess.icdoProcess.type_value == busConstant.ProcessType_Org)
                {
                    if (_istrOrgCodeId.IsNullOrEmpty())
                    {
                        lobjError = AddError(1032, String.Empty);
                        larrResult.Add(lobjError);
                        return larrResult;
                    }
                }
            }

            if (_iintPersonId != 0 && iintProcessID != busConstant.PRERETIREMENT_DEATH_WORKFLOW_PROCESS_ID && iintProcessID != busConstant.DISABILITY_WORKFLOW_PROCESS_ID
                                   && iintProcessID != busConstant.RETIREMENT_WORKFLOW_PROCESS_ID && iintProcessID != busConstant.WITHDRAWAL_WORKFLOW_PROCESS_ID
                                   && iintProcessID != busConstant.QDRO_WORKFLOW_PROCESS_ID)                       
            {
                DataTable ldtblistPerson = Select("cdoActivityInstance.LoadRunningInstancesByPersonAndProcess", new object[2] { _iintPersonId, iintProcessID });

                int lintlistPerson = (int)DBFunction.DBExecuteScalar("cdoActivityInstance.GetNotProcessedWFForPerson", new object[2] { iintProcessID,_iintPersonId },
                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                if (ldtblistPerson.Rows.Count > 0 || lintlistPerson > 0)
                {
                    lobjError = AddError(4074, String.Empty);
                    larrResult.Add(lobjError);
                    return larrResult;
                }
            }
            #region PIR 849
            if (_iintPersonId != 0 && iintProcessID == busConstant.RETIREMENT_WORKFLOW_PROCESS_ID)
            {
                DataTable ldtblistPerson = Select("cdoActivityInstance.LoadRunningInstancesByPersonAndProcess", new object[2] { _iintPersonId, iintProcessID });
                if (ldtblistPerson != null && ldtblistPerson.Rows.Count > 0)
                {
                    lobjError = AddError(6250, String.Empty);
                    larrResult.Add(lobjError);
                    return larrResult;
                }
            }
            #endregion

            if (_iintPersonId != 0 && iintProcessID == busConstant.REEMPLOYMENT_WORKFLOW_PROCESS_ID)
            {
                //Initialize the Workflow
                DataTable ldtbList = Select("cdoDroApplication.GetPayeeAccountsOfPaticipant", new object[1] { _iintPersonId });
                if (ldtbList.Rows.Count > 0)
                {
                    foreach (DataRow ldr in ldtbList.Rows)
                    {
                        cdoWorkflowRequest lcdoWorkflowRequest = new cdoWorkflowRequest();
                        lcdoWorkflowRequest.process_id = iintProcessID;
                        lcdoWorkflowRequest.person_id = _iintPersonId;
                        lcdoWorkflowRequest.reference_id = Convert.ToInt64(ldr["PAYEE_ACCOUNT_ID"]);
                        lcdoWorkflowRequest.org_code = _istrOrgCodeId;
                        lcdoWorkflowRequest.status_value = busConstant.WorkflowProcessStatus_UnProcessed;
                        lcdoWorkflowRequest.source_value = busConstant.WorkflowProcessSource_Online;
                        lcdoWorkflowRequest.Insert();
                        larrResult.Add(this);
                    }
                }
            }
            else
            {
                cdoWorkflowRequest lcdoWorkflowRequest = new cdoWorkflowRequest();
                lcdoWorkflowRequest.process_id = iintProcessID;
                lcdoWorkflowRequest.person_id = _iintPersonId;
                //  lcdoWorkflowRequest.reference_id = Convert.ToInt32(ldr["PAYEE_ACCOUNT_ID"]);
                lcdoWorkflowRequest.org_code = _istrOrgCodeId;
                lcdoWorkflowRequest.status_value = busConstant.WorkflowProcessStatus_UnProcessed;
                lcdoWorkflowRequest.source_value = busConstant.WorkflowProcessSource_Online;
                lcdoWorkflowRequest.Insert();

                if (_iintPersonId != 0 && iintProcessID == busConstant.SSN_MERGE_WORKFLOW_PROCESS_ID)
                {
                    cdoRequestParameter lcdoRequestParameter = new cdoRequestParameter();
                    lcdoRequestParameter.workflow_request_id = lcdoWorkflowRequest.workflow_request_id;
                    lcdoRequestParameter.parameter_name = "PERSON_ID";
                    lcdoRequestParameter.parameter_value = Convert.ToString(_iintPersonId);
                    lcdoRequestParameter.Insert();

                    cdoRequestParameter lcdoRequestParametr = new cdoRequestParameter();
                    lcdoRequestParametr.workflow_request_id = lcdoWorkflowRequest.workflow_request_id;
                    lcdoRequestParametr.parameter_name = "MPI_PERSON_ID";
                    lcdoRequestParametr.parameter_value = Convert.ToString(this.ibusPerson.icdoPerson.mpi_person_id);
                    lcdoRequestParametr.Insert();
                }

                larrResult.Add(this);
            }     

            return larrResult;
        }

        public void LoadPerson()
        {
            if (_ibusPerson == null)
            {
                _ibusPerson = new busPerson();
            }
            _ibusPerson.FindPerson(_iintPersonId);
        }

        //public void LoadOrganization()
        //{
        //    if (_ibusOrganization == null)
        //    {
        //        _ibusOrganization = new busOrganization();
        //    }
        //    _ibusOrganization.FindOrganizationByOrgCode(_istrOrgCodeId);
        //}

        //This method called when we click Hand Icon. That will reload the object and display the details
        public ArrayList ReloadHandButtonData()
        {
            ArrayList larrList = new ArrayList();
            if (_iintPersonId != 0)
            {
                LoadPerson();
            }
            if (!String.IsNullOrEmpty(_istrOrgCodeId))
            {
                //LoadOrganization();
            }
            larrList.Add(this);
            return larrList;
        }
    }
}
