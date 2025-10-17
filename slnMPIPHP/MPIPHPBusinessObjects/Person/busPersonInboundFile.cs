

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using System.Linq;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
using System.Data.SqlClient;
using Sagitec.DataObjects;
using System.Globalization;


namespace MPIPHP.BusinessObjects.Person
{
    [Serializable]
    public class busPersonInboundFile : busFileBase
    {

        public Collection<busDeathNotification> iclbPersonDeathNotificationForUpdate { get; set; }
        public Collection<busDeathNotification> iclbNotificationInitiateWorkflow { get; set; }
        public static DataTable ldtbPersonData { get; set; }
        public busDeathNotification ibusDeathNotification { get; set; }

        public Collection<busPerson> iclbPerson { get; set; }

        #region Constructors
        public busPersonInboundFile()
            : base()
        {
            iclbNotificationInitiateWorkflow = new Collection<busDeathNotification>();
            iclbPersonDeathNotificationForUpdate = new Collection<busDeathNotification>();
            iclbPerson = new Collection<busPerson>();
        }
        #endregion

        #region overriden methods.
        //PIR 806

        public override void InitializeFile()
        {
            base.InitializeFile();
            CreateTableDesignForPersonReport();

        }

        public override bool IgnoreRecord()
        {

            if (icdoFileDtl.record_data.StartsWith("C", true, System.Globalization.CultureInfo.CurrentCulture))
            {
                return true;
            }
            return false;
        }


        public override busBase NewHeader()
        {
            return base.NewHeader();
        }

        public override busBase NewDetail()
        {
            ibusDeathNotification = new busDeathNotification { icdoDeathNotification = new cdoDeathNotification() };
            ibusDeathNotification.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            return ibusDeathNotification;
        }


        public override string BeforeFieldAssigned(string astrFieldName, string astrFieldValue)
        {
            string lstrReturnValue = astrFieldValue;
            return lstrReturnValue;
        }

        public override bool ValidateFile()
        {
            return base.ValidateFile();


        }

