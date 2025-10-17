using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.BusinessObjects;
using System.Collections.ObjectModel;
using System.Collections;
using Sagitec.Common;
using Sagitec.DBUtility;
using System.Data;
using MPIPHP.CustomDataObjects;

namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public class busReassignWork : busMPIPHPBase
    {
        //Search Criteria Properties
        public int iintProcessID { get; set; }
        public int iintActivityID { get; set; }
        public int iintPersonID { get; set; }
        public int iintOrgID { get; set; }
        public int iintReferenceID { get; set; }
        public string istrCheckedOutUser { get; set; }
        public DateTime idtCreatedDateFrom { get; set; }
        public DateTime idtCreatedDateTo { get; set; }
        public int iintSupervisorRoleID { get; set; }
        public int iintRoleID { get; set; }
        public int iintPriority { get; set; }
        public string istrMPID { get; set; }

        public Collection<busActivityInstance> iclbReassignmentActivities { get; set; }

        //Load Activities reassigned to User
        public ArrayList SearchAndLoadReassignmentBasket()
        {
            ArrayList larrResult = new ArrayList();

            string lstrFinalQuery;
            string lstrQuery;
            Collection<utlWhereClause> lcolWhereClause;
            utlMethodInfo lobjMethodInfo;

            if(idtCreatedDateTo != DateTime.MinValue && idtCreatedDateFrom != DateTime.MinValue)
            {
                if (idtCreatedDateTo < idtCreatedDateFrom)
                {
                    utlError lobjError = new utlError { istrErrorID = "", istrErrorMessage = "Request date to cannot be less than Request date from" };
                    larrResult.Add(lobjError);
                    return larrResult;
                }
            }
            

            lstrQuery = "SearchAndLoadReassignWork";
            lcolWhereClause = BuildWhereClause(lstrQuery, "'INPC','RESU'");
            lobjMethodInfo = iobjPassInfo.isrvMetaDataCache.GetMethodInfo("cdoActivityInstance." + lstrQuery);
            lstrQuery = lobjMethodInfo.istrCommand;
            lstrFinalQuery = lstrQuery;//sqlFunction.AppendWhereClause(lstrQuery, lcolWhereClause, new Collection<IDbDataParameter>(), iobjPassInfo.iconFramework);
            lstrFinalQuery += " order by activity_instance_id desc";
            DataTable ldtbList = DBFunction.DBSelect(lstrFinalQuery, 
                                                                iobjPassInfo.iconFramework,  iobjPassInfo.itrnFramework);
            iclbReassignmentActivities = GetCollection<busActivityInstance>(ldtbList, "icdoActivityInstance");

            larrResult.Add(this);
            return larrResult;
        }

        private Collection<utlWhereClause> BuildWhereClause(string astrQueryId, string astrStatusValue)
        {
            Collection<utlWhereClause> lcolWhereClause = new Collection<utlWhereClause>();

            lcolWhereClause.Add(GetWhereClause(astrStatusValue, "", "sai.status_value", "string", "in", " ", astrQueryId));

            utlWhereClause lobjWhereClause = GetWhereClause(iobjPassInfo.istrUserID, "", "user_id", "string", "exists", " and ", "UserRole");
            utlMethodInfo lobjMethodInfo = iobjPassInfo.isrvMetaDataCache.GetMethodInfo("cdoActivityInstance.UserSupervisorRole");
            lobjWhereClause.istrSubSelect = lobjMethodInfo.istrCommand;
            lcolWhereClause.Add(lobjWhereClause);

            if (iintProcessID > 0)
                lcolWhereClause.Add(GetWhereClause(iintProcessID, "", "spi.process_id", "int", "=", " and ", astrQueryId));

            if (iintActivityID > 0)
                lcolWhereClause.Add(GetWhereClause(iintActivityID, "", "sai.activity_id", "int", "=", " and ", astrQueryId));
            
            if (iintOrgID > 0)
                lcolWhereClause.Add(GetWhereClause(iintOrgID, "", "spi.org_id", "int", "=", " and ", astrQueryId));

            if (istrMPID.IsNotNullOrEmpty())
                lcolWhereClause.Add(GetWhereClause(istrMPID, "", "p.mpi_person_id", "string", "=", " and ", astrQueryId));

            if (iintRoleID > 0)
                lcolWhereClause.Add(GetWhereClause(iintRoleID, "", "sa.role_id", "int", "=", " and ", astrQueryId));

            if (iintSupervisorRoleID > 0)
                lcolWhereClause.Add(GetWhereClause(iintSupervisorRoleID, "", "sa.supervisor_role_id", "int", "=", " and ", astrQueryId));

            if (iintPriority > 0)
                lcolWhereClause.Add(GetWhereClause(iintPriority, "", "pr.priority", "int", "=", " and ", astrQueryId));

            if (iintReferenceID > 0)
                lcolWhereClause.Add(GetWhereClause(iintReferenceID, "", "sai.reference_id", "int", "=", " and ", astrQueryId));

            if(istrCheckedOutUser.IsNotNullOrEmpty())
                lcolWhereClause.Add(GetWhereClause(istrCheckedOutUser, "", "sai.checked_out_user", "string", "=", " and ", astrQueryId));

            if ((idtCreatedDateFrom != DateTime.MinValue) || (idtCreatedDateTo != DateTime.MinValue))
            {
                if (idtCreatedDateFrom == DateTime.MinValue)
                {
                    idtCreatedDateFrom = DateTime.MinValue;
                }
                if (idtCreatedDateTo == DateTime.MinValue)
                {
                    idtCreatedDateTo = DateTime.MaxValue;
                }
                lcolWhereClause.Add(GetWhereClause(idtCreatedDateFrom, idtCreatedDateTo, "sai.created_date", "datetime", "between", " and ", astrQueryId));

            }
            return lcolWhereClause;
        }

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busActivityInstance)
            {
                busActivityInstance lbusActivityInstance = (busActivityInstance)aobjBus;

                lbusActivityInstance.ibusActivity = new busActivity { icdoActivity = new cdoActivity() };
                lbusActivityInstance.ibusActivity.ibusRoles = new busRoles { icdoRoles = new cdoRoles() };

                if (!Convert.IsDBNull(adtrRow["Name"]))
                {
                    lbusActivityInstance.ibusActivity.icdoActivity.name = adtrRow["Name"].ToString();
                }
                if (!Convert.IsDBNull(adtrRow["Display_Name"]))
                {
                    lbusActivityInstance.ibusActivity.icdoActivity.display_name = adtrRow["Display_Name"].ToString();
                }

                if (!Convert.IsDBNull(adtrRow["PROCESS_ID"]))
                {
                    lbusActivityInstance.ibusActivity.icdoActivity.process_id = Convert.ToInt32(adtrRow["PROCESS_ID"]);
                }

                if (!Convert.IsDBNull(adtrRow["Role_Id"]))
                {
                    lbusActivityInstance.ibusActivity.icdoActivity.role_id = Convert.ToInt32(adtrRow["Role_Id"]);
                }

                 if (!Convert.IsDBNull(adtrRow["supervisor_role_id"]))
                {
                    lbusActivityInstance.ibusActivity.icdoActivity.supervisor_role_id = Convert.ToInt32(adtrRow["supervisor_role_id"]);
                }

                if (!Convert.IsDBNull(adtrRow["role_description"]))
                {
                    lbusActivityInstance.ibusActivity.ibusRoles.icdoRoles.role_description = adtrRow["role_description"].ToString();
                }

                lbusActivityInstance.ibusProcessInstance = new busProcessInstance { icdoProcessInstance = new cdoProcessInstance() };
                lbusActivityInstance.ibusProcessInstance.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusActivityInstance.ibusProcessInstance.icdoProcessInstance.process_instance_id = lbusActivityInstance.icdoActivityInstance.process_instance_id;
                lbusActivityInstance.ibusProcessInstance.ibusProcess = new busProcess { icdoProcess = new cdoProcess() };
                
                //lobjActivityInstance.ibusProcessInstance.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                
                lbusActivityInstance.ibusProcessInstance.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };

                lbusActivityInstance.ibusProcessInstance.ibusWorkflowRequest = new busWorkflowRequest { icdoWorkflowRequest = new cdoWorkflowRequest() };

                if (!Convert.IsDBNull(adtrRow["process_instance_id"]))
                {
                    lbusActivityInstance.ibusProcessInstance.icdoProcessInstance.process_instance_id = Convert.ToInt32(adtrRow["process_instance_id"]);
                }

                if (!Convert.IsDBNull(adtrRow["person_id"]))
                {
                    lbusActivityInstance.ibusProcessInstance.icdoProcessInstance.person_id = Convert.ToInt32(adtrRow["person_id"]);
                    lbusActivityInstance.ibusProcessInstance.ibusPerson.icdoPerson.person_id = Convert.ToInt32(adtrRow["person_id"]);
                    lbusActivityInstance.ibusProcessInstance.ibusPerson.icdoPerson.first_name = adtrRow["first_name"].ToString();
                    lbusActivityInstance.ibusProcessInstance.ibusPerson.icdoPerson.last_name = adtrRow["last_name"].ToString();
                    lbusActivityInstance.ibusProcessInstance.ibusPerson.icdoPerson.middle_name = adtrRow["middle_name"].ToString();
                }

                if (!Convert.IsDBNull(adtrRow["mpi_person_id"]))
                {
                    lbusActivityInstance.ibusProcessInstance.ibusPerson.icdoPerson.mpi_person_id = Convert.ToString(adtrRow["mpi_person_id"]);
                }

               // if (!Convert.IsDBNull(adtrRow["org_id"]))
               // {
                    //lobjActivityInstance.ibusProcessInstance.icdoProcessInstance.org_id = Convert.ToInt32(adtrRow["org_id"]);
                    //lobjActivityInstance.ibusProcessInstance.ibusOrganization.icdoOrganization.org_id = Convert.ToInt32(adtrRow["org_id"]);
                    //lobjActivityInstance.ibusProcessInstance.ibusOrganization.icdoOrganization.org_code = adtrRow["org_code"].ToString();
                    //lobjActivityInstance.ibusProcessInstance.ibusOrganization.icdoOrganization.org_name = adtrRow["org_name"].ToString();
               // }

                //if (!Convert.IsDBNull(adtrRow["contact_ticket_id"]))
                //{
                //    lobjActivityInstance.ibusProcessInstance.icdoProcessInstance.contact_ticket_id = Convert.ToInt32(adtrRow["contact_ticket_id"]);
                //}

                if (!Convert.IsDBNull(adtrRow["Process_Name"]))
                {
                    lbusActivityInstance.ibusProcessInstance.ibusProcess.icdoProcess.name = adtrRow["Process_Name"].ToString();
                }

                if (!Convert.IsDBNull(adtrRow["Process_Description"]))
                {
                    lbusActivityInstance.ibusProcessInstance.ibusProcess.icdoProcess.description = adtrRow["Process_Description"].ToString();
                }                
            }        
        }
    }
}
