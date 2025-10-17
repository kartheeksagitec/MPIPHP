#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busPir:
	/// Inherited from busPirGen, the class is used to customize the business object busPirGen.
	/// </summary>
	[Serializable]
	public class busPir : busPirGen
	{

        public Collection<busPirAttachment> iclbPirAttachment { get; set; }

        public Collection<busPirHistory> iclbPirHistory {get;set;}

        public busUser ibusAssignedTo {get;set;}
        
        public busUser ibusReportedBy {get;set;}

        //PIR Scout/Effort Hours Implementation
        public busPirEffortsHours ibusPirEffortsHours { get; set; }
        public decimal idecTotalPirEffortsHours { get; set; }
        public Collection<busPirEffortsHours> iclbPirEffortsHours { get; set; }

        public void LoadPir()
        {
            this.FindPir(icdoPir.pir_id);
        }

        public void LoadPirHistory()
        {
            DataTable ldtbList = Select<cdoPirHistory>(
                new string[1] { "pir_id" },
                new object[1] { icdoPir.pir_id }, null, "pir_history_id desc");
            iclbPirHistory = GetCollection<busPirHistory>(ldtbList, "icdoPirHistory");
            foreach (busPirHistory lbusPIRHistory in iclbPirHistory)
            {
                lbusPIRHistory.LoadAssignedTo();
            }
        }
        public void LoadAssignedTo()
        {
            if (ibusAssignedTo == null)
            {
                ibusAssignedTo = new busUser();
            }
            ibusAssignedTo.FindUser(icdoPir.assigned_to_id);
        }

        public void LoadReportedBy()
        {
            if (ibusReportedBy == null)
            {
                ibusReportedBy = new busUser();
            }
            ibusReportedBy.FindUser(icdoPir.reported_by_id);
        }

        public busUser ibusLoggedUser { get; set; }

        public void LoadLoggedUser()
        {
            if (ibusLoggedUser.IsNull())
                ibusLoggedUser = new busUser { icdoUser = new cdoUser() };
            ibusLoggedUser.FindUser(iobjPassInfo.iintUserSerialID);
        }


        public void LoadPirAttachment()
        {
            DataTable ldtbList = Select<cdoPirAttachment>(
                new string[1] { "pir_id" },
                new object[1] { icdoPir.pir_id }, null, "pir_attachment_id desc");
            iclbPirAttachment = GetCollection<busPirAttachment>(ldtbList, "icdoPirAttachment");
        }

        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            base.ValidateHardErrors(aenmPageMode);
            utlError lobjError = null;
            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();

            bool IsLoggedPIR = false;
            busUserRoles lbusUserRoles = new busUserRoles { icdoRoles = new cdoRoles() };
            if (lbusUserRoles.FindUserRoles(iobjPassInfo.iintUserSerialID, busConstant.Role.LOGGED_PIR))
            {
                IsLoggedPIR = true;
            }

            if (icdoPir.ihstOldValues.Count > 0)
            {
                if (Convert.ToString(icdoPir.ihstOldValues[enmPir.status_value.ToString()]).IsNotNullOrEmpty()
                      && (icdoPir.status_value != busConstant.PIR_STATUS_Logged &&
                      icdoPir.status_value != busConstant.PIR_STATUS_Deploy_to_Production)
                      && IsLoggedPIR && Convert.ToString(icdoPir.ihstOldValues[enmPir.status_value.ToString()]) != icdoPir.status_value)
                {
                    lobjError = AddError(6224, "");
                    this.iarrErrors.Add(lobjError);
                }
            }
            else
            {
                if ((icdoPir.status_value != busConstant.PIR_STATUS_Logged &&
                    icdoPir.status_value != busConstant.PIR_STATUS_Deploy_to_Production)
                    && IsLoggedPIR)
                {
                    lobjError = AddError(6224, "");
                    this.iarrErrors.Add(lobjError);
                }
            }

            //PIR Scout/Effort Hours Implementation
            if (icdoPir != null && icdoPir.severity_value != "ENHC" && icdoPir.severity_value != "SUPP" && icdoPir.severity_value != "NICT" && icdoPir.created_date.Year >= 2015)
            {
                lobjError = AddError(6270, "");
                this.iarrErrors.Add(lobjError);
            }
        }

        //PIR Scout/Effort Hours Implementation
        public override void BeforePersistChanges()
        {
            // For removing icdoPirEffortsHours object, bcoz null(one) entry goes to DB, when press btnSave button of main buspir not btnSave1 of icdoPirEffortsHours 
            if (ibusPirEffortsHours != null && ibusPirEffortsHours.icdoPirEffortsHours != null)
            {
                if (iarrChangeLog != null && iarrChangeLog.Contains(ibusPirEffortsHours.icdoPirEffortsHours))
                {
                    iarrChangeLog.Remove(ibusPirEffortsHours.icdoPirEffortsHours);
                }
            }
        }

        public override void AfterPersistChanges()
        {
            LoadPir();
            LoadPirHistory();
            base.AfterPersistChanges();
        }

        #region PIR Scout/Effort Hours Implementation
        //PIR Scout/Effort Hours Implementation
        public void LoadTotalPirEffortHours()
        {
            ibusPirEffortsHours = new busPirEffortsHours() { icdoPirEffortsHours = new cdoPirEffortsHours() };
            DataTable ldtbList = Select<cdoPirEffortsHours>(
                new string[1] { "pir_id" },
                new object[1] { icdoPir.pir_id }, null, "PIR_EFFORTS_HOURS_ID desc");
            iclbPirEffortsHours = GetCollection<busPirEffortsHours>(ldtbList, "icdoPirEffortsHours");           
            idecTotalPirEffortsHours = iclbPirEffortsHours.Sum(lbusPirEffortsHours => lbusPirEffortsHours.icdoPirEffortsHours.effort_hours);            
        }

        public void LoadPirEffortHours()
        {
            ibusPirEffortsHours = new busPirEffortsHours() { icdoPirEffortsHours = new cdoPirEffortsHours() };
            DataTable ldtbList = Select<cdoPirEffortsHours>(
                new string[1] { "pir_id" },
                new object[1] { icdoPir.pir_id }, null, "PIR_EFFORTS_HOURS_ID desc");
            iclbPirEffortsHours = GetCollection<busPirEffortsHours>(ldtbList, "icdoPirEffortsHours");
            foreach (busPirEffortsHours lbusPirEffortsHours in iclbPirEffortsHours)
            {
                DataTable ldtbUserList = Select<cdoUser>(
                    new string[1] { "user_serial_id" },
                    new object[1] { lbusPirEffortsHours.icdoPirEffortsHours.user_id }, null, null);
                //PIR Scout/Effort Hours Implementation
                if (ldtbUserList != null && ldtbUserList.Rows.Count > 0)
                    lbusPirEffortsHours.istrUser = ldtbUserList.Rows[0]["user_id"].ToString();
            }
            idecTotalPirEffortsHours = iclbPirEffortsHours.Sum(lbusPirEffortsHours => lbusPirEffortsHours.icdoPirEffortsHours.effort_hours);
            ibusPirEffortsHours.icdoPirEffortsHours.effort_date = DateTime.Now;
        }

        public ArrayList SavePIREffortErrors()
        {
            ArrayList larrReturn = new ArrayList();            
            utlError lobjError = null;
            //PIR Scout/Effort Hours Implementation
            //if (ibusPirEffortsHours.icdoPirEffortsHours.effort_hours == 0 || ibusPirEffortsHours.icdoPirEffortsHours.effort_description.IsNullOrEmpty() ||
            //    ibusPirEffortsHours.icdoPirEffortsHours.user_id.IsNullOrEmpty() || ibusPirEffortsHours.icdoPirEffortsHours.task_code_value.IsNullOrEmpty() ||
            //    ibusPirEffortsHours.icdoPirEffortsHours.effort_date == DateTime.MinValue)
            if (ibusPirEffortsHours.icdoPirEffortsHours.effort_hours == 0)
            {
                lobjError = AddError(busConstant.MessageEffortHours, string.Empty);
                larrReturn.Add(lobjError);
            }
            if (ibusPirEffortsHours.icdoPirEffortsHours.effort_description.IsNullOrEmpty())
            {
                lobjError = AddError(busConstant.MessageEffortDescription, string.Empty);
                larrReturn.Add(lobjError);
            }
            if (ibusPirEffortsHours.icdoPirEffortsHours.user_id.IsNullOrEmpty() || ibusPirEffortsHours.icdoPirEffortsHours.user_id == "0")
            {
                lobjError = AddError(busConstant.MessageUserId, string.Empty);
                larrReturn.Add(lobjError);
            }
            if (ibusPirEffortsHours.icdoPirEffortsHours.task_code_value.IsNullOrEmpty())
            {
                lobjError = AddError(busConstant.MessageTaskRequire, string.Empty);
                larrReturn.Add(lobjError);
            }
            if (ibusPirEffortsHours.icdoPirEffortsHours.effort_date == DateTime.MinValue)
            {
                lobjError = AddError(busConstant.MessageEffortDate, string.Empty);
                larrReturn.Add(lobjError);
            }
            if (ibusPirEffortsHours.icdoPirEffortsHours.effort_date > DateTime.Now)
            {
                lobjError = AddError(busConstant.MessageEffortDateCannotBeFutureDate, string.Empty);
                larrReturn.Add(lobjError);
            }
            // check error count and insert record
            if (larrReturn.Count == 0)
            {
                ibusPirEffortsHours.icdoPirEffortsHours.pir_id = this.icdoPir.pir_id;
                ibusPirEffortsHours.icdoPirEffortsHours.task_code_id = busConstant.PirTaskCodeId;                
                ibusPirEffortsHours.istrUser = LoadUser(ibusPirEffortsHours.icdoPirEffortsHours.user_id);      
                ibusPirEffortsHours.icdoPirEffortsHours.Insert();
                //PIR Scout/Effort Hours Implementation
                if (iclbPirEffortsHours.IsNull())
                    iclbPirEffortsHours = new Collection<busPirEffortsHours>();
                iclbPirEffortsHours.Add(ibusPirEffortsHours);
                larrReturn.Add(this);
                idecTotalPirEffortsHours = iclbPirEffortsHours.Sum(lbusPirEffortsHours => lbusPirEffortsHours.icdoPirEffortsHours.effort_hours);
                this.ibusPirEffortsHours = new busPirEffortsHours() { icdoPirEffortsHours = new cdoPirEffortsHours() };
                this.ibusPirEffortsHours.icdoPirEffortsHours.effort_date = DateTime.Now;
            }
            return larrReturn;
        }

        public string LoadUser(string aintUserId)
        {
            DataTable ldtbUserList = Select<cdoUser>(
                    new string[1] { "user_serial_id" },
                    new object[1] { aintUserId }, null, null);
            return ldtbUserList.Rows[0]["user_id"].ToString();
        }
        #endregion
    }
}