        public override void ProcessDetail()
        {
            DateTime dateValue = DateTime.MinValue;
            string lstrMPIID = string.Empty;

            if (ibusDeathNotification.ibusPerson.icdoPerson.mpi_person_id.IsNotNullOrEmpty())
            {
                lstrMPIID = ibusDeathNotification.ibusPerson.icdoPerson.mpi_person_id; //icdoFileDtl.record_data.Substring(0, 19).Trim();
            }

            string lstrDateofDeath = string.Empty;
            if (ibusDeathNotification.icdoDeathNotification.istrDateofDeath.IsNotNullOrEmpty())
            {
                lstrDateofDeath = ibusDeathNotification.icdoDeathNotification.istrDateofDeath.ToString();//icdoFileDtl.record_data.Substring(142, 10);
            }
            else
            {
                return;
            }


            if (!DateTime.TryParse(lstrDateofDeath, out dateValue))
            {
                lstrDateofDeath = Convert.ToString(DateTime.MinValue);
            }

            string lstrSSN = string.Empty;
            if (ibusDeathNotification.ibusPerson.icdoPerson.ssn.IsNotNullOrEmpty())
            {
                lstrSSN = ibusDeathNotification.ibusPerson.icdoPerson.ssn;//icdoFileDtl.record_data.Substring(89, 9).Trim();
            }

            if (!lstrSSN.IsNullOrEmpty())
                lstrSSN = lstrSSN.PadLeft(9, '0');

            string lstrDateofBirth = string.Empty;
            if (ibusDeathNotification.ibusPerson.icdoPerson.idtDateofBirth != DateTime.MinValue)
            {
                lstrDateofBirth = ibusDeathNotification.ibusPerson.icdoPerson.idtDateofBirth.ToString();//icdoFileDtl.record_data.Substring(132, 10).Trim();//PIR 806
            }

            if (!DateTime.TryParse(lstrDateofBirth, out dateValue))
            {
                lstrDateofBirth = Convert.ToString(DateTime.MinValue);
            }

            string lstrPersonType = string.Empty;
            if (ibusDeathNotification.icdoDeathNotification.istrPersonType.IsNotNullOrEmpty())
            {
                lstrPersonType = ibusDeathNotification.icdoDeathNotification.istrPersonType;//icdoFileDtl.record_data.Substring(20, 20).Trim();//PIR 806
            }

            if (ibusDeathNotification.ibusPerson.icdoPerson.last_name.IsNullOrEmpty())
            {
                ibusDeathNotification.ibusPerson.icdoPerson.last_name = string.Empty;
            }

            if (ibusDeathNotification.ibusPerson.icdoPerson.first_name.IsNullOrEmpty())
            {
                ibusDeathNotification.ibusPerson.icdoPerson.first_name = string.Empty;
            }
            string lstrPersonName = ibusDeathNotification.ibusPerson.icdoPerson.last_name.Trim() + " " + ibusDeathNotification.ibusPerson.icdoPerson.first_name.Trim();

            busPerson lbusPerson = new busPerson();


            //Tushar-806-If SSN and DOB matches with OPUS then only process
            bool IsPersonExists = lbusPerson.FindPerson(lbusPerson.GetPersonIDFromSSN(lstrSSN));
            if (IsPersonExists && lstrDateofBirth.IsNotNullOrEmpty() && lbusPerson.icdoPerson.date_of_birth != DateTime.MinValue && lbusPerson.icdoPerson.date_of_birth == Convert.ToDateTime(lstrDateofBirth))
            {

                if (lstrDateofDeath.IsNotNull())
                {
                    if (lstrMPIID.IsNullOrEmpty())
                    {
                        if (lbusPerson.icdoPerson.mpi_person_id.IsNotNullOrEmpty())
                        {
                            ibusDeathNotification = ibusDeathNotification.LoadDetailsForInboundFile(lbusPerson.icdoPerson.mpi_person_id);
                            //Tushar-806- Add Persontype property to collection
                            ibusDeathNotification.icdoDeathNotification.istrPersonType = lstrPersonType;
                            ibusDeathNotification.icdoDeathNotification.istrMpiPersonId = lstrMPIID;
                            ibusDeathNotification.icdoDeathNotification.istrName = lstrPersonName;
                            if (lbusPerson.CheckMemberIsRetiree())
                                ibusDeathNotification.icdoDeathNotification.istrStatus = "RETIREE";
                            else
                                ibusDeathNotification.icdoDeathNotification.istrStatus = "ACTIVE";
                            ibusDeathNotification.icdoDeathNotification.idtDateOfBirth = Convert.ToDateTime(lstrDateofBirth);

                        }
                    }
                    else
                    {
                        lbusPerson.FindPerson(lstrMPIID);
                        ibusDeathNotification = ibusDeathNotification.LoadDetailsForInboundFile(lstrMPIID);
                        //Tushar-806- Add Persontype property to collection
                        ibusDeathNotification.icdoDeathNotification.istrPersonType = lstrPersonType;
                        ibusDeathNotification.icdoDeathNotification.istrMpiPersonId = lstrMPIID;
                        ibusDeathNotification.icdoDeathNotification.istrName = lstrPersonName;
                        if (lbusPerson.CheckMemberIsRetiree())
                            ibusDeathNotification.icdoDeathNotification.istrStatus = "RETIREE";
                        else
                            ibusDeathNotification.icdoDeathNotification.istrStatus = "ACTIVE";
                        ibusDeathNotification.icdoDeathNotification.idtDateOfBirth = Convert.ToDateTime(lstrDateofBirth);

                    }



                    if (ibusDeathNotification.icdoDeathNotification.IsNotNull() &&
                        (ibusDeathNotification.icdoDeathNotification.death_notification_status_value == busConstant.NOTIFICATION_STATUS_IN_PROGRESS
                            || ibusDeathNotification.icdoDeathNotification.death_notification_status_value == busConstant.NOTIFICATION_STATUS_CERTIFIED))
                    {

                        lbusPerson.FindPerson(lstrMPIID);
                        ibusDeathNotification = ibusDeathNotification.LoadDetailsForInboundFile(lstrMPIID);
                        ibusDeathNotification.icdoDeathNotification.istrPersonType = lstrPersonType;
                        ibusDeathNotification.icdoDeathNotification.istrMpiPersonId = lstrMPIID;
                        ibusDeathNotification.icdoDeathNotification.istrName = lstrPersonName;
                        if (lbusPerson.CheckMemberIsRetiree())
                            ibusDeathNotification.icdoDeathNotification.istrStatus = "RETIREE";
                        else
                            ibusDeathNotification.icdoDeathNotification.istrStatus = "ACTIVE";
                        ibusDeathNotification.icdoDeathNotification.idtDateOfBirth = lbusPerson.icdoPerson.date_of_birth;

                        ibusDeathNotification.icdoDeathNotification.date_of_death = Convert.ToString(lstrDateofDeath).IsNotNullOrEmpty() ?
                        Convert.ToDateTime(lstrDateofDeath) : Convert.ToDateTime("01/01/0001");

                        //PIR RID 56892
                        ibusDeathNotification.icdoDeathNotification.istrMPIVested = (lbusPerson.CheckAlreadyVested(busConstant.MPIPP) ? "Y" : "N");

                        DataRow ldrNewRow = FillData(ibusDeathNotification, "Not Proccesed", "Death Notification already exists");
                        if (ldrNewRow.IsNotNull())
                            ldtbPersonData.Rows.Add(ldrNewRow);

                    }
                    else
                    {
                        if (ibusDeathNotification.icdoDeathNotification.IsNotNull() &&
                        (ibusDeathNotification.icdoDeathNotification.death_notification_status_value == busConstant.NOTIFICATION_STATUS_NOT_DECEASED
                         || ibusDeathNotification.icdoDeathNotification.death_notification_status_value == busConstant.NOTIFICATION_STATUS_INCORRECTLY_REPORTED))
                        {
                            if (ibusDeathNotification.icdoDeathNotification.date_of_death != Convert.ToDateTime(lstrDateofDeath))
                            {
                                ibusDeathNotification.icdoDeathNotification.person_id = lbusPerson.icdoPerson.person_id;
                                ibusDeathNotification.icdoDeathNotification.date_of_death = Convert.ToDateTime(lstrDateofDeath);
                                iclbPersonDeathNotificationForUpdate.Add(ibusDeathNotification);
                            }
                            else
                            {
                                lbusPerson.FindPerson(lstrMPIID);
                                ibusDeathNotification = ibusDeathNotification.LoadDetailsForInboundFile(lstrMPIID);
                                ibusDeathNotification.icdoDeathNotification.istrPersonType = lstrPersonType;
                                ibusDeathNotification.icdoDeathNotification.istrMpiPersonId = lstrMPIID;
                                ibusDeathNotification.icdoDeathNotification.istrName = lstrPersonName;
                                if (lbusPerson.CheckMemberIsRetiree())
                                    ibusDeathNotification.icdoDeathNotification.istrStatus = "RETIREE";
                                else
                                    ibusDeathNotification.icdoDeathNotification.istrStatus = "ACTIVE";
                                ibusDeathNotification.icdoDeathNotification.idtDateOfBirth = lbusPerson.icdoPerson.date_of_birth;

                                ibusDeathNotification.icdoDeathNotification.date_of_death = Convert.ToString(lstrDateofDeath).IsNotNullOrEmpty() ?
                                    Convert.ToDateTime(lstrDateofDeath) : Convert.ToDateTime("01/01/0001");

                                //PIR RID 56892
                                ibusDeathNotification.icdoDeathNotification.istrMPIVested = (lbusPerson.CheckAlreadyVested(busConstant.MPIPP) ? "Y" : "N");

                                DataRow ldrNewRow = FillData(ibusDeathNotification, "NOT PPROCESSED", "Incorrectly Reported/ Not deceased Death Notification already exists with same date of death");
                                if (ldrNewRow.IsNotNull())
                                    ldtbPersonData.Rows.Add(ldrNewRow);
                            }
                        }
                        else
                        {
                            ibusDeathNotification.icdoDeathNotification.person_id = lbusPerson.icdoPerson.person_id;
                            ibusDeathNotification.icdoDeathNotification.date_of_death = Convert.ToDateTime(lstrDateofDeath);
                            iclbPersonDeathNotificationForUpdate.Add(ibusDeathNotification);
                        }

                        if (lbusPerson.icdoPerson.person_id != 0 && lbusPerson.icdoPerson.person_id.IsNotNull())
                        {
                            if (iclbPersonDeathNotificationForUpdate != null && iclbPersonDeathNotificationForUpdate.Count() > 0
                                && iclbPersonDeathNotificationForUpdate.Where(t => t.icdoDeathNotification.person_id == lbusPerson.icdoPerson.person_id).Count() > 1)
                            {
                                if (iclbPersonDeathNotificationForUpdate.
                                    Where(t => t.icdoDeathNotification.person_id == lbusPerson.icdoPerson.person_id && t.icdoDeathNotification.date_of_death == Convert.ToDateTime(lstrDateofDeath)).Count()
                                    == iclbPersonDeathNotificationForUpdate.Where(t => t.icdoDeathNotification.person_id == lbusPerson.icdoPerson.person_id).Count())
                                {
                                    lbusPerson.FindPerson(lstrMPIID);
                                    ibusDeathNotification = ibusDeathNotification.LoadDetailsForInboundFile(lstrMPIID);
                                    ibusDeathNotification.icdoDeathNotification.istrPersonType = lstrPersonType;
                                    ibusDeathNotification.icdoDeathNotification.istrMpiPersonId = lstrMPIID;
                                    ibusDeathNotification.icdoDeathNotification.istrName = lstrPersonName;
                                    if (lbusPerson.CheckMemberIsRetiree())
                                        ibusDeathNotification.icdoDeathNotification.istrStatus = "RETIREE";
                                    else
                                        ibusDeathNotification.icdoDeathNotification.istrStatus = "ACTIVE";
                                    ibusDeathNotification.icdoDeathNotification.idtDateOfBirth = lbusPerson.icdoPerson.date_of_birth;
                                    ibusDeathNotification.icdoDeathNotification.date_of_death = Convert.ToString(lstrDateofDeath).IsNotNullOrEmpty() ?
                                    Convert.ToDateTime(lstrDateofDeath) : Convert.ToDateTime("01/01/0001");

                                    //PIR RID 56892
                                    ibusDeathNotification.icdoDeathNotification.istrMPIVested = (lbusPerson.CheckAlreadyVested(busConstant.MPIPP) ? "Y" : "N");

                                    DataRow ldrNewRow = FillData(ibusDeathNotification, "REJECTED", "DUPLICATE RECORD.RECORD WITH SAME INFO HAS ALREADY BEEN INSERTED.");
                                    if (ldrNewRow.IsNotNull())
                                        ldtbPersonData.Rows.Add(ldrNewRow);
                                }
                                else
                                {

                                    if (ldtbPersonData != null && ldtbPersonData.Rows.Count > 0 &&
                                            ldtbPersonData.Rows.Cast<DataRow>().Where(r => r.Field<string>("MPI_PERSON_ID") == lbusPerson.icdoPerson.mpi_person_id).Count() > 0)
                                    {
                                        ldtbPersonData.Rows.Cast<DataRow>().Where(r => r.Field<string>("MPI_PERSON_ID") == lbusPerson.icdoPerson.mpi_person_id).ToList().ForEach(r1 => r1.Delete());
                                        ldtbPersonData.AcceptChanges();
                                    }

                                    foreach (busDeathNotification lbusUpdateDeathNotification in iclbPersonDeathNotificationForUpdate.Where(t => t.icdoDeathNotification.person_id == lbusPerson.icdoPerson.person_id))
                                    {
                                        lbusUpdateDeathNotification.icdoDeathNotification.istrPersonType = lstrPersonType;
                                        lbusUpdateDeathNotification.icdoDeathNotification.istrMpiPersonId = lstrMPIID;
                                        lbusUpdateDeathNotification.icdoDeathNotification.istrName = lstrPersonName;
                                        if (lbusPerson.CheckMemberIsRetiree())
                                            lbusUpdateDeathNotification.icdoDeathNotification.istrStatus = "RETIREE";
                                        else
                                            lbusUpdateDeathNotification.icdoDeathNotification.istrStatus = "ACTIVE";
                                        lbusUpdateDeathNotification.icdoDeathNotification.idtDateOfBirth = lbusPerson.icdoPerson.date_of_birth;

                                        //PIR RID 56892
                                        lbusUpdateDeathNotification.icdoDeathNotification.istrMPIVested = (lbusPerson.CheckAlreadyVested(busConstant.MPIPP) ? "Y" : "N");

                                        DataRow ldrNewRow = FillData(lbusUpdateDeathNotification, "REVIEW", "Duplicate Entries.Different Date Of Deaths");
                                        if (ldrNewRow.IsNotNull())
                                            ldtbPersonData.Rows.Add(ldrNewRow);
                                    }

                                    if (iclbNotificationInitiateWorkflow != null && iclbNotificationInitiateWorkflow.Where(t => t.icdoDeathNotification.person_id == lbusPerson.icdoPerson.person_id).Count() > 0)
                                    {
                                        foreach (var s in iclbNotificationInitiateWorkflow.Where(t => t.icdoDeathNotification.person_id == lbusPerson.icdoPerson.person_id).ToList())
                                        {
                                            iclbNotificationInitiateWorkflow.Remove(s);
                                        }
                                    }


                                }
                            }
                            else
                            {

                                if (ibusDeathNotification.icdoDeathNotification.IsNotNull() &&
                                        (ibusDeathNotification.icdoDeathNotification.death_notification_status_value == busConstant.NOTIFICATION_STATUS_NOT_DECEASED
                                    || ibusDeathNotification.icdoDeathNotification.death_notification_status_value == busConstant.NOTIFICATION_STATUS_INCORRECTLY_REPORTED))
                                {
                                    if (iclbPersonDeathNotificationForUpdate != null
                                    && iclbPersonDeathNotificationForUpdate.Where(t => t.icdoDeathNotification.person_id == lbusPerson.icdoPerson.person_id).Count() > 0)
                                    {
                                        ibusDeathNotification.icdoDeathNotification.person_id = lbusPerson.icdoPerson.person_id;
                                        ibusDeathNotification.icdoDeathNotification.date_of_death = Convert.ToDateTime(lstrDateofDeath);
                                        iclbNotificationInitiateWorkflow.Add(ibusDeathNotification);
                                    }
                                }
                                else
                                {
                                    ibusDeathNotification.icdoDeathNotification.person_id = lbusPerson.icdoPerson.person_id;
                                    ibusDeathNotification.icdoDeathNotification.date_of_death = Convert.ToDateTime(lstrDateofDeath);
                                    iclbNotificationInitiateWorkflow.Add(ibusDeathNotification);
                                }

                            }
                        }
                    }
                }
                else
                {

                    lbusPerson.FindPerson(lstrMPIID);
                    ibusDeathNotification = ibusDeathNotification.LoadDetailsForInboundFile(lstrMPIID);
                    ibusDeathNotification.icdoDeathNotification.istrPersonType = lstrPersonType;
                    ibusDeathNotification.icdoDeathNotification.istrMpiPersonId = lstrMPIID;
                    ibusDeathNotification.icdoDeathNotification.istrName = lstrPersonName;
                    if (lbusPerson.CheckMemberIsRetiree())
                        ibusDeathNotification.icdoDeathNotification.istrStatus = "RETIREE";
                    else
                        ibusDeathNotification.icdoDeathNotification.istrStatus = "ACTIVE";
                    ibusDeathNotification.icdoDeathNotification.idtDateOfBirth = lbusPerson.icdoPerson.date_of_birth;

                    //PIR RID 56892
                    ibusDeathNotification.icdoDeathNotification.istrMPIVested = (lbusPerson.CheckAlreadyVested(busConstant.MPIPP) ? "Y" : "N");

                    DataRow ldrNewRow = FillData(ibusDeathNotification, "REJECTED", "No date of Death");
                    if (ldrNewRow.IsNotNull())
                        ldtbPersonData.Rows.Add(ldrNewRow);
                }
                //return ibusDeathNotification;

            }
            else //For Rejected rechords fill the collection      
            {
                lbusPerson.FindPerson(lstrMPIID);
                ibusDeathNotification = ibusDeathNotification.LoadDetailsForInboundFile(lstrMPIID);
                ibusDeathNotification.icdoDeathNotification.istrPersonType = lstrPersonType;
                ibusDeathNotification.icdoDeathNotification.istrMpiPersonId = lstrMPIID;
                ibusDeathNotification.icdoDeathNotification.istrName = lstrPersonName;
                if (lbusPerson.CheckMemberIsRetiree())
                    ibusDeathNotification.icdoDeathNotification.istrStatus = "RETIREE";
                else
                    ibusDeathNotification.icdoDeathNotification.istrStatus = "ACTIVE";
                ibusDeathNotification.icdoDeathNotification.idtDateOfBirth = Convert.ToString(lstrDateofBirth).IsNotNullOrEmpty() ?
                    Convert.ToDateTime(lstrDateofBirth) : Convert.ToDateTime("01/01/0001");

                ibusDeathNotification.icdoDeathNotification.date_of_death = Convert.ToString(lstrDateofDeath).IsNotNullOrEmpty() ?
                   Convert.ToDateTime(lstrDateofDeath) : Convert.ToDateTime("01/01/0001");

                //PIR RID 56892
                ibusDeathNotification.icdoDeathNotification.istrMPIVested = (lbusPerson.CheckAlreadyVested(busConstant.MPIPP) ? "Y" : "N");

                DataRow ldrNewRow = FillData(ibusDeathNotification, "REJECTED", "SSN and DOB Not Matching");
                if (ldrNewRow.IsNotNull())
                    ldtbPersonData.Rows.Add(ldrNewRow);
            }


            if (iclbNotificationInitiateWorkflow != null && iclbNotificationInitiateWorkflow.Count() > 0)
            {
                foreach (busDeathNotification lbusDeathNotification in iclbNotificationInitiateWorkflow)
                {
                    if (iclbPerson.Where(t => t.icdoPerson.person_id == lbusDeathNotification.icdoDeathNotification.person_id).Count() == 0)
                    {
                        cdoDeathNotification lcdoDeathNotification = new cdoDeathNotification();
                        lcdoDeathNotification.person_id = lbusDeathNotification.icdoDeathNotification.person_id;
                        lcdoDeathNotification.death_notification_received_date = DateTime.Now;
                        lcdoDeathNotification.death_notification_status_value = busConstant.NOTIFICATION_STATUS_IN_PROGRESS;
                        lcdoDeathNotification.date_of_death = lbusDeathNotification.icdoDeathNotification.date_of_death;
                        lcdoDeathNotification.created_by = busConstant.DEATH_MATCH_BATCH;
                        lcdoDeathNotification.created_date = DateTime.Now;
                        lcdoDeathNotification.modified_by = busConstant.DEATH_MATCH_BATCH;
                        lcdoDeathNotification.modified_date = DateTime.Now;

                        iclbPerson.Add(lbusPerson);

                        ////PIR-806
                        //utlPassInfo lobjPassInfo = new utlPassInfo();
                        //lobjPassInfo.idictParams["ID"] = "CurrentDB_Integration";
                        //lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                        ////lobjPassInfo1.iconFramework = DBFunction.GetDBConnection("core");
                        //utlPassInfo.iobjPassInfo = lobjPassInfo;

                        try
                        {
                            // PIR-806                  
                            //iobjPassInfo.BeginTransaction();

                            lcdoDeathNotification.Insert();
                            //806- Tushar (Condition- initiate workflow for Participant only)
                            if (lbusDeathNotification.icdoDeathNotification.istrPersonType == "PARTICIPANT" || (iclbPersonDeathNotificationForUpdate != null &&
                                iclbPersonDeathNotificationForUpdate.Where(t => t.icdoDeathNotification.person_id == lbusDeathNotification.icdoDeathNotification.person_id &&
                                t.icdoDeathNotification.istrPersonType == "PARTICIPANT").Count() > 0))
                            {
                                busWorkflowHelper.InitializeWorkflowIfNotExists(busConstant.DEATH_NOTIFICATION_WORKFLOW_NAME, lbusDeathNotification.icdoDeathNotification.person_id, 0, lcdoDeathNotification.death_notification_id, null);//PIR 848
                            }
                            lbusDeathNotification.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                            lbusDeathNotification.ibusPerson.FindPerson(lbusDeathNotification.icdoDeathNotification.person_id);

                            //PIR 806
                            lbusDeathNotification.ibusPerson.icdoPerson.date_of_death = lbusDeathNotification.icdoDeathNotification.date_of_death;
                            lbusDeathNotification.ibusPerson.icdoPerson.Update();

                            //RID 
                            DataTable ldtbPayeeAccountIDs = busBase.Select("cdoPayeeAccount.GetPayeeAccountForSuspensionDeathNotification", new object[1] { lbusDeathNotification.icdoDeathNotification.person_id });
                            if (ldtbPayeeAccountIDs.Rows.Count > 0)
                            {
                                foreach (DataRow ldrPayeeAccountId in ldtbPayeeAccountIDs.Rows)
                                {
                                    cdoPayeeAccountStatus lcdoPayeeAccountStatus = new cdoPayeeAccountStatus();
                                    lcdoPayeeAccountStatus.payee_account_id = Convert.ToInt32(ldrPayeeAccountId["PAYEE_ACCOUNT_ID"]);
                                    lcdoPayeeAccountStatus.status_value = busConstant.PAYEE_ACCOUNT_STATUS_SUSPENDED;
                                    lcdoPayeeAccountStatus.terminated_status_reason_value = busConstant.PayeeAccountTerminationReasonDeath;
                                    lcdoPayeeAccountStatus.status_effective_date = DateTime.Now;
                                    lcdoPayeeAccountStatus.Insert();
                                }
                                //152801
                                DataTable ldtbAlternatePayeeAccountIDs = busBase.Select("cdoPayeeAccount.GetAlternatePayeeAccountForSuspensionDeathNotification", new object[1] { lbusDeathNotification.icdoDeathNotification.person_id });
                                if (ldtbAlternatePayeeAccountIDs.Rows.Count > 0)
                                {

                                    foreach (DataRow ldrAlternatePayeeAccountId in ldtbAlternatePayeeAccountIDs.Rows)
                                    {
                                        cdoPayeeAccountStatus lcdoPayeeAccountStatus = new cdoPayeeAccountStatus();
                                        lcdoPayeeAccountStatus.payee_account_id = Convert.ToInt32(ldrAlternatePayeeAccountId["PAYEE_ACCOUNT_ID"]);
                                        lcdoPayeeAccountStatus.status_value = busConstant.PAYEE_ACCOUNT_STATUS_SUSPENDED;
                                        lcdoPayeeAccountStatus.suspension_status_reason_value = "OTHR";
                                        lcdoPayeeAccountStatus.suspension_reason_description = "Participant's Death";
                                        lcdoPayeeAccountStatus.status_effective_date = DateTime.Now;
                                        lcdoPayeeAccountStatus.Insert();

                                    }
                                }

                                busWorkflowHelper.InitializeWorkflowIfNotExists(busConstant.DEATH_NOTIFICATION_WORKFLOW_NAME, lbusDeathNotification.icdoDeathNotification.person_id, 0, lcdoDeathNotification.death_notification_id, null);//PIR 848
                            }

                            //PIR 806 UnComment //Temporary Commented waiting for confimration from eligibility
                            // decommissioning demographics informations, since HEDB is retiring.
                            //  lbusDeathNotification.UpdateHEDB(busConstant.POTENTIAL_DEATH_CERTIFICATION);

                            //PIR RID 56892
                            lbusDeathNotification.icdoDeathNotification.istrMPIVested = (lbusPerson.CheckAlreadyVested(busConstant.MPIPP) ? "Y" : "N");

                            //Ticket#107455
                            var istrMessage = string.Empty;
                            DataTable ldtbBeneficiaryBenefitOption = busBase.Select("cdoBenefitApplication.GetBeneficiaryBenefitOptionByPersonId", new object[1] { lbusDeathNotification.icdoDeathNotification.person_id });
                           
                            if (ldtbBeneficiaryBenefitOption != null && ldtbBeneficiaryBenefitOption.Rows.Count > 0)
                            {
                                istrMessage = Convert.ToString(ldtbBeneficiaryBenefitOption.Rows[0]["BENEFIT_OPTION_VALUE"]);

                            }
                              
                            DataRow ldrNewRow = FillData(lbusDeathNotification, "RECORD INSERTED", istrMessage);
                            if (ldrNewRow.IsNotNull())
                                ldtbPersonData.Rows.Add(ldrNewRow);

                            //PIR-806                  
                            //iobjPassInfo.Commit();
                        }
                        catch (Exception ex)
                        {
                            //PIR-806                    
                            //iobjPassInfo.Rollback();

                            DataRow ldrNewRow = FillData(lbusDeathNotification, "FAILED", ex.InnerException.Message);
                            if (ldrNewRow.IsNotNull())
                                ldtbPersonData.Rows.Add(ldrNewRow);
                        }
                    }
                }
            }
        }

