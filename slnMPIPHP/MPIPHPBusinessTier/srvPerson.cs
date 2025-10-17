#region Using directives

using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using MPIPHP.BusinessObjects;
using Sagitec.Common;
using MPIPHP.CustomDataObjects;
using System.Collections.ObjectModel;
using MPIPHP.DataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Interface;
using Sagitec.DBUtility;
using System.Text.RegularExpressions;
using Sagitec.Bpm;
using NeoSpin.BusinessObjects;


#endregion

namespace MPIPHP.BusinessTier
{
    public class srvPerson : srvMPIPHP 
    {
        public srvPerson()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private string istrVIPAccess;
        private void SetWebParameters()
        {
            if (iobjPassInfo.idictParams.ContainsKey("Logged_In_User_is_VIP"))
                istrVIPAccess = (string)iobjPassInfo.idictParams["Logged_In_User_is_VIP"];

        }

        public busPerson FindPerson(int aintpersonid)
        {
            busPerson lobjPerson = new busPerson();
            if (lobjPerson.FindPerson(aintpersonid))
            {
                lobjPerson.LoadInitialData();
                lobjPerson.GetCurrentAge();
                lobjPerson.LoadPersonAddresss();
                lobjPerson.LoadBeneficiaries();
                lobjPerson.LoadBeneficiariesOf();
				//Ticket - 68547
                lobjPerson.LoadDependentOf();
                lobjPerson.LoadPersonContacts();
                lobjPerson.LoadPersonDependents();
                lobjPerson.LoadPersonBridgedService();
                lobjPerson.LoadCorrAddress();
                lobjPerson.LoadParticipantPlan();
                lobjPerson.LoadPersonSuspendibleMonth();
                lobjPerson.GetCaseAnalystById(aintpersonid);
                lobjPerson.LoadPersonCommPref();
                //  lobjPerson.GetMDAgeById(aintpersonid);
               //if(lobjPerson.icdoPerson.md_age_opt_id == 0)
               // {
               //     lobjPerson.icdoPerson.md_age_opt_id = 1;

               // } 



                lobjPerson.iclbNotes = busGlobalFunctions.LoadNotes(lobjPerson.icdoPerson.person_id, 0, busConstant.PERSON_MAINTAINENCE_FORM);
                lobjPerson.LoadRetirementContributionsbyPersonId(lobjPerson.icdoPerson.person_id);
                lobjPerson.icdoPerson.cell_phone_no = busGlobalFunctions.ExtractDigits(lobjPerson.icdoPerson.cell_phone_no);
                lobjPerson.icdoPerson.home_phone_no = busGlobalFunctions.ExtractDigits(lobjPerson.icdoPerson.home_phone_no);
                lobjPerson.icdoPerson.work_phone_no = busGlobalFunctions.ExtractDigits(lobjPerson.icdoPerson.work_phone_no);
            }

            return lobjPerson;
        }

        public busPersonOverview FindPersonOverview(int aintpersonid)
        {
            busPersonOverview lobjPersonoverview = new busPersonOverview();
            if (lobjPersonoverview.FindPerson(aintpersonid))
            {
                //PIR-857               
                //lobjPersonoverview.icdoPerson.recalculate_vesting_flag = busConstant.FLAG_YES;
                //lobjPersonoverview.icdoPerson.Update();
                //lobjPersonoverview.icdoPerson.Select();  //PIR 857 Suresh

                lobjPersonoverview.LoadInitialData(false);
                lobjPersonoverview.GetCurrentAge();
                lobjPersonoverview.LoadActiveContacts();
                lobjPersonoverview.LoadPersonAddresss();
                lobjPersonoverview.LoadCorrAddress();
                lobjPersonoverview.LoadBeneficiariesForOverview();
                lobjPersonoverview.LoadBeneficiariesOf();
		        //Ticket - 68547
                lobjPersonoverview.LoadDependentOf();
                //lobjPersonoverview.LoadParticipantPlan();
                lobjPersonoverview.LoadPersonDependents();
                lobjPersonoverview.LoadPersonDROApplications();
                lobjPersonoverview.LoadDeathNotifications();
                lobjPersonoverview.LoadBenefitApplication();
                lobjPersonoverview.LoadParticipantWorkFlows();
                lobjPersonoverview.LoadPersonNotes();
                lobjPersonoverview.LoadPlanDetails();
                //lobjPersonoverview.LoadWorkHistory();


                if (lobjPersonoverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI.Count > 0)
                    lobjPersonoverview.iblnParticipant = busConstant.YES;

                //lobjPersonoverview.LoadPlanDetails();
                //LoadWorkHistory updates the sgt_person record, so need to load it again
                lobjPersonoverview.icdoPerson.Select();

                //for PIR-857(To set RECALCULATE_VESTING_FLAG to Y)
                if (lobjPersonoverview.lbusBenefitApplication.idtVestingDtIAP != lobjPersonoverview.lbusBenefitApplication.idtVestingDtIAPaftrFlg ||
                    lobjPersonoverview.lbusBenefitApplication.idtForfeitureDtIAP != lobjPersonoverview.lbusBenefitApplication.idtForfeitureDtIAPaftrFlg)
                {
                    lobjPersonoverview.AuditLogHistoryForVestingDtPersnOvervw(lobjPersonoverview.lbusBenefitApplication.idtVestingDtIAP, lobjPersonoverview.lbusBenefitApplication.idtVestingDtIAPaftrFlg, "IAP");
                    lobjPersonoverview.AuditLogHistoryForfeitureDatePersnOvervw(lobjPersonoverview.lbusBenefitApplication.idtForfeitureDtIAP, lobjPersonoverview.lbusBenefitApplication.idtForfeitureDtIAPaftrFlg, "IAP");
                }
                if (lobjPersonoverview.lbusBenefitApplication.idtVestingDtMPI != lobjPersonoverview.lbusBenefitApplication.idtVestingDtMPIaftrFlg ||
                    lobjPersonoverview.lbusBenefitApplication.idtForfeitureDtMPI != lobjPersonoverview.lbusBenefitApplication.idtForfeitureDtMPIaftrFlg)
                {
                    lobjPersonoverview.AuditLogHistoryForVestingDtPersnOvervw(lobjPersonoverview.lbusBenefitApplication.idtVestingDtMPI, lobjPersonoverview.lbusBenefitApplication.idtVestingDtMPIaftrFlg, "MPIPP");
                    lobjPersonoverview.AuditLogHistoryForfeitureDatePersnOvervw(lobjPersonoverview.lbusBenefitApplication.idtForfeitureDtMPI, lobjPersonoverview.lbusBenefitApplication.idtForfeitureDtMPIaftrFlg, "MPIPP");
                }

                //lobjPersonoverview.GetRetireeHealthHours();
                lobjPersonoverview.iclbHealthWorkHistory = new Collection<cdoDummyWorkData>();
                lobjPersonoverview.iclbHealthWorkHistory = lobjPersonoverview.lbusBenefitApplication.aclbPersonWorkHistory_MPI;

                if (!lobjPersonoverview.iclbHealthWorkHistory.IsNullOrEmpty())
                {
                    lobjPersonoverview.CheckRetireeHealthEligibilityAndUpdateFlag();
                }
                lobjPersonoverview.LoadPayeeAccount();
                lobjPersonoverview.LoadPacketCorrespondences();

                lobjPersonoverview.LoadProcessActivityLog();

                //ID-68932
                lobjPersonoverview.LoadPensionVerificationHistory();

            }

            return lobjPersonoverview;
        }

        public busPersonOverviewWorkFlows FindPersonOverviewWorkFlow(int aintpersonid)
        {
            busPersonOverviewWorkFlows lobjPersonOverviewWorkflow = new busPersonOverviewWorkFlows();
            if (lobjPersonOverviewWorkflow.FindPerson(aintpersonid))
            {

                lobjPersonOverviewWorkflow.LoadInitialData();
                lobjPersonOverviewWorkflow.LoadParticipantWorkFlows();
            }

            return lobjPersonOverviewWorkflow;
        }

        public busAnnualBenefitSummaryOverview FindAnnualBenefitSummaryOverview(int aintpersonid)
        {
            busAnnualBenefitSummaryOverview lobjbusAnnualBenefitSummaryOverview = new busAnnualBenefitSummaryOverview();
            if (lobjbusAnnualBenefitSummaryOverview.FindPerson(aintpersonid))
            {
                //lobjbusAnnualBenefitSummaryOverview.LoadInitialData();
                lobjbusAnnualBenefitSummaryOverview.LoadWorkHistory();
                lobjbusAnnualBenefitSummaryOverview.GetTotalHours();
                //lobjbusAnnualBenefitSummaryOverview.LoadAnnualBenefitSummaryOverview();
            }

            return lobjbusAnnualBenefitSummaryOverview;
        }

        public busAnnualBenefitSummaryOverview LoadAndRecalculateEEInterestDetail(int aintPersonId)
        {
            busAnnualBenefitSummaryOverview lbusAnnualBenefitSummaryOverview = new busAnnualBenefitSummaryOverview { icdoPerson = new cdoPerson() };

            if (lbusAnnualBenefitSummaryOverview.FindPerson(aintPersonId))
            {
                lbusAnnualBenefitSummaryOverview.LoadEEcontributions();
            }
            return lbusAnnualBenefitSummaryOverview;
        }

        public busPerson NewPerson()
        {
            busPerson lobjPerson = new busPerson();
            lobjPerson.icdoPerson = new cdoPerson();

            if (iobjPassInfo.idictParams.ContainsKey("SelectedParticipantId") && !string.IsNullOrEmpty(iobjPassInfo.GetParamValue("SelectedParticipantId").ToString()))
            {
                lobjPerson.ibusPersonAddress = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() };
                //lobjPerson.ibusPersonAddress.LoadChecklist();
                lobjPerson.ibusPersonAddress.LoadPersonAddressChklists();
                lobjPerson.ibusPersonAddress.LoadPersonAddressChklistsOld();
                lobjPerson.iintSelectedParticipantId = Convert.ToInt32(iobjPassInfo.GetParamValue("SelectedParticipantId"));
                lobjPerson.lbusRetirementApplication = new busRetirementApplication { icdoBenefitApplication = new cdoBenefitApplication() };

            }
            lobjPerson.iclbNotes = new Collection<busNotes>();
            lobjPerson.iclbPersonBridgeHours = new Collection<busPersonBridgeHours>();
            return lobjPerson;
        }

        public busPersonLookup LoadPersons(DataTable adtbSearchResult)
        {
            busPersonLookup lobjPersonLookup = new busPersonLookup();
            lobjPersonLookup.LoadPersons(adtbSearchResult);
            return lobjPersonLookup;
        }

        public busSsnMergeHistoryLookup LoadSSNMergeHistorys(DataTable adtbSearchResult)
        {
            busSsnMergeHistoryLookup lobjSSNMergeHistoryLookup = new busSsnMergeHistoryLookup();
            lobjSSNMergeHistoryLookup.LoadSsnMergeHistorys(adtbSearchResult);
            return lobjSSNMergeHistoryLookup;

        }

        public busPersonBeneficiary FindPersonBeneficiary(long aintPrimaryKey,int aintbeneficiaryid, string astrCheckIfParticipant, string astrCheckIfBen, string astrCheckIfAltPayee)
        {
            busPersonBeneficiary lobjPersonBeneficiary = new busPersonBeneficiary();

            if (lobjPersonBeneficiary.FindRelationship(aintbeneficiaryid))
            {
                //lobjPersonBeneficiary.icdoRelationship.iintAPrimaryKey = aintPrimaryKey;

                if (aintPrimaryKey.IsNotNull() && aintPrimaryKey != 0) {
                    lobjPersonBeneficiary.iintAPrimaryKey = aintPrimaryKey;
                }
                else {
                    lobjPersonBeneficiary.iintAPrimaryKey = aintbeneficiaryid;
                }
                
                lobjPersonBeneficiary.ibusPerson = new busPerson();

                //Loads Participant's Data
                lobjPersonBeneficiary.ibusPerson.FindPerson(lobjPersonBeneficiary.icdoRelationship.person_id);
                if (astrCheckIfParticipant.IsNotNullOrEmpty() && astrCheckIfBen.IsNotNullOrEmpty() && astrCheckIfAltPayee.IsNotNullOrEmpty())
                {
                    lobjPersonBeneficiary.ibusPerson.iblnBeneficiary = astrCheckIfBen;
                    lobjPersonBeneficiary.ibusPerson.iblnParticipant = astrCheckIfParticipant;
                    lobjPersonBeneficiary.ibusPerson.iblnAlternatePayee = astrCheckIfAltPayee;
                }
                else
                {
                    lobjPersonBeneficiary.ibusPerson.LoadInitialData();                
                }

                if (string.IsNullOrEmpty(lobjPersonBeneficiary.icdoRelationship.beneficiary_from_value))
                {
                    if (lobjPersonBeneficiary.ibusPerson.iblnParticipant == busConstant.YES)
                    {
                        lobjPersonBeneficiary.icdoRelationship.beneficiary_from_value = "PART";
                    }
                }
                lobjPersonBeneficiary.LoadPersonAccountBeneficiarys();
                lobjPersonBeneficiary.ibusPerson.LoadBeneficiaries();

                //Load Beneficiary's Data
                lobjPersonBeneficiary.ibusParticipantBeneficiary = new busPerson();
                if (lobjPersonBeneficiary.icdoRelationship.beneficiary_person_id != 0)
                {
                    lobjPersonBeneficiary.istrBenType = "PER";
                    lobjPersonBeneficiary.ibusParticipantBeneficiary.FindPerson(lobjPersonBeneficiary.icdoRelationship.beneficiary_person_id);
                }
                else
                {
                    lobjPersonBeneficiary.ibusOrganization = new busOrganization();
                    lobjPersonBeneficiary.istrBenType = "ORG";
                    lobjPersonBeneficiary.ibusOrganization.FindOrganization(lobjPersonBeneficiary.icdoRelationship.beneficiary_org_id);
                }

                lobjPersonBeneficiary.LoadAllPersonAccountBeneficiaries();
                lobjPersonBeneficiary.LoadSoftErrors();
            }

            return lobjPersonBeneficiary;
        }

        public busPersonBeneficiary NewPersonBeneficiary(int aintParticipantId, string astrCheckIfParticipant, string astrCheckIfBen, string astrCheckIfAltPayee)
        {
            busPersonBeneficiary lobjPersonBeneficiary = new busPersonBeneficiary { icdoRelationship = new cdoRelationship() };
            lobjPersonBeneficiary.ibusPerson = new busPerson();
            lobjPersonBeneficiary.ibusPerson.FindPerson(aintParticipantId);

            if (astrCheckIfParticipant.IsNotNullOrEmpty() && astrCheckIfBen.IsNotNullOrEmpty() && astrCheckIfAltPayee.IsNotNullOrEmpty())
            {
                lobjPersonBeneficiary.ibusPerson.iblnBeneficiary = astrCheckIfBen;
                lobjPersonBeneficiary.ibusPerson.iblnParticipant = astrCheckIfParticipant;
                lobjPersonBeneficiary.ibusPerson.iblnAlternatePayee = astrCheckIfAltPayee;
            }
            else
            {
                lobjPersonBeneficiary.ibusPerson.LoadInitialData();
            }

            if (lobjPersonBeneficiary.ibusPerson.iblnParticipant == busConstant.YES)
            {
                lobjPersonBeneficiary.icdoRelationship.beneficiary_from_value = "PART";
            }
            else if (lobjPersonBeneficiary.ibusPerson.iblnBeneficiary == busConstant.YES)
            {
                lobjPersonBeneficiary.icdoRelationship.beneficiary_from_value = "SURV";
            }
            else if (lobjPersonBeneficiary.ibusPerson.iblnAlternatePayee == busConstant.YES)
            {
                lobjPersonBeneficiary.icdoRelationship.beneficiary_from_value = "ALTP";
            }

            lobjPersonBeneficiary.iclbPersonAccountBeneficiary = new Collection<busPersonAccountBeneficiary>();

            lobjPersonBeneficiary.ibusParticipantBeneficiary = new busPerson();
            lobjPersonBeneficiary.ibusOrganization = new busOrganization();
            lobjPersonBeneficiary.iclbPersonAccount = new Collection<busPersonAccount>();
            lobjPersonBeneficiary.LoadAllPersonAccountBeneficiaries();
            return lobjPersonBeneficiary;
        }

        public busPersonAccount FindPersonAccount(int aintpersonaccountid)
        {
            busPersonAccount lobjPersonAccount = new busPersonAccount();
            if (lobjPersonAccount.FindPersonAccount(aintpersonaccountid))
            {
            }

            return lobjPersonAccount;
        }

