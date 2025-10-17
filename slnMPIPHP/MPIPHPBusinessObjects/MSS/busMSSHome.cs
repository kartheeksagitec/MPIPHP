using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.BusinessObjects;
using System.Data;
using System.Collections.ObjectModel;
using MPIPHP.DataObjects;
using System.Collections;
using Sagitec.Common;
using System.Linq.Expressions;
using Sagitec.DBUtility;
using Sagitec.CustomDataObjects;
using MPIPHP.CustomDataObjects;
using Sagitec.DataObjects;
using MPIPHP.Common;
using System.IO;


namespace MPIPHP.BusinessObjects
{
    [Serializable]
    public class busMSSHome : busMPIPHPBase
    {
        public busPerson ibusPerson { get; set; }

        public string istrApplicationStatus { get; set; }
        public int iintYear { get; set; }
        public string istrFileName { get; set; }

        #region Benefit Estimate Starting Screen Options
        public string istrBenefitEstimateOptions { get; set; } //View , New
        public string istrBenefitEstimateType { get; set; }//Dro, Retirement
        #endregion

        #region Prop for applications list
        public Collection<busProcessInstance> iclbProcessInstance { get; set; }
        #endregion

        #region Prop for application activities
        public Collection<Application_Process> iclbApplicationActivties { get; set; }
        public Collection<Application_Process> iclbApplicationActivitiesForUser { get; set; }
        public Collection<cdoPlan> iColPlans { get; set; }
        public busProcess ibusProcess { get; set; }
        public busProcessInstance ibusProcessInstance { get; set; }
        public bool iblnLocal52SpecialAccount { get; set; }
        public bool iblnLocal161SpecialAccount { get; set; }
        public bool iblnPureIapForWithdrawal { get; set; }
        public bool iblnShowCancellationForm { get; set; }
        #endregion


        public bool LoadPerson(int aintPersonID)
        {
            if (ibusPerson == null)
                ibusPerson = new busPerson();

            return ibusPerson.FindPerson(aintPersonID);
        }

        public bool CheckIfDOBISNull()
        {
            if (this.ibusPerson.icdoPerson.date_of_birth == DateTime.MinValue)
            {
                return true;
            }
            return false;
        }