        //Tushar-806
        public DataTable CreateTableDesignForPersonReport()
        {
            ldtbPersonData = new DataTable();
            ldtbPersonData.Columns.Add(new DataColumn("MPI_PERSON_ID", typeof(string)));
            ldtbPersonData.Columns.Add(new DataColumn("PARTICIPANT_NAME", typeof(string)));
            ldtbPersonData.Columns.Add(new DataColumn("DOB", typeof(DateTime)));
            ldtbPersonData.Columns.Add(new DataColumn("DOD", typeof(DateTime)));
            ldtbPersonData.Columns.Add(new DataColumn("PERSON_TYPE", typeof(string)));
            ldtbPersonData.Columns.Add(new DataColumn("PERSON_STATUS", typeof(string)));
            ldtbPersonData.Columns.Add(new DataColumn("PROCESS_STATUS", typeof(string)));
            ldtbPersonData.Columns.Add(new DataColumn("COMMENTS", typeof(string)));
            ldtbPersonData.Columns.Add(new DataColumn("VESTED", typeof(string)));  //PIR RID 56892

            return ldtbPersonData;
        }

        public DataRow FillData(busDeathNotification lbusDeathNotification, string ProcessStatus, string Message = "")
        {
            //Tushar-806-Code to catch exception goes here
            DataRow ldrNewRow = ldtbPersonData.NewRow();
            ldrNewRow["MPI_PERSON_ID"] = (lbusDeathNotification.icdoDeathNotification.istrMpiPersonId.IsNotNullOrEmpty() ? lbusDeathNotification.icdoDeathNotification.istrMpiPersonId : " ");
            //ldrNewRow["FIRST_NAME"] = Convert.ToString(acdoPerson["FIRST_NAME"]);
            //ldrNewRow["LAST_NAME"] = Convert.ToString(acdoPerson["LAST_NAME"]);
            ldrNewRow["PARTICIPANT_NAME"] = (lbusDeathNotification.icdoDeathNotification.istrName.IsNotNullOrEmpty() ? lbusDeathNotification.icdoDeathNotification.istrName.ToUpper() : " ");
            ldrNewRow["DOB"] = (lbusDeathNotification.icdoDeathNotification.idtDateOfBirth.IsNotNull() ? lbusDeathNotification.icdoDeathNotification.idtDateOfBirth : DateTime.MinValue);
            ldrNewRow["DOD"] = (lbusDeathNotification.icdoDeathNotification.date_of_death.IsNotNull() ? lbusDeathNotification.icdoDeathNotification.date_of_death : DateTime.MinValue);
            ldrNewRow["PERSON_TYPE"] = (lbusDeathNotification.icdoDeathNotification.istrPersonType.IsNotNullOrEmpty() ? lbusDeathNotification.icdoDeathNotification.istrPersonType.ToUpper() : " ");
            ldrNewRow["PERSON_STATUS"] = (lbusDeathNotification.icdoDeathNotification.istrStatus.IsNotNullOrEmpty() ? lbusDeathNotification.icdoDeathNotification.istrStatus : " ");
            ldrNewRow["PROCESS_STATUS"] = (ProcessStatus.IsNotNullOrEmpty() ? ProcessStatus : " ");
            ldrNewRow["COMMENTS"] = (Message.IsNotNullOrEmpty() ? Message.ToUpper() : " ");
            ldrNewRow["VESTED"] = (lbusDeathNotification.icdoDeathNotification.istrMPIVested.IsNotNullOrEmpty() ? lbusDeathNotification.icdoDeathNotification.istrMPIVested : " ");  //PIR RID 56892
            return ldrNewRow;
        }

        public override void FinalizeFile()
        {
            base.FinalizeFile();

            // Tushar-806 Report
            if (ldtbPersonData != null && ldtbPersonData.Rows.Count > 0)
            {
                try
                {
                    busCreateReports lobjCreateReports = new busCreateReports();
                    ldtbPersonData.TableName = "ReportTable01";
                    lobjCreateReports.CreatePDFReport(ldtbPersonData, "rpt_SmallWorldInbound");
                }
                catch (Exception e)
                {

                }
            }
        }

        public override busFileBase.sfwOnFileError ContinueOnValueError(string astrObjectField, out string astrValue)
        {
            astrValue = String.Empty;
            string lstrObjectField = astrObjectField.IndexOf(".") > -1 ? astrObjectField.Substring(astrObjectField.LastIndexOf(".") + 1) : astrObjectField;
            switch (lstrObjectField.ToLower())
            {
                case "date_of_death":
                case "date_of_birth":
                    astrValue = String.Empty;
                    return sfwOnFileError.ContinueWithRecord;

                default: return base.ContinueOnValueError(astrObjectField, out astrValue);
            }
        }
        #endregion

    }
}
