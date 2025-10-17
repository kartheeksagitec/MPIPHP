#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using MPIPHP.DataObjects;
using System.Data;
using Sagitec.DBUtility;
using MPIPHP.BusinessObjects;

#endregion

namespace MPIPHP.CustomDataObjects
{
    /// <summary>
    /// Class MPIPHP.CustomDataObjects.cdoProcessInstance:
    /// Inherited from doProcessInstance, the class is used to customize the database object doProcessInstance.
    /// </summary>
    [Serializable]
    public class cdoProcessInstance : doProcessInstance
    {
        public cdoProcessInstance()
            : base()
        {
        }

        public bool FindByInstanceID(Guid aInstanceID)
        {
            DataTable ldtbResult = DBFunction.DBSelect("select * from sgw_process_instance where workflow_instance_guid = '" + aInstanceID.ToString() + "'", new System.Collections.ObjectModel.Collection<Sagitec.Common.utlWhereClause>(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            if (ldtbResult != null && ldtbResult.Rows.Count == 1)
            {
                LoadData(ldtbResult.Rows[0]);
                return true;
            }
            return false;
        }
        public bool FindByInstanceID(Guid aInstanceID, IDbConnection idbConnection)
        {
            DataTable ldtbResult = DBFunction.DBSelect("select * from sgw_process_instance where workflow_instance_guid = '" + aInstanceID.ToString() + "'", idbConnection);
            if (ldtbResult != null && ldtbResult.Rows.Count == 1)
            {
                LoadData(ldtbResult.Rows[0]);
                return true;
            }
            return false;
        }

        //public override int Insert()
        //{
        //    int lintResult = base.Insert();

        //    //Sometimes Workflow Created from Contact Ticket. Such cases we dont need to
        //    //we dont need to create contact ticket.
        //    if (contact_ticket_id == 0)
        //    {
        //        cdoContactTicket lcdoContactTicket = new cdoContactTicket();
        //        lcdoContactTicket.contact_type_value = "WKRF";
        //        lcdoContactTicket.person_id = person_id;
        //        lcdoContactTicket.org_id = org_id;
        //        lcdoContactTicket.status_value = "OPEN";
        //        lcdoContactTicket.Insert();

        //        //Update the Contact Ticket ID in Process Instance Table
        //        contact_ticket_id = lcdoContactTicket.contact_ticket_id;
        //        Update();
        //    }
        //    return lintResult;
        //}

        //public override int Update()
        //{
        //    int lintResult = base.Update();
        //    if ((status_value == busConstant.ProcessInstanceStatusAborted) ||
        //        (status_value == busConstant.ProcessInstanceStatusProcessed) ||
        //        (status_value == busConstant.ProcessInstanceStatusTerminated))
        //    {
        //        //If the Contact Ticket Workflow Flag is Set, Update the Workflow Instance ID in the Same Ticket
        //        //instead of creating New Ticket
        //        if (contact_ticket_id > 0)
        //        {
        //            cdoContactTicket lcdoContactTicket = new cdoContactTicket();
        //            if (lcdoContactTicket.SelectRow(new object[1] { contact_ticket_id }))
        //            {
        //                //For Contact Ticket Related worklfow.. it closes the instance as soon ticket gets closed.
        //                //We dont need to close it again.
        //                if (lcdoContactTicket.status_value != busConstant.ContactTicketStatusClosed)
        //                {
        //                    lcdoContactTicket.status_value = "CLOS";
        //                    lcdoContactTicket.NeedHistory = true;
        //                    lcdoContactTicket.Update();
        //                }
        //            }
        //        }
        //    }
        //    return lintResult;
        //}
    }
}