        public Collection<cdoCodeValue> GetBenefitEstimateTypeOptions(int aintPersonID)
        {
            Collection<cdoCodeValue> lclcValues = new Collection<cdoCodeValue>();
            cdoCodeValue lcdo = new cdoCodeValue();
            lcdo.code_value = "RTMT";
            lcdo.description = "Retirement Estimate";
            lclcValues.Add(lcdo);
            DataTable ldtDRO = busBase.Select("cdoMssBenefitCalculationHeader.GetQualifiedDROs", new object[1] { this.ibusPerson.icdoPerson.person_id });
            if (ldtDRO.Rows.Count > 0)
            {
                bool lblnAddDro = false;
                foreach (DataRow ldtRow in ldtDRO.Rows)
                {
                    if (Convert.ToString(ldtRow[enmDroBenefitDetails.plan_id.ToString()]).IsNotNullOrEmpty())
                    {
                        int lintPlanD = Convert.ToInt32(ldtRow[enmDroBenefitDetails.plan_id.ToString()]);
                        if (lintPlanD != 1)
                        {
                            int lintDroAppId = Convert.ToInt32(ldtRow[enmDroBenefitDetails.dro_application_id.ToString()]);
                            if (lintDroAppId > 0)
                            {
                                DataTable ldtDROCalc = busBase.Select("cdoMssBenefitCalculationHeader.GetCalculationForDRO", new object[2] { lintDroAppId,lintPlanD });
                                if (ldtDROCalc.Rows.Count > 0)
                                {
                                    lblnAddDro = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            lblnAddDro = true;
                            break;
                        }
                    }
                }
                if (lblnAddDro)
                {
                    lcdo = new cdoCodeValue();
                    lcdo.description = "QDRO Estimate";
                    lcdo.code_value = "DRO";
                    lclcValues.Add(lcdo);
                }
            }
            return lclcValues;

        }

        public void LoadPersonWorkflows(int aintpersonid)
        {
            DataTable ldtProcess = busBase.Select("cdoMssBenefitCalculationHeader.GetProcessForPerson", new object[1] { this.ibusPerson.icdoPerson.person_id });
            if (ldtProcess.IsNotNull() && ldtProcess.Rows.Count > 0)
            {
                iclbProcessInstance = GetCollection<busProcessInstance>(ldtProcess, "icdoProcessInstance");
                foreach (busProcessInstance lbusProcessInstance in iclbProcessInstance)
                {
                    int aintProcessID = Convert.ToInt32(ldtProcess.AsEnumerable().Where(row => Convert.ToInt32(row[enmProcessInstance.process_instance_id.ToString()]) == lbusProcessInstance.icdoProcessInstance.process_instance_id).FirstOrDefault()[enmProcessInstance.process_id.ToString()]);
                    lbusProcessInstance.ibusProcess = new busProcess { icdoProcess = new cdoProcess() };
                    lbusProcessInstance.ibusProcess.FindProcess(aintProcessID);
                }
            }

        }

        public void LoadAllActivitiesAssociatedWithTheProcess(int aintprocessinstanceid)
        {
            LoadInititedActivitiesForProcess(aintprocessinstanceid);
            if (this.ibusProcessInstance.icdoProcessInstance.status_value != "PROC")
            {
                LoadPendingActivitiesForProcess(aintprocessinstanceid);
            }
            LoadApplicationStatus();


        }

        public void LoadApplicationStatus()
        {
            string lstrApplicationRecievedMessage = string.Empty;
            string lstrApplicationApprovalMessage = string.Empty;
            string lstrCalculationApprovalMessage = string.Empty;
            string lstrPayeeApprovalMessage = string.Empty;
           
            if (this.ibusProcess.icdoProcess.name == busConstant.RETIREMENT_WORKFLOW_NAME)
            {
                if (this.ibusProcessInstance.icdoProcessInstance.status_value != WorkflowConstants.WorkflowProcessInstanceStatusProcessed)
                {
                    iColPlans = GetPlansForRetirementApplication();
                    lstrApplicationRecievedMessage = "RETIREMENT APPLICATION RECIEVED";
                    lstrApplicationApprovalMessage = "Approve Retirement Application";
                    lstrCalculationApprovalMessage = "Calculation";
                    lstrPayeeApprovalMessage = "Payee Account";
                    CheckIfApplicationApproved(lstrApplicationRecievedMessage, lstrApplicationApprovalMessage, lstrCalculationApprovalMessage, lstrPayeeApprovalMessage);
                }
                else
                {
                    iclbApplicationActivitiesForUser = iclbApplicationActivties;
                }
            }
            else if (this.ibusProcess.icdoProcess.name == busConstant.WITHDRAWAL_WORKFLOW_NAME)
            {
                if (this.ibusProcessInstance.icdoProcessInstance.status_value != WorkflowConstants.WorkflowProcessInstanceStatusProcessed)
                {
                    iColPlans = GetPlansForWithdrawalApplication();
                   
                    lstrApplicationRecievedMessage = "WITHDRAWAL APPLICATION RECIEVED";
                    lstrApplicationApprovalMessage = "Approve Withdrawal Application";
                    lstrCalculationApprovalMessage = "Calculation";
                    lstrPayeeApprovalMessage = "Payee Account";
                    CheckIfWithdrawalApplicationApproved(lstrApplicationRecievedMessage, lstrApplicationApprovalMessage, lstrCalculationApprovalMessage, lstrPayeeApprovalMessage);
                }
                else
                {
                    iclbApplicationActivitiesForUser = iclbApplicationActivties;
                }
            }
            else if (this.ibusProcess.icdoProcess.name == busConstant.DISABILITY_WORKFLOW_NAME)
            {
                if (this.ibusProcessInstance.icdoProcessInstance.status_value != WorkflowConstants.WorkflowProcessInstanceStatusProcessed)
                {
                    iColPlans = GetPlansForRetirementApplication();
                    lstrApplicationRecievedMessage = "DISABILITY APPLICATION RECIEVED";
                    lstrApplicationApprovalMessage = "Approve Disability Application";
                    lstrCalculationApprovalMessage = "Calculation";
                    lstrPayeeApprovalMessage = "Payee Account";
                    CheckIfApplicationApproved(lstrApplicationRecievedMessage, lstrApplicationApprovalMessage, lstrCalculationApprovalMessage, lstrPayeeApprovalMessage);

                }
                else
                {
                    iclbApplicationActivitiesForUser = iclbApplicationActivties;
                }
            }
            else if (this.ibusProcess.icdoProcess.name == busConstant.QDRO_WORKFLOW_NAME)
            {
                if (this.ibusProcessInstance.icdoProcessInstance.status_value != WorkflowConstants.WorkflowProcessInstanceStatusProcessed)
                {
                    iColPlans = GetPlansForRetirementApplication();
                    lstrApplicationRecievedMessage = "QDRO APPLICATION RECIEVED";
                    lstrApplicationApprovalMessage = "Approve Qdro Application";
                    lstrCalculationApprovalMessage = "Calculation";
                    lstrPayeeApprovalMessage = "Payee Account";
                    CheckIfApplicationApproved(lstrApplicationRecievedMessage, lstrApplicationApprovalMessage, lstrCalculationApprovalMessage, lstrPayeeApprovalMessage);

                }
                else
                {
                    iclbApplicationActivitiesForUser = iclbApplicationActivties;
                }
            }
            else if (this.ibusProcess.icdoProcess.name == busConstant.PROCESS_EARLY_TO_DISABILITY)
            {
                if (this.ibusProcessInstance.icdoProcessInstance.status_value != WorkflowConstants.WorkflowProcessInstanceStatusProcessed)
                {
                    iColPlans = GetPlansForRetirementApplication();
                    lstrApplicationRecievedMessage = "DISABILITY APPLICATION RECIEVED";
                    lstrApplicationApprovalMessage = "Approve Disability Application";
                    lstrCalculationApprovalMessage = "Calculation";
                    lstrPayeeApprovalMessage = "Payee Account";
                    CheckIfApplicationApproved(lstrApplicationRecievedMessage, lstrApplicationApprovalMessage, lstrCalculationApprovalMessage, lstrPayeeApprovalMessage);

                }
                else
                {
                    iclbApplicationActivitiesForUser = iclbApplicationActivties;
                }
            }
        }

        public void LoadInititedActivitiesForProcess(int aintprocessinstanceid)
        {
            iclbApplicationActivties = new Collection<Application_Process>();
            DataTable ldtActivities = busBase.Select("cdoMssBenefitCalculationHeader.GetActivitiesInitiatedForPerson", new object[1] { aintprocessinstanceid });
            Application_Process lapp;
            string lstrDisplayMessage = string.Empty;
            if (ldtActivities.IsNotNull() && ldtActivities.Rows.Count > 0)
            {
                foreach(DataRow ldtRow in ldtActivities.Rows)
                {
                    lstrDisplayMessage = Convert.ToString(ldtRow[enmActivity.mss_display_message.ToString()]);
                    if (this.ibusProcess.icdoProcess.process_id == 5)
                    {
                        if(Convert.ToString(ldtRow[enmActivity.name.ToString()]) == "Enter/Update Retirement Application")
                        {
                            this.iblnShowCancellationForm = true;
                            int lintRefApplicationID = 0;
                            if (Convert.ToString(ldtRow[enmActivityInstance.reference_id.ToString()]).IsNotNullOrEmpty())
                            {
                                lintRefApplicationID = Convert.ToInt32(ldtRow[enmActivityInstance.reference_id.ToString()]);
                            }
                            if (lintRefApplicationID > 0)
                            {
                                busBenefitApplication lbusbenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                                lbusbenefitApplication.FindBenefitApplication(lintRefApplicationID);
                                if (lbusbenefitApplication.icdoBenefitApplication.retirement_date < DateTime.Now)
                                {
                                    iblnShowCancellationForm = false;
                                }
                            }
                        }
                    }
                    if (lstrDisplayMessage.IsNotNullOrEmpty())
                    {
                        lapp = new Application_Process();
                        if (Convert.ToString(ldtRow[enmActivityInstance.reference_id.ToString()]).IsNotNullOrEmpty())
                        {
                            lapp.iintReferenceID = Convert.ToInt32(ldtRow[enmActivityInstance.reference_id.ToString()]);
                        }
                        if (Convert.ToString(ldtRow[enmActivity.plan_id.ToString()]).IsNotNullOrEmpty())
                        {
                            lapp.iintPlanID = Convert.ToInt32(ldtRow[enmActivity.plan_id.ToString()]);
                        }
                        lapp.istrName = Convert.ToString(ldtRow[enmActivity.name.ToString()]);
                        lapp.istrDisplay_Message = Convert.ToString(ldtRow[enmActivity.mss_display_message.ToString()]);
                        lapp.istrStatusValue = Convert.ToString(ldtRow[enmActivityInstance.status_value.ToString()]);
                        lapp.istrStatusDescription = GetStatusDescription(lapp.istrStatusValue);
                        if (lapp.istrStatusValue == "PROC")
                        {
                            lapp.idtCompletionDate = Convert.ToDateTime(ldtRow["MODIFIED_DATE"]);
                        }
                        iclbApplicationActivties.Add(lapp);
                    }
                   
                }
               
            }
        }

        public void LoadPendingActivitiesForProcess(int aintprocessinstanceid)
        {
            DataTable ldtActivities = busBase.Select("cdoMssBenefitCalculationHeader.GetPendingActivitiesForProcess", new object[1] { aintprocessinstanceid });
            if (ldtActivities.IsNotNull() && ldtActivities.Rows.Count > 0)
            {
                Application_Process lapp;
                string lstrDisplayMessage = string.Empty;
                foreach (DataRow ldtRow in ldtActivities.Rows)
                {
                    lstrDisplayMessage = Convert.ToString(ldtRow[enmActivity.mss_display_message.ToString()]);
                    if (this.ibusProcess.icdoProcess.process_id == 5)
                    {
                        if (Convert.ToString(ldtRow[enmActivity.name.ToString()]) == "Enter/Update Retirement Application")
                        {
                            this.iblnShowCancellationForm = true;
                        }
                    }
                    if (lstrDisplayMessage.IsNotNullOrEmpty())
                    {
                        lapp = new Application_Process();
                        if (Convert.ToString(ldtRow[enmActivity.plan_id.ToString()]).IsNotNullOrEmpty())
                        {
                            lapp.iintPlanID = Convert.ToInt32(ldtRow[enmActivity.plan_id.ToString()]);
                        }
                        lapp.istrName = Convert.ToString(ldtRow[enmActivity.name.ToString()]);
                        lapp.istrDisplay_Message = Convert.ToString(ldtRow[enmActivity.mss_display_message.ToString()]);
                        lapp.istrStatusDescription = "Pending";
                        lapp.istrStatusValue = "UNPC";
                        iclbApplicationActivties.Add(lapp);
                    }
                }
            }

        }


        #region Retirement/Disability/QDRO
        public Collection<cdoPlan> GetPlansForRetirementApplication()
        {
            Collection<cdoPlan> lColPlans = new Collection<cdoPlan>();
            DataTable ldtplan = busBase.Select("cdoMssBenefitCalculationHeader.GetPlanForMember", new object[1] { this.ibusPerson.icdoPerson.person_id });
            if (ldtplan.Rows.Count > 0)
            {
                lColPlans = doBase.LoadData<cdoPlan>(ldtplan);
            }

            return lColPlans;
        }


        public void CheckIfApplicationApproved(string astrApplicationRecieved, string astrApplicationApprovalAct,string astrCalculationApprovalAct,string astrPaymentApprovalAct)
        {

            Application_Process lbus = new Application_Process();
            if (iclbApplicationActivties.Where(item => item.istrName.ToLower().Trim() == astrApplicationApprovalAct.ToLower().Trim()).Count() > 0)
            {
                string lstrStatus = iclbApplicationActivties.Where(item => item.istrName.ToLower().Trim() == astrApplicationApprovalAct.ToLower().Trim()).FirstOrDefault().istrStatusValue;
                lbus = iclbApplicationActivties.Where(item => item.istrName.ToLower().Trim() == astrApplicationApprovalAct.ToLower().Trim()).FirstOrDefault();
                iclbApplicationActivitiesForUser.Add(lbus);
                if(lstrStatus.IsNotNullOrEmpty())
                {
                    if (lstrStatus == "CANC" || lstrStatus == "INPC" || lstrStatus == "SUSP" || lstrStatus == "RESU" || 
                        lstrStatus == "UNPC")
                    {

                        #region Calculation
                        CheckIfCalcualationApproved(astrCalculationApprovalAct);
                        #endregion

                        #region Payment
                        CheckIfPayeeAccountApproved(astrPaymentApprovalAct);
                        #endregion
                        /*
                        #region Calculation
                        lbus = new Application_Process();
                        lstrMessage = BuildString("UNPC", "Retirement Application", aclbPlans, iclbApplicationActivitiesForUser);
                        #endregion

                        #region Payment
                        lbus = new Application_Process();
                        lstrMessage = BuildString("UNPC", "Payment Set Up", aclbPlans, iclbApplicationActivitiesForUser);
                        #endregion*/

                    }
                   
                    else if (lstrStatus == "PROC")
                    {
                        int lintReferenceID = iclbApplicationActivties.Where(item => item.istrName == "Approve Retirement Application").FirstOrDefault().iintReferenceID;
                        #region GetPLans
                        iColPlans = new Collection<cdoPlan>();
                        DataTable ldtplan = busBase.Select("cdoMssBenefitCalculationHeader.GetPlansFromApplication", new object[1] { lintReferenceID });
                        if (ldtplan.Rows.Count > 0)
                        {
                            iColPlans = doBase.LoadData<cdoPlan>(ldtplan);
                        }
                        #endregion

                        #region Calculation
                        CheckIfCalcualationApproved(astrCalculationApprovalAct);
                        #endregion

                        #region Payment
                        CheckIfPayeeAccountApproved(astrPaymentApprovalAct);
                        #endregion
                    }
                   
                }

            }
        }

        public void CheckIfCalcualationApproved(string astrCalculationApprovalMessage)
        {
            foreach (cdoPlan lcdoPlan in iColPlans)
            {
                foreach (Application_Process lobj in iclbApplicationActivties)
                {
                    if (lcdoPlan.plan_id == lobj.iintPlanID)
                    {
                        if (lobj.istrName.Contains(astrCalculationApprovalMessage))
                        {
                            iclbApplicationActivitiesForUser.Add(lobj);
                        }
                    }
                }
                /*
                lstrPlan = lcdoPlan.plan_code;
                if (lcdoPlan.plan_code == busConstant.MPIPP)
                {
                    if (iclbApplicationActivties.Where(item => item.istrName == "Approve Final MPIPP Calculation").Count() > 0)
                    {
                        lstrMessage = iclbApplicationActivties.Where(item => item.istrName == "Approve Final MPIPP Calculation").FirstOrDefault().istrDisplay_Message;
                      lstrStatus = iclbApplicationActivties.Where(item => item.istrName == "Approve Final MPIPP Calculation").FirstOrDefault().istrStatusValue;
                      BuildString(lstrStatus, lstrMessage, null, iclbApplicationActivitiesForUser);
                    }
                }
                else if (lcdoPlan.plan_code == busConstant.IAP)
                {
                    if (iclbApplicationActivties.Where(item => item.istrName == "Approve Final IAP Calculation").Count() > 0)
                    {
                        lstrMessage = iclbApplicationActivties.Where(item => item.istrName == "Approve Final IAP Calculation").FirstOrDefault().istrDisplay_Message;
                        lstrStatus = iclbApplicationActivties.Where(item => item.istrName == "Approve Final IAP Calculation").FirstOrDefault().istrStatusValue;
                        BuildString(lstrStatus, lstrMessage, null, iclbApplicationActivitiesForUser);
                    }
                }
                else if (lcdoPlan.plan_code == busConstant.Local_161)
                {
                    lstrPlan = "161";
                    if (iclbApplicationActivties.Where(item => item.istrName == "Approve Final Local 161 Calculation").Count() > 0)
                    {
                        lstrMessage = iclbApplicationActivties.Where(item => item.istrName == "Approve Final Local 161 Calculation").FirstOrDefault().istrDisplay_Message;
                        lstrStatus = iclbApplicationActivties.Where(item => item.istrName == "Approve Final Local 161 Calculation").FirstOrDefault().istrStatusValue;
                        BuildString(lstrStatus, lstrMessage, null, iclbApplicationActivitiesForUser);
                    }
                }
                else if (lcdoPlan.plan_code == busConstant.Local_52)
                {
                    lstrPlan = "52";
                    if (iclbApplicationActivties.Where(item => item.istrName == "Approve Final Local 52 Calculation").Count() > 0)
                    {
                        lstrMessage = iclbApplicationActivties.Where(item => item.istrName == "Approve Final MPIPP Calculation").FirstOrDefault().istrDisplay_Message;
                        lstrStatus = iclbApplicationActivties.Where(item => item.istrName == "Approve Final MPIPP Calculation").FirstOrDefault().istrStatusValue;
                        BuildString(lstrStatus, lstrMessage, null, iclbApplicationActivitiesForUser);
                    }
                }
                else if (lcdoPlan.plan_code == busConstant.Local_600)
                {
                    lstrPlan = "600";
                    if (iclbApplicationActivties.Where(item => item.istrName == "Approve Final Local 600 Calculation").Count() > 0)
                    {
                        lstrMessage = iclbApplicationActivties.Where(item => item.istrName == "Approve Final Local 600 Calculation").FirstOrDefault().istrDisplay_Message;
                        lstrStatus = iclbApplicationActivties.Where(item => item.istrName == "Approve Final Local 600 Calculation").FirstOrDefault().istrStatusValue;
                        BuildString(lstrStatus, lstrMessage, null, iclbApplicationActivitiesForUser);
                    }

                }
                else if (lcdoPlan.plan_code == busConstant.LOCAL_700)
                {
                    lstrPlan = "700";
                    if (iclbApplicationActivties.Where(item => item.istrName == "Approve Final Local 700 Calculation").Count() > 0)
                    {
                        lstrMessage = iclbApplicationActivties.Where(item => item.istrName == "Approve Final Local 700 Calculation").FirstOrDefault().istrDisplay_Message;
                        lstrStatus = iclbApplicationActivties.Where(item => item.istrName == "Approve Final Local 700 Calculation").FirstOrDefault().istrStatusValue;
                        BuildString(lstrStatus, lstrMessage, null, iclbApplicationActivitiesForUser);
                    }
                }
                else if (lcdoPlan.plan_code == busConstant.Local_666)
                {
                    lstrPlan = "666";
                    if (iclbApplicationActivties.Where(item => item.istrName == "Approve Final Local 666 Calculation").Count() > 0)
                    {
                        lstrMessage = iclbApplicationActivties.Where(item => item.istrName == "Approve Final Local 666 Calculation").FirstOrDefault().istrDisplay_Message;
                        lstrStatus = iclbApplicationActivties.Where(item => item.istrName == "Approve Final Local 666 Calculation").FirstOrDefault().istrStatusValue;
                        BuildString(lstrStatus, lstrMessage, null, iclbApplicationActivitiesForUser);
                    }
                }*/
            }
        }

        public void CheckIfPayeeAccountApproved(string astrPayeeAccountApprovalMessage)
        {
            string lstrPlan = string.Empty;
            string lstrStatus = string.Empty;
            string lstrDisplayMessage = string.Empty;
            foreach (cdoPlan lcdoPlan in iColPlans)
            {
                foreach (Application_Process lobj in iclbApplicationActivties)
                {
                    if (lcdoPlan.plan_id == lobj.iintPlanID)
                    {
                        if (lobj.istrName.Contains(astrPayeeAccountApprovalMessage))
                        {
                            iclbApplicationActivitiesForUser.Add(lobj);
                        }
                    }
                }
                /*
                lstrPlan = lcdoPlan.plan_code;
                if (lcdoPlan.plan_code == busConstant.MPIPP)
                {
                    if (iclbApplicationActivties.Where(item => item.istrName == "Approve MPIPHP Payee Account").Count() > 0)
                    {
                        lstrDisplayMessage = iclbApplicationActivties.Where(item => item.istrName == "Approve MPIPHP Payee Account").FirstOrDefault().istrDisplay_Message;
                        lstrStatus = iclbApplicationActivties.Where(item => item.istrName == "Approve MPIPHP Payee Account").FirstOrDefault().istrStatusValue;
                        BuildString(lstrStatus, "PENSION PAYMENT SET UP", null, iclbApplicationActivitiesForUser);
                    }
                }
                else if (lcdoPlan.plan_code == busConstant.IAP)
                {
                    if (iclbApplicationActivties.Where(item => item.istrName == "Approve IAP Payee Account").Count() > 0)
                    {
                        lstrDisplayMessage = iclbApplicationActivties.Where(item => item.istrName == "Approve IAP Payee Account").FirstOrDefault().istrDisplay_Message;
                        lstrStatus = iclbApplicationActivties.Where(item => item.istrName == "Approve IAP Payee Account").FirstOrDefault().istrStatusValue;
                        BuildString(lstrStatus, "IAP PAYMENT SET UP", null, iclbApplicationActivitiesForUser);
                    }
                }
                else if (lcdoPlan.plan_code == busConstant.Local_161)
                {
                    lstrPlan = "161";
                    if (iclbApplicationActivties.Where(item => item.istrName == "Approve Local161 Payee Account").Count() > 0)
                    {
                        lstrDisplayMessage = iclbApplicationActivties.Where(item => item.istrName == "Approve Local161 Payee Account").FirstOrDefault().istrDisplay_Message;
                        lstrStatus = iclbApplicationActivties.Where(item => item.istrName == "Approve Local161 Payee Account").FirstOrDefault().istrStatusValue;
                        BuildString(lstrStatus, "LOCAL 161 PAYMENT SET UP", null, iclbApplicationActivitiesForUser);
                    }
                }
                else if (lcdoPlan.plan_code == busConstant.Local_52)
                {
                    lstrPlan = "52";
                    if (iclbApplicationActivties.Where(item => item.istrName == "Approve Local52 Payee Account").Count() > 0)
                    {
                        lstrDisplayMessage = iclbApplicationActivties.Where(item => item.istrName == "Approve Local52 Payee Account").FirstOrDefault().istrStatusValue;
                        lstrStatus = iclbApplicationActivties.Where(item => item.istrName == "Approve Local52 Payee Account").FirstOrDefault().istrStatusValue;
                        BuildString(lstrStatus, "LOCAL 52 PAYMENT SET UP", null, iclbApplicationActivitiesForUser);
                    }
                }
                else if (lcdoPlan.plan_code == busConstant.Local_600)
                {
                    lstrPlan = "600";
                    if (iclbApplicationActivties.Where(item => item.istrName == "Approve Local600 Payee Account").Count() > 0)
                    {
                        lstrStatus = iclbApplicationActivties.Where(item => item.istrName == "Approve Local600 Payee Account").FirstOrDefault().istrStatusValue;
                        BuildString(lstrStatus, "LOCAL 600 PAYMENT SET UP", null, iclbApplicationActivitiesForUser);
                    }

                }
                else if (lcdoPlan.plan_code == busConstant.LOCAL_700)
                {
                    lstrPlan = "700";
                    if (iclbApplicationActivties.Where(item => item.istrName == "Approve Local700 Payee Account").Count() > 0)
                    {
                        lstrStatus = iclbApplicationActivties.Where(item => item.istrName == "Approve Local700 Payee Account").FirstOrDefault().istrStatusValue;
                        BuildString(lstrStatus, "LOCAL 700 PAYMENT SET UP", null, iclbApplicationActivitiesForUser);
                    }
                }
                else if (lcdoPlan.plan_code == busConstant.Local_666)
                {
                    lstrPlan = "666";
                    if (iclbApplicationActivties.Where(item => item.istrName == "Approve Local666 Payee Account").Count() > 0)
                    {
                        lstrStatus = iclbApplicationActivties.Where(item => item.istrName == "Approve Local666 Payee Account").FirstOrDefault().istrStatusValue;
                        BuildString(lstrStatus, "LOCAL 666 PAYMENT SET UP", null, iclbApplicationActivitiesForUser);
                    }
                }*/
            }
        }
        #endregion

        #region Withdrawal Workflow
        public Collection<cdoPlan> GetPlansForWithdrawalApplication()
        {
            Collection<cdoPlan> lColPlans = new Collection<cdoPlan>();
            DataTable ldtplan = busBase.Select("cdoMssBenefitCalculationHeader.GetPlanForMember", new object[1] { this.ibusPerson.icdoPerson.person_id });
            if (ldtplan.Rows.Count > 0)
            {
                cdoPlan lcdoPlan = null;
                foreach (DataRow dr in ldtplan.Rows)
                {
                    lcdoPlan = new cdoPlan();
                    lcdoPlan.LoadData(dr);
                    if (lcdoPlan.plan_id == 1 || lcdoPlan.plan_id == 2)
                    {
                        lColPlans.Add(lcdoPlan);
                    }
                    else if (lcdoPlan.plan_id == 8)
                    {
                        string lstrSpecial = Convert.ToString(ldtplan.AsEnumerable().Where(row => Convert.ToInt32(row[enmPlan.plan_id.ToString()]) == 8).FirstOrDefault()[enmPersonAccount.special_account.ToString()]);
                        if (lstrSpecial == busConstant.FLAG_YES)
                        {
                            this.iblnLocal161SpecialAccount = true;
                        }
                    }
                    else if (lcdoPlan.plan_id == 7)
                    {
                        string lstrSpecial = Convert.ToString(ldtplan.AsEnumerable().Where(row => Convert.ToInt32(row[enmPlan.plan_id.ToString()]) == 7).FirstOrDefault()[enmPersonAccount.special_account.ToString()]);
                        if (lstrSpecial == busConstant.FLAG_YES)
                        {
                            this.iblnLocal52SpecialAccount = true;
                        }
                    }
                }
            }

            return lColPlans;
        } 


        public void CheckIfWithdrawalApplicationApproved(string astrApplicationRecieved, string astrApplicationApprovalAct, string astrCalculationApprovalAct, string astrPaymentApprovalAct)
        {
            Application_Process lbus = new Application_Process();
            lbus.istrStatusDescription = GetStatusDescription(lbus.istrStatusValue);
            if (iclbApplicationActivties.Where(item => item.istrName.ToLower().Trim() == astrApplicationApprovalAct.ToLower().Trim()).Count() > 0)
            {
                string lstrStatus = iclbApplicationActivties.Where(item => item.istrName.ToLower().Trim() == astrApplicationApprovalAct.ToLower().Trim()).FirstOrDefault().istrStatusValue;
                lbus = iclbApplicationActivties.Where(item => item.istrName.ToLower().Trim() == astrApplicationApprovalAct.ToLower().Trim()).FirstOrDefault();
                //lbus.istrStatusValue = lstrStatus;
                //lbus.istrStatusDescription = GetStatusDescription(lstrStatus);
                //lbus.istrDisplay_Message = iclbApplicationActivties.Where(item => item.istrName.ToLower().Trim() == astrApplicationApprovalAct.ToLower().Trim()).FirstOrDefault().istrDisplay_Message;
                iclbApplicationActivitiesForUser.Add(lbus);
                if (lstrStatus.IsNotNullOrEmpty())
                {
                    if (lstrStatus == "CANC" || lstrStatus == "INPC" || lstrStatus == "SUSP" || lstrStatus == "RESU" ||
                        lstrStatus == "UNPC")
                    {
                        #region Calculation
                        CheckIfCalculationApprovedForWithdrawal(astrCalculationApprovalAct);
                        #endregion

                        #region Payment
                        CheckIfPayeeAccountApprovedForWithdrawal(astrPaymentApprovalAct);
                        #endregion


                    }

                    else if (lstrStatus == "PROC")
                    {
                        int lintReferenceID = iclbApplicationActivties.Where(item => item.istrName.ToLower() == astrApplicationApprovalAct.Trim().ToLower()).FirstOrDefault().iintReferenceID;

                        iColPlans = new Collection<cdoPlan>();
                        this.iblnLocal52SpecialAccount = false;
                        this.iblnLocal161SpecialAccount = false;
                        DataTable ldtplan = busBase.Select("cdoMssBenefitCalculationHeader.GetPlansFromWithdrawalApplication", new object[1] { lintReferenceID });
                        if (ldtplan.Rows.Count > 0)
                        {
                            #region GetPLans
                            cdoPlan lcdoPlan = null;
                            foreach (DataRow dr in ldtplan.Rows)
                            {
                                lcdoPlan = new cdoPlan();
                                lcdoPlan.LoadData(dr);
                                if (lcdoPlan.plan_id == 1 || lcdoPlan.plan_id == 2)
                                {
                                    iColPlans.Add(lcdoPlan);
                                    if (lcdoPlan.plan_id == 1)
                                    {
                                        string lstrL161SpecialAccount = Convert.ToString(dr[enmBenefitApplicationDetail.l161_spl_acc_flag.ToString()]);
                                        string lstrL52SpecialAccount = Convert.ToString(dr[enmBenefitApplicationDetail.l52_spl_acc_flag.ToString()]);
                                        if (lstrL161SpecialAccount == busConstant.FLAG_YES)
                                        {
                                            this.iblnLocal161SpecialAccount = true;
                                        }
                                        else if (lstrL52SpecialAccount == busConstant.FLAG_YES)
                                        {
                                            this.iblnLocal52SpecialAccount = true;
                                        }
                                        else
                                            this.iblnPureIapForWithdrawal = true;
                                        
                                    }
                                }
                            }
                            #endregion

                            #region Calculation
                            CheckIfCalculationApprovedForWithdrawal(astrCalculationApprovalAct);
                            #endregion

                            #region Payment
                            CheckIfPayeeAccountApprovedForWithdrawal(astrPaymentApprovalAct);
                            #endregion

                        }
                    }

                }
            }
        }

        public void CheckIfCalculationApprovedForWithdrawal(string astrCalculationApprovalMessage)
        {
            foreach (cdoPlan lcdoPlan in iColPlans)
            {
                foreach (Application_Process lobj in iclbApplicationActivties)
                {
                    if (lcdoPlan.plan_id == lobj.iintPlanID)
                    {
                        if (lobj.istrName.Contains(astrCalculationApprovalMessage))
                        {
                            iclbApplicationActivitiesForUser.Add(lobj);
                        }
                    }
                }
            }
        }

        public void CheckIfPayeeAccountApprovedForWithdrawal(string astrPayeeAccountApprovalMessage)
        {
            string lstrPlan = string.Empty;
            string lstrStatus = string.Empty;
            string lstrDisplayMessage = string.Empty;
            foreach (cdoPlan lcdoPlan in iColPlans)
            {
                foreach (Application_Process lobj in iclbApplicationActivties)
                {
                    if (lcdoPlan.plan_id == lobj.iintPlanID)
                    {
                        if (lcdoPlan.plan_id == 2 || (lcdoPlan.plan_id == 1 && this.iblnPureIapForWithdrawal))
                        {
                            if (lobj.istrName.Contains(astrPayeeAccountApprovalMessage))
                            {
                                iclbApplicationActivitiesForUser.Add(lobj);
                            }
                        }
                    }
                    else if (lcdoPlan.plan_id == 1 && lobj.iintPlanID == 8 && this.iblnLocal161SpecialAccount)
                    {
                        if (lobj.istrName.Contains(astrPayeeAccountApprovalMessage))
                        {
                            iclbApplicationActivitiesForUser.Add(lobj);
                        }
                    }
                    else if (lcdoPlan.plan_id == 1 && lobj.iintPlanID == 7 && this.iblnLocal52SpecialAccount)
                    {
                        if(lobj.istrName.Contains(astrPayeeAccountApprovalMessage))
                        {
                            iclbApplicationActivitiesForUser.Add(lobj);
                        }
                    }
                }
                
            }
        }

        #endregion

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="astrStep"></param>
        /// <param name="astrStatus"></param> Pending, Approved, In Progress
        /// <returns></returns>
        public string BuildRetirementWorkflowString(Collection<cdoPlan> aclbPlans)
        {
            string lstrRetApplication = string.Empty;
            lstrRetApplication = "Retirement Application Recieved";
            string lstrPlan = string.Empty;
            istrApplicationStatus = "Retirement Application Recieved : 1";
            foreach (cdoPlan lcdoPlan in aclbPlans)
            {
                lstrPlan = lcdoPlan.plan_code;
                if (lcdoPlan.plan_code == busConstant.MPIPP)
                {
                    lstrRetApplication += ";Pension Application Approved" + "Pension Payment Set Up Complete"; ;
                }
                else if (lcdoPlan.plan_code == busConstant.IAP)
                {
                    lstrRetApplication += lcdoPlan.plan_code + " Application Approved;";
                }
                else if (lcdoPlan.plan_code == busConstant.Local_161)
                {
                    lstrRetApplication += lcdoPlan.plan_code + " Application Approved;";
                }
                else if (lcdoPlan.plan_code == busConstant.Local_52)
                {
                    lstrRetApplication += lcdoPlan.plan_code + " Application Approved;";
                }
                else if (lcdoPlan.plan_code == busConstant.Local_600)
                {
                    lstrRetApplication += lcdoPlan.plan_code + " Application Approved;";
                }
                else if (lcdoPlan.plan_code == busConstant.LOCAL_700)
                {
                    lstrRetApplication += lcdoPlan.plan_code + " Application Approved;";
                }
                else if (lcdoPlan.plan_code == busConstant.Local_666)
                {
                    lstrRetApplication += lcdoPlan.plan_code + " Application Approved;";
                }

            }
        }
        */

        public string BuildString(string astrStatus, string astrMessage, Collection<cdoPlan> aclbPlans, Collection<Application_Process> aclbProcess)
        {
            string lstrPlan = string.Empty;
            Application_Process lbus;
            if (!aclbPlans.IsNullOrEmpty())
            {
                foreach (cdoPlan lcdoPlan in aclbPlans)
                {
                    lbus = new Application_Process();

                    lstrPlan = lcdoPlan.plan_code + " ";
                    if (lcdoPlan.plan_code == busConstant.MPIPP)
                    {
                        lstrPlan = "Pension ";
                    }
                    lstrPlan = lstrPlan + astrMessage;
                    lbus.istrDisplay_Message = lstrPlan;
                    lbus.istrStatusValue = astrStatus;
                    aclbProcess.Add(lbus);
                }
            }
            else
            {
                lbus = new Application_Process();
                lbus.istrDisplay_Message = astrMessage;
                lbus.istrStatusValue = astrStatus;
                lbus.istrStatusDescription = GetStatusDescription(lbus.istrStatusValue);
                aclbProcess.Add(lbus);
            }
            return astrMessage;
        }

        public string GetStatusDescription(string astrStatus)
        {
            string lstrStatusDesc = string.Empty;
            if (astrStatus == "CANC")
            {
                lstrStatusDesc = "Cancelled";
            }
            else if (astrStatus == "INPC")
            {
                lstrStatusDesc = "In Progress";
            }
            else if (astrStatus == "PROC")
            {
                lstrStatusDesc = "Completed";
            }
            else if (astrStatus == "SUSP")
            {
                lstrStatusDesc = "Pending";
            }
            else if (astrStatus == "RESU")
            {
                lstrStatusDesc = "Pending";
            }
            else if (astrStatus == "UNPC")
            {
                lstrStatusDesc = "Pending";
            }
            return lstrStatusDesc;
        }

        public Collection<busAnnualStatementBatchData> iclbAnnualStatementBatchData { get; set; }
        public void GetLatestAnnualStatementFile(int aintpersonid)
        {
            iclbAnnualStatementBatchData = new Collection<busAnnualStatementBatchData>();
            DataTable iclbAnnualStatementsYrs = null;
            iclbAnnualStatementsYrs = busBase.Select("cdoDataExtractionBatchInfo.GetAnnualStatementYearsForPerson", new object[1] { aintpersonid });

            foreach (DataRow ldtr in iclbAnnualStatementsYrs.Rows)
            {
                busAnnualStatementBatchData lobjYear = new busAnnualStatementBatchData
                {
                    icdoAnnualStatementBatchData = new cdoAnnualStatementBatchData(),

                };
                lobjYear.icdoAnnualStatementBatchData.LoadData(ldtr);

                iclbAnnualStatementBatchData.Add(lobjYear);
            }
        }
    }

    [Serializable]
    public class Application_Process
    {
        public int iintPlanID { get; set; }
        public int iintReferenceID { get; set; }
        public string istrName { get; set; }
        public string istrDisplay_Message { get; set; }
        public string istrStatusDescription { get; set; }
        public string istrStatusValue { get; set; }
        public DateTime idtStartTime { get; set; }
        public DateTime idtCompletionDate { get; set; }
    }
}