        public busPersonAccountBeneficiary FindPersonAccountBeneficiary(int aintpersonaccountbeneficiaryid)
        {
            busPersonAccountBeneficiary lobjPersonAccountBeneficiary = new busPersonAccountBeneficiary();
            if (lobjPersonAccountBeneficiary.FindPersonAccountBeneficiary(aintpersonaccountbeneficiaryid))
            {
            }

            return lobjPersonAccountBeneficiary;
        }
        public override string GetMessageText(string astrMessage, int aintBusMessageID, int aintButtonMessageID, int aintDefaultMessgeId, params object[] aarrParam)
        {
            if ((iobjPassInfo.istrSenderForm== "wfmQDROApplicationMaintenance" || iobjPassInfo.istrSenderForm == "wfmParticipantBeneficiaryMaintenance") && iobjPassInfo.istrSenderID == "btnSave" && (iobjPassInfo.idictParams.ContainsKey("SaveMesssageDroBenefitDetails" ) || iobjPassInfo.idictParams.ContainsKey("SaveMesssageParticipantBeneficiary")))
            {
                astrMessage = "No changes to save.";
            }
            return base.GetMessageText(astrMessage, aintBusMessageID, aintButtonMessageID, aintDefaultMessgeId, aarrParam);
        }
        protected override void InitializeNewChildObject(object aobjParentObject, busBase aobjChildObject)
        {
            if (iobjPassInfo.istrFormName == "wfmQDROApplicationMaintenance"
                && iobjPassInfo.idictParams["RelatedGridID"].ToString() == busConstant.BENEFIT_INFORMATION_GRID)
            {
                Hashtable Param = (Hashtable)iobjPassInfo.idictParams["MVVMGridItemAddUpdate_" + busConstant.BENEFIT_INFORMATION_GRID];
                if (aobjChildObject is busDroBenefitDetails)
                {
                    busDroBenefitDetails lbusDroBenefitDetails = (busDroBenefitDetails)aobjChildObject;
                    lbusDroBenefitDetails.EvaluateInitialLoadRules();

                    Hashtable lhstParam = new Hashtable();

                    switch (Param["istrSubPlan"])
                    {
                        case busConstant.EE:
                            lbusDroBenefitDetails.icdoDroBenefitDetails.ee_flag = busConstant.FLAG_YES;
                            lbusDroBenefitDetails.istrSubPlanDescription = busConstant.EE;
                            break;
                        case busConstant.UVHP:
                            lbusDroBenefitDetails.icdoDroBenefitDetails.uvhp_flag = busConstant.FLAG_YES;
                            lbusDroBenefitDetails.istrSubPlanDescription = busConstant.UVHP;
                            break;
                        case busConstant.L52_SPL_ACC:
                            lbusDroBenefitDetails.icdoDroBenefitDetails.l52_spl_acc_flag = busConstant.FLAG_YES;
                            lbusDroBenefitDetails.istrSubPlanDescription = "Local-52 Special Account";
                            break;
                        case busConstant.L161_SPL_ACC:
                            lbusDroBenefitDetails.icdoDroBenefitDetails.l161_spl_acc_flag = busConstant.FLAG_YES;
                            lbusDroBenefitDetails.istrSubPlanDescription = "Local-161 Special Account";
                            break;
                        case "":
                            lbusDroBenefitDetails.istrSubPlanDescription = "";
                            break;
                    }

                    string lstrBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
                    IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrBusinessTierUrl);

                    int lintPlanID = 0;
                    if (Param.ContainsKey("icdoDroBenefitDetails.plan_id") && !string.IsNullOrEmpty(Convert.ToString(Param["icdoDroBenefitDetails.plan_id"])))
                    {
                        lintPlanID = Convert.ToInt32(Param["icdoDroBenefitDetails.plan_id"]);
                    }

                    lhstParam.Add(enmPlan.plan_id.ToString().ToUpper(), lintPlanID);

                    DataTable ldtblist = lsrvBusinessTier.ExecuteQuery("cdoPlan.GetPlanById", lhstParam, iobjPassInfo.idictParams);
                    if (ldtblist.Rows.Count > 0)
                    {
                        lbusDroBenefitDetails.icdoDroBenefitDetails.istrPlanName = ldtblist.Rows[0][busConstant.PLAN_NAME].ToString();
                        lbusDroBenefitDetails.icdoDroBenefitDetails.istrPlanCode = ldtblist.Rows[0][busConstant.PLAN_CODE].ToString();
                    }

                    lhstParam.Clear();
                    string istrBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
                    IBusinessTier isrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(istrBusinessTierUrl);

                    string listrBenefitOptionValue = string.Empty;
                    if (Param.ContainsKey("icdoDroBenefitDetails.istrBenefitOptionValue") && !string.IsNullOrEmpty(Convert.ToString(Param["icdoDroBenefitDetails.istrBenefitOptionValue"])))
                    {
                        listrBenefitOptionValue = Convert.ToString(Param["icdoDroBenefitDetails.istrBenefitOptionValue"]);
                    }

                    lhstParam.Add(enmPlanBenefitXr.benefit_option_value.ToString().ToUpper(), listrBenefitOptionValue);
                    lhstParam.Add(enmPlan.plan_id.ToString().ToUpper(), lintPlanID);
                    DataTable ldtbPlanBenefitId = isrvBusinessTier.ExecuteQuery("cdoBenefitApplication.GetPlanBenefitID", lhstParam, iobjPassInfo.idictParams);

                    if (ldtbPlanBenefitId.Rows.Count > 0)
                    {
                        DataRow drRow = ldtbPlanBenefitId.Rows[0];
                        lbusDroBenefitDetails.icdoDroBenefitDetails.plan_benefit_id = Convert.ToInt32(drRow[enmPlanBenefitXr.plan_benefit_id.ToString()]);
                        lbusDroBenefitDetails.icdoDroBenefitDetails.istrBenefitOptionDescription = drRow[busConstant.FIELD_DESCRIPTION].ToString();
                    }
                }
            }
            else if (iobjPassInfo.istrFormName == "wfmParticipantBeneficiaryMaintenance"
                && iobjPassInfo.idictParams["RelatedGridID"].ToString() == busConstant.PERSON_ACCOUNT_BENEFICIARY_GRID)
            {
                Hashtable Param = (Hashtable)iobjPassInfo.idictParams["MVVMGridItemAddUpdate_" + busConstant.PERSON_ACCOUNT_BENEFICIARY_GRID];

                if (aobjChildObject is busPersonAccountBeneficiary)
                {
                    busPersonAccountBeneficiary lbusPersonAccountBeneficiary = (busPersonAccountBeneficiary)aobjChildObject;
                    Hashtable lhstParam = new Hashtable();

                    string lstrBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
                    IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrBusinessTierUrl);

                    int lintPlanID = 0;
                    if (Param.ContainsKey("icdoPersonAccountBeneficiary.iaintPlan") && !string.IsNullOrEmpty(Convert.ToString(Param["icdoPersonAccountBeneficiary.iaintPlan"])))
                    {
                        lintPlanID = Convert.ToInt32(Param["icdoPersonAccountBeneficiary.iaintPlan"]);
                    }

                    lhstParam.Add(enmPlan.plan_id.ToString().ToUpper(), lintPlanID);
                    DataTable ldtblist = lsrvBusinessTier.ExecuteQuery("cdoPlan.GetPlanById", lhstParam, iobjPassInfo.idictParams);
                    if (ldtblist.Rows.Count > 0)
                    {
                        lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.istrPlan = ldtblist.Rows[0][0].ToString();
                    }
                    lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.start_date = Convert.ToDateTime(Param["start_date"]);
                }
            }
            else if (iobjPassInfo.istrFormName == "wfmPersonMaintenance"
                && iobjPassInfo.idictParams["RelatedGridID"].ToString() == busConstant.PERSON_BRIDGED_HOURS_GRID)
            {
                if (aobjChildObject is busPersonBridgeHours)
                {
                    busPersonBridgeHours lobjPersonBridgeHours = aobjChildObject as busPersonBridgeHours;
                    busPerson lobjPerson = (busPerson)aobjParentObject;

                    Hashtable Param = (Hashtable)iobjPassInfo.idictParams["MVVMGridItemAddUpdate_" + busConstant.PERSON_BRIDGED_HOURS_GRID];

                    if (lobjPersonBridgeHours.iclbPersonBridgeHoursDetails == null)
                    {
                        lobjPersonBridgeHours.iclbPersonBridgeHoursDetails = new Collection<busPersonBridgeHoursDetail>();
                    }

                    //Ticket#85664            
                    Hashtable lhstParam = new Hashtable();

                    //Ticket#85664
                    if (Convert.ToString(Param["icdoPersonBridgeHours.bridge_type_value"]) == "DSBL")
                    {
                        Dictionary<string, object> ldictTempParams = new Dictionary<string, object>();
                        string lstrSnn = lobjPerson.icdoPerson.istrSSNNonEncrypted;
                        lhstParam.Add("YR", Convert.ToInt32(Convert.ToDateTime(Param["icdoPersonBridgeHours.bridge_end_date"]).Year));
                        lhstParam.Add("SSN", lstrSnn.Replace("-", ""));

                        string lstrBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
                        IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrBusinessTierUrl);

                        DataTable ldtblist = lsrvBusinessTier.ExecuteQuery("cdoPerson.GetCreditedHoursbySSN", lhstParam, ldictTempParams);

                        decimal lintbrigdeCalculatedHours = 0;

                        if (ldtblist.Rows.Count > 0)
                        {
                            if (Convert.ToDecimal(ldtblist.Rows[0][0]) < 200)
                            {
                                lobjPersonBridgeHours.icdoPersonBridgeHours.hours_reported = 200;
                            }
                        }
                        else
                        {
                            lintbrigdeCalculatedHours = Convert.ToDecimal(ldtblist.Rows[0][0]);
                        }
                        if (lintbrigdeCalculatedHours > 200)
                        {
                            DateTime ldtBridgeStartDate = DateTime.MinValue;
                            if (Param.ContainsKey("icdoPersonBridgeHours.bridge_start_date") && !string.IsNullOrEmpty(Convert.ToString(Param["icdoPersonBridgeHours.bridge_start_date"])))
                            {
                                ldtBridgeStartDate = Convert.ToDateTime(Param["icdoPersonBridgeHours.bridge_start_date"]);
                            }
                            DateTime ldtBridgeEndDate = DateTime.MinValue;
                            if (Param.ContainsKey("icdoPersonBridgeHours.bridge_end_date") && !string.IsNullOrEmpty(Convert.ToString(Param["icdoPersonBridgeHours.bridge_end_date"])))
                            {
                                ldtBridgeEndDate = Convert.ToDateTime(Param["icdoPersonBridgeHours.bridge_end_date"]);
                            }
                            //  lobjPersonBridgeHours.icdoPersonBridgeHours.hours_reported = lintbrigdeCalculatedHours;
                            lobjPersonBridgeHours.icdoPersonBridgeHours.hours_reported = Convert.ToDecimal(busGlobalFunctions.WorkingDays(ldtBridgeStartDate, ldtBridgeEndDate) * 8);
                        }
                    }
                    //RID 119820 - Covid hardship bridging.
                    else if (Convert.ToString(Param["icdoPersonBridgeHours.bridge_type_value"]) == "COVD")
                    {
                        Dictionary<string, object> ldictTempParams = new Dictionary<string, object>();
                        string lstrSnn = lobjPerson.icdoPerson.istrSSNNonEncrypted;
                        lhstParam.Add("YR", Convert.ToInt32(Convert.ToDateTime(Param["icdoPersonBridgeHours.bridge_end_date"]).Year));
                        lhstParam.Add("SSN", lstrSnn.Replace("-", ""));

                        string lstrBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
                        IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrBusinessTierUrl);

                        DataTable ldtblist = lsrvBusinessTier.ExecuteQuery("cdoPerson.GetCreditedHoursbySSN", lhstParam, ldictTempParams);

                        if (ldtblist != null && ldtblist.Rows.Count > 0)
                        {
                            if (Convert.ToDecimal(ldtblist.Rows[0][0]) < 200)
                            {
                                lobjPersonBridgeHours.icdoPersonBridgeHours.hours_reported = 200;
                            }
                            else
                            {
                                lobjPersonBridgeHours.icdoPersonBridgeHours.hours_reported = Convert.ToDecimal(ldtblist.Rows[0][0]);
                            }
                        }
                        else
                        {
                            lobjPersonBridgeHours.icdoPersonBridgeHours.hours_reported = 200;
                        }
                    }
                    //RID 151812 - added new bridge code for MOA 2024.
                    else if (Convert.ToString(Param["icdoPersonBridgeHours.bridge_type_value"]) == "MA24")
                    {
                        Dictionary<string, object> ldictTempParams = new Dictionary<string, object>();
                        string lstrSnn = lobjPerson.icdoPerson.istrSSNNonEncrypted;
                        lhstParam.Add("YR", Convert.ToInt32(Convert.ToDateTime(Param["icdoPersonBridgeHours.bridge_end_date"]).Year));
                        lhstParam.Add("SSN", lstrSnn.Replace("-", ""));

                        string lstrBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
                        IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrBusinessTierUrl);

                        DataTable ldtblist = lsrvBusinessTier.ExecuteQuery("cdoPerson.GetCreditedHoursbySSN", lhstParam, ldictTempParams);

                        if (ldtblist != null && ldtblist.Rows.Count > 0)
                        {
                            if (Convert.ToDecimal(ldtblist.Rows[0][0]) < 200)
                            {
                                lobjPersonBridgeHours.icdoPersonBridgeHours.hours_reported = 200;
                            }
                            else
                            {
                                lobjPersonBridgeHours.icdoPersonBridgeHours.hours_reported = Convert.ToDecimal(ldtblist.Rows[0][0]);
                            }
                        }
                        else
                        {
                            lobjPersonBridgeHours.icdoPersonBridgeHours.hours_reported = 200;
                        }
                    }
                    else
                    {
                        DateTime ldtBridgeStartDate = DateTime.MinValue;
                        if (Param.ContainsKey("icdoPersonBridgeHours.bridge_start_date") && !string.IsNullOrEmpty(Convert.ToString(Param["icdoPersonBridgeHours.bridge_start_date"])))
                        {
                            ldtBridgeStartDate = Convert.ToDateTime(Param["icdoPersonBridgeHours.bridge_start_date"]);
                        }
                        DateTime ldtBridgeEndDate = DateTime.MinValue;
                        if (Param.ContainsKey("icdoPersonBridgeHours.bridge_end_date") && !string.IsNullOrEmpty(Convert.ToString(Param["icdoPersonBridgeHours.bridge_end_date"])))
                        {
                            ldtBridgeEndDate = Convert.ToDateTime(Param["icdoPersonBridgeHours.bridge_end_date"]);
                        }
                        lobjPersonBridgeHours.icdoPersonBridgeHours.hours_reported = Convert.ToDecimal(busGlobalFunctions.WorkingDays(ldtBridgeStartDate, ldtBridgeEndDate) * 8);
                    }
                }
            }
            else if ((iobjPassInfo.istrFormName == "wfmDeathPreRetirementMaintenance" || iobjPassInfo.istrFormName == "wfmDisabilityApplicationMaintenance" ||
                iobjPassInfo.istrFormName == "wfmRetirementApplicationMaintenance" || iobjPassInfo.istrFormName == "wfmWithdrawalApplicationMaintenance")
                && iobjPassInfo.idictParams["RelatedGridID"].ToString() == busConstant.BENEFIT_APPLICATION_DETAIL_GRID)
            {
                if (aobjChildObject is busBenefitApplicationDetail)
                {
                    Hashtable Param = (Hashtable)iobjPassInfo.idictParams["MVVMGridItemAddUpdate_" + busConstant.BENEFIT_APPLICATION_DETAIL_GRID];

                    busBenefitApplicationDetail lbusBenefitApplicationDetail = (busBenefitApplicationDetail)aobjChildObject;
                    lbusBenefitApplicationDetail.EvaluateInitialLoadRules();
                    Hashtable lhstParam = new Hashtable();

                    if (Param["istrSubPlan"].IsNull())
                        lbusBenefitApplicationDetail.istrSubPlan = String.Empty;

                    switch (Param["istrSubPlan"].ToString())
                    {
                        case busConstant.EE:
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.ee_flag = busConstant.FLAG_YES;
                            lbusBenefitApplicationDetail.istrSubPlanDescription = busConstant.EE;
                            break;
                        case busConstant.UVHP:
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.uvhp_flag = busConstant.FLAG_YES;
                            lbusBenefitApplicationDetail.istrSubPlanDescription = busConstant.UVHP;
                            break;
                        case busConstant.EE_UVHP:
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.uvhp_flag = busConstant.FLAG_YES;
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.ee_flag = busConstant.FLAG_YES;
                            lbusBenefitApplicationDetail.istrSubPlanDescription = busConstant.EE_UVHP;
                            break;
                        case busConstant.L52_SPL_ACC:
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.l52_spl_acc_flag = busConstant.FLAG_YES;
                            lbusBenefitApplicationDetail.istrSubPlanDescription = "Local-52 Special Account";
                            break;
                        case busConstant.L161_SPL_ACC:
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.l161_spl_acc_flag = busConstant.FLAG_YES;
                            lbusBenefitApplicationDetail.istrSubPlanDescription = "Local-161 Special Account";
                            break;
                    }

                    int lintPlanID = 0;
                    if (Param.ContainsKey("icdoBenefitApplicationDetail.iintPlan_ID") && !string.IsNullOrEmpty(Convert.ToString(Param["icdoBenefitApplicationDetail.iintPlan_ID"])))
                    {
                        lintPlanID = Convert.ToInt32(Param["icdoBenefitApplicationDetail.iintPlan_ID"]);
                    }

                    lhstParam.Add(enmPlan.plan_id.ToString().ToUpper(), lintPlanID);

                    string lstrBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
                    IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrBusinessTierUrl);

                    DataTable ldtblist = lsrvBusinessTier.ExecuteQuery("cdoPlan.GetPlanById", lhstParam, iobjPassInfo.idictParams);

                    if (ldtblist.Rows.Count > 0)
                    {
                        lbusBenefitApplicationDetail.istrPlanName = ldtblist.Rows[0][0].ToString();
                        lbusBenefitApplicationDetail.istrPlanCode = ldtblist.Rows[0][enmPlan.plan_code.ToString()].ToString();
                    }

                    if (aobjParentObject is busWithdrawalApplication)
                    {
                        if (string.IsNullOrEmpty(Convert.ToString(Param["icdoBenefitApplicationDetail.istrBenefitOptionValue"])))
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = string.Empty;
                    }

                    lhstParam.Clear();
                    if (!string.IsNullOrEmpty(Convert.ToString(Param["icdoBenefitApplicationDetail.istrBenefitOptionValue"])))
                    {
                        lhstParam.Add(enmPlanBenefitXr.benefit_option_value.ToString().ToUpper(), Convert.ToString(Param["icdoBenefitApplicationDetail.istrBenefitOptionValue"]));
                    }
                    else
                    {
                        lhstParam.Add(enmPlanBenefitXr.benefit_option_value.ToString().ToUpper(), Convert.ToString(Param["icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue"]));
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = Convert.ToString(Param["icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue"]);
                    }

                    lhstParam.Add(enmPlan.plan_id.ToString().ToUpper(), lintPlanID);

                    string istrBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
                    IBusinessTier isrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(istrBusinessTierUrl);

                    DataTable ldtbPlanBenefitId = isrvBusinessTier.ExecuteQuery("cdoBenefitApplication.GetPlanBenefitID", lhstParam, iobjPassInfo.idictParams);

                    if (ldtbPlanBenefitId.Rows.Count > 0)
                    {
                        DataRow drRow = ldtbPlanBenefitId.Rows[0];
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.plan_benefit_id = Convert.ToInt32(drRow[busConstant.PLAN_BENEFIT_ID]);
                    }

                    lhstParam.Clear();
                    lhstParam.Add(enmPlanBenefitXr.plan_benefit_id.ToString().ToUpper(), lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.plan_benefit_id);

                    string istrBusinessTierUrl0 = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
                    IBusinessTier isrvBusinessTier0 = WCFClient<IBusinessTier>.CreateChannel(istrBusinessTierUrl);

                    DataTable ldtbBenefitDescription = isrvBusinessTier0.ExecuteQuery("cdoBenefitApplicationDetail.GetBenDescriptionFromID", lhstParam, iobjPassInfo.idictParams);

                    if (ldtbBenefitDescription.Rows.Count > 0)
                    {
                        lbusBenefitApplicationDetail.istrPlanBenefitDescription = ldtbBenefitDescription.Rows[0][0].ToString();
                    }

                    lhstParam.Clear();
                    int aintParticipantId = 0;

                    if (aobjParentObject is busRetirementApplication)
                    {
                        busRetirementApplication lobjRetirementApp = (busRetirementApplication)aobjParentObject;
                        aintParticipantId = lobjRetirementApp.ibusPerson.icdoPerson.person_id;
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_subtype_value = lobjRetirementApp.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == lintPlanID).First().icdoPersonAccount.istrRetirementSubType;
                    }
                    else if (aobjParentObject is busWithdrawalApplication)
                    {
                        busWithdrawalApplication lobjWithdrawalApp = (busWithdrawalApplication)aobjParentObject;
                        aintParticipantId = lobjWithdrawalApp.ibusPerson.icdoPerson.person_id;
                    }
                    else if (aobjParentObject is busDisabilityApplication)
                    {
                        busDisabilityApplication lobjDisabilityApplication = (busDisabilityApplication)aobjParentObject;
                        aintParticipantId = lobjDisabilityApplication.ibusPerson.icdoPerson.person_id;
                    }
                    else if (aobjParentObject is busDeathPreRetirement)
                    {
                        busDeathPreRetirement lobjDeathPreRetirement = (busDeathPreRetirement)aobjParentObject;
                        aintParticipantId = lobjDeathPreRetirement.ibusPerson.icdoPerson.person_id;
                    }

                    int strBeneficiaryId = 0;

                    if (aobjParentObject is busDeathPreRetirement)
                    {
                        // if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.survivor_id != 0)
                        if (Convert.ToString(Param["icdoBenefitApplicationDetail.istrSurvivorTypeValue"]) == busConstant.SURVIVOR_TYPE_PERSON &&
                             Convert.ToInt32(Param["icdoBenefitApplicationDetail.survivor_id"]) != 0)
                        {
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.organization_id = 0;
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrOrganizationName = string.Empty;
                            strBeneficiaryId = Convert.ToInt32(Param["icdoBenefitApplicationDetail.survivor_id"]);
                            lhstParam.Add(enmPerson.person_id.ToString().ToUpper(), strBeneficiaryId);

                            string istrBusinessTierUrl1 = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
                            IBusinessTier isrvBusinessTier1 = WCFClient<IBusinessTier>.CreateChannel(istrBusinessTierUrl);

                            DataTable ldtbBenefitaryFullName = isrvBusinessTier1.ExecuteQuery("cdoBenefitApplication.GetPersonsFullname", lhstParam, iobjPassInfo.idictParams);

                            if (ldtbBenefitaryFullName.Rows.Count > 0)
                            {
                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSurvivorFullName = ldtbBenefitaryFullName.Rows[0][0].ToString();
                            }
                            else
                            {
                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSurvivorFullName = string.Empty;
                            }

                            lhstParam.Clear();
                            lhstParam.Add(enmRelationship.person_id.ToString().ToUpper(), aintParticipantId);
                            lhstParam.Add("SUVIVOR_ID", strBeneficiaryId);
                            lhstParam.Add("PLAN_CODE", lbusBenefitApplicationDetail.istrPlanCode);

                            string istrBusinessTierUrl2 = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
                            IBusinessTier isrvBusinessTier2 = WCFClient<IBusinessTier>.CreateChannel(istrBusinessTierUrl);

                            DataTable ldtbBenefitary = isrvBusinessTier2.ExecuteQuery("cdoBenefitApplication.GetSurvivorDetailsFromSurvivorId", lhstParam, iobjPassInfo.idictParams);

                            if (ldtbBenefitary.Rows.Count > 0)
                            {
                                DataRow ldtrRow = ldtbBenefitary.Rows[0];
                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrRelationShip = Convert.ToString(ldtrRow["RELATIONSHIP"]);
                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.idecPercentage = Convert.ToDecimal(ldtrRow["PERCENTAGE"]);
                            }

                            else
                            {
                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrRelationShip = string.Empty;
                            }
                        }
                        //   else if(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.organization_id != 0)
                        else if (Convert.ToString(Param["icdoBenefitApplicationDetail.istrSurvivorTypeValue"]) == busConstant.SURVIVOR_TYPE_ORG &&
                            Convert.ToInt32(Param["icdoBenefitApplicationDetail.organization_id"]) != 0)
                        {
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.survivor_id = 0;
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSurvivorFullName = string.Empty;
                            strBeneficiaryId = Convert.ToInt32(Param["icdoBenefitApplicationDetail.organization_id"]);
                            lhstParam.Add(enmOrganization.org_id.ToString().ToUpper(), strBeneficiaryId);

                            string istrBusinessTierUrl3 = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
                            IBusinessTier isrvBusinessTier3 = WCFClient<IBusinessTier>.CreateChannel(istrBusinessTierUrl);

                            DataTable ldtbBenefitaryFullName = isrvBusinessTier3.ExecuteQuery("cdoBenefitApplication.GetOrgFullName", lhstParam, iobjPassInfo.idictParams);
                            if (ldtbBenefitaryFullName.Rows.Count > 0)
                            {
                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrOrganizationName = ldtbBenefitaryFullName.Rows[0][0].ToString();
                            }
                            else
                            {
                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrOrganizationName = string.Empty;
                            }

                            lhstParam.Clear();
                            lhstParam.Add(enmRelationship.person_id.ToString().ToUpper(), aintParticipantId);
                            lhstParam.Add(enmRelationship.beneficiary_org_id.ToString().ToUpper(), strBeneficiaryId);
                            DataTable ldtbBenefitary = isrvBusinessTier.ExecuteQuery("cdoBenefitApplication.GetOrgDetailsFromOrgId", lhstParam, iobjPassInfo.idictParams);
                            if (ldtbBenefitary.Rows.Count > 0)
                            {
                                DataRow ldtrRow = ldtbBenefitary.Rows[0];
                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrRelationShip = Convert.ToString(ldtrRow["RELATIONSHIP"]);
                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.idecPercentage = Convert.ToDecimal(ldtrRow["PERCENTAGE"]);
                            }
                            else
                            {
                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrRelationShip = string.Empty;
                            }
                        }
                    }
                    else
                    {

                        if (Convert.ToString(Param["icdoBenefitApplicationDetail.iintJointAnnuaintID"]).IsNotNullOrEmpty() && Convert.ToInt32(Param["icdoBenefitApplicationDetail.iintJointAnnuaintID"]) != 0)
                        {
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.joint_annuitant_id = Convert.ToInt32(Param["icdoBenefitApplicationDetail.iintJointAnnuaintID"]);
                        }
                        if (Convert.ToString(Param["icdoBenefitApplicationDetail.iintJointAnnuaintID"]).IsNotNullOrEmpty())
                        {
                            strBeneficiaryId = Convert.ToInt32(Param["icdoBenefitApplicationDetail.iintJointAnnuaintID"]);
                        }
                        lhstParam.Add(enmRelationship.person_id.ToString(), aintParticipantId);
                        lhstParam.Add(enmRelationship.beneficiary_person_id.ToString(), strBeneficiaryId);

                        string istrBusinessTierUrl4 = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
                        IBusinessTier isrvBusinessTier4 = WCFClient<IBusinessTier>.CreateChannel(istrBusinessTierUrl);

                        object larrTemp = isrvBusinessTier4.ExecuteMethod("LoadParticipantDetails", lhstParam, true, iobjPassInfo.idictParams);

                        if (larrTemp is ArrayList && (larrTemp as ArrayList).Count > 0)
                        {
                            ArrayList larr = larrTemp as ArrayList;
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrFullName = larr[0].ToString();
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.idtDOB = Convert.ToDateTime(larr[3]);
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrRelationShip = larr[1].ToString();
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrGender = larr[2].ToString();

                        }
                        else
                        {
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrFullName = string.Empty;
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrDOB = string.Empty;
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrRelationShip = string.Empty;
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrGender = string.Empty;
                        }
                    }
                }
            }
            base.InitializeNewChildObject(aobjParentObject, aobjChildObject);
        }
        protected override ArrayList ValidateGridUpdateChild(string astrFormName, object aobjParentObject, object aobjChildObject, Hashtable ahstParams)
        {
            ArrayList iarrResult = new ArrayList();
            utlError lobjError = null;
            busMainBase lbusMainBase = new busMainBase();
            if (astrFormName == busConstant.PERSON_OVERVIEW_MAINTENANCE)
            {
                DateTime ldtReceivedDate = DateTime.MinValue; DateTime ldtResumptionDate = DateTime.MinValue;
                DateTime ldtConfirmationDate = DateTime.MinValue;
                ldtReceivedDate = Convert.ToDateTime(ahstParams["icdoPensionVerificationHistory.received_date"]);
                ldtConfirmationDate = Convert.ToDateTime(ahstParams["icdoPensionVerificationHistory.verification_confirmation_letter_sent"]);
                ldtResumptionDate = Convert.ToDateTime(ahstParams["icdoPensionVerificationHistory.resumption_letter_sent"]);
                if (ldtReceivedDate > DateTime.MinValue && ldtReceivedDate.ToShortDateString() != DateTime.Now.ToShortDateString())
                {
                    lobjError = lbusMainBase.AddError(0, "Only today's date is allowed");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (ldtReceivedDate > DateTime.MinValue && ldtConfirmationDate > DateTime.MinValue && ldtReceivedDate > ldtConfirmationDate)
                {
                    lobjError = lbusMainBase.AddError(6291, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (ldtResumptionDate > DateTime.MinValue && ldtConfirmationDate > DateTime.MinValue && ldtResumptionDate > ldtConfirmationDate)
                {
                    lobjError = lbusMainBase.AddError(6292, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if (ldtReceivedDate > DateTime.MinValue && ldtResumptionDate > DateTime.MinValue && ldtReceivedDate > ldtResumptionDate)
                {
                    lobjError = lbusMainBase.AddError(6293, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
            }
            else if (astrFormName == busConstant.PARTICIPANT_BENEFICIARY_MAINTENANCE)
            {
                int lintPlanId = 0; DateTime ldtStrtTime = DateTime.MinValue;
                string lstrBenefitType = string.Empty; DateTime ldtEndTime = DateTime.MinValue;
                decimal ldecPercent = 0; string lstrIsPrimary = string.Empty;
                ldtStrtTime = Convert.ToDateTime(ahstParams["icdoPersonAccountBeneficiary.start_date"]);
                ldtEndTime = Convert.ToDateTime(ahstParams["icdoPersonAccountBeneficiary.end_date"]);
                lstrBenefitType = Convert.ToString(ahstParams["icdoPersonAccountBeneficiary.beneficiary_type_value"]);

                if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoPersonAccountBeneficiary.iaintPlan"])))
                {
                    lintPlanId = Convert.ToInt32(ahstParams["icdoPersonAccountBeneficiary.iaintPlan"]);
                }
                if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["icdoPersonAccountBeneficiary.dist_percent"])))
                {
                    ldecPercent = Convert.ToDecimal(ahstParams["icdoPersonAccountBeneficiary.dist_percent"]);
                }
                if (lintPlanId == 0)
                {
                    lobjError = lbusMainBase.AddError(1126, "");
                    iarrResult.Add(lobjError);
                }
                if (lintPlanId != 9)
                {
                    if (string.IsNullOrEmpty(lstrBenefitType))
                    {
                        lobjError = lbusMainBase.AddError(1127, "");
                        iarrResult.Add(lobjError);
                    }
                    if (ldecPercent > 100)
                    {
                        lobjError = lbusMainBase.AddError(1121, "");
                        iarrResult.Add(lobjError);
                    }
                    if (ldecPercent == 0)
                    {
                        lobjError = lbusMainBase.AddError(1128, "");
                        iarrResult.Add(lobjError);
                    }
                    else if (IsNonNegative(ldecPercent.ToString()))
                    {
                        lobjError = lbusMainBase.AddError(5059, "");
                        iarrResult.Add(lobjError);
                    }
                    if (ldtStrtTime == DateTime.MinValue)
                    {
                        lobjError = lbusMainBase.AddError(1123, "");
                        iarrResult.Add(lobjError);
                    }
                    if (ldtEndTime != DateTime.MinValue && ldtEndTime < ldtStrtTime)
                    {
                        lobjError = lbusMainBase.AddError(1122, "");
                        iarrResult.Add(lobjError);
                    }
                }
                if (iarrResult.Count > 0)
                {
                    return iarrResult;
                }
            }
            else if (astrFormName == busConstant.QRDO_MAINTAINENCE)
            {
                busQdroApplication lobjQdroApplication = aobjParentObject as busQdroApplication;

                decimal ldecPercentage = 0; DateTime ldtNetInvestmentFromDate = DateTime.MinValue;
                int lintPlanID = 0; decimal ldecAmount = 0; DateTime ldtNetInvestmentToDate = DateTime.MinValue;
                string lstrtDroModel = string.Empty; string istrSubPlan = string.Empty;
                istrSubPlan = Convert.ToString(ahstParams["istrSubPlan"]);
                lstrtDroModel = Convert.ToString(ahstParams["icdoDroBenefitDetails.dro_model_value"]);
                ldtNetInvestmentFromDate = Convert.ToDateTime(ahstParams["icdoDroBenefitDetails.net_investment_from_date"]);
                ldtNetInvestmentToDate = Convert.ToDateTime(ahstParams["icdoDroBenefitDetails.net_investment_to_date"]);

                if (Convert.ToString(ahstParams["icdoDroBenefitDetails.plan_id"]).IsNotNullOrEmpty())
                {
                    lintPlanID = Convert.ToInt32(ahstParams["icdoDroBenefitDetails.plan_id"]);
                }
                if (Convert.ToString(ahstParams["icdoDroBenefitDetails.benefit_perc"]).IsNotNullOrEmpty())
                {
                    ldecPercentage = Convert.ToDecimal(ahstParams["icdoDroBenefitDetails.benefit_perc"]);
                }
                if (Convert.ToString(ahstParams["icdoDroBenefitDetails.benefit_amt"]).IsNotNullOrEmpty())
                {
                    ldecAmount = Convert.ToDecimal(ahstParams["icdoDroBenefitDetails.benefit_amt"]);
                }
                if (ldecAmount != 0 || ldecPercentage != 0)
                {
                    if (IsNegative(ldecAmount.ToString()) || IsNegative(ldecPercentage.ToString()))
                    {
                        lobjError = lbusMainBase.AddError(2020, "");
                        iarrResult.Add(lobjError);
                    }
                    if (ldecPercentage > 100)
                    {
                        lobjError = lbusMainBase.AddError(2020, "");
                        iarrResult.Add(lobjError);
                    }
                }
                if (ldecPercentage > 100)
                {
                    lobjError = lbusMainBase.AddError(1121, "");
                    iarrResult.Add(lobjError);
                }
                if (lintPlanID == 1 && ((ldtNetInvestmentFromDate == DateTime.MinValue && ldtNetInvestmentToDate > DateTime.MinValue)) ||
                    (ldtNetInvestmentFromDate > DateTime.MinValue && ldtNetInvestmentFromDate == DateTime.MinValue))
                {
                    lobjError = lbusMainBase.AddError(6275, "");
                    iarrResult.Add(lobjError);
                }
                int idecTotalAmount = (from obj in lobjQdroApplication.iclbDroBenefitDetails
                                       where obj.icdoDroBenefitDetails.plan_id == lintPlanID
                                       && obj.istrSubPlan == istrSubPlan && obj.icdoDroBenefitDetails.dro_benefit_id != Convert.ToInt32(ahstParams["icdoDroBenefitDetails.dro_benefit_id"])
                                       select obj).Count();
                if (idecTotalAmount >= 1)
                {
                    lobjError = lbusMainBase.AddError(2009, "");
                    iarrResult.Add(lobjError);
                }
                foreach (busDroBenefitDetails lbusDroBenefitDetails in lobjQdroApplication.iclbOtherQLFDDroBenefitDetails)
                {
                    if (lbusDroBenefitDetails.icdoDroBenefitDetails.plan_id == lintPlanID && lbusDroBenefitDetails.istrSubPlan == istrSubPlan)
                    {
                        lobjError = lbusMainBase.AddError(2009, "");
                        iarrResult.Add(lobjError);
                    }
                    if (lstrtDroModel == "SPDQ")
                    {
                        lobjError = lbusMainBase.AddError(1165, "");
                        iarrResult.Add(lobjError);
                    }
                }
                if (iarrResult.Count > 0)
                {
                    return iarrResult;
                }
            }
            else if (astrFormName == busConstant.PERSON_MAINTENANCE)
            {
                DateTime txtBridgeStartDate = DateTime.MinValue; DateTime txtBridgeEndDate = DateTime.MinValue; bool lblnValidDates = true;

                if (Convert.ToString(ahstParams["icdoPersonBridgeHours.bridge_type_value"]).IsNullOrEmpty())
                {
                    lobjError = lbusMainBase.AddError(5135, "");
                    iarrResult.Add(lobjError);
                }
                if (Convert.ToString(ahstParams["icdoPersonBridgeHours.bridge_start_date"]).IsNullOrEmpty() || Convert.ToDateTime(ahstParams["icdoPersonBridgeHours.bridge_start_date"]) == DateTime.MinValue)
                {
                    lblnValidDates = false;
                    lobjError = lbusMainBase.AddError(5137, "");
                    iarrResult.Add(lobjError);
                }
                if (Convert.ToString(ahstParams["icdoPersonBridgeHours.bridge_end_date"]).IsNullOrEmpty() || Convert.ToDateTime(ahstParams["icdoPersonBridgeHours.bridge_end_date"]) == DateTime.MinValue)
                {
                    lblnValidDates = false;
                    lobjError = lbusMainBase.AddError(5138, "");
                    iarrResult.Add(lobjError);
                }
                txtBridgeStartDate = Convert.ToDateTime(ahstParams["icdoPersonBridgeHours.bridge_start_date"]);
                txtBridgeEndDate = Convert.ToDateTime(ahstParams["icdoPersonBridgeHours.bridge_end_date"]);

                if (lblnValidDates)
                {
                    if (txtBridgeStartDate > txtBridgeEndDate)
                    {
                        lobjError = lbusMainBase.AddError(5139, "");
                        iarrResult.Add(lobjError);
                    }
                    if (txtBridgeEndDate > busGlobalFunctions.GetLastDateOfComputationYear(Convert.ToDateTime(txtBridgeEndDate).Year))
                    {
                        lobjError = lbusMainBase.AddError(0, "Bridge End Date cannot be Greater than Last Date of Computation Year");
                        iarrResult.Add(lobjError);
                    }
                    busPerson objPersonBase = aobjParentObject as busPerson;
                    foreach (busPersonBridgeHours lbusPersonBridgeHours in objPersonBase.iclbPersonBridgeHours)
                    {
                        if (ahstParams["icdoPersonBridgeHours.person_bridge_id"] != "" && lbusPersonBridgeHours.icdoPersonBridgeHours.person_bridge_id != Convert.ToInt32(ahstParams["icdoPersonBridgeHours.person_bridge_id"]))
                        {
                            //UAT PIR 130
                            if (busGlobalFunctions.CheckDateOverlapping(txtBridgeStartDate,
                                                                    lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_start_date, lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_end_date) ||
                                busGlobalFunctions.CheckDateOverlapping(txtBridgeEndDate,
                                                                    lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_start_date, lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_end_date) ||
                                busGlobalFunctions.CheckDateOverlapping(lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_start_date, txtBridgeStartDate, txtBridgeEndDate) ||
                                busGlobalFunctions.CheckDateOverlapping(lbusPersonBridgeHours.icdoPersonBridgeHours.bridge_end_date, txtBridgeStartDate, txtBridgeEndDate))
                            {
                                lobjError = lbusMainBase.AddError(5141, "");
                                iarrResult.Add(lobjError);
                            }
                        }
                    }
                }
                if (iarrResult.Count > 0)
                {
                    return iarrResult;
                }
            }
            else if (astrFormName == busConstant.DEATH_PRE_RETIREMENT_MAINTANENCE)
            {
                bool lblnCheckIfVestedForL161 = busConstant.BOOL_FALSE; int iintPlanID = 0; string istrSurvivorTypeValue = string.Empty;
                bool lblnCheckIfVestedForL700 = busConstant.BOOL_FALSE; string istrBenefitOptionValue = string.Empty; string istrSubPlan = string.Empty;

                busBenefitApplication abusBenefitApplication = aobjParentObject as busBenefitApplication;
                lblnCheckIfVestedForL161 = CheckAlreadyVested(abusBenefitApplication, busConstant.Local_161);
                
                istrBenefitOptionValue = Convert.ToString(ahstParams["istrBenefitOptionValue"]);
                istrSurvivorTypeValue = Convert.ToString(ahstParams["icdoBenefitApplicationDetail.istrSurvivorTypeValue"]);
                istrSubPlan = Convert.ToString(ahstParams["icdoBenefitApplicationDetail.istrSubPlan"]);

                if (!string.IsNullOrEmpty(Convert.ToString(ahstParams["iintPlan_ID"])))
                {
                    iintPlanID = Convert.ToInt32(ahstParams["iintPlan_ID"]);
                }
                foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in abusBenefitApplication.iclbBenefitApplicationDetail)
                {
                    if (Convert.ToString(ahstParams["benefit_application_detail_id"]) != string.Empty && lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id == Convert.ToInt32(ahstParams["icdoBenefitApplicationDetail.benefit_application_detail_id"]))
                    {
                        lbusBenefitApplicationDetail.istrSubPlanDescription = Convert.ToString(ahstParams["istrSubPlan"]);
                        lbusBenefitApplicationDetail.istrSubPlan = Convert.ToString(ahstParams["istrSubPlan"]);
                    }
                }

                if (iintPlanID == busConstant.LOCAL_161_PLAN_ID && istrBenefitOptionValue == busConstant.LIFE_ANNUTIY
                    && istrSurvivorTypeValue == busConstant.SURVIVOR_TYPE_PERSON && (lblnCheckIfVestedForL161 == false || abusBenefitApplication.QualifiedSpouseExistsForPlan == false))
                {
                    lobjError = lbusMainBase.AddError(5429, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                lblnCheckIfVestedForL700 = CheckAlreadyVested(abusBenefitApplication, busConstant.LOCAL_700);
                if (iintPlanID == busConstant.LOCAL_700_PLAN_ID && istrBenefitOptionValue == busConstant.LIFE_ANNUTIY
                              && istrSurvivorTypeValue == busConstant.SURVIVOR_TYPE_PERSON && (lblnCheckIfVestedForL700 == false || abusBenefitApplication.QualifiedSpouseExistsForPlan == false))
                {
                    lobjError = lbusMainBase.AddError(5429, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
                if ((abusBenefitApplication.icdoBenefitApplication.istrIsPersonVestedinMPI == busConstant.FLAG_NO || abusBenefitApplication.QualifiedSpouseExistsForPlan == false) && iintPlanID == busConstant.MPIPP_PLAN_ID
                    && Convert.ToString(ahstParams["istrBenefitOptionValue"]) == busConstant.LIFE_ANNUTIY && istrSubPlan == "" && istrSurvivorTypeValue == busConstant.SURVIVOR_TYPE_PERSON)
                {
                    lobjError = lbusMainBase.AddError(5429, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
            }
            else if (astrFormName == busConstant.WITHDRAWAL_APPLICATION_MAINTAINENCE)
            {
                int iintPlan = 0; string istrSubPlan = string.Empty;
                istrSubPlan = Convert.ToString(ahstParams["istrSubPlan"]);

                busBenefitApplication lbusBenefitApplication = aobjParentObject as busBenefitApplication;
                foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in lbusBenefitApplication.iclbBenefitApplicationDetail)
                {
                    if (Convert.ToString(ahstParams["icdoBenefitApplicationDetail.benefit_application_detail_id"]) != string.Empty && lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id == Convert.ToInt32(ahstParams["icdoBenefitApplicationDetail.benefit_application_detail_id"]))
                    {
                        lbusBenefitApplicationDetail.istrSubPlanDescription = Convert.ToString(ahstParams["istrSubPlan"]);
                        lbusBenefitApplicationDetail.istrSubPlan = Convert.ToString(ahstParams["istrSubPlan"]);
                    }
                }
                if (Convert.ToString(ahstParams["iintPlan_ID"]).IsNotNullOrEmpty())
                {
                    iintPlan = Convert.ToInt32(ahstParams["iintPlan_ID"]);
                }
                if (iintPlan == busConstant.MPIPP_PLAN_ID && istrSubPlan.IsNullOrEmpty())
                {
                    lobjError = lbusMainBase.AddError(5424, "");
                    iarrResult.Add(lobjError);
                    return iarrResult;
                }
            }
            else if (astrFormName == busConstant.RETIREMENT_APPLICATION_MAINTAINENCE || astrFormName == busConstant.DISABILITY_APPLICATION_MAINTAINENCE)
            {
                busBenefitApplication lbusBenefitApplication = aobjParentObject as busBenefitApplication;
                foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in lbusBenefitApplication.iclbBenefitApplicationDetail)
                {
                    if (Convert.ToString(ahstParams["icdoBenefitApplicationDetail.benefit_application_detail_id"]) != string.Empty && lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_application_detail_id == Convert.ToInt32(ahstParams["icdoBenefitApplicationDetail.benefit_application_detail_id"]))
                    {
                        lbusBenefitApplicationDetail.istrSubPlanDescription = Convert.ToString(ahstParams["istrSubPlan"]);
                        lbusBenefitApplicationDetail.istrSubPlan = Convert.ToString(ahstParams["istrSubPlan"]);
                    }
                }
            }
            if (astrFormName == "wfmQDROApplicationMaintenance"
                && iobjPassInfo.idictParams["RelatedGridID"].ToString() == busConstant.BENEFIT_INFORMATION_GRID)
            {
                if (aobjChildObject is busDroBenefitDetails)
                {
                    busDroBenefitDetails lbusDroBenefitDetails = (busDroBenefitDetails)aobjChildObject;

                    Hashtable lhstParam = new Hashtable();

                    switch (lbusDroBenefitDetails.istrSubPlan)
                    {
                        case busConstant.EE:
                            lbusDroBenefitDetails.icdoDroBenefitDetails.ee_flag = busConstant.FLAG_YES;
                            lbusDroBenefitDetails.istrSubPlanDescription = busConstant.EE;
                            break;
                        case busConstant.UVHP:
                            lbusDroBenefitDetails.icdoDroBenefitDetails.uvhp_flag = busConstant.FLAG_YES;
                            lbusDroBenefitDetails.istrSubPlanDescription = busConstant.UVHP;
                            break;
                        case busConstant.L52_SPL_ACC:
                            lbusDroBenefitDetails.icdoDroBenefitDetails.l52_spl_acc_flag = busConstant.FLAG_YES;
                            lbusDroBenefitDetails.istrSubPlanDescription = "Local-52 Special Account";
                            break;
                        case busConstant.L161_SPL_ACC:
                            lbusDroBenefitDetails.icdoDroBenefitDetails.l161_spl_acc_flag = busConstant.FLAG_YES;
                            lbusDroBenefitDetails.istrSubPlanDescription = "Local-161 Special Account";
                            break;
                        case "":
                            lbusDroBenefitDetails.istrSubPlanDescription = "";
                            break;
                    }

                    lhstParam.Add(enmPlan.plan_id.ToString().ToUpper(), lbusDroBenefitDetails.icdoDroBenefitDetails.plan_id);

                    string lstrBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
                    IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrBusinessTierUrl);

                    DataTable ldtblist = lsrvBusinessTier.ExecuteQuery("cdoPlan.GetPlanById", lhstParam, iobjPassInfo.idictParams);
                    if (ldtblist.Rows.Count > 0)
                    {
                        lbusDroBenefitDetails.icdoDroBenefitDetails.istrPlanName = ldtblist.Rows[0][busConstant.PLAN_NAME].ToString();
                        lbusDroBenefitDetails.icdoDroBenefitDetails.istrPlanCode = ldtblist.Rows[0][busConstant.PLAN_CODE].ToString();
                    }

                    lhstParam.Clear();

                    string istrBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
                    IBusinessTier isrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(istrBusinessTierUrl);

                    lhstParam.Add(enmPlanBenefitXr.benefit_option_value.ToString().ToUpper(), lbusDroBenefitDetails.icdoDroBenefitDetails.istrBenefitOptionValue);
                    lhstParam.Add(enmPlan.plan_id.ToString().ToUpper(), lbusDroBenefitDetails.icdoDroBenefitDetails.plan_id);
                    DataTable ldtbPlanBenefitId = isrvBusinessTier.ExecuteQuery("cdoBenefitApplication.GetPlanBenefitID", lhstParam, iobjPassInfo.idictParams);

                    if (ldtbPlanBenefitId.Rows.Count > 0)
                    {
                        DataRow drRow = ldtbPlanBenefitId.Rows[0];
                        lbusDroBenefitDetails.icdoDroBenefitDetails.plan_benefit_id = Convert.ToInt32(drRow[enmPlanBenefitXr.plan_benefit_id.ToString()]);
                        lbusDroBenefitDetails.icdoDroBenefitDetails.istrBenefitOptionDescription = drRow[busConstant.FIELD_DESCRIPTION].ToString();
                    }
                }
            }
            else if (iobjPassInfo.istrFormName == "wfmParticipantBeneficiaryMaintenance"
                && iobjPassInfo.idictParams["RelatedGridID"].ToString() == busConstant.PERSON_ACCOUNT_BENEFICIARY_GRID)
            {
                if (aobjChildObject is busPersonAccountBeneficiary)
                {
                    busPersonAccountBeneficiary lbusPersonAccountBeneficiary = (busPersonAccountBeneficiary)aobjChildObject;
                    Hashtable lhstParam = new Hashtable();

                    string istrBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
                    IBusinessTier isrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(istrBusinessTierUrl);

                    lhstParam.Add(enmPlan.plan_id.ToString().ToUpper(), lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.iaintPlan);
                    DataTable ldtblist = isrvBusinessTier.ExecuteQuery("cdoPlan.GetPlanById", lhstParam, iobjPassInfo.idictParams);

                    if (ldtblist.Rows.Count > 0)
                    {
                        lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.istrPlan = ldtblist.Rows[0][0].ToString();
                    }
                }
            }
            else if (iobjPassInfo.istrFormName == "wfmPersonMaintenance"
                && iobjPassInfo.idictParams["RelatedGridID"].ToString() == busConstant.PERSON_BRIDGED_HOURS_GRID)
            {
                if (aobjChildObject is busPersonBridgeHours)
                {
                    busPersonBridgeHours lobjPersonBridgeHours = aobjChildObject as busPersonBridgeHours;
                    busPerson lobjPerson = (busPerson)aobjParentObject;

                    if (lobjPersonBridgeHours.iclbPersonBridgeHoursDetails == null)
                    {
                        lobjPersonBridgeHours.iclbPersonBridgeHoursDetails = new Collection<busPersonBridgeHoursDetail>();
                    }

                    //Ticket#85664            
                    Hashtable lhstParam = new Hashtable();

                    //Ticket#85664
                    if (lobjPersonBridgeHours.icdoPersonBridgeHours.bridge_type_value == "DSBL")
                    {
                        Dictionary<string, object> ldictTempParams = new Dictionary<string, object>();
                        string lstrSnn = lobjPerson.icdoPerson.istrSSNNonEncrypted;
                        lhstParam.Add("YR", Convert.ToInt32(lobjPersonBridgeHours.icdoPersonBridgeHours.bridge_end_date.Year));
                        lhstParam.Add("SSN", lstrSnn.Replace("-", ""));

                        string lstrBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
                        IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrBusinessTierUrl);

                        DataTable ldtblist = lsrvBusinessTier.ExecuteQuery("cdoPerson.GetCreditedHoursbySSN", lhstParam, ldictTempParams);

                        decimal lintbrigdeCalculatedHours = 0;

                        if (ldtblist.Rows.Count > 0)
                        {
                            if (Convert.ToDecimal(ldtblist.Rows[0][0]) < 200)
                            {
                                //  lintbrigdeCalculatedHours = Convert.ToDecimal(ldtblist.Rows[0][0]);

                                lobjPersonBridgeHours.icdoPersonBridgeHours.hours_reported = 200;
                            }
                        }
                        else
                        {
                            lintbrigdeCalculatedHours = Convert.ToDecimal(ldtblist.Rows[0][0]);

                        }
                        if (lintbrigdeCalculatedHours > 200)
                        {
                            //  lobjPersonBridgeHours.icdoPersonBridgeHours.hours_reported = lintbrigdeCalculatedHours;
                            lobjPersonBridgeHours.icdoPersonBridgeHours.hours_reported = Convert.ToDecimal(busGlobalFunctions.WorkingDays(lobjPersonBridgeHours.icdoPersonBridgeHours.bridge_start_date, lobjPersonBridgeHours.icdoPersonBridgeHours.bridge_end_date) * 8);
                        }
                    }
                    //RID 119820 - Covid hardship bridging.
                    else if (lobjPersonBridgeHours.icdoPersonBridgeHours.bridge_type_value == "COVD")
                    {
                        Dictionary<string, object> ldictTempParams = new Dictionary<string, object>();
                        string lstrSnn = lobjPerson.icdoPerson.istrSSNNonEncrypted;
                        lhstParam.Add("YR", Convert.ToInt32(lobjPersonBridgeHours.icdoPersonBridgeHours.bridge_end_date.Year));
                        lhstParam.Add("SSN", lstrSnn.Replace("-", ""));

                        string lstrBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
                        IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrBusinessTierUrl);

                        DataTable ldtblist = lsrvBusinessTier.ExecuteQuery("cdoPerson.GetCreditedHoursbySSN", lhstParam, ldictTempParams);

                        if (ldtblist != null && ldtblist.Rows.Count > 0)
                        {
                            if (Convert.ToDecimal(ldtblist.Rows[0][0]) < 200)
                            {
                                lobjPersonBridgeHours.icdoPersonBridgeHours.hours_reported = 200;
                            }
                            else
                            {
                                lobjPersonBridgeHours.icdoPersonBridgeHours.hours_reported = Convert.ToDecimal(ldtblist.Rows[0][0]);
                            }
                        }
                        else
                        {
                            lobjPersonBridgeHours.icdoPersonBridgeHours.hours_reported = 200;
                        }
                    }
                    //RID 151812 - added new bridge code for MOA 2024.
                    else if (lobjPersonBridgeHours.icdoPersonBridgeHours.bridge_type_value == "MA24")
                    {
                        Dictionary<string, object> ldictTempParams = new Dictionary<string, object>();
                        string lstrSnn = lobjPerson.icdoPerson.istrSSNNonEncrypted;
                        lhstParam.Add("YR", Convert.ToInt32(lobjPersonBridgeHours.icdoPersonBridgeHours.bridge_end_date.Year));
                        lhstParam.Add("SSN", lstrSnn.Replace("-", ""));

                        string lstrBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
                        IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrBusinessTierUrl);

                        DataTable ldtblist = lsrvBusinessTier.ExecuteQuery("cdoPerson.GetCreditedHoursbySSN", lhstParam, ldictTempParams);

                        if (ldtblist != null && ldtblist.Rows.Count > 0)
                        {
                            if (Convert.ToDecimal(ldtblist.Rows[0][0]) < 200)
                            {
                                lobjPersonBridgeHours.icdoPersonBridgeHours.hours_reported = 200;
                            }
                            else
                            {
                                lobjPersonBridgeHours.icdoPersonBridgeHours.hours_reported = Convert.ToDecimal(ldtblist.Rows[0][0]);
                            }
                        }
                        else
                        {
                            lobjPersonBridgeHours.icdoPersonBridgeHours.hours_reported = 200;
                        }
                    }
                    else
                    {
                        lobjPersonBridgeHours.icdoPersonBridgeHours.hours_reported = Convert.ToDecimal(busGlobalFunctions.WorkingDays(lobjPersonBridgeHours.icdoPersonBridgeHours.bridge_start_date, lobjPersonBridgeHours.icdoPersonBridgeHours.bridge_end_date) * 8);

                    }
                }
            }
            else if ((iobjPassInfo.istrFormName == "wfmDeathPreRetirementMaintenance" || iobjPassInfo.istrFormName == "wfmDisabilityApplicationMaintenance"
                || iobjPassInfo.istrFormName == "wfmRetirementApplicationMaintenance" || iobjPassInfo.istrFormName == "wfmWithdrawalApplicationMaintenance")
                && iobjPassInfo.idictParams["RelatedGridID"].ToString() == busConstant.BENEFIT_APPLICATION_DETAIL_GRID)
            {
                if (aobjChildObject is busBenefitApplicationDetail)
                {
                    busBenefitApplicationDetail lbusBenefitApplicationDetail = (busBenefitApplicationDetail)aobjChildObject;
                    Hashtable lhstParam = new Hashtable();

                    if (lbusBenefitApplicationDetail.istrSubPlan.IsNull())
                        lbusBenefitApplicationDetail.istrSubPlan = String.Empty;

                    switch (lbusBenefitApplicationDetail.istrSubPlan)
                    {
                        case busConstant.EE:
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.ee_flag = busConstant.FLAG_YES;
                            lbusBenefitApplicationDetail.istrSubPlanDescription = busConstant.EE;
                            break;
                        case busConstant.UVHP:
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.uvhp_flag = busConstant.FLAG_YES;
                            lbusBenefitApplicationDetail.istrSubPlanDescription = busConstant.UVHP;
                            break;
                        case busConstant.EE_UVHP:
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.uvhp_flag = busConstant.FLAG_YES;
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.ee_flag = busConstant.FLAG_YES;
                            lbusBenefitApplicationDetail.istrSubPlanDescription = busConstant.EE_UVHP;
                            break;
                        case busConstant.L52_SPL_ACC:
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.l52_spl_acc_flag = busConstant.FLAG_YES;
                            lbusBenefitApplicationDetail.istrSubPlanDescription = "Local-52 Special Account";
                            break;
                        case busConstant.L161_SPL_ACC:
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.l161_spl_acc_flag = busConstant.FLAG_YES;
                            lbusBenefitApplicationDetail.istrSubPlanDescription = "Local-161 Special Account";
                            break;
                    }

                    lhstParam.Add(enmPlan.plan_id.ToString().ToUpper(), lbusBenefitApplicationDetail.iintPlan_ID);

                    string lstrBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
                    IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrBusinessTierUrl);

                    DataTable ldtblist = lsrvBusinessTier.ExecuteQuery("cdoPlan.GetPlanById", lhstParam, iobjPassInfo.idictParams);

                    if (ldtblist.Rows.Count > 0)
                    {
                        lbusBenefitApplicationDetail.istrPlanName = ldtblist.Rows[0][0].ToString();
                        lbusBenefitApplicationDetail.istrPlanCode = ldtblist.Rows[0][enmPlan.plan_code.ToString()].ToString();
                    }

                    if (aobjParentObject is busWithdrawalApplication)
                    {
                        if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue.IsNullOrEmpty())
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = string.Empty;
                    }

                    lhstParam.Clear();
                    if (Convert.ToString(ahstParams["istrBenefitOptionValue"]).IsNotNullOrEmpty())
                    {
                        lhstParam.Add(enmPlanBenefitXr.benefit_option_value.ToString().ToUpper(), Convert.ToString(ahstParams["istrBenefitOptionValue"]));
                    }
                    else
                    {
                        lhstParam.Add(enmPlanBenefitXr.benefit_option_value.ToString().ToUpper(), lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue);
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue;
                    }

                    lhstParam.Add(enmPlan.plan_id.ToString().ToUpper(), lbusBenefitApplicationDetail.iintPlan_ID);

                    string istrBusinessTierUrl = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
                    IBusinessTier isrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(istrBusinessTierUrl);

                    DataTable ldtbPlanBenefitId = isrvBusinessTier.ExecuteQuery("cdoBenefitApplication.GetPlanBenefitID", lhstParam, iobjPassInfo.idictParams);

                    if (ldtbPlanBenefitId.Rows.Count > 0)
                    {
                        DataRow drRow = ldtbPlanBenefitId.Rows[0];
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.plan_benefit_id = Convert.ToInt32(drRow[busConstant.PLAN_BENEFIT_ID]);
                    }

                    lhstParam.Clear();
                    lhstParam.Add(enmPlanBenefitXr.plan_benefit_id.ToString().ToUpper(), lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.plan_benefit_id);

                    string istrBusinessTierUrl0 = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
                    IBusinessTier isrvBusinessTier0 = WCFClient<IBusinessTier>.CreateChannel(istrBusinessTierUrl);

                    DataTable ldtbBenefitDescription = isrvBusinessTier0.ExecuteQuery("cdoBenefitApplicationDetail.GetBenDescriptionFromID", lhstParam, iobjPassInfo.idictParams);

                    if (ldtbBenefitDescription.Rows.Count > 0)
                    {
                        lbusBenefitApplicationDetail.istrPlanBenefitDescription = ldtbBenefitDescription.Rows[0][0].ToString();
                    }

                    lhstParam.Clear();
                    int aintParticipantId = 0;

                    if (aobjParentObject is busRetirementApplication)
                    {
                        busRetirementApplication lobjRetirementApp = (busRetirementApplication)aobjParentObject;
                        aintParticipantId = lobjRetirementApp.ibusPerson.icdoPerson.person_id;
                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.benefit_subtype_value = lobjRetirementApp.ibusPerson.iclbPersonAccount.Where(item => item.icdoPersonAccount.plan_id == lbusBenefitApplicationDetail.iintPlan_ID).First().icdoPersonAccount.istrRetirementSubType;
                    }
                    else if (aobjParentObject is busWithdrawalApplication)
                    {
                        busWithdrawalApplication lobjWithdrawalApp = (busWithdrawalApplication)aobjParentObject;
                        aintParticipantId = lobjWithdrawalApp.ibusPerson.icdoPerson.person_id;
                    }
                    else if (aobjParentObject is busDisabilityApplication)
                    {
                        busDisabilityApplication lobjDisabilityApplication = (busDisabilityApplication)aobjParentObject;
                        aintParticipantId = lobjDisabilityApplication.ibusPerson.icdoPerson.person_id;
                    }
                    else if (aobjParentObject is busDeathPreRetirement)
                    {
                        busDeathPreRetirement lobjDeathPreRetirement = (busDeathPreRetirement)aobjParentObject;
                        aintParticipantId = lobjDeathPreRetirement.ibusPerson.icdoPerson.person_id;
                    }

                    int strBeneficiaryId = 0;

                    if (aobjParentObject is busDeathPreRetirement)
                    {
                        // if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.survivor_id != 0)
                        if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSurvivorTypeValue == busConstant.SURVIVOR_TYPE_PERSON &&
                             lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.survivor_id != 0)
                        {
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.organization_id = 0;
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrOrganizationName = string.Empty;
                            strBeneficiaryId = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.survivor_id;
                            lhstParam.Add(enmPerson.person_id.ToString().ToUpper(), strBeneficiaryId);

                            string istrBusinessTierUrl1 = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
                            IBusinessTier isrvBusinessTier1 = WCFClient<IBusinessTier>.CreateChannel(istrBusinessTierUrl);

                            DataTable ldtbBenefitaryFullName = isrvBusinessTier1.ExecuteQuery("cdoBenefitApplication.GetPersonsFullname", lhstParam, iobjPassInfo.idictParams);

                            if (ldtbBenefitaryFullName.Rows.Count > 0)
                            {
                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSurvivorFullName = ldtbBenefitaryFullName.Rows[0][0].ToString();
                            }
                            else
                            {
                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSurvivorFullName = string.Empty;
                            }

                            lhstParam.Clear();
                            lhstParam.Add(enmRelationship.person_id.ToString().ToUpper(), aintParticipantId);
                            lhstParam.Add("SUVIVOR_ID", strBeneficiaryId);
                            lhstParam.Add("PLAN_CODE", lbusBenefitApplicationDetail.istrPlanCode);

                            string istrBusinessTierUrl2 = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
                            IBusinessTier isrvBusinessTier2 = WCFClient<IBusinessTier>.CreateChannel(istrBusinessTierUrl);

                            DataTable ldtbBenefitary = isrvBusinessTier2.ExecuteQuery("cdoBenefitApplication.GetSurvivorDetailsFromSurvivorId", lhstParam, iobjPassInfo.idictParams);

                            if (ldtbBenefitary.Rows.Count > 0)
                            {
                                DataRow ldtrRow = ldtbBenefitary.Rows[0];
                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrRelationShip = Convert.ToString(ldtrRow["RELATIONSHIP"]);
                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.idecPercentage = Convert.ToDecimal(ldtrRow["PERCENTAGE"]);
                            }

                            else
                            {
                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrRelationShip = string.Empty;
                            }

                        }
                        //   else if(lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.organization_id != 0)
                        else if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSurvivorTypeValue == busConstant.SURVIVOR_TYPE_ORG &&
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.organization_id != 0)
                        {
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.survivor_id = 0;
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrSurvivorFullName = string.Empty;
                            strBeneficiaryId = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.organization_id;
                            lhstParam.Add(enmOrganization.org_id.ToString().ToUpper(), strBeneficiaryId);

                            string istrBusinessTierUrl3 = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
                            IBusinessTier isrvBusinessTier3 = WCFClient<IBusinessTier>.CreateChannel(istrBusinessTierUrl);

                            DataTable ldtbBenefitaryFullName = isrvBusinessTier3.ExecuteQuery("cdoBenefitApplication.GetOrgFullName", lhstParam, iobjPassInfo.idictParams);
                            if (ldtbBenefitaryFullName.Rows.Count > 0)
                            {
                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrOrganizationName = ldtbBenefitaryFullName.Rows[0][0].ToString();
                            }
                            else
                            {
                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrOrganizationName = string.Empty;
                            }

                            lhstParam.Clear();
                            lhstParam.Add(enmRelationship.person_id.ToString().ToUpper(), aintParticipantId);
                            lhstParam.Add(enmRelationship.beneficiary_org_id.ToString().ToUpper(), strBeneficiaryId);
                            DataTable ldtbBenefitary = isrvBusinessTier.ExecuteQuery("cdoBenefitApplication.GetOrgDetailsFromOrgId", lhstParam, iobjPassInfo.idictParams);
                            if (ldtbBenefitary.Rows.Count > 0)
                            {
                                DataRow ldtrRow = ldtbBenefitary.Rows[0];
                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrRelationShip = Convert.ToString(ldtrRow["RELATIONSHIP"]);
                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.idecPercentage = Convert.ToDecimal(ldtrRow["PERCENTAGE"]);
                            }
                            else
                            {
                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrRelationShip = string.Empty;
                            }

                        }
                    }
                    else
                    {
                        if (lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.iintJointAnnuaintID != 0)
                        {
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.joint_annuitant_id = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.iintJointAnnuaintID;
                        }

                        strBeneficiaryId = lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.iintJointAnnuaintID;
                        lhstParam.Add(enmRelationship.person_id.ToString(), aintParticipantId);
                        lhstParam.Add(enmRelationship.beneficiary_person_id.ToString(), strBeneficiaryId);

                        string istrBusinessTierUrl4 = String.Format(MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl, "srvPerson");
                        IBusinessTier isrvBusinessTier4 = WCFClient<IBusinessTier>.CreateChannel(istrBusinessTierUrl);

                        object larrTemp = isrvBusinessTier4.ExecuteMethod("LoadParticipantDetails", lhstParam, true, iobjPassInfo.idictParams);

                        if (larrTemp is ArrayList && (larrTemp as ArrayList).Count > 0)
                        {
                            ArrayList larr = larrTemp as ArrayList;
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrFullName = larr[0].ToString();
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.idtDOB = Convert.ToDateTime(larr[3]);
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrRelationShip = larr[1].ToString();
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrGender = larr[2].ToString();

                        }
                        else
                        {
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrFullName = string.Empty;
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrDOB = string.Empty;
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrRelationShip = string.Empty;
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrGender = string.Empty;
                        }
                    }
                }
            }
            return base.ValidateGridUpdateChild(astrFormName, aobjParentObject, aobjChildObject, ahstParams);
        }
        public bool IsNonNegative(string astrNumber)
        {
            bool lblnValidPercentage = false;
            Regex lrexGex = new Regex("^[0-9,.]*$");
            if (!lrexGex.IsMatch(astrNumber))
            {
                lblnValidPercentage = true;
            }
            return lblnValidPercentage;
        }
        public bool IsNegative(string astrNumber)
        {
            bool lblnValidPercentage = false;
            Regex lrexGex = new Regex("^[0-9,.]*$");
            if (!lrexGex.IsMatch(astrNumber))
            {
                lblnValidPercentage = true;
            }
            return lblnValidPercentage;
        }
        public Collection<cdoOrganization> GetAllOrgBeneficaryOfParticipantabc(int PersonId)
        {
            busBenefitApplicationDetail objbusBenefitApplicationDetail = new busBenefitApplicationDetail();
            return objbusBenefitApplicationDetail.GetAllOrgBeneficaryOfParticipant(PersonId);
        }

        //Methods From QDRO Application
        #region QDRO Methods

        //FM upgrade: 6.0.0.31 changes - public to protected
        protected override ArrayList ValidateNew(string astrFormName, Hashtable ahstParam)
        {

            ArrayList larrErrors = null;
            iobjPassInfo.iconFramework.Open();
            try
            {
                if (astrFormName == busConstant.QRDO_LOOKUP)
                {
                    busQDROApplicationLookup lbusQrdoApplicationLookup = new busQDROApplicationLookup();
                    larrErrors = lbusQrdoApplicationLookup.ValidateNew(ahstParam);
                }
                // TODO: This code needs to be removed later. As of now we do not have the information that what needs to be done if person is not enrolled in any plan
                if (astrFormName == busConstant.PERSON_MAINTENANCE && (ahstParam.ContainsKey("aint_participant_id") || ahstParam.ContainsKey("abln_address_flag")))//PIR 525
                {
                    busPerson lbusPerson = new busPerson();
                    larrErrors = lbusPerson.ValidateNew(ahstParam);
                }
                if (astrFormName == busConstant.BENEFIT_LOOKUP || astrFormName == busConstant.Retirement_Application_Lookup_Form)
                {
                    busBenefitApplication lbusBenefitApplication = new busBenefitApplication();
                    larrErrors = lbusBenefitApplication.ValidateNew(ahstParam);
                }
                if (astrFormName == busConstant.DEATH_NOTIFICATION_LOOKUP)
                {
                    busDeathNotification lbusDeathNotification = new busDeathNotification();
                    larrErrors = lbusDeathNotification.ValidateNew(ahstParam);
                }
                if (astrFormName == busConstant.YEAR_END_PROCESS_REQUEST_LOOKUP)
                {
                    busYearEndProcessRequest lbusYearEndProcessRequest = new busYearEndProcessRequest();
                    larrErrors = lbusYearEndProcessRequest.ValidateNew(ahstParam);
                }

                //ID-68932
                if (astrFormName == busConstant.BUS_PENSION_VERIFICATION_HISTORY)
                {
                    busPersonOverview lobjPersonOverview = new busPersonOverview();
                    lobjPersonOverview.CheckDuplicatePlan(ahstParam, ref larrErrors);
                }

            }
            finally
            {
                iobjPassInfo.iconFramework.Close();
            }
            return larrErrors;
        }



        public busQdroApplication NewQdroApplication(string astrPersonMPId, string astrPayeeMPId)
        {
            string lstrMpiPersonId = astrPersonMPId.Trim();
            string lstrPayeeMPId = astrPayeeMPId.Trim();
            busQdroApplication lobjQdroApplication = new busQdroApplication { icdoDroApplication = new cdoDroApplication() };
            lobjQdroApplication.ibusParticipant = new busPerson { icdoPerson = new cdoPerson() };
            lobjQdroApplication.ibusAlternatePayee = new busPerson { icdoPerson = new cdoPerson() };


            DataTable ldtbPersonIDs = busBase.Select("cdoDroApplication.GetPersonID", new object[2] { lstrMpiPersonId, lstrPayeeMPId });


            DataRow[] iclbldtrRow = ldtbPersonIDs.Select(string.Format("{0} = '{1}'", enmPerson.mpi_person_id, lstrMpiPersonId));
            lobjQdroApplication.ibusParticipant.icdoPerson.LoadData(iclbldtrRow[0]);
            lobjQdroApplication.ibusParticipant.LoadPersonAccounts();
            iclbldtrRow.Clear();

            iclbldtrRow = ldtbPersonIDs.Select(string.Format("{0} = '{1}'", enmPerson.mpi_person_id, lstrPayeeMPId));
            lobjQdroApplication.ibusAlternatePayee.icdoPerson.LoadData(iclbldtrRow[0]);

            lobjQdroApplication.icdoDroApplication.person_id = lobjQdroApplication.ibusParticipant.icdoPerson.person_id;
            lobjQdroApplication.icdoDroApplication.alternate_payee_id = lobjQdroApplication.ibusAlternatePayee.icdoPerson.person_id;
            lobjQdroApplication.icdoDroApplication.life_conversion_factor_flag = busConstant.FLAG_YES;
            
            lobjQdroApplication.iclbDroBenefitDetails = new Collection<busDroBenefitDetails>();
            lobjQdroApplication.iclbAttorney = new Collection<busAttorney>();
            lobjQdroApplication.iclbDroStatusHistory = new Collection<busDroApplicationStatusHistory>();
            lobjQdroApplication.LoadAllBenefitDetails();
            lobjQdroApplication.EvaluateInitialLoadRules(utlPageMode.New);
            lobjQdroApplication.ibusParticipant.iclbNotes = new Collection<busNotes>();
            lobjQdroApplication.ibusParticipant.iclbNotes = busGlobalFunctions.LoadNotes(lobjQdroApplication.icdoDroApplication.person_id, 0, busConstant.DEATH_NOTIFICATION_MAINTANENCE_FORM);
            return lobjQdroApplication;
        }

        public busQdroApplication FindQdroApplication(int aintdroapplicationid)
        {
            busQdroApplication lobjQdroApplication = new busQdroApplication();
            if (lobjQdroApplication.FindQdroApplication(aintdroapplicationid))
            {
                lobjQdroApplication.ibusParticipant = new busPerson();
                lobjQdroApplication.ibusParticipant.FindPerson(lobjQdroApplication.icdoDroApplication.person_id);
                lobjQdroApplication.ibusParticipant.LoadInitialData();
                lobjQdroApplication.ibusParticipant.LoadPersonAddresss();
                lobjQdroApplication.ibusParticipant.LoadPersonContacts();
                lobjQdroApplication.ibusParticipant.LoadCorrAddress();
                lobjQdroApplication.ibusParticipant.LoadPersonAccounts();

                lobjQdroApplication.ibusAlternatePayee = new busPerson();
                lobjQdroApplication.ibusAlternatePayee.FindPerson(lobjQdroApplication.icdoDroApplication.alternate_payee_id);

                lobjQdroApplication.LoadBenefitDetails();
                lobjQdroApplication.LoadAllBenefitDetails();
                lobjQdroApplication.LoadAttorney();
                lobjQdroApplication.LoadDroStatusHistory();



                foreach (busDroBenefitDetails ibusDroBenefitDetails in lobjQdroApplication.iclbDroBenefitDetails)
                {
                    ibusDroBenefitDetails.EvaluateInitialLoadRules();
                }

                foreach (busAttorney ibusAttorney in lobjQdroApplication.iclbAttorney)
                {
                    ibusAttorney.EvaluateInitialLoadRules();
                }

                lobjQdroApplication.ibusParticipant.iclbNotes = busGlobalFunctions.LoadNotes(lobjQdroApplication.icdoDroApplication.person_id, 0, busConstant.QRDO_MAINTAINENCE_FORM);
            }

            return lobjQdroApplication;
        }


        public busQDROApplicationLookup LoadQDROApplications(DataTable adtbSearchResult)
        {
            busQDROApplicationLookup lobjQDROApplicationLookup = new busQDROApplicationLookup();
            lobjQDROApplicationLookup.LoadQdroApplications(adtbSearchResult);
            return lobjQDROApplicationLookup;
        }


        public busAttorney FindAttorney(int aintattorneyid)
        {
            busAttorney lobjAttorney = new busAttorney();
            if (lobjAttorney.FindAttorney(aintattorneyid))
            {
            }

            return lobjAttorney;
        }

        public busDroBenefitDetails FindDroBenefitDetails(int aintdrobenefitid)
        {
            busDroBenefitDetails lobjDroBenefitDetails = new busDroBenefitDetails();
            if (lobjDroBenefitDetails.FindDroBenefitDetails(aintdrobenefitid))
            {
            }

            return lobjDroBenefitDetails;
        }



        #endregion

        /*
		public busPersonAddress FindPersonAddress(int aintaddressid)
		{
			busPersonAddress lobjPersonAddress = new busPersonAddress();
			if (lobjPersonAddress.FindPersonAddress(aintaddressid))
			{
                lobjPersonAddress.ibusPerson = new busPerson();
                lobjPersonAddress.ibusPerson.FindPerson(lobjPersonAddress.icdoPersonAddress.person_id);
                lobjPersonAddress.LoadPersonAddressChklists();
                lobjPersonAddress.LoadPersonAddressHistorys();
                lobjPersonAddress.LoadInitialData();
			}

			return lobjPersonAddress;
		}*/

        public busPersonAddress FindPersonAddress(int aintaddressid, int aintMainParticipantAddressId, int aintPersonid)
        {
            busPersonAddress lobjPersonAddress = new busPersonAddress();
            
            lobjPersonAddress.iblnAddressSourceReadOnly = true;
            if (aintMainParticipantAddressId == 0 && lobjPersonAddress.FindPersonAddress(aintaddressid))
            {
                lobjPersonAddress.ibusPerson = new busPerson();
                lobjPersonAddress.ibusPerson.FindPerson(lobjPersonAddress.icdoPersonAddress.person_id);
                lobjPersonAddress.ibusPerson.LoadInitialData();
                lobjPersonAddress.ibusPerson.LoadPersonAddresss();
                lobjPersonAddress.LoadPersonAddressChklists();
                lobjPersonAddress.ibusRelationship = new busRelationship { icdoRelationship = new cdoRelationship() };
                lobjPersonAddress.LoadMainParticipantsAddress();
                lobjPersonAddress.LoadInitialData();
            }
            else
            {
                lobjPersonAddress.ibusPerson = new busPerson();
                lobjPersonAddress.ibusPerson.FindPerson(aintPersonid);
                lobjPersonAddress.ibusPerson.LoadInitialData();
                lobjPersonAddress.ibusPerson.LoadPersonAddresss();
                lobjPersonAddress.icdoPersonAddress = new cdoPersonAddress();
                lobjPersonAddress.LoadPersonAddressChklists();
                lobjPersonAddress.LoadMainParticipantAddress(aintMainParticipantAddressId);
                lobjPersonAddress.LoadBeneficiaryDependentData();
                lobjPersonAddress.icdoPersonAddress.istrAddSameAsParticipantFlag = busConstant.FLAG_YES;
                lobjPersonAddress.LoadInitialData();
            }
        
            return lobjPersonAddress;
        }

        public busPersonAddress NewPersonAddress(int aintPersonId, string astrAddressFlag)
        {
            busPersonAddress lobjPersonAddress = new busPersonAddress();
            lobjPersonAddress.icdoPersonAddress = new cdoPersonAddress();
            lobjPersonAddress.icdoPersonAddress.person_id = aintPersonId;
            lobjPersonAddress.icdoPersonAddress.start_date = DateTime.Today.Date;//PIR 525
            lobjPersonAddress.ibusPerson = new busPerson();
            lobjPersonAddress.ibusPerson.FindPerson(aintPersonId);
            lobjPersonAddress.ibusPerson.LoadInitialData();
            lobjPersonAddress.ibusPerson.LoadPersonAddresss();
            lobjPersonAddress.iclcPersonAddressChklist = new utlCollection<cdoPersonAddressChklist>();
            lobjPersonAddress.ibusRelationship = new busRelationship { icdoRelationship = new cdoRelationship() };
            lobjPersonAddress.LoadMainParticipantsAddress();
            lobjPersonAddress.EvaluateInitialLoadRules(utlPageMode.New);

            return lobjPersonAddress;
        }
        public busRetirementWizard NewRetirementWizard(string MpiPersonId)
        {
            string lstrMpiPersonId = MpiPersonId.Trim();
            busRetirementWizard lbusRetirementWizard = new busRetirementWizard { icdoBenefitApplication = new cdoBenefitApplication() };
            //if (Ben_Application_ID == 0)
            //{
            lbusRetirementWizard.ibusPerson = new busPerson() { icdoPerson = new cdoPerson() };
                DataTable ldtbPersonID = busBase.Select("cdoPerson.GetPersonDetails", new object[1] { lstrMpiPersonId });

                if (ldtbPersonID.Rows.Count > 0)
                {
                lbusRetirementWizard.ibusPerson.icdoPerson.LoadData(ldtbPersonID.Rows[0]);
                lbusRetirementWizard.icdoBenefitApplication.person_id = lbusRetirementWizard.ibusPerson.icdoPerson.person_id;
                }


            lbusRetirementWizard.icdoBenefitApplication.benefit_type_value = busConstant.BENEFIT_TYPE_RETIREMENT;
            lbusRetirementWizard.ibusPerson.LoadPersonAccounts(); //Load Person Accounts //Code- Abhishek            
            //lbusRetirementApplication.LoadandProcessWorkHistory_ForAllPlans(); //Code-Abhishek
            lbusRetirementWizard.LoadInitialData();
            lbusRetirementWizard.iclbBenefitApplicationDetail = new Collection<busBenefitApplicationDetail>();

            //lbusRetirementWizard.CheckEligibility_Retirement();
           // lbusRetirementWizard.LoadBenefitApplicationEligiblePlans();

            lbusRetirementWizard.EvaluateInitialLoadRules(utlPageMode.New);
            
            lbusRetirementWizard.strPlanDescription = "";
            lbusRetirementWizard.intPlan_Id = 2;
            lbusRetirementWizard.strIAPPlanDescription = "";
            lbusRetirementWizard.intIAPPlan_Id = 1;
           


            return lbusRetirementWizard;
        }

        public busRetirementWizard FindRetirementWizard(int aintbenefitapplicationid)
        {
            busRetirementWizard lbusRetirementWizard = new busRetirementWizard();
            if (lbusRetirementWizard.FindBenefitApplication(aintbenefitapplicationid))
            {
                lbusRetirementWizard.ibusPerson = new busPerson();
                lbusRetirementWizard.ibusPerson.FindPerson(lbusRetirementWizard.icdoBenefitApplication.person_id);

                if (lbusRetirementWizard.ibusPerson.FindPerson(lbusRetirementWizard.icdoBenefitApplication.person_id))
                {
                    lbusRetirementWizard.ibusPerson.LoadPacketCorrespondences();
                }

                lbusRetirementWizard.GetAgeAtRetirement(lbusRetirementWizard.icdoBenefitApplication.retirement_date);
                lbusRetirementWizard.ibusPerson.LoadPersonAccounts(); //Load Person Accounts //Code-Abhishek                
                lbusRetirementWizard.LoadandProcessWorkHistory_ForAllPlans(); //Code-Added Abhishek                
                lbusRetirementWizard.LoadInitialData();
                lbusRetirementWizard.LoadBenefitApplicationStatusHistorys();
                lbusRetirementWizard.LoadBenefitApplicationDetails();
               // lbusRetirementWizard.CheckIfManagerLogin(); //for pir-522
                //PROD PIR 799
                if (iobjPassInfo.istrFormName == busConstant.Retirement_Application_Maintenance_Form_2)
                {
                    lbusRetirementWizard.CheckEligibility_Retirement();
                    lbusRetirementWizard.LoadBenefitApplicationEligiblePlans();
                    //if (lbusRetirementApplication.Eligible_Plans != null && lbusRetirementApplication.Eligible_Plans.Count > 0)
                    //    lbusRetirementApplication.icdoBenefitApplication.istrMessageforPlan = "Participant is eligible for Plan " + string.Join(", ", lbusRetirementApplication.Eligible_Plans);
                    //else
                    //    lbusRetirementApplication.icdoBenefitApplication.istrMessageforPlan = string.Empty;
                }
            }

            lbusRetirementWizard.LoadPersonNotes(busConstant.RETIREMENT_APPLICATION_MAINTAINENCE_FORM);
            //lbusRetirementApplication.ibusPerson.iclbNotes = busGlobalFunctions.LoadNotes(lbusRetirementApplication.icdoBenefitApplication.person_id, 0, busConstant.RETIREMENT_APPLICATION_MAINTAINENCE_FORM);
            return lbusRetirementWizard;
        }

        public busRetirementApplication NewRetirementApplication(int Ben_Application_ID, string MPI_PERSON_ID, string benefit_type_value, int Estimate_Calculation_ID = 0)
        {
            string lstrMpiPersonId = MPI_PERSON_ID.Trim();
            busRetirementApplication lbusRetirementApplication = new busRetirementApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            if (Ben_Application_ID == 0)
            {
                lbusRetirementApplication.ibusPerson = new busPerson() { icdoPerson = new cdoPerson() };
                DataTable ldtbPersonID = busBase.Select("cdoPerson.GetPersonDetails", new object[1] { lstrMpiPersonId });

                if (ldtbPersonID.Rows.Count > 0)
                {
                    lbusRetirementApplication.ibusPerson.icdoPerson.LoadData(ldtbPersonID.Rows[0]);
                    lbusRetirementApplication.icdoBenefitApplication.person_id = lbusRetirementApplication.ibusPerson.icdoPerson.person_id;
                }

                lbusRetirementApplication.icdoBenefitApplication.benefit_type_value = benefit_type_value;
                lbusRetirementApplication.ibusPerson.LoadPersonAccounts(); //Load Person Accounts //Code- Abhishek            
                //lbusRetirementApplication.LoadandProcessWorkHistory_ForAllPlans(); //Code-Abhishek
                lbusRetirementApplication.LoadInitialData();
                lbusRetirementApplication.iclbBenefitApplicationDetail = new Collection<busBenefitApplicationDetail>();
                lbusRetirementApplication.EvaluateInitialLoadRules(utlPageMode.New);

                lbusRetirementApplication.ibusPerson.iclbNotes = new Collection<busNotes>();
                lbusRetirementApplication.ibusPerson.iclbNotes = busGlobalFunctions.LoadNotes(lbusRetirementApplication.icdoBenefitApplication.person_id, 0, busConstant.RETIREMENT_APPLICATION_MAINTAINENCE_FORM);
                lbusRetirementApplication.CheckIfManagerLogin(); //FOR PIR-522

                //PIR-799 For (coming from workflow -> activity-Enter/Update Retirement Application -> checkout ->) if approved estimate calculation exists -> take calculation retirement date
                if (Estimate_Calculation_ID != 0)
                {
                    busBenefitCalculationHeader lbusBenefitCalculationHeader = new busBenefitCalculationHeader { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
                    if (lbusBenefitCalculationHeader.FindBenefitCalculationHeader(Estimate_Calculation_ID))
                    {
                        lbusRetirementApplication.icdoBenefitApplication.retirement_date = lbusBenefitCalculationHeader.icdoBenefitCalculationHeader.retirement_date;
                    }
                }
                lbusRetirementApplication.LoadPersonSpousDetails();
                lbusRetirementApplication.ibusBenefitApplicationChecklist = new busBenefitApplicationChecklist();
                lbusRetirementApplication.LoadListOfDivorces();
            }
            else
            {
                if (lbusRetirementApplication.FindBenefitApplication(Ben_Application_ID))
                {
                    lbusRetirementApplication.CreateLateRetirementApplication();
                    lbusRetirementApplication.EvaluateInitialLoadRules();
                    lbusRetirementApplication.ibusPerson.iclbNotes = new Collection<busNotes>();
                    lbusRetirementApplication.ibusPerson.iclbNotes = busGlobalFunctions.LoadNotes(lbusRetirementApplication.icdoBenefitApplication.person_id, 0, busConstant.RETIREMENT_APPLICATION_MAINTAINENCE_FORM);

                }
            }

            return lbusRetirementApplication;
        }

        public busRetirementApplication FindRetirementApplication(int aintbenefitapplicationid)
        {
            busRetirementApplication lbusRetirementApplication = new busRetirementApplication();
            if (lbusRetirementApplication.FindBenefitApplication(aintbenefitapplicationid))
            {
                lbusRetirementApplication.ibusPerson = new busPerson();
                lbusRetirementApplication.ibusPerson.FindPerson(lbusRetirementApplication.icdoBenefitApplication.person_id);

                if (lbusRetirementApplication.ibusPerson.FindPerson(lbusRetirementApplication.icdoBenefitApplication.person_id))
                {
                    lbusRetirementApplication.ibusPerson.LoadPacketCorrespondences();
                }

                lbusRetirementApplication.GetAgeAtRetirement(lbusRetirementApplication.icdoBenefitApplication.retirement_date);
                lbusRetirementApplication.ibusPerson.LoadPersonAccounts(); //Load Person Accounts //Code-Abhishek                
                lbusRetirementApplication.LoadandProcessWorkHistory_ForAllPlans(); //Code-Added Abhishek                
                lbusRetirementApplication.LoadInitialData();
                lbusRetirementApplication.LoadBenefitApplicationStatusHistorys();
                lbusRetirementApplication.LoadBenefitApplicationDetails();
                lbusRetirementApplication.CheckIfManagerLogin(); //for pir-522
                //PROD PIR 799
                if (iobjPassInfo.istrFormName == busConstant.Retirement_Application_Maintenance_Form_2)
                {
                    lbusRetirementApplication.CheckEligibility_Retirement();
                    lbusRetirementApplication.LoadBenefitApplicationEligiblePlans();
                    //if (lbusRetirementApplication.Eligible_Plans != null && lbusRetirementApplication.Eligible_Plans.Count > 0)
                    //    lbusRetirementApplication.icdoBenefitApplication.istrMessageforPlan = "Participant is eligible for Plan " + string.Join(", ", lbusRetirementApplication.Eligible_Plans);
                    //else
                    //    lbusRetirementApplication.icdoBenefitApplication.istrMessageforPlan = string.Empty;
                }
                lbusRetirementApplication.LoadPersonSpousDetails();

                lbusRetirementApplication.ibusBenefitApplicationChecklist = new busBenefitApplicationChecklist();
                DataTable ldtblist = busBase.Select("entPersonAccount.LoadCheckList", new object[1] { aintbenefitapplicationid });
                if (ldtblist.Rows.Count > 0) 
                {
                    lbusRetirementApplication.ibusBenefitApplicationChecklist.FindBenefitApplicationChecklist(Convert.ToInt32(ldtblist.Rows[0]["CHECKLIST_BENEFIT_APPLICATION_ID"]));
                }
                else
                {
                    if (lbusRetirementApplication.ibusBenefitApplicationChecklist.icdoBenefitApplicationChecklist.IsNull())
                    {
                        lbusRetirementApplication.ibusBenefitApplicationChecklist.icdoBenefitApplicationChecklist = new doBenefitApplicationChecklist();
                        lbusRetirementApplication.ibusBenefitApplicationChecklist.icdoBenefitApplicationChecklist.benefit_application_id = aintbenefitapplicationid;
                        lbusRetirementApplication.ibusBenefitApplicationChecklist.icdoBenefitApplicationChecklist.ienuObjectState = ObjectState.Insert;
                        lbusRetirementApplication.ibusBenefitApplicationChecklist.icdoBenefitApplicationChecklist.Insert();

                        DataTable ldtblist2 = busBase.Select("entPersonAccount.LoadCheckList", new object[1] { aintbenefitapplicationid });
                        lbusRetirementApplication.ibusBenefitApplicationChecklist.FindBenefitApplicationChecklist(Convert.ToInt32(ldtblist2.Rows[0]["CHECKLIST_BENEFIT_APPLICATION_ID"]));
                    }
                }
                lbusRetirementApplication.LoadBenefitAuditingCheckList(aintbenefitapplicationid);
                if (lbusRetirementApplication.iclbBenefitApplicationAuditingChecklist.IsNullOrEmpty())
                {
                    lbusRetirementApplication.AddBenefitAuditingCheckList(aintbenefitapplicationid);
                }
                
                lbusRetirementApplication.LoadListOfDivorces();
            }

            lbusRetirementApplication.LoadPersonNotes(busConstant.RETIREMENT_APPLICATION_MAINTAINENCE_FORM);
            //lbusRetirementApplication.ibusPerson.iclbNotes = busGlobalFunctions.LoadNotes(lbusRetirementApplication.icdoBenefitApplication.person_id, 0, busConstant.RETIREMENT_APPLICATION_MAINTAINENCE_FORM);
            return lbusRetirementApplication;
        }

        public busWithdrawalApplication NewWithdrawalApplication(string MPI_PERSON_ID, string benefit_type_value, int dro_application_id, string ALTERNATE_PAYEE_MPID)
        {
            string lstrMpiPersonId = MPI_PERSON_ID.Trim();
            busWithdrawalApplication lbusWithdrawalApplication = new busWithdrawalApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            lbusWithdrawalApplication.iclbBenefitApplicationDetail = new Collection<busBenefitApplicationDetail>();
            lbusWithdrawalApplication.iblnWithdrawalForAlternatePayee = false;
            lbusWithdrawalApplication.ibusPerson = new busPerson() { icdoPerson = new cdoPerson() };

            #region Alternate Payee Withdrawal Application
            if (dro_application_id != 0)
            {

                lbusWithdrawalApplication.iblnWithdrawalForAlternatePayee = true;
                lbusWithdrawalApplication.icdoBenefitApplication.dro_application_id = dro_application_id;
                lbusWithdrawalApplication.ibusQdroApplication = new busQdroApplication();
                if (lbusWithdrawalApplication.ibusQdroApplication.FindQdroApplication(dro_application_id))
                {
                    lbusWithdrawalApplication.ibusPerson.FindPerson(lbusWithdrawalApplication.ibusQdroApplication.icdoDroApplication.alternate_payee_id);
                    lbusWithdrawalApplication.icdoBenefitApplication.person_id = lbusWithdrawalApplication.ibusPerson.icdoPerson.person_id;

                    lbusWithdrawalApplication.ibusQdroApplication.ibusParticipant = new busPerson();
                    lbusWithdrawalApplication.ibusQdroApplication.ibusParticipant.FindPerson(lbusWithdrawalApplication.ibusQdroApplication.icdoDroApplication.person_id);
                    lbusWithdrawalApplication.ibusQdroApplication.ibusParticipant.iclbPersonAccount = new Collection<busPersonAccount>();
                    lbusWithdrawalApplication.ibusQdroApplication.ibusParticipant.LoadPersonAccounts();

                    lbusWithdrawalApplication.ibusQdroApplication.ibusAlternatePayee = new busPerson();
                    lbusWithdrawalApplication.ibusQdroApplication.ibusAlternatePayee.FindPerson(lbusWithdrawalApplication.ibusQdroApplication.icdoDroApplication.alternate_payee_id);
                    lbusWithdrawalApplication.ibusQdroApplication.LoadBenefitDetails();

                    lbusWithdrawalApplication.icdoBenefitApplication.alternate_payee_id = lbusWithdrawalApplication.ibusQdroApplication.icdoDroApplication.alternate_payee_id;
                    lbusWithdrawalApplication.icdoBenefitApplication.withdrawal_date = lbusWithdrawalApplication.ibusQdroApplication.icdoDroApplication.dro_commencement_date; //PIR 1054

                    foreach (busDroBenefitDetails lbusDroBenefitDetails in lbusWithdrawalApplication.ibusQdroApplication.iclbDroBenefitDetails)
                    {
                        if (lbusDroBenefitDetails.icdoDroBenefitDetails.ee_flag == busConstant.FLAG_YES || lbusDroBenefitDetails.icdoDroBenefitDetails.uvhp_flag == busConstant.FLAG_YES
                            || lbusDroBenefitDetails.icdoDroBenefitDetails.l52_spl_acc_flag == busConstant.FLAG_YES || lbusDroBenefitDetails.icdoDroBenefitDetails.l161_spl_acc_flag == busConstant.FLAG_YES)
                        {
                            DataTable ltblPlanBenefit = new DataTable();

                            busBenefitApplicationDetail lbusBenefitApplicationDetail = new busBenefitApplicationDetail { icdoBenefitApplicationDetail = new cdoBenefitApplicationDetail() };
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.plan_benefit_id = lbusDroBenefitDetails.icdoDroBenefitDetails.plan_benefit_id;

                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.dro_model_id = busConstant.DRO_MODEL_ID;
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.dro_model_value = lbusDroBenefitDetails.icdoDroBenefitDetails.dro_model_value;
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.dro_model_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(busConstant.DRO_MODEL_ID,
                                   lbusDroBenefitDetails.icdoDroBenefitDetails.dro_model_value);

                            ltblPlanBenefit = lbusWithdrawalApplication.GetPlanAndBenefitOptionValue(lbusDroBenefitDetails.icdoDroBenefitDetails.plan_benefit_id);

                            if (ltblPlanBenefit.Rows.Count > 0)
                            {
                                lbusBenefitApplicationDetail.iintPlan_ID = Convert.ToInt32(ltblPlanBenefit.Rows[0][0]);

                                if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.MPIPP_PLAN_ID)
                                {
                                    lbusBenefitApplicationDetail.istrPlanCode = busConstant.MPIPP;
                                }
                                else if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.IAP_PLAN_ID)
                                {
                                    lbusBenefitApplicationDetail.istrPlanCode = busConstant.IAP;
                                }

                                DataTable ldtblist = busBase.Select("cdoPlan.GetPlanById", new object[1] { lbusBenefitApplicationDetail.iintPlan_ID });
                                if (ldtblist.Rows.Count > 0)
                                {
                                    lbusBenefitApplicationDetail.istrPlanName = ldtblist.Rows[0][busConstant.PLAN_NAME].ToString();
                                }

                                lbusBenefitApplicationDetail.istrPlanBenefitDescription =
                                    iobjPassInfo.isrvDBCache.GetCodeDescriptionString(busConstant.BENEFIT_OPTION_CODE_ID, Convert.ToString(ltblPlanBenefit.Rows[0][1]));
                                lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = Convert.ToString(ltblPlanBenefit.Rows[0][1]);
                            }

                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.uvhp_flag = lbusDroBenefitDetails.icdoDroBenefitDetails.uvhp_flag;
                            if (lbusDroBenefitDetails.icdoDroBenefitDetails.uvhp_flag == busConstant.FLAG_YES)
                            {
                                lbusBenefitApplicationDetail.istrSubPlan = busConstant.UVHP;
                                lbusBenefitApplicationDetail.istrSubPlanDescription = busConstant.UVHP;
                            }
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.ee_flag = lbusDroBenefitDetails.icdoDroBenefitDetails.ee_flag;
                            if (lbusDroBenefitDetails.icdoDroBenefitDetails.ee_flag == busConstant.FLAG_YES)
                            {
                                lbusBenefitApplicationDetail.istrSubPlan = busConstant.EE;
                                lbusBenefitApplicationDetail.istrSubPlanDescription = busConstant.EE;
                            }
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.l52_spl_acc_flag = lbusDroBenefitDetails.icdoDroBenefitDetails.l52_spl_acc_flag;
                            if (lbusDroBenefitDetails.icdoDroBenefitDetails.l52_spl_acc_flag == busConstant.FLAG_YES)
                            {
                                lbusBenefitApplicationDetail.istrSubPlan = busConstant.L52_SPL_ACC;
                                lbusBenefitApplicationDetail.istrSubPlanDescription = busConstant.LOCAL_52_SPECIAL_ACCOUNT;
                            }
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.l161_spl_acc_flag = lbusDroBenefitDetails.icdoDroBenefitDetails.l161_spl_acc_flag;
                            if (lbusDroBenefitDetails.icdoDroBenefitDetails.l161_spl_acc_flag == busConstant.FLAG_YES)
                            {
                                lbusBenefitApplicationDetail.istrSubPlan = busConstant.L161_SPL_ACC;
                                lbusBenefitApplicationDetail.istrSubPlanDescription = busConstant.LOCAL1_161_SPECIAL_ACCOUNT;
                            }
                            lbusWithdrawalApplication.iclbBenefitApplicationDetail.Add(lbusBenefitApplicationDetail);
                        }
                    }


                }
            }
            #endregion Alternate Payee Withdrawal Application
            else
            {
                DataTable ldtbPersonID = busBase.Select("cdoPerson.GetPersonDetails", new object[1] { lstrMpiPersonId });

                if (ldtbPersonID.Rows.Count > 0)
                {
                    lbusWithdrawalApplication.ibusPerson.icdoPerson.LoadData(ldtbPersonID.Rows[0]);
                    lbusWithdrawalApplication.icdoBenefitApplication.person_id = lbusWithdrawalApplication.ibusPerson.icdoPerson.person_id;
                }
            }

            lbusWithdrawalApplication.icdoBenefitApplication.benefit_type_value = benefit_type_value;
            lbusWithdrawalApplication.ibusPerson.LoadPersonAccounts();
            //lbusWithdrawalApplication.LoadandProcessWorkHistory_ForAllPlans();
            lbusWithdrawalApplication.LoadInitialData();
            lbusWithdrawalApplication.EvaluateInitialLoadRules(utlPageMode.New);
            lbusWithdrawalApplication.ibusPerson.iclbNotes = new Collection<busNotes>();
            lbusWithdrawalApplication.ibusPerson.iclbNotes = busGlobalFunctions.LoadNotes(lbusWithdrawalApplication.icdoBenefitApplication.person_id, 0, busConstant.RETIREMENT_APPLICATION_MAINTAINENCE_FORM);




            return lbusWithdrawalApplication;
        }

        public busWithdrawalApplication FindWithdrawalApplication(int aintbenefitapplicationid)
        {
            busWithdrawalApplication lbusWithdrawalApplication = new busWithdrawalApplication();
            if (lbusWithdrawalApplication.FindBenefitApplication(aintbenefitapplicationid))
            {
                lbusWithdrawalApplication.ibusPerson = new busPerson();
                lbusWithdrawalApplication.ibusPerson.FindPerson(lbusWithdrawalApplication.icdoBenefitApplication.person_id);
                lbusWithdrawalApplication.LoadInitialData();
                lbusWithdrawalApplication.GetAgeAtRetirement(lbusWithdrawalApplication.icdoBenefitApplication.withdrawal_date);
                lbusWithdrawalApplication.ibusPerson.LoadPersonAccounts();
                lbusWithdrawalApplication.LoadandProcessWorkHistory_ForAllPlans();
                lbusWithdrawalApplication.LoadBenefitApplicationStatusHistorys();
                lbusWithdrawalApplication.LoadBenefitApplicationDetails();

                lbusWithdrawalApplication.iblnWithdrawalForAlternatePayee = busConstant.BOOL_FALSE;
                if (lbusWithdrawalApplication.icdoBenefitApplication.dro_application_id != 0)
                {
                    if (lbusWithdrawalApplication.icdoBenefitApplication.application_status_value == busConstant.BENEFIT_APPLICATION_STATUS_APPROVED)
                    {
                        lbusWithdrawalApplication.ibusQdroApplication = new busQdroApplication();
                        if (lbusWithdrawalApplication.ibusQdroApplication.FindQdroApplication(lbusWithdrawalApplication.icdoBenefitApplication.dro_application_id))
                        {
                            lbusWithdrawalApplication.ibusQdroApplication.ibusParticipant = new busPerson();
                            lbusWithdrawalApplication.ibusQdroApplication.ibusParticipant.FindPerson(lbusWithdrawalApplication.ibusQdroApplication.icdoDroApplication.person_id);
                            lbusWithdrawalApplication.ibusQdroApplication.ibusParticipant.iclbPersonAccount = new Collection<busPersonAccount>();
                            lbusWithdrawalApplication.ibusQdroApplication.ibusParticipant.LoadPersonAccounts();

                            lbusWithdrawalApplication.ibusQdroApplication.ibusAlternatePayee = new busPerson();
                            lbusWithdrawalApplication.ibusQdroApplication.ibusAlternatePayee.FindPerson(lbusWithdrawalApplication.ibusQdroApplication.icdoDroApplication.alternate_payee_id);
                            lbusWithdrawalApplication.iblnWithdrawalForAlternatePayee = busConstant.BOOL_TRUE;

                            lbusWithdrawalApplication.ibusQdroApplication.LoadBenefitDetails();
                        }
                    }
                    else
                    {
                        foreach (busBenefitApplicationDetail lbusBenefitApplicationDetail in lbusWithdrawalApplication.iclbBenefitApplicationDetail)
                        {
                            lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.Delete();
                        }
                        lbusWithdrawalApplication.iclbBenefitApplicationDetail.Clear();

                        #region Alternate Payee Withdrawal Application

                        lbusWithdrawalApplication.iblnWithdrawalForAlternatePayee = true;
                        lbusWithdrawalApplication.icdoBenefitApplication.dro_application_id = lbusWithdrawalApplication.icdoBenefitApplication.dro_application_id;
                        lbusWithdrawalApplication.ibusQdroApplication = new busQdroApplication();
                        if (lbusWithdrawalApplication.ibusQdroApplication.FindQdroApplication(lbusWithdrawalApplication.icdoBenefitApplication.dro_application_id))
                        {
                            lbusWithdrawalApplication.ibusPerson.FindPerson(lbusWithdrawalApplication.ibusQdroApplication.icdoDroApplication.alternate_payee_id);
                            lbusWithdrawalApplication.icdoBenefitApplication.person_id = lbusWithdrawalApplication.ibusPerson.icdoPerson.person_id;

                            lbusWithdrawalApplication.ibusQdroApplication.ibusParticipant = new busPerson();
                            lbusWithdrawalApplication.ibusQdroApplication.ibusParticipant.FindPerson(lbusWithdrawalApplication.ibusQdroApplication.icdoDroApplication.person_id);
                            lbusWithdrawalApplication.ibusQdroApplication.ibusParticipant.iclbPersonAccount = new Collection<busPersonAccount>();
                            lbusWithdrawalApplication.ibusQdroApplication.ibusParticipant.LoadPersonAccounts();

                            lbusWithdrawalApplication.ibusQdroApplication.ibusAlternatePayee = new busPerson();
                            lbusWithdrawalApplication.ibusQdroApplication.ibusAlternatePayee.FindPerson(lbusWithdrawalApplication.ibusQdroApplication.icdoDroApplication.alternate_payee_id);
                            lbusWithdrawalApplication.ibusQdroApplication.LoadBenefitDetails();

                            lbusWithdrawalApplication.icdoBenefitApplication.alternate_payee_id = lbusWithdrawalApplication.ibusQdroApplication.icdoDroApplication.alternate_payee_id;

                            foreach (busDroBenefitDetails lbusDroBenefitDetails in lbusWithdrawalApplication.ibusQdroApplication.iclbDroBenefitDetails)
                            {
                                if (lbusDroBenefitDetails.icdoDroBenefitDetails.ee_flag == busConstant.FLAG_YES || lbusDroBenefitDetails.icdoDroBenefitDetails.uvhp_flag == busConstant.FLAG_YES
                                    || lbusDroBenefitDetails.icdoDroBenefitDetails.l52_spl_acc_flag == busConstant.FLAG_YES || lbusDroBenefitDetails.icdoDroBenefitDetails.l161_spl_acc_flag == busConstant.FLAG_YES)
                                {
                                    DataTable ltblPlanBenefit = new DataTable();

                                    busBenefitApplicationDetail lbusBenefitApplicationDetail = new busBenefitApplicationDetail { icdoBenefitApplicationDetail = new cdoBenefitApplicationDetail() };
                                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.plan_benefit_id = lbusDroBenefitDetails.icdoDroBenefitDetails.plan_benefit_id;

                                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.dro_model_id = busConstant.DRO_MODEL_ID;
                                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.dro_model_value = lbusDroBenefitDetails.icdoDroBenefitDetails.dro_model_value;
                                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.dro_model_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(busConstant.DRO_MODEL_ID,
                                        lbusDroBenefitDetails.icdoDroBenefitDetails.dro_model_value);


                                    ltblPlanBenefit = lbusWithdrawalApplication.GetPlanAndBenefitOptionValue(lbusDroBenefitDetails.icdoDroBenefitDetails.plan_benefit_id);

                                    if (ltblPlanBenefit.Rows.Count > 0)
                                    {
                                        lbusBenefitApplicationDetail.iintPlan_ID = Convert.ToInt32(ltblPlanBenefit.Rows[0][0]);

                                        if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.MPIPP_PLAN_ID)
                                        {
                                            lbusBenefitApplicationDetail.istrPlanCode = busConstant.MPIPP;
                                        }
                                        else if (lbusBenefitApplicationDetail.iintPlan_ID == busConstant.IAP_PLAN_ID)
                                        {
                                            lbusBenefitApplicationDetail.istrPlanCode = busConstant.IAP;
                                        }

                                        DataTable ldtblist = busBase.Select("cdoPlan.GetPlanById", new object[1] { lbusBenefitApplicationDetail.iintPlan_ID });
                                        if (ldtblist.Rows.Count > 0)
                                        {
                                            lbusBenefitApplicationDetail.istrPlanName = ldtblist.Rows[0][busConstant.PLAN_NAME].ToString();
                                        }

                                        lbusBenefitApplicationDetail.istrPlanBenefitDescription =
                                            iobjPassInfo.isrvDBCache.GetCodeDescriptionString(busConstant.BENEFIT_OPTION_CODE_ID, Convert.ToString(ltblPlanBenefit.Rows[0][1]));
                                        lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.istrBenefitOptionValue = Convert.ToString(ltblPlanBenefit.Rows[0][1]);
                                    }

                                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.uvhp_flag = lbusDroBenefitDetails.icdoDroBenefitDetails.uvhp_flag;
                                    if (lbusDroBenefitDetails.icdoDroBenefitDetails.uvhp_flag == busConstant.FLAG_YES)
                                    {
                                        lbusBenefitApplicationDetail.istrSubPlan = busConstant.UVHP;
                                        lbusBenefitApplicationDetail.istrSubPlanDescription = busConstant.UVHP;
                                    }
                                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.ee_flag = lbusDroBenefitDetails.icdoDroBenefitDetails.ee_flag;
                                    if (lbusDroBenefitDetails.icdoDroBenefitDetails.ee_flag == busConstant.FLAG_YES)
                                    {
                                        lbusBenefitApplicationDetail.istrSubPlan = busConstant.EE;
                                        lbusBenefitApplicationDetail.istrSubPlanDescription = busConstant.EE;
                                    }
                                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.l52_spl_acc_flag = lbusDroBenefitDetails.icdoDroBenefitDetails.l52_spl_acc_flag;
                                    if (lbusDroBenefitDetails.icdoDroBenefitDetails.l52_spl_acc_flag == busConstant.FLAG_YES)
                                    {
                                        lbusBenefitApplicationDetail.istrSubPlan = busConstant.L52_SPL_ACC;
                                        lbusBenefitApplicationDetail.istrSubPlanDescription = busConstant.LOCAL_52_SPECIAL_ACCOUNT;
                                    }
                                    lbusBenefitApplicationDetail.icdoBenefitApplicationDetail.l161_spl_acc_flag = lbusDroBenefitDetails.icdoDroBenefitDetails.l161_spl_acc_flag;
                                    if (lbusDroBenefitDetails.icdoDroBenefitDetails.l161_spl_acc_flag == busConstant.FLAG_YES)
                                    {
                                        lbusBenefitApplicationDetail.istrSubPlan = busConstant.L161_SPL_ACC;
                                        lbusBenefitApplicationDetail.istrSubPlanDescription = busConstant.LOCAL1_161_SPECIAL_ACCOUNT;
                                    }
                                    lbusWithdrawalApplication.iclbBenefitApplicationDetail.Add(lbusBenefitApplicationDetail);
                                }
                            }


                        }

                        #endregion Alternate Payee Withdrawal Application
                    }
                }
                //Temp For Correspondence : Need a better solution
                lbusWithdrawalApplication.ibusRetirementApplication = new busRetirementApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                lbusWithdrawalApplication.ibusRetirementApplication.icdoBenefitApplication = lbusWithdrawalApplication.icdoBenefitApplication;
                lbusWithdrawalApplication.ibusRetirementApplication.ibusPerson = lbusWithdrawalApplication.ibusPerson;
            }
            lbusWithdrawalApplication.LoadPersonNotes(busConstant.WITHDRAWL_APPLICATION_MAINTAINENCE_FORM);
            //lbusWithdrawalApplication.ibusPerson.iclbNotes = busGlobalFunctions.LoadNotes(lbusWithdrawalApplication.ibusPerson.icdoPerson.person_id, 0, busConstant.WITHDRAWL_APPLICATION_MAINTAINENCE_FORM);
            return lbusWithdrawalApplication;
        }

        public busDisabilityApplication NewDisabilityApplication(string MPI_PERSON_ID, string benefit_type_value)
        {
            string lstrMpiPersonId = MPI_PERSON_ID.Trim();
            busDisabilityApplication lbusDisabilityApplication = new busDisabilityApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            lbusDisabilityApplication.ibusPerson = new busPerson() { icdoPerson = new cdoPerson() };
            DataTable ldtbPersonID = busBase.Select("cdoPerson.GetPersonDetails", new object[1] { lstrMpiPersonId });

            if (ldtbPersonID.Rows.Count > 0)
            {
                lbusDisabilityApplication.ibusPerson.icdoPerson.LoadData(ldtbPersonID.Rows[0]);
                lbusDisabilityApplication.icdoBenefitApplication.person_id = lbusDisabilityApplication.ibusPerson.icdoPerson.person_id;
            }

            lbusDisabilityApplication.iclbDisabilityBenefitHistory = new Collection<busDisabilityBenefitHistory>();
            lbusDisabilityApplication.icdoBenefitApplication.benefit_type_value = benefit_type_value;
            lbusDisabilityApplication.ibusPerson.LoadPersonAccounts(); //Load Person Accounts //Code-Abhishek
            //lbusDisabilityApplication.LoadandProcessWorkHistory_ForAllPlans(); //Code-Abhishek 
            lbusDisabilityApplication.LoadInitialData();
            lbusDisabilityApplication.iclbBenefitApplicationDetail = new Collection<busBenefitApplicationDetail>();
            lbusDisabilityApplication.EvaluateInitialLoadRules(utlPageMode.New);
            lbusDisabilityApplication.ibusPerson.iclbNotes = new Collection<busNotes>();
            lbusDisabilityApplication.ibusPerson.iclbNotes = busGlobalFunctions.LoadNotes(lbusDisabilityApplication.ibusPerson.icdoPerson.person_id, 0, busConstant.WITHDRAWL_APPLICATION_MAINTAINENCE_FORM);

            lbusDisabilityApplication.iclbPayeeAccount = new Collection<busPayeeAccount>();
            lbusDisabilityApplication.GetPayeeAccountsInReceivingSatus();

            return lbusDisabilityApplication;
        }

        public busDisabilityApplication FindDisabilityApplication(int aintbenefitapplicationid)
        {
            busDisabilityApplication lbusDisabilityApplication = new busDisabilityApplication();

            if (lbusDisabilityApplication.FindBenefitApplication(aintbenefitapplicationid))
            {
                lbusDisabilityApplication.ibusPerson = new busPerson();
                lbusDisabilityApplication.ibusPerson.FindPerson(lbusDisabilityApplication.icdoBenefitApplication.person_id);
                lbusDisabilityApplication.GetAgeAtRetirement(lbusDisabilityApplication.icdoBenefitApplication.retirement_date);
                lbusDisabilityApplication.ibusPerson.LoadPersonAccounts(); //Load Person Accounts //Code-Abhishek
                lbusDisabilityApplication.LoadandProcessWorkHistory_ForAllPlans(); //Code-Abhishek 
                lbusDisabilityApplication.LoadInitialData();
                lbusDisabilityApplication.LoadBenefitApplicationStatusHistorys();
                lbusDisabilityApplication.LoadBenefitApplicationDetails();
                lbusDisabilityApplication.LoadDisabilityBenefitHistory();

                //Temp For Correspondence : Need a better solution
                lbusDisabilityApplication.ibusRetirementApplication = new busRetirementApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                lbusDisabilityApplication.ibusRetirementApplication.icdoBenefitApplication = lbusDisabilityApplication.icdoBenefitApplication;
                lbusDisabilityApplication.ibusRetirementApplication.ibusPerson = lbusDisabilityApplication.ibusPerson;


                #region Disability Conversion

                lbusDisabilityApplication.iclbPayeeAccount = new Collection<busPayeeAccount>();
                lbusDisabilityApplication.GetPayeeAccountsInReceivingSatus();
                //Not required.
                //lbusDisabilityApplication.LoadForDisabilityConversion();

                #endregion
            }

            lbusDisabilityApplication.LoadPersonNotes(busConstant.DISABILITY_APPLICATION_MAINTAINENCE_FORM);
            // lbusDisabilityApplication.ibusPerson.iclbNotes = busGlobalFunctions.LoadNotes(lbusDisabilityApplication.ibusPerson.icdoPerson.person_id,0,busConstant.DISABILITY_APPLICATION_MAINTAINENCE_FORM);
            return lbusDisabilityApplication;
        }

        public busBenefitApplicationStatusHistory FindBenefitApplicationStatusHistory(int aintbenefitapplicationstatushistoryid)
        {
            busBenefitApplicationStatusHistory lobjBenefitApplicationStatusHistory = new busBenefitApplicationStatusHistory();
            if (lobjBenefitApplicationStatusHistory.FindBenefitApplicationStatusHistory(aintbenefitapplicationstatushistoryid))
            {
                lobjBenefitApplicationStatusHistory.LoadBenefitApplication();
            }

            return lobjBenefitApplicationStatusHistory;
        }

        public busBenefitApplicationLookup LoadBenefitApplications(DataTable adtbSearchResult)
        {
            busBenefitApplicationLookup lobjBenefitApplicationLookup = new busBenefitApplicationLookup();
            lobjBenefitApplicationLookup.LoadBenefitApplications(adtbSearchResult);
            return lobjBenefitApplicationLookup;
        }

        public busPlanBenefitXr FindPlanBenefitXr(int aintplanbenefitid)
        {
            busPlanBenefitXr lobjPlanBenefitXr = new busPlanBenefitXr();
            if (lobjPlanBenefitXr.FindPlanBenefitXr(aintplanbenefitid))
            {
            }

            return lobjPlanBenefitXr;
        }

        public busPlanBenefitXr NewPlanBenefitXr()
        {
            busPlanBenefitXr lobjPlanBenefitXr = new busPlanBenefitXr();
            lobjPlanBenefitXr.icdoPlanBenefitXr = new cdoPlanBenefitXr();
            return lobjPlanBenefitXr;
        }

        public busBenefitApplicationDetail FindBenefitApplicationDetail(int aintbenefitapplicationdetailid)
        {
            busBenefitApplicationDetail lobjBenefitApplicationDetail = new busBenefitApplicationDetail();
            if (lobjBenefitApplicationDetail.FindBenefitApplicationDetail(aintbenefitapplicationdetailid))
            {
                lobjBenefitApplicationDetail.LoadPlanBenefitXr();
            }

            return lobjBenefitApplicationDetail;
        }


        public busDeathPreRetirement NewDeathPreRetirement(string MPI_PERSON_ID, string benefit_type_value)
        {
            string lstrMpiPersonId = MPI_PERSON_ID.Trim();
            busDeathPreRetirement lbusDeathPreRetirement = new busDeathPreRetirement { icdoBenefitApplication = new cdoBenefitApplication() };
            lbusDeathPreRetirement.ibusPerson = new busPerson() { icdoPerson = new cdoPerson() };
            DataTable ldtbPersonID = busBase.Select("cdoPerson.GetPersonDetails", new object[1] { lstrMpiPersonId });

            if (ldtbPersonID.Rows.Count > 0)
            {
                lbusDeathPreRetirement.ibusPerson.icdoPerson.LoadData(ldtbPersonID.Rows[0]);
                lbusDeathPreRetirement.icdoBenefitApplication.person_id = lbusDeathPreRetirement.ibusPerson.icdoPerson.person_id;
            }

            lbusDeathPreRetirement.icdoBenefitApplication.benefit_type_value = benefit_type_value;
            lbusDeathPreRetirement.GetAgeAtDeath();
            lbusDeathPreRetirement.ibusPerson.LoadPersonAccounts(); //Code-Abhishek
            //lbusDeathPreRetirement.LoadandProcessWorkHistory_ForAllPlans();//Code-Abhishekfor Eligibility
            lbusDeathPreRetirement.LoadInitialData();
            lbusDeathPreRetirement.iclbBenefitApplicationDetail = new Collection<busBenefitApplicationDetail>();
            lbusDeathPreRetirement.iclbPayeeAccount = new Collection<busPayeeAccount>();
            lbusDeathPreRetirement.GetPayeeAccountsInApprovedOrReviewSatus();
            lbusDeathPreRetirement.EvaluateInitialLoadRules(utlPageMode.New);
            lbusDeathPreRetirement.ibusPerson.iclbNotes = new Collection<busNotes>();
            lbusDeathPreRetirement.ibusPerson.iclbNotes = busGlobalFunctions.LoadNotes(lbusDeathPreRetirement.ibusPerson.icdoPerson.person_id, 0, busConstant.DISABILITY_APPLICATION_MAINTAINENCE_FORM);

            return lbusDeathPreRetirement;
        }


        public busDeathPreRetirement FindDeathPreRetirement(int aintbenefitapplicationid)
        {
            busDeathPreRetirement lbusDeathPreRetirement = new busDeathPreRetirement();

            if (lbusDeathPreRetirement.FindBenefitApplication(aintbenefitapplicationid))
            {
                lbusDeathPreRetirement.ibusPerson = new busPerson();
                lbusDeathPreRetirement.ibusPerson.FindPerson(lbusDeathPreRetirement.icdoBenefitApplication.person_id);
                lbusDeathPreRetirement.GetAgeAtDeath();
                lbusDeathPreRetirement.GetAgeAtRetirement(lbusDeathPreRetirement.icdoBenefitApplication.retirement_date);
                lbusDeathPreRetirement.ibusPerson.LoadPersonAccounts();
                lbusDeathPreRetirement.LoadandProcessWorkHistory_ForAllPlans();//Code-Abhishekfor Eligibility
                lbusDeathPreRetirement.LoadInitialData();
                lbusDeathPreRetirement.LoadBenefitApplicationDetails();
                //This has been Added since Eligibility CHecking is Dynamic and thereFore Older Soft Error should not be displayed
                lbusDeathPreRetirement.ValidateSoftErrors();
                lbusDeathPreRetirement.LoadErrors();
                lbusDeathPreRetirement.EvaluateInitialLoadRules();
                lbusDeathPreRetirement.iclbPayeeAccount = new Collection<busPayeeAccount>();
                lbusDeathPreRetirement.GetPayeeAccountsInApprovedOrReviewSatus();

                //Temp For Correspondence : Need a better solution
                lbusDeathPreRetirement.ibusRetirementApplication = new busRetirementApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                lbusDeathPreRetirement.ibusRetirementApplication.icdoBenefitApplication = lbusDeathPreRetirement.icdoBenefitApplication;
                lbusDeathPreRetirement.ibusRetirementApplication.ibusPerson = lbusDeathPreRetirement.ibusPerson;
            }

            lbusDeathPreRetirement.LoadPersonNotes(busConstant.DEATH_PRE_RETIREMENT_MAINTANENCE_FORM);
            // lbusDeathPreRetirement.ibusPerson.iclbNotes = busGlobalFunctions.LoadNotes(lbusDeathPreRetirement.ibusPerson.icdoPerson.person_id, 0, busConstant.DEATH_PRE_RETIREMENT_MAINTANENCE);
            return lbusDeathPreRetirement;
        }

        //FM upgrade: 6.0.0.31 changes - public to protected
        protected override ArrayList ValidateNewChild(string astrFormName, object aobjParentObject, Type atypBusObject, Hashtable ahstParams)
        {
            ArrayList larrErrors = new ArrayList();
            iobjPassInfo.iconFramework.Open();
            try
            {

                if (astrFormName == busConstant.RETIREMENT_APPLICATION_MAINTAINENCE)
                {
                    busRetirementApplication lbusRetirmentApplication = aobjParentObject as busRetirementApplication;

                    if (atypBusObject.Name == busConstant.BUS_BENEFIT_APPLICATION_DETAIL)
                    {
                        ahstParams["icdoBenefitApplicationDetail.istrBenefitOptionValue"] = ahstParams["istrBenefitOptionValue"];
                        ahstParams["icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue"] = ahstParams["istrSubPlanBenefitOptionValue"];
                        ahstParams["icdoBenefitApplicationDetail.iintPlan_ID"] = ahstParams["iintPlan_ID"];
                        ahstParams["icdoBenefitApplicationDetail.plan_benefit_id"] = ahstParams["plan_benefit_id"];
                        ahstParams["icdoBenefitApplicationDetail.iintJointAnnuaintID"] = ahstParams["iintJointAnnuaintID"];
                        ahstParams["icdoBenefitApplicationDetail.spousal_consent_flag"] = ahstParams["spousal_consent_flag"];

                        larrErrors = lbusRetirmentApplication.CheckErrorOnAddButton(aobjParentObject as busRetirementApplication, ahstParams, ref larrErrors);
                        lbusRetirmentApplication.CheckDateOfMarriage(lbusRetirmentApplication, ahstParams, ref larrErrors);
                    }
                }
                else if (astrFormName == busConstant.WITHDRAWAL_APPLICATION_MAINTAINENCE)
                {
                    busWithdrawalApplication lbusWithdrawalApplication = aobjParentObject as busWithdrawalApplication;

                    if (atypBusObject.Name == busConstant.BUS_BENEFIT_APPLICATION_DETAIL)
                    {
                        ahstParams["icdoBenefitApplicationDetail.istrBenefitOptionValue"] = ahstParams["istrBenefitOptionValue"];
                        ahstParams["icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue"] = ahstParams["istrSubPlanBenefitOptionValue"];
                        ahstParams["icdoBenefitApplicationDetail.iintPlan_ID"] = ahstParams["iintPlan_ID"];
                        ahstParams["icdoBenefitApplicationDetail.plan_benefit_id"] = ahstParams["plan_benefit_id"];
                        ahstParams["icdoBenefitApplicationDetail.iintJointAnnuaintID"] = ahstParams["iintJointAnnuaintID"];
                        ahstParams["icdoBenefitApplicationDetail.spousal_consent_flag"] = ahstParams["spousal_consent_flag"];

                        larrErrors = lbusWithdrawalApplication.CheckErrorOnAddButton(aobjParentObject as busWithdrawalApplication, ahstParams, ref larrErrors);
                    }
                }
                else if (astrFormName == busConstant.DISABILITY_APPLICATION_MAINTAINENCE)
                {
                    busDisabilityApplication lbusDisabilityApplication = aobjParentObject as busDisabilityApplication;

                    if (atypBusObject.Name == busConstant.BUS_BENEFIT_APPLICATION_DETAIL)
                    {
                        ahstParams["icdoBenefitApplicationDetail.istrBenefitOptionValue"] = ahstParams["istrBenefitOptionValue"];
                        ahstParams["icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue"] = ahstParams["istrSubPlanBenefitOptionValue"];
                        ahstParams["icdoBenefitApplicationDetail.iintPlan_ID"] = ahstParams["iintPlan_ID"];
                        ahstParams["icdoBenefitApplicationDetail.plan_benefit_id"] = ahstParams["plan_benefit_id"];
                        ahstParams["icdoBenefitApplicationDetail.iintJointAnnuaintID"] = ahstParams["iintJointAnnuaintID"];
                        ahstParams["icdoBenefitApplicationDetail.spousal_consent_flag"] = ahstParams["spousal_consent_flag"];

                        lbusDisabilityApplication.checkMarriage(lbusDisabilityApplication, ahstParams, ref larrErrors);
                        lbusDisabilityApplication.CheckDatesOnBenefitAppDetails(lbusDisabilityApplication, ahstParams, ref larrErrors);
                        larrErrors = lbusDisabilityApplication.CheckErrorOnAddButton(aobjParentObject as busDisabilityApplication, ahstParams, ref larrErrors);

                    }
                    else if (atypBusObject.Name == busConstant.BUS_DISABILITY_BENEFIT_HISTORY)
                    {
                        lbusDisabilityApplication.CheckDuplicatePlan(ahstParams, ref larrErrors);
                    }
                }
                else if (astrFormName == busConstant.DEATH_PRE_RETIREMENT_MAINTANENCE)
                {

                    busDeathPreRetirement lbusDeathPreRetirement = aobjParentObject as busDeathPreRetirement;
                    if (atypBusObject.Name == busConstant.BUS_BENEFIT_APPLICATION_DETAIL)
                    {
                        ahstParams["icdoBenefitApplicationDetail.istrSurvivorTypeValue"] = ahstParams["istrSurvivorTypeValue"];
                        ahstParams["icdoBenefitApplicationDetail.istrSubPlanBenefitOptionValue"] = ahstParams["istrSubPlanBenefitOptionValue"];
                        ahstParams["icdoBenefitApplicationDetail.iintPlan_ID"] = ahstParams["iintPlan_ID"];
                        ahstParams["icdoBenefitApplicationDetail.survivor_id"] = ahstParams["survivor_id"];
                        ahstParams["icdoBenefitApplicationDetail.organization_id"] = ahstParams["organization_id"];
                        ahstParams["icdoBenefitApplicationDetail.istrBenefitOptionValue"] = ahstParams["istrBenefitOptionValue"];
                        larrErrors = lbusDeathPreRetirement.CheckErrorOnAddButtonDeath(aobjParentObject as busDeathPreRetirement, ahstParams, ref larrErrors);
                    }
                }
                //else if (astrFormName == busConstant.QRDO_MAINTAINENCE)
                //{
                //    if (aobjParentObject is busQdroApplication)
                //    {
                //        busQdroApplication lbusQdroApplication = aobjParentObject as busQdroApplication;
                //        larrErrors = lbusQdroApplication.ValidateNewChild(ahstParams,true);
                //    }
                //}
                else if (astrFormName == busConstant.RETIREMENT_WIZARD)
                {
                    busRetirementWizard lbusRetirementWizard = aobjParentObject as busRetirementWizard;

                  //  if (atypBusObject.Name == busConstant.BUS_BENEFIT_APPLICATION_DETAIL)
                    //{

                        larrErrors = lbusRetirementWizard.CheckErrorOnAddButton(aobjParentObject as busRetirementWizard, ahstParams, ref larrErrors);
                       // lbusRetirmentApplication.CheckDateOfMarriage(lbusRetirmentApplication, ahstParams, ref larrErrors);
                    //}
                }
                else if (astrFormName == busConstant.PARTICIPANT_BENEFICIARY_MAINTENANCE)
                {

                    if (aobjParentObject is busRelationship)
                    {
                        busPersonBeneficiary lbusPersonBeneficiary = aobjParentObject as busPersonBeneficiary;
                        //if (Convert.ToString(ahstParams["start_date"]).IsNullOrEmpty())
                        //{
                        //    ahstParams["start_date"] = DateTime.Today;
                        //}
                        larrErrors = lbusPersonBeneficiary.ValidateNewChild(ahstParams);
                    }
                }
                else if (astrFormName == busConstant.PERSON_MAINTENANCE)
                {
                    busPerson lbusPerson = aobjParentObject as busPerson;
                    if (atypBusObject.Name == busConstant.BUS_PERSON_BRIDGE_HOURS)
                    {
                        ahstParams["icdoPersonBridgeHours.person_bridge_id"] = 0;
                        ahstParams["icdoPersonBridgeHours.bridge_type_value"] = ahstParams["bridge_type_value"];
                        ahstParams["icdoPersonBridgeHours.bridge_start_date"] =  ahstParams["bridge_start_date"];
                        ahstParams["icdoPersonBridgeHours.bridge_end_date"] =  ahstParams["bridge_end_date"];

                        larrErrors = lbusPerson.CheckErrorOnAddButton(aobjParentObject as busPerson, ahstParams, ref larrErrors);
                        if (larrErrors.Count > 0)
                        {
                            return larrErrors;
                        }
                        lbusPerson.CheckOverlappingPeriod(aobjParentObject as busPerson, ahstParams, ref larrErrors);
                    }
                }

                else if (astrFormName == busConstant.QRDO_MAINTAINENCE)
                {
                    if (atypBusObject.Name == busConstant.BUS_DRO_BENIFIT_DETAILS)
                    {
                        ahstParams["icdoDroBenefitDetails.plan_id"] = ahstParams["plan_id"];
                        ahstParams["icdoDroBenefitDetails.dro_model_value"] = ahstParams["dro_model_value"];
                        ahstParams["icdoDroBenefitDetails.benefit_perc"] = ahstParams["benefit_perc"];
                        ahstParams["icdoDroBenefitDetails.istrBenefitOptionValue"] = ahstParams["istrBenefitOptionValue"];
                        ahstParams["icdoDroBenefitDetails.benefit_amt"] = ahstParams["benefit_amt"];
                        ahstParams["icdoDroBenefitDetails.benefit_flat_perc"] = ahstParams["benefit_flat_perc"];
                        ahstParams["icdoDroBenefitDetails.dro_withheld_perc"] = ahstParams["dro_withheld_perc"];
                        ahstParams["icdoDroBenefitDetails.alt_payee_increase"] = ahstParams["alt_payee_increase"];
                        ahstParams["icdoDroBenefitDetails.alt_payee_early_ret"] = ahstParams["alt_payee_early_ret"];
                        ahstParams["icdoDroBenefitDetails.balance_as_of_plan_year"] = ahstParams["balance_as_of_plan_year"];
                        ahstParams["icdoDroBenefitDetails.alt_payee_benfit_cap_year"] = ahstParams["alt_payee_benefit_cap_year"];
                        ahstParams["icdoDroBenefitDetails.net_investment_from_date"] = ahstParams["net_investment_from_date"];
                        ahstParams["icdoDroBenefitDetails.net_investment_to_date"] = ahstParams["net_investment_to_date"];
                        ahstParams["icdoDroBenefitDetails.is_alt_payee_eligible_for_participant_retiree_increase"] = ahstParams["is_alt_payee_eligible_for_participant_retiree_increase"];

                        busQdroApplication lbusQdroApplication = aobjParentObject as busQdroApplication;
                        larrErrors = lbusQdroApplication.ValidateNewChild(ahstParams, true);
                    }
                }

            }
            finally
            {
                iobjPassInfo.iconFramework.Close();
            }
            return larrErrors;
        }


        public busPersonContact NewPersonContact(int aintPersonId)
        {
            busPersonContact lobjPersonContact = new busPersonContact();
            lobjPersonContact.icdoPersonContact = new cdoPersonContact();
            lobjPersonContact.icdoPersonContact.person_id = aintPersonId;

            lobjPersonContact.ibusPerson = new busPerson();
            lobjPersonContact.ibusPerson.FindPerson(aintPersonId);
            lobjPersonContact.LoadActiveAddress();

            return lobjPersonContact;
        }


        public busPersonContact FindPersonContact(int aintpersoncontactid)
        {
            busPersonContact lobjPersonContact = new busPersonContact();
            if (lobjPersonContact.FindPersonContact(aintpersoncontactid))
            {
                lobjPersonContact.LoadPerson();
                lobjPersonContact.LoadActiveAddress();
            }

            return lobjPersonContact;
        }

        public busPersonDependent NewPersonDependent(int aintPersonId)
        {
            busPersonDependent lobjPersonDependent = new busPersonDependent();
            lobjPersonDependent.icdoRelationship = new cdoRelationship();
            lobjPersonDependent.icdoRelationship.person_id = aintPersonId;

            lobjPersonDependent.ibusPerson = new busPerson();
            lobjPersonDependent.ibusPerson.FindPerson(aintPersonId);

            return lobjPersonDependent;
        }

        public busPersonDependent FindPersonDependent(int aintdependentid)
        {
            busPersonDependent lobjPersonDependent = new busPersonDependent();
            if (lobjPersonDependent.FindRelationship(aintdependentid))
            {
                lobjPersonDependent.ibusPersonDependent = new busPerson();
                if (lobjPersonDependent.icdoRelationship.dependent_person_id != 0)
                {
                    lobjPersonDependent.ibusPersonDependent.FindPerson(lobjPersonDependent.icdoRelationship.dependent_person_id);
                }
                lobjPersonDependent.ibusPerson = new busPerson();
                lobjPersonDependent.ibusPerson.FindPerson(lobjPersonDependent.icdoRelationship.person_id);
            }

            return lobjPersonDependent;
        }

        public busPersonAddressHistory FindPersonAddressHistory(int aintpersonaddresshistoryid)
        {
            busPersonAddressHistory lobjPersonAddressHistory = new busPersonAddressHistory();
            if (lobjPersonAddressHistory.FindPersonAddressHistory(aintpersonaddresshistoryid))
            {
                lobjPersonAddressHistory.LoadPersonAddress();
            }

            return lobjPersonAddressHistory;
        }

        public busNotes FindNotes(int aintnoteid)
        {
            busNotes lobjNotes = new busNotes();
            if (lobjNotes.FindNotes(aintnoteid))
            {
            }

            return lobjNotes;
        }

        public busDisabilityBenefitHistory FindDisabilityBenefitHistory(int aintdisabilitybenefithistoryid)
        {
            busDisabilityBenefitHistory lobjDisabilityBenefitHistory = new busDisabilityBenefitHistory();
            if (lobjDisabilityBenefitHistory.FindDisabilityBenefitHistory(aintdisabilitybenefithistoryid))
            {
            }

            return lobjDisabilityBenefitHistory;
        }


        public busDeathNotification NewDeathNotification(string MPI_PERSON_ID)
        {
            string lstrMpiPersonId = MPI_PERSON_ID.Trim();
            busDeathNotification lbusDeathNotification = new busDeathNotification { icdoDeathNotification = new cdoDeathNotification() };
            lbusDeathNotification.ibusPerson = new busPerson() { icdoPerson = new cdoPerson() };
            DataTable ldtbPersonID = busBase.Select("cdoPerson.GetPersonDetails", new object[1] { lstrMpiPersonId });

            if (ldtbPersonID.Rows.Count > 0)
            {
                lbusDeathNotification.ibusPerson.icdoPerson.LoadData(ldtbPersonID.Rows[0]);
                lbusDeathNotification.icdoDeathNotification.person_id = lbusDeathNotification.ibusPerson.icdoPerson.person_id;

                lbusDeathNotification.ibusPerson.FindPerson(lbusDeathNotification.icdoDeathNotification.person_id);
                lbusDeathNotification.ibusPerson.LoadPersonAccounts();
                foreach (busPersonAccount lbusPersonAccount in lbusDeathNotification.ibusPerson.iclbPersonAccount)
                {
                    DataTable ldtblist = busBase.Select("cdoPlan.GetPlanById", new object[1] { lbusPersonAccount.icdoPersonAccount.plan_id });
                    if (ldtblist.Rows.Count > 0)
                    {
                        lbusPersonAccount.icdoPersonAccount.istrPlan = ldtblist.Rows[0][0].ToString();
                    }
                }

                lbusDeathNotification.ibusPerson.LoadPersonContacts();
                lbusDeathNotification.ibusPerson.LoadBeneficiariesForDeath();
                foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in lbusDeathNotification.ibusPerson.iclbPersonAccountBeneficiary)
                {
                    lbusPersonAccountBeneficiary.ibusPerson = new busPerson();
                    lbusPersonAccountBeneficiary.ibusPerson.FindPerson(lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.iaintBenID);

                }


                lbusDeathNotification.ibusPerson.LoadPersonDependents();
                foreach (busRelationship lbusRelationship in lbusDeathNotification.ibusPerson.iclbPersonDependent)
                {
                    lbusRelationship.ibusPerson = new busPerson();
                    lbusRelationship.ibusPerson.FindPerson(lbusRelationship.icdoRelationship.dependent_person_id);

                }


                lbusDeathNotification.LoadBenficiaryOf();
                lbusDeathNotification.LoadDependentOf();
                lbusDeathNotification.LoadDroApplicationDetails();
                lbusDeathNotification.LoadBenefitApplicationDetails();
                lbusDeathNotification.LoadPayeeAccount();



            }
            lbusDeathNotification.EvaluateInitialLoadRules(utlPageMode.New);
            lbusDeathNotification.ibusPerson.iclbNotes = new Collection<busNotes>();
            lbusDeathNotification.ibusPerson.iclbNotes = busGlobalFunctions.LoadNotes(lbusDeathNotification.ibusPerson.icdoPerson.person_id, 0, busConstant.DEATH_PRE_RETIREMENT_MAINTANENCE);
            return lbusDeathNotification;
        }

        public busDeathNotification FindDeathNotification(int aintdeathnotificationid)
        {
            busDeathNotification lobjDeathNotification = new busDeathNotification();
            if (lobjDeathNotification.FindDeathNotification(aintdeathnotificationid))
            {
                lobjDeathNotification.ibusPerson = new busPerson();
                lobjDeathNotification.ibusPerson.FindPerson(lobjDeathNotification.icdoDeathNotification.person_id);
                lobjDeathNotification.ibusPerson.LoadPersonAccounts();
                foreach (busPersonAccount lbusPersonAccount in lobjDeathNotification.ibusPerson.iclbPersonAccount)
                {
                    DataTable ldtblist = busBase.Select("cdoPlan.GetPlanById", new object[1] { lbusPersonAccount.icdoPersonAccount.plan_id });
                    if (ldtblist.Rows.Count > 0)
                    {
                        lbusPersonAccount.icdoPersonAccount.istrPlan = ldtblist.Rows[0][0].ToString();
                    }
                }

                lobjDeathNotification.ibusPerson.LoadPersonContacts();
                lobjDeathNotification.ibusPerson.LoadBeneficiariesForDeath();
                foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in lobjDeathNotification.ibusPerson.iclbPersonAccountBeneficiary)
                {
                    lbusPersonAccountBeneficiary.ibusPerson = new busPerson();
                    lbusPersonAccountBeneficiary.ibusPerson.FindPerson(lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.iaintBenID);
                }


                //lobjDeathNotification.ibusPerson.LoadPersonDependentsForDeath();
                //foreach (busRelationship lbusRelationship in lobjDeathNotification.ibusPerson.iclbPersonDependent)
                //{

                //        lbusRelationship.ibusPerson = new busPerson();
                //        lbusRelationship.ibusPerson.FindPerson(lbusRelationship.icdoRelationship.dependent_person_id);
                //}



                lobjDeathNotification.LoadBenficiaryOf();
                lobjDeathNotification.LoadDependentOf();
                lobjDeathNotification.LoadDroApplicationDetails();
                lobjDeathNotification.LoadBenefitApplicationDetails();
                lobjDeathNotification.LoadPayeeAccount();


            }

            lobjDeathNotification.ibusPerson.iclbNotes = busGlobalFunctions.LoadNotes(lobjDeathNotification.ibusPerson.icdoPerson.person_id, 0, busConstant.DEATH_NOTIFICATION_MAINTANENCE_FORM);
            return lobjDeathNotification;
        }

        public int GetActivityInstanceCount(int aintPersonId, int aintPersonAccountId, string astrControlId)
        {
            DataTable ldtblist;
            if (astrControlId== "btnCancelBPM")
            {
                ldtblist = busBase.Select("entFramework.CheckCancelActivityDuplicateInstanceIsRunning", new object[1] { aintPersonId });
            }
            else
            {
                ldtblist = busBase.Select("entFramework.CheckDuplicateInstanceIsRunning", new object[3] { busConstant.PersonAccountMaintenance.SERVICE_RETIREMENT_BPM, aintPersonId, aintPersonAccountId });
            }
            if (ldtblist.Rows.Count > 0)
            {
                return 1;
            }
            return busConstant.ZERO_INT;
        }

        public int CancelActivityInstance(int aintPersonId, int aintPersonAccountId, string astrControlId)
        {
            DataTable ldtblist = busBase.Select("entFramework.CheckCancelActivityDuplicateInstanceIsRunning", new object[1] { aintPersonId});
            if (ldtblist.Rows.Count > 0)
            {
                foreach (var item in ldtblist.Rows)
                {
                    busBpmCaseInstance lbusBpmCaseInstance = new busBpmCaseInstance { icdoBpmCaseInstance = new doBpmCaseInstance() };
                    lbusBpmCaseInstance.FindByPrimaryKey(Convert.ToInt32(((System.Data.DataRow)item).ItemArray[0]));
                    lbusBpmCaseInstance.AbortCaseInstance();
                }

            }
            if (ldtblist.Rows.Count > 0)
            {
                return 1;
            }
            return busConstant.ZERO_INT;
        }

        public busPersonAccount FindPersonAccountDetail(int aintPersonAccountId, string astrPlanDescription, string astrPersonScreen)
        {
            busPersonAccount lbusPersonAccount = new busPersonAccount();

            if (lbusPersonAccount.FindPersonAccount(aintPersonAccountId))
            {
                lbusPersonAccount.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusPersonAccount.ibusPerson.FindPerson(lbusPersonAccount.icdoPersonAccount.person_id);
                lbusPersonAccount.icdoPersonAccount.istrPlan = astrPlanDescription;
                lbusPersonAccount.PopulateDescriptions();
                if (utlPassInfo.iobjPassInfo.itrnFramework == null)
                {
                    BPMDBHelper.BeginTransaction(utlPassInfo.iobjPassInfo);
                }
                if (astrPersonScreen == busConstant.FLAG_YES)
                {   
                    DataTable ldtblist = busBase.Select("entFramework.CheckDuplicateInstanceIsRunning", new object[3] { busConstant.PersonAccountMaintenance.SERVICE_RETIREMENT_BPM, lbusPersonAccount.icdoPersonAccount.person_id, aintPersonAccountId });
                    if (ldtblist.Rows.Count <= 0)
                    {
                        ArrayList larrResult = new ArrayList();
                        Dictionary<string, object> lhstRequestParameters = new Dictionary<string, object>();
                        lhstRequestParameters.Add("ReferenceId", lbusPersonAccount.icdoPersonAccount.person_account_id);
                        lhstRequestParameters.Add(busConstant.PersonAccountMaintenance.PERSON_ID, lbusPersonAccount.icdoPersonAccount.person_id);
                        lhstRequestParameters.Add(busConstant.PersonAccountMaintenance.PERSON_ACCOUNT_ID, lbusPersonAccount.icdoPersonAccount.person_account_id);
                        lhstRequestParameters.Add(busConstant.PersonAccountMaintenance.PLAN_DESCRIPTION, astrPlanDescription);
                        larrResult = BpmHelper.InitiateCaseInstance(busConstant.PersonAccountMaintenance.SERVICE_RETIREMENT_BPM, busConstant.PersonAccountMaintenance.SERVICE_RETIREMENT_PROCESS, lbusPersonAccount.icdoPersonAccount.person_id, 0, lbusPersonAccount.icdoPersonAccount.person_account_id, utlPassInfo.iobjPassInfo, astrUserIdToWhomActivityToBeAssigned: iobjPassInfo.istrUserID, adctRequestParameters: lhstRequestParameters, ablnCheckForExistingInstance: true, ablnReturnCaseInstance: true, aenmActivityInitiateType: enmActivityInitiateType.Initiate);
                        if (larrResult != null && larrResult.Count > 0 && !(larrResult[0] is utlError))
                        {
                            if (larrResult.Count == 2 && larrResult[0] is busSolBpmActivityInstance)
                            {
                                lbusPersonAccount.LoadActivityInstance(larrResult[0] as busSolBpmActivityInstance);
                            }
                        }

                        DBFunction.DBNonQuery("entPersonAccount.ResetAllField",
                        new object[1] { lbusPersonAccount.icdoPersonAccount.person_account_id },
                                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                        iobjPassInfo.Commit();
                        lbusPersonAccount.FindPersonAccount(aintPersonAccountId);
                    }
                }
                else if (astrPersonScreen == busConstant.FLAG_NO)
                {
                    DataTable ldtblist = busBase.Select("entFramework.CheckDuplicateInstanceIsRunning", new object[3] { busConstant.PersonAccountMaintenance.CANCEL_SERVICE_RETIREMENT_BPM, lbusPersonAccount.icdoPersonAccount.person_id, aintPersonAccountId });
                    if (ldtblist.Rows.Count <= 0)
                    {
                        ArrayList larrResult = new ArrayList();
                        Dictionary<string, object> lhstRequestParameters = new Dictionary<string, object>();
                        lhstRequestParameters.Add("ReferenceId", lbusPersonAccount.icdoPersonAccount.person_account_id);
                        lhstRequestParameters.Add(busConstant.PersonAccountMaintenance.PERSON_ID, lbusPersonAccount.icdoPersonAccount.person_id);
                        lhstRequestParameters.Add(busConstant.PersonAccountMaintenance.PERSON_ACCOUNT_ID, lbusPersonAccount.icdoPersonAccount.person_account_id);
                        lhstRequestParameters.Add(busConstant.PersonAccountMaintenance.PLAN_DESCRIPTION, astrPlanDescription);
                        lhstRequestParameters.Add(busConstant.PersonAccountMaintenance.SOURCE_OF_CANCELLATION, "Cancel Application");
                        larrResult = BpmHelper.InitiateCaseInstance(busConstant.PersonAccountMaintenance.CANCEL_SERVICE_RETIREMENT_BPM, busConstant.PersonAccountMaintenance.CANCEL_SERVICE_RETIREMENT_APPLICATION, lbusPersonAccount.icdoPersonAccount.person_id, 0, lbusPersonAccount.icdoPersonAccount.person_account_id, utlPassInfo.iobjPassInfo, astrUserIdToWhomActivityToBeAssigned: iobjPassInfo.istrUserID, adctRequestParameters: lhstRequestParameters, ablnCheckForExistingInstance: true, ablnReturnCaseInstance: true, aenmActivityInitiateType: enmActivityInitiateType.Initiate);
                        if (larrResult != null && larrResult.Count > 0 && !(larrResult[0] is utlError))
                        {
                            if (larrResult.Count == 2 && larrResult[0] is busSolBpmActivityInstance)
                            {
                                lbusPersonAccount.LoadActivityInstance(larrResult[0] as busSolBpmActivityInstance);
                            }
                        }
                    }
                }
            }
            return lbusPersonAccount;
        }

        public busDeathNotificationLookup LoadDeathNotifications(DataTable adtbSearchResult)
        {
            busDeathNotificationLookup lobjDeathNotificationLookup = new busDeathNotificationLookup();
            lobjDeathNotificationLookup.LoadDeathNotifications(adtbSearchResult);
            return lobjDeathNotificationLookup;
        }

        public busDroApplicationStatusHistory FindDroApplicationStatusHistory(int aintdroapplicationstatushistoryid)
        {
            busDroApplicationStatusHistory lobjDroApplicationStatusHistory = new busDroApplicationStatusHistory();
            if (lobjDroApplicationStatusHistory.FindDroApplicationStatusHistory(aintdroapplicationstatushistoryid))
            {
            }

            return lobjDroApplicationStatusHistory;
        }

        public busPersonAccountEligibility FindPersonAccountEligibility(int aintpersonaccounteligibilityid)
        {
            busPersonAccountEligibility lobjPersonAccountEligibility = new busPersonAccountEligibility();
            if (lobjPersonAccountEligibility.FindPersonAccountEligibility(aintpersonaccounteligibilityid))
            {
            }

            return lobjPersonAccountEligibility;
        }

        public void InsertPersonRecordFromOPUSWebService(string astrPrefix, string astrFirstName, string astrLastName, string astrMiddleName, string astrSuffix, string astrDateofBirth, string astrSSN)
        {
            int lPersonID = 0;
            busPerson lbusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lbusPerson.icdoPerson.name_prefix_id = 6025;
            lbusPerson.icdoPerson.name_prefix_value = astrPrefix;
            lbusPerson.icdoPerson.first_name = astrFirstName;
            lbusPerson.icdoPerson.middle_name = astrMiddleName;
            lbusPerson.icdoPerson.last_name = astrLastName;
            lbusPerson.icdoPerson.name_suffix = astrSuffix;
            // lbusPerson.icdoPerson.idtDateofBirth = Convert.ToDateTime(astrDateofBirth);
            lbusPerson.icdoPerson.ssn = astrSSN;
            lbusPerson.icdoPerson.created_by = "OPUSWebService";
            lbusPerson.icdoPerson.modified_by = "OPUSWebService";
            lbusPerson.icdoPerson.Insert();

            lPersonID = lbusPerson.icdoPerson.person_id;
            lbusPerson.icdoPerson.mpi_person_id = "M" + lPersonID;
            lbusPerson.icdoPerson.Update();

        }

        public void AddUpdatePersonAddressFromOPUSService(string astrSSN, string astrAddressLine1, string astrAddressLine2, string astrCity, string astrState, string astrZipCode, string astrZipCode4, string astrCountryCode,
        string astrAddressType, string astrAddressEndDate)
        {
            int aintPersonID = 0;

            string lstrEncryptedSSN = astrSSN;

            if (!string.IsNullOrEmpty(astrSSN))
            {
                DataTable ldtblist = busBase.Select("cdoPerson.GetPersonIDFromSSN", new object[] { lstrEncryptedSSN });
                if (ldtblist.Rows.Count > 0)
                {
                    aintPersonID = Convert.ToInt32(ldtblist.Rows[0][0]);
                }
            }

            DataTable ldtbPersonAddressList = busBase.Select<cdoPersonAddress>(
            new string[1] { enmPersonAddress.person_id.ToString() },
            new object[1] { aintPersonID }, null, enmPersonAddress.start_date.ToString());


            if (ldtbPersonAddressList.Rows.Count > 0)
            {
                foreach (DataRow ldtRow in ldtbPersonAddressList.Rows)
                {

                    if (Convert.ToDateTime(ldtRow[enmPersonAddress.start_date.ToString().ToUpper()]) <= DateTime.Now
                        && (ldtRow[enmPersonAddress.end_date.ToString().ToUpper()] == DBNull.Value
                        || Convert.ToDateTime(ldtRow[enmPersonAddress.end_date.ToString().ToUpper()]) >= DateTime.Now
                        || Convert.ToDateTime(ldtRow[enmPersonAddress.end_date.ToString().ToUpper()]) == DateTime.MinValue))
                    {
                        busPersonAddress lbusPersonAddress = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() };
                        lbusPersonAddress.icdoPersonAddress.LoadData(ldtRow);

                        lbusPersonAddress.LoadPersonAddressChklists();

                        if (!string.IsNullOrEmpty(astrAddressLine1))
                        {
                            lbusPersonAddress.icdoPersonAddress.addr_line_1 = astrAddressLine1;
                        }
                        if (!string.IsNullOrEmpty(astrAddressLine2))
                        {
                            lbusPersonAddress.icdoPersonAddress.addr_line_2 = astrAddressLine2;
                        }
                        if (!string.IsNullOrEmpty(astrCity))
                        {
                            lbusPersonAddress.icdoPersonAddress.addr_city = astrCity;
                        }
                        if (!string.IsNullOrEmpty(astrState))
                        {
                            if ((!string.IsNullOrEmpty(astrCountryCode)) && (astrCountryCode == "0001" || astrCountryCode == "0011" || astrCountryCode == "0036" ||
                               astrCountryCode == "0133" || astrCountryCode == "0147") && astrState.Length <= 4)
                            {
                                lbusPersonAddress.icdoPersonAddress.addr_state_value = astrState;
                            }
                            else
                            {
                                lbusPersonAddress.icdoPersonAddress.foreign_province = astrState;
                            }
                        }
                        if (!string.IsNullOrEmpty(astrZipCode))
                        {
                            lbusPersonAddress.icdoPersonAddress.addr_zip_code = astrZipCode;
                        }
                        if (!string.IsNullOrEmpty(astrZipCode4))
                        {
                            lbusPersonAddress.icdoPersonAddress.addr_zip_4_code = astrZipCode4;
                        }
                        if (!string.IsNullOrEmpty(astrCountryCode))
                        {
                            lbusPersonAddress.icdoPersonAddress.addr_country_value = astrCountryCode;
                        }
                        if (!string.IsNullOrEmpty(astrAddressEndDate))
                        {
                            lbusPersonAddress.icdoPersonAddress.end_date = Convert.ToDateTime(astrAddressEndDate);
                        }

                        lbusPersonAddress.icdoPersonAddress.modified_by = "NCOA";
                        lbusPersonAddress.icdoPersonAddress.Update();

                        foreach (cdoPersonAddressChklist icdoPersonAddressChklist in lbusPersonAddress.iclcPersonAddressChklist)
                        {
                            icdoPersonAddressChklist.Delete();
                        }

                        lbusPersonAddress.iclcPersonAddressChklist.Clear();



                        if (astrAddressType == "BOTH")
                        {
                            cdoPersonAddressChklist icdoPersonAddressChklistMailing = new cdoPersonAddressChklist();
                            icdoPersonAddressChklistMailing.address_id = lbusPersonAddress.icdoPersonAddress.address_id;
                            icdoPersonAddressChklistMailing.address_type_id = 6013;
                            icdoPersonAddressChklistMailing.address_type_value = "MAIL";
                            icdoPersonAddressChklistMailing.created_by = "NCOA";
                            icdoPersonAddressChklistMailing.modified_by = "NCOA";


                            lbusPersonAddress.iclcPersonAddressChklist.Add(icdoPersonAddressChklistMailing);

                            cdoPersonAddressChklist icdoPersonAddressChklistPhysical = new cdoPersonAddressChklist();
                            icdoPersonAddressChklistPhysical.address_id = lbusPersonAddress.icdoPersonAddress.address_id;
                            icdoPersonAddressChklistPhysical.address_type_id = 6013;
                            icdoPersonAddressChklistPhysical.address_type_value = "PYSL";
                            icdoPersonAddressChklistPhysical.created_by = "NCOA";
                            icdoPersonAddressChklistPhysical.modified_by = "NCOA";

                            lbusPersonAddress.iclcPersonAddressChklist.Add(icdoPersonAddressChklistPhysical);
                        }
                        else
                        {
                            cdoPersonAddressChklist icdoPersonAddressChklist = new cdoPersonAddressChklist();
                            icdoPersonAddressChklist.address_id = lbusPersonAddress.icdoPersonAddress.address_id;
                            icdoPersonAddressChklist.address_type_id = 6013;
                            icdoPersonAddressChklist.address_type_value = astrAddressType;
                            icdoPersonAddressChklist.created_by = "NCOA";
                            icdoPersonAddressChklist.modified_by = "NCOA";

                            lbusPersonAddress.iclcPersonAddressChklist.Add(icdoPersonAddressChklist);
                        }

                        foreach (cdoPersonAddressChklist icdoPersonAddressChklist in lbusPersonAddress.iclcPersonAddressChklist)
                        {
                            icdoPersonAddressChklist.Insert();
                        }
                    }
                }

            }
            else
            {
                busPersonAddress lbusPersonAddress = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() };

                lbusPersonAddress.iclcPersonAddressChklist = new utlCollection<cdoPersonAddressChklist>();

                lbusPersonAddress.icdoPersonAddress.person_id = aintPersonID;
                lbusPersonAddress.icdoPersonAddress.addr_line_1 = astrAddressLine1;
                lbusPersonAddress.icdoPersonAddress.addr_line_2 = astrAddressLine2;
                lbusPersonAddress.icdoPersonAddress.addr_city = astrCity;
                lbusPersonAddress.icdoPersonAddress.addr_state_id = 150;

                if ((!string.IsNullOrEmpty(astrCountryCode)) && (astrCountryCode == "0001" || astrCountryCode == "0011" || astrCountryCode == "0036" ||
                               astrCountryCode == "0133" || astrCountryCode == "0147") && astrState.Length <= 4)
                {
                    lbusPersonAddress.icdoPersonAddress.addr_state_value = astrState;
                }
                else
                {
                    lbusPersonAddress.icdoPersonAddress.foreign_province = astrState;
                }

                lbusPersonAddress.icdoPersonAddress.addr_zip_code = astrZipCode;
                lbusPersonAddress.icdoPersonAddress.addr_zip_4_code = astrZipCode4;
                lbusPersonAddress.icdoPersonAddress.addr_country_id = 151;
                lbusPersonAddress.icdoPersonAddress.addr_country_value = astrCountryCode;
                lbusPersonAddress.icdoPersonAddress.start_date = DateTime.Now;
                lbusPersonAddress.icdoPersonAddress.end_date = Convert.ToDateTime(astrAddressEndDate);
                lbusPersonAddress.icdoPersonAddress.created_by = "NCOA";
                lbusPersonAddress.icdoPersonAddress.modified_by = "NCOA";
                lbusPersonAddress.icdoPersonAddress.Insert();

                if (astrAddressType == "BOTH")
                {
                    cdoPersonAddressChklist icdoPersonAddressChklistMailing = new cdoPersonAddressChklist();
                    icdoPersonAddressChklistMailing.address_id = lbusPersonAddress.icdoPersonAddress.address_id;
                    icdoPersonAddressChklistMailing.address_type_id = 6013;
                    icdoPersonAddressChklistMailing.address_type_value = "MAIL";
                    icdoPersonAddressChklistMailing.created_by = "NCOA";
                    icdoPersonAddressChklistMailing.modified_by = "NCOA";


                    lbusPersonAddress.iclcPersonAddressChklist.Add(icdoPersonAddressChklistMailing);

                    cdoPersonAddressChklist icdoPersonAddressChklistPhysical = new cdoPersonAddressChklist();
                    icdoPersonAddressChklistPhysical.address_id = lbusPersonAddress.icdoPersonAddress.address_id;
                    icdoPersonAddressChklistPhysical.address_type_id = 6013;
                    icdoPersonAddressChklistPhysical.address_type_value = "PYSL";
                    icdoPersonAddressChklistPhysical.created_by = "NCOA";
                    icdoPersonAddressChklistPhysical.modified_by = "NCOA";

                    lbusPersonAddress.iclcPersonAddressChklist.Add(icdoPersonAddressChklistPhysical);
                }
                else
                {
                    cdoPersonAddressChklist icdoPersonAddressChklist = new cdoPersonAddressChklist();
                    icdoPersonAddressChklist.address_id = lbusPersonAddress.icdoPersonAddress.address_id;
                    icdoPersonAddressChklist.address_type_id = 6013;
                    icdoPersonAddressChklist.address_type_value = astrAddressType;
                    icdoPersonAddressChklist.created_by = "NCOA";
                    icdoPersonAddressChklist.modified_by = "NCOA";

                    lbusPersonAddress.iclcPersonAddressChklist.Add(icdoPersonAddressChklist);
                }

                foreach (cdoPersonAddressChklist icdoPersonAddressChklist in lbusPersonAddress.iclcPersonAddressChklist)
                {
                    icdoPersonAddressChklist.Insert();
                }

            }


        }

        public busPerson GetPersonInformationFromOPUSService(int aintPersonID)
        {
            busPerson lbusPerson = new busPerson();
            if (lbusPerson.FindPerson(aintPersonID))
            {
                lbusPerson.iclbPersonAddress = new Collection<busPersonAddress>();
                lbusPerson.iclbPersonContact = new Collection<busPersonContact>();

                DataTable ldtbPersonAddressList = busBase.Select<cdoPersonAddress>(
                     new string[1] { enmPersonAddress.person_id.ToString() },
                     new object[1] { aintPersonID }, null, enmPersonAddress.start_date.ToString());

                DataTable ldtbPersonContactList = busBase.Select<cdoPersonContact>(
                    new string[1] { enmPersonContact.person_id.ToString() },
                    new object[1] { aintPersonID }, null, enmPersonContact.effective_start_date.ToString());

                foreach (DataRow ldtRow in ldtbPersonAddressList.Rows)
                {
                    busPersonAddress lbusPersonAddress = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() };
                    lbusPersonAddress.icdoPersonAddress.LoadData(ldtRow);
                    lbusPerson.iclbPersonAddress.Add(lbusPersonAddress);
                }

                foreach (DataRow ldtRow in ldtbPersonContactList.Rows)
                {
                    busPersonContact lbusPersonContact = new busPersonContact { icdoPersonContact = new cdoPersonContact() };
                    lbusPersonContact.icdoPersonContact.LoadData(ldtRow);
                    lbusPerson.iclbPersonContact.Add(lbusPersonContact);
                }
            }

            return lbusPerson;
        }


        public busPersonBridgeHours FindPersonBridgeHours(int aintpersonbridgeid)
        {
            busPersonBridgeHours lobjPersonBridgeHours = new busPersonBridgeHours();
            if (lobjPersonBridgeHours.FindPersonBridgeHours(aintpersonbridgeid))
            {
                lobjPersonBridgeHours.LoadPersonBridgeHoursDetails();
            }

            return lobjPersonBridgeHours;
        }

        public busPersonBatchFlags FindPersonBatchFlags(int aintpersonbatchflagid)
        {
            busPersonBatchFlags lobjPersonBatchFlags = new busPersonBatchFlags();
            if (lobjPersonBatchFlags.FindPersonBatchFlags(aintpersonbatchflagid))
            {
            }

            return lobjPersonBatchFlags;
        }


        public busPerson FindPersonAccountRetirementContributionForEE(int aintPersonID)
        {
            busPerson lbusPerson = new busPerson();
            if (lbusPerson.FindPerson(aintPersonID))
            {
                lbusPerson.iclbPersonAccountRetirementContribution = null;
                lbusPerson.LoadRetirementContributionsForEE(aintPersonID);
            }
            return lbusPerson;
        }


        public ArrayList LoadParticipantDetails(int person_id, int beneficiary_person_id)
        {
            ArrayList larrBenAppDet = new ArrayList();
            DataTable ldtblist = busBase.Select("cdoBenefitApplication.GetDetailsOfParticipant", new object[2] { person_id, beneficiary_person_id });
            if (ldtblist.Rows.Count > 0)
            {

                larrBenAppDet.Add(ldtblist.Rows[0][0].ToString());
                larrBenAppDet.Add(ldtblist.Rows[0][2].ToString());
                larrBenAppDet.Add(ldtblist.Rows[0][3].ToString());
                string lstr = Convert.ToString(ldtblist.Rows[0][1]);

                larrBenAppDet.Add(lstr);
            }
            return larrBenAppDet;
        }

        public DateTime GetDecryptedDOB(string astrEncrypted)
        {
            DateTime ldtDOB = new DateTime();
            ldtDOB = Convert.ToDateTime(astrEncrypted);
            return ldtDOB;
        }

        public string GetMPIDFromSSN(string astrSSN)
        {
            string lstrMPID = string.Empty;
            string lstrEncryptedSSN = astrSSN;
            DataTable ldtbMPID = busBase.Select("cdoPerson.GetMPIDFromSSN", new object[1] { lstrEncryptedSSN });
            if (ldtbMPID.Rows.Count > 0)
            {
                lstrMPID = Convert.ToString(ldtbMPID.Rows[0][0]);
            }
            else
            {
                lstrMPID = "SSN Does Not Exists";
            }
            return lstrMPID;
        }

        public bool CheckAlreadyVested(busBenefitApplication abusBenefitApplication, string astrPlanCode)
        {
            abusBenefitApplication.ibusTempPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };
            int lintAccountId = 0;
            if (abusBenefitApplication.ibusPerson.iclbPersonAccount != null)
            {
                if (abusBenefitApplication.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == astrPlanCode).Count() > 0)
                {
                    lintAccountId = abusBenefitApplication.ibusPerson.iclbPersonAccount.Where(plan => plan.icdoPersonAccount.istrPlanCode == astrPlanCode).First().icdoPersonAccount.person_account_id;
                }
            }

            if (lintAccountId > 0)
            {
                DataTable ldtbPersonAccountEligibility = busBase.Select("cdoPersonAccountEligibility.GetEligibilityInfoFromAccountID", new object[1] { lintAccountId });
                if (ldtbPersonAccountEligibility.Rows.Count > 0)
                {
                    abusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.LoadData(ldtbPersonAccountEligibility.Rows[0]);
                    if (abusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date.IsNotNull() && abusBenefitApplication.ibusTempPersonAccountEligibility.icdoPersonAccountEligibility.vested_date != DateTime.MinValue)
                    {
                        return true;
                    }
                }

            }
            return false;
        }

        public busPersonSuspendibleMonth NewPersonSuspendibleMonth(int aintPersonId)
        {
            busPersonSuspendibleMonth lbusPersonSuspendibleMonth = new busPersonSuspendibleMonth();
            lbusPersonSuspendibleMonth.icdoPersonSuspendibleMonth = new cdoPersonSuspendibleMonth();
            lbusPersonSuspendibleMonth.icdoPersonSuspendibleMonth.person_id = aintPersonId;

            lbusPersonSuspendibleMonth.ibusPerson = new busPerson();
            lbusPersonSuspendibleMonth.ibusPerson.FindPerson(aintPersonId);

            lbusPersonSuspendibleMonth.icdoPersonSuspendibleMonth.status_id = busConstant.SUSPENDIBLE_MONTH_STATUS_ID;
            lbusPersonSuspendibleMonth.icdoPersonSuspendibleMonth.status_value = busConstant.SUSPENDIBLE_MONTH_STATUS_PENDING;
            lbusPersonSuspendibleMonth.icdoPersonSuspendibleMonth.status_description = busConstant.SUSPENDIBLE_MONTH_STATUS_PENDING_DESC;

            return lbusPersonSuspendibleMonth;
        }

        public busPersonSuspendibleMonth FindPersonSuspendibleMonth(int aintPersonSuspendibleMonthID)
        {
            busPersonSuspendibleMonth lbusPersonSuspendibleMonth = new busPersonSuspendibleMonth();
            if (lbusPersonSuspendibleMonth.FindPersonSuspendibleMonth(aintPersonSuspendibleMonthID))
            {
                lbusPersonSuspendibleMonth.ibusPerson = new busPerson();
                if (lbusPersonSuspendibleMonth.icdoPersonSuspendibleMonth.person_id != 0)
                {
                    lbusPersonSuspendibleMonth.ibusPerson.FindPerson(lbusPersonSuspendibleMonth.icdoPersonSuspendibleMonth.person_id);
                }
            }
            return lbusPersonSuspendibleMonth;
        }

        public busHealthEligibiltyActuaryData FindHealthEligibiltyActuaryData(int ainthealthactuarydataid)
        {
            busHealthEligibiltyActuaryData lobjHealthEligibiltyActuaryData = new busHealthEligibiltyActuaryData();
            if (lobjHealthEligibiltyActuaryData.FindHealthEligibiltyActuaryData(ainthealthactuarydataid))
            {
            }

            return lobjHealthEligibiltyActuaryData;
        }

        public busDataExtractionBatchInfo FindDataExtractionBatchInfo(int aintdataextractionbatchinfoid)
        {
            busDataExtractionBatchInfo lobjDataExtractionBatchInfo = new busDataExtractionBatchInfo();
            if (lobjDataExtractionBatchInfo.FindDataExtractionBatchInfo(aintdataextractionbatchinfoid))
            {
            }

            return lobjDataExtractionBatchInfo;
        }

        public busDataExtractionBatchHourInfo FindDataExtractionBatchHourInfo(int aintdataextractionbatchhourinfoid)
        {
            busDataExtractionBatchHourInfo lobjDataExtractionBatchHourInfo = new busDataExtractionBatchHourInfo();
            if (lobjDataExtractionBatchHourInfo.FindDataExtractionBatchHourInfo(aintdataextractionbatchhourinfoid))
            {
            }

            return lobjDataExtractionBatchHourInfo;
        }

        public busPensionActuaryLookup LoadPensionActuarys(DataTable adtbSearchResult)
        {
            busPensionActuaryLookup lobjPensionActuaryLookup = new busPensionActuaryLookup();
            lobjPensionActuaryLookup.LoadDataExtractionBatchInfos(adtbSearchResult);
            return lobjPensionActuaryLookup;
        }

        public busYearEndProcessRequest NewbusYearEndProcessRequest(string astrYearEndProcessValue)
        {
            busYearEndProcessRequest lbusYearEndProcessRequest = new busYearEndProcessRequest();
            lbusYearEndProcessRequest.icdoYearEndProcessRequest = new cdoYearEndProcessRequest();

            lbusYearEndProcessRequest.icdoYearEndProcessRequest.year_end_process_id = busConstant.YEAR_END_PROCESS_NAME_ID;
            lbusYearEndProcessRequest.icdoYearEndProcessRequest.year_end_process_value = astrYearEndProcessValue;
            lbusYearEndProcessRequest.EvaluateInitialLoadRules(utlPageMode.New);
            return lbusYearEndProcessRequest;
        }

        public busYearEndProcessRequest FindYearEndProcessRequest(int aintyearendprocessrequestid)
        {
            busYearEndProcessRequest lobjYearEndProcessRequest = new busYearEndProcessRequest();
            if (lobjYearEndProcessRequest.FindYearEndProcessRequest(aintyearendprocessrequestid))
            {
                lobjYearEndProcessRequest.LoadPayment1099rs();
            }

            return lobjYearEndProcessRequest;
        }

        public busPayment1099r FindPayment1099r(int aintpayment1099rid)
        {
            busPayment1099r lobjPayment1099r = new busPayment1099r();
            if (lobjPayment1099r.FindPayment1099r(aintpayment1099rid))
            {
                lobjPayment1099r.LoadPayment1099rHistoryLinks();
            }

            return lobjPayment1099r;
        }

        public busPayment1099rHistoryLink FindPayment1099rHistoryLink(int aintpayment1099rhistorylinkid)
        {
            busPayment1099rHistoryLink lobjPayment1099rHistoryLink = new busPayment1099rHistoryLink();
            if (lobjPayment1099rHistoryLink.FindPayment1099rHistoryLink(aintpayment1099rhistorylinkid))
            {
            }

            return lobjPayment1099rHistoryLink;
        }

        public busYearEndProcessRequestLookup LoadYearEndProcessRequests(DataTable adtbSearchResult)
        {
            busYearEndProcessRequestLookup lobjYearEndProcessRequestLookup = new busYearEndProcessRequestLookup();
            lobjYearEndProcessRequestLookup.LoadYearEndProcessRequests(adtbSearchResult);
            return lobjYearEndProcessRequestLookup;
        }

        public busYearEndDataExtractionHeader FindYearEndDataExtractionHeader(int aintYearEndDataExtractionHeaderId)
        {
            busYearEndDataExtractionHeader lobjYearEndDataExtractionHeader = new busYearEndDataExtractionHeader();
            if (lobjYearEndDataExtractionHeader.FindYearEndDataExtractionHeader(aintYearEndDataExtractionHeaderId))
            {
            }

            return lobjYearEndDataExtractionHeader;
        }

        public busSSNMerge FindSSNMergeData(string astrMPIID)
        {
            DateTime ldtMinVal = new DateTime(1753, 01, 01);
            busSSNMerge lobjPerson = new busSSNMerge();
            if (lobjPerson.FindPerson(astrMPIID))
            {
                lobjPerson.ibusPersonAddress = new busPersonAddress();
                lobjPerson.ibusPersonAddress.LoadActiveAddress(lobjPerson.icdoPerson.person_id);
                if (lobjPerson.icdoPerson.date_of_birth == ldtMinVal)
                    lobjPerson.icdoPerson.date_of_birth = DateTime.MinValue;

                if (lobjPerson.icdoPerson.date_of_birth != DateTime.MinValue && lobjPerson.icdoPerson.first_name.IsNotNullOrEmpty() && lobjPerson.icdoPerson.last_name.IsNotNullOrEmpty()
                    && lobjPerson.icdoPerson.ssn.IsNotNullOrEmpty())
                     lobjPerson.GetPayeeAccounts(lobjPerson.icdoPerson.person_id);

                lobjPerson.istrNewMergedMPIID = astrMPIID;

                lobjPerson.istrPersonAddress = lobjPerson.GetMailingAddress(lobjPerson.icdoPerson.person_id);

                string lstrSSN = lobjPerson.icdoPerson.istrSSNNonEncrypted;
                if (!string.IsNullOrEmpty(lstrSSN))
                {
                    string lstrEmployerName = string.Empty;
                    int lintUnionCode = 0;
                    lobjPerson.GetEmployerNameBySSN(lstrSSN, ref lstrEmployerName, ref lintUnionCode);

                    lobjPerson.icdoPerson.istrEmployerName = lstrEmployerName;
                    lobjPerson.icdoPerson.UnionCode = Convert.ToString(lintUnionCode);
                }

                if(lobjPerson.icdoPerson.date_of_birth!=DateTime.MinValue && lobjPerson.icdoPerson.first_name.IsNotNullOrEmpty() && lobjPerson.icdoPerson.last_name.IsNotNullOrEmpty()
                    && lobjPerson.icdoPerson.ssn.IsNotNullOrEmpty())
                     lobjPerson.LoadPossibleSSNRecords(lobjPerson);
                     lobjPerson.GetLatestPayeeAccountsForPossibleDuplicates(lobjPerson);

                lobjPerson.LoadInitialData();
                if (lobjPerson.iblnBeneficiary == busConstant.YES)
                {
                    lobjPerson.icdoPerson.istrPersonType = busGlobalFunctions.GetCodeValueDescriptionByValue(1509, busConstant.PERSON_TYPE_SURVIVOR).description;
                }
                if (lobjPerson.iblnAlternatePayee == busConstant.YES)
                {
                    lobjPerson.icdoPerson.istrPersonType = busGlobalFunctions.GetCodeValueDescriptionByValue(1509, busConstant.PERSON_TYPE_ALTERNATE_PAYEE).description;
                }
                if (lobjPerson.iblnParticipant == busConstant.YES)
                {
                    lobjPerson.icdoPerson.istrPersonType = busGlobalFunctions.GetCodeValueDescriptionByValue(1509, busConstant.PERSON_TYPE_PARTICIPANT).description;
                }
            }

            return lobjPerson;
        }

        public busPersonBase FindPersonBase(int aintPersonId)
        {
            busPersonBase lobjPersonBase = new busPersonBase();
            if (lobjPersonBase.FindPersonBase(aintPersonId))
            {
                lobjPersonBase.LoadInitialData();
                lobjPersonBase.GetCurrentAge();
                lobjPersonBase.LoadPersonAddresss();
                lobjPersonBase.LoadBeneficiaries();
                lobjPersonBase.LoadPersonContacts();
                lobjPersonBase.LoadPersonDependents();
                lobjPersonBase.LoadPersonBridgedService();
                lobjPersonBase.LoadCorrAddress();
                lobjPersonBase.LoadParticipantPlan();
                lobjPersonBase.LoadPersonSuspendibleMonth();

                lobjPersonBase.iclbNotes = busGlobalFunctions.LoadNotes(lobjPersonBase.icdoPersonBase.person_id, 0, busConstant.PERSON_MAINTAINENCE_FORM);
                lobjPersonBase.LoadRetirementContributionsbyPersonId(lobjPersonBase.icdoPersonBase.person_id);
            }

            return lobjPersonBase;
        }

        public busSSNMerge FindSSNMergeDemoGraphicInfo(string astrOldMpiId, string astrNewMpiId)
        {
            busSSNMerge lobjPerson = new busSSNMerge();
            if (lobjPerson.FindPerson(astrOldMpiId))
            {
                lobjPerson.ibusDuplicateRecord = new busPerson();
                lobjPerson.ibusDuplicateRecord.FindPerson(astrNewMpiId);

                lobjPerson.ibusDuplicateRecord.LoadTotalEEUVHPAndIAPContributions(lobjPerson.ibusDuplicateRecord.icdoPerson.person_id);

                lobjPerson.iclbNewPerson = new Collection<busPerson>();
                lobjPerson.ibusDuplicateRecord.ibusPersonAddress = new busPersonAddress();
                lobjPerson.ibusDuplicateRecord.ibusPersonAddress.LoadActiveAddress(lobjPerson.ibusDuplicateRecord.icdoPerson.person_id);

                if (lobjPerson.ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress != null && 
                    lobjPerson.ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.address_id != 0 &&
                    lobjPerson.ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_state_value != null)
                {
                    lobjPerson.ibusDuplicateRecord.istrPersonAddress = lobjPerson.ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_line_1 + " " +
                                                   lobjPerson.ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_line_2 + " " +
                                                   busGlobalFunctions.GetCodeValueDescriptionByValue(lobjPerson.ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_state_id,
                                                   lobjPerson.ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_state_value).description + " " +
                                                   lobjPerson.ibusDuplicateRecord.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_zip_code;

                }
                lobjPerson.iclbNewPerson.Add(lobjPerson.ibusDuplicateRecord);

                lobjPerson.ibusPersonAddress = new busPersonAddress();
                lobjPerson.ibusPersonAddress.LoadActiveAddress(lobjPerson.icdoPerson.person_id);
                lobjPerson.iclbOldPersonToBeMerged = new Collection<busPerson>();

                lobjPerson.LoadTotalEEUVHPAndIAPContributions(lobjPerson.icdoPerson.person_id);

                if (lobjPerson.ibusPersonAddress.ibusMainParticipantAddress != null && 
                    lobjPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.address_id != 0 &&
                    lobjPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_state_value != null)
                {
                    lobjPerson.istrPersonAddress = lobjPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_line_1 + " " +
                                                   lobjPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_line_2 + " " +
                                                   busGlobalFunctions.GetCodeValueDescriptionByValue(lobjPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_state_id,
                                                   lobjPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_state_value).description + " " +
                                                    lobjPerson.ibusPersonAddress.ibusMainParticipantAddress.icdoPersonAddress.addr_zip_code;

                }


                lobjPerson.iclbOldPersonToBeMerged.Add(lobjPerson);

                lobjPerson.iclbMergedPersonReord = new Collection<busPerson>();

                foreach (busPerson lbusPerson in lobjPerson.iclbNewPerson)
                {
                    lbusPerson.IsToSSNChecked = "Y"; lbusPerson.IsToMiddleNameChecked = "Y";
                    lbusPerson.IsToLastNameChecked = "Y"; lbusPerson.IsToFirstNameChecked = "Y";
                    lbusPerson.IsToPrefixNameChecked = "Y"; lbusPerson.IsToSuffixNameChecked = "Y";
                    lbusPerson.IsToDOBChecked = "Y"; lbusPerson.IsToDODChecked = "Y";
                    lbusPerson.IsToAddressChecked = "Y";
                }
            }

            return lobjPerson;
        }

        public busPersonBaseLookup LoadPersonBases(DataTable adtbSearchResult)
        {
            busPersonBaseLookup lobjPersonBaseLookup = new busPersonBaseLookup();
            lobjPersonBaseLookup.LoadPersonBases(adtbSearchResult);
            return lobjPersonBaseLookup;
        }
        public busReturnedMail NewReturnedMail()
        {
            SetWebParameters();
            busReturnedMail lbusReturnedMail = new busReturnedMail { icdoReturnedMail = new cdoReturnedMail() };
            if (iobjPassInfo.istrFormName == "wfmReturnMailWizard"  && istrVIPAccess.IsNotNullOrEmpty() && istrVIPAccess == "VIPAccessUser")
            {
                lbusReturnedMail.astrVipAcc = "Y";
            }
            //lbusReturnedMail.ibusPerson = new busPerson() { icdoPerson=new cdoPerson()};
            //lbusReturnedMail.ibusLookupPerson = new busPerson() { icdoPerson = new cdoPerson() };
            //lbusReturnedMail.ibusLookupPerson.ibusPersonAddress = new busPersonAddress() { icdoPersonAddress = new cdoPersonAddress() };
            //lbusReturnedMail.iclbPerson = new Collection<busPerson>();
            return lbusReturnedMail;
        }
        public busReturnedMail FindReturnedMail(int aintReturnedMailId)
        {
            busReturnedMail lbusReturnedMail = new busReturnedMail();
            if (lbusReturnedMail.FindReturnedMail(aintReturnedMailId))
            {
                //lbusReturnedMail.LoadAllPersonAddresss();
                lbusReturnedMail.LoadPersonAddress();
                if (lbusReturnedMail.ibusPersonAddress != null)
                {
                    lbusReturnedMail.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                    lbusReturnedMail.ibusPerson.FindPerson(lbusReturnedMail.ibusPersonAddress.icdoPersonAddress.person_id);
                    lbusReturnedMail.LoadDoucument();
                }
                
            }

            return lbusReturnedMail;
        }
        public busPersonLookup LoadPersonsReturnMail(DataTable adtbSearchResult)
        {
            busPersonLookup lobjPersonLookup = new busPersonLookup();
            lobjPersonLookup.LoadPersons(adtbSearchResult);
            return lobjPersonLookup;
        }
        public busPerson FindPersonReturnMail(int aintpersonid)
        {
            busPerson lobjPerson = new busPerson();
            if (lobjPerson.FindPerson(aintpersonid))
            {
                lobjPerson.LoadReturnedMail();
               
            }
            return lobjPerson;
        }
        public busPersonBeneficiary GetBusObjectFromDB(string astrFormName, int aintPrimaryKey)
        {
            busPersonBeneficiary lobjMain = (busPersonBeneficiary)busMainBase.GetObjectFromDB(astrFormName, aintPrimaryKey);
            return lobjMain;
        }
    }
}
